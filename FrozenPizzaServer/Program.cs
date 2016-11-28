using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{
    public class Server
    {
        bool _running;
        Level _level;
        String _mapName;
        static void Main(string[] args)
        {
            Server server = new Server(args[0]);

            while (server.isRunning())
                server.Update();
        }

        public Server(String mapName)
        {
            _running = true;
            _mapName = "maps/world.tmx";
            if (mapName == null || mapName.Length == 0)
            Console.Write("Starting FrozenPizza Server...\n");
            Console.Write("Loading level " + _mapName);
            _level = new Level(mapName);
        }

        public bool isRunning()
        {
            return (_running);
        }

        public void updatePlayers()
        {

        }

        public void updateWorld()
        {

        }
        public void Update()
        {
            updatePlayers();
            updateWorld();
        }
    }
}
