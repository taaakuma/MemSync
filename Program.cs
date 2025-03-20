using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace MemSync
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager.ParseCommands(
                [
                    2,  //命令数
                    00,00,00,00,00,00,00,00, //経過時間
                        255, //オペコード
                        03,  //引数の個数
                            02,00,00,00,00,00,00,00, //引数の長さ
                            55,22,  //引数

                            03,00,00,00,00,00,00,00,
                            255,34,00,

                            01,00,00,00,00,00,00,00,
                            10,
                        100,
                        01,
                            01,00,00,00,00,00,00,00,
                            31
                ]);
            return;
            //Windows 2001:f75:92a0:810:d326:f5fb:989e:62d3
            //Mac 2001:f75:92a0:810:c45:d054:dc87:7930
            foreach (var ipv6 in GetLocalIPv6Addresses())
            {
                Console.WriteLine($"IPv6 Address: {ipv6}");
            }
            
            var rce = new RemoteCommandExecutor();
            Server server = new Server();

            Console.Write("input ipAddress: ");
            string ip = Console.ReadLine();
            Console.WriteLine($"ip: {ip}");
            while(true) {
                string message = Console.ReadLine();
                rce.send(ip,System.Text.Encoding.UTF8.GetBytes(message));

            }
            Console.WriteLine("実行が終わりました。");

        }

        static IPAddress[] GetLocalIPv6Addresses()
        {
            return Array.FindAll(
                Dns.GetHostAddresses(Dns.GetHostName()),
                ip => ip.AddressFamily == AddressFamily.InterNetworkV6 && !ip.IsIPv6LinkLocal
            
            );
        }
    }
}
