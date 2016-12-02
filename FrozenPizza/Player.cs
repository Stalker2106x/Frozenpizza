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
        Dehydrated
    }
    public class Player : Character
    {
        //Stats
        int _hunger, _thirst;
        int _maxHunger, _maxThirst;

        //Triggers
        bool _inventoryOpen, _cooldown, _sprinting, _aimlock;

        //Aim View
        float _aim;

        //Inventory
        Item _hands;
        List<Item> _inventory;
        List<PlayerStates> _states;

        //Sound
        SoundEffect[] _stepSound;

        //Timers
		TimeSpan _stepTimer;
        TimeSpan _stateTimer;
        TimeSpan[] _cooldownTimer;
        float _aimSensivity;

		public Player(String name, Vector2 spawn) : base(name, spawn)
        {
            //Set Stats
            _hunger = 50;
            _maxHunger = 100;
            _thirst = 25;
            _maxThirst = 100;

            _states = new List<PlayerStates>();
            //Set triggers
            _inventoryOpen = false;
            _cooldown = false;
            _aimlock = false;
			_sprinting = false;

            //Set aim view
            _aim = 0;
            _aimSensivity = 0.005f;

            //Init Inventory
            _inventory = new List<Item>();

            //Init timers
            _stateTimer = new TimeSpan();
            _stepTimer = new TimeSpan();
            _cooldownTimer = new TimeSpan[2];
		}

        // ACCESSORS / SETTERS
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

		public ItemType HandsType
		{
			get { return (_hands.Type); }
		}

        public bool Cooldown
        {
            get { return (_cooldown); }
        }

        //Initialization routine
        public void Load(ContentManager content)
        {
            _skinRect = new Rectangle(64, 0, 32, 16);
            _skin = content.Load<Texture2D>(@"gfx/players");
            _origin = new Vector2(16, 8);
            _stepSound = new SoundEffect[8];
            _stepSound[0] = content.Load<SoundEffect>("sounds/player/step1");
            _stepSound[1] = content.Load<SoundEffect>("sounds/player/step2");
            _stepSound[2] = content.Load<SoundEffect>("sounds/player/step3");
            _stepSound[3] = content.Load<SoundEffect>("sounds/player/step4");
            _stepSound[4] = content.Load<SoundEffect>("sounds/player/rstep1");
            _stepSound[5] = content.Load<SoundEffect>("sounds/player/rstep2");
            _stepSound[6] = content.Load<SoundEffect>("sounds/player/rstep3");
            _stepSound[7] = content.Load<SoundEffect>("sounds/player/rstep4");
        }

        //Getters for private attributes or with operations

        public int getCooldownPercent(int width)
        {
            float elapsed = (float)(_cooldownTimer[1].TotalMilliseconds - _cooldownTimer[0].TotalMilliseconds);

            return ((int)((float)(elapsed / _cooldownTimer[1].TotalMilliseconds) * width));
        }

        public float getCooldown()
        {
            Weapon hands = (Weapon)_hands;

            if (hands != null)
                return ((float)TimeSpan.FromSeconds(hands.Cooldown).TotalMilliseconds);
            else
                return ((float)TimeSpan.FromSeconds(Engine.collection.MeleeList[0].Cooldown).TotalMilliseconds);
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



		public float[] getAimAccuracyAngle(bool real)
		{
			float[] aimAccuracyAngle = new float[2];

			aimAccuracyAngle[0] = MathHelper.PiOver2 * 1.5f;
			aimAccuracyAngle[1] = MathHelper.PiOver2 * 0.5f;
			if (_hands != null && _hands.Type == ItemType.Firearm)
			{
				Firearm weapon = (Firearm)_hands;

				aimAccuracyAngle[0] -= weapon.Accuracy * 0.1f;
				aimAccuracyAngle[1] += weapon.Accuracy * 0.1f;
			}
			if (_sprinting)
			{
				aimAccuracyAngle[0] += 0.2f;
				aimAccuracyAngle[1] -= 0.2f;
			}
            if (real)
            {
                aimAccuracyAngle[0] += _aim - MathHelper.PiOver2;
                aimAccuracyAngle[1] += _aim - MathHelper.PiOver2;
            }
			return (aimAccuracyAngle);
		}

		public float getSpeed(KeyboardState keybState)
        {
			float speed = 1.25f;

			if (_sprinting)
				speed = 2.0f;
			return (speed - (getWeight() / 10));
        }

        //Miscillaneous
        public void die()
        {
            //TODO: die mechanism
        }
        void stepSound(GameTime gameTime, bool run)
		{
			_stepTimer += gameTime.ElapsedGameTime;
			if (run && _stepTimer >= _stepSound[4].Duration)
			{
				Random rnd = new Random();

				_stepSound[rnd.Next(4, 8)].Play(0.5f, 0f, 0f);
				_stepTimer = TimeSpan.Zero;
			}
			else if (!run && _stepTimer >= _stepSound[0].Duration)
			{
				Random rnd = new Random();

				_stepSound[rnd.Next(0, 4)].Play(0.5f, 0f, 0f);
                _stepTimer = TimeSpan.Zero;
			}
        }

        //Moving mechanism functions
        bool checkMove(Level level, KeyboardState[] keybStates, Vector2 oldpos)
        {
            if (keybStates[1].IsKeyDown(Keys.LeftShift))
                _sprinting = true;
            if (level.Collide(_pos))
			{
                Vector2 posx, posy;

                posx = new Vector2(oldpos.X, _pos.Y);
                posy = new Vector2(_pos.X, oldpos.Y);
                if (!level.Collide(posx))
				    _pos = posx;
                else if (!level.Collide(posy))
                    _pos = posy;
                return (false);
			}
			return (true);
        }

        void updateMove(GameTime gameTime, KeyboardState[] keybStates, Level level)
        {
			
            Vector2 oldpos = _pos;
			bool move = false;
			float speed = getSpeed(keybStates[1]);

            if (keybStates[1].IsKeyUp(Keys.LeftShift))
                _sprinting = false;
            if (keybStates[1].IsKeyDown(Keys.A))
            {
                if (keybStates[1].IsKeyDown(Keys.W) || keybStates[1].IsKeyDown(Keys.S))
                    speed *= 0.75f;
                _pos += new Vector2((float)Math.Cos(_aim) * -speed, (float)Math.Sin(_aim) * speed);
                move = checkMove(level, keybStates, oldpos);
            }
            else if (keybStates[1].IsKeyDown(Keys.D))
            {
                if (keybStates[1].IsKeyDown(Keys.W) || keybStates[1].IsKeyDown(Keys.S))
                    speed *= 0.75f;
                _pos += new Vector2((float)Math.Cos(_aim) * speed, (float)-Math.Sin(_aim) * speed);
                move = checkMove(level, keybStates, oldpos);
            }
            speed = getSpeed(keybStates[1]); //Reset speed
            if (keybStates[1].IsKeyDown(Keys.W))
            {
                _pos += new Vector2((float)Math.Sin(_aim) * -speed, (float)Math.Cos(_aim) * -speed);
                move = checkMove(level, keybStates, oldpos);
            }
            else if (keybStates[1].IsKeyDown(Keys.S))
            {
                _pos += new Vector2((float)Math.Sin(_aim) * speed, (float)Math.Cos(_aim) * speed);
                move = checkMove(level, keybStates, oldpos);
            }
            if (move)
				stepSound(gameTime, keybStates[1].IsKeyDown(Keys.LeftShift) ? true : false);
        }

        //Attack & Cooldown
        public void updateCooldown(GameTime gameTime)
        {

            _cooldownTimer[0] -= gameTime.ElapsedGameTime;
            if (_cooldownTimer[0].TotalMilliseconds <= 0)
                    _cooldown = false;
        }
        public void useHands(List<Projectile> projectiles)
        {
            if (_hands == null || _hands.GetType() == typeof(Melee))
            {
                Melee weapon;

                if (_hands == null)
                    weapon = Engine.collection.MeleeList[0];
                else
                    weapon = (Melee)_hands;
                weapon.attack();
            }
            else if (_hands.GetType() == typeof(Firearm))
            {
                Firearm weapon = (Firearm)_hands;

                weapon.fire(projectiles, _pos, getAimAccuracyAngle(true));
            }
        }

        public void updateHands(GameTime gameTime, KeyboardState[] keybStates, MouseState[] mStates, List<Projectile> projectiles)
        {
            if (_cooldown)
                updateCooldown(gameTime);
            if (_inventoryOpen)
                return;
            if ((_hands != null && _hands.Type == ItemType.Firearm)
                && (keybStates[0].IsKeyUp(Keys.R) && keybStates[1].IsKeyDown(Keys.R)))
            {
                Firearm weapon = (Firearm)_hands;
                if (weapon.reload())
                {
                    _cooldown = true;
                    _cooldownTimer[1] = TimeSpan.FromMilliseconds(getCooldown() * 10);
                    _cooldownTimer[0] = _cooldownTimer[1];
                }
            }
            if (mStates[1].LeftButton == ButtonState.Pressed && mStates[0].LeftButton == ButtonState.Released && !_cooldown)
            {
                useHands(projectiles);
                _cooldown = true;
                _cooldownTimer[1] = TimeSpan.FromMilliseconds(getCooldown());
                _cooldownTimer[0] = _cooldownTimer[1];
            }
            if (mStates[1].RightButton == ButtonState.Pressed)
                _aimlock = true;
            else if (mStates[1].RightButton == ButtonState.Released)
                _aimlock = false;
        }

        //View & Lookup Update
        void updateAimAngle(Camera cam, MouseState[] mStates)
        {
			if (mStates[1].X != cam.getViewport().Width / 2)
			if (mStates[0].X < mStates[1].X)
				_aim += (mStates[0].X - mStates[1].X) * _aimSensivity;
			else if (mStates[0].X > mStates[1].X)
				_aim -= (mStates[1].X - mStates[0].X) * _aimSensivity;
            if (_aim < 0)
				_aim = MathHelper.TwoPi;
			else if (_aim > MathHelper.TwoPi)
				_aim = 0;
			cam.Rotation = _aim;
        }

        //Player states
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
            if (HP <= 0)
            {
                HP = 0;
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
                HP -= 1;
        }

        //Inventory & Item Management
        void toggleInventory(Cursor cursor)
        {
            _inventoryOpen = _inventoryOpen ? false : true;
            cursor.Show = _inventoryOpen;
        }

        public void pickupItem(Level level, int index)
        {
            if (_hands == null)
            {
                List<Item> entities = level.getEntities(level.vmapToGrid(_pos));
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
            if (_hands == null)
                return;
            List<Item> entities = level.getEntities(level.vmapToGrid(_pos));
            if (entities == null)
                entities = new List<Item>();
            if (slot == SlotType.Hands)
            {
                entities.Add(_hands);
                level.setEntities(level.vmapToGrid(_pos), entities);
                _hands = null;
            }

        }

        //Base update call
        public void Update(GameTime gameTime, Level level, KeyboardState[] keybStates, MouseState[] mStates, Camera cam, Cursor cursor, List<Projectile> projectiles)
        {
            if (!InventoryOpen)
                updateAimAngle(cam, mStates);
            updateMove(gameTime, keybStates, level);
            updateHands(gameTime, keybStates, mStates, projectiles);
            if (keybStates[1].IsKeyDown(Keys.Tab) && !keybStates[0].IsKeyDown(Keys.Tab))
                toggleInventory(cursor);
            if (keybStates[1].IsKeyDown(Keys.E) && !keybStates[0].IsKeyDown(Keys.E))
                pickupItem(level, 0);
            if (keybStates[1].IsKeyDown(Keys.G) && !keybStates[0].IsKeyDown(Keys.G))
                dropItem(level, SlotType.Hands, 0);
            updateStates(gameTime);
            if (_states.Count > 0 && _stateTimer.TotalSeconds >= 20)
                applyStates();
            cam.Pos = _pos;
        }

        //Base draw call
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_skin, _pos, _skinRect, Color.White, -_aim, _origin, 1.0f, SpriteEffects.None, 0);
        }
    }

}