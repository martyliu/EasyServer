using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MobaServer
{
    public class TCPClientState
    {
        public TcpClient TcpClient { get; private set; }

        public byte[] Buffer { get; private set; }

        public NetworkStream NetworkStream
        {
            get { return TcpClient.GetStream(); }
        }

        public TCPClientState(TcpClient tc, byte[] buffer)
        {
            TcpClient = tc;
            Buffer = buffer;
        }


    }
}
