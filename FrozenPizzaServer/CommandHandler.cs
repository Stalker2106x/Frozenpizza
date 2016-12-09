using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{
    public class CommandHandler
    {
        Dictionary<String, Func<String[], bool>> _commands;
        NetCli _client;

        public CommandHandler(NetCli client)
        {
            _client = client;
            _commands = new Dictionary<String, Func<String[], bool>>();
            _commands.Add("!VERSION", checkVersion);
            _commands.Add("!WHOIS", whoisClient);
            _commands.Add("?MOVE", movePlayer);
            _commands.Add("!AIM", aimPlayer);
            _commands.Add("!FIRE", fireWeapon);
            _commands.Add("!+ITEM", spawnItem);
            _commands.Add("!-ITEM", removeItem);
            _commands.Add("?WORLD", sendWorldData);
            _commands.Add("?PLAYERS", sendPlayers);
            _commands.Add(".READY", accept);
        }

        public static String getCmd(String msg)
        {
            if (msg.IndexOf(' ') == -1)
                return (msg);
            return (msg.Substring(0, msg.IndexOf(' ')));
        }

        public static String[] getArgs(String msg)
        {
            String[] args;
            int argc;

            argc = msg.Split(' ').Length - 1;
            args = new String[argc];
            msg = msg.Remove(0, msg.IndexOf(' ') + 1);
            for (int i = 0; i < argc; i++)
            {
                int nextSpace = msg.IndexOf(' ');

                if (nextSpace > 0)
                {
                    args[i] = msg.Substring(0, nextSpace);
                    msg = msg.Remove(0, nextSpace + 1);
                }
                else
                    args[i] = msg;
            }
            return (args);
        }

        public bool ParseCmd(String msg)
        {
            String cmd;
            String[] args;

            cmd = getCmd(msg);
            if (cmd == ".ACK")
                return (true);
            args = getArgs(msg);
            if (!_commands.ContainsKey(cmd))
                return (false);
            
            _commands[cmd](args);
            return (true);
        }

        public bool ParseExpectedCmd(String msg, String expected)
        {
            String cmd = getCmd(msg);

            if (cmd != expected)
                return (false);
            return (ParseCmd(msg));
        }

        //Handshake
        bool checkVersion(String[] args)
        {
            //No mismatch for now
            accept(null);
            return (true);
        }

        bool whoisClient(String[] args)
        {
            accept(null);
            return (true);
        }

        bool handShake(String[] args)
        {
            _client.send(".HANDSHAKE");
            return (true);
        }

        //Player
        bool movePlayer(String[] args)
        {
            Vector2 pos;
            
            float.TryParse(args[0], out pos.X);
            float.TryParse(args[1], out pos.Y);
            if (Server.Level.Collide(pos))
            {
                _client.send("!MOVE " + _client.Id + " " + _client.Player.Pos.X + " " + _client.Player.Pos.Y);
                return (true);
            }
            Server.ClientList[_client.Id].Player.Pos = pos;          
            Server.broadcast(_client.Id, "!MOVE " + _client.Id + " " + args[0] + " " + args[1]);
            return (true);
        }

        bool aimPlayer(String[] args)
        {
            float aim;
            
            float.TryParse(args[0], out aim);
            Server.ClientList[_client.Id].Player.Aim = aim;
            Server.broadcast(_client.Id, "!AIM " + _client.Id + " " + args[0]);
            return (true);
        }


        //World data
        bool sendWorldData(String[] args)
        {
            Level level = Server.Level;
            for (int i = 0; i < level.Entities.Length; i++)
            {
                if (level.Entities[i] != null)
                {
                    for (int j = 0; j < level.Entities[i].Count; j++)
                    {
                        Item item = level.Entities[i][j];
                        int x, y;

                        x = i % Server.Level.Map.Width;
                        y = i / Server.Level.Map.Width;
                        _client.send("!+ITEM " + item.Id + " " + x + " " + y);
                        String ncmd = _client.receive();
                        if (!ParseExpectedCmd(ncmd, ".ACK"))
                            return (false);
                    }
                }
            }
            _client.send(".READY");
            return (true);
        }

        bool sendPlayers(String[] args)
        {
            for (int i = 0; i < Server.ClientList.Count; i++)
            {
                Player player = Server.ClientList[i].Player;

                if (_client.Id != i)
				    _client.send("!+PLAYER " + Server.ClientList[i].Id + " " + player.Pos.X + " " + player.Pos.Y);
            }
            accept(null);
            return (true);
        }
        bool fireWeapon(String[] args)
        {
            int type, damage;
            float angle, velocity;
            Vector2 pos;

            Int32.TryParse(args[0], out type);
            float.TryParse(args[1], out pos.X);
            float.TryParse(args[2], out pos.Y);
            float.TryParse(args[3], out angle);
            float.TryParse(args[4], out velocity);
            Int32.TryParse(args[5], out damage);
            Server.Level.Projectiles.Add(new Projectile((ProjectileType)type, pos, angle, velocity, damage));
            Server.broadcast(_client.Id, "!FIRE " + args[0] + " " + args[1] + " " + args[2] + " " + args[3] + " " + args[4] + " " + args[5]);
            accept(null);
            return (true);
        }

        bool spawnItem(String[] args)
        {
            Vector2 pos;
            int id;

            Int32.TryParse(args[0], out id);
            float.TryParse(args[1], out pos.X);
            float.TryParse(args[2], out pos.Y);
            if (Server.Level.Entities[(int)(pos.Y * Server.Level.Map.Width) + (int)pos.X] == null)
                Server.Level.Entities[(int)(pos.Y * Server.Level.Map.Width) + (int)pos.X] = new List<Item>();
            Server.Level.Entities[(int)(pos.Y * Server.Level.Map.Width) + (int)pos.X].Add(new Item(id));
            accept(null);
            Server.broadcast(_client.Id, "!+ITEM " + args[0] + " " + args[1] + " " + args[2]);
            return (true);
        }

        bool removeItem(String[] args)
        {
            int x, y, index;

            Int32.TryParse(args[0], out index);
            Int32.TryParse(args[1], out x);
            Int32.TryParse(args[2], out y);
            if (Server.Level.Entities[(y * Server.Level.Map.Width) + x] != null)
                Server.Level.Entities[(y * Server.Level.Map.Width) + x].RemoveAt(index);
            else if (Server.Level.Entities[(y * Server.Level.Map.Width) + x].Count == 0)
                Server.Level.Entities[(y * Server.Level.Map.Width) + x] = null;
            accept(null);
            Server.broadcast(_client.Id, "!-ITEM " + args[0] + " " + args[1] + " " + args[2]);
            return (true);
        }

        //Accept / Refuse switches
        bool accept(String[] args)
        {
            _client.send(".OK");
            return (true);
        }

        void refuse(String[] args)
        {
            _client.send(".KO");
        }
    }
}
