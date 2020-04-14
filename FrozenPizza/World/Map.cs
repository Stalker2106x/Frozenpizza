using FrozenPizza.Entities;
using FrozenPizza.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TiledSharp;

namespace FrozenPizza.World
{
  public class Map : BaseMap
  {
    Texture2D _tileset;
    int _tilesetStartGid;

    public List<BaseItem> items;

    enum Tileset {
      Meta = 0,
      City
    }

    public Map(string mapName) : base(mapName)
    {
      items = new List<BaseItem>();
      _tileset = Collection.LoadTileset(_map.Tilesets[(int)Tileset.City].Name.ToString());

      _tilesetStartGid = (int)(_map.Tilesets[0].Image.Width / tileSize.X); //Compute meta size for offset
    }

    Rectangle GetGidRect(int gid)
    {
      int tileRelativeGid = gid - _tilesetStartGid - 1;
      return (new Rectangle((tileRelativeGid % _tilesetSizeInUnits.X) * tileSize.X,
                            (tileRelativeGid / _tilesetSizeInUnits.X) * tileSize.Y,
                             tileSize.X, tileSize.Y));
    }

    void DrawTile(SpriteBatch spriteBatch, TmxLayerTile tile, bool skipVisible)
    {
      if (tile.Gid == 0) return; //Only not Empty tile

      //Convert to world size
      float x = tile.X * _map.TileWidth;
      float y = tile.Y * _map.TileHeight;
      if (skipVisible && isTileInView(new Vector2(x, y))) return;

      spriteBatch.Draw(_tileset, Tools.GetDrawRect((int)x, (int)y, tileSize), GetGidRect(tile.Gid), Color.White);
    }

    void DrawLayer(SpriteBatch spriteBatch, Layer layer, bool skipVisible = false)
    {
      Rectangle viewPort = GameMain.cam.getGridViewport();
      Rectangle drawBounds = new Rectangle(viewPort.X - (viewPort.Width / 2), viewPort.Y - (viewPort.Height / 2), (int)(viewPort.X + (viewPort.Width * 1.5)), (int)(viewPort.Y + (viewPort.Height * 1.5)));
      for (int y = drawBounds.Y; y < drawBounds.Height; y++)
      {
        for (int x = drawBounds.X; x < drawBounds.Width; x++)
        {
          DrawTile(spriteBatch, _map.Layers[(int)layer].Tiles[x + (y * _map.Width)], skipVisible);
        }
      }
    }

    bool isTileInView(Vector2 tilePos)
    {
      AccuracyAngle viewAngle = new AccuracyAngle();
      viewAngle.EnforceConsistency();
      Vector2 vecToPoint = tilePos - (GameMain.mainPlayer.position + GameMain.mainPlayer.getDirectionVector(Direction.Backward, 50));
      float tileAngle = (3 * MathHelper.PiOver2) - (float)Math.Atan2(vecToPoint.Y, vecToPoint.X);

      if (tileAngle >= MathHelper.TwoPi) tileAngle -= MathHelper.TwoPi;
      else if (tileAngle <= 0) tileAngle += MathHelper.TwoPi;

      return (viewAngle.Contains(tileAngle));
    }

    List<TmxLayerTile> GetTilesInView(Layer layer)
    {
      List<TmxLayerTile> inViewTiles = new List<TmxLayerTile>();
      AccuracyAngle viewAngle = new AccuracyAngle();
      Vector2 viewOrigin = (GameMain.mainPlayer.position + GameMain.mainPlayer.getDirectionVector(Direction.Backward, 50));
      viewAngle.EnforceConsistency();
      foreach (var tile in _map.Layers[(int)layer].Tiles)
      {
        Vector2 vecToPoint = new Vector2(tile.X - viewOrigin.X, tile.Y - viewOrigin.Y);
        float tileAngle = (3 * MathHelper.PiOver2) - (float)Math.Atan2(vecToPoint.Y, vecToPoint.X);

        if (tileAngle >= MathHelper.TwoPi) tileAngle -= MathHelper.TwoPi;
        else if (tileAngle <= 0) tileAngle += MathHelper.TwoPi;
        if (viewAngle.Contains(tileAngle)) inViewTiles.Add(tile);
      }
      return (inViewTiles);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      for (Layer layer = Layer.Floor; (int)layer < _map.Layers.Count; layer++)
      {
        if (layer == Layer.Meta || layer == Layer.Ceiling) continue; //Skip certain layers
        else DrawLayer(spriteBatch, layer);
      }
      foreach (var item in items)
      {
        item.Draw(spriteBatch, null);
      }
      DrawLayer(spriteBatch, Layer.Ceiling, true);
    }
  }
}
