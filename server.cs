using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MemSync
{
    class Server
    {
        private UdpClient udpClient;
        public int port = 2001;

        public Server()
        {
            udpClient = new UdpClient(AddressFamily.InterNetworkV6);
            udpClient.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0);
            udpClient.Client.Bind(new IPEndPoint(IPAddress.IPv6Any, port));

            // 複数の受信処理を開始（並行処理）
            for (int i = 0; i < 5; i++) // 例として5個の並行タスク
            {
                Task.Run(ReceiveLoop);
            }
        }

        private async Task ReceiveLoop()
        {
            while (true)
            {
                try
                {
                    UdpReceiveResult result = await udpClient.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(result.Buffer);

                    Console.WriteLine($"[{result.RemoteEndPoint.Address} ({result.RemoteEndPoint.Port})] > {message}");
                }
                catch (ObjectDisposedException)
                {
                    Console.WriteLine("Socketは閉じられました。");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"受信エラー: {ex.Message}");
                }
            }
        }
    }
}