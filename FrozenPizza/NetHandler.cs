using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class NetHandler
    {
        TcpClient _client;
        NetworkStream _stream;
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

        void ConnectCallback()
        {
            try
            {
                _client.Connect(Ip, Port);
                _stream = _client.GetStream();
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
                Ip = ip;
                Port = port;
                _thread = new Thread(threadLoop);
                _thread.Start();
            }
        }

        public void disconnect()
        {
            _thread.Abort();
        }

        public void sendWhois()
        {

        }
        public bool handShake()
        {
            if (receive() == ".WELCOME")
                send(".ACK");
            else
                return (false);
            if (receive() == "?VERSION")
                send("!VERSION " + Assembly.GetEntryAssembly().GetName().Version.ToString());
            else
                return (false);
            if (receive() == "?WHOIS")
                sendWhois();
            else
                return (false);
            if (receive() == ".OK")
                return (true);
            return (false);


        }

        public void send(String msg)
        {
            byte[] buffer = new byte[msg.Length];

            buffer = Encoding.UTF8.GetBytes(msg);
            _stream.Write(buffer, 0, buffer.Length);
        }

        public String receive()
        {
            int receivingBufferSize = (int)_client.ReceiveBufferSize;
            byte[] buffer = new byte[receivingBufferSize];
            int readCount = _stream.Read(buffer, 0, receivingBufferSize);
            String msg;

            if (readCount <= 0)
                return ("");
            msg = Encoding.UTF8.GetString(buffer, 0, readCount);
            return (msg);
        }

        void threadLoop()
        {
            ConnectCallback();
            if (Connected)
            {
                handShake();
            }
            while (Connected)
            {
            }
        }
    }
}
