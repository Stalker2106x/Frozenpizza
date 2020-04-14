using FrozenPizza;
using FrozenPizza.Entities;
using FrozenPizza.World;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using Server;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
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
    private bool _quit;

    //Game
    public static List<Projectile> projectiles;
    public static ServerMap map;
    public static Dictionary<NetPeer, BasePlayer> players;

    public ServerV2(string mapName)
    {
      _listener = new EventBasedNetListener();
      _server = new NetManager(_listener);
      _clients = new List<NetPeer>();
      map = new ServerMap(mapName);
      projectiles = new List<Projectile>();
      players = new Dictionary<NetPeer, BasePlayer>();
    }

    public void start(int port, int slots)
    {
      _server.Start(port);
      _server.DisconnectTimeout = 60000;

      //Connection Request handler
      _listener.ConnectionRequestEvent += request =>
      {
        if (_server.PeersCount < slots)
        {
          request.AcceptIfKey("FrozenPizza");
        }
        else
        {
          NetDataWriter writer = new NetDataWriter();
          writer.Put("Server is full");
          request.Reject(writer);
        }
      };
      //Connect handler
      _listener.PeerConnectedEvent += peer =>
      {
        _clients.Add(peer);
        Console.WriteLine("New client connected: {0}", peer.EndPoint);
        players.Add(peer, new BasePlayer(peer.Id, "Player"+ peer.Id, 100, new Vector2(10, 10)));
        NetDataWriter writer = new NetDataWriter();
        var payload = new { version = Assembly.GetExecutingAssembly().GetName().Version.ToString() };
        writer.Put(".WELCOME " + JsonConvert.SerializeObject(payload));
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
      _quit = true;
    }

    public void run()
    {
      Console.WriteLine("Server Listening...");
      while (!_quit)
      {
        _server.PollEvents();
        update(Program.gameTime);
        Program.gameTime.Update();
        Thread.Sleep(15);
      }
    }

    public void update(ServerTime gameTime)
    {
      for (int i = projectiles.Count - 1; i >= 0; i--)
      {
        if (!projectiles[i].Update(gameTime)) projectiles.RemoveAt(i);
      }
    }

    public void broadcast(NetPeer owner, NetDataWriter writer, DeliveryMethod method)
    {
      if (owner == null) _server.SendToAll(writer, method);
      else _server.SendToAll(writer, method, owner); //Broadcast to all but owner
    }
  }
}
