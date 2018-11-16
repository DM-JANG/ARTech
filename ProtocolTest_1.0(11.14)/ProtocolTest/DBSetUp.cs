using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProtocolTest
{
    public partial class DBSetUp : Form
    {
        public DBSetUp()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DBSetUp_Load(object sender, EventArgs e)
        {       
            string[] TextValue = System.IO.File.ReadAllLines(@"..\..\DBset.txt");
                if (TextValue.Length > 0)
                {
                    for (int i = 0; i < TextValue.Length; i++)
                    {
                        int index = TextValue[i].IndexOf(':');
                        TextValue[i] = TextValue[i].Substring(index+1, TextValue[i].Length - index-1);

                    }
                }
            Server.Text = TextValue[0];
            ID.Text = TextValue[1];
            Pass.Text = TextValue[2];
            DBName.Text = TextValue[3];

        }

        private void Ok_Click(object sender, EventArgs e)
        {
            string[] lines = new string[4];
            lines[0] = "Server :" +Server.Text;
            lines[1] = "ID :"+ID.Text;
            lines[2] = "Pass :"+Pass.Text;
            lines[3] = "DBName :" + DBName.Text;
            using (StreamWriter outputFile = new StreamWriter(@"..\..\DBset.txt"))
            {
                foreach (string line in lines)
                {
                    outputFile.WriteLine(line);
                }
            }
            MessageBox.Show("설정 저장 완료","설정 창");
        }
    }
}
