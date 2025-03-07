using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace MemSync
{
    class Server
    {
        private System.Net.Sockets.UdpClient udpClient = null;
        public int port = 2001;
        public Server()
        {

            //UdpClientを作成し、指定したポート番号にバインドする
            udpClient =
                new System.Net.Sockets.UdpClient(
                    System.Net.Sockets.AddressFamily.InterNetworkV6);

            udpClient.Client.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IPv6,
                System.Net.Sockets.SocketOptionName.IPv6Only,
                0);
                
            System.Net.IPEndPoint localEP =
                new System.Net.IPEndPoint(System.Net.IPAddress.IPv6Any, this.port);
            udpClient.Client.Bind(localEP);
            


            //非同期的なデータ受信を開始する
            udpClient.BeginReceive(ReceiveCallback, udpClient);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            System.Net.Sockets.UdpClient udp =
                (System.Net.Sockets.UdpClient)ar.AsyncState;

            //非同期受信を終了する
            System.Net.IPEndPoint remoteEP = null;
            byte[] rcvBytes;
            try
            {
                rcvBytes = udp.EndReceive(ar, ref remoteEP);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Console.WriteLine("受信エラー({0}/{1})",
                    ex.Message, ex.ErrorCode);
                return;
            }
            catch (ObjectDisposedException ex)
            {
                //すでに閉じている時は終了
                Console.WriteLine("Socketは閉じられています。");
                return;
            }

            //データを文字列に変換する
            string rcvMsg = System.Text.Encoding.UTF8.GetString(rcvBytes);

            //受信したデータと送信者の情報をRichTextBoxに表示する
            string displayMsg = string.Format("[{0} ({1})] > {2}",
                remoteEP.Address, remoteEP.Port, rcvMsg);
            Console.WriteLine(displayMsg);

            //再びデータ受信を開始する
            udp.BeginReceive(ReceiveCallback, udp);
        }

    }
}
