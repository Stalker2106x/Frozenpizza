using FrozenPizza;
using FrozenPizza.Entities;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using Server;
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
        {"?RESPAWN", SendSpawn},
        {".HANDSHAKE", SendHandshake},
        {".PLAYER", UpdatePlayer},
        {".ITEM", UpdateItem},
        {".PROJECTILE", UpdateProjectile},
        {".MELEE", UpdateMelee}
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
      GameData payload = new GameData(client.Id, ServerV2.map.name, new Point(1445, 1090));

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
      FullPlayerData payload = new FullPlayerData(player.name, player.active, player.hp, new PlayerData(player.id, player.position, player.orientation));
      writer.Reset();
      writer.Put(".NEWPLAYER "+JsonConvert.SerializeObject(payload));
      Program.server.broadcast(client, writer, DeliveryMethod.ReliableOrdered);
    }

    public static void SendSpawn(NetPeer client, string body) //!GAMEDATA
    {
      NetDataWriter writer = new NetDataWriter();
      BasePlayer player = ServerV2.players[client];
      FullPlayerData payload = new FullPlayerData(player.name, player.active, player.hp, new PlayerData(player.id, new Vector2(1000, 1000), player.orientation));

      writer.Put("!FPLAYER " + JsonConvert.SerializeObject(payload));
      Program.server.broadcast(null, writer, DeliveryMethod.ReliableUnordered);
    }

    public static void UpdatePlayer(NetPeer client, string body) //.PLAYER
    {
      NetDataWriter writer = new NetDataWriter();
      PlayerData payload = JsonConvert.DeserializeObject<PlayerData>(body);

      BasePlayer player = ServerV2.players[client];
      player.position = new Vector2(payload.x, payload.y);
      player.orientation = payload.orientation;
      writer.Put(".PLAYER " + body);
      Program.server.broadcast(client, writer, DeliveryMethod.Unreliable);
    }

    public static void UpdateItem(NetPeer client, string body) //.ITEM
    {
      Console.WriteLine(body);
      NetDataWriter writer = new NetDataWriter();
      ItemData payload = JsonConvert.DeserializeObject<ItemData>(body);

      BaseItem item = ServerV2.map.items.Find((it) => { return (it.uid == payload.uid); });
      item.position = payload.GetPosition();

      writer.Put(".ITEM " + body);
      Program.server.broadcast(client, writer, DeliveryMethod.ReliableUnordered);
    }

    public static void UpdateProjectile(NetPeer client, string body) //.PROJECTILE
    {
      Console.WriteLine(body);
      ProjectileData payload = JsonConvert.DeserializeObject<ProjectileData>(body);
      NetDataWriter writer = new NetDataWriter();

      ServerV2.projectiles.Add(new Projectile(payload.ownerId, new Vector2(payload.x, payload.y), payload.angle, payload.velocity, payload.damage));
      writer.Put(".PROJECTILE " + body);
      Program.server.broadcast(client, writer, DeliveryMethod.ReliableUnordered);
    }

    public static void UpdateMelee(NetPeer client, string body) //.MELEE
    {
      Console.WriteLine(body);
      MeleeHitData payload = JsonConvert.DeserializeObject<MeleeHitData>(body);

      foreach (var entry in ServerV2.players)
      {
        if (!entry.Value.active) continue;
        BasePlayer player = entry.Value;
        if (player.getHitbox().Contains(new Point((int)payload.x, (int)payload.y)))
        {
          player.addHealth(-payload.damage);
          ServerSenderV2.SendFullPlayerData(new FullPlayerData(player.name, player.active, player.hp, new PlayerData(player.id, player.position, player.orientation)));
        }
      }
    }
  }
}
