using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenPizza.World
{
  public class Map : BaseMap
  {
    Texture2D _tileset;
    Point _tilesetSizeInUnits;
    int _tilesetStartGid;

    enum Tileset {
      Meta = 0,
      City
    }

    public Map(string mapName) : base(mapName)
    {
      _tileset = Collection.LoadTileset(_map.Tilesets[(int)Tileset.City].Name.ToString());

      _tilesetStartGid = (int)(_map.Tilesets[0].Image.Width / _tileSize.X); //Compute meta size for offset
      _tilesetSizeInUnits = new Point(_tileset.Width / _tileSize.X, _tileset.Height / _tileSize.Y);
    }

    Rectangle GetGidRect(int gid)
    {
      int tileRelativeGid = gid - _tilesetStartGid - 1;
      return (new Rectangle((tileRelativeGid % _tilesetSizeInUnits.X) * _tileSize.X,
                            (tileRelativeGid / _tilesetSizeInUnits.X) * _tileSize.Y,
                             _tileSize.X, _tileSize.Y));
    }

    void DrawLayer(SpriteBatch spriteBatch, Layer layer)
    {
      for (var i = 0; i < _map.Layers[(int)layer].Tiles.Count; i++)
      {
        var tile = _map.Layers[(int)layer].Tiles[i];
        if (tile.Gid == 0) continue; //Only not Empty tile

        //Convert to world size
        float x = tile.X * _map.TileWidth;
        float y = tile.Y * _map.TileHeight;

        spriteBatch.Draw(_tileset, new Rectangle((int)x, (int)y, _tileSize.X, _tileSize.Y), GetGidRect(tile.Gid), Color.White);
      }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      for (Layer layer = Layer.Floor; (int)layer < _map.Layers.Count; layer++)
      {
        if (layer == Layer.Meta) continue; //Skip certain layers (meta)
        DrawLayer(spriteBatch, layer);
      }
    }
  }
}
