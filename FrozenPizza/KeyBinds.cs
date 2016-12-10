using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class KeyBinds
    {
        static Dictionary<string, Keys> _keybinds;

        public KeyBinds()
        {
            unbindall();
            readKeybinds();
        }

        public void readKeybinds()
        {
            StreamReader file = new StreamReader("./Data/cfg/controls.cfg");
            String line;

            while ((line = file.ReadLine()) != null)
            {
                bind(line.Substring(0, line.IndexOf('=')), line.Substring(line.IndexOf('=') + 1, line.Length - line.IndexOf('=') - 1));
            }
            file.Close();
        }

        public void unbindall()
        {
            _keybinds = new Dictionary<String, Keys>();
        }

        public void bind(String action, String key)
        {
            if (!alreadyBound(stringToKey(key)))
                _keybinds.Add(action, stringToKey(key));
        }

        public Keys stringToKey(String key)
        {
            return ((Keys)Enum.Parse(typeof(Keys), key));
        }


        public bool alreadyBound(Keys key)
        {
            if (_keybinds.FirstOrDefault(x => x.Value == key).Key != null)
                return (true);
            return (false);
        }

        public static Keys getKey(String action)
        {
            if (_keybinds.ContainsKey(action))
                return (_keybinds[action]);
            else
                return (Keys.None);
        }
    }
}
