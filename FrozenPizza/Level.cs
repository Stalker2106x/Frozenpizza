using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
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

    public enum Meta
    {
        Melee = 257,
        Pistol,
        Rifle
    }

    public class Level
    {
		//Tiles
        int _drawMargin;
		Texture2D _tileset;
        int _twidth, _theight;
        int _ttwidth, _ttheight;

		//Map
        TmxMap _map;
        List<Item> _entities;
        List<Projectile> _projectiles;

		public List<Item> Entities { get { return (_entities); } }
        public List<Projectile> Projectiles { get { return (_projectiles); } }

        public Level(string mapName)
        {
            _map = new TmxMap(mapName);
            _twidth = _map.Tilesets[0].TileWidth;
            _theight = _map.Tilesets[0].TileHeight;
			_entities = new List<Item>();
            _projectiles = new List<Projectile>();
            _drawMargin = 10;
        }

        public bool Load(ContentManager content)
        { 
            _tileset = content.Load<Texture2D>("maps/" + _map.Tilesets[0].Name.ToString());
            _ttwidth = _tileset.Width / _twidth;
            _ttheight = _tileset.Height / _theight;
            return (true);
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

		/*//Generation
        public void GenerateItems(Collection collection)
        {
            Random rnd = new Random();

            for (int i = 0; i < _map.Layers[(int)Layers.Meta].Tiles.Count; i++)
            {
                int gid = _map.Layers[(int)Layers.Meta].Tiles[i].Gid;

                if (gid == 0 || rnd.Next(0, 2) == 0) //Skip empty & 50% chance of spawn
                    continue;
                if (_entities[i] == null)
                    _entities[i] = new List<Item>();
                if (gid == (int)Meta.Melee)
                {
                    
                    _entities[i].Add((Melee)collection.MeleeList[rnd.Next(1, collection.MeleeList.Count)]);
                }
                else if (gid == (int)Meta.Pistol)
                {
                    _entities[i] = new List<Item>();
                    _entities[i].Add((Firearm)collection.FirearmList[rnd.Next(0, collection.FirearmList.Count)]);
                }
            }
        }*/

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

            if ((realpos.X < 0 || realpos.X > _map.Width)
                || (realpos.Y < 0 || realpos.Y > _map.Height))
                return (true);
            if (_map.Layers[(int)Layers.Wall].Tiles[(int)((_map.Width * realpos.Y) + realpos.X)].Gid != 0)
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
				if (_entities[i].Uid == uid)
					return (i);
			}
			return (-1);
        }

		public Item getEntityByPos(Vector2 pos)
		{
			Rectangle range = new Rectangle((pos - new Vector2(16, 16)).ToPoint(), (pos + new Vector2(16, 16)).ToPoint());

			for (int i = 0; i < _entities.Count; i++)
			{
				if (range.Contains(_entities[i].Pos))
					return (_entities[i]);
			}
			return (null);
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
                    for (int l = _map.Layers.Count - 1; l >= 0; l--)
                    {
                        if ((Layers)l == Layers.Meta || (Layers)l == Layers.Spawn || (Indoor(mainPlayer.Pos) && (Layers)l == Layers.Ceiling))
                            continue;
                        if (((yoffset + y) < 0 || (xoffset + x) < 0) //TopLeft
                            || ((yoffset + y) > (_map.Height - 1) || (xoffset + x) > (_map.Width - 1))) //TopRight
                            continue;
                        int gid = _map.Layers[l].Tiles[((yoffset + y) * _map.Width) + xoffset + x].Gid;

                        // Empty tile, do nothing
                        if (gid == 0)
                            continue;

                        int tileFrame = gid - 1;
                        int column = tileFrame % _ttwidth;
                        int row = (int)Math.Floor((double)tileFrame / (double)_ttwidth);

                        Rectangle tilesetRec = new Rectangle(_twidth * column, _theight * row, _twidth, _theight);

                        spriteBatch.Draw(_tileset, new Rectangle((int)(xoffset + x) * _twidth, (int)(yoffset + y) * _theight, _twidth, _theight), tilesetRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, (Layers)l == Layers.Ceiling ? 0.2f : 0.5f);
                        drawEntities(spriteBatch, xoffset, yoffset, x, y);
                        break;
                    }
                }

            }
        }

        public void Update()
        {
            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                _projectiles[i].Update();
            }
        }

        //base draw call, includes tilemap algorithm
        public void Draw(SpriteBatch spriteBatch, Camera cam, MainPlayer mainPlayer, Collection collection)
        {
            drawTiles(spriteBatch, cam, mainPlayer);
            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                _projectiles[i].Draw(spriteBatch);
            }
        }
    }
}
