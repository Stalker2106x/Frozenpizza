using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using Server.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenPizza.Network
{
  static class ClientSenderV2
  {
    public static void ContinueSync()
    {
      NetDataWriter writer = new NetDataWriter();
      ClientV2.step += 1;
      switch (ClientV2.step)
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

    public static void SendPlayerData(PlayerData payload) //.PLAYER
    {
      NetDataWriter writer = new NetDataWriter();

      writer.Put(".PLAYER " + JsonConvert.SerializeObject(payload));
      Engine.networkClient.send(writer, DeliveryMethod.Unreliable);
    }

    public static void SendItemPickup(ItemData payload)
    {
      NetDataWriter writer = new NetDataWriter();

      writer.Put(".ITEM " + JsonConvert.SerializeObject(payload));
      Engine.networkClient.send(writer, DeliveryMethod.ReliableUnordered);
    }

    public static void SendProjectile(InteractionData payload) //.PROJECTILE
    {
      NetDataWriter writer = new NetDataWriter();

      writer.Put(".PROJECTILE " + JsonConvert.SerializeObject(payload));
      Engine.networkClient.send(writer, DeliveryMethod.ReliableUnordered);
    }

  }
}
