using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        static Queue<String> _receiveStack;
        CommandHandler _cmdHandle;

        public static Action FailureCallback;
        public static Action HandshakeCallback;
        static NetworkStream _stream;
        public String Ip { get; set; }
        public int Port { get; set; }
        public static bool Connected { get; set; }
        public bool GameReady { get; set; }
        public bool WorldData { get; set; }
        public static String ConnectionStatus { get; set; }
        public bool Ready { get; set; }
        public bool Handshake { get; set; }

        public NetHandler()
        {
            _client = new TcpClient();
            _client.NoDelay = true;
            _cmdHandle = new CommandHandler();
            _receiveStack = new Queue<string>();
            Connected = false;
            Handshake = false;
            GameReady = false;
            WorldData = false;
            Ready = false;
            ConnectionStatus = "Disconnected.";
        }

        bool ConnectCallback()
        {
            try
            {
                _client.Connect(Ip, Port);
                _stream = _client.GetStream();
                ConnectionStatus = "Connected!";
                Connected = true;
                return (true);
            }
            catch (SocketException e)
            {
                ConnectionStatus = "Cannot connect to server.";
            }
            return (false);
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

        public static void startServer(String mapName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.Arguments = "Data/maps/" + mapName + ".tmx";
            startInfo.FileName = "FrozenPizzaServer.exe";
            try {
                Process.Start(startInfo);
            } catch (Exception e) {
                //Error
            }
        }

        public static String sendAndReceive(String msg)
        {
            send(msg);
            return (receive());
        }
        public static void send(String msg)
        {
            byte[] buffer = new byte[msg.Length];

            buffer = Encoding.UTF8.GetBytes(msg + "\r\n");
            try
            {
                _stream.Write(buffer, 0, buffer.Length);
                _stream.Flush();
            }
            catch (System.Exception e)
            {
                disconnect();
            }
        }

        public static String receive()
        {
            int receivingBufferSize = (int)_client.ReceiveBufferSize;
            byte[] buffer = new byte[receivingBufferSize];
            int readCount = 0;
            String msg, msgprocess;

            if (_receiveStack.Count != 0) //If we have loads of messages to treat
                return (_receiveStack.Dequeue());
            try
            {
                readCount = _stream.Read(buffer, 0, receivingBufferSize);
            }
            catch (System.Exception e)
            {
                disconnect();
                return (null);
            }
            if (readCount == 0)
                disconnect();
            msg = Encoding.UTF8.GetString(buffer, 0, readCount);
            msg = msg.Substring(0, msg.IndexOf("\r\n"));
            if (msg.IndexOf("\r\n") != msg.Length - 2)
            {
                msgprocess = msg;
                while (msgprocess.IndexOf("\r\n") != -1)
                {
                    msgprocess = msgprocess.Remove(0, msgprocess.IndexOf("\r\n") + 2);
                    _receiveStack.Enqueue(msgprocess.Substring(0, msgprocess.IndexOf("\r\n")));
                }
            }
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
            msg = receive();
            while (!_cmdHandle.ParseExpectedCmd(msg, ".OK"))
            {
                _cmdHandle.ParseCmd(msg);
                msg = receive();
            }
            WorldData = true;
            return (true);
        }

        void threadLoop()
        {
            if (!ConnectCallback()) NetHandler.FailureCallback();
            while (Connected)
            {
                if (!WorldData && Handshake && GameReady)
                    getServerData();
                String msg = receive();

                if (msg != null)
                    _cmdHandle.ParseCmd(msg);
            }
        }
    }
}
