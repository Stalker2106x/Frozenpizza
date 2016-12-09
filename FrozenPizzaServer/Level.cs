using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TiledSharp;

namespace FrozenPizzaServer
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
        Melee = 1,
        Pistol,
        Rifle
    }
    public class Level
    {
        //Tiles
        int _twidth, _theight;
        int _ttwidth, _ttheight;

        //Map
        TmxMap _map;
        List<Item>[] _entities;

        //Dynamic
        List<Projectile> _projectiles;

        //Thread
        Thread _thread;

        public TmxMap Map {  get { return (_map); } }
        public List<Item>[] Entities { get { return (_entities); } }
        public List<Projectile> Projectiles { get { return (_projectiles); } }

        public Level(String mapName)
        {
            _map = new TmxMap(mapName);
            _twidth = _map.Tilesets[0].TileWidth;
            _theight = _map.Tilesets[0].TileHeight;
            _entities = new List<Item>[_map.Width * _map.Height];
            _projectiles = new List<Projectile>();
            GenerateItems();
        }

        public Vector2 vmapToGrid(Vector2 pos)
        {
            return (new Vector2((int)pos.X / _twidth, (int)pos.Y / _theight));
        }

        public Vector2 vgridToMap(Vector2 pos)
        {
            return (new Vector2((int)pos.X * _twidth, (int)pos.Y * _theight));
        }

        public void startUpdateThread()
        {
            _thread = new Thread(Update);
            _thread.Start();
        }
        //Generation
        public void GenerateItems()
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
                    _entities[i].Add(new Item(rnd.Next(1, 4)));
                }
                else if (gid == (int)Meta.Pistol)
                {
                    _entities[i].Add(new Item(rnd.Next(1000, 1002)));
                }
            }
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

        public Vector2 getSpawnLocation()
		{
			Random rnd = new Random();
			int pos;
			TmxLayerTile spawn;

			pos = rnd.Next(0, _map.Layers[(int)Layers.Spawn].Tiles.Count);
			while (_map.Layers[(int)Layers.Spawn].Tiles[pos].Gid == 0)
				pos = rnd.Next(0, _map.Layers[(int)Layers.Spawn].Tiles.Count);
			spawn = _map.Layers[(int)Layers.Spawn].Tiles[pos];
			return (new Vector2(spawn.X, spawn.Y));
		}

        public void updateProjectiles()
        {
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                if (!Projectiles[i].Update())
                    Projectiles.RemoveAt(i);
            }
        }

        public void Update()
        {
            while (true)
            {
                updateProjectiles();
                Thread.Sleep(10);
            }
        }
    }
}
