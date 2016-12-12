using FrozenPizza;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class CommandHandler
    {
        Dictionary<String, Func<String[], bool>> _commands;

        public CommandHandler()
        {
            _commands = new Dictionary<String, Func<String[], bool>>();
            _commands.Add(".WELCOME", acknowledge);
            _commands.Add("?VERSION", sendVersion);
            _commands.Add("?WHOIS", sendWhois);
            _commands.Add("!MAP", setMap);
			_commands.Add("!PLAYER", setMainPlayer);
            _commands.Add("!+PLAYER", addNewPlayer);
            _commands.Add("!MOVE", movePlayer);
            _commands.Add("!AIM", aimPlayer);
            _commands.Add("!+FIRE", createProjectile);
            _commands.Add("!HIT", playerHit);
            _commands.Add("!DIE", killPlayer);
            _commands.Add("!++ITEM", spawnItem);
            _commands.Add("!+ITEM", dropItem);
            _commands.Add("!-ITEM", pickItem);
            _commands.Add(".HANDSHAKE", handShake);
            _commands.Add(".READY", ready);
            _commands.Add(".OK", accept);
            _commands.Add(".KO", error);
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

            if (msg == null)
                return (false);
            cmd = getCmd(msg);
            args = getArgs(msg);
            if (!_commands.ContainsKey(cmd))
                return (false);
            _commands[cmd](args);
            return (true);
        }

        public bool ParseExpectedCmd(String msg, String expected)
        {
            if (msg == null)
                return (false);
            if (getCmd(msg) != expected)
                return (false);
            return (ParseCmd(msg));
        }

        //Handshake
        bool sendVersion(String[] args)
        {
            NetHandler.send("!VERSION " + Assembly.GetEntryAssembly().GetName().Version.ToString());
            NetHandler.ConnectionStatus = "Checking version...";
            return (true);
        }

        bool sendWhois(String[] args)
        {
            NetHandler.send("!WHOIS");
            NetHandler.ConnectionStatus = "Sending client info...";
        return (true);
        }

        bool handShake(String[] args)
        {
            NetHandler.send(".HANDSHAKE");
            NetHandler.ConnectionStatus = "Handshake success!";
            Engine.netHandle.Handshake = true;
            return (true);
        }

		//Player
		bool setMainPlayer(String[] args)
		{
            Vector2 pos;
			int id;

			Int32.TryParse(args[0], out id);
			Engine.MainPlayer.Id = id;
			float.TryParse(args[1], out pos.X);
			float.TryParse(args[2], out pos.Y);
			Engine.MainPlayer.Pos = Engine.Level.vgridToMap(pos);
            acknowledge(null);
            return (true);
		}

        bool setMap(String[] args)
        {
            List<String> levels = Level.getAvailableLevels();
            int index = args[0].LastIndexOf('/') + 1;

            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i] == args[0].Substring(index, args[0].Length - (index + 4)))
                {
                    Level.MapName = "Data/maps/" + levels[i] + ".tmx";
                    acknowledge(null);
                    return (true);
                }
            }
            NetHandler.ConnectionStatus = args[0].Substring(index, args[0].Length - (index + 4)) + ": Missing map.";
            nacknowledge(null);
            return (false);
        }

        bool addNewPlayer(String[] args)
        {
            int id;
            Vector2 pos;

            Int32.TryParse(args[0], out id);
            float.TryParse(args[1], out pos.X);
            float.TryParse(args[2], out pos.Y);
            Engine.Players.Add(new Player(id, "RP", Engine.Level.vgridToMap(pos)));
            acknowledge(null);
            return (true);
        }
        bool movePlayer(String[] args)
        {
            int id;
            Vector2 pos;

            Int32.TryParse(args[0], out id);
            float.TryParse(args[1], out pos.X);
            float.TryParse(args[2], out pos.Y);
            if (Engine.MainPlayer != null && id == Engine.MainPlayer.Id)
                Engine.MainPlayer.Pos = pos;
            else if (Engine.getPlayerById(id) != null)
                Engine.getPlayerById(id).Pos = pos;
            return (true);
        }

        bool aimPlayer(String[] args)
        {
            int id;
            float aim;
            Player player;

            Int32.TryParse(args[0], out id);
            float.TryParse(args[1], out aim);
            player = Engine.getPlayerById(id);
            if (player == null)
                return (false);
            player.Aim = aim;
            return (true);
        }

        bool createProjectile(String[] args)
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
            Engine.Level.Projectiles.Add(new Projectile(ProjectileType.Bullet, pos, angle, velocity, damage));
            acknowledge(null);
            return (true);
        }

        bool playerHit(String[] args)
        {
            int id, damage;

            Int32.TryParse(args[0], out id);
            Int32.TryParse(args[1], out damage);
            if (id == Engine.MainPlayer.Id)
                Engine.MainPlayer.HP -= damage;
            else
                Engine.getPlayerById(id).HP -= damage;
            acknowledge(null);
            return (true);
        }

        bool killPlayer(String[] args)
        {

            int id;

            Int32.TryParse(args[0], out id);
            if (id == Engine.MainPlayer.Id)
                Engine.MainPlayer.die();
            else
                Engine.getPlayerById(id).die();
            return (true);
        }

        //World data
        bool spawnItem(String[] args)
        {
            Vector2 pos;
            int id;
            Int64 uid;

            Int64.TryParse(args[0], out uid);
            Int32.TryParse(args[1], out id);
            float.TryParse(args[2], out pos.X);
            float.TryParse(args[3], out pos.Y);
			Engine.Level.Entities.Add(Engine.collection.getNewItemById(uid, id));
			Engine.Level.Entities.Last().Pos = pos;
            acknowledge(null);
            return (true);
        }

        bool dropItem(String[] args)
        {
            Vector2 pos;
            Int64 uid;

            Int64.TryParse(args[0], out uid);
            float.TryParse(args[1], out pos.X);
            float.TryParse(args[2], out pos.Y);
            Engine.Level.Entities[Engine.Level.getEntityIndex(uid)].Pos = pos;
            acknowledge(null);
            return (true);
        }

        bool pickItem(String[] args)
        {
            Int64 uid;

            Int64.TryParse(args[0], out uid);
            Engine.Level.Entities[Engine.Level.getEntityIndex(uid)].Pos = -Vector2.One;
            return (true);
        }

        //ready switch
        bool ready(String[] args)
        {
            NetHandler.send(".READY");
            Engine.netHandle.Ready = true;
            return (true);
        }

        //Ack & Nack switches
        bool accept(String[] args)
        {
            return (true);
        }
        bool error(String[] args)
        {
            return (false);
        }
        bool acknowledge(String[] args)
        {
            NetHandler.send(".ACK");
            return (true);
        }

        bool nacknowledge(String[] args)
        {
            NetHandler.send(".NACK");
            return (true);
        }
    }
}
