using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace KinectCalibration
{
    class MappedRect
    {
        public Rect rect { get; set; }
        public Point[] corners { get; set; }
        public int head {get; set;}
        public bool isSet { get; set; }

        public MappedRect()
        {
            this.head = 0;
            this.corners = new Point[2];
        }

        public void InsertPoint(Point p)
        {
            if(this.head >= 2)
            {
                return;
            }

            this.corners[this.head] = p;
            this.head += 1;

            if(this.head == 2)
            {
                this.CreateRect();
            }
        }

        public void CreateRect()
        {
            this.isSet = true;
            this.rect = new Rect(this.corners[0], this.corners[1]);
        }

    }
}
