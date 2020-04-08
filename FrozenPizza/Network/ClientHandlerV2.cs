using FrozenPizza.Entities;
using FrozenPizza.World;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Server.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LiteNetLib.EventBasedNetListener;

namespace FrozenPizza.Network
{
  public static class ClientHandlerV2
  {
    public static Dictionary<string, Action<string>> commands;
    public static void Initialize()
    {
      commands = new Dictionary<string, Action<string>>()
      {
        { ".WELCOME", (s) => { ClientSenderV2.ContinueSync(); } },
        { "!GAMEDATA", ReceiveGamedata },
        { "!ENTITIES", ReceiveEntities },
        { ".HANDSHAKE", ReceiveHandshake },
        { ".NEWPLAYER", AddNewPlayer },
        { ".RMPLAYER", RemovePlayer },
        { ".PROJECTILE", AddProjectile },
        { ".PLAYER", UpdatePlayer },
        { ".ITEM", UpdateItem }
      };
    }

    public static void Parse(NetPeer peer, NetPacketReader reader, DeliveryMethod method)
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
        commands[command](body);
      } catch (Exception e) {
        Console.WriteLine("Unhandled command:" + e.Message);
      }
    }

    public static void ReceiveGamedata(string body) //!GAMEDATA
    {
      GameData payload = JsonConvert.DeserializeObject<GameData>(body);
      GameMain.map = new Map(payload.mapName);
      GameMain.mainPlayer = new MainPlayer(payload.clientId, "Bernie");
      ClientSenderV2.ContinueSync();
    }

    public static void ReceiveEntities(string body) //!ENTITIES
    {
      GameMain.players = new List<Player>();
      EntitiesData payload = JsonConvert.DeserializeObject<EntitiesData>(body);
      payload.players.ForEach((it) =>
      {
        GameMain.players.Add(new Player(it.data.id, it.name, new Vector2(it.data.x, it.data.y), it.hp));
      });
      payload.items.ForEach((it) =>
      {
        var item = Collection.GetItemWithId(it.id);
        item.uid = it.uid;
        item.position = it.GetPosition();
        GameMain.map.items.Add(item);
      });
      Console.WriteLine(body);
      ClientSenderV2.ContinueSync();
    }
    public static void ReceiveHandshake(string body) //.HANDSHAKE
    {
      Console.WriteLine("Handshake");
      Engine.setState(GameState.Playing);
      ClientV2.step = ConnectionStep.Synced; //Ready
    }

    public static void AddNewPlayer(string body) //.ADDPLAYER
    {
      NewPlayerData payload = JsonConvert.DeserializeObject<NewPlayerData>(body);

      GameMain.players.Add(new Player(payload.data.id, payload.name, new Vector2(payload.data.x, payload.data.y), payload.hp));
    }
    
    public static void AddProjectile(string body) //.PROJECTILE"
    {
      InteractionData payload = JsonConvert.DeserializeObject<InteractionData>(body);
      var player = GameMain.players.First((it) => { return (it.id == payload.playerId); });
      GameMain.projectiles.Add(new Projectile(player.position, player.orientation, 200f, (int)payload.value));
    }

    public static void RemovePlayer(string body) //.RMPLAYER
    {
      int id;
      int.TryParse(body, out id);
      GameMain.players.Remove(GameMain.players.First((it) => { return (it.id == id); }));
    }

    public static void UpdatePlayer(string body) //.PLAYER
    {
      PlayerData payload = JsonConvert.DeserializeObject<PlayerData>(body);

      Player player = GameMain.players.First((it) => { return (it.id == payload.id); });
      player.position = new Vector2(payload.x, payload.y);
      player.orientation = payload.orientation;
    }

    public static void UpdateItem(string body) //.ITEM
    {
      ItemData payload = JsonConvert.DeserializeObject<ItemData>(body);

      BaseItem item = GameMain.map.items.Find((it) => { return (it.uid == payload.uid); });
      item.position = payload.GetPosition();
    }
  }
}
