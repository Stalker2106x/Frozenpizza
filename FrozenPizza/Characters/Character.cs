using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
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
        protected bool _alive;

        //Movement
        protected Vector2 _pos, _origin;
		protected float _aim;

		//Graphics
        protected Rectangle _skinRect;

        //Sound
        protected SoundEffect[] _sounds;

        public Character(String name)
        {
            _alive = true;
            _name = name;
            _hp = 100;
            _maxHp = 100;
        }

        //Initialization routine
        public void Load(ContentManager content)
        {
            _skinRect = new Rectangle(0, 0, 32, 16);
            _sounds = new SoundEffect[Enum.GetNames(typeof(PlayerSounds)).Length];
            _sounds[(int)PlayerSounds.Step1] = content.Load<SoundEffect>("sounds/player/step1");
            _sounds[(int)PlayerSounds.Step2] = content.Load<SoundEffect>("sounds/player/step2");
            _sounds[(int)PlayerSounds.Step3] = content.Load<SoundEffect>("sounds/player/step3");
            _sounds[(int)PlayerSounds.Step4] = content.Load<SoundEffect>("sounds/player/step4");
            _sounds[(int)PlayerSounds.RunStep1] = content.Load<SoundEffect>("sounds/player/rstep1");
            _sounds[(int)PlayerSounds.RunStep2] = content.Load<SoundEffect>("sounds/player/rstep2");
            _sounds[(int)PlayerSounds.RunStep3] = content.Load<SoundEffect>("sounds/player/rstep3");
            _sounds[(int)PlayerSounds.RunStep4] = content.Load<SoundEffect>("sounds/player/rstep4");
            _sounds[(int)PlayerSounds.Hurt] = content.Load<SoundEffect>("sounds/player/hurt");
            _sounds[(int)PlayerSounds.Die] = content.Load<SoundEffect>("sounds/player/die");
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

        public bool Alive
        {
            get { return (_alive);  }
        }

        public Rectangle getHitbox()
        {
            return (new Rectangle((Pos - new Vector2(8, 8)).ToPoint(), new Point(16, 16)));
        }

        public float getDistanceTo(Vector2 pos)
        {
            return ((float)Math.Sqrt(Math.Pow(Pos.X - pos.X, 2) + Math.Pow(Pos.Y - pos.Y, 2)));
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Engine.collection.Players, _pos, _skinRect, Color.White, -_aim, _origin, 1.0f, SpriteEffects.None, 0.3f);
        }
    }
}
