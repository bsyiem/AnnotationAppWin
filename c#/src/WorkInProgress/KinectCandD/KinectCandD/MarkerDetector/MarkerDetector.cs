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

namespace KinectCandD
{
    class MarkerDetector
    {
        //need to use the same dictionary as PupilLabs
        Dictionary dict;

        public Emgu.CV.Util.VectorOfInt ids { get; set; } = new Emgu.CV.Util.VectorOfInt();
        public Emgu.CV.Util.VectorOfVectorOfPointF corners { get; set; } = new Emgu.CV.Util.VectorOfVectorOfPointF();

        public MarkerDetector()
        {
            this.dict = new Dictionary(Dictionary.PredefinedDictionaryName.DictArucoOriginal);
        }
        public MarkerDetector(Dictionary.PredefinedDictionaryName dictName)
        {
            this.dict = new Dictionary(dictName);
        }

        public void DetectMarkers(Bitmap img)
        {
            try
            {
                Image<Bgr, Byte> cvimg = new Image<Bgr, Byte>(img);
                ArucoInvoke.DetectMarkers(cvimg, this.dict, this.corners, this.ids, DetectorParameters.GetDefault());
                //ArucoInvoke.DrawDetectedMarkers(cvimg, this.corners, this.ids, new MCvScalar(255, 255, 0));
            }
            catch (Exception e)
            {
                Trace.Write(e);
            }

        }

        public Image<Bgr,Byte> DrawMarker(Bitmap img,Emgu.CV.Util.VectorOfVectorOfPointF corners)
        {
            Image<Bgr, Byte> cvimg = new Image<Bgr, Byte>(img);
            
            try
            { 
                ArucoInvoke.DrawDetectedMarkers(cvimg, corners, this.ids, new MCvScalar(255, 0, 0));
                return (cvimg);
            }catch(Exception e)
            {
                Trace.WriteLine(e);
                return null;
            }
            
        }

        public Image<Bgr,Byte> DetectAndDraw(Image<Bgr,Byte> cvimg)
        {
            try
            {
                ArucoInvoke.DetectMarkers(cvimg, this.dict, this.corners, this.ids, DetectorParameters.GetDefault());
                ArucoInvoke.DrawDetectedMarkers(cvimg, this.corners, this.ids, new MCvScalar(255, 255, 0));
                return cvimg;
            }
            catch (Exception e)
            {
                Trace.Write(e);
                return null;
            }
        }

        public void ClearCache()
        {
            this.ids.Clear();
            this.corners.Clear();
        }

    }
}
