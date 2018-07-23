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

namespace KinectCalibration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MappedRect tempM;
        MappedRect ProjectionMRect;
        MappedRect PaperMRect;

        DisplayRect tempD;
        DisplayRect ProjectionDRect;
        DisplayRect PaperDRect;

        public MainWindow()
        {
            this.tempM = new MappedRect();
            this.ProjectionMRect = new MappedRect();
            this.PaperMRect = new MappedRect();

            this.tempD = null;
            this.ProjectionDRect = null;
            this.PaperDRect = null;

            this.DataContext = this;
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.tempM.InsertPoint(Mouse.GetPosition(Application.Current.MainWindow));
            if (this.tempM.isSet && this.tempD == null)
            {
                this.tempD = new DisplayRect(canvas, this.tempM.rect, this.tempM.corners);
            }
        }

        private void OnConfirmProjection(object sender, RoutedEventArgs e)
        {
            Confirm(ref this.ProjectionMRect,ref this.ProjectionDRect);
            ConfirmProjection.Visibility = Visibility.Hidden;
        }

        private void OnClearProjection(object sender, RoutedEventArgs e)
        {
            Clear(ref this.ProjectionMRect,ref this.ProjectionDRect);
            ConfirmProjection.Visibility = Visibility.Visible;
        }

        private void OnConfirmPaper(object sender, RoutedEventArgs e)
        {
            Confirm(ref this.PaperMRect,ref this.PaperDRect);
            ConfirmPaper.Visibility = Visibility.Hidden;
        }

        private void OnClearPaper(object sender, RoutedEventArgs e)
        {
            Clear(ref this.PaperMRect,ref this.PaperDRect);
            ConfirmPaper.Visibility = Visibility.Visible;
        }

        //clears tempM and tempD if both or one of the sizes are not set
        private void ClearTemp()
        {
            if(!this.PaperMRect.isSet || !this.ProjectionMRect.isSet)
            {
                this.tempD = null;
                this.tempM = new MappedRect();
            }
        }

        private void Confirm(ref MappedRect m,ref DisplayRect d)
        {
            Trace.WriteLine(this.ProjectionMRect.isSet);
            Trace.WriteLine(this.tempM.isSet);
            if (!m.isSet)
            {
                m = this.tempM;
                d = this.tempD;
                ClearTemp();
            }
        }

        private void Clear(ref MappedRect m,ref DisplayRect d)
        {
            m = new MappedRect();
            d.RemoveDisplayRect();
            d = null;
            ClearTemp();
        }
    }
}
