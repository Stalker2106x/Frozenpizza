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

        //Game
        bool _running;
        String _mapName;
        static int Main(string[] args)
        {
            Server server;

            if (args.Length < 1)
                return (-1);
            server = new Server(args[0]);
            server.Start();
            while (server.isRunning())
                server.mainLoop();
            return (0);
        }
        public bool isRunning()
        {
            return (_running);
        }

        public Server(String mapName)
        {
            //Server
            _port = 27420;
            _localAddr = IPAddress.Parse("127.0.0.1");
            _server = new TcpListener(_localAddr, _port);
            _client = default(TcpClient);
            _connections = 0;
            //Game
            _running = false;
            _mapName = "maps/world.tmx";
            Console.Write("Starting FrozenPizza Server...\n");
        }

        public void Start()
        {
            _server.Start();
            _running = true;
            Console.Write("DONE!\n");
            Console.WriteLine("The server is running on port " + _port + "...");
        }

        public void mainLoop()
        {
            try
            {
                _client = _server.AcceptTcpClient();
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket error occured!");
            }
            Console.WriteLine(" >> " + "Client connected with ID " + _connections + "!");
            NetCli client = new NetCli(_client, _connections);
            client.startClient();
            _connections++;
        }
    }
}
