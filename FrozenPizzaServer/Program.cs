using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{
    public class Server
    {
        //Server
        int _port;
        TcpListener _server;
        TcpClient _client;
        IPAddress _localAddr;
        int _connections;

        //Data
        Byte[] _bytes;
        String _data;

        //Game
        bool _running;
        String _mapName;
        static void Main(string[] args)
        {
            Server server = new Server(args[0]);

            server.Start();
            while (server.isRunning())
                server.Update();
        }
        public bool isRunning()
        {
            return (_running);
        }

        public Server(String mapName)
        {
            //Server
            _port = 13000;
            _localAddr = IPAddress.Parse("127.0.0.1");
            _server = new TcpListener(_localAddr, _port);
            _client = default(TcpClient);
            _connections = 0;
            //Game
            _running = false;
            _mapName = "maps/world.tmx";
            Console.Write("Starting FrozenPizza Server...\n");
            // Buffer for reading data
             _bytes = new Byte[1024];
             _data = null;
        }

        public void Start()
        {
            _server.Start();
            _running = true;
            Console.Write("DONE!\n");
            Console.WriteLine("The server is running on port " + _port + "...");
        }

        public void Update()
        {
            _client = _server.AcceptTcpClient();
            Console.WriteLine(" >> " + "Client connected with ID " + _connections + "!");
            netCli client = new netCli(_client, _connections);
            client.startClient();
            _connections++;
        }
    }

    public class netCli
    {
        TcpClient _client;
        Thread _cThread;
        NetworkStream _stream;
        byte[] _buffer;
        int _id;
        String _name;

        public netCli(TcpClient inClientSocket, int cliId)
        {
            _client = inClientSocket;
            _id = cliId;
            _buffer = new byte[1024];
        }

        public void startClient()
        {
            _stream = _client.GetStream();
            _cThread = new Thread(Update);
            _cThread.Start();
        }

        public void terminateClient()
        {
            _cThread.Abort();
        }

        private bool Enumerate()
        {
            return (true);
        }

        private bool isConnected()
        {
            if (_client.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] buff = new byte[1];
                if (_client.Client.Receive(buff, SocketFlags.Peek) == 0)
                {
                    // Client disconnected
                    return (false);
                }
            }
            return (true);
        }

        private async void Update()
        {
            while (isConnected())
            {
                int readCount = await _stream.ReadAsync(_buffer, 0, 1024);
                String msg;

                msg = Encoding.UTF8.GetString(_buffer, 0, readCount);
                Console.WriteLine(msg);
            }
        }

        private void WriteData()
        {

        }
    }
}
