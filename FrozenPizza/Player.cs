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
		bool _inventoryOpen, _cooldown;
        Item _hands;
        List<Item> _inventory;
        List<PlayerStates> _states;
        SoundEffect[] _stepSound;

        //Timers
		TimeSpan _stepTimer;
        TimeSpan _stateTimer;
        TimeSpan _cooldownTimer;
        float _mouseSensivity;

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
			_inventoryOpen = false;
            _inventory = new List<Item>();
            _states = new List<PlayerStates>();
            _stateTimer = new TimeSpan();
            _stepTimer = new TimeSpan();
            _mouseSensivity = 0.01f;
            _cooldown = false;
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
		public bool InventoryOpen
		{
			get { return (_inventoryOpen); }
			set { _inventoryOpen = value; }
		}

        public Item Hands
        {
            get { return (_hands); }
            set { _hands = value; }
        }

        public int Cooldown
        {
            get { return ((int)(_cooldownTimer.TotalMilliseconds / 1000) * 250); }
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

        void updateMove(GameTime gameTime, KeyboardState[] keybStates, Level level)
        {
			
            Vector2 oldpos = _pos;
			bool move = false;
			float speed = getSpeed(keybStates[1]);

            if (keybStates[1].IsKeyDown(Keys.A))
            {
                if (keybStates[1].IsKeyDown(Keys.W) || keybStates[1].IsKeyDown(Keys.S))
                    speed *= 0.75f;
                _pos += new Vector2((float)Math.Cos(_aim) * -speed, (float)Math.Sin(_aim) * speed);
                move = checkMove(level, oldpos);
            }
            else if (keybStates[1].IsKeyDown(Keys.D))
            {
                if (keybStates[1].IsKeyDown(Keys.W) || keybStates[1].IsKeyDown(Keys.S))
                    speed *= 0.75f;
                _pos += new Vector2((float)Math.Cos(_aim) * speed, (float)-Math.Sin(_aim) * speed);
                move = checkMove(level, oldpos);
            }
            speed = getSpeed(keybStates[1]); //Reset speed
            if (keybStates[1].IsKeyDown(Keys.W))
            {
                _pos += new Vector2((float)Math.Sin(_aim) * -speed, (float)Math.Cos(_aim) * -speed);
                move = checkMove(level, oldpos);
            }
            else if (keybStates[1].IsKeyDown(Keys.S))
            {
                _pos += new Vector2((float)Math.Sin(_aim) * speed, (float)Math.Cos(_aim) * speed);
                move = checkMove(level, oldpos);
            }
            if (move)
				stepSound(gameTime, keybStates[1].IsKeyDown(Keys.LeftShift) ? true : false);
        }

        public void updateCooldown(GameTime gameTime)
        {
            _cooldownTimer += gameTime.ElapsedGameTime;
            if (_cooldownTimer > TimeSpan.FromSeconds(1))
                    _cooldown = false;
        }
        public void updateAttack(GameTime gameTime, MouseState[] mStates)
        {
            if (_cooldown)
                updateCooldown(gameTime);
            if (mStates[1].LeftButton == ButtonState.Pressed && !_cooldown)
            {
                if (_hands != null)
                    _hands.use();
                _cooldown = true;
                _cooldownTimer = TimeSpan.Zero;
            }
            if (mStates[1].RightButton == ButtonState.Pressed)
                _cooldown = true;
        }

        void updateAimAngle(Camera cam, MouseState[] mStates)
        {
			if (mStates[1].X != cam.getViewport().Width / 2)
			if (mStates[0].X < mStates[1].X)
				_aim += (mStates[0].X - mStates[1].X) * _mouseSensivity;
			else if (mStates[0].X > mStates[1].X)
				_aim -= (mStates[1].X - mStates[0].X) * _mouseSensivity;
            if (_aim < 0)
				_aim = MathHelper.TwoPi;
			else if (_aim > MathHelper.TwoPi)
				_aim = 0;
			cam.Rotation = _aim;
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

        void toggleInventory()
        {
            _inventoryOpen = _inventoryOpen ? false : true;
        }

        public void pickupItem(Level level, int index)
        {
            if (_hands == null)
            {
                List<Item> entities = level.getEntities(_pos);
                if (entities == null)
                    return;
                _hands = entities[index];
                entities.RemoveAt(index);
                if (entities.Count == 0)
                    entities = null;
            }
        }

        public void dropItem(Level level, SlotType slot, int index)
        {
            List<Item> entities = level.getEntities(_pos);
            if (entities == null)
                entities = new List<Item>();
            if (slot == SlotType.Hands)
            {
                entities.Add(_hands);
                level.setEntities(_pos, entities);
                _hands = null;
            }

        }

        public void Update(GameTime gameTime, Level level, KeyboardState[] keybStates, MouseState[] mStates, Camera cam)
        {
            if (!InventoryOpen)
                updateAimAngle(cam, mStates);
            updateMove(gameTime, keybStates, level);
            updateAttack(gameTime, mStates);
            if (keybStates[1].IsKeyDown(Keys.Tab) && !keybStates[0].IsKeyDown(Keys.Tab))
                toggleInventory();
            if (keybStates[1].IsKeyDown(Keys.E) && !keybStates[0].IsKeyDown(Keys.E))
                pickupItem(level, 0);
            if (keybStates[1].IsKeyDown(Keys.G) && !keybStates[0].IsKeyDown(Keys.G))
                dropItem(level, SlotType.Hands, 0);
            updateStates(gameTime);
            if (_states.Count > 0 && _stateTimer.TotalSeconds >= 20)
                applyStates();
            cam.Pos = _pos;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_skin, _pos, _skinRect, Color.White, -_aim, _origin, 1.0f, SpriteEffects.None, 0);
        }
    }

}