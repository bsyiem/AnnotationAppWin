using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KinectCandD
{
    class PointMapper
    {
        Rect projectionSpace;

        public PointMapper(Rect projectionSpace)
        {
            this.projectionSpace = projectionSpace;
        }

        public Point MapPoint(Point p)
        {
            Point mappedPoint = new Point();
            mappedPoint.X = p.X - projectionSpace.TopLeft.X;
            mappedPoint.Y = p.Y - projectionSpace.TopLeft.Y;

            if(mappedPoint.X >= 0 && mappedPoint.Y >= 0)
            {
                return mappedPoint;
            }else
            {
                return (new Point(-1, -1));
            }
        } 
    }
}
