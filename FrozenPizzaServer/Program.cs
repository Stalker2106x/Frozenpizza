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
        public static List<NetCli> ClientList;
        IPAddress _localAddr;
        static String _ip;
        int _connections;

        //Game
        bool _running;
        String _mapName;
        public static Level Level { get; set; }
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

        public static void broadcast(int senderId, String msg)
        {
            for (int i = 0; i < ClientList.Count; i++)
            {
                if (i == senderId || !ClientList[i].Ready)
                    continue;
                ClientList[i].send(msg);
            }
        }

        public Server(String mapName)
        {
            //Server
            _port = 27420;
            _localAddr = IPAddress.Any;
            _ip = _localAddr.ToString();
            _server = new TcpListener(_localAddr, _port);
            _client = default(TcpClient);
            _connections = 0;
            ClientList = new List<NetCli>();
            //Game
            _running = false;
			_mapName = mapName;
            Console.Write("Starting FrozenPizza Server...\n");
        }

        public void Start()
        {
            _server.Start();
            _running = true;
            Level = new Level(_mapName);
            Console.Write("DONE!\n");
            Console.Write("Listening on IP: " + _ip + "\n");
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
            ClientList.Add(new NetCli(_client, _connections));
            ClientList.Last().startClient();
            _connections++;
        }
    }
}
