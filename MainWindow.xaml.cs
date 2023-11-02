using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Cable.Models;

namespace Cable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly List<Circle> _circles = new();

        private readonly List<Definition> _definitions = new();
        
        public MainWindow()
        {
            FillList(ReadFile());
            InitializeComponent();
            ShowCables();
        }

        /// <summary>
        /// Read file with diameters
        /// </summary>
        /// <returns>List of doubles as cable diameters</returns>
        private List<double> ReadFile()
        {
            List<double> diameters = new ();
            
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Document", // Default file name
                DefaultExt = ".txt", // Default file extension
                Filter = "Text documents (.txt)|*.txt" // Filter files by extension
            };

            // Show open file dialog box
            dialog.ShowDialog();
            
            try
            {
                // Open the text file using a stream reader.
                using var sr = new StreamReader(dialog.FileName);
                while (sr.ReadLine() is { } line)
                {
                    if (double.TryParse(line, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out var diameter))
                    {
                        // multiply by 10 due to scale output
                        diameters.Add(diameter * 10);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return diameters;
        }

        /// <summary>
        /// Find best arrangement for the smallest cable diameter
        /// </summary>
        /// <param name="diameters">List of cable diameters</param>
        private void FillList(List<double> diameters)
        {
            var listOfDiameters = diameters.OrderByDescending(c => c).ToList();

            // iteration for the best positioning of the biggest cable diameter
            for (double firstX = 0; firstX < listOfDiameters.Max(); firstX += 5)
            {
                for (double firstY = 0; firstY < listOfDiameters.Max(); firstY += 5)
                {
                    // arrange cables
                    var cableDiameter = CreateCircles(firstX, firstY, listOfDiameters, false);
            
                    // save position of biggest cable and cover diameter into list
                    _definitions.Add(new Definition{FirstX = firstX, FirstY = firstY, CableDiameter = cableDiameter});
                }
            }

            // find the smallest cover diameter in list of iteration
            var minCableDiameter = _definitions.Min(c=>c.CableDiameter);

            // find position of first cable for the smallest cover
            var definition = _definitions.First(d => Math.Abs(d.CableDiameter - minCableDiameter) < 1);
            
            // save the cable arrangement
            CreateCircles(definition.FirstX, definition.FirstY, listOfDiameters, true);
        }

        /// <summary>
        /// Arrangement of cables in cable bundle
        /// </summary>
        /// <param name="positionX">X-coordinate of the biggest cable</param>
        /// <param name="positionY">Y-coordinate of the biggest cable</param>
        /// <param name="listOfDiameters">List of cable diameters</param>
        /// <param name="save">Save the positions of cables</param>
        /// <returns></returns>
        private double CreateCircles(double positionX, double positionY, List<double> listOfDiameters, bool save)
        {
            List<Circle> circles = new();
            
            foreach (var t in listOfDiameters)
            {
                if (!circles.Any())
                {
                    // position for first cable
                    circles.Add(new Circle{PositionX =  positionX, PositionY = positionY, Radius = t/2});
                }
                else
                {
                    double radius = 0;

                    bool inCollision = true;

                    while (inCollision)
                    {
                        // try to find best position for the cable
                        for (int i = 0; i < 360; i++)
                        {
                            var position = GetPosition(radius, i);

                            var newCircle = new Circle { PositionX = position.X, PositionY = position.Y, Radius = t/2 };
                        
                            // check if cable is not in collision
                            inCollision = CheckCollisionWithOthers(newCircle, circles);

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
            var outerDiameter = circles.Select(CoverDiameter).Prepend(0).Max();

            if (save)
            {
                // save position
                _circles.AddRange(circles);
            }

            return outerDiameter;
        }

        /// <summary>
        /// Check if cable is in collision with any other
        /// </summary>
        /// <param name="newCircle">Position of the cable</param>
        /// <param name="circles">All others in cable bundle</param>
        /// <returns>True/False</returns>
        private bool CheckCollisionWithOthers(Circle newCircle, List<Circle> circles)
        {
            foreach (var circle in circles)
            {
                if (circle.IsCollision(newCircle.PositionX, newCircle.PositionY,
                        newCircle.Radius))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Show cable bundle in canvas
        /// </summary>
        private void ShowCables()
        {
            int centerX = 400;
            int centerY = 250;

            // create 0,0 mark
            Line lineX = new ()
            {
                X1 = centerX - 5,
                X2 = centerX + 5,
                Y1 = centerY,
                Y2 = centerY,
                Stroke = Brushes.GreenYellow
            };
            MyCanvas.Children.Add(lineX);
            
            // create 0,0 mark
            Line lineY = new Line
            {
                X1 = centerX,
                X2 = centerX,
                Y1 = centerY - 5,
                Y2 = centerY + 5,
                Stroke = Brushes.GreenYellow
            };
            MyCanvas.Children.Add(lineY);

            // create circles as cables
            foreach (var t in _circles)
            {
                Ellipse e = new Ellipse
                {
                    Height = t.Radius * 2,
                    Width = t.Radius * 2,
                    Stroke = Brushes.Black
                };

                Canvas.SetTop(e, centerY - t.PositionY - t.Radius); //HERE
                Canvas.SetLeft(e, centerX + t.PositionX - t.Radius);
                
                MyCanvas.Children.Add(e);
            }
            
            // find cover radius
            var coverDiameter = _circles.Select(CoverDiameter).Prepend(0).Max();
            
            Ellipse c = new Ellipse
            {
                Height = coverDiameter,
                Width = coverDiameter,
                Stroke = Brushes.Red
            };

            Canvas.SetTop(c, centerY - coverDiameter / 2); //HERE
            Canvas.SetLeft(c, centerX- coverDiameter / 2);
                
            MyCanvas.Children.Add(c);
            
            // show cover radius as text
            TextBlock textBlock = new TextBlock
            {
                Text = $"Diameter of cable is {(coverDiameter/10).ToString(CultureInfo.InvariantCulture)}mm."
            };
            //textBlock.Foreground = new SolidColorBrush(color);
            Canvas.SetLeft(textBlock, 0);
            Canvas.SetTop(textBlock, 0);
            MyCanvas.Children.Add(textBlock);

        }

        /// <summary>
        /// Calculate distance of radius from middle point -> add circle radius for calculate cover diameter
        /// </summary>
        /// <param name="circle">Cable</param>
        /// <returns>Cover diameter</returns>
        private static double CoverDiameter(Circle circle)
        {
            var c = Math.Sqrt(Math.Pow(circle.PositionX, 2) + Math.Pow(circle.PositionY, 2));

            return 2 * (c + circle.Radius);
        }

        private static Coordinates GetPosition(double radius, int angle)
        {
            double angleInRadians = Math.PI / 180.0 * angle;

            double x = radius * Math.Cos(angleInRadians);
            double y = radius * Math.Sin(angleInRadians);

            return new Coordinates { X = x, Y = y };
        }
    }
}