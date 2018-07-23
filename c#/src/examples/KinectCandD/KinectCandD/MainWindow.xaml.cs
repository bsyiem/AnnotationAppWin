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

namespace KinectCandD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        Bitmap bi;
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

            this.DataContext = this;

            setimage();

            InitializeComponent();
        }

        public ImageSource img
        {
            get
            {
                return ImageConverter.BitmapToBitmapImage(this.bi);
            }
        }

        public void setimage()
        {
            this.bi = new Bitmap("../../Resources/PupilMarkers.jpg");
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.displayM.InsertPoint(Mouse.GetPosition(canvas));
            //this.tempM.InsertPoint(GetActualMousePosition(Mouse.GetPosition(StreamImage)));
            this.tempM.InsertPoint(Mouse.GetPosition(StreamImage));
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

        //the ConversionRate must be Examined
        private void StartDetect(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Starting");
            Start.IsEnabled = false;

            md.DetectMarkers(this.bi);

            //For Testing 
            //printMapped(md);

            double actualHeight = Math.Abs(md.corners.ToArrayOfArray()[0][1].Y - md.corners.ToArrayOfArray()[0][2].Y);
            double actualWidth = Math.Abs(md.corners.ToArrayOfArray()[0][1].X - md.corners.ToArrayOfArray()[0][0].X);
            double depictedHeight = this.MarkerMRect.rect.Height;
            double depictedWidth = this.MarkerMRect.rect.Width;

            double conversionRateHeight = GetConversionRate(actualHeight, depictedHeight);
            double conversionRateWidth = GetConversionRate(actualWidth, depictedWidth);

            CorrectMRects(conversionRateHeight, conversionRateWidth);

            CornerAdjuster cd = new CornerAdjuster(md.ids.ToArray(), md.corners.ToArrayOfArray(), this.PaperMRect.rect);

            this.bi = md.DrawMarker(this.bi, cd.Adjust());

            CreateDisplayWindow();

            //Network Tests
            Connection conn = new Connection();
            conn.Connect();
            conn.SendSurfaceDetails(CreateSurfaceList.GetList(md.ids.ToArray(), cd.Adjust().ToArrayOfArray()));

            //while (true)
            //{
            //    conn.SendSurfaceDetails(CreateSurfaceList.GetList(md.ids.ToArray(), cd.Adjust().ToArrayOfArray()));
            //}

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

        private void CreateDisplayWindow()
        {
            DisplayWindow d = new DisplayWindow();
            d.Show();
            d.display.Source = ImageConverter.BitmapToBitmapImage(this.bi);
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

        private void SendData()
        {

        }

        //For testing only
        private void printMapped(MarkerDetector md)
        {
            Trace.WriteLine(this.ProjectionMRect.rect.TopLeft);
            for (int i = 0; i < md.ids.ToArray().Length; i++)
            {
                PointMapper pm = new PointMapper(this.ProjectionMRect.rect);
                Trace.WriteLine("original id =" + md.ids.ToArray()[i] + " topcorner" + md.corners.ToArrayOfArray()[i][0]);
                Trace.WriteLine("mapped =" + pm.MapPoint(PointConverter.ConvertFromDrawingPFToWindowsP(md.corners.ToArrayOfArray()[i][0])));
            }
        }


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
