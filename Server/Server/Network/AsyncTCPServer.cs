using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;

namespace MobaServer
{


    public class AsyncTCPServer : IDisposable
    {
        private TcpListener _listener;
        private List<TCPClientState> _clients;

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


            _clients = new List<TCPClientState>();
            _listener = new TcpListener(IpAddress, Port);
            //_listener.AllowNatTraversal(true);
        }

        public void Start()
        {
            if(!IsRunning)
            {
                IsRunning = true;
                _listener.Start();
                _listener.BeginAcceptTcpClient(new AsyncCallback(HandleTcpClientAccepted), _listener);
                Console.WriteLine("listening... ");
            }
        }

        private void HandleTcpClientAccepted(IAsyncResult ar)
        {
            if(IsRunning)
            {
                TcpClient client = _listener.EndAcceptTcpClient(ar);
                byte[] buffer = new byte[client.ReceiveBufferSize];

                TCPClientState state = new TCPClientState(client, buffer);
                lock(_clients)
                {
                    _clients.Add(state);
                    OnClientConnected(state);
                }

                NetworkStream stream = state.NetworkStream;
                stream.BeginRead(state.Buffer, 0, state.Buffer.Length, HandleDataReceived, state);

                _listener.BeginAcceptTcpClient(new AsyncCallback(HandleTcpClientAccepted), ar.AsyncState);
            }
        }

        void HandleDataReceived(IAsyncResult ar)
        {
            if(IsRunning)
            {
                TCPClientState state = (TCPClientState)ar.AsyncState;
                NetworkStream stream = state.NetworkStream;

                int recv = 0;
                try
                {
                    recv = stream.EndRead(ar);
                }catch
                {
                    recv = 0;
                }

                if(recv == 0)
                {
                    Console.WriteLine("client out ");
                    lock(_clients)
                    {
                        _clients.Remove(state);
                        OnClientDisconnected(state);
                    }
                    return;
                }

                byte[] buffer = new byte[recv];
                Buffer.BlockCopy(state.Buffer, 0, buffer, 0, recv);

                HandleMessage(buffer, state);

                stream.BeginRead(state.Buffer, 0, state.Buffer.Length, HandleDataReceived, state);
            }
        }

        void SendFinish(IAsyncResult ar)
        {
            Console.WriteLine("Send finish");
        }

        /// <summary>
        /// 直接转发，模拟延迟
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="state"></param>
        void HandleMessage(byte[] buffer, TCPClientState state)
        {
            //Console.WriteLine("receive mess");
            state.NetworkStream.BeginWrite(buffer, 0, buffer.Length, SendFinish, state);
            
            /*

            Console.WriteLine("get message");
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(JsonWrapData));
            JsonWrapData data = deserializer.ReadObject(new MemoryStream(buffer)) as JsonWrapData;

            if(data.protocolName == "moveInput")
            {
                //Console.Write(data.jsonData);
                
                deserializer = new DataContractJsonSerializer(typeof(Vector2List));
                var dataBuffer = System.Text.Encoding.Default.GetBytes(data.jsonData);
                var input = deserializer.ReadObject(new MemoryStream(dataBuffer)) as Vector2List;
                foreach(var i in input.data)
                {
                    Console.WriteLine(i.x + ", " + i.y);
                }

            }
            */
        }

        void OnClientConnected(TCPClientState state )
        {
            Console.WriteLine("On client connect! ");
        }

        void OnClientDisconnected(TCPClientState state)
        {

        }
        

        public void Dispose()
        {
            
        }
    }

    [DataContract]
    public class Vector2
    {
        [DataMember(Order = 0)]
        public float x;
        [DataMember(Order = 1)]
        public float y;
    }


    [DataContract]
    public class JsonWrapData
    {
        [DataMember( Order = 0)]
        public string protocolName;
        [DataMember(Order = 1)]
        public string jsonData;

    }
}
