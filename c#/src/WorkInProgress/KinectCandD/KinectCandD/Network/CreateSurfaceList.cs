using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace KinectCandD
{
    class CreateSurfaceList
    {
        public static List<Surface> GetList(int[] ids,PointF[][] corners)
        {
            List<Surface> surfaceList = new List<Surface>();

            for (int i = 0;i<ids.Length;i++)
            {
                surfaceList.Add(CreateSurface(ids[i], corners[i]));
            }
            return surfaceList;
        } 

        private static Surface CreateSurface(int id, PointF[] corners)
        {
            return (new Surface(id,corners));
        }
    }
}
