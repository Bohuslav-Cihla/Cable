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

        private List<Definition> _definitions = new();
        
        public MainWindow()
        {
            FillList(0, 0);
            InitializeComponent();
            ShowCables();
        }

        public void FillList(double positionX, double positionY)
        {
            var diameters = new List<double>()
            {
                7.5,
                5.0,
                12.5,
                10,
                //25,
                10,
                1.5,
                8.6667,
                7.5,
                18.0,
                7.5,
                19,
                2,
                20
            };
            
            var listOfDiameters = diameters.OrderByDescending(c => c).ToList();

            for (double firstX = 0; firstX < listOfDiameters.Max(); firstX++)
            {
                for (double firstY = 0; firstY < listOfDiameters.Max(); firstY++)
                {
                    var cableDiameter = CreateCircles(firstX, firstY, listOfDiameters, false);
            
                    _definitions.Add(new Definition{FirstX = firstX, FirstY = firstY, CableDiameter = cableDiameter});
                }
            }

            var minCableDiameter = _definitions.Min(c=>c.CableDiameter);

            var definition = _definitions.First(d => Math.Abs(d.CableDiameter - minCableDiameter) < 1);
            
            var finalCableDiameter = CreateCircles(definition.FirstX, definition.FirstY, listOfDiameters, true);
        }

        public double CreateCircles(double positionX, double positionY, List<double> listOfDiameters, bool save)
        {
            List<Cirlce> circles = new();
            
            for (int k = 0; k < listOfDiameters.Count; k++)
            {
                if (!circles.Any())
                {
                    circles.Add(new Cirlce{PositionX =  positionX, PositionY = positionY, Radius = listOfDiameters[k]/2});
                }
                else
                {
                    double radius = 0;

                    bool inCollision = true;

                    while (inCollision)
                    {
                        for (int i = 0; i < 360; i++)
                        {
                            var positon = GetPositon(radius, i);

                            var newCircle = new Cirlce { PositionX = positon.X, PositionY = positon.Y, Radius = listOfDiameters[k]/2 };
                        
                            // check if circle is not in collision

                            inCollision = CheckColisionWithOthers(newCircle, circles);

                            if (!inCollision)
                            {
                                circles.Add(newCircle);
                                break;
                            }
                        }

                        radius++;
                    }
                }
            }
            
            // return cover diameter
            var outerDiameter = circles.Select(circle => BiggestDiameter(circle)).Prepend(0).Max();

            if (save)
            {
                _circles.AddRange(circles);
            }

            return outerDiameter;
        }

        private bool CheckColisionWithOthers(Cirlce newCircle, List<Cirlce> circles)
        {
            for (int i = 0; i < circles.Count; i++)
            {
                if (circles[i].IsCollision(newCircle.PositionX, newCircle.PositionY,
                        newCircle.Radius))
                {
                    return true;
                }
            }

            return false;
        }

        public void ShowCables()
        {
            int centerX = 400;
            int centerY = 250;

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

                Canvas.SetTop(e, centerY - _circles[i].PositionY - _circles[i].Radius); //HERE
                Canvas.SetLeft(e, centerX + _circles[i].PositionX - _circles[i].Radius);
                
                MyCanvas.Children.Add(e);
            }
            
            // find the biggest distant
            var coverDiameter = _circles.Select(circle => BiggestDiameter(circle)).Prepend(0).Max();
            
            Ellipse c = new Ellipse();
            c.Height = coverDiameter;
            c.Width = coverDiameter;
                
            c.Stroke =Brushes.Red;

            Canvas.SetTop(c, centerY - coverDiameter / 2); //HERE
            Canvas.SetLeft(c, centerX- coverDiameter / 2);
                
            MyCanvas.Children.Add(c);
            
            // show cover radius
            

            TextBlock textBlock = new TextBlock();
            textBlock.Text = $"Diameter of cable is {coverDiameter.ToString()}mm.";
            //textBlock.Foreground = new SolidColorBrush(color);
            Canvas.SetLeft(textBlock, 0);
            Canvas.SetTop(textBlock, 0);
            MyCanvas.Children.Add(textBlock);

        }

        public double BiggestDiameter(Cirlce cirlce)
        {
            var c = Math.Sqrt(Math.Pow(cirlce.PositionX, 2) + Math.Pow(cirlce.PositionY, 2));

            return 2 * (c + cirlce.Radius);
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