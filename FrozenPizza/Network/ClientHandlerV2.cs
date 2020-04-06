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
  public enum ConnectionStep
  {
    Begin,
    SyncGamedata,
    SyncEntities,
    Handshake,
    Synced
  }
  public class ClientHandlerV2
  {
    public static ConnectionStep step;
    public static Dictionary<string, Action<string>> commands;

    public static void Reset()
    {
      step = ConnectionStep.Begin;
    }

    public static void Initialize()
    {
      commands = new Dictionary<string, Action<string>>()
      {
        { ".WELCOME", (s) => { ContinueSync(); } },
        { "!GAMEDATA", ReceiveGamedata },
        { "!ENTITIES", ReceiveEntities },
        { ".HANDSHAKE", ReceiveHandshake },
        { ".NEWPLAYER", AddNewPlayer },
        { ".RMPLAYER", RemovePlayer },
        { ".PLAYER", UpdatePlayer }
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

    public static void ContinueSync()
    {
      NetDataWriter writer = new NetDataWriter();
      step += 1;
      switch (step)
      {
        case ConnectionStep.SyncGamedata:
          writer.Put("?GAMEDATA"); //Ask for gamedata
          Engine.networkClient.send(writer, DeliveryMethod.ReliableOrdered);
          break;
        case ConnectionStep.SyncEntities:
          writer.Put("?ENTITIES"); //Ask for entities
          Engine.networkClient.send(writer, DeliveryMethod.ReliableOrdered);
          break;
        case ConnectionStep.Handshake:
          writer.Put(".HANDSHAKE"); //Handshake
          Engine.networkClient.send(writer, DeliveryMethod.ReliableOrdered);
          break;
      }
    }

    public static void ReceiveGamedata(string body)
    {
      GameData payload = JsonConvert.DeserializeObject<GameData>(body);
      GameMain.map = new Map(payload.mapName);
      GameMain.mainPlayer = new MainPlayer(payload.clientId, "Bernie");
      ContinueSync();
    }

    public static void ReceiveEntities(string body)
    {
      GameMain.players = new List<Player>();
      EntitiesData payload = JsonConvert.DeserializeObject<EntitiesData>(body);
      payload.players.ForEach((it) =>
      {
        GameMain.players.Add(new Player(it.data.id, it.name, new Vector2(it.data.x, it.data.y), it.hp));
      });
      Console.WriteLine("Entities Received");
      ContinueSync();
    }
    public static void ReceiveHandshake(string body)
    {
      Console.WriteLine("Handshake Received");
      Engine.setState(GameState.Playing);
      step = ConnectionStep.Synced; //Ready
    }

    public static void AddNewPlayer(string body)
    {
      NewPlayerData payload = JsonConvert.DeserializeObject<NewPlayerData>(body);

      GameMain.players.Add(new Player(payload.data.id, payload.name, new Vector2(payload.data.x, payload.data.y), payload.hp));
    }
    public static void RemovePlayer(string body)
    {
      int id;
      int.TryParse(body, out id);
      GameMain.players.Remove(GameMain.players.First((it) => { return (it.id == id); }));
    }

    public static void UpdatePlayer(string body)
    {
      PlayerData payload = JsonConvert.DeserializeObject<PlayerData>(body);

      Player player = GameMain.players.First((it) => { return (it.id == payload.id); });
      player.position = new Vector2(payload.x, payload.y);
      player.orientation = payload.orientation;
    }
  }
}
