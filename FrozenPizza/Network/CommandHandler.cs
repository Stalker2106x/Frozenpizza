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
			_commands.Add("!PLAYER", setMainPlayer);
            _commands.Add("!+PLAYER", addNewPlayer);
            _commands.Add("!MOVE", movePlayer);
            _commands.Add("!+ITEM", spawnItem);
            _commands.Add("!-ITEM", removeItem);
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

            cmd = getCmd(msg);
            args = getArgs(msg);
            if (!_commands.ContainsKey(cmd))
                return (false);
            _commands[cmd](args);
            return (true);
        }

        public bool ParseExpectedCmd(String msg, String expected)
        {
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

        bool addNewPlayer(String[] args)
        {
            int id;
            Vector2 pos;

            Int32.TryParse(args[0], out id);
            float.TryParse(args[1], out pos.X);
            float.TryParse(args[2], out pos.Y);
            Engine.Players.Add(new Player(id, "RP", Engine.Level.vgridToMap(pos)));
            return (true);
        }
        bool movePlayer(String[] args)
        {
            int id;
            Vector2 pos;
            Player player;

            Int32.TryParse(args[0], out id);
            float.TryParse(args[1], out pos.X);
            float.TryParse(args[2], out pos.Y);
            player = Engine.getPlayerById(id);
            if (player == null)
                return (false);
            player.Pos = pos;
            return (true);
        }

        //World data
        bool spawnItem(String[] args)
        {
            List<Item> entities;
            Vector2 pos;
            int id;

            Int32.TryParse(args[0], out id);
            float.TryParse(args[1], out pos.X);
            float.TryParse(args[2], out pos.Y);
            if (Engine.Level.getEntities(pos) == null)
                entities = new List<Item>();
            else
                entities = Engine.Level.getEntities(pos);
            entities.Add(Engine.collection.getItemById(id));
            Engine.Level.setEntities(pos, entities);
            acknowledge(null);
            return (true);
        }

        bool removeItem(String[] args)
        {
            int x, y, index;

            Int32.TryParse(args[0], out index);
            Int32.TryParse(args[1], out x);
            Int32.TryParse(args[2], out y);
            Engine.Level.getEntities(new Vector2(x, y)).RemoveAt(index);
            return (true);
        }

        //ready switch
        bool ready(String[] args)
        {
            NetHandler.send(".READY");
            Engine.netHandle.Hooked = true;
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
