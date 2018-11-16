using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ProtocolTest
{
    class clsHeader2
    {
        Function Fun = new Function();
        DBConnect DbCon = new DBConnect();
        DBConnect DbCon2 = new DBConnect();
        public static bool Demo = false;
        DateTime comdate;
        string DBName = string.Empty;
        string Sstart = string.Empty;
        string Send = string.Empty;
        string MStart = string.Empty;
        string Mend = string.Empty;
        string Lstart = string.Empty;
        string Lend = string.Empty;
        bool startck = false;
        bool endck = false;
        bool Middck = false;
        int Ss = 0;
        int Ls = 0;
        int Ms = 0;
        int Ss2 = 0;
        int Ls2 = 0;
        int Ms2 = 0;
        int count = 0;
        int count1 = 0;
        int count2 = 0;
        bool tk = true;
        bool tk1 = true;
        int fourcount = 0;
        int Other = 0;
        int updateCkcount = 0;
        bool T1 = false;
        bool T2 = false;
        bool T3 = false;
        System.Timers.Timer timer = new System.Timers.Timer();
        //System.Timers.Timer timer2 = new System.Timers.Timer();
        #region 데이터 입력 함수 부분
        public clsHeader2()
        {
            DBName = DbCon.TextInputData();
            DbCon2.TextInputData2();
            timer.Interval = 10000;
            timer.Elapsed += new ElapsedEventHandler(PositionClear);
            timer.Start();
            //timer2.Interval = 1000;
            //timer2.Elapsed += new ElapsedEventHandler(TimerCount2);
            //timer2.Start();
        }
        #endregion

        #region 열차수 세기
        public int PositionTF(int PrePosition, DataTable RootDB, DateTime PreDate, int PreID) //  이전 Position 기록값을 통하여 기차 수 알아내기 ( 기존 로그 DB 가 없을 시 사용 )
        {


            string[] Startend = Fun.Split(@"..\..\ZoneData.txt");

            int count = 0; // 기차 수
            RootDB = DbCon.dbConn("select * from alarm_tab where  ID > " + PreID + "order by ALARM_DATETIME_UTC asc", DBName);
            Fun.insertTrainData(ref count, PreID);

            return count;
        }
        #endregion
        public void ChangeString(DataTable RootDb, int num, int Position, ref bool Positionck, ref string Space, ref string DateArr, int Strat, int End, List<string> TrainTag)
        {
            if (Strat < Position && Position < End && Positionck)
            {
                // Positionck = false;
                TrainTag.Add(RootDb.Rows[num].ItemArray[3].ToString());
                Space += RootDb.Rows[num].ItemArray[9].ToString() + "/";
                DateArr += RootDb.Rows[num].ItemArray[6].ToString() + "/";
            }
        }
        #region 열차 위치 변화값 전달 
        public int[] ChangeCk(DataTable PositionTb, DataTable RootDb, ref string[] Date, List<string> TrainTag)
        {
            int num = 0;
            string Space = string.Empty; // Space ID 를 얻기위한 문자열
            string DateArr = string.Empty; // Date를 얻기위한 문자열

            string[] Startend = Fun.Split(@"..\..\ZoneData.txt");
            bool[] Positionck = new bool[Startend.Length / 2]; // Root DB안에 같은 Position 값이 들어가 있기 때문에 판별용도
            for (int j = 0; j < Positionck.Length; j++)
                Positionck[j] = true;

            int strint = 0;
            int endint = 1;
            while (true)
            {
                if (PositionTb.Rows[0].ItemArray[2].ToString() == RootDb.Rows[num].ItemArray[0].ToString()) // 기존 Position 기록과 RootDB의 기록이 같을 경우에만 멈춤
                {
                    break;
                }
                else
                {
                    int Position = Convert.ToInt32(RootDb.Rows[num].ItemArray[9]);
                    for (int i = 0; i < Positionck.Length; i++)
                    {
                        ChangeString(RootDb, num, Position, ref Positionck[i], ref Space, ref DateArr, Convert.ToInt32(Startend[strint]), Convert.ToInt32(Startend[endint]), TrainTag);
                        strint += 2;
                        endint += 2;
                    }
                    num++;
                    strint = 0;
                    endint = 1;
                }
            }

            if (DateArr.Length > 0)
            {
                DateArr = DateArr.Remove(DateArr.Length - 1, 1);// 마지막 '/' 제거
                Space = Space.Remove(Space.Length - 1, 1); // 마지막 '/' 제거
                string[] SpaceIDstring = Space.Split('/'); // 바뀐 지점의 ID를 받기 위한 배열
                string[] Datestring = DateArr.Split('/');// 바뀐 지점의 Date를 받기 위한 배열
                Date = Datestring; // 값 전달
                int[] SpaceID = new int[SpaceIDstring.Length];
                for (int i = 0; i < SpaceIDstring.Length; i++) // 바뀐 지점의 ID를 int형으로 변환
                {
                    SpaceID[i] = Convert.ToInt32(SpaceIDstring[i]);
                }
                return SpaceID;
            }
            else
            {
                return null;
            }
        }
        #endregion


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

        public void DataUpdate(int[] pos, string[] ChDate, string PreID, List<string> TrainTag) // root의 시간 데이터도 넘기기
        {
            DateTime[] Date = new DateTime[ChDate.Length];

            for (int i = 0; i < ChDate.Length; i++)
            {
                Date[i] = Convert.ToDateTime(ChDate[i]);
                Date[i] = Date[i].AddHours(9);
            }
            Array.Reverse(pos);
            Array.Reverse(Date);
            TrainTag.Reverse();
            int[] PosID = PositionToID(pos); // 변화된 POS의 ID 값
            DataTable TrainData = DbCon2.dbConn("select * from TrainData ", "ARTech");

            // 새로 들어온 열차 확인
            DataTable sdata;
            DataTable mdata;
            DataTable ldata;
            string STag = string.Empty;
            string MTag = string.Empty;
            string LTag = string.Empty;

            string[] Startend = Fun.Split(@"..\..\ZoneData.txt");
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
                                Ss2 = (int)sdata.Rows[j].ItemArray[0];
                        }

                        if (Ss != 0 && Ss2 != 0)
                        {

                            if (Ss < Ss2)
                            {

                                startck = true;
                                for (int i = 0; i < TrainData.Rows.Count; i++)
                                {

                                    if ((int)TrainData.Rows[i].ItemArray[2] == 1 && Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 1)
                                    {
                                        startck = true;
                                    }
                                    if ((int)TrainData.Rows[i].ItemArray[2] == 2 && (Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 1 || Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 2 || Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 3 || Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 4))
                                    {
                                        startck = false;
                                        Ss = 0;
                                        Ss2 = 0;
                                        count = 0;
                                    }
                                }
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
                            Lstart = ldata.Rows[j].ItemArray[2].ToString().Trim();
                        }
                        else if (Ls != 0 && Ls2 == 0)// && Lstart == ldata.Rows[j].ItemArray[2].ToString().Trim() && Ls > (int)ldata.Rows[j].ItemArray[0])
                        {
                            if (Ls > (int)ldata.Rows[j].ItemArray[0])
                                Ls2 = (int)ldata.Rows[j].ItemArray[0];
                        }

                        if (Ls != 0 && Ls2 != 0)
                        {
                            endck = true;
                            for (int i = 0; i < TrainData.Rows.Count; i++)
                            {

                                if ((int)TrainData.Rows[i].ItemArray[2] == 2 && Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 29)
                                {
                                    endck = true;
                                }

                                if ((int)TrainData.Rows[i].ItemArray[2] == 1 && (Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 29 || Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 28))
                                {
                                    endck = false;
                                    Ls = 0;
                                    Ls2 = 0;
                                    count1 = 0;
                                }

                            }
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
                                        for (int i = 0; i < TrainData.Rows.Count; i++)
                                        {
                                            if (Convert.ToInt32(TrainData.Rows[i].ItemArray[2]) == 1 && (Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 19 || Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 18 || Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 17 || Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) == 16))
                                            {
                                                Middck = false;
                                                Ms = 0;
                                                Ms2 = 0;
                                                count2 = 0;
                                            }

                                        }

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

            for (int i = 0; i < TrainData.Rows.Count; i++)
            {
                if (TrainData.Rows[i].ItemArray[2].ToString() == "1") // 상행일 경우
                {
                    int PrePosid = (int)TrainData.Rows[i].ItemArray[3];
                    int PrePos = (int)TrainData.Rows[i].ItemArray[4];
                    if (pos.Length > 0)
                        for (int j = 0; j < pos.Length; j++)
                        {
                            //if (TrainTag[j].ToString().Trim() == TrainData.Rows[i].ItemArray[6].ToString().Trim())
                            if (PrePos > pos[j])
                                if (PrePosid - 1 == PosID[j] || PrePosid - 2 == PosID[j] || PrePosid - 3 == PosID[j]) // 상행의 경우
                                {
                                    bool Traking = true;
                                    if (PrePosid == 19 && (int)TrainData.Rows[i].ItemArray[2] == 1)
                                    {
                                        DataTable TrainData2 = DbCon2.dbConn("select * from TrainData ", "ARTech");
                                        for (int z = 0; z < TrainData2.Rows.Count; z++)
                                        {
                                            if (((int)TrainData2.Rows[z].ItemArray[3] == 18 || (int)TrainData2.Rows[z].ItemArray[3] == 17 || (int)TrainData2.Rows[z].ItemArray[3] == 16) && (int)TrainData2.Rows[z].ItemArray[2] == 1)
                                                Traking = false;


                                        }
                                    }

                                    if ((PrePosid == 19) && (PosID[j] == 16 || PosID[j] == 15 || PosID[j] == 17))
                                        Traking = false;

                                    if (PrePosid == 28 && (int)TrainData.Rows[i].ItemArray[2] == 1)
                                    {
                                        DataTable TrainData2 = DbCon2.dbConn("select * from TrainData ", "ARTech");
                                        for (int z = 0; z < TrainData2.Rows.Count; z++)
                                            if (((int)TrainData2.Rows[z].ItemArray[3] == 27 || (int)TrainData2.Rows[z].ItemArray[3] == 26 || (int)TrainData2.Rows[z].ItemArray[3] == 25) && (int)TrainData2.Rows[z].ItemArray[2] == 1)
                                                Traking = false;

                                    }
                                    DataTable TrainData3 = DbCon2.dbConn("select * from TrainData ", "ARTech");
                                    for (int z = 0; z < TrainData3.Rows.Count; z++)
                                    {
                                        if (TrainTag[j] == "Other")
                                            if ((int)TrainData3.Rows[z].ItemArray[2] == 1 && TrainData3.Rows[z].ItemArray[6].ToString().Trim() == "KTX")
                                            {
                                                int Ktxid = (int)TrainData3.Rows[z].ItemArray[3];
                                                if (PrePosid - 4 > 0)
                                                    if (PrePosid - 1 == Ktxid && PrePosid - 2 == Ktxid && PrePosid - 3 == Ktxid && PrePosid - 4 == Ktxid)
                                                    {
                                                        Traking = false;
                                                    }
                                            }
                                        //1 2번 포인트 크로스 방지(하행 )
                                        if (PrePosid == 4 && (PosID[j] == 1 || PosID[j] == 2))
                                            Traking = false;
                                    }
                                    if (Traking)
                                    {
                                        if (PrePosid - 3 == PosID[j])
                                        {
                                            DataTable Predate = DbCon2.dbConn("  select * from Logdata where UpDown ='상행' and Space ='" + Fun.reutrnPos(PrePosid) + "' order by [Index] desc ", "ARTech");
                                            DateTime DD = (DateTime)Predate.Rows[0].ItemArray[5];
                                            double Tim = 0;
                                            TimeSpan tt = Date[j].Subtract(DD);
                                            if (tt.TotalSeconds > 0)
                                                Tim = tt.TotalSeconds / 2.0;
                                            else if (Tim < 0)
                                                Tim = 0;
                                            DbCon2.insertqry("insert into LogData Values ('" + TrainData.Rows[i].ItemArray[6] + "','상행','" + Fun.reutrnPos(PrePosid - 1) + "'," + Fun.position(PrePosid - 1) + ",'" + Date[j].AddSeconds(-Tim * 1.5).ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                                            DbCon2.insertqry("insert into LogData Values ('" + TrainData.Rows[i].ItemArray[6] + "','상행','" + Fun.reutrnPos(PrePosid - 2) + "'," + Fun.position(PrePosid - 2) + ",'" + Date[j].AddSeconds(-Tim).ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");

                                            DbCon2.Updateqry("Update TrainData Set Jump = 2 where SpaceID = " + PrePosid + " and UpDown = 1", "ARTech");

                                        }
                                        else if (PrePosid - 2 == PosID[j])
                                        {
                                            DataTable Predate = DbCon2.dbConn("  select * from Logdata where UpDown ='상행' and Space ='" + Fun.reutrnPos(PrePosid) + "' order by [Index] desc ", "ARTech");
                                            DateTime DD = (DateTime)Predate.Rows[0].ItemArray[5];
                                            double Tim = 0;
                                            TimeSpan tt = Date[j].Subtract(DD);
                                            if (tt.TotalSeconds > 0)
                                                Tim = tt.TotalSeconds / 2.0;
                                            else if (Tim < 0)
                                                Tim = 0;
                                            DbCon2.insertqry("insert into LogData Values ('" + TrainData.Rows[i].ItemArray[6] + "','상행','" + Fun.reutrnPos(PrePosid - 1) + "'," + Fun.position(PrePosid - 1) + ",'" + Date[j].AddSeconds(-Tim).ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                                            DbCon2.Updateqry("Update TrainData Set Jump = 1 where SpaceID = " + PrePosid + " and UpDown = 1", "ARTech");

                                        }

                                        DbCon2.Updateqry("Update TrainData Set Pos=" + pos[j] + ", SpaceID = " + PosID[j] + ", TimeCk = 0 , DateTime ='" + Date[j].ToString("yyyy-MM-dd HH:mm:ss") + "' where SpaceID = " + PrePosid + " and UpDown = 1", "ARTech");
                                        DbCon2.insertqry("insert into LogData Values ('" + TrainData.Rows[i].ItemArray[6] + "','상행','" + Fun.reutrnPos(PosID[j]) + "'," + pos[j] + ",'" + Date[j].ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                                        PrePos = pos[j];
                                        PrePosid = PosID[j];
                                        break;

                                    }


                                }
                                else if (PrePosid == PosID[j])
                                {
                                    // if (TrainTag[j].ToString().Trim() == TrainData.Rows[i].ItemArray[6].ToString().Trim())
                                    if (PrePos > pos[j])
                                    {
                                        //   DbCon2.Updateqry("Update TrainData Set Pos = " + pos[j] + " , TimeCk = 0 , DateTime ='" + Date[j].ToString("yyyy-MM-dd HH:mm:ss") + "'  where SpaceID = " + PrePosid + " and UpDown = 1", "ARTech");
                                        PrePos = pos[j];

                                    }
                                }
                        }

                    //if((int)TrainData.Rows[i].ItemArray[3] == 15 && (int)TrainData.Rows[i].ItemArray[2] == 1)
                    //{
                    //    DbCon.Updateqry("Update TrainData Set Pos=" + 2602 + ", SpaceID = " + 12 + ", TimeCk = 0 , DateTime ='" + Date[0].ToString("yyyy-MM-dd HH:mm:ss") + "' where SpaceID = " + Convert.ToInt32(TrainData.Rows[i].ItemArray[3]) + " and UpDown = 1", "ARTech");
                    //    DbCon.insertqry("insert into LogData Values ('" + TrainData.Rows[i].ItemArray[6] + "','상행','" + 12 + "'," + 2602 + ",'" + Date[0].ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                    //    PrePos = 2602;
                    //}
                }
                else if (TrainData.Rows[i].ItemArray[2].ToString() == "2") //하행일 경우
                {
                    int PrePosid = (int)TrainData.Rows[i].ItemArray[3];
                    int PrePos = (int)TrainData.Rows[i].ItemArray[4];
                    bool tt = true; // 업데이트를 막기 위한 변수
                    if (pos.Length > 0)
                        for (int j = 0; j < pos.Length; j++)
                        {
                            //만종역에서 6000알람이땡기는거 방지
                            DataTable TrainData4 = DbCon2.dbConn("select * from TrainData ", "ARTech");
                            for (int z = 0; z < TrainData4.Rows.Count; z++)
                            {
                                if ((PrePosid == 17 || PrePosid == 18) && (int)TrainData4.Rows[z].ItemArray[3] != 19)
                                {
                                    tt = false;
                                    if (5300 < pos[j] && pos[j] < 5700 && T2)
                                        T3 = true;
                                    if (4900 < pos[j] && pos[j] < 5300 && T1)
                                        T2 = true;
                                    if (4500 < pos[j] && pos[j] < 4900)
                                        T1 = true;

                                    if (T3)
                                    {
                                        tt = true;
                                    }
                                }
                            }
                            if (PrePos < pos[j])
                                if (PrePosid + 1 == PosID[j] || PrePosid + 2 == PosID[j] || PrePosid + 3 == PosID[j]) // 하행의 경우
                                {

                                    DataTable MidData = DbCon2.dbConn("select * from TrainData ", "ARTech");

                                    //4번쨰 포인트부터 열차 구분을 하기위한 조건식
                                    if (PrePosid == 3 && (PosID[j] == 4 || PosID[j] == 5 || PosID[j] == 6))
                                    {
                                        tt = false;
                                        if (PosID[j] == 4 && pos[j] > 900)
                                            if ("Other" == TrainTag[j].ToString().Trim())
                                                Other++;
                                        if (PosID[j] == 5 || PosID[j] == 6)
                                            if (updateCkcount > 0)
                                            {
                                                tt = true;
                                            }
                                            else
                                                updateCkcount++;
                                    }

                                    //새로 생긴 열차가 차종 구분을 위해 3번 지점으로 가게하기 위한 식
                                    if (PrePosid == 1 && (PosID[j] == 2 || PosID[j] == 4))
                                        tt = false;

                                    //1,2번 포인트가 4번 포인트 따라가지 않도록
                                    DataTable TrainData3 = DbCon2.dbConn("select * from TrainData ", "ARTech");
                                    for (int z = 0; z < TrainData3.Rows.Count; z++)
                                    {
                                        //초입에서 끌고가는거 방지
                                        if (((int)TrainData3.Rows[z].ItemArray[3] == 4) && (int)TrainData3.Rows[z].ItemArray[2] == 1)
                                            if (PrePosid == 1 || PrePosid == 2)
                                                tt = false;

                                        //6000미터 알람 울려도 만종역에 있는 열차를 못끌고 가게 하기 위해
                                        if ((PrePosid == 16 || PrePosid == 17 || PrePosid == 18) && (int)TrainData3.Rows[z].ItemArray[3] == 19)
                                            tt = false;


                                    }
                                    if (tt)
                                    {

                                        if (PrePosid == 3)
                                        {
                                            String Tagz = "KTX";
                                            if (Other > 2)
                                                Tagz = "Other";
                                            DbCon2.Updateqry("Update TrainData Set TrainTag = '" + Tagz + "', Pos=" + 1464 + ", SpaceID = " + 4 + ", TimeCk = 0 , DateTime ='" + Date[j].ToString("yyyy-MM-dd HH:mm:ss") + "' where SpaceID = " + PrePosid + " and UpDown = 2", "ARTech");
                                            DbCon2.insertqry("insert into LogData Values ('" + Tagz + "','하행','" + Fun.reutrnPos(4) + "'," + 1464 + ",'" + Date[j].ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                                            Other = 0;
                                            updateCkcount = 0;
                                            PrePos = pos[j];
                                            PrePosid = PosID[j];
                                            break;
                                        }
                                        else
                                        {
                                            if (PrePosid == 17 || PrePosid == 18)
                                            {
                                                T1 = false;
                                                T2 = false;
                                                T3 = false;
                                            }
                                            if (PrePosid + 3 == PosID[j])
                                            {
                                                DataTable Predate = DbCon2.dbConn("  select * from Logdata where UpDown ='하행' and Space ='" + Fun.reutrnPos(PrePosid) + "' order by [Index] desc ", "ARTech");
                                                DateTime DD = (DateTime)Predate.Rows[0].ItemArray[5];
                                                double Tim = 0;
                                                TimeSpan Times = Date[j].Subtract(DD);
                                                if (Times.TotalSeconds > 0)
                                                    Tim = Times.TotalSeconds / 2.0;
                                                else if (Tim < 0)
                                                    Tim = 0;
                                                DbCon2.insertqry("insert into LogData Values ('" + TrainData.Rows[i].ItemArray[6] + "','하행','" + Fun.reutrnPos(PrePosid + 1) + "'," + Fun.position(PrePosid + 1) + ",'" + Date[j].AddSeconds(-Tim * 1.5).ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                                                DbCon2.insertqry("insert into LogData Values ('" + TrainData.Rows[i].ItemArray[6] + "','하행','" + Fun.reutrnPos(PrePosid + 2) + "'," + Fun.position(PrePosid + 2) + ",'" + Date[j].AddSeconds(-Tim).ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                                                DbCon2.Updateqry("Update TrainData Set Jump = 2 where SpaceID = " + PrePosid + " and UpDown = 2", "ARTech");

                                            }
                                            else if (PrePosid + 2 == PosID[j])
                                            {
                                                DataTable Predate = DbCon2.dbConn("  select * from Logdata where UpDown ='하행' and Space ='" + Fun.reutrnPos(PrePosid) + "' order by [Index] desc ", "ARTech");
                                                DateTime DD = (DateTime)Predate.Rows[0].ItemArray[5];
                                                double Tim = 0;
                                                TimeSpan Times = Date[j].Subtract(DD);
                                                if (Times.TotalSeconds > 0)
                                                    Tim = Times.TotalSeconds / 2.0;
                                                else if (Tim < 0)
                                                    Tim = 0;
                                                DbCon2.insertqry("insert into LogData Values ('" + TrainData.Rows[i].ItemArray[6] + "','하행','" + Fun.reutrnPos(PrePosid + 1) + "'," + Fun.position(PrePosid + 1) + ",'" + Date[j].AddSeconds(-Tim).ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                                                DbCon2.Updateqry("Update TrainData Set Jump = 1 where SpaceID = " + PrePosid + " and UpDown = 2", "ARTech");

                                            }

                                            DbCon2.Updateqry("Update TrainData Set Pos=" + pos[j] + ", SpaceID = " + PosID[j] + ", TimeCk = 0 , DateTime ='" + Date[j].ToString("yyyy-MM-dd HH:mm:ss") + "' where SpaceID = " + PrePosid + " and UpDown = 2", "ARTech");
                                            DbCon2.insertqry("insert into LogData Values ('" + TrainData.Rows[i].ItemArray[6] + "','하행','" + Fun.reutrnPos(PosID[j]) + "'," + pos[j] + ",'" + Date[j].ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
                                            PrePos = pos[j];
                                            PrePosid = PosID[j];
                                            break;
                                        }
                                       

                                    }

                                }
                                else if (PrePosid == PosID[j])
                                {
                                    //  if (TrainTag[j].ToString().Trim() == TrainData.Rows[i].ItemArray[6].ToString().Trim())
                                    if (PrePos < pos[j])
                                    {
                                        //   DbCon2.Updateqry("Update TrainData Set Pos = " + pos[j] + ", TimeCk = 0 , DateTime ='" + Date[j].ToString("yyyy-MM-dd HH:mm:ss") + "' where UpDown = 2 and SpaceID = " + PrePosid, "ARTech");
                                        PrePos = pos[j];



                                    }
                                }
                        }
                }

            }





            if (endck)
            {
                // DataTable ldata2 = DbCon.dbConn("SELECT DISTINCT  POSITION ,ALARM_DATETIME_UTC,TAG  FROM alarm_tab where id >= " + PreID + " and POSITION > " + Startend[54] + "and POSITION < " + Startend[55] + " order by ALARM_DATETIME_UTC asc ", DBName);
                if (ldata.Rows.Count > 0)
                {
                    DateTime Datez = (DateTime)ldata.Rows[0].ItemArray[1];
                    Datez = Datez.AddHours(9);
                    int posz = (int)ldata.Rows[0].ItemArray[0];
                    string Tagz = (string)ldata.Rows[0].ItemArray[2];
                    DbCon2.insertqry("insert into TrainData Values ('" + Datez.ToString("yyyy-MM-dd HH:mm:ss") + "',0,1,28," + posz + ",0,'" + "KTX" + "')", "ARTech");
                    DbCon2.insertqry("insert into LogData Values ('" + "KTX" + "','상행','원주천교(출)'," + posz + ",'" + Datez.ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
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
                    string Tagz = "Train";//(string)sdata.Rows[0].ItemArray[2];
                    DbCon2.insertqry("insert into TrainData Values ('" + Datez.ToString("yyyy-MM-dd HH:mm:ss") + "',0,2,1," + posz + ",0,'" + Tagz + "')", "ARTech");
                    DbCon2.insertqry("insert into LogData Values ('" + Tagz + "','하행','지정천교(입)'," + posz + ",'" + Datez.ToString("yyyy-MM-dd HH:mm:ss") + "')", "ARTech");
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
                Ms = 0;
                Ms2 = 0;
                count2 = 0;
            }

        }
        #region 위치값을 ID 로 변경
        public int[] PositionToID(int[] Position) // 위치값을 ID 로 변경
        {
            string[] Startend = Fun.Split(@"..\..\ZoneData.txt");
            int[] PosID = new int[Position.Length];
            int num = 0;
            for (int i = 0; i < Position.Length; i++)
            {
                if (Convert.ToInt32(Startend[0]) < Position[i] && Position[i] < Convert.ToInt32(Startend[1]))
                {
                    PosID[num] = 0;
                    num++;
                }
                else if (Convert.ToInt32(Startend[2]) < Position[i] && Position[i] < Convert.ToInt32(Startend[3]))
                {
                    PosID[num] = 1;
                    num++;
                }
                else if (Convert.ToInt32(Startend[4]) < Position[i] && Position[i] < Convert.ToInt32(Startend[5]))
                {
                    PosID[num] = 2;
                    num++;
                }
                else if (Convert.ToInt32(Startend[6]) < Position[i] && Position[i] < Convert.ToInt32(Startend[7]))
                {
                    PosID[num] = 3;
                    num++;
                }
                else if (Convert.ToInt32(Startend[8]) < Position[i] && Position[i] < Convert.ToInt32(Startend[9]))
                {
                    PosID[num] = 4;
                    num++;
                }
                else if (Convert.ToInt32(Startend[10]) < Position[i] && Position[i] < Convert.ToInt32(Startend[11]))
                {
                    PosID[num] = 5;
                    num++;
                }
                else if (Convert.ToInt32(Startend[12]) < Position[i] && Position[i] < Convert.ToInt32(Startend[13]))
                {
                    PosID[num] = 6;
                    num++;
                }
                else if (Convert.ToInt32(Startend[14]) < Position[i] && Position[i] < Convert.ToInt32(Startend[15]))
                {
                    PosID[num] = 7;
                    num++;
                }
                else if (Convert.ToInt32(Startend[16]) < Position[i] && Position[i] < Convert.ToInt32(Startend[17]))
                {
                    PosID[num] = 8;
                    num++;
                }
                else if (Convert.ToInt32(Startend[18]) < Position[i] && Position[i] < Convert.ToInt32(Startend[19]))
                {
                    PosID[num] = 9;
                    num++;
                }
                else if (Convert.ToInt32(Startend[20]) < Position[i] && Position[i] < Convert.ToInt32(Startend[21]))
                {
                    PosID[num] = 10;
                    num++;
                }
                else if (Convert.ToInt32(Startend[22]) < Position[i] && Position[i] < Convert.ToInt32(Startend[23]))
                {
                    PosID[num] = 11;
                    num++;
                }
                else if (Convert.ToInt32(Startend[24]) < Position[i] && Position[i] < Convert.ToInt32(Startend[25]))
                {
                    PosID[num] = 12;
                    num++;
                }
                else if (Convert.ToInt32(Startend[26]) < Position[i] && Position[i] < Convert.ToInt32(Startend[27]))
                {
                    PosID[num] = 13;
                    num++;
                }
                else if (Convert.ToInt32(Startend[28]) < Position[i] && Position[i] < Convert.ToInt32(Startend[29]))
                {
                    PosID[num] = 14;
                    num++;
                }
                else if (Convert.ToInt32(Startend[30]) < Position[i] && Position[i] < Convert.ToInt32(Startend[31]))
                {
                    PosID[num] = 15;
                    num++;
                }
                else if (Convert.ToInt32(Startend[32]) < Position[i] && Position[i] < Convert.ToInt32(Startend[33]))
                {
                    PosID[num] = 16;
                    num++;
                }
                else if (Convert.ToInt32(Startend[34]) < Position[i] && Position[i] < Convert.ToInt32(Startend[35]))
                {
                    PosID[num] = 17;
                    num++;
                }
                else if (Convert.ToInt32(Startend[36]) < Position[i] && Position[i] < Convert.ToInt32(Startend[37]))
                {
                    PosID[num] = 18;
                    num++;
                }
                else if (Convert.ToInt32(Startend[38]) < Position[i] && Position[i] < Convert.ToInt32(Startend[39]))
                {
                    PosID[num] = 19;
                    num++;
                }
                else if (Convert.ToInt32(Startend[40]) < Position[i] && Position[i] < Convert.ToInt32(Startend[41]))
                {
                    PosID[num] = 20;
                    num++;
                }
                else if (Convert.ToInt32(Startend[42]) < Position[i] && Position[i] < Convert.ToInt32(Startend[43]))
                {
                    PosID[num] = 21;
                    num++;
                }
                else if (Convert.ToInt32(Startend[44]) < Position[i] && Position[i] < Convert.ToInt32(Startend[45]))
                {
                    PosID[num] = 22;
                    num++;
                }
                else if (Convert.ToInt32(Startend[46]) < Position[i] && Position[i] < Convert.ToInt32(Startend[47]))
                {
                    PosID[num] = 23;
                    num++;
                }
                else if (Convert.ToInt32(Startend[48]) < Position[i] && Position[i] < Convert.ToInt32(Startend[49]))
                {
                    PosID[num] = 24;
                    num++;
                }
                else if (Convert.ToInt32(Startend[50]) < Position[i] && Position[i] < Convert.ToInt32(Startend[51]))
                {
                    PosID[num] = 25;
                    num++;
                }
                else if (Convert.ToInt32(Startend[52]) < Position[i] && Position[i] < Convert.ToInt32(Startend[53]))
                {
                    PosID[num] = 26;
                    num++;
                }
                else if (Convert.ToInt32(Startend[54]) < Position[i] && Position[i] < Convert.ToInt32(Startend[55]))
                {
                    PosID[num] = 27;
                    num++;
                }
                else if (Convert.ToInt32(Startend[56]) < Position[i] && Position[i] < Convert.ToInt32(Startend[57]))
                {
                    PosID[num] = 28;
                    num++;
                }

            }
            return PosID;

        }

        #endregion



        private byte CarryNible(int number)
        {
            byte[] css = BitConverter.GetBytes(number);
            return css[0];
        }
        private int CalculateChecksum2(byte[] dataToCalculate)
        {
            int checksum = 0;
            foreach (byte chData in dataToCalculate)
            {
                checksum += chData;
            }
            return checksum;
        }
        #region 데이터 전송 부분
        //데이터 업데이트
        public void DataUpdate()
        {
            DataTable TrainDataDb = DbCon2.dbConn("select * from TrainData order by dateTime desc", "ARTech");
            DataTable RootDb = DbCon.dbConn("select * from alarm_tab order by ID desc", DBName);
            if (RootDb.Rows.Count != 0) // 루트 DB에 값이 있는 경우에만 
            {
                if (TrainDataDb.Rows.Count == 0) // 열차 테이블에 값이 없는 경우( 기록된 열차가 없는 경우 )
                {
                    DataTable Postion = DbCon2.dbConn("select * from Position order by ID desc", "ARTech");
                    if (RootDb.Rows.Count != 0) // 로그 DB의 Posiotion을 이용해 변화를 감지( 로그 DB의 Posiotion에 데이터가 있는 경우에 )
                    {
                        if (Postion.Rows.Count != 0) // 처음 포지션 값이 있을경우
                        {
                            if (Convert.ToInt32(Postion.Rows[0].ItemArray[2]) != Convert.ToInt32(RootDb.Rows[0].ItemArray[0])) // 기존 Position 값과 루트 DB를 비교하여 변화가 생길시 //완
                            {
                                DateTime RootDate = Convert.ToDateTime(RootDb.Rows[0].ItemArray[6]); // 이전 Position 기록용
                                PositionTF(Convert.ToInt32(Postion.Rows[0].ItemArray[0]), RootDb, Convert.ToDateTime(Postion.Rows[0].ItemArray[1]), Convert.ToInt32(Postion.Rows[0].ItemArray[2])); // 변화된 Root DB를 통하여 기차 수 알아내기
                                DbCon2.insertqry("insert into Position Values (" + RootDb.Rows[0].ItemArray[9] + ",'" + RootDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + RootDb.Rows[0].ItemArray[0] + ")", "ARTech"); // 이전 Position 기록
                            }
                        }
                        else// ( 로그 DB의 Posiotion에 데이터가 없는 경우에 ) ( 0 값을 전송 )
                        {
                            DateTime RootDate = Convert.ToDateTime(RootDb.Rows[0].ItemArray[6]);
                            DbCon2.insertqry("insert into Position Values (" + RootDb.Rows[0].ItemArray[9] + ",'" + RootDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + RootDb.Rows[0].ItemArray[0] + ")", "ARTech"); // 완                  
                        }
                    }
                }
                else
                {
                    DataTable Postion = DbCon2.dbConn("select * from Position order by  ID desc", "ARTech");
                    RootDb = DbCon.dbConn("select * from alarm_tab order by ID desc", DBName);
                    if (Convert.ToInt32(Postion.Rows[0].ItemArray[2]) != Convert.ToInt32(RootDb.Rows[0].ItemArray[0])) // 이전 Position기록과 Root Db를비교하여 변화가 있다
                    {
                        string[] ChDate = null;
                        int[] ChangePos = null;
                        List<string> TrainTag = new List<string>();
                        DateTime RootDate = Convert.ToDateTime(RootDb.Rows[0].ItemArray[6]); // 이전 Position 기록용
                        ChangePos = ChangeCk(Postion, RootDb, ref ChDate, TrainTag); // 열차 변화를 감지하여 기존 열차 정보를 업데이트(변경된 Pos 값을 가져온다)
                        if (ChangePos != null)
                            DataUpdate(ChangePos, ChDate, Postion.Rows[0].ItemArray[2].ToString(), TrainTag);
                        DbCon2.insertqry("insert into Position Values (" + RootDb.Rows[0].ItemArray[9] + ",'" + RootDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + RootDb.Rows[0].ItemArray[0] + ")", "ARTech"); // 이전 Position 기록
                    }
                }
            }

        }

        //데이터 넣는곳(보낼떄) 
        public byte[] ResponseHeader(Define TrainData)
        {

            DataTable TrainDataDb = DbCon2.dbConn("select * from TrainData order by dateTime desc", "ARTech");
            DataTable RootDb = DbCon.dbConn("select * from alarm_tab order by ID desc", DBName);
            //시퀀스 넘버
            TrainData.OpCode[3] = 2;

            if (TrainData.SeqNo == 32767)
                TrainData.SeqNo = 1;
            else
                TrainData.SeqNo += 1;

            //if (TrainData.LData != null)
            //    TrainData.LData.TrainData.Clear();
            if (RootDb.Rows.Count != 0) // 루트 DB에 값이 있는 경우에만 
            {
                if (TrainDataDb.Rows.Count == 0) // 기존 로그 DB에 값이 없는 경우( 기록된 열차가 없는 경우 )
                {
                    DataTable Postion = DbCon2.dbConn("select * from Position order by ID desc", "ARTech");
                    if (RootDb.Rows.Count != 0) // 로그 DB의 Posiotion을 이용해 변화를 감지( 로그 DB의 Posiotion에 데이터가 있는 경우에 )
                    {
                        if (Postion.Rows.Count != 0) // 처음 포지션 값이 있을경우
                        {
                            if (Convert.ToInt32(Postion.Rows[0].ItemArray[2]) != Convert.ToInt32(RootDb.Rows[0].ItemArray[0])) // 기존 Position 값과 루트 DB를 비교하여 변화가 생길시 //완
                            {

                                int count = 0; ; // 기차 수
                                //DateTime RootDate = Convert.ToDateTime(RootDb.Rows[0].ItemArray[6]); // 이전 Position 기록용
                                //PositionTF(Convert.ToInt32(Postion.Rows[0].ItemArray[0]), RootDb, Convert.ToDateTime(Postion.Rows[0].ItemArray[1]), Convert.ToInt32(Postion.Rows[0].ItemArray[2])); // 변화된 Root DB를 통하여 기차 수 알아내기
                                //DbCon2.insertqry("insert into Position Values (" + RootDb.Rows[0].ItemArray[9] + ",'" + RootDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + RootDb.Rows[0].ItemArray[0] + ")", "ARTech"); // 이전 Position 기록
                                DataTable Train = DbCon2.dbConn("select * from TrainData", "ARTech");
                                DataTable Trains = new DataTable();
                                for (int z = 0; z < Train.Rows.Count; z++)
                                    if (((int)Train.Rows[z].ItemArray[3] == 1 || (int)Train.Rows[z].ItemArray[3] == 2) && (int)(int)Train.Rows[z].ItemArray[2] == 2)
                                    { }
                                    else if (((int)Train.Rows[z].ItemArray[3] == 28 || (int)Train.Rows[z].ItemArray[3] == 27) && (int)(int)Train.Rows[z].ItemArray[2] == 1)
                                    { }
                                    else
                                    {
                                        Trains.ImportRow(Train.Rows[z]);
                                    }
                                count = Convert.ToInt32(Trains.Rows.Count);
                                // 방향성이 정확하지 않으므로 0값 전송  
                                byte[] data = new byte[42 + (count * 23)];
                                if (data.Length == 42)//|| TrainData.LData.TrainData.Count == 0) // 오알람시
                                    return Fun.defualtData(TrainData); // 기본 데이터 전송
                                Fun.stringToByte(data, TrainData.Preamble, 0); //Preamble (0)
                                Fun.intToByte(data, 34 + (count * 23), 4, 4); //Length (1)
                                Fun.intToByte(data, TrainData.SeqNo, 8, 2); // SeqNo (2)
                                Fun.ToByte(data, TrainData.OpCode, 10, 4); //OpCode (3)
                                Fun.TrainDatainput(ref TrainData, count, Trains); // 열차 Data 넣는곳 ( 메인 Data, 넣을 열차 Data 수 ) (4)
                                data = Fun.byteinput(data, TrainData, 14, count);
                                // Reserved 데이터 (5)
                                TrainData.Reserved = new byte[2];
                                Fun.ToByte(data, TrainData.Reserved, 39 + (count * 23), 2); // Data 의 갯수(N)만큼 37바이트의 데이터가 추가된다.

                                // Checksum 데이터 (6)
                                int checksum = Fun.CalculateChecksum(data); // 체크넘버
                                Fun.intToByte(data, checksum, 41 + (count * 23), 1); // Data 의 갯수(N)만큼 37바이트의 데이터가 추가된다.

                                return data;

                            }
                            else if (Convert.ToInt32(Postion.Rows[0].ItemArray[2]) == Convert.ToInt32(RootDb.Rows[0].ItemArray[0]))// 변화가 생기지 않을 시( 0 값을 전송 ) // 완
                            {
                                return Fun.defualtData(TrainData);
                            }
                        }
                        else// ( 로그 DB의 Posiotion에 데이터가 없는 경우에 ) ( 0 값을 전송 )
                        {
                            //DateTime RootDate = Convert.ToDateTime(RootDb.Rows[0].ItemArray[6]);
                            //DbCon2.insertqry("insert into Position Values (" + RootDb.Rows[0].ItemArray[9] + ",'" + RootDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + RootDb.Rows[0].ItemArray[0] + ")", "ARTech"); // 완                  
                            return Fun.defualtData(TrainData);
                        }
                    }
                }
                else // 기존 로그 DB에 값이 있는 경우
                {
                    DataTable Postion = DbCon2.dbConn("select * from Position order by  ID desc", "ARTech");
                    RootDb = DbCon.dbConn("select * from alarm_tab order by ID desc", DBName);
                    if (Convert.ToInt32(Postion.Rows[0].ItemArray[2]) != Convert.ToInt32(RootDb.Rows[0].ItemArray[0])) // 이전 Position기록과 Root Db를비교하여 변화가 있다
                    {
                        //string[] ChDate = null;
                        //int[] ChangePos = null;
                        //List<string> TrainTag = new List<string>();
                        //DateTime RootDate = Convert.ToDateTime(RootDb.Rows[0].ItemArray[6]); // 이전 Position 기록용
                        //ChangePos = ChangeCk(Postion, RootDb, ref ChDate, TrainTag); // 열차 변화를 감지하여 기존 열차 정보를 업데이트(변경된 Pos 값을 가져온다)
                        //if (ChangePos != null)
                        //    DataUpdate(ChangePos, ChDate, Postion.Rows[0].ItemArray[2].ToString(), TrainTag);
                        //DbCon2.insertqry("insert into Position Values (" + RootDb.Rows[0].ItemArray[9] + ",'" + RootDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + RootDb.Rows[0].ItemArray[0] + ")", "ARTech"); // 이전 Position 기록

                        DataTable datas = DbCon2.dbConn("select * from Traindata order by DateTime desc", "ARTech");
                        int count = datas.Rows.Count;
                        DataTable Trains = new DataTable();
                        for (int z = 0; z < datas.Rows.Count; z++)
                            if (((int)datas.Rows[z].ItemArray[3] == 1 || (int)datas.Rows[z].ItemArray[3] == 2) && (int)(int)datas.Rows[z].ItemArray[2] == 2)
                            { }
                            else if (((int)datas.Rows[z].ItemArray[3] == 28 || (int)datas.Rows[z].ItemArray[3] == 27) && (int)(int)datas.Rows[z].ItemArray[2] == 1)
                            { }
                            else
                            {
                                Trains.ImportRow(datas.Rows[z]);
                            }
                        count = Trains.Rows.Count;
                        byte[] data = new byte[42 + (count * 23)];
                        if (data.Length == 42)//|| TrainData.LData.TrainData.Count == 0) // 오알람시
                            return Fun.defualtData(TrainData); // 기본 데이터 전송

                        Fun.stringToByte(data, TrainData.Preamble, 0); //Preamble (0)
                        Fun.intToByte(data, 34 + (count * 23), 4, 4); //Length (1) 수정하기

                        Fun.intToByte(data, TrainData.SeqNo, 8, 2); // SeqNo (2)
                        Fun.ToByte(data, TrainData.OpCode, 10, 4); //OpCode (3)
                        Fun.TrainDatainput(ref TrainData, count, Trains); // 열차 Data 넣는곳 ( 메인 Data, 넣을 열차 Data 수 ) (4) 수정하기
                        data = Fun.byteinput(data, TrainData, 14, count);
                        // Reserved 데이터 (5)
                        TrainData.Reserved = new byte[2];
                        Fun.ToByte(data, TrainData.Reserved, 39 + (count * 23), 2); // Data 의 갯수(N)만큼 37바이트의 데이터가 추가된다.

                        // Checksum 데이터 (6)
                        int checksum = Fun.CalculateChecksum(data); // 체크넘버
                        Fun.intToByte(data, checksum, 41 + (count * 23), 1); // Data 의 갯수(N)만큼 37바이트의 데이터가 추가된다.

                        return data;
                    }
                    else if (Convert.ToInt32(Postion.Rows[0].ItemArray[2]) == Convert.ToInt32(RootDb.Rows[0].ItemArray[0])) // 변화가 없다. // 완
                    {
                        DataTable datas = DbCon2.dbConn("select * from Traindata order by DateTime desc", "ARTech");
                        int count = datas.Rows.Count;
                        DataTable Trains = new DataTable();
                        
                        for (int z = 0; z < datas.Rows.Count; z++)
                            if (((int)datas.Rows[z].ItemArray[3] == 1 || (int)datas.Rows[z].ItemArray[3] == 2) && (int)(int)datas.Rows[z].ItemArray[2] == 2)
                            { }
                            else if (((int)datas.Rows[z].ItemArray[3] == 28 || (int)datas.Rows[z].ItemArray[3] == 27) && (int)(int)datas.Rows[z].ItemArray[2] == 1)
                            { }
                            else
                            {
                                Trains.ImportRow(datas.Rows[z]);
                            }
                        count = Trains.Rows.Count;
                        byte[] data = new byte[42 + (count * 23)];

                        if (data.Length == 42)//|| TrainData.LData.TrainData.Count == 0) // 오알람시
                            return Fun.defualtData(TrainData); // 기본 데이터 전송

                        Fun.stringToByte(data, TrainData.Preamble, 0); //Preamble (0)
                        Fun.intToByte(data, 34 + (count * 23), 4, 4); //Length (1) 수정하기

                        Fun.intToByte(data, TrainData.SeqNo, 8, 2); // SeqNo (2)
                        Fun.ToByte(data, TrainData.OpCode, 10, 4); //OpCode (3)
                        Fun.TrainDatainput(ref TrainData, count, datas); // 열차 Data 넣는곳 ( 메인 Data, 넣을 열차 Data 수 ) (4) 수정하기
                        data = Fun.byteinput(data, TrainData, 14, count);
                        // Reserved 데이터 (5)
                        TrainData.Reserved = new byte[2];
                        Fun.ToByte(data, TrainData.Reserved, 39 + (count * 23), 2); // Data 의 갯수(N)만큼 37바이트의 데이터가 추가된다.

                        // Checksum 데이터 (6)
                        int checksum = Fun.CalculateChecksum(data); // 체크넘버
                        Fun.intToByte(data, checksum, 41 + (count * 23), 1); // Data 의 갯수(N)만큼 37바이트의 데이터가 추가된다.

                        return data;
                    }
                }
            }

            return null;

        }
        #endregion
    }
}
