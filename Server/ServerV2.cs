using FrozenPizza;
using FrozenPizza.World;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FPServer
{
  public class ServerV2
  {
    EventBasedNetListener _listener;
    List<NetPeer> _clients;
    NetManager _server;

    //Game
    public static ServerMap map;
    public static Dictionary<NetPeer, BasePlayer> players;

    public ServerV2(string mapName)
    {
      _listener = new EventBasedNetListener();
      _server = new NetManager(_listener);
      _clients = new List<NetPeer>();
      map = new ServerMap(mapName);
      players = new Dictionary<NetPeer, BasePlayer>();
    }

    public void start(int port, int slots)
    {
      _server.Start(port);

      //Connection Request handler
      _listener.ConnectionRequestEvent += request =>
      {
        if (_server.PeersCount < slots)
          request.AcceptIfKey("SomeConnectionKey");
        else
          request.Reject();
      };
      //Connect handler
      _listener.PeerConnectedEvent += peer =>
      {
        _clients.Add(peer);
        Console.WriteLine("New client connected: {0}", peer.EndPoint);
        players.Add(peer, new BasePlayer(peer.Id, "Player"+ peer.Id, 100, new Vector2(10, 10)));
        NetDataWriter writer = new NetDataWriter();
        writer.Put(".WELCOME");
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
      };
      //Disconnect handler
      _listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
      {
        Console.WriteLine("Client disconnected: {0}", peer.EndPoint);
        players.Remove(peer);
        _clients.Remove(peer);
        NetDataWriter writer = new NetDataWriter();
        writer.Put(".RMPLAYER "+peer.Id);
        broadcast(peer, writer, DeliveryMethod.ReliableOrdered);
      };
      _listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) => { ServerHandlerV2.Parse(fromPeer, dataReader, deliveryMethod); };
      run();
    }

    public void stop()
    {
      _server.Stop();
    }

    public void run()
    {
      Console.WriteLine("Server Listening...");
      while (true)
      {
        _server.PollEvents();
        Thread.Sleep(15);
      }
    }

    public void broadcast(NetPeer owner, NetDataWriter writer, DeliveryMethod method)
    {
      _server.SendToAll(writer, method, owner); //Broadcast to all but owner
    }
  }
}
