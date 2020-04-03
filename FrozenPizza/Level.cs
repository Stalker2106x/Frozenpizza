using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TiledSharp;

namespace FrozenPizza
{
    public enum Layers
    {
        Floor,
        HWall,
        Wall,
        Ceiling,
        Meta,
        Spawn
    }

    public enum Meta
    {
        Melee = 257,
        Pistol,
        Rifle
    }

    public enum Shadows
    {
        Full,
        BottomLeft,
        TopRight
    }

    public class Level
    {
		//Tiles
        int _drawMargin;
		Texture2D _tileset, _shadows;
        int _metaOffset;
        int _twidth, _theight;
        int _ttwidth, _ttheight;

        //Map
        TmxMap _map;
        List<Item> _entities;
        List<Projectile> _projectiles;

        public static String MapName { get; set; }
        public TmxMap Map { get { return (_map);  } }
		public List<Item> Entities { get { return (_entities); } }
        public List<Projectile> Projectiles { get { return (_projectiles); } }

        public Level()
        {
            _map = new TmxMap(MapName);
            _twidth = _map.Tilesets[0].TileWidth;
            _theight = _map.Tilesets[0].TileHeight;
			_entities = new List<Item>();
            _projectiles = new List<Projectile>();
            _metaOffset = (int)(_map.Tilesets[0].Image.Width / _twidth);
            _drawMargin = 10;
        }

        public bool Load(ContentManager content)
        { 
            _tileset = content.Load<Texture2D>("maps/" + _map.Tilesets[1].Name.ToString());
            _shadows = content.Load<Texture2D>("gfx/shadowmap");
            _ttwidth = _tileset.Width / _twidth;
            _ttheight = _tileset.Height / _theight;
            return (true);
        }

        public static List<String> getAvailableLevels()
        {
            List<String> levelList = new List<String>();
            String[] levels = Directory.GetFiles(@"Data/maps/", "*.tmx", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < levels.Length; i++)
            {
                int index = levels[i].LastIndexOf('/') + 1;

                levelList.Add(levels[i].Substring(index, levels[i].Length - (index + 4)));
            }
            return (levelList);
        }

        //Convesion of Coordinates
        public Rectangle mapToGrid(Vector2 pos)
        {
            return (new Rectangle((int)pos.X / _twidth, (int)pos.Y / _theight, ((int)pos.X / _twidth), (int)pos.Y / _twidth));
        }

        public Vector2 vmapToGrid(Vector2 pos)
        {
            return (new Vector2((int)pos.X / _twidth, (int)pos.Y / _theight));
        }

		public Vector2 vgridToMap(Vector2 pos)
		{
			return (new Vector2((int)pos.X * _twidth, (int)pos.Y * _theight));
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
			return (vgridToMap(new Vector2(spawn.X, spawn.Y)));
        }

		//Bool checks
        public bool Collide(Vector2 pos)
        {
            Vector2 realpos = vmapToGrid(pos);

            if ((realpos.X < 0 || realpos.X >= _map.Width - 1)
                || (realpos.Y < 0 || realpos.Y >= _map.Height - 1))
                return (true);
            if (_map.Layers[(int)Layers.Wall].Tiles[(int)((_map.Width * realpos.Y) + realpos.X)].Gid != 0)
                return (true);
            return (false);
        }

        public bool RCollide(Rectangle rect)
        {
            if ((Collide(rect.Location.ToVector2()) || Collide(rect.Location.ToVector2() + rect.Size.ToVector2()))
                || Collide(rect.Location.ToVector2() + new Vector2(rect.Width, 0)) || Collide(rect.Location.ToVector2() + new Vector2(0, rect.Height)))
                return (true);
            return (false);
        }

        public bool Indoor(Vector2 pos)
        {
            Vector2 realpos = vmapToGrid(pos);

            if (_map.Layers[(int)Layers.Ceiling].Tiles[(int)((_map.Width * realpos.Y) + realpos.X)].Gid != 0)
                return (true);
            return (false);
        }

		//Items management
        public int getEntityIndex(Int64 uid)
        {
			for (int i = 0; i < _entities.Count; i++)
			{
				if (_entities[i].uid == uid)
					return (i);
			}
			return (-1);
        }

		public Item getEntityByPos(Vector2 pos)
		{
			Rectangle range = new Rectangle((pos - new Vector2(16, 16)).ToPoint(), new Vector2(32, 32).ToPoint());

			for (int i = 0; i < _entities.Count; i++)
			{
				if (range.Contains(_entities[i].pos))
					return (_entities[i]);
			}
			return (null);
		}

