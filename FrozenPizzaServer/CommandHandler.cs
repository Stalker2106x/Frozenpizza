using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{
    class CommandHandler
    {
        Dictionary<String, Func<String[], bool>> _commands;

        CommandHandler()
        {
            _commands.Add("?ENUMERATE", enumerateItems);
        }

        bool ParseCmd(String msg)
        {
            String cmd;
            String[] args;
            int argc;

            argc = msg.Split(' ').Length + 1;
            args = new String[argc];
            cmd = msg.Substring(0, msg.IndexOf(' ') - 1);
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
            if (!_commands.ContainsKey(cmd))
                return (false);
            _commands[cmd](args);
            return (true);
        }

        bool whoIs(String[] args)
        {
            return (true);
        }

        bool enumerateItems(String[] args)
        {
            //ENUMERATE ITEMS
            return (true);
        }
    }
}
