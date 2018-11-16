using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using static ProtocolTest.MainWindow;

namespace ProtocolTest
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        bool Pass = true;
        bool TimerCk = true;
        byte[] byteData;
        Event ev = null;
        DBConnect DbCon = new DBConnect();
        DBConnect DbCon2 = new DBConnect();
        Function Fun = new Function();
        bool send = false;
        Define RequestData;
        Train Train;
        public List<byte[]> Data = new List<byte[]>();
        clsHeader2 cHeader = new clsHeader2();
        Define Datas = new Define();
        System.Threading.Thread listenerThread;
        System.Threading.Thread Dataupadate;
        System.Timers.Timer timer = new System.Timers.Timer();
        System.Timers.Timer TrainTimeCk = new System.Timers.Timer();
        System.Timers.Timer Demoon = new System.Timers.Timer();
        int DemoPos = 100;
        int fID = 0;
        int fPos = 0;
        bool ff = true;
        bool con = true;
        bool Timecon = true;
        string IP;
        int Port;
        bool bContinue = true;
        ushort[] RegisterArray = new ushort[ushort.MaxValue];
        private TcpClient tcpSynClient;
        private List<clsClient> ClientPool = new List<clsClient>();
        public System.Windows.Forms.NotifyIcon notify;

        //시간 동기화를 위한 변수
        int Year;
        int Mon;
        int Day;
        int Hor;
        int MIn;
        int Sec;

        public MainWindow()
        {
            InitializeComponent();
            Status.Content = "시작 대기중..";
            DbCon2.TextInputData2();

        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SetSystemTime(ref SystemTime systemTime);

        public void DataUp()
        {
            while (true)
            {
                    cHeader.DataUpdate();
            }
        }
        public void Start()
        {

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { Status.Content = "Server 와의 연결 시도중..."; }));
            DbCon2.deleteqry("delete from Position", "ARTech");// Rootdb와 Pos 삭제 
            DbCon2.deleteqry("delete from TrainData", "ARTech");// Rootdb와 Pos 삭제 
            bContinue = true;
            bool Connect = true;
            while (Connect)
            {
                WaitAcceptClient(Connect);
            }

        }

        public void TimeCk(object sender, ElapsedEventArgs e)
        {
            DataTable TimeCk = DbCon2.dbConn("select * from TrainData", "ARTech");
            if (TimeCk != null)
                if (TimeCk.Rows.Count != 0)
                {
                    for (int i = 0; i < TimeCk.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) != 40 && (Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 20 || Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 19) && Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) == 2)
                            DbCon2.Updateqry("Update TrainData set TimeCk =  " + (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) + 1) + "where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID =  " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");
                        if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) != 30 && (Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 21 || Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 22) && Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) == 2)
                            DbCon2.Updateqry("Update TrainData set TimeCk =  " + (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) + 1) + "where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID =  " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");
                        if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) != 60 && (Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 1 || Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 2) && Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) == 2)
                            DbCon2.Updateqry("Update TrainData set TimeCk =  " + (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) + 1) + "where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID =  " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");
                        if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) != 25 && (Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 27 || Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 28) && Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) == 1)
                            DbCon2.Updateqry("Update TrainData set TimeCk =  " + (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) + 1) + "where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID =  " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");
                        if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) != 200 && (Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 19) && Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) == 1)
                            DbCon2.Updateqry("Update TrainData set TimeCk =  " + (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) + 1) + "where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID =  " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");
                        if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) != 15 && (Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 22 || Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 23 || Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]) == 24) && Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) == 1)
                            DbCon2.Updateqry("Update TrainData set TimeCk =  " + (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) + 1) + "where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID =  " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");
                    }
                }

        }

       

        public void WaitAcceptClient(bool Connect)
        {

            try
            {

                IPAddress localAddr = IPAddress.Parse(IP);
                if (tcpSynClient == null)
                {
                    tcpSynClient = new TcpClient();
                    tcpSynClient.ConnectAsync(IP, Port);
                    //tsk.Wait(5000);
                    tcpSynClient.ReceiveBufferSize = 256;
                    tcpSynClient.SendBufferSize = 256;
                    LingerOption lingerOption = new LingerOption(true, 1);
                    tcpSynClient.LingerState = lingerOption;

                }
                bContinue = true;
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { Status.Content = "Server 와의 연결 성공 "; }));

            }
            catch (Exception e)
            {

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { Status.Content = "Server 와의 연결 실패 "; }));
                bContinue = false;

            }
            while (bContinue)
            {
                try
                {
                    if (RequestData == null)
                        RequestData = new Define();

                    byte[] byteClientHeader = new byte[41];
  

                    //지속적인 업데이트
                    if (Dataupadate == null)
                    {
                        Dataupadate = new Thread(DataUp);
                        Dataupadate.Start();
                    }

                    NetworkStream stream;
                    stream = tcpSynClient.GetStream(); // 해당 정보를 읽어옴
                    var Len = stream.ReadAsync(byteClientHeader, 0, byteClientHeader.Length); // 해당 정보를 읽어서 저장  
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { Status.Content = "Server 의 요청 대기중.. "; }));
                    if (Len.Result != 0)
                    {

                        RequestData.Preamble = System.Text.Encoding.ASCII.GetString(Fun.ByteString(byteClientHeader, 0, 4)); // Preamble (0)
                        RequestData.Lenght = Fun.Byteint(byteClientHeader, 4, 4);  // Length (1)
                        RequestData.SeqNo = Fun.Byteint(byteClientHeader, 8, 2); // SeqNo (2) 
                        RequestData.OpCode = Fun.ByteString(byteClientHeader, 10, 4); // OpCode (3)
                        RequestData.Reserved = Fun.ByteString(byteClientHeader, 38, 2); // Reserved (5)

                        // Data 내용 (4)
                        if (Train == null)
                            Train = new Train();

                        Train.ScrDevName = System.Text.Encoding.ASCII.GetString(Fun.ByteString(byteClientHeader, 14, 4));
                        Train.DstDevName = System.Text.Encoding.ASCII.GetString(Fun.ByteString(byteClientHeader, 30, 3));
                        RequestData.LData = Train;
                        if (Timecon) // 최초 구동시 시간 동기화
                        {
                            Year = 2000 + Fun.Byteint(byteClientHeader, 22, 1);
                            Mon = Fun.Byteint(byteClientHeader, 23, 1);
                            Day = Fun.Byteint(byteClientHeader, 24, 1);
                            Hor = Fun.Byteint(byteClientHeader, 25, 1);
                            MIn = Fun.Byteint(byteClientHeader, 26, 1);
                            Sec = Fun.Byteint(byteClientHeader, 27, 1);
                            SystemTime ServerTime = new SystemTime();
                            ServerTime.wYear = (ushort)Year;
                            ServerTime.wMonth = (ushort)Mon;
                            ServerTime.wDay = (ushort)Day;
                            ServerTime.wHour = (ushort)Hor;
                            ServerTime.wMinute = (ushort)MIn;
                            ServerTime.wSecond = (ushort)Sec;
                            SetSystemTime(ref ServerTime);
                            Timecon = false;
                        }

                        //매 시간마다 시간 동기화
                        if (Hor != DateTime.Now.Hour)
                        {
                            Timecon = true;
                        }

                        if (RequestData.Preamble == "PRT3" && RequestData.Lenght == 33 && RequestData.LData.ScrDevName == "TNMS" && RequestData.LData.DstDevName == "DAS") // 해당 조건에 맞을 경우에만 요청에 대해 Respone JDM
                        {
                            RequestData.LData.TrainData.Clear();
                            byteData = cHeader.ResponseHeader(RequestData);
                            SendData(tcpSynClient, byteData);
                            //bContinue = false;
                            //tcpSynClient.Close();
                            //tcpSynClient = null;
                        }
                    }
                    else if (Len.Result == 0)
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { Status.Content = "Server 와의 연결 실패 "; }));
                        bContinue = false;
                        tcpSynClient.Close();
                        tcpSynClient = null;
                    }
                }
                catch (ThreadAbortException ex)
                {
                    bContinue = false;
                    tcpSynClient.Close();
                    tcpSynClient = null;
                }
                catch (Exception e)
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { Status.Content = "Server 와의 연결 실패 "; }));
                  //  MessageBox.Show("예외 : " + e.ToString() + "\n", "오류 발생");
                    bContinue = false;
                    tcpSynClient.Close();
                    tcpSynClient = null;
                }

            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            if (listenerThread == null)
            {
                listenerThread = new Thread(Start);
                listenerThread.Start();

                this.IP = IPText.Text;
                this.Port = Convert.ToInt32(PortText.Text);
                timer.Interval = 1000; // 이벤트 확인 시간
                if (TimerCk)
                    timer.Elapsed += new ElapsedEventHandler(ConnectionEvent);
                timer.Start();

                TrainTimeCk.Interval = 1000; // 빠져나간 기차정보 삭제(30초 이상된)
                if (TimerCk)
                {
                    TrainTimeCk.Elapsed += new ElapsedEventHandler(TimeCk);
                    TimerCk = false;
                }
                TrainTimeCk.Start();
            }

        }

        private void SendData(TcpClient Client, byte[] byteData)
        {
            Client.GetStream().Write(byteData, 0, byteData.Length);
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { StatusData.Content = "데이터 전송 성공 ( " + DateTime.Now.ToString("HH:mm:ss") + " )"; }));
        }

        public void ConnectionEvent(object sender, ElapsedEventArgs e)
        {
            if (ev == null)
                ev = new Event();
            string DBName = DbCon.TextInputData();
            DbCon2.TextInputData2();
            DataTable tbl = DbCon.dbConn("select * from alarm_tab order by ID desc", DBName); //새로운 디비가 생겻을 경우에만 실행
            int tt = Convert.ToInt32(tbl.Rows[0].ItemArray[0]);
            if (tcpSynClient != null)
            {
                if (ff) // 최초 1회 비교값 설정
                {
                    fID = Convert.ToInt32(tbl.Rows[0].ItemArray[0]); // ID
                    fPos = Convert.ToInt32(tbl.Rows[0].ItemArray[9]); // 위치
                    ff = false;
                    ev.IP = IP;
                    ev.Port = Convert.ToInt32(Port);
                }
                if (!con) //연결을 실패할경우 계속 시도
                {
                    con = ev.GetValue(fPos, fID, ref Data, tcpSynClient);
                    tbl = DbCon.dbConn("select * from alarm_tab order by ID desc", DBName);
                    fID = Convert.ToInt32(tbl.Rows[0].ItemArray[0]); // ID
                    fPos = Convert.ToInt32(tbl.Rows[0].ItemArray[9]); // 위치
                }
                if (fPos != 0) // 데이터 값이 바뀔경우 들어감
                    if (Convert.ToInt32(tbl.Rows[0].ItemArray[0]) != fID && Convert.ToInt32(tbl.Rows[0].ItemArray[9]) != fPos && con && Pass)
                    {
                        Pass = false;
                        con = ev.GetValue(fPos, fID, ref Data, tcpSynClient);
                        tbl = DbCon.dbConn("select * from alarm_tab order by ID desc", DBName);
                        fID = Convert.ToInt32(tbl.Rows[0].ItemArray[0]); // ID
                        fPos = Convert.ToInt32(tbl.Rows[0].ItemArray[9]); // 위치
                        Pass = true;

                    }
            }

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void cancel_exit_cross_close_png_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Dataupadate != null)
                Dataupadate.Abort();
            if (listenerThread != null)
                listenerThread.Abort();
            this.Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            DBSetUp DBSet = new DBSetUp();
            DBSet.Show();
        }

        private void menu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        public static RoutedCommand MyCommand = new RoutedCommand();

        private void Demo_Click(object sender, RoutedEventArgs e)
        {
            DemoPos = 100;
            Demoon.Interval = 100;
            Demoon.Elapsed += new ElapsedEventHandler(Demo_Play);
            Demoon.Start();
        }

        private void DemoOFF_Click(object sender, RoutedEventArgs e)
        {
            Demoon.Stop();
        }
        private void Demo_Play(object sender, ElapsedEventArgs e)
        {
            DemoPos += 15;
            string DBName = DbCon.TextInputData();
            DbCon.insertqry(" INSERT INTO alarm_tab VALUES (1, 0 , 'KTX' , '', '' ,'"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"' , 0, 0, "+DemoPos+", '', 0, 0, '', 0, 0 , 0)", DBName);
        }

            private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (listenerThread != null)
            {
                listenerThread.Abort();
                listenerThread = null;
                if (Dataupadate != null)
                    Dataupadate.Abort();
                Dataupadate = null;
                timer.Stop();
                TrainTimeCk.Stop();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Icon test = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack:\application:,,,\\ProtocolTest;component\\Resources\\cloud-computing.png.ico")).Stream);//@"\cloud-computing.png.ico");
            //notify = new System.Windows.Forms.NotifyIcon();
            //notify.Icon = test;
            //notify.Text = "DAS 통신";
            //notify.DoubleClick += Notify_DoubleClicke;


        }

        private void Notify_DoubleClicke(object sender, EventArgs e)
        {
            //notify.Visible = false;
            //this.Visibility = Visibility.Visible;
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //notify.Visible = true;
            //this.Visibility = Visibility.Collapsed;
            //e.Cancel = true;
            //return;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (Dataupadate != null)
                Dataupadate.Abort();
            if (listenerThread != null)
                listenerThread.Abort();
            this.Close();
        }

       
    }

    class clsClient
    {
        public TcpClient mClient;
        public System.Threading.Thread mThread;
        public clsClient(TcpClient cClient, System.Threading.Thread cThread)
        {
            mClient = cClient;
            mThread = cThread;
        }
    }
}
