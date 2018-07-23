using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;

namespace KinectCandD
{
    class PointConverter
    {
        public static System.Windows.Point ConvertFromDrawingPFToWindowsP(PointF pf)
        {
            return (new System.Windows.Point((int)pf.X,(int)pf.Y));
        }

        public static System.Windows.Point ConvertFromDrawingPToWindowsP(System.Drawing.Point dp)
        {
            return (new System.Windows.Point((int)dp.X, (int)dp.Y));
        }

        public static System.Drawing.PointF ConvertFromDrawingPToDrawingPF(System.Drawing.Point dp)
        {
            return (ConvertFromWindowsPToDrawingPF(ConvertFromDrawingPToWindowsP(dp)));
        }

        public static PointF ConvertFromWindowsPToDrawingPF(System.Windows.Point p)
        {
            return (new PointF((float)p.X, (float)p.Y));
        }
    }
}
