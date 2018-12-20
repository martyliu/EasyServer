using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Network
{
    public class AsyncTCPServer : IDisposable
    {
        private TcpListener _listener;
        private List<Object> _clients;

        public IPAddress IpAddress { get; private set; }
        public int Port { get; private set; }

        bool disposed = false;

        public bool IsRunning { get; private set; } = false;

        public AsyncTCPServer(int listenPort) 
        {

        }

        public AsyncTCPServer(IPEndPoint localEP) : this(localEP.Address, localEP.Port)
        {

        }

        public AsyncTCPServer(IPAddress localIPAddress, int listenPort)
        {
            IpAddress = localIPAddress;
            Port = listenPort;

            
            _clients = new List<object>();
            _listener = new TcpListener(IpAddress, Port);
            //_listener.AllowNatTraversal(true);
        }

        public void Start()
        {
            
        }

        

        public void Dispose()
        {
            
        }
    }
}
