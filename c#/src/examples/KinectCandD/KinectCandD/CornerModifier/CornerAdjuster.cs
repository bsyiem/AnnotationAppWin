using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using Emgu.CV.Util;

namespace KinectCandD
{
    class CornerAdjuster
    {
        //to be looked into
        //seemingly randon conversion between emgu and rect metrics

        int[] ids;
        PointF[][] corners;
        double paperWidth;
        double paperHeight;
        
        public CornerAdjuster(int[] ids,PointF[][] corners,Rect paperSize)
        {
            this.ids = ids;
            this.corners = corners;
            this.paperWidth = paperSize.Width;
            this.paperHeight = paperSize.Height;
        }

        public VectorOfVectorOfPointF Adjust()
        {
            for(int i = 0;i< this.ids.Length; i++)
            {
                System.Windows.Point[] markerCornerPoint = new System.Windows.Point[4];
                markerCornerPoint = GetPoints(this.corners[i]);

                Vector first = System.Windows.Point.Subtract(markerCornerPoint[1],markerCornerPoint[0]);
                Vector second = System.Windows.Point.Subtract(markerCornerPoint[2], markerCornerPoint[1]);
                Vector third = System.Windows.Point.Subtract(markerCornerPoint[3], markerCornerPoint[0]);

                first.Normalize();
                second.Normalize();
                third.Normalize();

                this.corners[i][1] = PointConverter.ConvertFromWindowsPToDrawingPF(System.Windows.Point.Add(markerCornerPoint[0], Vector.Multiply(first, paperWidth)));
                this.corners[i][2] = PointConverter.ConvertFromWindowsPToDrawingPF(System.Windows.Point.Add(PointConverter.ConvertFromDrawingPFToWindowsP(this.corners[i][1]), Vector.Multiply(second, paperHeight)));
                this.corners[i][3] = PointConverter.ConvertFromWindowsPToDrawingPF(System.Windows.Point.Add(markerCornerPoint[0], Vector.Multiply(third, paperHeight)));
            }
            return (new VectorOfVectorOfPointF(this.corners));
        }

        private System.Windows.Point[] GetPoints(PointF[] markerCornerPointF)
        {
            System.Windows.Point[] markerCornerPoint = new System.Windows.Point[4];
            markerCornerPoint[0] = PointConverter.ConvertFromDrawingPFToWindowsP(markerCornerPointF[0]);
            markerCornerPoint[1] = PointConverter.ConvertFromDrawingPFToWindowsP(markerCornerPointF[1]);
            markerCornerPoint[2] = PointConverter.ConvertFromDrawingPFToWindowsP(markerCornerPointF[2]);
            markerCornerPoint[3] = PointConverter.ConvertFromDrawingPFToWindowsP(markerCornerPointF[3]);
            return markerCornerPoint;
        }
    }
}
