using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrozenPizza.Network
{
  public enum ConnectionStep
  {
    Begin,
    SyncGamedata,
    SyncEntities,
    Handshake,
    Synced
  }
  public class ClientV2
  {
    private EventBasedNetListener _listener;
    private NetManager _client;
    private Task _runTask;
    private bool _quit;

    public static ConnectionStep step;

    public ClientV2()
    {
      _quit = false;
      _listener = new EventBasedNetListener();
      _client = new NetManager(_listener);
      step = ConnectionStep.Begin;
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
      _quit = true;
      _runTask.Wait();
    }

    public void run()
    {
      _runTask = Task.Run(() =>
      {
        while (!_quit)
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
