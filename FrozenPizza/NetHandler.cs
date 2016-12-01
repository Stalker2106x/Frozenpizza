using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class NetHandler
    {
        TcpClient _client;
        Thread _thread;
        public String Ip { get; set; }
        public int Port { get; set; }
        public static bool Connected { get; set; }
        public static String ConnectionStatus { get; set; }
        public NetHandler()
        {
            _client = new TcpClient();
            Connected = false;
            ConnectionStatus = "Disconnected.";
        }

        private static void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                TcpClient client = (TcpClient)asyncResult.AsyncState;
                client.EndConnect(asyncResult);
                ConnectionStatus = "Connected!";
                Connected = true;
            }
            catch (SocketException e)
            {
                ConnectionStatus = "Cannot connect to server.";
            }
        }
        public void connect(String ip, int port)
        {
            if (!_client.Connected)
            {
                ConnectionStatus = "Connecting...";
                _thread = new Thread(threadLoop);
            }
        }

        public void disconnect()
        {
            _thread.Abort();
        }

        public void send()
        {

        }

        public String receive()
        {
            String msg = "";

            return (msg);
        }

        void threadLoop()
        {
            _client.Connect(Ip, Port);
            while (Connected)
            {
            }
        }
    }
}
