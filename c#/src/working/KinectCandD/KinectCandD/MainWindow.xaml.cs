//note: using any other classes for conversion of images seems to be less efficient than writing the code within
//this class itself 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.Aruco;
using Microsoft.Kinect;
using System.ComponentModel;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace KinectCandD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        //flag to see if all required objects are set
        bool started = false;

        //stores the rect that is used to display on the canvas as actual image size is different
        MappedRect displayM;
        //store rect corresponding to actual image size
        MappedRect tempM;

        MappedRect ProjectionMRect;
        MappedRect PaperMRect;
        MappedRect MarkerMRect;

        DisplayRect tempD;

        DisplayRect ProjectionDRect;
        DisplayRect PaperDRect;
        DisplayRect MarkerDRect;

        //Kinect
        private KinectSensor kinectSensor = null;
        private WriteableBitmap colorBitmap = null;
        private ColorFrameReader colorFrameReader = null;

        //private BitmapImage displayed;

        //Image
        private Bitmap bi;

        MarkerDetector md = new MarkerDetector(Dictionary.PredefinedDictionaryName.Dict4X4_1000);
        public MainWindow()
        {
            this.displayM = new MappedRect();

            this.tempM = new MappedRect();
            this.ProjectionMRect = new MappedRect();
            this.PaperMRect = new MappedRect();
            this.MarkerMRect = new MappedRect();

            this.tempD = null;
            this.ProjectionDRect = null;
            this.PaperDRect = null;
            this.MarkerDRect = null;

            //Kinect
            this.InitializeKinectComponents();

            this.DataContext = this;
            
            //image
            //SetImage();

            InitializeComponent();
        }



        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.displayM.InsertPoint(Mouse.GetPosition(canvas));
            //this.tempM.InsertPoint(GetActualMousePosition(Mouse.GetPosition(StreamImage)));
            this.tempM.InsertPoint(Mouse.GetPosition(InitImage));
            //this.tempM.InsertPoint(Mouse.GetPosition(WFH));
            //this.tempM.InsertPoint(Mouse.GetPosition(Application.Current.MainWindow));
            ConfirmButtonInteractivity();
            if (this.displayM.isSet && this.tempD == null)
            {
                this.tempD = new DisplayRect(canvas, this.displayM.rect, this.displayM.corners);
            }
        }

        private void ConfirmButtonInteractivity()
        {
            if (this.tempM.isSet)
            {
                ConfirmProjection.IsEnabled = true;
                ConfirmPaper.IsEnabled = true;
                ConfirmMarker.IsEnabled = true;
            }
            else
            {
                ConfirmProjection.IsEnabled = false;
                ConfirmPaper.IsEnabled = false;
                ConfirmMarker.IsEnabled = false;
            }
        }

        private void OnConfirmProjection(object sender, RoutedEventArgs e)
        {
            Confirm(ref this.ProjectionMRect, ref this.ProjectionDRect);
            ConfirmProjection.Visibility = Visibility.Hidden;
            ClearProjection.Visibility = Visibility.Visible;
        }

        private void OnClearProjection(object sender, RoutedEventArgs e)
        {
            Clear(ref this.ProjectionMRect, ref this.ProjectionDRect);
            ConfirmProjection.Visibility = Visibility.Visible;
            ClearProjection.Visibility = Visibility.Hidden;
        }

        private void OnConfirmPaper(object sender, RoutedEventArgs e)
        {
            Confirm(ref this.PaperMRect, ref this.PaperDRect);
            ConfirmPaper.Visibility = Visibility.Hidden;
            ClearPaper.Visibility = Visibility.Visible;
        }

        private void OnClearPaper(object sender, RoutedEventArgs e)
        {
            Clear(ref this.PaperMRect, ref this.PaperDRect);
            ConfirmPaper.Visibility = Visibility.Visible;
            ClearPaper.Visibility = Visibility.Hidden;
        }

        private void OnConfirmMarker(object sender, RoutedEventArgs e)
        {
            Confirm(ref this.MarkerMRect, ref this.MarkerDRect);
            ConfirmMarker.Visibility = Visibility.Hidden;
            ClearMarker.Visibility = Visibility.Visible;
        }

        private void OnClearMarker(object sender, RoutedEventArgs e)
        {
            Clear(ref this.MarkerMRect, ref this.MarkerDRect);
            ConfirmMarker.Visibility = Visibility.Visible;
            ClearMarker.Visibility = Visibility.Hidden;
        }

        //clears tempM and tempD if both or one of the sizes are not set
        private void ClearTemp()
        {
            if (!this.PaperMRect.isSet || !this.ProjectionMRect.isSet || !this.MarkerMRect.isSet)
            {
                this.tempD = null;
                this.tempM = new MappedRect();
                this.displayM = new MappedRect();
                ConfirmButtonInteractivity();
            }
        }

        private void Confirm(ref MappedRect m, ref DisplayRect d)
        {
            if (!m.isSet)
            {
                m = this.tempM;
                d = this.tempD;
                ClearTemp();
            }
            CheckStart();
        }

        private void Clear(ref MappedRect m, ref DisplayRect d)
        {
            m = new MappedRect();
            d.RemoveDisplayRect();
            d = null;
            if (this.tempD != null)
            {
                this.tempD.RemoveDisplayRect();
            }
            ClearTemp();
            CheckStart();
        }

        private void CheckStart()
        {
            if (this.ProjectionMRect.isSet && this.PaperMRect.isSet && this.MarkerMRect.isSet)
            {
                Start.Visibility = Visibility.Visible;
            }
            else
            {
                Start.Visibility = Visibility.Hidden;
            }
        }

        private void StartDetect(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Starting");
            Start.IsEnabled = false;

            this.started = true;

        }

        private void SwitchImages()
        {
            InitImage.IsEnabled = false;
            InitImage.Visibility = Visibility.Hidden;

            FinalImage.Visibility = Visibility.Visible;
        }
        //Kinect
        private void InitializeKinectComponents()
        {
            this.kinectSensor = KinectSensor.GetDefault();

            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;

            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            this.kinectSensor.Open();
        }

        //Kinect
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.colorFrameReader != null)
            {
                // ColorFrameReder is IDisposable
                this.colorFrameReader.Dispose();
                this.colorFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        //Kinect
        private void Reader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    if (!started)
                    {
                        using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                        {
                            this.colorBitmap.Lock();

                            // verify data and write the new color frame data to the display bitmap
                            if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                            {
                                colorFrame.CopyConvertedFrameDataToIntPtr(
                                    this.colorBitmap.BackBuffer,
                                    (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                    ColorImageFormat.Bgra);

                                this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                            }

                            this.colorBitmap.Unlock();
                        }
                    }
                    else
                    {
                        byte[] buffer = new byte[colorFrameDescription.Width * colorFrameDescription.Height * PixelFormats.Bgr32.BitsPerPixel / 8];

                        colorFrame.CopyConvertedFrameDataToArray(buffer, ColorImageFormat.Bgra);

                        //to be tested 
                        this.bi = new System.Drawing.Bitmap(colorFrameDescription.Width, colorFrameDescription.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                        BitmapData bitmapData = this.bi.LockBits(
                            new System.Drawing.Rectangle(0, 0, this.bi.Width, this.bi.Height),
                            ImageLockMode.WriteOnly,
                            this.bi.PixelFormat);
                        Marshal.Copy(buffer, 0, bitmapData.Scan0, buffer.Length);
                        this.bi.UnlockBits(bitmapData);

                        Image<Bgr, Byte> cvimg = new Image<Bgr, byte>(this.bi);

                        //DetectMarker should run on a different Thread
                        //Should send data to server
                        Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Background,
                            new Action(() => ImageBox.Image = md.DetectAndDraw(cvimg)));
                    }
                }
            }
        }

        //Kinect
        public ImageSource img
        {
            get
            {
                return this.colorBitmap;
            }
        }

        public void SendSurfaceDetails()
        {
            md.DetectMarkers(this.bi);
            
            if (md.ids.Size != 0)
            {
                Trace.WriteLine("size = " + md.ids.Size);
                foreach (int i in md.ids.ToArray())
                {
                    Trace.WriteLine("id =" + i);
                }
            }else
            {
                Trace.WriteLine("no markers detected");
            }

            ImageBox.Image = md.DrawMarker(this.bi,md.corners);

            //if (md.corners.Size != 0)
            //{
            //    double actualHeight = Math.Abs(md.corners.ToArrayOfArray()[0][1].Y - md.corners.ToArrayOfArray()[0][2].Y);
            //    double actualWidth = Math.Abs(md.corners.ToArrayOfArray()[0][1].X - md.corners.ToArrayOfArray()[0][0].X);
            //    double depictedHeight = this.MarkerMRect.rect.Height;
            //    double depictedWidth = this.MarkerMRect.rect.Width;

            //    double conversionRateHeight = GetConversionRate(actualHeight, depictedHeight);
            //    double conversionRateWidth = GetConversionRate(actualWidth, depictedWidth);

            //    CorrectMRects(conversionRateHeight, conversionRateWidth);

            //    CornerAdjuster cd = new CornerAdjuster(md.ids.ToArray(), md.corners.ToArrayOfArray(), this.PaperMRect.rect);

            //    ////Network Tests
            //    //Connection conn = new Connection();
            //    //conn.Connect();
            //    //conn.SendSurfaceDetails(CreateSurfaceList.GetList(md.ids.ToArray(), cd.Adjust().ToArrayOfArray()));

            //    //return md.DrawMarker(this.bi, cd.Adjust());
            //}
            //md.ClearCache();
        }
        
        private void CorrectMRects(double cHeight, double cWidth)
        {
            this.PaperMRect.rect = this.PaperMRect.ScaleRect(cWidth, cHeight);
            //may have to scale projectionRect as well
            //this.ProjectionMRect.rect = this.ProjectionMRect.ScaleRect(cWidth, cHeight);
        }

        private double GetConversionRate(double actual, double depicted)
        {
            return (actual / depicted);
        }

        //For testing only
        //private void printMapped(MarkerDetector md)
        //{
        //    Trace.WriteLine(this.ProjectionMRect.rect.TopLeft);
        //    for (int i = 0; i < md.ids.ToArray().Length; i++)
        //    {
        //        PointMapper pm = new PointMapper(this.ProjectionMRect.rect);
        //        Trace.WriteLine("original id =" + md.ids.ToArray()[i] + " topcorner" + md.corners.ToArrayOfArray()[i][0]);
        //        Trace.WriteLine("mapped =" + pm.MapPoint(PointConverter.ConvertFromDrawingPFToWindowsP(md.corners.ToArrayOfArray()[i][0])));
        //    }
        //}


        //private System.Windows.Point GetActualMousePosition(System.Windows.Point p)
        //{
        //    double pixelWidth = StreamImage.Source.Width;
        //    double pixelHeight = StreamImage.Source.Height;
        //    double x = pixelWidth * p.X / StreamImage.ActualWidth;
        //    double y = pixelHeight * p.Y / StreamImage.ActualHeight;
        //    return (new System.Windows.Point(x, y));
        //}

    }
}
