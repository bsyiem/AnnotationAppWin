using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace KinectCandD
{
    class DisplayRect
    {
        Shape displayRect;
        CanvasController canvasController;

        public DisplayRect(Canvas canvas,Rect rect, Point[] corners)
        {
            this.canvasController = new CanvasController(canvas);
            this.CreateDisplayRect(rect, corners);
        }

        public void RemoveDisplayRect()
        {
            this.canvasController.RemoveShapeFromCanvas(this.displayRect);
        }

        private void CreateDisplayRect(Rect rect,Point[] corners)
        {
            float centerX = (float)(corners[0].X < corners[1].X ? corners[0].X + (rect.Width / 2) : corners[1].X + (rect.Width / 2));
            float centerY = (float)(corners[0].Y < corners[1].Y ? corners[0].Y + (rect.Height / 2) : corners[1].Y + (rect.Width / 2));
            this.displayRect = this.canvasController.DisplayRectangle(rect, centerX, centerY);
        }
    }
}
