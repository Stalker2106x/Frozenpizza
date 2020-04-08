using FrozenPizza.Entities;
using FrozenPizza.World;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FPServer
{
  public class ServerMap : BaseMap
  {
    protected enum MetaTile
    {
      Melee = 1,
      Pistol
    }

    private static Random _randomGenerator = new Random();

    public List<BaseItem> items;

    public ServerMap(string mapName) : base(mapName)
    {
      items = new List<BaseItem>();
      GenerateItems();
    }

    public void GenerateItems()
    {
      int uid = 0;
      foreach (var tile in _map.Layers[(int)Layer.Meta].Tiles)
      {
        switch ((MetaTile)tile.Gid)
        {
          case MetaTile.Melee:
            var melee = BaseCollection.MeleeList[_randomGenerator.Next(0, BaseCollection.MeleeList.Count)].Copy();
            melee.position = new Point(tile.X, tile.Y);
            melee.uid = uid;
            items.Add(melee);
            uid++;
            break;
          case MetaTile.Pistol:
            var pistol = BaseCollection.FirearmList[_randomGenerator.Next(0, BaseCollection.FirearmList.Count)].Copy();
            pistol.position = new Point(tile.X, tile.Y);
            pistol.uid = uid;
            items.Add(pistol);
            uid++;
            break;
          default:
            break;
        }
      }
    }
  }
}
