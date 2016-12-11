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
    abstract class SettingMenu : Menu
    {
        protected int _settingCount;
        String[] _settings;
        protected List<String>[] _settingValues;
        protected int[] _settingSelectedValue;
        Rectangle[] _settingRect;
        public SettingMenu(Engine engine, String name) : base(engine, name)
        {
            XElement bundle = XElement.Load("Data/items/menus.xml");
            var menu = bundle.Elements(name);
            int id = 0;

            _selected = 0;
            _settingCount = menu.Elements("Setting").Count();
            _settings = new String[_settingCount];
            _settingValues = new List<string>[_settingCount];
            _settingSelectedValue = new int[_settingCount];
            _settingRect = new Rectangle[_settingCount];
            foreach (var item in menu.Elements("Setting"))
            {
                _settings[id] = item.Value.ToString();
                _settingSelectedValue[id] = 0;
                id++;
            }
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            for (int id = 0; id < _settingCount; id++)
            {
                Vector2 size = _font.MeasureString(_settings[id]);

                _settingRect[id] = new Rectangle(50, 50 + (int)(id * (size.Y * _fontsize)), (int)(size.X * _fontsize), (int)(size.Y * _fontsize));
            }
        }

        public abstract override void itemClicked(int index);
        public abstract void setSettingValues();
        public void settingClicked(int index, bool next)
        {
            if (next)
                _settingSelectedValue[index]++;
            else
                _settingSelectedValue[index]--;

            if (_settingSelectedValue[index] >= _settingValues[index].Count)
                _settingSelectedValue[index] = 0;
            else if (_settingSelectedValue[index] < 0)
                _settingSelectedValue[index] = _settingValues[index].Count - 1;
        }


        bool itemHovered(MouseState[] mStates)
        {
            for (int i = 0; i < _settingCount; i++)
            {
                if (_settingRect[i].Contains(mStates[0].Position) && _settingRect[i].Contains(mStates[1].Position) && _selected == i)
                    return (true);
                else if ((!_settingRect[i].Contains(mStates[0].Position) && _settingRect[i].Contains(mStates[1].Position))
                    || (_settingRect[i].Contains(mStates[0].Position) && _settingRect[i].Contains(mStates[1].Position) && _selected != i))
                {
                    Engine.collection.MenuSounds[0].Play();
                    _selected = i;
                    return (true);
                }
            }
            for (int i = 0; i < _itemCount; i++)
            {
                if (_itemRect[i].Contains(mStates[0].Position) && _itemRect[i].Contains(mStates[1].Position) && _selected == _settingCount + i)
                    return (true);
                else if ((!_itemRect[i].Contains(mStates[0].Position) && _itemRect[i].Contains(mStates[1].Position))
                    || (_itemRect[i].Contains(mStates[0].Position) && _itemRect[i].Contains(mStates[1].Position) && _selected != _settingCount + i))
                {
                    Engine.collection.MenuSounds[0].Play();
                    _selected = _settingCount + i;
                    return (true);
                }
            }
            return (false);
        }

        public override void Update(KeyboardState[] keybStates, MouseState[] mStates)
        {
            if (!itemHovered(mStates))
                _selected = -1;
            if (_selected >= 0 && mStates[0].LeftButton == ButtonState.Released && mStates[1].LeftButton == ButtonState.Pressed)
            {
                Engine.collection.MenuSounds[1].Play();
                if (_selected < _settingCount)
                    settingClicked(_selected, true);
                else
                    itemClicked(_selected - _settingCount);
            }
            else if (_selected >= 0 && mStates[0].RightButton == ButtonState.Released && mStates[1].RightButton == ButtonState.Pressed)
            {
                if (_selected < _settingCount)
                    settingClicked(_selected, false);
            }
        }
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            drawBase(spriteBatch, graphicsDevice);
            for (int i = 0; i < _settingCount; i++)
            {
                Color color = Color.White;

                if (i == _selected)
                    color = Color.Red;
                spriteBatch.DrawString(_font, _settings[i], _settingRect[i].Location.ToVector2(), color, 0f, Vector2.Zero, _fontsize, SpriteEffects.None, 0f);
                spriteBatch.DrawString(_font, _settingValues[i][_settingSelectedValue[i]], _settingRect[i].Location.ToVector2() + new Vector2(_engine.GraphicsDevice.Viewport.Width - (50 + 400), 0), Color.White, 0f, Vector2.Zero, _fontsize, SpriteEffects.None, 0f);
            }
            for (int i = 0; i < _itemCount; i++)
            {
                Color color = Color.White;

                if (i + _settingCount == _selected)
                    color = Color.Red;
                spriteBatch.DrawString(_font, _items[i], _itemRect[i].Location.ToVector2(), color, 0f, Vector2.Zero, _fontsize, SpriteEffects.None, 0f);
            }
        }
    }
}
