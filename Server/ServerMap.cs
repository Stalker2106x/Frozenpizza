using FrozenPizza.Entities;
using FrozenPizza.World;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
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
    public List<Rectangle> spawnAreas;

    public ServerMap(string mapName) : base(mapName)
    {
      items = new List<BaseItem>();
      LoadSpawnAreas();
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
            var melee = new MeleeWeapon();
            melee.Copy(BaseCollection.MeleeList[_randomGenerator.Next(1, BaseCollection.MeleeList.Count)]);
            melee.Init();
            melee.position = new Point(tile.X, tile.Y);
            melee.uid = uid;
            items.Add(melee);
            uid++;
            break;
          case MetaTile.Pistol:
            var pistol = new FireWeapon();
            pistol.Copy(BaseCollection.FirearmList[_randomGenerator.Next(0, BaseCollection.FirearmList.Count)]);
            pistol.Init();
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
    public Vector2 GetRandomSpawnArea()
    {
      var area = spawnAreas[_randomGenerator.Next(0, spawnAreas.Count)];
      return (new Vector2(_randomGenerator.Next(area.X, area.X + area.Width), _randomGenerator.Next(area.Y, area.Y + area.Height)));
    }

    public void LoadSpawnAreas()
    {
      spawnAreas = new List<Rectangle>();
      foreach (var area in _map.ObjectGroups["Spawn"].Objects)
      {
        spawnAreas.Add(new Rectangle((int)area.X, (int)area.Y, (int)area.Width, (int)area.Height));
      }
    }
  }
}
