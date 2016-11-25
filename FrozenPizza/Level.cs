using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using TiledSharp;

namespace FrozenPizza
{
    public enum Layers
    {
        Floor,
        Wall,
        Ceiling,
        Meta,
        Spawn
    }

	public enum Orientation
	{
		North,
		East,
		South,
		West
	}

    public class Level
    {
        int _twidth, _theight;
        int _ttwidth, _ttheight;
        TmxMap _map;
        Texture2D _tileset;

        public Level(string mapName)
        {
            _map = new TmxMap(mapName);
            _twidth = _map.Tilesets[0].TileWidth;
            _theight = _map.Tilesets[0].TileHeight;
        }

        public bool Load(ContentManager content)
        { 
            _tileset = content.Load<Texture2D>("maps/" + _map.Tilesets[0].Name.ToString());
            _ttwidth = _tileset.Width / _twidth;
            _ttheight = _tileset.Height / _theight;
            return (true);
        }

        public Rectangle mapToGrid(Vector2 pos)
        {
            return (new Rectangle((int)pos.X / _twidth, (int)pos.Y / _theight, ((int)pos.X / _twidth), (int)pos.Y / _twidth));
        }

        public void GenerateItems()
        {
			for (int i = 0; i < _map.Layers[(int)Layers.Meta].Tiles.Count; i++)
            {
				if (_map.Layers[(int)Layers.Meta].Tiles[i].Gid == 0)
					continue;				
            }
        }

        public Vector2 getSpawnPoint()
        {
            Random rnd = new Random();
			int pos;
			TmxLayerTile spawn;

			pos = rnd.Next(0, _map.Layers[(int)Layers.Spawn].Tiles.Count);
			while (_map.Layers[(int)Layers.Spawn].Tiles[pos].Gid == 0)
				pos = rnd.Next(0, _map.Layers[(int)Layers.Spawn].Tiles.Count);
			spawn = _map.Layers[(int)Layers.Spawn].Tiles[pos];
			return (new Vector2(spawn.X * _twidth, spawn.Y * _theight));
        }

        public bool Collide(Vector2 pos)
        {
            if ((pos.X < 0 || pos.X > (_map.Width * _twidth))
                || (pos.Y < 0 || pos.Y > (_map.Height * _theight)))
                return (true);
            Rectangle posCell = mapToGrid(pos);

            if ((_map.Layers[(int)Layers.Wall].Tiles[(int)(posCell.X + (posCell.Y * _map.Width))].Gid != 0)
                || (_map.Layers[(int)Layers.Wall].Tiles[(int)(posCell.Width + (posCell.Height * _map.Width))].Gid != 0))
                return (true);
            return (false);
        }

        public bool Indoor(Vector2 pos)
        {
            Rectangle posCell = mapToGrid(pos);

            if ((_map.Layers[(int)Layers.Ceiling].Tiles[(int)(posCell.X + (posCell.Y * _map.Width))].Gid != 0)
                || (_map.Layers[(int)Layers.Ceiling].Tiles[(int)(posCell.Width + (posCell.Height * _map.Width))].Gid != 0))
                return (true);
            return (false);
        }

        public void Draw(SpriteBatch spriteBatch, Camera cam, Player mainPlayer)
        {
            Rectangle viewport = cam.getViewport();
            int xoffset = viewport.X / _twidth;
            int yoffset = viewport.Y / _theight;

            for (int y = 0; y <= viewport.Height / _theight; y++)
            {
                for (int x = 0; x <= viewport.Width / _twidth; x++)
                {
                    for (int l = _map.Layers.Count - 1; l >= 0; l--)
                    {
						if ((Layers)l == Layers.Meta || (Layers)l == Layers.Spawn || (Indoor(mainPlayer.Pos) && (Layers)l == Layers.Ceiling))
                            continue;
                        if ((yoffset + y) < 0 || (xoffset + x) < 0)
                            continue;
                        int gid = _map.Layers[l].Tiles[((yoffset + y) * _map.Width) + xoffset + x].Gid;

                        // Empty tile, do nothing
                        if (gid == 0)
                            continue;
                        else
                        {
                            int tileFrame = gid - 1;
                            int column = tileFrame % _ttwidth;
                            int row = (int)Math.Floor((double)tileFrame / (double)_ttwidth);

                            Rectangle tilesetRec = new Rectangle(_twidth * column, _theight * row, _twidth, _theight);

                            spriteBatch.Draw(_tileset, new Rectangle((int)(xoffset + x) * _twidth, (int)(yoffset + y) * _theight, _twidth, _theight), tilesetRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, l / 10);
                            break;
                        }
                    }
                }

            }
        }
    }
}
