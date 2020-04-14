#if GAME
  using Microsoft.Xna.Framework;
#else
  using System.Drawing;
  using System.Numerics;
#endif
using FrozenPizza;
using FrozenPizza.Entities;
using System.Collections.Generic;
using System.IO;
using TiledSharp;


namespace FrozenPizza.World
{
  public class BaseMap
  {
    protected enum Layer
    {
      Floor = 0,
      Wall,
      Ceiling,
      Meta
    }

    public string name;
    
    protected TmxMap _map;

    public Point tileSize;
    protected Point _tilesetSizeInUnits;

    public BaseMap(string mapName)
    {
      name = mapName;
      _map = new TmxMap("Data/maps/"+ mapName + ".tmx");

      tileSize = new Point(_map.Tilesets[1].TileWidth, _map.Tilesets[1].TileHeight);
      _tilesetSizeInUnits = new Point(_map.Tilesets[1].Image.Width.GetValueOrDefault() / tileSize.X, _map.Tilesets[1].Image.Width.GetValueOrDefault() / tileSize.Y);
    }

    public static List<string> getAvailableLevels()
    {
      List<string> levelList = new List<string>();
      string[] levels = Directory.GetFiles(@"Data/maps/", "*.tmx", SearchOption.TopDirectoryOnly);

      for (int i = 0; i < levels.Length; i++)
      {
        int index = levels[i].LastIndexOf('/') + 1;

        levelList.Add(levels[i].Substring(index, levels[i].Length - (index + 4)));
      }
      return (levelList);
    }

    public Point WorldToGrid(Vector2 coord)
    {
      return (new Point((int)((coord.X / tileSize.X) + 0.5f), (int)((coord.Y/ tileSize.Y) + 0.5f)));
    }

    public int GridToIndex(Point gridCoord)
    {
      return ((gridCoord.Y * _map.Width) + gridCoord.X);
    }

    public bool isValidPosition(Vector2 position)
    {
      Point gridPos = WorldToGrid(position);
      if ((gridPos.X < 0 || gridPos.X >= _map.Width - 1) || (gridPos.Y < 0 || gridPos.Y >= _map.Height - 1)
        || _map.Layers[(int)Layer.Wall].Tiles[GridToIndex(gridPos)].Gid != 0)
      {
        return (false);
      }
      return (true);
    }

    public bool isValidPosition(Rectangle rect)
    {
      return (isValidPosition(new Vector2(rect.X, rect.Y)) && isValidPosition(new Vector2(rect.X, rect.Y + rect.Width))
           && isValidPosition(new Vector2(rect.X + rect.Height, rect.Y)) && isValidPosition(new Vector2(rect.X + rect.Height, rect.Y + rect.Width)));
    }

  }

}
