using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        String _fontfile, _bgfile, _gameVersion;
        protected SpriteFont _font;
        Texture2D _background;
        protected Engine _engine;

        public float BackgroundOpacity { get; set; }

        public Menu(Engine engine, String name)
        {
            XElement bundle = XElement.Load("Data/items/menus.xml");
            var menu = bundle.Elements(name);
            int id = 0;

            _engine = engine;
            _selected = 0;
            _gameVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
            _itemCount = menu.Elements("Item").Count();
            _items = new String[_itemCount];
            _itemRect = new Rectangle[_itemCount];
            _fontfile = menu.Elements("Font").First().Value.ToString();
            _fontsize = 1f;
            _bgfile = menu.Elements("Background").First().Value.ToString();
            BackgroundOpacity = 1f;
            foreach (var item in menu.Elements("Item"))
            {
                _items[id] = item.Value.ToString();
                id++;
            }
        }

        public virtual void Load(ContentManager content)
        {
            _background = content.Load<Texture2D>("gfx/bg/"+_bgfile);
            _font = content.Load<SpriteFont>("font/"+_fontfile);
            initItems();
        }

        public void initItems()
        {
            for (int id = 0; id < _itemCount; id++)
            {
                Vector2 size = _font.MeasureString(_items[id]);

                _itemRect[id] = new Rectangle(50, (int)(_engine.GraphicsDevice.Viewport.Height - 50) - (int)((_itemCount - id) * (size.Y * _fontsize)), (int)(size.X * _fontsize), (int)(size.Y * _fontsize));
            }
        }

        abstract public void itemClicked(int index);

        bool itemHovered(MouseState[] mStates)
        {
            for (int i = 0; i < _itemCount; i++)
            {
                if (_itemRect[i].Contains(mStates[0].Position) && _itemRect[i].Contains(mStates[1].Position) && _selected == i)
                    return (true);
                else if ((!_itemRect[i].Contains(mStates[0].Position) && _itemRect[i].Contains(mStates[1].Position))
                    || (_itemRect[i].Contains(mStates[0].Position) && _itemRect[i].Contains(mStates[1].Position) && _selected != i))
                {
                    Engine.collection.MenuSounds[0].Play(Options.Config.SoundVolume, 0f, 0f);
                    _selected = i;
                    return (true);
                }
            }
            return (false);
        }

        public virtual void Update(KeyboardState[] keybStates, MouseState[] mStates)
        {
            if (!itemHovered(mStates))
                _selected = -1;
            if (_selected >= 0 && mStates[0].LeftButton == ButtonState.Released && mStates[1].LeftButton == ButtonState.Pressed)
            {
                Engine.collection.MenuSounds[1].Play(Options.Config.SoundVolume, 0f, 0f);
                itemClicked(_selected);
            }
        }

        public void drawBase(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            spriteBatch.DrawString(_font, _gameVersion, Vector2.Zero, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_background, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), null, Color.White * BackgroundOpacity, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            drawBase(spriteBatch, graphicsDevice);
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
