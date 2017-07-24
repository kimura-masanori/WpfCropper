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
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace WpfCropper {
 
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window {
    private bool isDragging = false;
    private Point anchorPoint = new Point();
    private Point image1Point = new Point();
    private BitmapSource croppedImage;

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

        Go_Click(sender, new RoutedEventArgs());
      }
    }

    private void image1_MouseMove(object sender, MouseEventArgs e) {
      if (isDragging) {
        double x = e.GetPosition(BackPanel).X;
        double y = e.GetPosition(BackPanel).Y;

        if (x > (image1Point.X + this.Gridimage1.ColumnDefinitions[0].ActualWidth))
          x = this.Gridimage1.ColumnDefinitions[0].ActualWidth;

        selectionRectangle.SetValue(Canvas.LeftProperty,Math.Min(x,anchorPoint.X));
        selectionRectangle.SetValue(Canvas.TopProperty,Math.Min(y,anchorPoint.Y));

        selectionRectangle.Width = Math.Abs(x - anchorPoint.X);
        selectionRectangle.Height = Math.Abs(y - anchorPoint.Y);

        Console.WriteLine(x + ":" + (this.Gridimage1.ColumnDefinitions[0].ActualWidth));

        if (selectionRectangle.Visibility != Visibility.Visible)
          selectionRectangle.Visibility = Visibility.Visible;

      }
    }

    private void Go_Click(object sender, RoutedEventArgs e) {
      if(image1.Source != null) {
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

        if (!(rcFrom.X > 0 && rcFrom.Y > 0 && rcFrom.Width > 0 && rcFrom.Height > 0))
          return;

        croppedImage = new CroppedBitmap(source,rcFrom);
        image2.Source = croppedImage;

        if(croppedImage != null) {
          this.btnOCR.IsEnabled = true;
        }

      }
    }

    private void image1_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {

      ResetRect();
      
      if (isDragging == false) {
        image1Point = image1.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));

        anchorPoint.X = e.GetPosition(BackPanel).X;
        anchorPoint.Y = e.GetPosition(BackPanel).Y;
        selectionRectangle.Height = selectionRectangle.Width = 0;

        isDragging = true;
      }
    }

    private void ResetRect() {
      selectionRectangle.Visibility = Visibility.Collapsed;
      isDragging = false;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      this.image1.Source = new BitmapImage( new Uri("./sampleimages/sample.jpg",UriKind.Relative));
    }

    private async void btnOCR_Click(object sender, RoutedEventArgs e)
    {
      this.Cursor = Cursors.Wait;
     
      var results = await MakeAnalysisRequest();

      var resultstring = new StringBuilder();

      foreach(var result in results) {
        resultstring.Append(result);
      }

      MessageBox.Show(resultstring.ToString(), "結果",
        MessageBoxButton.OK,MessageBoxImage.Information);

      this.Cursor = Cursors.Arrow;
    }

    private async Task<IEnumerable<string>> MakeAnalysisRequest() {

      var client = new HttpClient();
      var queryString = HttpUtility.ParseQueryString(string.Empty);

      client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "36703bdde60d441fa0e386773fa4051f");

      queryString["language"] = "unk";
      queryString["detectOrientation"] = "true";
      var uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?" + queryString;

      HttpResponseMessage response;
      byte[] byteData = ConvertJpegToByteArray(croppedImage);

      using (var content = new ByteArrayContent(byteData)) {
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        response = await client.PostAsync(uri, content);

        string contentString = await response.Content.ReadAsStringAsync();

        var convert = new JsonConverter(contentString);
        return convert.GetJsonValues();
      }

    }

    private byte[] ConvertJpegToByteArray(BitmapSource bs) {
      JpegBitmapEncoder encoder = new JpegBitmapEncoder();
      encoder.QualityLevel = 100;

      using (var stream = new MemoryStream()) {
        encoder.Frames.Add(BitmapFrame.Create(bs));
        encoder.Save(stream);
        byte[] bits = stream.ToArray();
        stream.Close();

        return bits;
      }
    }   
  }
}
