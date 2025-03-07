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
            foreach (var ipv6 in GetLocalIPv6Addresses())
            {
                Console.WriteLine($"IPv6 Address: {ipv6}");
            }
            string select;
            while (true)
            {
                select = Console.ReadLine();
                if (select == "s" || select == "c") break;
            }
            if (select == "s")
            {
                Server server = new Server();
                while (true)
                {

                }
            }
            else
            {
                RemoteCommandExecutor rce = new RemoteCommandExecutor();
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
