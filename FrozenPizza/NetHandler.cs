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

        }

        public String receive()
        {
            String msg = "";

            return (msg);
        }

        void threadLoop()
        {
            ConnectCallback();
            while (Connected)
            {
            }
        }
    }
}
