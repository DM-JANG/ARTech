using CrystalDecisions.Shared;
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
    public partial class CReportView : Form
    {
        string mFileName;
        string mServerDNS = string.Empty;
        string Datavase = string.Empty;

        public CReportView()
        {
            InitializeComponent();
        }

        public string ReportName
        {
            get { return mFileName; }
            set { mFileName = value;
                CryDoc.FileName = mFileName;
            }
        }

        public void Param(string[] aField , object[] aVlaue)
        {
            ParameterValues currentValues;
            string FieldName;
            try
            {
                for (int i =0; i<aField.Length; i++)
                {
                    FieldName = "@" + aField[i];
                    currentValues = CryDoc.DataDefinition.ParameterFields[FieldName].CurrentValues;
                    if(currentValues.Count >0)
                    currentValues.RemoveAt(0);
                    currentValues.AddValue(aVlaue[i]);
                    CryDoc.DataDefinition.ParameterFields[FieldName].ApplyCurrentValues(currentValues);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void Formula(string[] aField, string[] aValue)
        {
            try
            {
                for(int i=0; i<aField.Length; i++)
                {
                    CryDoc.DataDefinition.FormulaFields[aField[i]].Text = "'" + aValue[i] + "'";

                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CReportView_Load(object sender, EventArgs e)
        {
            string[] TextValue = System.IO.File.ReadAllLines(@"..\..\DBset2.txt");
            if (TextValue.Length > 0)
            {
                for (int i = 0; i < TextValue.Length; i++)
                {
                    int index = TextValue[i].IndexOf(':');
                    TextValue[i] = TextValue[i].Substring(index + 1, TextValue[i].Length - index - 1);
                }
            }
            if(TextValue[2].ToString()=="")
            {
                CryDoc.DataSourceConnections[0].SetConnection(TextValue[0], "ARTech", false);
            }
            else
            {
                CryDoc.DataSourceConnections[0].IntegratedSecurity = false;
                CryDoc.DataSourceConnections[0].SetConnection(TextValue[0], "ARTech", TextValue[1], TextValue[2]);
            }

            ReportViewer.ReportSource = CryDoc;
            ReportViewer.BringToFront();

            Size S, S1;
            S = ReportViewer.PreferredSize;
            S1 = this.Size;

            this.SizeFromClientSize(ReportViewer.PreferredSize);
        }
    }
}
