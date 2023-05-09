using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace QRCodeWPF.NET6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,                
                Options = new QrCodeEncodingOptions { Margin = 1, Height = 300, Width = 300, CharacterSet = "UTF-8" }
            };

            var result = writer.Write("sessionIdsessionIdsessionIdsessionIdsessionIdsessionIdsessionIdsessionIdsessionId");
            result.Save("123.png", ImageFormat.Png);
            string abc = decodeBarcodeText(result);            
        }

        private string decodeBarcodeText(Bitmap barcodeBitmap)
        {
            var source = new BitmapLuminanceSource(barcodeBitmap);

            // using http://zxingnet.codeplex.com/
            // PM> Install-Package ZXing.Net
            var reader = new BarcodeReader(null, null, ls => new GlobalHistogramBinarizer(ls))
            {
                AutoRotate = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    TryInverted = true
                }
            };


            var result = reader.Decode(source);
            if (result == null)
            {
                Console.WriteLine("Decode failed.");
                return string.Empty;
            }

            Console.WriteLine("BarcodeFormat: {0}", result.BarcodeFormat);
            Console.WriteLine("Result: {0}", result.Text);

            return result.Text;
        }
    }
}
