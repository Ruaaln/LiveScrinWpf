using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.Title = "📡 Screenshot Sender";

        Console.Write("Server IP (boş → 127.0.0.1): ");
        string ip = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(ip)) ip = "127.0.0.1";

        Console.Write("Port (boş → 8080): ");
        string portStr = Console.ReadLine();
        int port = string.IsNullOrWhiteSpace(portStr) ? 8080 : int.Parse(portStr);

        Console.WriteLine($"Bağlantı: {ip}:{port}");
        Console.WriteLine("Enter basıldıqda canlı yayım başlayacaq...");

        while (true)
        {
            Console.WriteLine("▶ Göndərmə başladı...");
            SendLive(ip, port);
        }
    }

    static void SendLive(string ip, int port)
    {
        try
        {
            while (!Console.KeyAvailable) // Enter basılana qədər davam et
            {
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                using Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);

                using MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Jpeg);
                byte[] data = ms.ToArray();

                using TcpClient client = new TcpClient(ip, port);
                using NetworkStream ns = client.GetStream();

                byte[] sizeInfo = BitConverter.GetBytes(data.Length);
                ns.Write(sizeInfo, 0, 4);
                ns.Write(data, 0, data.Length);

                Thread.Sleep(100); // saniyədə təxminən 10 şəkil
            }

            Console.ReadKey(true); // Enter-i sil
            Console.WriteLine("⏹ Yayım dayandırıldı.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Xəta: " + ex.Message);
        }
    }
}
