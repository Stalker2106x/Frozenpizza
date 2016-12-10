using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class Character
    {
		//Stats
		int _id;
        String _name;
        int _hp, _maxHp;

		//Movement
        protected Vector2 _pos, _origin;
		protected float _aim;

		//Graphics
        protected Rectangle _skinRect;

        public Character(String name)
        {
            _name = name;
            _hp = 100;
            _maxHp = 100;
        }

        public virtual void Load()
        {

        }

		public int Id
		{
			get { return (_id); }
			set { _id = value; }
		}
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }
        public float Aim
        {
            get { return _aim; }
            set { _aim = value; }
        }
        public int maxHP
        {
            get { return (_maxHp); }
            set { _maxHp = value; }
        }
        public int HP
        {
            get { return _hp; }
            set { _hp = value; }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Engine.collection.Players, _pos, _skinRect, Color.White, -_aim, _origin, 1.0f, SpriteEffects.None, 0.3f);
        }
    }
}
