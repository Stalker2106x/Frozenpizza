using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TiledSharp;

namespace FrozenPizza.World
{
  public class BaseMap
  {
    protected enum Layer
    {
      Floor,
      Wall,
      Ceiling,
      Meta
    }

    public string name;
    
    protected TmxMap _map;

    protected Point _tileSize;

    public BaseMap(string mapName)
    {
      name = mapName;
      _map = new TmxMap("Data/maps/"+ mapName + ".tmx");

      _tileSize = new Point(_map.Tilesets[0].TileWidth, _map.Tilesets[0].TileHeight);
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

  }

}
