using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinectCalibration
{
    class CanvasController
    {
        private Canvas canvas;

        public CanvasController(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void AddShapeToCanvas(Shape obj)
        {
            this.canvas.Children.Add(obj);
        }

        public void RemoveShapeFromCanvas(Shape obj)
        {
            this.canvas.Children.Remove(obj);
        }

        public SolidColorBrush CreateSolidColorBrush(byte r, byte g, byte b, Double opacity)
        {
            SolidColorBrush scb = new SolidColorBrush();
            scb.Color = Color.FromRgb(r, g, b);
            scb.Opacity = opacity;
            return scb;
        }

        public Ellipse CreateCircle(int radius)
        {
            Ellipse circle = new Ellipse();
            circle.Height = radius;
            circle.Width = radius;
            return circle;
        }

        //Normalized Coordinates
        public void SetPositionEyeTracker(Shape obj, float x, float y)
        {
            Canvas.SetLeft(obj, x - obj.Width / 2);
            Canvas.SetBottom(obj, y - obj.Height / 2);
        }
        //inverted Y-coordinate
        public void SetPosition(Shape obj, float x, float y)
        {
            Canvas.SetLeft(obj, x - obj.Width / 2);
            Canvas.SetTop(obj, y - obj.Height / 2);
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
            SolidColorBrush scbCircle = this.CreateSolidColorBrush(Convert.ToByte(random.Next(0, 255)), Convert.ToByte(random.Next(0, 255)), Convert.ToByte(random.Next(0, 255)), 0.5);
            SolidColorBrush scbStroke = this.CreateSolidColorBrush(0, 0, 255, 0.5);
            this.SetFill(scbCircle, circle);
            this.SetStroke(scbStroke, 5, circle);
            this.AddShapeToCanvas(circle);

            return circle;
        }

        public Rectangle CreateDisplayRectangle(System.Windows.Rect rect)
        {
            Rectangle displayRect = new Rectangle();
            displayRect.Height = rect.Height;
            displayRect.Width = rect.Width;
            return displayRect;
        }
        
        public Shape DisplayRectangle(Rect rect, float x ,float y)
        {
            Random random = new Random();
            Rectangle displayRect = this.CreateDisplayRectangle(rect);
            this.SetPosition(displayRect, x , y);
            //SolidColorBrush scbCircle = this.CreateSolidColorBrush(Convert.ToByte(random.Next(0, 255)), Convert.ToByte(random.Next(0, 255)), Convert.ToByte(random.Next(0, 255)), 0.5);
            SolidColorBrush scbStroke = this.CreateSolidColorBrush(0, 0, 255, 0.5);
            //this.SetFill(scbCircle, circle);
            this.SetStroke(scbStroke, 5, displayRect);
            this.AddShapeToCanvas(displayRect);

            return displayRect;
        }

        internal void removeObj(Shape displayRect)
        {
            this.canvas.Children.Remove(displayRect);
        }
    }
}
