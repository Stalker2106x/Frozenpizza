using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FrozenPizza
{
    public abstract class Menu
    {
        protected String[] _items;
        protected Rectangle[] _itemRect;
        protected int _selected, _itemCount;
        protected float _fontsize;
        String _fontfile, _bgfile;
        SpriteFont _font;
        Texture2D _background;
        protected Engine _engine;

        public Menu(Engine engine, String name)
        {
            XElement bundle = XElement.Load("Data/items/menus.xml");
            var menu = bundle.Elements(name);
            int id = 0;

            _engine = engine;
            _selected = 0;
            _itemCount = menu.Elements("Item").Count() + menu.Elements("Setting").Count();
            _items = new String[_itemCount];
            _itemRect = new Rectangle[_itemCount];
            _fontfile = menu.Elements("Font").First().Value.ToString();
            _fontsize = 2f;
            _bgfile = menu.Elements("Background").First().Value.ToString();
            foreach (var item in menu.Elements("Item"))
            {
                _items[id] = item.Value.ToString();
                id++;
            }
        }

        public void Load(ContentManager content)
        {
            _background = content.Load<Texture2D>("gfx/bg/"+_bgfile);
            _font = content.Load<SpriteFont>("font/"+_fontfile);
            for (int id = 0; id < _itemCount; id++)
            {
                Vector2 size = _font.MeasureString(_items[id]);

                _itemRect[id] = new Rectangle(50, (int)(_engine.GraphicsDevice.Viewport.Height - 50) - (int)((_itemCount - id) * (size.Y * _fontsize)), (int)(size.X * _fontsize), (int)(size.Y * _fontsize));
            }
        }

        abstract public void itemClicked(int index);
        public virtual void Update(KeyboardState[] keybStates, MouseState[] mStates)
        {
            _selected = -1;
            for (int i = 0; i < _itemCount; i++)
            {
                if (_itemRect[i].Contains(mStates[1].Position))
                    _selected = i;                 
            }
            if (_selected >= 0 && mStates[0].LeftButton == ButtonState.Released && mStates[1].LeftButton == ButtonState.Pressed)
            {
                itemClicked(_selected);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            spriteBatch.Draw(_background, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            for (int i = 0; i < _itemCount; i++)
            {
                Color color = Color.White;

                if (i == _selected)
                    color = Color.Red;
                spriteBatch.DrawString(_font, _items[i], _itemRect[i].Location.ToVector2(), color, 0f, Vector2.Zero, _fontsize, SpriteEffects.None, 0f);
            }
        }
    }
}
