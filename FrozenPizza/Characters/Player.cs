using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace FrozenPizza
{
	public class Player : Character
	{
		public Player(int id, String name, Vector2 pos) : base(name)
        {
            Id = id;
            _origin = new Vector2(16, 8);
            _skinRect = new Rectangle(64, 0, 32, 16);
            _pos = pos;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
