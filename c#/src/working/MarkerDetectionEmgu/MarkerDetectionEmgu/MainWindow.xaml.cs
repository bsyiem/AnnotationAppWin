using System;
using System.Collections.Generic;
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
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using Emgu.CV.Aruco;

namespace MarkerDetectionEmgu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {      
        Bitmap bi;
        MarkerDetector md = new MarkerDetector(Dictionary.PredefinedDictionaryName.Dict4X4_1000);
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            setimage();
            this.bi = md.DetectMarkers(this.bi);
            this.WindowState = WindowState.Maximized;
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
    }
}
