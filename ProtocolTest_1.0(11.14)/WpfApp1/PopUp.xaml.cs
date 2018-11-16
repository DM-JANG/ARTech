using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// PopUp.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopUp : Window
    {
        public DateTime Date;
        public String Tag;
        public int UPDown;
        public int Position;

        public PopUp()
        {
            InitializeComponent();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Time.Content = Date.ToString("yyyy-MM-dd HH:mm:ss");
            Train.Content = Tag;
            if (UPDown == 2)
                Updown.Content = "하행";
            else if (UPDown == 1)
                Updown.Content = "상행";
            Pos.Content = Position + " (m)";

        }
    }
}