        public void drawShadow(SpriteBatch spriteBatch, int layer, int xoffset, int yoffset, int x, int y)
        {
            Shadows shadow = Shadows.Full;

            if (yoffset <= 1 || xoffset <= 1
                || _map.Layers[(int)Layers.Ceiling].Tiles[((yoffset + y) * _map.Width) + (xoffset + x + 1)].Gid != 0
                || _map.Layers[(int)Layers.Wall].Tiles[((yoffset + y) * _map.Width) + (xoffset + x)].Gid == 0) //Under ceiling! no shadow
                return;
            if (_map.Layers[(int)Layers.Wall].Tiles[((yoffset + y + 1) * _map.Width) + (xoffset + x)].Gid == 0 //Down shadow
                && _map.Layers[layer].Tiles[((yoffset + y + 1) * _map.Width) + (xoffset + x - 1)].Gid == 0) //Check for duplicate
            {
                if (_map.Layers[(int)Layers.Wall].Tiles[((yoffset + y) * _map.Width) + (xoffset + x - 1)].Gid == 0) //Left tile
                    shadow = Shadows.TopRight;
                spriteBatch.Draw(_shadows, new Rectangle((xoffset + x) * _twidth, (yoffset + y + 1) * _theight, _twidth, _theight), new Rectangle((int)shadow * _twidth, 0, _twidth, _theight), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.25f);
            }
            shadow = Shadows.Full;
            if (_map.Layers[layer].Tiles[((yoffset + y) * _map.Width) + (xoffset + x + 1)].Gid == 0) //Right shadow
            {
                if (_map.Layers[(int)Layers.Wall].Tiles[((yoffset + y - 1) * _map.Width) + (xoffset + x)].Gid == 0) //Top tile
                    shadow = Shadows.BottomLeft;
                else if (_map.Layers[layer].Tiles[(((yoffset + y) + 1) * _map.Width) + (xoffset + x)].Gid == 0) //Bottom tile
                    shadow = Shadows.Full;
                spriteBatch.Draw(_shadows, new Rectangle((xoffset + x + 1) * _twidth, (yoffset + y) * _theight, _twidth, _theight), new Rectangle((int)shadow * _twidth, 0, _twidth, _theight), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.25f);
            }
            if (_map.Layers[(int)Layers.Wall].Tiles[((yoffset + y + 1) * _map.Width) + (xoffset + x + 1)].Gid == 0) //Bottom Right shadow
            {
                if (_map.Layers[(int)Layers.Wall].Tiles[((yoffset + y) * _map.Width) + (xoffset + x)].Gid != 0
                    && _map.Layers[(int)Layers.Wall].Tiles[((yoffset + y + 1) * _map.Width) + (xoffset + x)].Gid == 0
                    && _map.Layers[(int)Layers.Wall].Tiles[((yoffset + y) * _map.Width) + (xoffset + x + 1)].Gid == 0) //Top tile
                {
                    shadow = Shadows.Full;
                    spriteBatch.Draw(_shadows, new Rectangle((xoffset + x + 1) * _twidth, (yoffset + y + 1) * _theight, _twidth, _theight), new Rectangle((int)shadow * _twidth, 0, _twidth, _theight), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.25f);
                }
            }
        }

        public void drawTiles(SpriteBatch spriteBatch, Camera cam, MainPlayer mainPlayer)
        {
            Rectangle viewport = cam.getViewport();
            int xoffset = (viewport.X / _twidth);
            int xend = ((viewport.X + viewport.Width) / _twidth);
            int yoffset = (viewport.Y / _theight);
            int yend = ((viewport.Y + viewport.Height) / _theight);

            if (xoffset > _drawMargin)
                xoffset -= _drawMargin;
            if (yoffset > _drawMargin)
                yoffset -= _drawMargin;
            if (xend + _drawMargin < _map.Width)
                xend += _drawMargin;
            if (yend + _drawMargin < _map.Height)
                yend += _drawMargin;
            for (int y = 0; y <= yend; y++)
            {
                for (int x = 0; x <= xend; x++)
                {
                    bool shadow = false;

                    if (((yoffset + y) < 0 || (xoffset + x) < 0) //TopLeft
                        || ((yoffset + y) >= (_map.Height - 1) || (xoffset + x) >= (_map.Width - 1))) //BottomRight
                        continue;
                    if (Indoor(mainPlayer.pos) && _map.Layers[(int)Layers.Ceiling].Tiles[((yoffset + y) * _map.Width) + xoffset + x].Gid == 0)
                        continue;
                    for (int l = _map.Layers.Count - 1; l >= 0; l--)
                    {
                        if ((Layers)l == Layers.Meta || (Layers)l == Layers.Spawn || (Indoor(mainPlayer.pos) && (Layers)l == Layers.Ceiling))
                            continue;
                        int gid = _map.Layers[l].Tiles[((yoffset + y) * _map.Width) + xoffset + x].Gid;

                        // Empty tile, do nothing
                        if (gid == 0)
                            continue;

                        int tileFrame = gid - _metaOffset - 1;
                        int column = tileFrame % _ttwidth;
                        int row = (int)Math.Floor((double)tileFrame / (double)_ttwidth);

                        Rectangle tilesetRec = new Rectangle(_twidth * column, _theight * row, _twidth, _theight);

                        spriteBatch.Draw(_tileset, new Rectangle((int)(xoffset + x) * _twidth, (int)(yoffset + y) * _theight, _twidth, _theight), tilesetRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, (Layers)l == Layers.Ceiling ? 0.2f : 0.5f);
                        if (!shadow)
                        {
                            drawShadow(spriteBatch, l, xoffset, yoffset, x, y);
                            shadow = true;
                        }
                        if (l != (int)Layers.Ceiling)
                            break;
                    }
                }

            }
        }

        public void Update()
        {
            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                if (!_projectiles[i].Update())
                    _projectiles.RemoveAt(i);
            }
        }

        //base draw call, includes tilemap algorithm
        public void Draw(SpriteBatch spriteBatch, Camera cam, MainPlayer mainPlayer, Collection collection)
        {
            drawTiles(spriteBatch, cam, mainPlayer);
            for (int i = _entities.Count - 1; i >= 0; i--)
            {
                if (_entities[i].pos != - Vector2.One && (!Indoor(mainPlayer.pos) || (Indoor(mainPlayer.pos) && Indoor(_entities[i].pos))))
                    spriteBatch.Draw(Engine.collection.Tilesets[(int)_entities[i].type], _entities[i].pos, _entities[i].skinRect, Color.White, 0, _entities[i].origin, 1f, SpriteEffects.None, 0.3f);
            }
            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                _projectiles[i].Draw(spriteBatch);
            }
        }
    }
}
