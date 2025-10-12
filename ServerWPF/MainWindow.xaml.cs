using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Forms; // Screen üçün
using Timer = System.Timers.Timer;

namespace LiveScreenServer
{
    public partial class MainWindow : Window
    {
        private bool _isRunning = false;
        private Thread? _captureThread;
        private TcpClient? _client;
        private string _ip;
        private int _port;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            _ip = IpBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(_ip)) _ip = "127.0.0.1";

            if (!int.TryParse(PortBox.Text, out _port))
                _port = 8080;

            _isRunning = true;
            BtnStart.Visibility = Visibility.Collapsed;
            BtnStop.Visibility = Visibility.Visible;
            StatusText.Text = $"📡 Yayım başladı → {_ip}:{_port}";

            _captureThread = new Thread(CaptureLoop)
            {
                IsBackground = true
            };
            _captureThread.Start();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _isRunning = false;
            BtnStop.Visibility = Visibility.Collapsed;
            BtnStart.Visibility = Visibility.Visible;
            StatusText.Text = "🛑 Yayım dayandırıldı.";
        }

        private void CaptureLoop()
        {
            while (_isRunning)
            {
                try
                {
                    Rectangle bounds = Screen.PrimaryScreen.Bounds;
                    using Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);

                    using MemoryStream ms = new MemoryStream();
                    bmp.Save(ms, ImageFormat.Jpeg);
                    byte[] data = ms.ToArray();

                    using TcpClient client = new TcpClient(_ip, _port);
                    using NetworkStream ns = client.GetStream();

                    byte[] sizeInfo = BitConverter.GetBytes(data.Length);
                    ns.Write(sizeInfo, 0, sizeInfo.Length);
                    ns.Write(data, 0, data.Length);

                    Dispatcher.Invoke(() =>
                        StatusText.Text = $"✅ Kadr göndərildi: {data.Length / 1024} KB");

                    Thread.Sleep(100); // ~10 FPS
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                        StatusText.Text = $"⚠️ Xəta: {ex.Message}");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
