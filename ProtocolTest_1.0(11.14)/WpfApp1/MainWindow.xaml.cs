using DAS_GUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        double orginalWidth, originalHeight;
        ScaleTransform scale = new ScaleTransform();
        TrainGUI TrainGUI = new TrainGUI(); // 열차 및 선 그리기 클래스
        DbConn Dbconn = new DbConn(); //  디비 클래스
        DbConn Dbconn2 = new DbConn(); //  디비 클래스
        Function Fun = new Function(); // 함수 클래스
        List<Image> GUITrain = new List<Image>(); // 열차 정보
        PopUp doc = new PopUp();
        PopData[] PopUpdata = new PopData[60]; // 팝업 정보저장
        List<Line> Line = new List<Line>(); // 라인 그리기 
        List<Image> CkButton = new List<Image>(); // 체크 버튼 그리기
        bool[] PopupCk = new bool[60];
        bool[] buttonColorChange = new bool[60];
        Thread listenerThread;
        System.Timers.Timer DataGridTimer = new System.Timers.Timer();
        System.Timers.Timer TrainTimeCk = new System.Timers.Timer(); // 열차 로그 삭제
        System.Timers.Timer Play = new System.Timers.Timer(); // 로그 자동 플레이
        ProtocolTest.MainWindow Protocol;
        int index = 0;
        bool ReviewOnoff = false;
        bool onoff = true;
        int removecount = 0;
        int removecount2 = 0;
        string indexnumber = "";
        bool reviewck = true;
        bool Prereview = true;
        bool Logck = false;

        public MainWindow()
        {
            InitializeComponent();
            Dbconn2.TextInputData2();
            for (int i = 0; i < PopupCk.Length; i++)
            {
                PopupCk[i] = false;
                buttonColorChange[i] = true;
            }
            Dbconn2.deleteqry("delete from TrainData", "ARTech");
            listenerThread = new Thread(Mapping);
            listenerThread.Start();
            Common_Image_Add();
            Protocol = new ProtocolTest.MainWindow();
            Protocol.Show();
            DataGridTimer.Interval = 1000; // 이벤트 확인 시간
            DataGridTimer.Elapsed += new System.Timers.ElapsedEventHandler(DataGridUpdate);
            DataGridTimer.Start();

            TrainTimeCk.Interval = 1000; //오알람 삭제 삭제(30초 이상된)
            TrainTimeCk.Elapsed += new ElapsedEventHandler(TimeCk);
            TrainTimeCk.Start();

            Play.Interval = 2000; //오알람 삭제 삭제(30초 이상된)
            Play.Elapsed += new ElapsedEventHandler(NextTimer);

        }

        #region 그외 업데이트 함수
        public void NextTimer(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                DataTable tbl = null;

                Year.IsEnabled = false;
                Month.IsEnabled = false;
                Day.IsEnabled = false;
                Hour.IsEnabled = false;
                Min.IsEnabled = false;
                Sec.IsEnabled = false;
                Prebtn.IsEnabled = true;

                string Date = string.Empty;
                Date += (Year.Text + "-");
                Date += (Month.Text + "-");
                Date += (Day.Text + " ");
                Date += (Hour.Text + ":");
                Date += (Min.Text + ":");
                Date += (Sec.Text);
                if (reviewck)
                    tbl = Dbconn2.dbConn("select * from LogData where Date > '" + Date + "' order by Date asc", "ARTech");
                else
                    tbl = Dbconn2.dbConn("select * from LogData where [index] > '" + index + "' order by [index] asc", "ARTech");
                if (tbl.Rows.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("데이터가 존재하지 않습니다.");
                }
                else
                {
                    DateTime datetm = (DateTime)tbl.Rows[0].ItemArray[5];
                    string Dtstring = datetm.ToString("yyyy-MM-dd HH:mm:ss");
                    Year.Text = Dtstring.Substring(0, 4);
                    Month.Text = Dtstring.Substring(5, 2);
                    Day.Text = Dtstring.Substring(8, 2);
                    Hour.Text = Dtstring.Substring(11, 2);
                    Min.Text = Dtstring.Substring(14, 2);
                    Sec.Text = Dtstring.Substring(17, 2);
                    index = (int)tbl.Rows[0].ItemArray[0];
                    Date += (Year.Text + "-");
                    Date += (Month.Text + "-");
                    Date += (Day.Text + " ");
                    Date += (Hour.Text + ":");
                    Date += (Min.Text + ":");
                    Date += (Sec.Text);
                    NextReviewMapping(tbl);
                    reviewck = false;
                }

            }));
        }
        public void TimeCk(object sender, ElapsedEventArgs e)
        {
            bool brk = false;
            DataTable TimeCk = Dbconn2.dbConn("select * from TrainData", "ARTech");
            if (TimeCk != null)
                if (TimeCk.Rows.Count != 0)
                {

                    for (int i = 0; i < TimeCk.Rows.Count; i++)
                    {

                        if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) == 40 && ((int)TimeCk.Rows[i].ItemArray[3] == 19 || (int)TimeCk.Rows[i].ItemArray[3] == 20 ) && (int)TimeCk.Rows[i].ItemArray[2] == 2)
                        {
                            Dbconn2.deleteqry("delete from Traindata where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");

                        }

                        if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) == 30 && ((int)TimeCk.Rows[i].ItemArray[3] == 21 || (int)TimeCk.Rows[i].ItemArray[3] == 22) && (int)TimeCk.Rows[i].ItemArray[2] == 2)
                        {
                            Dbconn2.deleteqry("delete from Traindata where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");

                        }
                        if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) == 200 && (int)TimeCk.Rows[i].ItemArray[3] == 19 && (int)TimeCk.Rows[i].ItemArray[2] == 1)
                        {
                            DateTime date = (DateTime)TimeCk.Rows[i].ItemArray[0];
                            Dbconn2.deleteqry("delete from Traindata where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");
                            
                            Dbconn2.deleteqry("delete from Logdata where [index] in(select top 1 [index] from Logdata where UpDown = '상행' and Postion > 5200 and Postion< 6047 order by[index] desc)", "ARTech");
                            Logck = true;
                        }
                        if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) == 20 && ((int)TimeCk.Rows[i].ItemArray[3] == 28 || (int)TimeCk.Rows[i].ItemArray[3] == 27) && (int)TimeCk.Rows[i].ItemArray[2] == 1)
                        {
                            DateTime date = (DateTime)TimeCk.Rows[i].ItemArray[0];
                            Dbconn2.deleteqry("delete from Traindata where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID =  " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");
                          
                            Dbconn2.deleteqry(" delete from Logdata where[index] in(select top 1 [index] from Logdata where UpDown = '상행' and Postion > 9968 order by[index] desc)", "ARTech");
                            Logck = true;

                        }
                        if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) == 60 && ((int)TimeCk.Rows[i].ItemArray[3] == 1 || (int)TimeCk.Rows[i].ItemArray[3] == 2) && (int)TimeCk.Rows[i].ItemArray[2] == 2)
                        {
                            DateTime date = (DateTime)TimeCk.Rows[i].ItemArray[0];
                            Dbconn2.deleteqry("delete from Traindata where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID =  " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");
                            if ((int)TimeCk.Rows[i].ItemArray[3] == 1)
                                Dbconn2.deleteqry("delete from Logdata where[index] in(select top 1 [index] from Logdata where UpDown = '하행' and Postion < 390 order by[index] desc) ", "ARTech");
                            else if ((int)TimeCk.Rows[i].ItemArray[3] == 2)
                            {
                                DataTable Log = Dbconn2.dbConn("select top 1[index] from Logdata where UpDown = '하행' and Postion > 390 and Postion < 567 order by[index] desc", "ARTech");

                                Dbconn2.deleteqry("delete from Logdata where[index] in(select top 1[index] from Logdata where UpDown = '하행' and Postion > 390 and Postion< 567 order by[index] desc)", "ARTech");
                                int index = (int)Log.Rows[0].ItemArray[0];
                                Dbconn2.deleteqry("delete from Logdata where [Index] = " + (index - 1), "ARTech");
                            }
                            Logck = true;
                        }

                        //if (Convert.ToInt32(TimeCk.Rows[i].ItemArray[5]) == 20 && ((int)TimeCk.Rows[i].ItemArray[3] == 22 || (int)TimeCk.Rows[i].ItemArray[3] == 23 || (int)TimeCk.Rows[i].ItemArray[3] == 24) && (int)TimeCk.Rows[i].ItemArray[2] == 1)
                        //{
                        //    DateTime date = (DateTime)TimeCk.Rows[i].ItemArray[0];
                        //    Dbconn2.deleteqry("delete from Traindata where UpDown = " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[2]) + " and SpaceID =  " + Convert.ToInt32(TimeCk.Rows[i].ItemArray[3]), "ARTech");
                        //    Dbconn2.deleteqry("delete from LogData where UpDown = '상행'and Date  = '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'and  Postion > 6693 and Position < 8433 ", "ARTech");

                        //    DataTable Log = Dbconn2.dbConn("select [Index] from Logdata where UpDown = '상행'and Date  >= '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'and  Postion > 6693 and Position < 8433", "ARTech");
                        //    Dbconn2.deleteqry("delete from LogData where UpDown = '상행'and Date > = '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'and  Postion > 390 and  Postion < 567 ", "ARTech");
                        //    int index = (int)Log.Rows[0].ItemArray[0];
                        //    Dbconn2.deleteqry("delete from Logdata where [Index] = " + (index - 1), "ARTech");
                        //    Dbconn2.deleteqry("delete from Logdata where [Index] = " + (index - 2), "ARTech");
                        //    Logck = true;

                        //}


                    }

                    //TimeCk = Dbconn2.dbConn("select * from TrainData", "ARTech");
                    //for (int i = 0; i < TimeCk.Rows.Count; i++)
                    //{
                    //    int Updown = (int)TimeCk.Rows[i].ItemArray[2];
                    //    int Posid = (int)TimeCk.Rows[i].ItemArray[3];
                    //    for (int j = 0; j < TimeCk.Rows.Count; j++)
                    //    {

                    //        if (i != j)
                    //            if (Updown == (int)TimeCk.Rows[j].ItemArray[2] && Posid == (int)TimeCk.Rows[j].ItemArray[3])
                    //            {
                    //                Dbconn2.deleteqry("  delete from TrainData where [Index] in (select Min([Index]) from TrainData where Updown = " + Updown + " and SpaceId =" + Posid + "  )", "ARTech");
                    //                brk = true;
                    //                break;
                    //            }
                    //    }
                    //    if (brk)
                    //        break;
                    //}
                }

        }

        public void DataGridUpdate(object sender, ElapsedEventArgs e)
        {

            // Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { LogDataGrid.Items.Clear(); }));
            DataTable log = Dbconn2.dbConn("select * from LogData where [index] > '" + indexnumber + "' order by [index] asc", "ARTech");
            if (Logck)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { LogDataGrid.Items.Clear(); }));
                log = Dbconn2.dbConn("select * from LogData order by [Index] asc", "ARTech");
                Logck = false;
            }
            if (log.Rows.Count > 0)
            {
                indexnumber = log.Rows[log.Rows.Count - 1].ItemArray[0].ToString();
                //date = dt.ToString("yyyy-MM-dd HH:mm:ss");
                if (log != null)
                    for (int i = 0; i < log.Rows.Count; i++)
                    {
                        var LogData = new Log
                        {
                            Index = log.Rows[i].ItemArray[0].ToString(),
                            Tag = log.Rows[i].ItemArray[1].ToString(),
                            UpDown = log.Rows[i].ItemArray[2].ToString(),
                            Space = log.Rows[i].ItemArray[3].ToString(),
                            Position = log.Rows[i].ItemArray[4].ToString(),
                            Date = log.Rows[i].ItemArray[5].ToString()
                        };
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { LogDataGrid.Items.Insert(0, LogData); }));
                    }
            }


        }


        public void DataCheck(DataTable Traindata) // 지나갈시 데이터 넣기
        {
            int UpdownId = 0;
            int row = 0;
            int Jumrows = 0;
            if (ReviewOnoff)
            {
                Traindata = Dbconn2.dbConn("select * from ReviewTrain", "ARTech");
                row = 5;
                Jumrows = 8;
            }
            else
            {
                Traindata = Dbconn2.dbConn("select * from TrainData ", "ARTech");
                row = 6;
                Jumrows = 1;
            }
            for (int i = 0; i < Traindata.Rows.Count; i++)
                if (Traindata.Rows.Count != 0)
                    if (Convert.ToInt32(Traindata.Rows[i].ItemArray[2]) != 0)
                    {
                        int Id = Convert.ToInt32(Traindata.Rows[i].ItemArray[3]);
                        UpdownId = Id;
                        if (Convert.ToInt32(Traindata.Rows[i].ItemArray[2]) == 1)
                            UpdownId = Id + 30;
                       
                        PopUpdata[UpdownId] = Fun.Popupdatainsert(new PopData(), Id, CkButton[UpdownId], Traindata.Rows[i], ref PopupCk[UpdownId], buttonColorChange, UpdownId, row,1,0);
                        if (Convert.ToInt32(Traindata.Rows[i].ItemArray[2]) == 2 && UpdownId > 1) // 하행 데이터가 1개가 넘어갈시
                        {
                            string nowImageName = CkButton[UpdownId].Source.ToString();
                            nowImageName = nowImageName.Remove(0, 30);
                            string preImageName = CkButton[UpdownId - 1].Source.ToString();
                            preImageName = preImageName.Remove(0, 30);
                            //   if (PopUpdata[UpdownId - 1] == null || nowImageName != preImageName)
                            if ((int)Traindata.Rows[i].ItemArray[Jumrows] == 1)
                            {
                                Dbconn2.Updateqry("Update TrainData Set Jump = 0 where SpaceID = " + Id + " and UpDown = 2", "ARTech");

                                DataTable Predate = Dbconn2.dbConn("  select * from Logdata where UpDown ='하행' and Space ='" + Fun.reutrnPos2((int)Traindata.Rows[i].ItemArray[3] - 2) + "' order by [Index] desc ", "ARTech");
                                DateTime DD = (DateTime)Predate.Rows[0].ItemArray[5];
                                double Tim = 0;
                                TimeSpan tt = ((DateTime)Traindata.Rows[i].ItemArray[0]).Subtract(DD);
                                if (tt.TotalSeconds > 0)
                                    Tim = tt.TotalSeconds / 2.0;
                                else if (Tim < 0)
                                    Tim = 0;
                                PopUpdata[UpdownId - 1] = Fun.Popupdatainsert(new PopData(), Id - 1, CkButton[UpdownId - 1], Traindata.Rows[i], ref PopupCk[UpdownId - 1], buttonColorChange, UpdownId - 1, row, 2,-Tim);

                            } if (UpdownId > 2) // 2개가 넘어갈시
                                if ((int)Traindata.Rows[i].ItemArray[Jumrows] == 2)
                                {
                                    Dbconn2.Updateqry("Update TrainData Set Jump = 0 where SpaceID = " + Id + " and UpDown = 2", "ARTech");

                                    DataTable Predate = Dbconn2.dbConn("  select * from Logdata where UpDown ='하행' and Space ='" + Fun.reutrnPos2((int)Traindata.Rows[i].ItemArray[3] - 3) + "' order by [Index] desc ", "ARTech");
                                    DateTime DD = (DateTime)Predate.Rows[0].ItemArray[5];
                                    double Tim = 0;
                                    TimeSpan tt = ((DateTime)Traindata.Rows[i].ItemArray[0]).Subtract(DD);
                                    if (tt.TotalSeconds > 0)
                                        Tim = tt.TotalSeconds / 2.0;
                                    else if (Tim < 0)
                                        Tim = 0;
                                    PopUpdata[UpdownId - 1] = Fun.Popupdatainsert(new PopData(), Id - 1, CkButton[UpdownId - 1], Traindata.Rows[i], ref PopupCk[UpdownId - 1], buttonColorChange, UpdownId - 1, row, 2, -Tim);
                                        PopUpdata[UpdownId - 2] = Fun.Popupdatainsert(new PopData(), Id - 2, CkButton[UpdownId - 2], Traindata.Rows[i], ref PopupCk[UpdownId - 2], buttonColorChange, UpdownId - 2, row, 3, -Tim*1.5);
                                  
                                }
                        }
                        else if (Convert.ToInt32(Traindata.Rows[i].ItemArray[2]) == 1 && UpdownId < 58) // 상행 데이터가 1개가 넘어갈시ek
                        {
                            if (Traindata.Rows[i].ItemArray[6].ToString().Trim() == "Other" && UpdownId == 49)
                            {

                            }
                            else
                            {
                                string nowImageName = CkButton[UpdownId].Source.ToString();
                                nowImageName = nowImageName.Remove(0, 30);
                                string preImageName = CkButton[UpdownId + 1].Source.ToString();
                                preImageName = preImageName.Remove(0, 30);
                                //   if (PopUpdata[UpdownId + 1] == null || nowImageName != preImageName)
                                if ((int)Traindata.Rows[i].ItemArray[Jumrows] == 1)
                                {
                                    Dbconn2.Updateqry("Update TrainData Set Jump = 0 where SpaceID = " + Id + " and UpDown = 1", "ARTech");
                                    DataTable Predate = Dbconn2.dbConn("  select * from Logdata where UpDown ='상행' and Space ='" + Fun.reutrnPos2((int)Traindata.Rows[i].ItemArray[3] +2) + "' order by [Index] desc ", "ARTech");
                                    DateTime DD = (DateTime)Predate.Rows[0].ItemArray[5];
                                    double Tim = 0;
                                    TimeSpan tt = ((DateTime)Traindata.Rows[i].ItemArray[0]).Subtract(DD);
                                    if (tt.TotalSeconds > 0)
                                        Tim = tt.TotalSeconds / 2.0;
                                    else if (Tim < 0)
                                        Tim = 0;
                                    PopUpdata[UpdownId + 1] = Fun.Popupdatainsert(new PopData(), Id + 1, CkButton[UpdownId + 1], Traindata.Rows[i], ref PopupCk[UpdownId + 1], buttonColorChange, UpdownId + 1, row, 2,-Tim);

                                }// if (Id != 21 && Id != 20)
                                // {
                                if (UpdownId < 57)
                                    if ((int)Traindata.Rows[i].ItemArray[Jumrows] == 2)
                                    {

                                        DataTable Predate = Dbconn2.dbConn("  select * from Logdata where UpDown ='상행' and Space ='" + Fun.reutrnPos2((int)Traindata.Rows[i].ItemArray[3] + 3) + "' order by [Index] desc ", "ARTech");
                                        DateTime DD = (DateTime)Predate.Rows[0].ItemArray[5];
                                        double Tim = 0;
                                        TimeSpan tt = ((DateTime)Traindata.Rows[i].ItemArray[0]).Subtract(DD);
                                        if (tt.TotalSeconds > 0)
                                            Tim = tt.TotalSeconds / 2.0;
                                        else if (Tim < 0)
                                            Tim = 0;
                                        Dbconn2.Updateqry("Update TrainData Set Jump = 0 where SpaceID = " + Id + " and UpDown = 1", "ARTech");
                                        PopUpdata[UpdownId + 1] = Fun.Popupdatainsert(new PopData(), Id + 1, CkButton[UpdownId + 1], Traindata.Rows[i], ref PopupCk[UpdownId + 1], buttonColorChange, UpdownId + 1, row, 2,-Tim);
                                        PopUpdata[UpdownId + 2] = Fun.Popupdatainsert(new PopData(), Id + 2, CkButton[UpdownId + 2], Traindata.Rows[i], ref PopupCk[UpdownId + 2], buttonColorChange, UpdownId + 2, row, 3,-Tim*1.5);
                                    }
                            }
                            // }
                            //else
                            //{
                            //    if (GUITrain.Count == 0)
                            //    {
                            //        if (UpdownId < 57)
                            //            if (PopUpdata[UpdownId + 2] == null || nowImageName != preImageName)
                            //                PopUpdata[UpdownId + 2] = Fun.Popupdatainsert(new PopData(), Id + 2, CkButton[UpdownId + 2], Traindata.Rows[i], ref PopupCk[UpdownId + 2], buttonColorChange, UpdownId + 2, row);
                            //    }
                            //}
                        }
                    }
        }

        public void Point_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) // 팝업 이미지 함수
        {
            Image Point = (Image)sender;
            int number = Convert.ToInt32(Point.Name.Remove(0, 5));
            if (PopupCk[number] && PopUpdata[number] != null)
            {
                doc.Owner = this;
                doc.Hide();
                doc.Date = PopUpdata[number].Date;
                doc.Tag = PopUpdata[number].Tag;
                doc.UPDown = PopUpdata[number].UPDown;
                doc.Position = PopUpdata[number].Position;
                if (number == 14 || number == 44)
                {
                    doc.Top = System.Windows.Forms.Control.MousePosition.Y;
                    doc.Left = System.Windows.Forms.Control.MousePosition.X - doc.Width;
                }
                else if (number == 15 || number == 45)
                {
                    doc.Left = System.Windows.Forms.Control.MousePosition.X - doc.Width;
                    doc.Top = System.Windows.Forms.Control.MousePosition.Y - doc.Height;
                }
                else if (15 < number && number < 30)
                {
                    doc.Left = System.Windows.Forms.Control.MousePosition.X;
                    doc.Top = System.Windows.Forms.Control.MousePosition.Y - doc.Height;
                }
                else
                {
                    doc.Left = System.Windows.Forms.Control.MousePosition.X;
                    doc.Top = System.Windows.Forms.Control.MousePosition.Y;
                }

                doc.Show();
                PopupCk[number] = false;
            }
            else
            {
                doc.Hide();
                PopupCk[number] = true;
            }
        }

        public void SpaceLogData(object sender, MouseButtonEventArgs e)
        {
            PointReview Prt = new PointReview();
            Prt.Show();
            Prt.reFlush();
            Image img = (Image)sender;
            string pointnum = img.Name.Remove(0, 5);
            Prt.RightbuttonLog((Convert.ToInt32(pointnum)));
        }
        #endregion

        #region 기본 이미지 그리기 함수
        public void Common_Image_Add() // 기본 이미지 그리기 함수
        {


            Main_gird.Children.Add(TrainGUI.ImageAdd(0, 185, @"\Images\train-station.png"));
            //라벨 넣기
            Main_gird.Children.Add(TrainGUI.LabelAdd(-12, 240, "동화역"));

            //라인 그리기
            Line.Add(TrainGUI.LineAdd(40, 240, 0, 240, Brushes.Black));
            Line.Add(TrainGUI.LineAdd(40, 300, 0, 300, Brushes.Wheat));
            Line.Add(TrainGUI.LineAdd(0, 238, 0, 560, Brushes.Black));
            Line.Add(TrainGUI.LineAdd(0, 298, 0, 500, Brushes.Wheat));
            Line.Add(TrainGUI.LineAdd(0, 500, 40, 500, Brushes.Wheat));
            Line.Add(TrainGUI.LineAdd(0, 560, 70, 560, Brushes.Black));
            // 6000m 나가는 라인
            Line.Add(TrainGUI.LineAdd(0, 560, 70, 560, Brushes.Black));
            Line.Add(TrainGUI.LineAdd(0, 560, 70, 560, Brushes.Black));
            Line.Add(TrainGUI.LineAdd(0, 560, 70, 560, Brushes.Wheat));
            Line.Add(TrainGUI.LineAdd(0, 560, 70, 560, Brushes.Wheat));
            for (int i = 0; i < Line.Count; i++)
                Main_gird.Children.Add(Line[i]);

            //체크 버튼 그리기 && 라벨 넣기
            int x = 0;
            for (int i = 0; i < 60; i++)
            {

                if (i < 15)
                {
                    CkButton.Add(TrainGUI.CkImasgeAdd(60 + x, 220));
                    CkButton[i].MouseRightButtonDown += SpaceLogData;
                    if (i == 0)
                    {
                        Main_gird.Children.Add(TrainGUI.ImageAdd(45 + x, 110, @"\Images\Left-arrow.png"));
                        System.Windows.Controls.Label lb = TrainGUI.LabelAdd(98 + x, 115, "서울");
                        lb.FontSize = 10;
                        Main_gird.Children.Add(lb);
                    }
                    if (i == 1 || i == 6 || i == 9)
                    {
                        Main_gird.Children.Add(TrainGUI.ImageAdd(110 + x, 190, @"\Images\bridge.png"));
                        if (i == 1)
                            Main_gird.Children.Add(TrainGUI.LabelAdd(98 + x, 240, "지정천교"));
                        else if (i == 6)
                            Main_gird.Children.Add(TrainGUI.LabelAdd(98 + x, 240, "보통고가"));
                        else if (i == 9)
                            Main_gird.Children.Add(TrainGUI.LabelAdd(98 + x, 240, "광터고가"));
                    }
                    if (i == 5)
                        Main_gird.Children.Add(TrainGUI.LabelAdd(98 + x, 240, "선로상태"));
                    if (i == 8)
                        Main_gird.Children.Add(TrainGUI.LabelAdd(98 + x, 240, "서원주 변전소"));
                    if (i == 3)
                    {
                        Main_gird.Children.Add(TrainGUI.ImageAdd(110 + x, 180, @"\Images\tunnel.png"));
                        Main_gird.Children.Add(TrainGUI.LabelAdd(98 + x, 240, "소막골 터널"));
                    }
                    if (i == 11 || i == 13)
                    {
                        Main_gird.Children.Add(TrainGUI.ImageAdd(110 + x, 175, @"\Images\rocks.png"));
                        if (i == 11)
                            Main_gird.Children.Add(TrainGUI.LabelAdd(98 + x, 240, "지장물1"));
                        else if (i == 13)
                            Main_gird.Children.Add(TrainGUI.LabelAdd(98 + x, 240, "지장물2"));
                    }
                    if (i != 14)
                        x += 125;
                    CkButton[i].MouseLeftButtonDown += Point_MouseLeftButtonDown;

                    CkButton[i].Name = "Point" + i;
                    Common_Image_List.Children.Add(CkButton[i]);
                }
                else if (i < 30)
                {

                    CkButton.Add(TrainGUI.CkImasgeAdd(60 + x, 480));
                    CkButton[i].MouseRightButtonDown += SpaceLogData;
                    if (i == 19 || i == 23 || i == 25 || i == 27)
                    {
                        Main_gird.Children.Add(TrainGUI.ImageAdd(x - 15, 450, @"\Images\bridge.png"));
                        if (i == 19)
                            Main_gird.Children.Add(TrainGUI.LabelAdd(x - 27, 500, "만종천교"));
                        else if (i == 23)
                            Main_gird.Children.Add(TrainGUI.LabelAdd(x - 27, 500, "가현교"));
                        else if (i == 25)
                            Main_gird.Children.Add(TrainGUI.LabelAdd(x - 27, 500, "점실교"));
                        else if (i == 27)
                            Main_gird.Children.Add(TrainGUI.LabelAdd(x - 27, 500, "원주천교"));
                    }

                    if (i == 15 || i == 21)
                    {
                        Main_gird.Children.Add(TrainGUI.ImageAdd(x - 15, 440, @"\Images\tunnel.png"));
                        if (i == 15)
                        {
                            Main_gird.Children.Add(TrainGUI.LabelAdd(x - 27, 500, "만종 터널"));
                            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\Left-arrow_black.png"));
                            System.Windows.Controls.Label lb = TrainGUI.LabelAdd(x - 27, 660, "횡성");
                            lb.FontSize = 10;
                            Main_gird.Children.Add(lb);
                        }
                        else if (i == 21)
                            Main_gird.Children.Add(TrainGUI.LabelAdd(x - 27, 500, "호저 터널"));
                    }
                    if (i == 17)
                    {
                        Main_gird.Children.Add(TrainGUI.ImageAdd(x - 15, 445, @"\Images\train-station.png"));
                        Main_gird.Children.Add(TrainGUI.LabelAdd(x - 27, 500, "만종역"));
                    }
                    if (i != 29)
                        x -= 125;
                    CkButton[i].MouseLeftButtonDown += Point_MouseLeftButtonDown;
                    CkButton[i].Name = "Point" + i;
                    Common_Image_List.Children.Add(CkButton[i]);
                }
                else if (i < 45)
                {

                    CkButton.Add(TrainGUI.CkImasgeAdd(60 + x, 280));
                    CkButton[i].MouseRightButtonDown += SpaceLogData;
                    if (i != 44)
                        x += 125;
                    CkButton[i].MouseLeftButtonDown += Point_MouseLeftButtonDown;
                    CkButton[i].Name = "Point" + i;
                    Common_Image_List.Children.Add(CkButton[i]);
                }
                else if (i < 60)
                {
                    CkButton.Add(TrainGUI.CkImasgeAdd(60 + x, 540));
                    CkButton[i].MouseRightButtonDown += SpaceLogData;
                    if (i != 59)
                        x -= 125;
                    CkButton[i].MouseLeftButtonDown += Point_MouseLeftButtonDown;
                    CkButton[i].Name = "Point" + i;
                    Common_Image_List.Children.Add(CkButton[i]);
                }
            }
            CkButton[0].Visibility = Visibility.Hidden;
            CkButton[30].Visibility = Visibility.Hidden;
            CkButton[29].Visibility = Visibility.Hidden;
            CkButton[59].Visibility = Visibility.Hidden;
            var uriSource = new Uri(@"\Images\uncheck.png", UriKind.Relative);
            for (int i = 0; i < Common_Image_List.Children.Count; i++)
            {
                if (Common_Image_List.Children[i].GetType().Name == "Image")
                {
                    Image Point = (Image)Common_Image_List.Children[i];
                    Point.Source = new BitmapImage(uriSource);

                }
            }

            System.Windows.Controls.Label LogoLabel = TrainGUI.LabelAdd(0, 0, "위험요소 감지시스템(DAS)");
            LogoLabel.FontSize = 20;
            LogoLabel.Width = this.Width * 0.9;
            LogoLabel.Height = this.Height;
            //광케이블 거리
            Main_gird.Children.Add(LogoLabel);
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "390m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "567m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "749m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "1464m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "1478m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "1488m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "1619m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "1698m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "1740m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "2442m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "2539m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "2602m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "2652m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "2703m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "2831m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "3182m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "3419m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "3775m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "6047m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "6117m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "6693m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "7993m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "8370m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "8433m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "9281m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "9546m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "9968m"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "10350m"));

            //토목거리
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(459m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(619m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(816m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(1500m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, ""));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(1552m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(1657m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, ""));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(1785m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(2471m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, ""));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, ""));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, ""));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, ""));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(2851m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(3173m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(3505m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, ""));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(5821m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(5881m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(6446m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(7720m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(8095m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(8140m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(8990m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(9235m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(9667m)"));
            Main_gird.Children.Add(TrainGUI.LabelAdd(0, 0, "(10027m)"));



            //방향성 이미지
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\right-arrow.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\Down-arrow_black.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\Left-arrow_black.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\left-arrow.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\Up-arrow.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\right-arrow_white.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\right-arrow.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\Down-arrow_black.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\Left-arrow_black.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\left-arrow.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\Up-arrow.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\right-arrow_white.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\Left-arrow_black.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\Left-arrow_black.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\right-arrow_white.png"));
            Main_gird.Children.Add(TrainGUI.ImageAdd(x + 44, 655, @"\Images\right-arrow_white.png"));


        }
        #endregion

        #region 열차 생성 및 위치 업데이트
        public void Mapping()
        {
            DataTable Traindata;
            bool AddDataCK = false;
            List<int> delete = new List<int>();
            string[] Ckpos = null;
            bool Pass = false;
            Uri uriSource2;
            while (true)
            {
                if (ReviewOnoff)
                {
                    if (onoff)
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {


                        onoff = false;
                        for (int i = 0; i < CkButton.Count; i++)
                        {
                            uriSource2 = new Uri(@"\Images\uncheck.png", UriKind.Relative);
                            CkButton[i].Source = new BitmapImage(uriSource2);
                        }
                        PopUpdata = new PopData[60];
                        PopupCk = new bool[60];
                        buttonColorChange = new bool[60];
                        for (int i = 0; i < PopupCk.Length; i++)
                        {
                            PopupCk[i] = false;
                            buttonColorChange[i] = true;
                        }

                    }));
                    }
                    Traindata = Dbconn2.dbConn("select * from ReviewTrain", "ARTech");
                }
                else
                    Traindata = Dbconn2.dbConn("select * from TrainData ", "ARTech");
                if (Traindata != null)
                {


                    if (true)
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            while (Traindata.Rows.Count < GUITrain.Count)
                            {
                                GUITrain.RemoveAt(GUITrain.Count - 1);
                                Train.Children.RemoveAt(Train.Children.Count - 1);
                            }
                        }));
                        if (Traindata.Rows.Count > 0)
                        {

                            for (int i = 0; i < Traindata.Rows.Count; i++)
                            {
                                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                                {
                                    Point point = Fun.Pointretrun(Convert.ToInt32(Traindata.Rows[i].ItemArray[3]), Convert.ToInt32(Traindata.Rows[i].ItemArray[2]), CkButton);
                                    if (Traindata.Rows.Count > GUITrain.Count)
                                    {
                                        GUITrain.Add(TrainGUI.TrainAdd(point, Convert.ToInt32(Traindata.Rows[i].ItemArray[2]))); // 새로운 열차정보 넣기
                                        AddDataCK = true;
                                    }
                                    if (14 < Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) && Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) < 30 && Convert.ToInt32(Traindata.Rows[i].ItemArray[2]) == 2)
                                    {
                                        var uriSource = new Uri(@"\Images\train_Left.png", UriKind.Relative);
                                        GUITrain[i].Source = new BitmapImage(uriSource);
                                    }
                                    else if (0 <= Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) && Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) < 15 && Convert.ToInt32(Traindata.Rows[i].ItemArray[2]) == 2)
                                    {
                                        var uriSource = new Uri(@"\Images\train.png", UriKind.Relative);
                                        GUITrain[i].Source = new BitmapImage(uriSource);
                                    }
                                    else if (14 < Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) && Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) < 30 && Convert.ToInt32(Traindata.Rows[i].ItemArray[2]) == 1)
                                    {
                                        var uriSource = new Uri(@"\Images\train.png", UriKind.Relative);
                                        GUITrain[i].Source = new BitmapImage(uriSource);
                                    }
                                    else if (0 <= Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) && Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) < 15 && Convert.ToInt32(Traindata.Rows[i].ItemArray[2]) == 1)
                                    {
                                        var uriSource = new Uri(@"\Images\train_Left.png", UriKind.Relative);
                                        GUITrain[i].Source = new BitmapImage(uriSource);
                                    }
                                    if (!GUITrain[i].Margin.Equals(new Thickness(point.X, point.Y, 0, 0)))
                                    {
                                        GUITrain[i].Margin = new Thickness(point.X, point.Y, 0, 0);
                                    }
                                    if (AddDataCK)
                                    {
                                        Train.Children.Add(GUITrain[GUITrain.Count - 1]); // Grid에 트레인 데이터넣기
                                        AddDataCK = false;
                                    }


                                }));
                            }

                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { DataCheck(Traindata); }));

                            string[] Startend = Fun.Split(@"..\..\ZoneData.txt");
                            string DBName = Dbconn.TextInputData();
                            Dbconn2.TextInputData2();
                            for (int i = 0; i < Traindata.Rows.Count; i++)
                            {
                                if (Convert.ToInt32(Traindata.Rows[i].ItemArray[2]) == 1 && Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) == 1)
                                {
                                    DataTable Postion = Dbconn2.dbConn("select * from Position order by  ID desc", "ARTech");
                                    DataTable data = Dbconn.dbConn("  SELECT DISTINCT Top(2) POSITION ,ALARM_DATETIME_UTC  FROM alarm_tab where id > " + Postion.Rows[0].ItemArray[2].ToString() + " and POSITION > " + Startend[2] + "and POSITION < " + Startend[3] + " order by ALARM_DATETIME_UTC asc ", DBName);
                                    if (data.Rows.Count == 0)
                                        Dbconn2.deleteqry("delete from TrainData where SpaceID =" + Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) + "and Pos =" + Convert.ToInt32(Traindata.Rows[i].ItemArray[4]), "ARTech");
                                }
                                else if (Convert.ToInt32(Traindata.Rows[i].ItemArray[2]) == 2 && Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) == 28)
                                {
                                    DataTable Postion = Dbconn2.dbConn("select * from Position order by  ID desc", "ARTech");
                                    DataTable data = Dbconn.dbConn("  SELECT DISTINCT Top(2) POSITION ,ALARM_DATETIME_UTC  FROM alarm_tab where id > " + Postion.Rows[0].ItemArray[2].ToString() + " and POSITION > " + Startend[56] + "and POSITION < " + Startend[57] + " order by ALARM_DATETIME_UTC asc ", DBName);
                                    if (data.Rows.Count == 0)
                                        Dbconn2.deleteqry("delete from TrainData where SpaceID =" + Convert.ToInt32(Traindata.Rows[i].ItemArray[3]) + "and Pos =" + Convert.ToInt32(Traindata.Rows[i].ItemArray[4]), "ARTech");
                                }

                            }

                            //Ckpos = new string[Traindata.Rows.Count];
                            //for (int i = 0; i < Traindata.Rows.Count; i++)
                            //    Ckpos[i] = Traindata.Rows[i].ItemArray[3].ToString();
                            //Pass = false;

                        }

                    }
                }
            }
        }
        #endregion

        public void NextReviewMapping(DataTable tbl)
        {
            DataTable ReTb = Dbconn2.dbConn("select * from ReviewTrain", "ARTech");
            DateTime ReviewDate = (DateTime)tbl.Rows[0].ItemArray[5];
            int UpDown = 0;
            int Pos = (int)tbl.Rows[0].ItemArray[4];
            string Tag = tbl.Rows[0].ItemArray[1].ToString();
            if (tbl.Rows[0].ItemArray[2].ToString().Trim() == "상행")
                UpDown = 1;
            else
                UpDown = 2;
            int posint = Fun.reutrnPos(tbl.Rows[0].ItemArray[3].ToString().Trim());

            if (ReTb.Rows.Count == 0)
            {

                if (UpDown == 1 && (posint == 19))
                    Dbconn2.insertqry("insert into ReviewTrain values('" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + UpDown + "," + posint + "," + Pos + ",'" + Tag.Trim() + "'," + 0 + ",1,0)", "ARTech");
                else if (((posint == 1) && UpDown == 2) || (posint == 28 && UpDown == 1))
                    Dbconn2.insertqry("insert into ReviewTrain values('" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + UpDown + "," + posint + "," + Pos + ",'" + Tag.Trim() + "'," + 0 + ",0,0)", "ARTech");
            }
            else
            {
                bool addck = true;
                for (int i = 0; i < ReTb.Rows.Count; i++)
                {

                    if ((int)ReTb.Rows[i].ItemArray[2] == 2 && (int)ReTb.Rows[i].ItemArray[3] == 19) //6000 미터 나가는 데이터 
                    {
                        removecount += 1;
                        if (removecount == 5)
                        {
                            Dbconn2.deleteqry("delete from ReviewTrain where UpDown= 2 and SpaceID = 19 or SpaceID = 20 ", "ARTech");
                            removecount = 0;
                        }
                    }
                    if ((int)ReTb.Rows[i].ItemArray[2] == 2 && ((int)ReTb.Rows[i].ItemArray[3] == 20 || (int)ReTb.Rows[i].ItemArray[3] == 21)) //6000 미터 지나갈시 카운트 초기화 
                    {
                        removecount = 0;
                    }

                    if ((int)ReTb.Rows[i].ItemArray[3] == 1 && (int)ReTb.Rows[i].ItemArray[2] == 1) // 상행 나가는 데이터
                    {
                        Dbconn2.deleteqry("delete from ReviewTrain where UpDown= 1 and SpaceID = 1", "ARTech");
                    }
                    else if ((int)ReTb.Rows[i].ItemArray[3] == 28 && (int)ReTb.Rows[i].ItemArray[2] == 2) // 하행 나가는 데이터
                    {
                        Dbconn2.deleteqry("delete from ReviewTrain where UpDown= 2 and SpaceID = 28", "ARTech");

                    }

                    if (UpDown == 1 && (int)ReTb.Rows[i].ItemArray[2] == 1 && ((int)ReTb.Rows[i].ItemArray[3] - 1 == posint || (int)ReTb.Rows[i].ItemArray[3] - 2 == posint || (int)ReTb.Rows[i].ItemArray[3] - 3 == posint))// 상행의 경우
                    {
                        if((int)ReTb.Rows[i].ItemArray[3] - 2 == posint)
                        Dbconn2.Updateqry("Update ReviewTrain Set Jump = 1, Pos = " + Pos + ", SpaceID = " + posint + ", Datetime = '" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "' where UpDown =" + UpDown + "and  SpaceID = " + (int)ReTb.Rows[i].ItemArray[3], "ARTech");
                        if((int)ReTb.Rows[i].ItemArray[3] - 3 == posint)
                        Dbconn2.Updateqry("Update ReviewTrain Set Jump = 2, Pos = " + Pos + ", SpaceID = " + posint + ", Datetime = '" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "' where UpDown =" + UpDown + "and  SpaceID = " + (int)ReTb.Rows[i].ItemArray[3], "ARTech");
                        else
                        Dbconn2.Updateqry("Update ReviewTrain Set Pos = " + Pos + ", SpaceID = " + posint + ", Datetime = '" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "' where UpDown =" + UpDown + "and  SpaceID = " + (int)ReTb.Rows[i].ItemArray[3], "ARTech");
                        addck = false;
                    }
                    if (UpDown == 2 && (int)ReTb.Rows[i].ItemArray[2] == 2 && ((int)ReTb.Rows[i].ItemArray[3] + 1 == posint || (int)ReTb.Rows[i].ItemArray[3] + 2 == posint || (int)ReTb.Rows[i].ItemArray[3] + 3 == posint))// 하행의 경우
                    {
                        if((int)ReTb.Rows[i].ItemArray[3] + 2 == posint)
                        Dbconn2.Updateqry("Update ReviewTrain Set Jump = 1,Pos = " + Pos + ", SpaceID = " + posint + ", Datetime = '" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "' where UpDown =" + UpDown + "and  SpaceID = " + (int)ReTb.Rows[i].ItemArray[3], "ARTech");
                        if((int)ReTb.Rows[i].ItemArray[3] + 3 == posint)
                        Dbconn2.Updateqry("Update ReviewTrain Set Jump = 2,Pos = " + Pos + ", SpaceID = " + posint + ", Datetime = '" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "' where UpDown =" + UpDown + "and  SpaceID = " + (int)ReTb.Rows[i].ItemArray[3], "ARTech");
                        else
                        Dbconn2.Updateqry("Update ReviewTrain Set Pos = " + Pos + ", SpaceID = " + posint + ", Datetime = '" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "' where UpDown =" + UpDown + "and  SpaceID = " + (int)ReTb.Rows[i].ItemArray[3], "ARTech");
                        addck = false;
                    }
                    if (UpDown == 1 && (int)ReTb.Rows[i].ItemArray[2] == 1 && (int)ReTb.Rows[i].ItemArray[3] == posint)
                    {
                        addck = false;
                    }
                    if (UpDown == 2 && (int)ReTb.Rows[i].ItemArray[2] == 2 && (int)ReTb.Rows[i].ItemArray[3] == posint)
                        addck = false;



                }

                if (addck)
                {
                    if (UpDown == 1 && (posint == 19))
                        Dbconn2.insertqry("insert into ReviewTrain values('" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + UpDown + "," + posint + "," + Pos + ",'" + Tag.Trim() + "'," + 0 + ",1,0)", "ARTech");
                    else if (((posint == 1) && UpDown == 2) || (posint == 28 && UpDown == 1))
                        Dbconn2.insertqry("insert into ReviewTrain values('" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + UpDown + "," + posint + "," + Pos + ",'" + Tag.Trim() + "'," + 0 + ",0,0)", "ARTech");

                }
                InvalidateVisual();

            }
        }


        public void PreReviewMapping(DataTable tbl)
        {
            DataTable ReTb = Dbconn2.dbConn("select * from ReviewTrain", "ARTech");
            DateTime ReviewDate = (DateTime)tbl.Rows[0].ItemArray[5];
            int UpDown = 0;
            int Pos = (int)tbl.Rows[0].ItemArray[4];

            string Tag = tbl.Rows[0].ItemArray[1].ToString();
            if (tbl.Rows[0].ItemArray[2].ToString().Trim() == "상행")
                UpDown = 1;
            else
                UpDown = 2;
            int posint = Fun.reutrnPos(tbl.Rows[0].ItemArray[3].ToString().Trim());
            bool addck = true;
            for (int i = 0; i < ReTb.Rows.Count; i++)
            {
                addck = true;
                if ((int)ReTb.Rows[i].ItemArray[3] == 28 && (int)ReTb.Rows[i].ItemArray[2] == 1) // 상행 나가는 데이터
                {
                    Dbconn2.deleteqry("delete from ReviewTrain where UpDown= 1 and SpaceID = 28", "ARTech");
                    //여기도 추가
                    Uri uriSource = null;
                    uriSource = new Uri(@"\Images\uncheck.png", UriKind.Relative);
                    buttonColorChange[(int)ReTb.Rows[i].ItemArray[3] + 30] = true;
                    if (uriSource != null)
                        CkButton[(int)ReTb.Rows[i].ItemArray[3] + 30].Source = new BitmapImage(uriSource);
                    PopUpdata[(int)ReTb.Rows[i].ItemArray[3] + 30] = null;
                    PopupCk[(int)ReTb.Rows[i].ItemArray[3] + 30] = false;
                }
                if ((int)ReTb.Rows[i].ItemArray[3] == 1 && (int)ReTb.Rows[i].ItemArray[2] == 2) // 하행 나가는 데이터
                {
                    Dbconn2.deleteqry("delete from ReviewTrain where UpDown= 2 and SpaceID = 1", "ARTech");
                    //여기도 추가
                    Uri uriSource = null;
                    uriSource = new Uri(@"\Images\uncheck.png", UriKind.Relative);
                    buttonColorChange[(int)ReTb.Rows[i].ItemArray[3]] = true;
                    if (uriSource != null)
                        CkButton[(int)ReTb.Rows[i].ItemArray[3]].Source = new BitmapImage(uriSource);
                    PopUpdata[(int)ReTb.Rows[i].ItemArray[3]] = null;
                    PopupCk[(int)ReTb.Rows[i].ItemArray[3]] = false;
                }
                if ((int)ReTb.Rows[i].ItemArray[3] == 19 && (int)ReTb.Rows[i].ItemArray[2] == 1) // 상행 나가는 데이터(중간)
                {

                    removecount2 += 1;
                    if (removecount2 == 5)
                    {
                        Dbconn2.deleteqry("delete from ReviewTrain where   UpDown= 1 and SpaceID = 19", "ARTech"); // 7번째 값이 1 인경우만 지우게 하기
                                                                                                                   //여기도 추가
                        Uri uriSource = null;
                        uriSource = new Uri(@"\Images\uncheck.png", UriKind.Relative);
                        if (uriSource != null)
                            CkButton[(int)ReTb.Rows[i].ItemArray[3]].Source = new BitmapImage(uriSource);
                        buttonColorChange[(int)ReTb.Rows[i].ItemArray[3] + 30] = true;
                        PopUpdata[(int)ReTb.Rows[i].ItemArray[3] + 30] = null;
                        PopupCk[(int)ReTb.Rows[i].ItemArray[3] + 30] = false;
                        removecount2 = 0;
                    }
                }
                if ((int)ReTb.Rows[i].ItemArray[2] == 1 && ((int)ReTb.Rows[i].ItemArray[3] == 20 || (int)ReTb.Rows[i].ItemArray[3] == 21)) //6000 미터 지나갈시 카운트 초기화 
                {
                    removecount2 = 0;
                }

                if (UpDown == 1 && (int)ReTb.Rows[i].ItemArray[2] == 1 && ((int)ReTb.Rows[i].ItemArray[3] + 1 == posint || (int)ReTb.Rows[i].ItemArray[3] + 2 == posint || (int)ReTb.Rows[i].ItemArray[3] + 3 == posint)) //상행(<-)의 경우
                {
                    Dbconn2.Updateqry("Update ReviewTrain Set Pos = " + Pos + ", SpaceID = " + posint + ", Datetime = '" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "' where UpDown =" + UpDown + "and  SpaceID = " + (int)ReTb.Rows[i].ItemArray[3], "ARTech");
                    addck = false;
                    Uri uriSource = null;
                    for (int z = 0; z < 4; z++)
                    {
                        if (posint == (int)ReTb.Rows[i].ItemArray[3] + z)
                            break;
                        if ((int)ReTb.Rows[i].ItemArray[3] + z + 30 == 60)
                            break;
                        uriSource = new Uri(@"\Images\uncheck.png", UriKind.Relative);
                        if (uriSource != null)
                            CkButton[(int)ReTb.Rows[i].ItemArray[3] + z + 30].Source = new BitmapImage(uriSource);
                        buttonColorChange[(int)ReTb.Rows[i].ItemArray[3] + z + 30] = true;
                        PopUpdata[(int)ReTb.Rows[i].ItemArray[3] + z + 30] = null;
                        PopupCk[(int)ReTb.Rows[i].ItemArray[3] + z + 30] = false;
                    }

                }
                if (UpDown == 2 && (int)ReTb.Rows[i].ItemArray[2] == 2 && ((int)ReTb.Rows[i].ItemArray[3] - 1 == posint || (int)ReTb.Rows[i].ItemArray[3] - 2 == posint || (int)ReTb.Rows[i].ItemArray[3] - 3 == posint)) //하행(->)의 경우
                {
                    Dbconn2.Updateqry("Update ReviewTrain Set Pos = " + Pos + ", SpaceID = " + posint + ", Datetime = '" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "' where UpDown =" + UpDown + "and  SpaceID = " + (int)ReTb.Rows[i].ItemArray[3], "ARTech");
                    addck = false;
                    Uri uriSource = null;
                    for (int z = 0; z < 4; z++)
                    {
                        if (posint == (int)ReTb.Rows[i].ItemArray[3] - z)
                            break;
                        if ((int)ReTb.Rows[i].ItemArray[3] - z < 0)
                            break;
                        uriSource = new Uri(@"\Images\uncheck.png", UriKind.Relative);
                        if (uriSource != null)
                            CkButton[(int)ReTb.Rows[i].ItemArray[3] - z].Source = new BitmapImage(uriSource);
                        buttonColorChange[(int)ReTb.Rows[i].ItemArray[3] - z] = true;
                        PopUpdata[(int)ReTb.Rows[i].ItemArray[3] - z] = null;
                        PopupCk[(int)ReTb.Rows[i].ItemArray[3] - z] = false;
                    }

                }
                if (ReTb.Rows.Count > 0)
                    for (int j = 0; j < ReTb.Rows.Count; j++)
                    {

                        if ((int)ReTb.Rows[j].ItemArray[2] == UpDown && (int)ReTb.Rows[j].ItemArray[3] == posint)
                            addck = false;
                    }

            }
            if (addck)
            {
                if ((UpDown == 1 && posint == 1) || (UpDown == 2 && posint == 28))
                    Dbconn2.insertqry("insert into ReviewTrain values('" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + UpDown + "," + posint + "," + Pos + ",'" + Tag.Trim() + "'," + 0 + ",0,0)", "ARTech");
                else if ((UpDown == 2 && posint == 19))
                {
                    Dbconn2.insertqry("insert into ReviewTrain values('" + ReviewDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + UpDown + "," + posint + "," + Pos + ",'" + Tag.Trim() + "'," + 0 + ",0,0)", "ARTech");
                }
            }
            InvalidateVisual();
        }
        #region 메인이벤트 관련 함수들
        private void Main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeSize(e.NewSize.Width, e.NewSize.Height);
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            orginalWidth = this.Width;
            originalHeight = this.Height;
            ChangeSize(this.ActualWidth, this.ActualHeight);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //폼 닫을시 발생하는 함수들
        private void Main_Closed(object sender, EventArgs e)
        {
            listenerThread.Abort();
            doc.Close();
            Protocol.Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            listenerThread.Abort();
            doc.Close();
            Protocol.Close();
            this.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Report rpt = new Report();
            rpt.Show();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            PointReview Prt = new PointReview();
            Prt.Show();

        }
        private void Demo_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "DAS (Review Mode)";
            Review.IsEnabled = false;
            ReviewOff.IsEnabled = true;

            Prebtn.Visibility = Visibility.Visible;
            Nextbtn.Visibility = Visibility.Visible;
            Year.Visibility = Visibility.Visible;
            Month.Visibility = Visibility.Visible;
            Day.Visibility = Visibility.Visible;
            Min.Visibility = Visibility.Visible;
            Hour.Visibility = Visibility.Visible;
            Sec.Visibility = Visibility.Visible;
            Playbtn.Visibility = Visibility.Visible;
            stopbtn.Visibility = Visibility.Visible;

            Yearbk.Visibility = Visibility.Visible;
            Monbk.Visibility = Visibility.Visible;
            Daybk.Visibility = Visibility.Visible;
            Minbk.Visibility = Visibility.Visible;
            Hourbk.Visibility = Visibility.Visible;
            Secbk.Visibility = Visibility.Visible;

            Prebtn.IsEnabled = false;
            Year.IsEnabled = true;
            Month.IsEnabled = true;
            Day.IsEnabled = true;
            Hour.IsEnabled = true;
            Min.IsEnabled = true;
            Sec.IsEnabled = true;
            Playbtn.IsEnabled = true;
            stopbtn.IsEnabled = false;

            index = 0;
            ReviewOnoff = true;
            Dbconn2.deleteqry("delete from ReviewTrain", "ARTech");

        }

        private void ReviewOff_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "DAS";
            Review.IsEnabled = true;
            ReviewOff.IsEnabled = false;
            Prebtn.Visibility = Visibility.Hidden;
            Nextbtn.Visibility = Visibility.Hidden;
            Playbtn.Visibility = Visibility.Hidden;
            stopbtn.Visibility = Visibility.Hidden;

            Year.Visibility = Visibility.Hidden;
            Month.Visibility = Visibility.Hidden;
            Day.Visibility = Visibility.Hidden;
            Min.Visibility = Visibility.Hidden;
            Hour.Visibility = Visibility.Hidden;
            Sec.Visibility = Visibility.Hidden;

            Yearbk.Visibility = Visibility.Hidden;
            Monbk.Visibility = Visibility.Hidden;
            Daybk.Visibility = Visibility.Hidden;
            Minbk.Visibility = Visibility.Hidden;
            Hourbk.Visibility = Visibility.Hidden;
            Secbk.Visibility = Visibility.Hidden;


            Uri uriSource2;
            for (int i = 0; i < CkButton.Count; i++)
            {
                uriSource2 = new Uri(@"\Images\uncheck.png", UriKind.Relative);
                CkButton[i].Source = new BitmapImage(uriSource2);
            }
            PopUpdata = new PopData[60];
            PopupCk = new bool[60];
            buttonColorChange = new bool[60];
            for (int i = 0; i < PopupCk.Length; i++)
            {
                PopupCk[i] = false;
                buttonColorChange[i] = true;
            }
            ReviewOnoff = false;
            reviewck = true;
            Prereview = true;

            Dbconn2.deleteqry("delete from ReviewTrain", "ARTech");
        }

        private void Nextbtn_Click(object sender, RoutedEventArgs e)
        {
            DataTable tbl = null;
            if (Year.Text == "")
                System.Windows.Forms.MessageBox.Show("년도를 설정해주시오.");
            else if (Month.Text == "")
                System.Windows.MessageBox.Show("월을 설정해주시오.");
            else if (Day.Text == "")
                System.Windows.MessageBox.Show("일을 설정해주시오.");
            else if (Hour.Text == "")
                System.Windows.MessageBox.Show("시를 설정해주시오.");
            else if (Min.Text == "")
                System.Windows.MessageBox.Show("분을 설정해주시오.");
            else if (Sec.Text == "")
                System.Windows.MessageBox.Show("초를 설정해주시오.");
            else
            {
                Year.IsEnabled = false;
                Month.IsEnabled = false;
                Day.IsEnabled = false;
                Hour.IsEnabled = false;
                Min.IsEnabled = false;
                Sec.IsEnabled = false;
                Prebtn.IsEnabled = true;

                string Date = string.Empty;
                Date += (Year.Text + "-");
                Date += (Month.Text + "-");
                Date += (Day.Text + " ");
                Date += (Hour.Text + ":");
                Date += (Min.Text + ":");
                Date += (Sec.Text);
                if (reviewck)
                    tbl = Dbconn2.dbConn("select * from LogData where Date > '" + Date + "' order by Date asc", "ARTech");
                else
                    tbl = Dbconn2.dbConn("select * from LogData where [index] > '" + index + "' order by [index] asc", "ARTech");
                if (tbl.Rows.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("데이터가 존재하지 않습니다.");
                }
                else
                {
                    DateTime datetm = (DateTime)tbl.Rows[0].ItemArray[5];
                    string Dtstring = datetm.ToString("yyyy-MM-dd HH:mm:ss");
                    Year.Text = Dtstring.Substring(0, 4);
                    Month.Text = Dtstring.Substring(5, 2);
                    Day.Text = Dtstring.Substring(8, 2);
                    Hour.Text = Dtstring.Substring(11, 2);
                    Min.Text = Dtstring.Substring(14, 2);
                    Sec.Text = Dtstring.Substring(17, 2);
                    index = (int)tbl.Rows[0].ItemArray[0];
                    Date += (Year.Text + "-");
                    Date += (Month.Text + "-");
                    Date += (Day.Text + " ");
                    Date += (Hour.Text + ":");
                    Date += (Min.Text + ":");
                    Date += (Sec.Text);
                    NextReviewMapping(tbl);
                    reviewck = false;
                }
            }


        }

        private void Prebtn_Click(object sender, RoutedEventArgs e)
        {
            DataTable tbl = null;
            string Date = string.Empty;
            Date += (Year.Text + "-");
            Date += (Month.Text + "-");
            Date += (Day.Text + " ");
            Date += (Hour.Text + ":");
            Date += (Min.Text + ":");
            Date += (Sec.Text);
            if (Prereview)
                tbl = Dbconn2.dbConn("select * from LogData where Date < '" + Date + "' order by Date desc", "ARTech");
            else
                tbl = Dbconn2.dbConn("select * from LogData where [index] < '" + index + "' order by [index] desc", "ARTech");
            if (tbl.Rows.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("데이터가 존재하지 않습니다.");
            }
            else
            {
                DateTime datetm = (DateTime)tbl.Rows[0].ItemArray[5];
                string Dtstring = datetm.ToString("yyyy-MM-dd HH:mm:ss");
                Year.Text = Dtstring.Substring(0, 4);
                Month.Text = Dtstring.Substring(5, 2);
                Day.Text = Dtstring.Substring(8, 2);
                Hour.Text = Dtstring.Substring(11, 2);
                Min.Text = Dtstring.Substring(14, 2);
                Sec.Text = Dtstring.Substring(17, 2);
                index = (int)tbl.Rows[0].ItemArray[0];
                PreReviewMapping(tbl);
                Prereview = false;
            }



        }

        private void Month_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Month.Text != "")
                if (Convert.ToInt32(Month.Text) > 12)
                {
                    System.Windows.Forms.MessageBox.Show("12이하의 숫자를 적으시오");
                    Month.Text = "";
                }
        }

        private void Day_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Day.Text != "")
                if (Convert.ToInt32(Day.Text) > 31)
                {
                    System.Windows.Forms.MessageBox.Show("31이하의 숫자를 적으시오");
                    Day.Text = "";
                }
        }

        private void Hour_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Hour.Text != "")
                if (Convert.ToInt32(Hour.Text) > 23)
                {
                    System.Windows.Forms.MessageBox.Show("24미만의 숫자를 적으시오");
                    Hour.Text = "";
                }
        }

        private void Min_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Min.Text != "")
                if (Convert.ToInt32(Min.Text) > 59)
                {
                    System.Windows.Forms.MessageBox.Show("60미만의 숫자를 적으시오");
                    Min.Text = "";
                }
        }

        private void Sec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Sec.Text != "")
                if (Convert.ToInt32(Sec.Text) > 59)
                {
                    System.Windows.Forms.MessageBox.Show("60미만의 숫자를 적으시오");
                    Sec.Text = "";
                }
        }

        private void Playbtn_Click(object sender, RoutedEventArgs e)
        {
            if (Year.Text == "")
                System.Windows.Forms.MessageBox.Show("년도를 설정해주시오.");
            else if (Month.Text == "")
                System.Windows.MessageBox.Show("월을 설정해주시오.");
            else if (Day.Text == "")
                System.Windows.MessageBox.Show("일을 설정해주시오.");
            else if (Hour.Text == "")
                System.Windows.MessageBox.Show("시를 설정해주시오.");
            else if (Min.Text == "")
                System.Windows.MessageBox.Show("분을 설정해주시오.");
            else if (Sec.Text == "")
                System.Windows.MessageBox.Show("초를 설정해주시오.");
            else
            {
                Play.Start();
                Prebtn.IsEnabled = false;
                Prebtn.IsEnabled = false;
                stopbtn.IsEnabled = true;
                Playbtn.IsEnabled = false;
            }
        }

        private void stopbtn_Click(object sender, RoutedEventArgs e)
        {
            Play.Stop();
            Prebtn.IsEnabled = true;
            Prebtn.IsEnabled = true;
            stopbtn.IsEnabled = false;
            Playbtn.IsEnabled = true;
        }






        #endregion

        #region 화면 크기 맞추기
        //화면 크기에 따른 위치 맞추기
        private void ChangeSize(double width, double height)
        {
            doc.Hide();
            //scale.ScaleX = width / orginalWidth;
            //scale.ScaleY = height / originalHeight;
            double num = width / 16;
            double num2 = height / 20;
            List<Image> Img = new List<Image>();
            List<System.Windows.Controls.Label> lbg = new List<System.Windows.Controls.Label>();

            if (GUITrain.Count != 0)
                for (int i = 0; i < GUITrain.Count; i++)
                {
                    GUITrain[i].Width = num * 0.225;
                    GUITrain[i].Height = num2 * 0.225;
                    // GUITrain[i].LayoutTransform = scale;
                }
            for (int i = 0; i < Main_gird.Children.Count; i++)
            {
                if (Main_gird.Children[i].GetType().Name == "Image")
                {
                    Image img = (Image)Main_gird.Children[i];
                    //  img.LayoutTransform = scale;
                    Img.Add(img);
                }
                else if (Main_gird.Children[i].GetType().Name == "Label")
                {
                    System.Windows.Controls.Label lb = (System.Windows.Controls.Label)Main_gird.Children[i];
                    // lb.LayoutTransform = scale;
                    lbg.Add(lb);
                }
            }

            Logo.Margin = new Thickness(num * 18.5, num2 * 0.3, 0, 0);

            //메뉴바 위치 및 크기
            menu.Height = num2 * 1.2;
            menu.Width = width;
            menu.Margin = new Thickness(0, 0, 0, num2 * 18.8);

            //데이터 그리드뷰 위치F
            LogDataGrid.Height = num2 * 3.4;
            LogDataGrid.Width = width * 0.992;
            LogDataGrid.Margin = new Thickness(0, num2 * 15.8, 0, 0);

            //Pre, Next 버튼 위치 및 크기
            Prebtn.Height = num2 * 0.4;
            Prebtn.Width = num * 0.25;
            Prebtn.Margin = new Thickness(num * 5.8, num2 * 0.7, 0, 0);
            Nextbtn.Height = num2 * 0.4;
            Nextbtn.Width = num * 0.25;
            Nextbtn.Margin = new Thickness(num * 6.1, num2 * 0.7, 0, 0);

            //play,stop 버튼 위치 및 크기
            Playbtn.Height = num2 * 0.4;
            Playbtn.Width = num * 0.25;
            Playbtn.Margin = new Thickness(num * 5.8, num2 * 1.2, 0, 0);
            stopbtn.Height = num2 * 0.4;
            stopbtn.Width = num * 0.25;
            stopbtn.Margin = new Thickness(num * 6.1, num2 * 1.2, 0, 0);

            //시간데이터 위치 위치 및 크기
            Year.Height = num2 * 0.4;
            Year.Width = num * 0.35;
            Year.Margin = new Thickness(num * 2, num2 * 0.7, 0, 0);
            Yearbk.Margin = new Thickness(num * 2.4, num2 * 0.7, 0, 0);
            Yearbk.FontSize = height * 0.0126;
            //Yearbk.Width = num;

            Month.Height = num2 * 0.4;
            Month.Width = num * 0.35;
            Month.Margin = new Thickness(num * 2.6, num2 * 0.7, 0, 0);
            Monbk.Margin = new Thickness(num * 3, num2 * 0.7, 0, 0);
            Monbk.FontSize = height * 0.0126;
            //Monbk.Width = num;

            Day.Height = num2 * 0.4;
            Day.Width = num * 0.35;
            Day.Margin = new Thickness(num * 3.2, num2 * 0.7, 0, 0);
            Daybk.Margin = new Thickness(num * 3.6, num2 * 0.7, 0, 0);
            Daybk.FontSize = height * 0.0126;
            // Daybk.Width = num;

            Hour.Height = num2 * 0.4;
            Hour.Width = num * 0.35;
            Hour.Margin = new Thickness(num * 4, num2 * 0.7, 0, 0);
            Hourbk.Margin = new Thickness(num * 4.4, num2 * 0.7, 0, 0);
            Hourbk.FontSize = height * 0.0126;
            //Hourbk.Width = num;

            Min.Height = num2 * 0.4;
            Min.Width = num * 0.35;
            Min.Margin = new Thickness(num * 4.6, num2 * 0.7, 0, 0);
            Minbk.Margin = new Thickness(num * 5, num2 * 0.7, 0, 0);
            Minbk.FontSize = height * 0.0126;
            //Minbk.Width = num;

            Sec.Height = num2 * 0.4;
            Sec.Width = num * 0.35;
            Sec.Margin = new Thickness(num * 5.2, num2 * 0.7, 0, 0);
            Secbk.Margin = new Thickness(num * 5.6, num2 * 0.7, 0, 0);
            Secbk.FontSize = height * 0.0126;
            //Secbk.Width = num;

            //팝업 크기
            doc.Width = num * 2.5;
            doc.Height = num2 * 2.6;

            double lbHnum = 0;
            double lbWnum = 0;
            for (int i = 0; i < doc.Labels.Children.Count; i++)
            {

                if (i < 4)
                {
                    lbHnum += doc.Height * 0.15;
                    System.Windows.Controls.Label lbs = (System.Windows.Controls.Label)doc.Labels.Children[i];
                    lbs.FontSize = doc.Height * 0.076;
                    lbs.Margin = new Thickness(doc.Width * 0.10, lbHnum, 0, 0);
                    if (i == 3)
                    {
                        lbHnum = 0;
                        lbWnum += doc.Width * 0.35;
                    }
                }
                else if (i < 8)
                {
                    lbHnum += doc.Height * 0.15;
                    System.Windows.Controls.Label lbs = (System.Windows.Controls.Label)doc.Labels.Children[i];
                    lbs.FontSize = doc.Height * 0.076;
                    lbs.Margin = new Thickness(lbWnum, lbHnum, 0, 0);
                }

            }
            //라인 위치
            for (int i = 0; i < Line.Count; i++)
            {
                Line[i].Width = width;
                Line[i].Height = height;
                if (i == 0)
                {
                    Line[i].X1 = num * 0.35;
                    Line[i].X2 = num * 15.65;
                    Line[i].Y1 = num2 * 2.6;
                    Line[i].Y2 = num2 * 2.6;
                }
                else if (i == 1)
                {
                    Line[i].X1 = num * 0.35;
                    Line[i].X2 = num * 14.7;
                    Line[i].Y1 = num2 * 4.6;
                    Line[i].Y2 = num2 * 4.6;
                }
                else if (i == 2)
                {
                    Line[i].X1 = Line[0].X2;
                    Line[i].X2 = Line[0].X2;
                    Line[i].Y1 = Line[0].Y2;
                    Line[i].Y2 = num2 * 13.6;
                }
                else if (i == 3)
                {
                    Line[i].X1 = Line[1].X2;
                    Line[i].X2 = Line[1].X2;
                    Line[i].Y1 = Line[1].Y2;
                    Line[i].Y2 = num2 * 11.6;
                }
                else if (i == 4)
                {
                    Line[i].X1 = num * 14.7;
                    Line[i].X2 = num * 0.35;
                    Line[i].Y1 = num2 * 11.6;
                    Line[i].Y2 = num2 * 11.6;
                }
                else if (i == 5)
                {
                    Line[i].X1 = num * 15.65;
                    Line[i].X2 = num * 0.35;
                    Line[i].Y1 = num2 * 13.6;
                    Line[i].Y2 = num2 * 13.6;
                }
                else if (i == 6)
                {
                    Line[i].X1 = num * 10.6;
                    Line[i].X2 = num * 10.6;
                    Line[i].Y1 = num2 * 13.6;
                    Line[i].Y2 = num2 * 15.1;
                }
                else if (i == 7)
                {
                    Line[i].X1 = num * 10.6;
                    Line[i].X2 = num * 7.6;
                    Line[i].Y1 = Line[i - 1].Y2;
                    Line[i].Y2 = Line[i - 1].Y2;
                }
                else if (i == 8)
                {
                    Line[i].X1 = num * 10.6;
                    Line[i].X2 = num * 10.6;
                    Line[i].Y1 = num2 * 11.6;
                    Line[i].Y2 = num2 * 10.1;
                }
                else if (i == 9)
                {
                    Line[i].X1 = num * 10.6;
                    Line[i].X2 = num * 7.6;
                    Line[i].Y1 = Line[i - 1].Y2;
                    Line[i].Y2 = Line[i - 1].Y2;

                }
            }

            // 버튼 이미지 위치
            double x = 0;
            double x3 = ((num * 0.95) * 15);
            for (int i = 0; i < CkButton.Count; i++)
            {
                CkButton[i].Width = num * 0.6;
                CkButton[i].Height = num2 * 0.6;
                // CkButton[i].LayoutTransform = scale;
                if (i < 15)
                {
                    x += num * 0.95;
                    CkButton[i].Margin = new Thickness(x, num2 * 2.26, 0, 0);
                    if (i > 0)
                    {
                        lbg[i + 18].Margin = new Thickness(x - num * 0.35, num2 * 3.26, 0, 0);
                        lbg[i + 18 + 28].Margin = new Thickness(x - num * 0.35, num2 * 3.76, 0, 0);



                    }
                }
                else if (i < 30)
                {
                    if (i != 15)
                        x -= num * 0.95;
                    CkButton[i].Margin = new Thickness(x, num2 * 13.26, 0, 0);
                    if (i < 29)
                    {
                        lbg[i + 18].Margin = new Thickness(x - num * 0.35, num2 * 12.26, 0, 0);
                        lbg[i + 18 + 28].Margin = new Thickness(x - num * 0.35, num2 * 12.76, 0, 0);
                    }
                }
                else if (i < 45)
                {
                    if (i != 30)
                        x += num * 0.95;
                    CkButton[i].Margin = new Thickness(x, num2 * 4.26, 0, 0);


                }
                else if (i < 60)
                {
                    if (i != 45)
                        x -= num * 0.95;
                    CkButton[i].Margin = new Thickness(x, num2 * 11.26, 0, 0);

                }
            }

            //이미지 위치
            for (int i = 0; i < Img.Count; i++)
            {
                Img[i].Width = num * 1;
                Img[i].Height = num2 * 1;
                if (i == 0) // 로고이미지
                {
                    Img[i].Width = num * 2.5;
                    Img[i].Height = num2 * 2.5;
                    Img[i].Margin = new Thickness(num * 6.5, num2 * 0.5, 0, 0);
                }
                else if (i == 1) // 동화역
                    Img[i].Margin = new Thickness(0, num2 * 1.64, 0, 0);
                else if (i == 2)//화살표
                    Img[i].Margin = new Thickness(num * 0.372, num2 * 0.5, 0, 0);
                else if (i == 3) //지정천교
                    Img[i].Margin = new Thickness(num * 2.30, num2 * 1.7, 0, 0);
                else if (i == 4)//소막골 터널
                    Img[i].Margin = new Thickness(num * 4.2, num2 * 1.5, 0, 0);
                else if (i == 5)//보통 고가
                    Img[i].Margin = new Thickness(num * 7.05, num2 * 1.7, 0, 0);
                else if (i == 6)//광터 고가
                    Img[i].Margin = new Thickness(num * 9.9, num2 * 1.7, 0, 0);
                else if (i == 7)//지장물1
                    Img[i].Margin = new Thickness(num * 11.8, num2 * 1.5, 0, 0);
                else if (i == 8)//지장물2
                    Img[i].Margin = new Thickness(num * 13.75, num2 * 1.5, 0, 0);
                else if (i == 9)//만종터널
                    Img[i].Margin = new Thickness(num * 13.7, num2 * 10.5, 0, 0);
                else if (i == 10)//화살표2
                    Img[i].Margin = new Thickness(num * 0.372, num2 * 14.62, 0, 0);
                else if (i == 11)//만종역
                    Img[i].Margin = new Thickness(num * 11.8, num2 * 10.6, 0, 0);
                else if (i == 12)//만종천교
                    Img[i].Margin = new Thickness(num * 9.9, num2 * 10.7, 0, 0);
                else if (i == 13)//호저 터널
                    Img[i].Margin = new Thickness(num * 8, num2 * 10.5, 0, 0);
                else if (i == 14)//가현교
                    Img[i].Margin = new Thickness(num * 6.1, num2 * 10.7, 0, 0);
                else if (i == 15)//점실교
                    Img[i].Margin = new Thickness(num * 4.2, num2 * 10.7, 0, 0);
                else if (i == 16)//원주천교
                    Img[i].Margin = new Thickness(num * 2.3, num2 * 10.7, 0, 0);
                else if (i == 17)//방향성_하행
                    Img[i].Margin = new Thickness(num, num2 * 2.1, 0, 0);
                else if (i == 18)//방향성_하행
                    Img[i].Margin = new Thickness(num * 15.33, num2 * 7.1, 0, 0);
                else if (i == 19)//방향성-하행
                    Img[i].Margin = new Thickness(num, num2 * 13.1, 0, 0);
                else if (i == 20)//방향성_상행
                    Img[i].Margin = new Thickness(num, num2 * 4.1, 0, 0);
                else if (i == 21)//방향성_상행
                    Img[i].Margin = new Thickness(num * 14.38, num2 * 7.1, 0, 0);
                else if (i == 22)//방향성-상행
                    Img[i].Margin = new Thickness(num, num2 * 11.1, 0, 0);
                else if (i == 23)//방향성-하행_2
                    Img[i].Margin = new Thickness(num * 0.8, num2 * 2.1, 0, 0);
                else if (i == 24)//방향성_하행_2
                    Img[i].Margin = new Thickness(num * 15.33, num2 * 6.7, 0, 0);
                else if (i == 25)//방향성-하행_2
                    Img[i].Margin = new Thickness(num * 1.2, num2 * 13.1, 0, 0);
                else if (i == 26)//방향성_상행_2
                    Img[i].Margin = new Thickness(num * 1.2, num2 * 4.1, 0, 0);
                else if (i == 27)//방향성_상행_2
                    Img[i].Margin = new Thickness(num * 14.38, num2 * 7.5, 0, 0);
                else if (i == 28)//방향성-상행_2
                    Img[i].Margin = new Thickness(num * 0.8, num2 * 11.1, 0, 0);
                else if (i == 29)//중간-하행_2
                    Img[i].Margin = new Thickness(num * 9.3, num2 * 14.6, 0, 0);
                else if (i == 30)//중간-하행_2
                    Img[i].Margin = new Thickness(num * 9.5, num2 * 14.6, 0, 0);
                else if (i == 31)//중간-상행_2
                    Img[i].Margin = new Thickness(num * 9.3, num2 * 9.6, 0, 0);
                else if (i == 32)//중간-상행_2
                    Img[i].Margin = new Thickness(num * 9.1, num2 * 9.6, 0, 0);
            }

            //라벨 위치
            for (int i = 0; i < lbg.Count; i++)
            {
                if (i != 1 && i != 11 && i != 18)
                {
                    lbg[i].FontSize = height * 0.01;
                    lbg[i].Width = num;
                }
                else if (i == 18)
                {
                    lbg[i].FontSize = height * 0.0750;
                    lbg[i].Width = num * 12;
                    lbg[i].Height = num2 * 3.5;
                }
                else
                {
                    lbg[i].FontSize = height * 0.0300;
                    lbg[i].Width = num;
                    lbg[i].Height = num2;
                }
                if (i == 0) // 동화역
                {
                    lbg[i].Margin = new Thickness(-num * 0.15, num2 * 2.54, 0, 0);
                }
                else if (i == 1)//화살표
                    lbg[i].Margin = new Thickness(num * 1, num2 * 0.48, 0, 0);
                else if (i == 2) //지정천교
                    lbg[i].Margin = new Thickness(num * 2.1, num2 * 2.54, 0, 0);
                else if (i == 3)//소막골 터널
                    lbg[i].Margin = new Thickness(num * 3.95, num2 * 2.54, 0, 0);
                else if (i == 4)//선로상태
                    lbg[i].Margin = new Thickness(num * 5.45, num2 * 2.74, 0, 0);
                else if (i == 5)//보통 고가
                    lbg[i].Margin = new Thickness(num * 6.85, num2 * 2.54, 0, 0);
                else if (i == 6)//서원주 변전소
                    lbg[i].Margin = new Thickness(num * 8.25, num2 * 2.74, 0, 0);
                else if (i == 7)//광터고가
                    lbg[i].Margin = new Thickness(num * 9.7, num2 * 2.54, 0, 0);
                else if (i == 8)//지장물1
                    lbg[i].Margin = new Thickness(num * 11.65, num2 * 2.54, 0, 0);
                else if (i == 9)//지장물2
                    lbg[i].Margin = new Thickness(num * 13.55, num2 * 2.54, 0, 0);
                else if (i == 10)//만종터널
                    lbg[i].Margin = new Thickness(num * 13.5, num2 * 11.56, 0, 0);
                else if (i == 11)//화살표2
                    lbg[i].Margin = new Thickness(num * 1, num2 * 14.6, 0, 0);
                else if (i == 12)//만종역
                    lbg[i].Margin = new Thickness(num * 11.6, num2 * 11.56, 0, 0);
                else if (i == 13)//만종천교
                    lbg[i].Margin = new Thickness(num * 9.7, num2 * 11.56, 0, 0);
                else if (i == 14)//호저 터널
                    lbg[i].Margin = new Thickness(num * 7.8, num2 * 11.56, 0, 0);
                else if (i == 15)//가현교
                    lbg[i].Margin = new Thickness(num * 5.9, num2 * 11.56, 0, 0);
                else if (i == 16)//점실교
                    lbg[i].Margin = new Thickness(num * 4.0, num2 * 11.56, 0, 0);
                else if (i == 17)//원주천교
                    lbg[i].Margin = new Thickness(num * 2.1, num2 * 11.56, 0, 0);
                else if (i == 18)//제목 라벨
                    lbg[i].Margin = new Thickness(num * 2, num2 * 6.3, 0, 0);

            }
        }
    }
    #endregion

    //로그데이터를 넘기기위한 클래스
    public class Log
    {
        public String Index { get; set; }
        public String Tag { get; set; }
        public String UpDown { get; set; }
        public String Space { get; set; }
        public String Position { get; set; }
        public String Date { get; set; }
    }

}
