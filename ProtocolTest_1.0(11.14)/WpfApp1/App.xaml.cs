using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private List<PopUp> popUp = new List<PopUp>();



        public List<PopUp> PopUp
        {
            get
            {
                return popUp;
            }
            set
            {
                popUp = value;
            }
        }



    }
}
