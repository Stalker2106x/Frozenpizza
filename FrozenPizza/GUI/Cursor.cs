using FrozenPizza.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
  public class Cursor
  {
    Texture2D _skin;
    Rectangle _skinRect;
    Vector2 _origin;
    bool _clicked;

    public Cursor()
    {
      Show = true;
      _origin = Vector2.Zero;
      _clicked = false;
    }

    public void Load(ContentManager content)
    {
      _skin = content.Load<Texture2D>("gfx/cursors");
      _skinRect = new Rectangle(0, 0, 25, 25);
    }

    public Vector2 Pos { get; set; }
    public bool Show { get; set; }

    public void Update(DeviceState state, DeviceState prevState)
    {
      Pos = state.mouse.Position.ToVector2();
      if (state.mouse.LeftButton == ButtonState.Pressed)
        _clicked = true;
      else if (prevState.mouse.LeftButton == ButtonState.Pressed && state.mouse.LeftButton == ButtonState.Released)
        _clicked = false;
    }
    public void Draw(SpriteBatch spriteBatch)
    {
      if (!Show) return;
      Rectangle realrect = _skinRect;

      if (_clicked)
        realrect.X += 25;
      spriteBatch.Draw(_skin, Pos - _origin, realrect, Color.White, 0f, _origin, 1f, SpriteEffects.None, 0f);
    }
  }
}
