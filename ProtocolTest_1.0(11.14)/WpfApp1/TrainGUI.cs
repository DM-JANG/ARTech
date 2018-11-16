using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    class TrainGUI
    {
        public Image TrainAdd(Point vector,int Updown)
        {
            var uriSource = new Uri(@"\Images\train.png", UriKind.Relative);
            Image Train = new Image();
            if (Updown == 2)
            {
                uriSource = new Uri(@"\Images\train.png", UriKind.Relative);
            }
            else if(Updown ==1)
            {
                uriSource = new Uri(@"\Images\train_Left.png", UriKind.Relative);
            }
                Train.Source = new BitmapImage(uriSource);
            Train.Height = 36;
            Train.Width = 34;
            Train.Margin = new Thickness(vector.X+100,vector.Y+100,0,0);
            Train.Visibility = Visibility.Visible;
            Train.HorizontalAlignment = HorizontalAlignment.Left;
            Train.VerticalAlignment = VerticalAlignment.Top; 
            return Train;
        }
        public Image CkImasgeAdd(int X, int Y)
        {
            Image button = new Image();
            var uriSource = new Uri(@"\Images\uncheck.png", UriKind.Relative);
            button.Source = new BitmapImage(uriSource);
            button.Height = 13.4;
            button.Width = 12.5;
            button.Margin = new Thickness(X , Y , 0, 0);
            button.Visibility = Visibility.Visible;
            button.HorizontalAlignment = HorizontalAlignment.Left; 
            button.VerticalAlignment = VerticalAlignment.Top;
            return button;
        }
        public Label LabelAdd(int X , int Y  , string text)
        {
            Label lb = new Label();
            lb.Height = 30;
            lb.Width = 40;
            lb.Margin = new Thickness(X, Y, 0, 0);
            lb.Visibility = Visibility.Visible;
            lb.HorizontalAlignment = HorizontalAlignment.Left;
            lb.VerticalAlignment = VerticalAlignment.Top;
            lb.HorizontalContentAlignment = HorizontalAlignment.Center;
            lb.Content = text;
            lb.FontStyle = FontStyles.Normal;
            lb.FontWeight = FontWeights.Bold;
            return lb;
        }
        public Image ImageAdd(int X, int Y,string uri)
        {
            Image button = new Image();
            var uriSource = new Uri(uri, UriKind.Relative);
            button.Source = new BitmapImage(uriSource);
            button.Height = 21;
            button.Width = 18;
            button.Margin = new Thickness(X, Y, 0, 0);
            button.Visibility = Visibility.Visible;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            return button;
        } 
        public Line LineAdd(int x1,int y1,int x2, int y2, Brush Color)
        {
            Line Line = new Line();
            Line.X1 = x1;
            Line.Y1 = y1;
            Line.X2 = x2;
            Line.Y2 = y2;

            Line.Stroke = Color;//선색 지정
            Line.StrokeThickness = 4;//선 두께 지정

            return Line;
        }


    }


}
