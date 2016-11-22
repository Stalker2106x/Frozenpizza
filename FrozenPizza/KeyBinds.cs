using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class KeyBinds
    {
        Dictionary<string, Keys> _keybinds;

        public KeyBinds()
        {
            unbindall();
        }

        public void unbindall()
        {
            _keybinds = new Dictionary<string, Keys>();
        }

        public void bind(string action, Keys key)
        {
            if (!alreadyBound(key))
                _keybinds.Add(action, key);
        }

        public bool alreadyBound(Keys key)
        {
            if (_keybinds.FirstOrDefault(x => x.Value == key).Key != null)
                return (true);
            return (false);
        }
    }
}
