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

        public Cursor()
        {
            Show = true;
            _origin = new Vector2(16, 16);
        }

        public void Load(ContentManager content)
        {
            _skin = content.Load<Texture2D>("gfx/cursors");
            _skinRect = new Rectangle(192, 0, 64, 64);
        }

        public Vector2 Pos { get; set; }
        public bool Show { get; set; }

        public void Update(MouseState[] mStates)
        {
            Pos = mStates[1].Position.ToVector2();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_skin, Pos - _origin, _skinRect, Color.White, 0f, _origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
