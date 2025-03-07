using System;
using System.Net;
using System.Net.Sockets;

namespace MemSync
{
    class RemoteCommandExecutor
    {
        private UdpClient udpClient = null;
        public int port = 2001; // デフォルトポートを設定

        // コンストラクタ
        public RemoteCommandExecutor()
        {
        }

        public void send(string ipAddress, byte[] sendBytes)
        {
            // UdpClientを作成する
            if (udpClient == null)
            {
                // IPv6アドレスを扱うためにAddressFamily.InterNetworkV6を指定
                udpClient = new UdpClient(AddressFamily.InterNetworkV6);
            }

            IPEndPoint endPoint;

            if (IPAddress.TryParse(ipAddress, out IPAddress ip))
            {
                endPoint = new IPEndPoint(ip, port);
                Console.WriteLine("IPv6 Address is valid");
            }
            else
            {
                Console.WriteLine("Invalid IPv6 Address");
                return;
            }

            // 非同期的にデータを送信する
            udpClient.BeginSend(sendBytes, sendBytes.Length,
                endPoint,
                SendCallback, udpClient);
        }

        // データを送信した時
        private void SendCallback(IAsyncResult ar)
        {
            UdpClient udp = (UdpClient)ar.AsyncState;

            // 非同期送信を終了する
            try
            {
                udp.EndSend(ar);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("送信エラー({0}/{1})", ex.Message, ex.ErrorCode);
            }
            catch (ObjectDisposedException ex)
            {
                // すでに閉じている時は終了
                Console.WriteLine("Socketは閉じられています。");
            }
        }
    }
}