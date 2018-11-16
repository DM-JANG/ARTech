using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ProtocolTest
{
    class TCPIP
    {
        int rows = 0;
        string IP;
        int Port;
        string PreIP = "";
        int PrePort = 0;
        private static int _timeout = 3000;
        private static bool _connected = false;
        public TcpClient tcpSynClient;
        public const byte fctReadHoldingRegister = 3;
        public List<byte[]> Data;
        DBConnect DbCon = new DBConnect();
        Function Fun = new Function();
        List<string> datast = new List<string>();
        int SeqNo = 0;



        public TCPIP(string IP, int Port, TcpClient tcpSynClient)
        {
            this.IP = IP;
            this.Port = Port;
            this.tcpSynClient = tcpSynClient;
            _connected = true;

        }
        public bool connected
        {
            get { return _connected; }
        }
        // Create modbus header for read action 




        private byte[] CreateReadHeader(int Prepos, int Preid) // 데이터 입력
        {
            string datastr = string.Empty;
            string DBName = DbCon.TextInputData();

            DataTable tbl = DbCon.dbConn("select * from alarm_tab where ID > " + Preid + " order by ID asc  ", DBName);
            // tbl 배열의 9번은 Pos 값 6번은 Date값
            int num = 0;

            string[] Startend = Fun.Split(@"..\..\ZoneData.txt");
            bool[] Positionck = new bool[Startend.Length / 2]; // Root DB안에 같은 Position 값이 들어가 있기 때문에 판별용도
            for (int j = 0; j < Positionck.Length; j++)
                Positionck[j] = true;

            int startint = 0;
            int endint = 1;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {

                if (Convert.ToInt32(tbl.Rows[i].ItemArray[0]) != Preid)
                {
                    int Position = Convert.ToInt32(tbl.Rows[i].ItemArray[9]);

                    for (int j = 0; j < Positionck.Length; j++)
                    {
                        Fun.insertTrainDataTCP(tbl, Position, ref Positionck[j], Preid, ref datastr, j, ref num, Convert.ToInt32(Startend[startint]), Convert.ToInt32(Startend[endint]));
                        startint += 2;
                        endint += 2;
                    }
                }
                startint = 0;
                endint = 1;
            }
            if (datastr != "")
            {
                datastr = datastr.Remove(datastr.Length - 1, 1);
                int nums = datastr.Split('/').Count();
                string[] Data = datastr.Split('/');
                for (int i = 0; i < nums; i++)
                {
                    if (datast != null)
                        if (datast.Count > 64)
                            datast.RemoveRange(0, 4);
                    datast.Add(Data[i].ToString());
                }

                int cou = 0;
                cou += datast.Count / 4;
                if (cou > 16)
                    cou = 16;
                byte[] datas = new byte[42 + (cou * 23)];

                Fun.stringToByte(datas, "PRT3", 0); //Preamble (0)
                Fun.intToByte(datas, 34 + (cou * 23), 4, 4); //Length (1)

                Fun.intToByte(datas, SeqNo, 8, 2); //SeqNo (2)

                byte[] Opcode = { 0X08, 0X00, 0x00, 0x05 };

                Fun.ToByte(datas, Opcode, 10, 4); //OpCode (3)            
                byteinput(datas, tbl, 14, cou, datast); //(4) Data
                // Reserved 데이터 (5)
                byte[] Reserved = new byte[2];
                Fun.ToByte(datas, Reserved, 39 + (cou * 23), 2); // Data 의 갯수(N)만큼 23바이트의 데이터가 추가된다.

                // Checksum 데이터 (6)
                int Checksum = Fun.CalculateChecksum(datas);
                Fun.intToByte(datas, Checksum, 41 + (cou * 23), 1); // Data 의 갯수(N)만큼 23바이트의 데이터가 추가된다.

                return datas;
            }

            return null;
        }

        public void byteinput(byte[] bytedata, DataTable data, int start, int count, List<string> datast)
        {
            int j = 0;

            Fun.stringToByte(bytedata, "DAS", start); start += 8; //ScrDevName
            DateTime dd = DateTime.Now;
            Fun.intToByte(bytedata, Convert.ToInt32(string.Format("{0:yy}", dd)), start, 1); start++; //년
            Fun.intToByte(bytedata, Convert.ToInt32(string.Format("{0:MM}", dd)), start, 1); start++; //월
            Fun.intToByte(bytedata, Convert.ToInt32(string.Format("{0:dd}", dd)), start, 1); start++; //일
            Fun.intToByte(bytedata, Convert.ToInt32(string.Format("{0:HH}", dd)), start, 1); start++; //시
            Fun.intToByte(bytedata, Convert.ToInt32(string.Format("{0:mm}", dd)), start, 1); start++; //분
            Fun.intToByte(bytedata, Convert.ToInt32(string.Format("{0:ss}", dd)), start, 1); start++; //초
            Fun.intToByte(bytedata, Convert.ToInt32(0), start, 2); start += 2; //Reserved
            Fun.stringToByte(bytedata, "TNMS", start); start += 8;
            Fun.intToByte(bytedata, count, start, 1); start += 1; //Reserved


            for (int i = 0; i < count; i++)
            {

                //열차 상세정보
                Fun.intToByte(bytedata, Convert.ToInt32(datast[j]), start, 4); start += 4; //열차 지점 ID

                string pos = string.Empty; // 위치 정보
                switch (Convert.ToInt32(datast[j]))
                {
                    case 0:
                        pos = "0";
                        break;
                    case 1:
                        pos = "390";
                        break;
                    case 2:
                        pos = "567";
                        break;
                    case 3:
                        pos = "749";
                        break;
                    case 4:
                        pos = "1464";
                        break;
                    case 5:
                        pos = "1478";
                        break;
                    case 6:
                        pos = "1488";
                        break;
                    case 7:
                        pos = "1619";
                        break;
                    case 8:
                        pos = "1698";
                        break;
                    case 9:
                        pos = "1740";
                        break;
                    case 10:
                        pos = "2442";
                        break;
                    case 11:
                        pos = "2539";
                        break;
                    case 12:
                        pos = "2602";
                        break;
                    case 13:
                        pos = "2652";
                        break;
                    case 14:
                        pos = "2703";
                        break;
                    case 15:
                        pos = "2831";
                        break;
                    case 16:
                        pos = "3182";
                        break;
                    case 17:
                        pos = "3419";
                        break;
                    case 18:
                        pos = "3755";
                        break;
                    case 19:
                        pos = "6047";
                        break;
                    case 20:
                        pos = "6117";
                        break;
                    case 21:
                        pos = "6693";
                        break;
                    case 22:
                        pos = "7993";
                        break;
                    case 23:
                        pos = "8370";
                        break;
                    case 24:
                        pos = "8433";
                        break;
                    case 25:
                        pos = "9281";
                        break;
                    case 26:
                        pos = "9546";
                        break;
                    case 27:
                        pos = "9968";
                        break;
                    case 28:
                        pos = "10350";
                        break;


                }
                Fun.intToByte(bytedata, Convert.ToInt32(pos), start, 2); start += 2; //열차 지점 위치정보
                string date = datast[j + 2].ToString();
                Fun.intToByte(bytedata, Convert.ToInt32(date.Substring(2, 2)), start, 1); start++; //열차검지 시간(최종) 년
                Fun.intToByte(bytedata, Convert.ToInt32(date.Substring(5, 2)), start, 1); start++; //열차검지 시간(최종) 월
                Fun.intToByte(bytedata, Convert.ToInt32(date.Substring(8, 2)), start, 1); start++; //열차검지 시간(최종) 일
                if (date.Substring(15, 1) == ":")
                {
                    Fun.intToByte(bytedata, Convert.ToInt32(date.Substring(14, 1)), start, 1); start++; //열차검지 시간(최종) 시
                    Fun.intToByte(bytedata, Convert.ToInt32(date.Substring(16, 2)), start, 1); start++; //열차검지 시간(최종) 분
                    Fun.intToByte(bytedata, Convert.ToInt32(date.Substring(19, 2)), start, 1); start++; //열차검지 시간(최종) 초
                    Fun.intToByte(bytedata, 0, start, 2); start += 2; //열차검지 시간(최종) 마이크로초
                }
                else
                {
                    Fun.intToByte(bytedata, Convert.ToInt32(date.Substring(14, 2)), start, 1); start++; //열차검지 시간(최종) 시           
                    Fun.intToByte(bytedata, Convert.ToInt32(date.Substring(17, 2)), start, 1); start++; //열차검지 시간(최종) 분
                    Fun.intToByte(bytedata, Convert.ToInt32(date.Substring(20, 2)), start, 1); start++; //열차검지 시간(최종) 초
                    Fun.intToByte(bytedata, 0, start, 2); start += 2; //열차검지 시간(최종) 마이크로초
                }
                Fun.intToByte(bytedata, Convert.ToInt32(datast[j + 1]), start, 1); start++; // 상 하행
                if (datast[j + 3].Trim() == "KTX")
                {
                    Fun.intToByte(bytedata, 0, start, 1); start++; // 열차종류
                }
                else // Other
                {
                    Fun.intToByte(bytedata, 1, start, 1); start++; // 열차종류
                }
                Fun.intToByte(bytedata, 0, start, 4); start += 4; // 운행번호
                Fun.intToByte(bytedata, 0, start, 1); start++; // 진입진출
                Fun.intToByte(bytedata, 0, start, 1); start++; // 속도
                Fun.intToByte(bytedata, 0, start, 1); start++; // 예약

                j = j + 4;
            }
        }



        public bool ReadHoldingRegister(ref byte[] values, int Prepos, int Preid, ref List<byte[]> Datas)
        {
            try
            {
                _connected = true;
                return WriteSyncData(CreateReadHeader(Prepos, Preid));
            }
            catch (Exception e)
            {
                string m = e.Message;
                SeqNo = 0;
                _connected = false;
                // tcpSynClient.Close();
                tcpSynClient = null;
                return false;
            }

        }

        private bool WriteSyncData(byte[] write_data)
        {
            if (write_data != null)
            {


                if (WriteData(tcpSynClient, write_data, true, _connected, _timeout))
                {

                    return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public void connection(string IP, int Port)
        {
            try
            {
                if (tcpSynClient == null || !_connected)
                {

                    tcpSynClient = new TcpClient();
                    tcpSynClient.Connect(IP, Port);
                    //tsk.Wait(5000);
                    tcpSynClient.ReceiveBufferSize = 256;
                    tcpSynClient.SendBufferSize = 256;
                    LingerOption lingerOption = new LingerOption(true, 1);
                    tcpSynClient.LingerState = lingerOption;
                    PreIP = IP;
                    PrePort = Port;
                    _connected = true;
                }
            }
            catch (System.IO.IOException error)
            {
                _connected = false;
            }
            catch (Exception ex)
            {
                _connected = false;
            }
        }
        internal delegate void ExceptionData(int id, byte function, byte exception);
        public bool WriteData(TcpClient _tcpClient, byte[] write_data, bool _sync, bool _connected, int _timeout)
        {

            byte[] buffer = new byte[256];

            if (_connected)
            {

                _tcpClient.GetStream().BeginWrite(write_data, 0, write_data.Length, new AsyncCallback(SendCallback), _tcpClient);
                byte[] ChEvent = new byte[41];
                _tcpClient.GetStream().Read(ChEvent, 0, ChEvent.Length);
                string Pre = System.Text.Encoding.ASCII.GetString(Fun.ByteString(ChEvent, 0, 4)); // Preamble (0)
                string Src = System.Text.Encoding.ASCII.GetString(Fun.ByteString(ChEvent, 14, 4));
                string Dst = System.Text.Encoding.ASCII.GetString(Fun.ByteString(ChEvent, 30, 3));
                if (Fun.Byteint(ChEvent, 8, 2) + 1 > 32767)
                    SeqNo = 1;
                else
                    SeqNo = Fun.Byteint(ChEvent, 8, 2) + 1;
                if (Pre == "PRT3" && Src == "TNMS" && Dst == "DAS")
                {
                    write_data = null;
                    datast.Clear();
                }

                if (write_data == null)
                    return true;
                else
                    return false;

            }



            return false;
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                TcpClient client = (TcpClient)ar.AsyncState;

                // Complete sending the data to the remote device.  
                client.GetStream().EndWrite(ar);

                // Signal that all bytes have been sent.  

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


    }




}
