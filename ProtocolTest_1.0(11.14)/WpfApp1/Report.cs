using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAS_GUI
{
    public partial class Report : Form
    {
        public Report()
        {
            InitializeComponent();
        }

        private void Report_Load(object sender, EventArgs e)
        {
            KindCombo.SelectedIndex = 0;
        }

        private void Cancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Print_Click(object sender, EventArgs e)
        {
            string[] aFormField = { "DATTM1", "DATTM2", "CIRCDE" };
            string[] aFromValue = new string[aFormField.Length ];
            string[] aParmField = { "DATTM1", "DATTM2", "CIRNME" };
            string[] aParmValue = new string[aParmField.Length ];
            string ReportName = string.Empty;
            string FileName = string.Empty;

            DateTime Start = StartDate.Value;
            DateTime End = EndDate.Value;
            aFromValue[0] = Start.ToString("yyyy-MM-dd");
            aFromValue[1] = End.ToString("yyyy-MM-dd");
            aFromValue[2] = KindCombo.SelectedItem.ToString();
            if (KindCombo.SelectedIndex == 0)
                aFromValue[2] = "*";

            aParmValue[0] = StartDate.Text;
            aParmValue[1] = EndDate.Text;
            aParmValue[2] = KindCombo.SelectedItem.ToString();

            ReportName = "Trainrpt.rpt";

            FileName = "..\\..\\" + ReportName; //Application.StartupPath + "\\" + ReportName;
            CReportView crpt = new CReportView();
            crpt.Text = "Report viwer-Train List";
            if(System.IO.File.Exists(@FileName))
            {
                crpt.ReportName = FileName;
                crpt.Param(aFormField, aFromValue);
                crpt.Formula(aParmField, aParmValue);
                crpt.Show();
            }
        }
        
    }
}
