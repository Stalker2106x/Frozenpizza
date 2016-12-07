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
		float _aim;

		//Graphics
        protected Rectangle _skinRect;
        protected Texture2D _skin;

        public Character(String name)
        {
            _name = name;
            _hp = 100;
            _maxHp = 100;
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
    }
}
