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
            _commands.Add("?WORLD", sendWorldData);
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
            for (int i = 0; i < argc; i++)
            {
                int nextSpace = msg.IndexOf(' ');

                if (nextSpace < 0)
                {
                    args[i] = msg.Substring(0, nextSpace);
                    msg.Remove(0, nextSpace + 1);
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
            String cmd = getCmd(msg);

            if (cmd != expected)
                return (false);
            return (ParseCmd(msg));
        }

        //Handshake
        bool checkVersion(String[] args)
        {
            //No mismatch for now
            accept();
            return (true);
        }

        bool whoisClient(String[] args)
        {
            accept();
            return (true);
        }

        bool handShake(String[] args)
        {
            _client.send(".HANDSHAKE");
            return (true);
        }

        //World data
        //Send world Data
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
                        _client.send("!ITEM " + x + " " + y + " " + item.Id);
                    }
                }
            }
            return (true);
        }

        //Accept / Refuse switches
        void accept()
        {
            _client.send(".OK");
        }

        void refuse()
        {
            _client.send(".KO");
        }
    }
}
