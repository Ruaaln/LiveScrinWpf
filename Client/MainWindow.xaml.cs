using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LiveClient
{
    public partial class MainWindow : Window
    {
        private TcpListener listener;
        private bool isRunning = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnClick_Start(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] parts = EndPointBox.Text.Split(':');
                string ip = parts[0];
                int port = int.Parse(parts[1]);

                listener = new TcpListener(IPAddress.Parse(ip), port);
                listener.Start();
                isRunning = true;

                BtnStart.Visibility = Visibility.Collapsed;
                BtnStop.Visibility = Visibility.Visible;

                _ = Task.Run(ListenLoop);

                MessageBox.Show($"Server {ip}:{port} ilə görüntü qəbuluna başladı.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xəta: " + ex.Message);
            }
        }

        private async Task ListenLoop()
        {
            while (isRunning)
            {
                try
                {
                    var client = await listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClient(client));
                }
                catch { }
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            using (client)
            using (var ns = client.GetStream())
            {
                byte[] sizeInfo = new byte[4];
                await ns.ReadAsync(sizeInfo, 0, 4);
                int size = BitConverter.ToInt32(sizeInfo, 0);

                byte[] data = new byte[size];
                int total = 0;
                while (total < size)
                {
                    int bytes = await ns.ReadAsync(data, total, size - total);
                    if (bytes == 0) break;
                    total += bytes;
                }

                await Dispatcher.InvokeAsync(() =>
                {
                    using var ms = new MemoryStream(data);
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.StreamSource = ms;
                    bmp.EndInit();
                    ImageBox.Source = bmp;
                });
            }
        }

        private void OnClick_Stop(object sender, RoutedEventArgs e)
        {
            try
            {
                isRunning = false;
                listener?.Stop();
                ImageBox.Source = null;

                BtnStop.Visibility = Visibility.Collapsed;
                BtnStart.Visibility = Visibility.Visible;

                MessageBox.Show("Bağlantı dayandırıldı.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xəta: " + ex.Message);
            }
        }
    }
}
