﻿using System;
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
        Melee = 257,
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
            GenerateItems();
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
                    _entities[i] = new List<Item>();
                    _entities[i].Add(new Item(rnd.Next(1000, 1002)));
                }
            }
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
            for (int i = 0; i < Projectiles.Count; i++)
            {
                Projectiles[i].Update();
                //Server.broadcast("!");
            }
        }

        public void Update()
        {
            while (true)
            {
                updateProjectiles();
                Thread.Sleep(50);
            }
        }
    }
}
