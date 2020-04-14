using FPServer;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using Server.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
  public static class ServerSenderV2
  {

    public static void SendFullPlayerData(FullPlayerData payload) //.PLAYER
    {
      NetDataWriter writer = new NetDataWriter();

      writer.Put(".FPLAYER " + JsonConvert.SerializeObject(payload));
      Program.server.broadcast(null, writer, DeliveryMethod.ReliableUnordered);
    }
  }
}
