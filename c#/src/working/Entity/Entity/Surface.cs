using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace Entity
{
    [Serializable]
    public class Surface
    {
        public int i; //id
        public PointF[] v; // vertex

        public Surface(int i, PointF[] v)
        {
            this.i = i;
            this.v = v;
        }

    }
}