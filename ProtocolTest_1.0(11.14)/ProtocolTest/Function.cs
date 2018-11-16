using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ProtocolTest
{
    class Function
    {
        string DBName = string.Empty;
        DBConnect DbCon = new DBConnect();
        DBConnect DbCon2 = new DBConnect();
        string[] stratData = new string[2];
        bool startck = false;
        bool endck = false;
        bool Middck = false;
        int Ss = 0;
        int Ls = 0;
        int Ms = 0;
        int Ss2 = 0;
        int Ls2 = 0;
        int Ms2 = 0;
        string Sstart = string.Empty;
        string Send = string.Empty;
        string MStart = string.Empty;
        string Mend = string.Empty;
        string Lstart = string.Empty;
        string Lend = string.Empty;
        int count = 0;
        int count1 = 0;
        int count2 = 0;
        System.Timers.Timer timer = new System.Timers.Timer();
        //System.Timers.Timer timer2 = new System.Timers.Timer();
        int other = 0;
        int ktx = 0;
        public Function()
        {
            DBName = DbCon.TextInputData();
            DbCon2.TextInputData2();
            timer.Interval = 10000;
            timer.Elapsed += new ElapsedEventHandler(PositionClear);
            timer.Start();
        }
        public void stringToByte(byte[] bytedata, string data, int start)
        {
            byte[] datas; // 스트링 문자열의 데이터를  저장해둘 byte 배열
            string re = string.Empty; // 해당 문자열을 16진수로 변환한 문자열
            datas = Encoding.Default.GetBytes(data); // 해당 문자열을 byte 배열로 변환

            foreach (byte bytestr in datas)
            {
                re += string.Format("{0:X2}", bytestr);
            }
            for (int i = 0; i < re.Length / 2; i++)
            {
                bytedata[start + i] = Convert.ToByte(re.Substring(i * 2, 2), 16);
            }
        }

        public byte[] ByteString(byte[] data, int start, int num)
        {
            int j = 0;
            byte[] data2 = new byte[num];
            for (int i = start; i < start + num; i++)
            {
                data2[j] = data[i];
                j++;
            }
            return data2;
        }

        public int Byteint(byte[] data, int start, int num)
        {
            int data2 = 0;
            for (int i = start; i < start + num; i++)
                data2 += data[i];
            return data2;
        }

        public void intToByte(byte[] bytedata, int data, int start, int bytenum)
        {
            string hex = Convert.ToString(data, 16);
            int count = bytenum;
            int num = 0;
            if (hex.Length % 2 != 0)
            {
                hex = "0" + hex;
            }
            for (int i = start; i < start + bytenum; i++)
            {
                if (count <= hex.Length / 2)
                {
                    bytedata[i] = Convert.ToByte(hex.Substring(num * 2, 2), 16);
                    num++;
                }
                count--;
            }
        }

        public void ToByte( byte[] bytedata, byte[] data, int start, int bytenum)
        {
            int j = 0;

            //if (bytenum == 4)
            //    data[3] = 2;

            for (int i = start; i < start + bytenum; i++)
            {
                bytedata[i] = data[j];
                j++;
            }
        }

        public int CalculateChecksum2(byte[] dataToCalculate)
        {
            dataToCalculate[dataToCalculate.Length - 1] = new byte();
            int checksum = 0;
            foreach (byte chData in dataToCalculate)
            {
                checksum += chData;
            }
            return checksum;
        }


        public string[] Split(string txt)
        {
            string[] test = System.IO.File.ReadAllLines(txt);
            string[] Startend = new string[test.Length * 2];
            for (int i = 0; i < Startend.Length;)
            {
                string[] splitData = test[i / 2].Split('/');
                Startend[i] = splitData[0];
                Startend[i + 1] = splitData[1];
                i = i + 2;
            }
            return Startend;
        }

        public byte[] defualtData(Define TrainData)
        {
            byte[] data = new byte[65];
            TrainData.Preamble = "PRT3";
            stringToByte(data, TrainData.Preamble, 0); //Preamble (0)
            intToByte(data, 57, 4, 4); //Length (1)

            intToByte(data, TrainData.SeqNo, 8, 2); // SeqNo (2)
            ToByte(data, TrainData.OpCode, 10, 4); //OpCode (3)
            TrainDatainput(ref TrainData, 1, null); // 열차 Data 넣는곳 ( 메인 Data, 넣을 열차 Data 수 ) (4)
            data = byteinput(data, TrainData, 14, 1);
            // Reserved 데이터 (5)
            TrainData.Reserved = new byte[2];
            ToByte(data, TrainData.Reserved, 62, 2); // Data 의 갯수(N)만큼 37바이트의 데이터가 추가된다.

            // Checksum 데이터 (6)

            int checksum = CalculateChecksum(data); // 체크넘버
            intToByte(data, checksum, 64, 1); // Data 의 갯수(N)만큼 37바이트의 데이터가 추가된다.
            return data;
        }

        public void insertTrainDataTCP(DataTable tbl, int Position, ref bool Positionck, int PreID, ref string datastr, int PositionID, ref int num, int start, int end)
        {
            if (start < Position && Position < end && Positionck)
            {
                DataTable data = DbCon.dbConn("  SELECT DISTINCT Top(2) POSITION ,ALARM_DATETIME_UTC,TAG ,ID FROM alarm_tab where id > " + PreID + " and POSITION > " + start + "and POSITION < " + end + " order by ALARM_DATETIME_UTC asc ", DBName);
                if (data != null)
                    if (data.Rows.Count == 1)//Convert.ToInt32(tbl.Rows[count + count2].ItemArray[9]))  //같은 위치값이 들어올경우 다음값을 확인
                    {
                        datastr += PositionID + "/" + 0 + "/" + data.Rows[0].ItemArray[1] + "/" + data.Rows[0].ItemArray[2]+"/";
                    }
                    else if (Convert.ToInt32(data.Rows[0].ItemArray[0]) < Convert.ToInt32(data.Rows[1].ItemArray[0])) // 하행
                    {
                        datastr += PositionID + "/" + 2 + "/" + data.Rows[0].ItemArray[1] + "/" + data.Rows[0].ItemArray[2] + "/";
                    }
                    else if (Convert.ToInt32(data.Rows[0].ItemArray[0]) > Convert.ToInt32(data.Rows[1].ItemArray[0])) // 상행
                    {
                        datastr += PositionID + "/" + 1 + "/" + data.Rows[0].ItemArray[1] + "/" + data.Rows[0].ItemArray[2] + "/";
                    }
                num++;
                Positionck = false;
            }
        }

        public int CalculateChecksum(byte[] dataToCalculate)
        {
            int checksum = 0;
            foreach (byte chData in dataToCalculate)
            {
                checksum += chData;
            }
            byte[] css = BitConverter.GetBytes(checksum);
            checksum = (byte)0xff - css[0];
            css = BitConverter.GetBytes(checksum);
            return css[0];
        }
        public string reutrnPos(int i)
        {
            string num = string.Empty;
            switch (i)
            {
                case 0:
                    num = "";
                    break;
                case 1:
                    num = "지정천교(입)";
                    break;
                case 2:
                    num = "지정천교(출)";
                    break;
                case 3:
                    num = "소막골 터널(입)";
                    break;
                case 4:
                    num = "소막골 터널(출)";
                    break;
                case 5:
                    num = "선로 상태";
                    break;
                case 6:
                    num = "보통고가(입)";
                    break;
                case 7:
                    num = "보통고가(출)";
                    break;
                case 8:
                    num = "서원주 변전소";
                    break;
                case 9:
                    num = "광터고가(입)";
                    break;
                case 10:
                    num = "광터고가(출)";
                    break;
                case 11:
                    num = "지장물1(입)";
                    break;
                case 12:
                    num = "지장물1(출)";
                    break;
                case 13:
                    num = "지장물2(입)";
                    break;
                case 14:
                    num = "지장물2(출)";
                    break;
                case 15:
                    num = "만종터널(입)";
                    break;
                case 16:
                    num = "만종터널(출)";
                    break;
                case 17:
                    num = "만종역(입)";
                    break;
                case 18:
                    num = "만종역(출)";
                    break;
                case 19:
                    num = "만종천교(입)";
                    break;
                case 20:
                    num = "만종천교(출)";
                    break;
                case 21:
                    num = "호저터널(입)";
                    break;
                case 22:
                    num = "호저터널(출)";
                    break;
                case 23:
                    num = "가현교(입)";
                    break;
                case 24:
                    num = "가현교(출)";
                    break;
                case 25:
                    num = "점실교(입)";
                    break;
                case 26:
                    num = "점실교(출)";
                    break;
                case 27:
                    num = "원주천교(입)";
                    break;
                case 28:
                    num = "원주천교(출)";
                    break;
                case 29:
                    num = "";
                    break;

            }
            return num;
        }
        public int[] PosCk(int Position)
        {
            int[] Staend = new int[2];
            string[] Startend = Split(@"..\..\ZoneData.txt");
            if (Convert.ToInt32(Startend[0]) < Position && Position < Convert.ToInt32(Startend[1]))
            {
                Staend[0] = Convert.ToInt32(Startend[0]);
                Staend[1] = Convert.ToInt32(Startend[1]);
            }
            else if (Convert.ToInt32(Startend[2]) < Position && Position < Convert.ToInt32(Startend[3]))
            {
                Staend[0] = Convert.ToInt32(Startend[2]);
                Staend[1] = Convert.ToInt32(Startend[3]);
            }
            else if (Convert.ToInt32(Startend[4]) < Position && Position < Convert.ToInt32(Startend[5]))
            {
                Staend[0] = Convert.ToInt32(Startend[4]);
                Staend[1] = Convert.ToInt32(Startend[5]);
            }
            else if (Convert.ToInt32(Startend[6]) < Position && Position < Convert.ToInt32(Startend[7]))
            {
                Staend[0] = Convert.ToInt32(Startend[6]);
                Staend[1] = Convert.ToInt32(Startend[7]);
            }
            else if (Convert.ToInt32(Startend[8]) < Position && Position < Convert.ToInt32(Startend[9]))
            {
                Staend[0] = Convert.ToInt32(Startend[8]);
                Staend[1] = Convert.ToInt32(Startend[9]);
            }
            else if (Convert.ToInt32(Startend[10]) < Position && Position < Convert.ToInt32(Startend[11]))
            {
                Staend[0] = Convert.ToInt32(Startend[10]);
                Staend[1] = Convert.ToInt32(Startend[11]);
            }
            else if (Convert.ToInt32(Startend[12]) < Position && Position < Convert.ToInt32(Startend[13]))
            {
                Staend[0] = Convert.ToInt32(Startend[12]);
                Staend[1] = Convert.ToInt32(Startend[13]);
            }
            else if (Convert.ToInt32(Startend[14]) < Position && Position < Convert.ToInt32(Startend[15]))
            {
                Staend[0] = Convert.ToInt32(Startend[14]);
                Staend[1] = Convert.ToInt32(Startend[15]);
            }
            else if (Convert.ToInt32(Startend[16]) < Position && Position < Convert.ToInt32(Startend[17]))
            {
                Staend[0] = Convert.ToInt32(Startend[16]);
                Staend[1] = Convert.ToInt32(Startend[17]);
            }
            else if (Convert.ToInt32(Startend[18]) < Position && Position < Convert.ToInt32(Startend[19]))
            {
                Staend[0] = Convert.ToInt32(Startend[18]);
                Staend[1] = Convert.ToInt32(Startend[19]);
            }
            else if (Convert.ToInt32(Startend[20]) < Position && Position < Convert.ToInt32(Startend[21]))
            {
                Staend[0] = Convert.ToInt32(Startend[20]);
                Staend[1] = Convert.ToInt32(Startend[21]);
            }
            else if (Convert.ToInt32(Startend[22]) < Position && Position < Convert.ToInt32(Startend[23]))
            {
                Staend[0] = Convert.ToInt32(Startend[22]);
                Staend[1] = Convert.ToInt32(Startend[23]);
            }
            else if (Convert.ToInt32(Startend[24]) < Position && Position < Convert.ToInt32(Startend[25]))
            {
                Staend[0] = Convert.ToInt32(Startend[24]);
                Staend[1] = Convert.ToInt32(Startend[25]);
            }
            else if (Convert.ToInt32(Startend[26]) < Position && Position < Convert.ToInt32(Startend[27]))
            {
                Staend[0] = Convert.ToInt32(Startend[26]);
                Staend[1] = Convert.ToInt32(Startend[27]);
            }
            else if (Convert.ToInt32(Startend[28]) < Position && Position < Convert.ToInt32(Startend[29]))
            {
                Staend[0] = Convert.ToInt32(Startend[28]);
                Staend[1] = Convert.ToInt32(Startend[29]);
            }
            else if (Convert.ToInt32(Startend[30]) < Position && Position < Convert.ToInt32(Startend[31]))
            {
                Staend[0] = Convert.ToInt32(Startend[30]);
                Staend[1] = Convert.ToInt32(Startend[31]);
            }
            else if (Convert.ToInt32(Startend[32]) < Position && Position < Convert.ToInt32(Startend[33]))
            {
                Staend[0] = Convert.ToInt32(Startend[32]);
                Staend[1] = Convert.ToInt32(Startend[33]);
            }
            else if (Convert.ToInt32(Startend[34]) < Position && Position < Convert.ToInt32(Startend[35]))
            {
                Staend[0] = Convert.ToInt32(Startend[34]);
                Staend[1] = Convert.ToInt32(Startend[35]);
            }
            else if (Convert.ToInt32(Startend[36]) < Position && Position < Convert.ToInt32(Startend[37]))
            {
                Staend[0] = Convert.ToInt32(Startend[36]);
                Staend[1] = Convert.ToInt32(Startend[37]);
            }
            else if (Convert.ToInt32(Startend[38]) < Position && Position < Convert.ToInt32(Startend[39]))
            {
                Staend[0] = Convert.ToInt32(Startend[38]);
                Staend[1] = Convert.ToInt32(Startend[39]);
            }
            else if (Convert.ToInt32(Startend[40]) < Position && Position < Convert.ToInt32(Startend[41]))
            {
                Staend[0] = Convert.ToInt32(Startend[40]);
                Staend[1] = Convert.ToInt32(Startend[41]);
            }
            else if (Convert.ToInt32(Startend[42]) < Position && Position < Convert.ToInt32(Startend[43]))
            {
                Staend[0] = Convert.ToInt32(Startend[42]);
                Staend[1] = Convert.ToInt32(Startend[43]);
            }
            else if (Convert.ToInt32(Startend[44]) < Position && Position < Convert.ToInt32(Startend[45]))
            {
                Staend[0] = Convert.ToInt32(Startend[44]);
                Staend[1] = Convert.ToInt32(Startend[45]);
            }
            else if (Convert.ToInt32(Startend[46]) < Position && Position < Convert.ToInt32(Startend[47]))
            {
                Staend[0] = Convert.ToInt32(Startend[46]);
                Staend[1] = Convert.ToInt32(Startend[47]);
            }
            else if (Convert.ToInt32(Startend[48]) < Position && Position < Convert.ToInt32(Startend[49]))
            {
                Staend[0] = Convert.ToInt32(Startend[48]);
                Staend[1] = Convert.ToInt32(Startend[49]);
            }
            else if (Convert.ToInt32(Startend[50]) < Position && Position < Convert.ToInt32(Startend[51]))
            {
                Staend[0] = Convert.ToInt32(Startend[50]);
                Staend[1] = Convert.ToInt32(Startend[51]);
            }
            else if (Convert.ToInt32(Startend[52]) < Position && Position < Convert.ToInt32(Startend[53]))
            {
                Staend[0] = Convert.ToInt32(Startend[52]);
                Staend[1] = Convert.ToInt32(Startend[53]);
            }
            else if (Convert.ToInt32(Startend[54]) < Position && Position < Convert.ToInt32(Startend[55]))
            {
                Staend[0] = Convert.ToInt32(Startend[54]);
                Staend[1] = Convert.ToInt32(Startend[55]);
            }
            else if (Convert.ToInt32(Startend[56]) < Position && Position < Convert.ToInt32(Startend[57]))
            {
                Staend[0] = Convert.ToInt32(Startend[56]);
                Staend[1] = Convert.ToInt32(Startend[57]);
            }



            return Staend;
        }

        public void TrainDatainput(ref Define Data, int counts, DataTable tbl) // 처음 열차가 들어왔을때 발생하는 함수
        {

            string[] Startend = Split(@"..\..\ZoneData.txt");
            int count = counts;

            //데이타 내용
            DateTime dd = DateTime.Now;
            Train TData = new Train();
            TData.ScrDevName = "DAS";   //소스장치명 
            string date = string.Empty;
            string pos = string.Empty;

            //TimeStamp
            TData.TimeStamp[0] = string.Format("{0:yy}", dd); //년
            TData.TimeStamp[1] = string.Format("{0:MM}", dd); //월
            TData.TimeStamp[2] = string.Format("{0:dd}", dd); //일
            TData.TimeStamp[3] = string.Format("{0:HH}", dd); //시
            TData.TimeStamp[4] = string.Format("{0:mm}", dd); //분
            TData.TimeStamp[5] = string.Format("{0:ss}", dd); //초
            TData.TimeStamp[6] = "0";

            TData.DstDevName = "TNMS"; // 목적장치명
      
            for (int j = 0; j < count; j++)
            {
              
                //열차 운행 상제 정보
                TrainDetail TDetailData = new TrainDetail();
                if (tbl == null) // 열차가 없을 시 ( 시간 정보)
                {
                    TDetailData.TrainLast_space_ID = "0"; // 지점 ID
                    TDetailData.Position = 0; // 열차 위치 정보
                    //열차 검지 시간(최종)
                    TDetailData.LastTime[0] = "0";//string.Format("{0:yy}", dd);
                    TDetailData.LastTime[1] = "0";// string.Format("{0:MM}", dd);
                    TDetailData.LastTime[2] = "0";// string.Format("{0:dd}", dd);
                    TDetailData.LastTime[3] = "0";// string.Format("{0:HH}", dd);
                    TDetailData.LastTime[4] = "0"; //string.Format("{0:mm}", dd);
                    TDetailData.LastTime[5] = "0"; //string.Format("{0:ss}", dd);
                    TDetailData.LastTime[6] = "0";
                }
                else
                {
                    date = tbl.Rows[j].ItemArray[0].ToString(); // 시간 데이터 받기 
                    TDetailData.TrainLast_space_ID = tbl.Rows[j].ItemArray[3].ToString().Trim(); // 지점 ID

                    switch (Convert.ToInt32(TDetailData.TrainLast_space_ID))
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
                    TDetailData.Position = Convert.ToInt32(pos); // 열차 위치 정보

                    //열차 검지 시간(최종)
                    TDetailData.LastTime[0] = date.Substring(2, 2);//string.Format("{0:yy}", dd);
                    TDetailData.LastTime[1] = date.Substring(5, 2);// string.Format("{0:MM}", dd);
                    TDetailData.LastTime[2] = date.Substring(8, 2);// string.Format("{0:dd}", dd);
                    if (date.Substring(15, 1) == ":")
                    {
                        TDetailData.LastTime[3] = date.Substring(14, 1);// string.Format("{0:HH}", dd);
                        TDetailData.LastTime[4] = date.Substring(16, 2); //string.Format("{0:mm}", dd);
                        TDetailData.LastTime[5] = date.Substring(19, 2); //string.Format("{0:ss}", dd);
                    }
                    else
                    {
                        TDetailData.LastTime[3] = date.Substring(14, 2);// string.Format("{0:HH}", dd);
                        TDetailData.LastTime[4] = date.Substring(17, 2); //string.Format("{0:mm}", dd);
                        TDetailData.LastTime[5] = date.Substring(20, 2); //string.Format("{0:ss}", dd);
                    }
                    TDetailData.LastTime[6] = "0";
                }
                if (tbl != null)
                    TDetailData.UpDown = Convert.ToInt32(tbl.Rows[j].ItemArray[2]); // 1 상행 2 하행 
                else
                    TDetailData.UpDown = 0;
                if (tbl == null)
                    TDetailData.TrainKind = "0";
                else
                if (tbl.Rows[j].ItemArray[6].ToString().Trim() == "Train")
                    TDetailData.TrainKind = "0"; // 열차 종류
                else if (tbl.Rows[j].ItemArray[6].ToString().Trim() == "KTX") // Other
                    TDetailData.TrainKind = "1";
                else
                    TDetailData.TrainKind = "2";
                TDetailData.TrainID = "0"; // 열자 구분용 ID

                TData.TrainData.Add(TDetailData);
            }
            if (tbl == null)
                count = 0;
            TData.TrainNum = count; // 검지된 열차갯수
            Data.LData = TData;
        }

        public byte[] byteinput(byte[] bytedata, Define data, int start, int count)
        {
            //데이터 내용( 헤 더  )
            stringToByte(bytedata, data.LData.ScrDevName, start); start += 8; //ScrDevName
            intToByte(bytedata, Convert.ToInt32(data.LData.TimeStamp[0]), start, 1); start++; //년
            intToByte(bytedata, Convert.ToInt32(data.LData.TimeStamp[1]), start, 1); start++; //월
            intToByte(bytedata, Convert.ToInt32(data.LData.TimeStamp[2]), start, 1); start++; //일
            intToByte(bytedata, Convert.ToInt32(data.LData.TimeStamp[3]), start, 1); start++; //시
            intToByte(bytedata, Convert.ToInt32(data.LData.TimeStamp[4]), start, 1); start++; //분
            intToByte(bytedata, Convert.ToInt32(data.LData.TimeStamp[5]), start, 1); start++; //초
            intToByte(bytedata, Convert.ToInt32(data.LData.TimeStamp[6]), start, 2); start += 2; //Reserved
            stringToByte(bytedata, data.LData.DstDevName, start); start += 8; //DstDevName
            intToByte(bytedata, data.LData.TrainNum, start, 1); start++; // 열차 운행 갯수
            for (int i = 0; i < count; i++)
            {
                //열차 상세정보
                intToByte(bytedata, Convert.ToInt32(data.LData.TrainData[i].TrainLast_space_ID), start, 4); start += 4; //열차검지 ID
                intToByte(bytedata, data.LData.TrainData[i].Position, start, 2); start += 2; //열차위치 정보
                intToByte(bytedata, Convert.ToInt32(data.LData.TrainData[i].LastTime[0]), start, 1); start++; //열차검지 시간(최종) 년
                intToByte(bytedata, Convert.ToInt32(data.LData.TrainData[i].LastTime[1]), start, 1); start++; //열차검지 시간(최종) 월
                intToByte(bytedata, Convert.ToInt32(data.LData.TrainData[i].LastTime[2]), start, 1); start++; //열차검지 시간(최종) 일
                intToByte(bytedata, Convert.ToInt32(data.LData.TrainData[i].LastTime[3]), start, 1); start++; //열차검지 시간(최종) 시
                intToByte(bytedata, Convert.ToInt32(data.LData.TrainData[i].LastTime[4]), start, 1); start++; //열차검지 시간(최종) 분
                intToByte(bytedata, Convert.ToInt32(data.LData.TrainData[i].LastTime[5]), start, 1); start++; //열차검지 시간(최종) 초
                intToByte(bytedata, Convert.ToInt32(data.LData.TrainData[i].LastTime[6]), start, 2); start += 2; //열차검지 시간(최종) 예약
                intToByte(bytedata, Convert.ToInt32(data.LData.TrainData[i].UpDown), start, 1); start++; //상행 하행 구분
                intToByte(bytedata, Convert.ToInt32(data.LData.TrainData[i].TrainKind), start, 1); start++; //열차 종류
                intToByte(bytedata, Convert.ToInt32(data.LData.TrainData[i].TrainID), start, 4); start += 4; //열차 ID
                intToByte(bytedata, 0, start, 1); start++; //진출진입
                intToByte(bytedata, 0, start, 1); start++; //속도
                intToByte(bytedata, 0, start, 1); start++; //예약
            }
            return bytedata;
        }

        public string position(int ID)
        {
            string pos = string.Empty;
            switch (ID)
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
            return pos;
        }
        public void PositionClear(object sender, ElapsedEventArgs e)
        {
            Ls = 0;
            Ls2 = 0;
            Ms = 0;
            Ms2 = 0;
            Ss = 0;
            Ss2 = 0;
            count = 0;
            count1 = 0;
            count2 = 0;

        }

        public void insertTrainData(ref int Traincount, int PreID)
        {
            DataTable TrainData = DbCon2.dbConn("select * from TrainData ", "ARTech");
            DataTable sdata;
            DataTable mdata;
            DataTable ldata;
            string STag = string.Empty;
            string MTag = string.Empty;
            string LTag = string.Empty;
            string[] Startend = Split(@"..\..\ZoneData.txt");
            sdata = DbCon.dbConn("  SELECT DISTINCT  POSITION ,ALARM_DATETIME_UTC,TAG  FROM alarm_tab where id > " + PreID + " and POSITION > " + Startend[2] + "and POSITION < " + Startend[3] + " order by ALARM_DATETIME_UTC asc ", DBName);


            if (sdata != null)
                if (sdata.Rows.Count > 0)
                {
                    for (int j = 0; j < sdata.Rows.Count; j++)
                    {
                        if (Ss == 0)
                        {
                            Ss = (int)sdata.Rows[j].ItemArray[0];
                        }
                        else if (Ss != 0 && Ss2 == 0)
                        {

                            if (Ss < (int)sdata.Rows[j].ItemArray[0])
                            {
                                Ss2 = (int)sdata.Rows[j].ItemArray[0];

                            }
                        }

                        if (Ss != 0 && Ss2 != 0)
                        {
                            if (Ss < Ss2)
                            {
                                startck = true;
                            }
                            count++;
                        }
                    }
                    if (Ss != 0 && Ss2 != 0 && count > 1)
                    {
                        Ss = 0;
                        Ss2 = 0;
                        count = 0;
                    }

                }
            ldata = DbCon.dbConn("  SELECT DISTINCT  POSITION ,ALARM_DATETIME_UTC,TAG  FROM alarm_tab where id > " + PreID + " and POSITION > " + Startend[56] + "and POSITION < " + Startend[57] + " order by ALARM_DATETIME_UTC asc ", DBName);
            if (ldata != null)
                if (ldata.Rows.Count > 0)
                {
                    for (int j = 0; j < ldata.Rows.Count; j++)
                    {
                        if (Ls == 0)
                        {
                            Ls = (int)ldata.Rows[j].ItemArray[0];
                        }
                        else if (Ls != 0 && Ls2 == 0)
                        {
                            if (Ls > (int)ldata.Rows[j].ItemArray[0])
                                Ls2 = (int)ldata.Rows[j].ItemArray[0];
                        }

                        if (Ls != 0 && Ls2 != 0)
                        {
                            endck = true;
                            count1++;
                        }
                    }
                    if (Ls != 0 && Ls2 != 0 && count1 > 1)
                    {
                        Ls = 0;
                        Ls2 = 0;
                        count1 = 0;
                    }
                }
            mdata = DbCon.dbConn("  SELECT DISTINCT  POSITION ,ALARM_DATETIME_UTC,TAG  FROM alarm_tab where id > " + PreID + " and POSITION > " + Startend[38] + "and POSITION < " + Startend[39] + " order by ALARM_DATETIME_UTC asc ", DBName);

            if (mdata != null)
                if (mdata.Rows.Count > 0)
                    if (mdata.Rows.Count > 0)
                    {

                        for (int j = 0; j < mdata.Rows.Count; j++)
                        {
                            if (Ms == 0)
                            {

                                Ms = (int)mdata.Rows[j].ItemArray[0];
                            }
                            else if (Ms != 0 && Ms2 == 0)
                            {
                                if (Ms > (int)mdata.Rows[j].ItemArray[0])
                                    Ms2 = (int)mdata.Rows[j].ItemArray[0];
                            }

                            if (Ms != 0 && Ms2 != 0)
                            {
                                if (Ms > Ms2)
                                    if (Ms2 > (int)mdata.Rows[j].ItemArray[0])
                                    {
                                        Middck = true;
                                    }
                                count2++;
                            }

                        }
                        if (Ms != 0 && Ms2 != 0 && count2 > 2)
                        {
                            Ms = 0;
                            Ms2 = 0;
                            count2 = 0;
                        }
                    }

            if (endck)
            {
                //  DataTable ldata2 = DbCon.dbConn("SELECT DISTINCT  POSITION ,ALARM_DATETIME_UTC,TAG  FROM alarm_tab where id >= " + PreID + " and POSITION > " + Startend[54] + "and POSITION < " + Startend[55] + " order by ALARM_DATETIME_UTC asc ", DBName);

                if (ldata.Rows.Count > 0)
                {
                    DateTime Datez = (DateTime)ldata.Rows[0].ItemArray[1];
                    Datez = Datez.AddHours(9);
                    int posz = (int)ldata.Rows[0].ItemArray[0];
                    string Tagz = (string)ldata.Rows[0].ItemArray[2];
                    DbCon2.insertqry("insert into TrainData Values ('" + Datez.ToString("yyyy-MM-dd HH:mm:ss") + "',0,1,28," + posz + ",0,'" + "KTX" + "')", "ARTech");
                    DbCon2.insertqry("insert into LogData Values ('" + "KTX" + "','상행','원주천교(출)'," + posz + ",'" + Datez.ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                    Traincount++;
                    endck = false;
                    Ls = 0;
                    Ls2 = 0;
                    count1 = 0;
                }
            }
            if (startck)
            {
                //  DataTable sdata2 = DbCon.dbConn("SELECT DISTINCT  POSITION ,ALARM_DATETIME_UTC,TAG  FROM alarm_tab where id >= " + PreID + " and POSITION > " + Startend[4] + "and POSITION < " + Startend[5] + " order by ALARM_DATETIME_UTC asc ", DBName);

                if (sdata.Rows.Count > 0)
                {
                    DateTime Datez = (DateTime)sdata.Rows[0].ItemArray[1];
                    Datez = Datez.AddHours(9);
                    int posz = (int)sdata.Rows[0].ItemArray[0];
                    string Tagz = "Train"; // (string)sdata.Rows[0].ItemArray[2];

                    DbCon2.insertqry("insert into TrainData Values ('" + Datez.ToString("yyyy-MM-dd HH:mm:ss") + "',0,2,1," + posz + ",0,'" + Tagz + "')", "ARTech");
                    DbCon2.insertqry("insert into LogData Values ('" + Tagz + "','하행','지정천교(입)'," + posz + ",'" + Datez.ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                    other = 0;
                    ktx = 0;
                    startck = false;
                    Ss = 0;
                    Ss2 = 0;
                    count = 0;
                }
            }
            if (Middck)
            {
                DateTime Datez = (DateTime)mdata.Rows[0].ItemArray[1];
                Datez = Datez.AddHours(9);
                int posz = (int)mdata.Rows[0].ItemArray[0];
                string Tagz = (string)mdata.Rows[0].ItemArray[2];
                DbCon2.insertqry("insert into TrainData Values ('" + Datez.ToString("yyyy-MM-dd HH:mm:ss") + "',0,1,19," + posz + ",0,'" + "Other" + "')", "ARTech");
                DbCon2.insertqry("insert into LogData Values ('" + "Other" + "','상행','만종천교(입)'," + posz + ",'" + Datez.ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                Middck = false;
                Traincount++;
                Ms = 0;
                Ms2 = 0;
                count2 = 0;
            }

        }
    }
}
