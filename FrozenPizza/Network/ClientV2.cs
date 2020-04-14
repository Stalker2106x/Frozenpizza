using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrozenPizza.Network
{
  public enum ConnectionStep
  {
    Disconnected,
    Connecting,
    SyncGamedata,
    SyncEntities,
    Handshake,
    Synced
  }
  public class ClientV2
  {
    //Server
    public static Process activeProcess;
    //Client
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
      _client.DisconnectTimeout = 60000;
      step = ConnectionStep.Disconnected;
    }

    public void connect(string host, int port)
    {
      if (step != ConnectionStep.Disconnected) return; //One connection at a time
      Console.WriteLine("Client connecting...");
      step++;
      _client.Start();
      _client.Connect(host, port, "FrozenPizza" /* text key or NetDataWriter */);
      //Disconnect handler
      _listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
      {
        step = ConnectionStep.Disconnected;

        Menu.OpenModal("Disconnected from server:" + disconnectInfo.Reason.ToString(), "Error");
        Engine.setState(GameState.Menu);
        GameMain.Reset();
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

    public static void startServer(string mapName)
    {
      ProcessStartInfo startInfo = new ProcessStartInfo();

      startInfo.Arguments = "exec Server.dll Data/maps/" + mapName + ".tmx";
      startInfo.FileName = "dotnet";
      try
      {
        activeProcess = Process.Start(startInfo);
      }
      catch (Exception e)
      {
        //Error
      }
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
