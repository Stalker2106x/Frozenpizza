using FrozenPizza;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Payloads
{
  public class EntitiesData
  {
    public List<NewPlayerData> players;

    public EntitiesData(int clientId, List<BasePlayer> playerList)
    {
      players = new List<NewPlayerData>();
      if (playerList == null) return; //No players to send
      playerList.ForEach((it) =>
      {
        if (clientId != -1 && clientId == it.id) return; //Skip owner if set, we dont want to propagate itself
        players.Add(new NewPlayerData(it.name, it.hp, new PlayerData(it.id, it.position, it.orientation)));
      });
    }
  }
}
