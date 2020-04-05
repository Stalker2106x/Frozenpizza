using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrozenPizza.Network
{
  public class ClientV2
  {
    EventBasedNetListener _listener;
    NetManager _client;
    Task _runTask;

    public ClientV2()
    {
      _listener = new EventBasedNetListener();
      _client = new NetManager(_listener);
      ClientHandlerV2.Reset();
    }
    public void connect(string host, int port)
    {
      Console.WriteLine("Client connecting...");
      _client.Start();
      _client.Connect(host, port, "SomeConnectionKey" /* text key or NetDataWriter */);
      //Disconnect handler
      _listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
      {
        Console.WriteLine("Disconnected: (ORG:{0}) {1}", peer.EndPoint, disconnectInfo.Reason.ToString());
        Engine.setState(GameState.Menu);
        GameMain.Unload();
      };
      _listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) => { ClientHandlerV2.Parse(fromPeer, dataReader, deliveryMethod); };
      run();
    }

    public void disconnect()
    {
      _client.Stop();
    }

    public void run()
    {
      _runTask = Task.Run(() =>
      {
        while (true)
        {
          _client.PollEvents();
          Thread.Sleep(15);
        }
      });
    }

    public void send(NetDataWriter writer, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
    {
      _client.SendToAll(writer, method);
    }

  }
}
