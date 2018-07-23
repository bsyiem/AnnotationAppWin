using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeCollector
{
    public class Gazepoint
    {
        public float x { get; set; }
        public float y { get; set; }

        public Gazepoint (float x, float y)
        {
            this.x = x;
            this.y = y;
        } 
    }
}
