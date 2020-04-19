using FrozenPizza.Entities;
using FrozenPizza.Utils;
using FrozenPizza.World;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Server.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        { ".WELCOME", StartConnection },
        { "!GAMEDATA", ReceiveGamedata },
        { "!ENTITIES", ReceiveEntities },
        { ".HANDSHAKE", ReceiveHandshake },
        { ".NEWPLAYER", AddNewPlayer },
        { ".RMPLAYER", RemovePlayer },
        { ".PROJECTILE", AddProjectile },
        { ".FPLAYER", UpdateFullPlayer },
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

    public static void StartConnection(string body) //.WELCOME
    {
      var payloadDef = new { version = "" };
      var payload = JsonConvert.DeserializeAnonymousType(body, payloadDef);
      if (payload.version != Assembly.GetExecutingAssembly().GetName().Version.ToString()) //Version mismatch
      {
        Menu.OpenModal("The server is running a different version (" + payload.version + ")", "Error");
        Engine.networkClient.disconnect();
      }
      ClientSenderV2.ContinueSync();
    }

    public static void ReceiveGamedata(string body) //!GAMEDATA
    {
      GameData payload = JsonConvert.DeserializeObject<GameData>(body);
      GameMain.map = new Map(payload.mapName);
      GameMain.mainPlayer = new MainPlayer(payload.clientId, "Bernie", new Vector2(payload.spawnX, payload.spawnY));
      ClientSenderV2.ContinueSync();
    }

    public static void ReceiveEntities(string body) //!ENTITIES
    {
      EntitiesData payload = JsonConvert.DeserializeObject<EntitiesData>(body);
      payload.players.ForEach((it) =>
      {
        GameMain.players.Add(new Player(it.data.id, it.name, it.data.position, it.hp));
      });
      payload.items.ForEach((it) =>
      {
        var item = EntityStore.GetItemWithId(it.id);
        item.uid = it.uid;
        item.position = it.position;
        GameMain.map.items.Add(item);
      });
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
      FullPlayerData payload = JsonConvert.DeserializeObject<FullPlayerData>(body);

      GameMain.players.Add(new Player(payload.data.id, payload.name, payload.data.position, payload.hp));
    }
    
    public static void AddProjectile(string body) //.PROJECTILE"
    {
      ProjectileData payload = JsonConvert.DeserializeObject<ProjectileData>(body);
      GameMain.projectiles.Add(new Projectile(payload.ownerId, payload.position, payload.angle, payload.velocity, payload.damage));
    }

    public static void RemovePlayer(string body) //.RMPLAYER
    {
      int id;
      int.TryParse(body, out id);
      GameMain.players.Remove(GameMain.players.First((it) => { return (it.id == id); }));
    }

    public static void UpdateFullPlayer(string body) //.FPLAYER
    {
      FullPlayerData payload = JsonConvert.DeserializeObject<FullPlayerData>(body);

      Player player = null;
      if (GameMain.mainPlayer.id == payload.data.id) player = GameMain.mainPlayer;
      else player = GameMain.players.First((it) => { return (it.id == payload.data.id); });
      if (!player.active && payload.active) //respawn
      {
        player.Reset();
        if (player.id == payload.data.id) GameMain.hud.toggleDeathPanel(); //Close death panel
      }
      if (payload.hp != player.hp) player.addHealth(payload.hp - player.hp);
      if (!payload.active && player.active) player.die();
      player.position = payload.data.position;
      player.orientation = payload.data.orientation;
    }

    public static void UpdatePlayer(string body) //.PLAYER
    {
      PlayerData payload = JsonConvert.DeserializeObject<PlayerData>(body);

      Player player = GameMain.players.First((it) => { return (it.id == payload.id); });
      player.position = payload.position;
      player.orientation = payload.orientation;
    }

    public static void UpdateItem(string body) //.ITEM
    {
      ItemData payload = JsonConvert.DeserializeObject<ItemData>(body);

      Item item = GameMain.map.items.Find((it) => { return (it.uid == payload.uid); });
      item.position = payload.position;
    }
  }
}
