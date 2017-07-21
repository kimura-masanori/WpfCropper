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

namespace WpfCropper {
 
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window {
    private bool isDragging = false;
    private Point anchorPoint = new Point();
    public MainWindow() {
      InitializeComponent();
      this.Gridimage1.MouseLeftButtonDown += image1_MouseLeftButtonDown;
      this.Gridimage1.MouseMove += image1_MouseMove;
      this.Gridimage1.MouseLeftButtonUp += image1_MouseLeftButtonUp;
      Go.IsEnabled = false;
      image2.Source = null;
    }

    private void image1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
      if (isDragging) {
        isDragging = false;
        if(selectionRectangle.Width > 0) {
          Go.Visibility = System.Windows.Visibility.Visible;
          Go.IsEnabled = true;
        }

        if(selectionRectangle.Visibility != Visibility.Visible) {
          selectionRectangle.Visibility = Visibility.Visible;
        }
      }
    }

    private void image1_MouseMove(object sender, MouseEventArgs e) {
      if (isDragging) {
        double x = e.GetPosition(BackPanel).X;
        double y = e.GetPosition(BackPanel).Y;
        selectionRectangle.SetValue(Canvas.LeftProperty,Math.Min(x,anchorPoint.X));
        selectionRectangle.SetValue(Canvas.TopProperty,Math.Min(y,anchorPoint.Y));
        selectionRectangle.Width = Math.Abs(x - anchorPoint.X);
        selectionRectangle.Height = Math.Abs(y - anchorPoint.Y);

        if (selectionRectangle.Visibility != Visibility.Visible)
          selectionRectangle.Visibility = Visibility.Visible;
      }
    }

    private void Go_Click(object sender, RoutedEventArgs e) {
      if(image1.Source != null) {
        //Rect rect1 = new Rect(Canvas.GetLeft(selectionRectangle), Canvas.GetTop(selectionRectangle), 
        //  selectionRectangle.Width, selectionRectangle.Height);
        var source = image1.Source as BitmapSource;
        Rect selectionRect = new Rect(selectionRectangle.RenderSize);
        var sourceRect = selectionRectangle.TransformToVisual(this.image1).TransformBounds(selectionRect);
        var xMultiplier = source.PixelWidth / image1.ActualWidth;
        var yMultiplier = source.PixelHeight / image1.ActualHeight;
        sourceRect.Scale(xMultiplier, yMultiplier);
                
        System.Windows.Int32Rect rcFrom = new Int32Rect();
        rcFrom.X = (int)sourceRect.X;
        rcFrom.Y = (int)sourceRect.Y;
        rcFrom.Width = (int)sourceRect.Width;
        rcFrom.Height = (int)sourceRect.Height;
        BitmapSource bs = new CroppedBitmap(source,rcFrom);
        image2.Source = bs;
      }
    }

    private void image1_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {

      if (isDragging == false) {
        anchorPoint.X = e.GetPosition(BackPanel).X;
        anchorPoint.Y = e.GetPosition(BackPanel).Y;
        isDragging = true;
      }

    }

    private void RestRect() {
      selectionRectangle.Visibility = Visibility.Collapsed;
      isDragging = false;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      this.image1.Source = new BitmapImage( new Uri("./sampleimages/sample.jpeg",UriKind.Relative));
      Console.WriteLine(image1.Source.ToString());
    }
  }
}
