using FrozenPizza;
using FrozenPizza.Entities;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using Server.Payloads;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;

namespace FPServer
{
  public enum ConnectionStep
  {
    Begin,
    SyncGamedata,
    SyncDynamic,
    Handshake,
    Synced
  }
  public class NetworkClient
  {
    public NetPeer peer;
    public ConnectionStep step;
  }

  public class ServerHandlerV2
  {
    public static Dictionary<string, Action<NetPeer, string>> commands;

    public static void Initialize()
    {
      commands = new Dictionary<string, Action<NetPeer, string>>()
      {
        {"?GAMEDATA", SendGamedata},
        {"?ENTITIES", SendEntities},
        {".HANDSHAKE", SendHandshake},
        {".PLAYER", UpdatePlayer},
        {".ITEM", UpdateItem}
      };
    }
    public static void Parse(NetPeer client, NetPacketReader reader, DeliveryMethod method)
    {
      string command = reader.GetString(2048 /* max length of string */);
      string body = string.Empty;
      int separatorIndex = command.IndexOf(" ");

      reader.Recycle();
      if (separatorIndex != -1) //Handle body
      {
        body = command.Remove(0, separatorIndex + 1);
        command = command.Substring(0, separatorIndex);
      }
      try {
        commands[command](client, body);
      } catch (Exception e) {
        Console.WriteLine("Unhandled command:"+ e.Message);
      }
    }

    public static void SendGamedata(NetPeer client, string body) //!GAMEDATA
    {
      NetDataWriter writer = new NetDataWriter();
      GameData payload = new GameData(client.Id, ServerV2.map.name);

      writer.Put("!GAMEDATA "+ JsonConvert.SerializeObject(payload));
      client.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public static void SendEntities(NetPeer client, string body) //!ENTITIES
    {
      NetDataWriter writer = new NetDataWriter();
      EntitiesData payload = new EntitiesData(client.Id, ServerV2.players.Values.ToList(), ServerV2.map.items);

      writer.Put("!ENTITIES "+JsonConvert.SerializeObject(payload));
      client.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public static void SendHandshake(NetPeer client, string body) //.HANDSHAKE
    {
      //Client
      NetDataWriter writer = new NetDataWriter();

      writer.Put(".HANDSHAKE");
      client.Send(writer, DeliveryMethod.ReliableOrdered);
      //Propagate
      BasePlayer player = ServerV2.players[client];
      NewPlayerData payload = new NewPlayerData(player.name, player.hp, new PlayerData(player.id, player.position, player.orientation));
      writer.Reset();
      writer.Put(".NEWPLAYER "+JsonConvert.SerializeObject(payload));
      Program.server.broadcast(client, writer, DeliveryMethod.ReliableOrdered);
    }

    public static void UpdatePlayer(NetPeer client, string body) //.PLAYER
    {
      NetDataWriter writer = new NetDataWriter();
      PlayerData payload = JsonConvert.DeserializeObject<PlayerData>(body);

      ServerV2.players[client].position = new Vector2(payload.x, payload.y);
      writer.Put(".PLAYER " + body);
      Program.server.broadcast(client, writer, DeliveryMethod.Unreliable);
    }

    public static void UpdateItem(NetPeer client, string body) //.ITEM
    {
      NetDataWriter writer = new NetDataWriter();
      ItemData payload = JsonConvert.DeserializeObject<ItemData>(body);

      BaseItem item = ServerV2.map.items.Find((it) => { return (it.uid == payload.uid); });
      if (payload.onmap) item.position = new Point(payload.x, payload.y);
      else item.position = null;

      writer.Put(".ITEM " + body);
      Program.server.broadcast(client, writer, DeliveryMethod.ReliableUnordered);
    }
  }
}
