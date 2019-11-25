
using System;
using System.Net;
using System.Net.Sockets;

namespace MobaServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 6666);
            AsyncTCPServer server = new AsyncTCPServer(ip);
            server.Start();

            while(true)
            {
                var input = Console.ReadLine();
                if (input == "Q" || input == "q")
                    break;
            }
            
            Console.WriteLine("Hello World!");
        }
    }
}
