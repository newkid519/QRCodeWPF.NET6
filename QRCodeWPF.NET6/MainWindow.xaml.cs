﻿using AForge.Video.DirectShow;
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

            //var result = writer.Write("sessionIdsessionIdsessionIdsessionIdsessionIdsessionIdsessionIdsessionIdsessionId");
            //result.Save("123.png", ImageFormat.Png);
            //string abc = decodeBarcodeText(result);
            FilterInfoCollection filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var device = filterInfoCollection.Cast<FilterInfo>().FirstOrDefault();
            VideoCaptureDevice videoCaptureDevice = new VideoCaptureDevice(device.MonikerString);

            videoCaptureDevice.VideoResolution = videoCaptureDevice.VideoCapabilities[6];
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
        }

        int counter = 15;
        private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {            
            MemoryStream ms = new MemoryStream();
            eventArgs.Frame.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            Dispatcher.BeginInvoke(new Action(() => {
                qrCode.Source = bitmapImage;
            }));

            if (counter-- == 0)
            {
                counter = 15;
                var result = decodeBarcodeText(eventArgs.Frame);
                if (!string.IsNullOrEmpty(result))
                {
                    System.Diagnostics.Debug.WriteLine(result);
                }
            }
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
