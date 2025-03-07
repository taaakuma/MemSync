using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace MemSync
{
    class RemoteCommandExecutor
    {
        private System.Net.Sockets.UdpClient udpClient = null;
        public int port = 0;
        //Button2のClickイベントハンドラ
        //データを送信する
        public RemoteCommandExecutor()
        {
        }
        public void send(string ipAddress,byte[] sendBytes)
        {
            //UdpClientを作成する
            if (udpClient == null)
            {
                udpClient = new System.Net.Sockets.UdpClient();
            }

            //非同期的にデータを送信する
            udpClient.BeginSend(sendBytes, sendBytes.Length,
                ipAddress, 2001,
                SendCallback, udpClient);
        }

        //データを送信した時
        private void SendCallback(IAsyncResult ar)
        {
            System.Net.Sockets.UdpClient udp =
                (System.Net.Sockets.UdpClient)ar.AsyncState;

            //非同期送信を終了する
            try
            {
                udp.EndSend(ar);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Console.WriteLine("送信エラー({0}/{1})",
                    ex.Message, ex.ErrorCode);
            }
            catch (ObjectDisposedException ex)
            {
                //すでに閉じている時は終了
                Console.WriteLine("Socketは閉じられています。");
            }
        }


    }
}
