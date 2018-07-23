using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Aruco;
using System.Diagnostics;
using System.Drawing;

namespace MarkerDetectionEmgu
{
    class MarkerDetector
    {
        //need to use the same dictionary as PupilLabs
        Dictionary dict;

        Emgu.CV.Util.VectorOfInt ids { get; set; } = new Emgu.CV.Util.VectorOfInt();
        Emgu.CV.Util.VectorOfVectorOfPointF corners { get; set; } = new Emgu.CV.Util.VectorOfVectorOfPointF();

        public MarkerDetector()
        {
            this.dict = new Dictionary(Dictionary.PredefinedDictionaryName.DictArucoOriginal);
        }
        public MarkerDetector(Dictionary.PredefinedDictionaryName dictName)
        {
            this.dict = new Dictionary(dictName);
        }

        public Bitmap DetectMarkers(Bitmap img)
        {
            try
            {
                Image<Bgr, Byte> cvimg = new Image<Bgr, Byte>(img);
                ArucoInvoke.DetectMarkers(cvimg, this.dict, corners, ids, DetectorParameters.GetDefault());
                ArucoInvoke.DrawDetectedMarkers(cvimg, this.corners, this.ids, new MCvScalar(255, 255, 0));
                return cvimg.ToBitmap();
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return null;
            }

        }

    }
}
