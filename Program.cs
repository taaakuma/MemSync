﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace MemSync
{
    class Program
    {
        static void Main(string[] args)
        {
            //Windows 2001:f75:92a0:810:d326:f5fb:989e:62d3
            //Mac 2001:f75:92a0:810:c45:d054:dc87:7930
            foreach (var ipv6 in GetLocalIPv6Addresses())
            {
                Console.WriteLine($"IPv6 Address: {ipv6}");
            }
            Server server = new Server();
            var rce = new RemoteCommandExecutor();
            Console.Write("input ipAddress: ");
            string ip = Console.ReadLine();
            Console.WriteLine($"ip: {ip}");
            while(true) {
                string message = Console.ReadLine();
                //rce.send(ip,System.Text.Encoding.UTF8.GetBytes(message));
                Socket socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IPAddress.Parse(ip), 2001));
                socket.Disconnect(false);
                socket.Dispose();
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
