﻿using FrozenPizzaServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FrozenPizzaServer
{
  class Server
  {
    //Server
    int _port;
    TcpListener _server;
    TcpClient _client;
    public static List<NetCli> ClientList;
    IPAddress _localAddr;
    static String _ip;
    public static String MapName;
    int _connections;

    //Game
    bool _running;
    public static Level Level { get; set; }
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
      MapName = mapName;
      Console.Write("Starting FrozenPizza Server...\n");
    }
    public bool isRunning()
    {
      return (_running);
    }

    public static void broadcast(int senderId, String msg)
    {
      for (int i = ClientList.Count - 1; i >= 0; i--)
      {
        if (i == senderId || ClientList[i] == null || ClientList[i].Player == null || !ClientList[i].Ready)
          continue;
        ClientList[i].send(msg);
      }
    }

    public void Start()
    {
      _server.Start();
      _running = true;
      Level = new Level(MapName);
      Level.startUpdateThread();
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
      _client.NoDelay = true;
      ClientList.Add(new NetCli(_client, _connections));
      ClientList.Last().startClient();
      _connections++;
    }
  }
}
