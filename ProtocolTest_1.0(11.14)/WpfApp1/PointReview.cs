using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WpfApp1;

namespace DAS_GUI
{
    public partial class PointReview : Form
    {
        DbConn DbCon = new DbConn();
        DbConn DbCon2 = new DbConn();
        int pointnum = 0;
        bool SearckCk = false;
        public PointReview()
        {
            InitializeComponent();
            DbCon2.TextInputData2();
        }



        public string Namereturn(int num)
        {
            switch (num)
            {
                case 1:
                    return "지정천교(입)";
                case 2:
                    return "지정천교(출)";
                case 3:
                    return "소막골 터널(입)";
                case 4:
                    return "소막골 터널(출)";
                case 5:
                    return "선로상태";
                case 6:
                    return "보통고가(입)";
                case 7:
                    return "보통고가(출)";
                case 8:
                    return "서원주 변전소";
                case 9:
                    return "광터고가(입)";
                case 10:
                    return "광터고가(출)";
                case 11:
                    return "지장물1(입)";
                case 12:
                    return "지장물1(출)";
                case 13:
                    return "지장물2(입)";
                case 14:
                    return "지장물2(출)";
                case 15:
                    return "만종터널(입)";
                case 16:
                    return "만종터널(출)";
                case 17:
                    return "만종역(입)";
                case 18:
                    return "만종역(출)";
                case 19:
                    return "만종천교(입)";
                case 20:
                    return "만종천교(출)";
                case 21:
                    return "호저터널(입)";
                case 22:
                    return "호저터널(출)";
                case 23:
                    return "가현교(입)";
                case 24:
                    return "가현교(출)";
                case 25:
                    return "점실교(입)";
                case 26:
                    return "점실교(출)";
                case 27:
                    return "원주천교(입)";
                case 28:
                    return "원주천교(출)";

            }
            return "";
        }
        public void RightbuttonLog(int num)
        {
            if (num > 29)
                num = num - 30;
            pointnum = num;
            int Index = 0;
            String Tag = string.Empty;
            String UpDown = string.Empty;
            String Space = string.Empty;
            String Position = string.Empty;
            String Date = string.Empty;
            SearckCk = false;
            DataTable PointData = DbCon2.dbConn("Select * from LogData where Space = '" + Namereturn(num).Trim() + "' order by Date asc", "ARTech");
            PointList.SelectedIndex = num - 1;
      
        }
        public void reFlush()
        {
            ZoneDataList.Rows.Clear();
            ZoneDataList.Refresh();
        }
        private void PointList_SelectedIndexChanged(object sender, EventArgs e)
        {

            ZoneDataList.Rows.Clear();
            ZoneDataList.Refresh();
            String[] Point = PointList.GetItemText(PointList.SelectedItem).Trim().Split('.');
            
            pointnum = Convert.ToInt32(Point[0]);
            int Index = 0;
            String Tag = string.Empty;
            String UpDown = string.Empty;
            String Space = string.Empty;
            String Position = string.Empty;
            String Date = string.Empty;
            DataTable PointData = DbCon2.dbConn("Select * from LogData where Space = '" + Point[1].Trim() + "' order by Date asc", "ARTech");

            for (int i = 0; i < PointData.Rows.Count; i++)
            {

                Index = Index + 1;
                Tag = PointData.Rows[i].ItemArray[1].ToString();
                UpDown = PointData.Rows[i].ItemArray[2].ToString();
                Space = PointData.Rows[i].ItemArray[3].ToString();
                Position = PointData.Rows[i].ItemArray[4].ToString();
                Date = PointData.Rows[i].ItemArray[5].ToString();

                ZoneDataList.Rows.Add(Index, Tag, UpDown, Space, Position, Date);

            }
        }

        private void Print_Click(object sender, EventArgs e)
        {
            if (SearckCk)
            {
                String[] Point = PointList.GetItemText(PointList.SelectedItem).Trim().Split('.');
                string[] aFormField = { "DATTM1", "DATTM2", "CIRCDE" };
                string[] aFromValue = new string[aFormField.Length];
                string[] aParmField = { "DATTM1", "DATTM2", "CIRNME" };
                string[] aParmValue = new string[aParmField.Length];
                string ReportName = string.Empty;
                string FileName = string.Empty;

                DateTime Start = StartDate.Value;
                DateTime End = EndDate.Value; 
                aFromValue[0] = Start.ToString("yyyy-MM-dd");
                aFromValue[1] = End.ToString("yyyy-MM-dd");
                aFromValue[2] = Point[1].Trim();


                aParmValue[0] = StartDate.Text;
                aParmValue[1] = EndDate.Text;
                aParmValue[2] = Point[1].Trim();

                ReportName = "PointTrain.rpt";

                FileName = "..\\..\\" + ReportName; //Application.StartupPath + "\\" + ReportName;
                CReportView crpt = new CReportView();
                crpt.Text = "Report viwer-Train List";
                if (System.IO.File.Exists(@FileName))
                {
                    crpt.ReportName = FileName;
                    crpt.Param(aFormField, aFromValue);
                    crpt.Formula(aParmField, aParmValue);
                    crpt.Show();
                    SearckCk = false;
                }
            }else
            {
                MessageBox.Show("날짜를 검색해 주세요.");
            }
        }

        private void Search_Click(object sender, EventArgs e)
        {
            ZoneDataList.Rows.Clear();
            ZoneDataList.Refresh();
            String[] Point = PointList.GetItemText(PointList.SelectedItem).Trim().Split('.');

            int Index = 0;
            String Tag = string.Empty;
            String UpDown = string.Empty;
            String Space = string.Empty;
            String Position = string.Empty;
            String Date = string.Empty;
            DateTime Sd = StartDate.Value;
            DateTime Ed = EndDate.Value;
            Ed = Ed.AddDays(1);

            DataTable PointData = DbCon2.dbConn("Select * from LogData where Space = '" + Namereturn(pointnum).Trim() + "' and Date >= '" + Sd.ToString("yyyy-MM-dd") + "' and Date <= '" + Ed.ToString("yyyy-MM-dd") + "' order by Date asc ", "ARTech");

            for (int i = 0; i < PointData.Rows.Count; i++)
            {

                Index = Index + 1;
                Tag = PointData.Rows[i].ItemArray[1].ToString();
                UpDown = PointData.Rows[i].ItemArray[2].ToString();
                Space = PointData.Rows[i].ItemArray[3].ToString();
                Position = PointData.Rows[i].ItemArray[4].ToString();
                Date = PointData.Rows[i].ItemArray[5].ToString();

                ZoneDataList.Rows.Add(Index, Tag, UpDown, Space, Position, Date);

            }
            SearckCk = true;
        }
    }
}
