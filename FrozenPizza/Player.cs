using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace FrozenPizza
{
    public enum PlayerStates
    {
        Hungry,
        Thirsty,
        Starving,
        Dehydrated,
    }
    public class Player
    {
        String _name;
        int _hp, _hunger, _thirst;
        int _maxHp, _maxHunger, _maxThirst;
        float _aim;
        Vector2 _pos, _origin;
        Rectangle _skinRect;
        Texture2D _skin;
        List<Item> _inventory;
        List<PlayerStates> _states;
        SoundEffect[] _stepSound;
		TimeSpan _stepTimer;
        TimeSpan _stateTimer;

		public Player(String name, Vector2 spawn)
        {
            _name = name;
            _hp = 100;
            _maxHp = 100;
            _hunger = 50;
            _maxHunger = 100;
            _thirst = 25;
            _maxThirst = 100;
			_pos = spawn;
            _aim = 0;
            _inventory = new List<Item>();
            _states = new List<PlayerStates>();
            _stateTimer = new TimeSpan();
            _stepTimer = new TimeSpan();
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
        public int maxHunger
        {
            get { return (_maxHunger); }
            set { _maxHunger = value; }
        }
        public int Hunger
        {
            get { return _hunger; }
            set { _hunger = value; }
        }
        public int maxThirst
        {
            get { return (_maxThirst); }
            set { _maxThirst = value; }
        }
        public int Thirst
        {
            get { return _thirst; }
            set { _thirst = value; }
        }

        public void Load(ContentManager content)
        {
            _skinRect = new Rectangle(64, 0, 32, 16);
            _skin = content.Load<Texture2D>(@"gfx/players");
            _origin = new Vector2(16, 8);
            _stepSound = new SoundEffect[8];
            _stepSound[0] = content.Load<SoundEffect>("sounds/step1");
            _stepSound[1] = content.Load<SoundEffect>("sounds/step2");
            _stepSound[2] = content.Load<SoundEffect>("sounds/step3");
            _stepSound[3] = content.Load<SoundEffect>("sounds/step4");
			_stepSound[4] = content.Load<SoundEffect>("sounds/rstep1");
			_stepSound[5] = content.Load<SoundEffect>("sounds/rstep2");
			_stepSound[6] = content.Load<SoundEffect>("sounds/rstep3");
			_stepSound[7] = content.Load<SoundEffect>("sounds/rstep4");
        }

        public void die()
        {

		}

        public int getArmor()
        {
            return (0);
        }

        public float getWeight()
        {
            float weight = 0;

            foreach (Item item in _inventory)
            {
                weight += item.Weight;
            }
            return (weight);
        }

		public float getSpeed(KeyboardState keybState)
        {
			float speed = 1.25f;

            if (keybState.IsKeyDown(Keys.LeftShift))
				speed = 2.0f;
			return (speed - (getWeight() / 10));
        }

		void stepSound(GameTime gameTime, bool run)
		{
			_stepTimer += gameTime.ElapsedGameTime;
			if (run && _stepTimer >= _stepSound[4].Duration)
			{
				Random rnd = new Random();

				_stepSound[rnd.Next(4, 8)].Play();
				_stepTimer = TimeSpan.Zero;
			}
			else if (!run && _stepTimer >= _stepSound[0].Duration)
			{
				Random rnd = new Random();

				_stepSound[rnd.Next(0, 4)].Play();
				_stepTimer = TimeSpan.Zero;
			}
        }

        bool checkMove(Level level, Vector2 oldpos)
        {
			if (level.Collide(_pos))
			{
				_pos = oldpos;
				return (false);
			}
			return (true);
        }

        void updateMove(GameTime gameTime, KeyboardState keybState, Level level)
        {
			
            Vector2 oldpos = _pos;
			bool move = false;
			float speed = getSpeed(keybState);

            if (keybState.IsKeyDown(Keys.A))
            {
				if (keybState.IsKeyDown(Keys.W) || keybState.IsKeyDown(Keys.S))
					speed *= 0.75f;
				_pos += new Vector2((float)Math.Sin(_aim + MathHelper.PiOver2) * speed, (float)Math.Cos(_aim + MathHelper.PiOver2) * -speed);
                move = checkMove(level, oldpos);
            }
            else if (keybState.IsKeyDown(Keys.D))
            {
				if (keybState.IsKeyDown(Keys.W) || keybState.IsKeyDown(Keys.S))
					speed *= 0.75f;
				_pos += new Vector2((float)Math.Sin(_aim + MathHelper.PiOver2) * -speed, (float)Math.Cos(_aim + MathHelper.PiOver2) * speed);
				move = checkMove(level, oldpos);
            }
			speed = getSpeed(keybState); //Reset speed
            if (keybState.IsKeyDown(Keys.W))
            {
                _pos += new Vector2((float)Math.Sin(_aim) * -speed, (float)Math.Cos(_aim) * speed);
                move = checkMove(level, oldpos);
            }
            else if (keybState.IsKeyDown(Keys.S))
            {
				_pos += new Vector2((float)Math.Sin(_aim) * speed, (float)Math.Cos(_aim) * -speed);
				move = checkMove(level, oldpos);
            }
			if (move)
				stepSound(gameTime, keybState.IsKeyDown(Keys.LeftShift) ? true : false);
        }

        float getAimAngle(Camera cam, MouseState mState)
        {
            Vector2 aimVector = _pos - Vector2.Transform(new Vector2(mState.X, mState.Y), Matrix.Invert(cam.getTransformation()));

            aimVector.Normalize();
            return ((float)Math.Atan2((double)aimVector.Y, (double)aimVector.X) + MathHelper.PiOver2);
        }

        public bool checkState(PlayerStates state)
        {
            for (int i = 0; i < _states.Count; i++)
                if (_states[i] == state)
                    return (true);
            return (false);
        }
        void updateStates(GameTime gameTime)
        {
            _stateTimer += gameTime.ElapsedGameTime;
            if (_hp <= 0)
            {
                _hp = 0;
                die();
            }
            if (_hunger > 0 && _hunger < 50 && !checkState(PlayerStates.Hungry))
                _states.Add(PlayerStates.Hungry);
            else if (_hunger <= 0 && !checkState(PlayerStates.Starving))
                _states.Add(PlayerStates.Starving);
			if (_thirst > 0 && _thirst < 50 && !checkState(PlayerStates.Thirsty))
				_states.Add(PlayerStates.Thirsty);
			else if (_thirst <= 0 && !checkState(PlayerStates.Dehydrated))
				_states.Add(PlayerStates.Dehydrated);
        }

        void applyStates()
        {
            _stateTimer = TimeSpan.Zero;
            if (checkState(PlayerStates.Starving))
                _hp -= 1;
        }

        public void Update(GameTime gameTime, Level level, KeyboardState keybState, MouseState mState, Camera cam)
        {
            _aim = getAimAngle(cam, mState);
            updateMove(gameTime, keybState, level);
            updateStates(gameTime);
            if (_states.Count > 0 && _stateTimer.TotalSeconds >= 20)
                applyStates();
            cam.Pos = _pos;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_skin, _pos, _skinRect, Color.White, _aim, _origin, 1.0f, SpriteEffects.None, 0);
        }
    }

}