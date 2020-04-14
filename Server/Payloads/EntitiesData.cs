using FrozenPizza;
using FrozenPizza.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Payloads
{
  public class EntitiesData
  {
    public List<FullPlayerData> players;
    public List<NewItemData> items;

    public EntitiesData() { } //default
    public EntitiesData(int clientId, List<BasePlayer> playerList, List<BaseItem> itemList)
    {
      players = new List<FullPlayerData>();
      playerList.ForEach((it) =>
      {
        if (clientId != -1 && clientId == it.id) return; //Skip owner if set, we dont want to propagate itself
        players.Add(new FullPlayerData(it.name, it.active, it.hp, new PlayerData(it.id, it.position, it.orientation)));
      });
      items = new List<NewItemData>();
      itemList.ForEach((it) =>
      {
        items.Add(new NewItemData(it.uid, it.id, it.position.GetValueOrDefault()));
      });
    }
  }
}
