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
        static TcpClient _client;
        static Thread _thread;
        CommandHandler _cmdHandle;
        public static NetworkStream _stream;
        public String Ip { get; set; }
        public int Port { get; set; }
        public static bool Connected { get; set; }
        public bool GetData { get; set; }
        public static String ConnectionStatus { get; set; }
        public bool Hooked { get; set; }
        public bool Handshake { get; set; }
        public NetHandler()
        {
            _client = new TcpClient();
            _cmdHandle = new CommandHandler();
            Handshake = false;
            Hooked = false;
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

        public static void disconnect()
        {
            ConnectionStatus = "Disconnected.";
            _client.Close();
            _thread.Abort();
        }

        public static void send(String msg)
        {
            byte[] buffer = new byte[msg.Length];

            buffer = Encoding.UTF8.GetBytes(msg);
            _stream.Write(buffer, 0, buffer.Length);
        }

        public static String receive()
        {
            int receivingBufferSize = (int)_client.ReceiveBufferSize;
            byte[] buffer = new byte[receivingBufferSize];
            int readCount = 0;
            String msg;

            try
            {
                readCount = _stream.Read(buffer, 0, receivingBufferSize);
            }
            catch (System.IO.IOException e)
            {
                disconnect();
                return (null);
            }
            if (readCount <= 0)
                return (null);
            msg = Encoding.UTF8.GetString(buffer, 0, readCount);
            return (msg);
        }

        bool getServerData()
        {
            String msg;

            send("?PLAYERS");
            msg = receive();
            while (!_cmdHandle.ParseExpectedCmd(msg, ".OK"))
            {
                _cmdHandle.ParseCmd(msg);
                msg = receive();
            }
            send("?WORLD");
            GetData = false;
            return (true);
        }

        void threadLoop()
        {
            ConnectCallback();
            while (Connected)
            {
                if (GetData && !getServerData())
                    _thread.Abort();
                String msg = receive();

                if (msg != null)
                    _cmdHandle.ParseCmd(msg);
            }
        }
    }
}
