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
    Point _tilesetSize;

    public Map(string mapName) : base(mapName)
    {
      _tileset = Collection.LoadTileset(_map.Tilesets[0].Name.ToString());
      _tilesetSize = new Point(_tileset.Width / _tileSize.X, _tileset.Height / _tileSize.Y);
    }

    void DrawLayer(SpriteBatch spriteBatch, Layer layer)
    {
      for (var i = 0; i < _map.Layers[0].Tiles.Count; i++)
      {
        int gid = _map.Layers[(int)layer].Tiles[i].Gid;

        if (gid != 0) //Only not Empty tile
        {
          int tileFrame = gid - 1;
          int column = tileFrame % _tilesetSize.X;
          int row = (int)Math.Floor((double)tileFrame / (double)_tilesetSize.X);

          float x = (i % _map.Width) * _map.TileWidth;
          float y = (float)Math.Floor(i / (double)_map.Width) * _map.TileHeight;

          Rectangle tilesetRec = new Rectangle(_tileSize.X * column, _tileSize.Y * row, _tileSize.X, _tileSize.Y);

          spriteBatch.Draw(_tileset, new Rectangle((int)x, (int)y, _tileSize.X, _tileSize.Y), tilesetRec, Color.White);
        }
      }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      for (Layer layer = Layer.Floor; (int)layer < _map.Layers.Count; layer++)
      {
        DrawLayer(spriteBatch, layer);
      }
    }
  }
}
