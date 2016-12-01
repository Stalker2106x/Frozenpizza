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

        public CommandHandler()
        {
            _commands.Add("!VERSION", checkVersion);
        }

        public static String getCmd(String msg)
        {
            return (msg.Substring(0, msg.IndexOf(' ') - 1));
        }

        public static String[] getArgs(String msg)
        {
            String[] args;
            int argc;

            argc = msg.Split(' ').Length;
            args = new String[argc];
            for (int i = 0; i < argc; i++)
            {
                int nextSpace = msg.IndexOf(' ');

                if (nextSpace < 0)
                {
                    args[i] = msg.Substring(0, nextSpace - 1);
                    msg.Remove(0, nextSpace);
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
            ParseCmd(msg);
            return (true);
        }

        bool whoIs(String[] args)
        {
            return (true);
        }

        bool checkVersion(String[] args)
        {
            //ENUMERATE ITEMS
            return (true);
        }
    }
}
