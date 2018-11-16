using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    class Function
    {

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

        public PopData Popupdatainsert(PopData Pop, int SpaceID, Image image, DataRow Row, ref bool Popck, bool[] ColorChange, int Posid,int row,int index,double tim)
        {

            Uri uriSource = null;
            string ImageName = image.Source.ToString();
            if (ColorChange[Posid])
            {
                ImageName = ImageName.Remove(0, 30);
                if (ImageName == "uncheck.png")
                {
                    uriSource = new Uri(@"\Images\check.png", UriKind.Relative);
                }
                else if (ImageName == "check.png")
                {
                    uriSource = new Uri(@"\Images\check.png", UriKind.Relative);
                }
                else if (ImageName == "check.png")
                {
                    uriSource = new Uri(@"\Images\check.png", UriKind.Relative);
                }
                //ColorChange[Posid] = false;
                if (Convert.ToInt32(Row.ItemArray[2]) == 2)
                {
                    if (Posid - 2 >= 0 && Posid + 2 < 30)
                    {
                        ColorChange[Posid - 2] = true;
                        ColorChange[Posid + 2] = true;
                    }
                }
                if (Convert.ToInt32(Row.ItemArray[2]) == 1)
                {
                    if (Posid - 2 >= 30 && Posid + 2 < 60)
                    {
                        ColorChange[Posid - 2] = true;
                        ColorChange[Posid + 2] = true;
                    }

                }
            }
            if (uriSource != null)
                image.Source = new BitmapImage(uriSource);
            Popck = true;
            Pop.Date = Convert.ToDateTime(Row.ItemArray[0]).AddSeconds(tim);
            Pop.Tag = Row.ItemArray[row].ToString();
            Pop.UPDown = Convert.ToInt32(Row.ItemArray[2]);
            switch (SpaceID)
            {
                case 0:
                    Pop.Position = 270;
                    break;
                case 1:
                    Pop.Position = 390;
                    break;
                case 2:
                    Pop.Position = 567;
                    break;
                case 3:
                    Pop.Position = 749;
                    break;
                case 4:
                    Pop.Position = 1464;
                    break;
                case 5:
                    Pop.Position = 1478;
                    break;
                case 6:
                    Pop.Position = 1488;
                    break;
                case 7:
                    Pop.Position = 1619;
                    break;
                case 8:
                    Pop.Position = 1698;
                    break;
                case 9:
                    Pop.Position = 1740;
                    break;
                case 10:
                    Pop.Position = 2442;
                    break;
                case 11:
                    Pop.Position = 2539;
                    break;
                case 12:
                    Pop.Position = 2602;
                    break;
                case 13:
                    Pop.Position = 2652;
                    break;
                case 14:
                    Pop.Position = 2703;
                    break;
                case 15:
                    Pop.Position = 2831;
                    break;
                case 16:
                    Pop.Position = 3182;
                    break;
                case 17:
                    Pop.Position = 3419;
                    break;
                case 18:
                    Pop.Position = 3755;
                    break;
                case 19:
                    Pop.Position = 6047;
                    break;
                case 20:
                    Pop.Position = 6117;
                    break;
                case 21:
                    Pop.Position = 6693;
                    break;
                case 22:
                    Pop.Position = 7993;
                    break;
                case 23:
                    Pop.Position = 8370;
                    break;
                case 24:
                    Pop.Position = 8433;
                    break;
                case 25:
                    Pop.Position = 9281;
                    break;
                case 26:
                    Pop.Position = 9546;
                    break;
                case 27:
                    Pop.Position = 9968;
                    break;
                case 28:
                    Pop.Position = 10350;
                    break;
                case 29:
                    Pop.Position = 10485;
                    break;

            }
            return Pop;
        }
        public Point Pointretrun(int SpaceID, int UpDown, List<Image> CkButton)
        {
            int y = SpaceID;
            Point Point = new Point();
            if (UpDown == 1)
            {
                y += 30;
            }
            Point.X = CkButton[y].Margin.Left;
            Point.Y = CkButton[y].Margin.Top;

            return Point;
        }
        public string reutrnPos2(int i)
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
        public int reutrnPos(string i)
        {
            int num = 0;
            switch (i)
            {

                case "지정천교(입)":
                    num = 1;
                    break;
                case "지정천교(출)":
                    num = 2;
                    break;
                case "소막골 터널(입)":
                    num = 3;
                    break;
                case "소막골 터널(출)":
                    num = 4;
                    break;
                case "선로 상태":
                    num = 5;
                    break;
                case "보통고가(입)":
                    num = 6;
                    break;
                case "보통고가(출)":
                    num = 7;
                    break;
                case "서원주 변전소":
                    num = 8;
                    break;
                case "광터고가(입)":
                    num = 9;
                    break;
                case "광터고가(출)":
                    num = 10;
                    break;
                case "지장물1(입)":
                    num = 11;
                    break;
                case "지장물1(출)":
                    num = 12;
                    break;
                case "지장물2(입)":
                    num = 13;
                    break;
                case "지장물2(출)":
                    num = 14;
                    break;
                case "만종터널(입)":
                    num = 15;
                    break;
                case "만종터널(출)":
                    num = 16;
                    break;
                case "만종역(입)":
                    num = 17;
                    break;
                case "만종역(출)":
                    num = 18;
                    break;
                case "만종천교(입)":
                    num = 19;
                    break;
                case "만종천교(출)":
                    num = 20;
                    break;
                case "호저터널(입)":
                    num = 21;
                    break;
                case "호저터널(출)":
                    num = 22;
                    break;
                case "가현교(입)":
                    num = 23;
                    break;
                case "가현교(출)":
                    num = 24;
                    break;
                case "점실교(입)":
                    num = 25;
                    break;
                case "점실교(출)":
                    num = 26;
                    break;
                case "원주천교(입)":
                    num = 27;
                    break;
                case "원주천교(출)":
                    num = 28;
                    break;


            }
            return num;
        }
    }
}
