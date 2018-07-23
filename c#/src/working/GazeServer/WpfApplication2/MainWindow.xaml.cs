using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace GazeCollector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Server server = new Server();
        public Canvas canvas;
        public MainWindow()
        {
            InitializeComponent();
            MakeSeeThrough(0);

            server.StartServer(this);

            //while (true)
            //{   
            //    if(!this.server.dataStore.CheckSurfaceEmpty())
            //    {
            //        foreach (int id in this.server.dataStore.GetIDsSurface())
            //        {
            //            Trace.WriteLine(this.server.dataStore.getSurface(id)[0]);
            //        }
            //    }
                
            //}
        }

        public void MakeSeeThrough(Double value)
        {
            //sets opacity,click through,startup at centre, maximised window size
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;

            this.Background = CreateSolidColorBrush(255, 255, 255, value);

            MakeCentered();
            MakeClickThrough();
            MakeMaximized();
            MakeTopMost();

            //creates circle
            //Ellipse circle = CreateCircle(75);

            // determines circle position withing canvas
            //SetPosition(circle, 100, 100);

            //determines circle fill color and opacity
            SolidColorBrush scbCircle = CreateSolidColorBrush(255, 0, 255, 0.5);

            //determines circle stroke thickness, color and opacity
            SolidColorBrush scbStroke = CreateSolidColorBrush(0, 0, 255, 0.5);

            //applies circle properties
            //SetFill(scbCircle, circle);

            //applies stroke properties
            //SetStroke(scbStroke, 5, circle);

            //creates a new canvas and adds the circle
            this.canvas = new Canvas();
            //canvas.Children.Add(circle);

            //adds canvas to window.
            this.AddChild(this.canvas);
        }

        public void AddShapeToCanvas(Shape obj)
        {
            this.canvas.Children.Add(obj);
        }
        
        public SolidColorBrush CreateSolidColorBrush(byte r, byte g, byte b, Double opacity)
        {
            SolidColorBrush scb = new SolidColorBrush();
            scb.Color = Color.FromRgb(r, g, b);
            scb.Opacity = opacity;
            return scb;
        }

        public void MakeCentered()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public void MakeMaximized()
        {
            this.WindowState = WindowState.Maximized;
        }

        public void MakeClickThrough()
        {

            this.IsHitTestVisible = false;
        }
        public void MakeTopMost()
        {
            this.Topmost = true;
        }

        public Ellipse CreateCircle(int radius)
        {
            Ellipse circle = new Ellipse();
            circle.Height = radius;
            circle.Width = radius;
            return circle;
        }

        public void SetPosition(Shape obj, float x, float y)
        {
            Canvas.SetLeft(obj, x-obj.Width/2);
            Canvas.SetBottom(obj, y - obj.Height/2);
        }
        public void SetFill(SolidColorBrush fill, Shape obj)
        {
            obj.Fill = fill;

        }
        public void SetStroke(SolidColorBrush stroke, Double thickness, Shape obj)
        {
            obj.Stroke = stroke;
            obj.StrokeThickness = thickness;
        }

        public Shape CreateFilledCircle()
        {
            Random random = new Random();
            Ellipse circle = this.CreateCircle(75);
            this.SetPosition(circle, 100, 100);
            SolidColorBrush scbCircle = this.CreateSolidColorBrush(Convert.ToByte(random.Next(0,255)), Convert.ToByte(random.Next(0, 255)), Convert.ToByte(random.Next(0, 255)), 0.5);
            SolidColorBrush scbStroke = this.CreateSolidColorBrush(0, 0, 255, 0.5);
            this.SetFill(scbCircle, circle);
            this.SetStroke(scbStroke, 5, circle);
            this.AddShapeToCanvas(circle);

            return circle;
        }
    }
}
