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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cable
{
    
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Cirlce> _circles = new();
        public class Cirlce
        {
            public Double PositionX { get; set; }
            public Double PositionY { get; set; }
            public Double Radius { get; set; }

            public bool IsCollision(double secondX, double secondY, double secondRadius)
            {
                var test = Math.Sqrt(Math.Pow(PositionX - secondX, 2) + Math.Pow(PositionY - secondY, 2));

                if (test < secondRadius + Radius)
                {
                    return true;
                }

                return false;
            }
        }

        public class Coordinates
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        
        public MainWindow()
        {
            FillList();
            InitializeComponent();
            ShowCables();
        }

        public void FillList()
        {
            // var radiuses = new List<double>()
            // {
            //     07.5,
            //     // 05.0,
            //     // 12.5,
            //     1,
            //     // 25,
            //     // 30,
            //     // 01.5,
            //     //86.667,
            //     // 07.5,
            //     // 18.0,
            //     // 27.5,
            //     // 19,
            //     // 2,
            // };
            //
            // var test = radiuses.OrderByDescending(c => c).ToList();
            //
            // for (int i = 0; i < test.Count; i++)
            // {
            //     _circles.Add(new Cirlce{PositionX =  - test[i] / 2 + i*100, PositionY = test[i] / 2 + i*100, Radius = test[i]});
            // }
            
            _circles.Add(new Cirlce{PositionX =  0, PositionY = 0, Radius = 10});
            // _circles.Add(new Cirlce{PositionX =  55, PositionY = -55, Radius = 20});
            // _circles.Add(new Cirlce{PositionX =  30, PositionY = 30, Radius = 20});

            for (int i = 0; i < 360; i+=10)
            {
                var positon = GetPositon(20, i);
                
                _circles.Add(new Cirlce{PositionX =  positon.X, PositionY = positon.Y, Radius = 10});
            }
            
        }

        public void ShowCables()
        {
            int centerX = 200;
            int centerY = 100;

            Line lineX = new Line();
            lineX.X1 = centerX - 5;
            lineX.X2 = centerX + 5;
            lineX.Y1 = centerY;
            lineX.Y2 = centerY;
            lineX.Stroke =Brushes.GreenYellow;
            MyCanvas.Children.Add(lineX);
            
            Line lineY = new Line();
            lineY.X1 = centerX;
            lineY.X2 = centerX;
            lineY.Y1 = centerY - 5;
            lineY.Y2 = centerY + 5;
            lineY.Stroke =Brushes.GreenYellow;
            MyCanvas.Children.Add(lineY);

            for (int i = 0; i < _circles.Count; i++)
            {
                Ellipse e = new Ellipse();
                e.Height = _circles[i].Radius * 2;
                e.Width = _circles[i].Radius * 2;
                
                e.Stroke =Brushes.Black;

                for (int j = 0; j < i; j++)
                {
                    if (_circles[i] != _circles[j])
                    {
                        if (_circles[i].IsCollision(_circles[j].PositionX, _circles[j].PositionY,
                                _circles[j].Radius))
                        {
                            e.Stroke =Brushes.Red;
                        }
                    }
                }

                Canvas.SetTop(e, centerY - _circles[i].PositionY - _circles[i].Radius); //HERE
                Canvas.SetLeft(e, centerX + _circles[i].PositionX - _circles[i].Radius);
                
                MyCanvas.Children.Add(e);
            }
        }
        
        public Coordinates GetPositon(double radius, int angle)
        {
            double angleInRadians = Math.PI / 180.0 * angle; // Úhel v radiánech (např. π/4 pro 45 stupňů)

            double x = radius * Math.Cos(angleInRadians);
            double y = radius * Math.Sin(angleInRadians);

            return new Coordinates { X = x, Y = y };
        }
    }
}