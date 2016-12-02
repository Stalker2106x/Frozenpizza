using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace FrozenPizzaServer
{
    public class Level
    {
        //Tiles
        int _twidth, _theight;
        int _ttwidth, _ttheight;

        //Map
        TmxMap _map;
        List<Item>[] _entities;

        //Dynamic
        List<Player> _players;
        List<Projectile> _projectiles;

        public TmxMap Map {  get { return (_map); } }
        public List<Item>[] Entities { get { return (_entities); } }
        public List<Player> Players { get { return (_players); } }
        public List<Projectile> Projectiles { get { return (_projectiles); } }

        public Level(String mapName)
        {
            _map = new TmxMap(mapName);
            _twidth = _map.Tilesets[0].TileWidth;
            _theight = _map.Tilesets[0].TileHeight;
            _entities = new List<Item>[_map.Width * _map.Height];
        }
    }
}
