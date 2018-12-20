using System;
using System.Net;
using System.Net.Sockets;

namespace SimpleServer
{
    class Program
    {
        static void Main(string[] args)
        {

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 6666);
            
            Console.WriteLine("Hello World!");
        }
    }
}
