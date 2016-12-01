using FrozenPizza;
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
            Player _client;

            public CommandHandler(Player client)
            {
                _client = client;
                _commands = new Dictionary<String, Func<String[], bool>>();
                _commands.Add(".WELCOME", acknowledge);
                _commands.Add("?VERSION", sendVersion);
                _commands.Add("?WHOIS", sendWhois);
                _commands.Add(".HANDSHAKE", handShake);
                _commands.Add(".OK", accept);
                _commands.Add(".KO", error);
            }

            public static String getCmd(String msg)
            {
                if (msg.IndexOf(' ') == -1)
                    return (msg);
                return (msg.Substring(0, msg.IndexOf(' ') - 1));
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
