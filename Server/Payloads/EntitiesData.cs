using FrozenPizza;
using FrozenPizza.Entities;
using System.Collections.Generic;

namespace Server.Payloads
{
  public class EntitiesData
  {
    public List<FullPlayerData> players;
    public List<NewItemData> items;

    public EntitiesData() { } //default
    public EntitiesData(int clientId, List<Player> playerList, List<Item> itemList)
    {
      players = new List<FullPlayerData>();
      playerList.ForEach((it) =>
      {
        if (clientId != -1 && clientId == it.uid) return; //Skip owner if set, we dont want to propagate itself
        players.Add(new FullPlayerData(it.name, it.active, it.hp, new PlayerData(it.uid, it.position, it.orientation)));
      });
      items = new List<NewItemData>();
      itemList.ForEach((it) =>
      {
        items.Add(new NewItemData(it.uid, it.id, it.position.GetValueOrDefault()));
      });
    }
  }
}
