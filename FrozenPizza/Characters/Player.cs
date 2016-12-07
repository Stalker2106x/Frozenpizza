using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace FrozenPizza
{
	public class Player : Character
	{
		public Player(String name, Vector2 pos) : base(name)
        {
            _skinRect = new Rectangle(64, 0, 32, 16);
            _pos = pos;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
