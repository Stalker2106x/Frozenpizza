using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FrozenPizza
{
    public enum PlayerStates
    {
        Hungry,
        Thirsty,
        Starving,
        Dehydrated
    }


    public class MainPlayer : Player
    {
        //Stats
        int _hunger, _thirst;
        int _maxHunger, _maxThirst;

        //Triggers
        bool _inventoryOpen, _cooldown, _sprinting, _aimlock;

        //Inventory
        Item _hands;
        Inventory _inventory;
        List<PlayerStates> _states;


        //Timers
		TimeSpan _stepTimer;
        TimeSpan _stateTimer;
        TimeSpan[] _cooldownTimer;

        //Net Timer
        TimeSpan _netcodeTimer;

        //Netcode
        Vector2 _netLastPos;
        float _netLastAim;

        float _aimSensivity;

		public MainPlayer(String name) : base(-1, name, new Vector2(0, 0))
        {
            //Set Stats
            _hunger = 50;
            _maxHunger = 100;
            _thirst = 25;
            _maxThirst = 100;
			Pos = Vector2.Zero;

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
            _inventory = new Inventory();

            //Init timers
            _stateTimer = new TimeSpan();
            _stepTimer = new TimeSpan();
            _cooldownTimer = new TimeSpan[2];
            _netcodeTimer = new TimeSpan();
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

                aimAccuracyAngle[0] -= (float)(Math.PI / 180) * weapon.Accuracy;
                aimAccuracyAngle[1] += (float)(Math.PI / 180) * weapon.Accuracy;
            }
            if (_sprinting)
			{
				aimAccuracyAngle[0] += 0.15f;
				aimAccuracyAngle[1] -= 0.15f;
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
        public override void die()
        {
            base.die();
            dropItem(0);
            _inventoryOpen = false;
        }
        void stepSound(GameTime gameTime, bool run)
		{
			_stepTimer += gameTime.ElapsedGameTime;
			if (run && _stepTimer >= _sounds[(int)PlayerSounds.RunStep1].Duration)
			{
				Random rnd = new Random();

				_sounds[rnd.Next(4, 8)].Play(0.5f, 0f, 0f);
				_stepTimer = TimeSpan.Zero;
			}
			else if (!run && _stepTimer >= _sounds[(int)PlayerSounds.Step1].Duration)
			{
				Random rnd = new Random();

				_sounds[rnd.Next(0, 4)].Play(0.5f, 0f, 0f);
                _stepTimer = TimeSpan.Zero;
			}
        }

        //Moving mechanism functions
        bool checkSprint(KeyboardState[] keybStates)
        {
            if (keybStates[1].IsKeyDown(Keys.LeftShift))
            {
                _sprinting = true;
                return (true);
            }
            return (false);
        }

        void updateMove(GameTime gameTime, KeyboardState[] keybStates, Level level)
        {
			bool move = false;
			float speed = getSpeed(keybStates[1]);

            if (keybStates[1].IsKeyUp(KeyBinds.getKey("Sprint")))
                _sprinting = false;
            if (keybStates[1].IsKeyDown(KeyBinds.getKey("StrafeLeft")))
            {
                if (keybStates[1].IsKeyDown(KeyBinds.getKey("Forward")) || keybStates[1].IsKeyDown(KeyBinds.getKey("Backward")))
                    speed *= 0.5f;
                _pos += new Vector2((float)Math.Cos(_aim) * -speed, (float)Math.Sin(_aim) * speed);
                checkSprint(keybStates);
                move = true;
            }
            else if (keybStates[1].IsKeyDown(KeyBinds.getKey("StrafeRight")))
            {
                if (keybStates[1].IsKeyDown(KeyBinds.getKey("Forward")) || keybStates[1].IsKeyDown(KeyBinds.getKey("Backward")))
                    speed *= 0.5f;
                _pos += new Vector2((float)Math.Cos(_aim) * speed, (float)-Math.Sin(_aim) * speed);
                checkSprint(keybStates);
                move = true;
            }
            speed = getSpeed(keybStates[1]); //Reset speed
            if (keybStates[1].IsKeyDown(KeyBinds.getKey("Forward")))
            {
                _pos += new Vector2((float)Math.Sin(_aim) * -speed, (float)Math.Cos(_aim) * -speed);
                checkSprint(keybStates);
                move = true;
            }
            else if (keybStates[1].IsKeyDown(KeyBinds.getKey("Backward")))
            {
                _pos += new Vector2((float)Math.Sin(_aim) * speed, (float)Math.Cos(_aim) * speed);
                checkSprint(keybStates);
                move = true;
            }
            if (move)
            {
                stepSound(gameTime, _sprinting);
            }
        }

        //Attack & Cooldown
        public void updateCooldown(GameTime gameTime)
        {

            _cooldownTimer[0] -= gameTime.ElapsedGameTime;
            if (_cooldownTimer[0].TotalMilliseconds <= 0)
                    _cooldown = false;
        }

        public void useHands()
        {
            if (_hands == null || _hands.GetType() == typeof(Melee))
            {
                Melee weapon;

                if (_hands == null)
                    weapon = Engine.collection.MeleeList[0];
                else
                    weapon = (Melee)_hands;
                weapon.attack(_pos);
            }
            else if (_hands.GetType() == typeof(Firearm))
            {
                Firearm weapon = (Firearm)_hands;

                weapon.fire(_pos, getAimAccuracyAngle(true));
            }
        }

        public void updateHands(GameTime gameTime, KeyboardState[] keybStates, MouseState[] mStates)
        {
            if (_cooldown)
                updateCooldown(gameTime);
            if (_inventoryOpen)
                return;
            if ((_hands != null && _hands.Type == ItemType.Firearm)
                && (keybStates[0].IsKeyUp(KeyBinds.getKey("Reload")) && keybStates[1].IsKeyDown(KeyBinds.getKey("Reload"))))
            {
                Firearm weapon = (Firearm)_hands;
                if (weapon.reload())
                {
                    _cooldown = true;
                    _cooldownTimer[1] = TimeSpan.FromSeconds(weapon.ReloadCooldown);
                    _cooldownTimer[0] = _cooldownTimer[1];
                }
            }
            if (mStates[1].LeftButton == ButtonState.Pressed && mStates[0].LeftButton == ButtonState.Released && !_cooldown)
            {
				useHands();
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
            if (cam.Rotation != _aim)
            {
                cam.Rotation = _aim;
            }
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

        public void pickupItem()
        {
            if (_hands == null)
            {
				Item ent = Engine.Level.getEntityByPos(_pos);

				if (ent == null)
					return;
				_hands = ent;
				NetHandler.send("!-ITEM " + ent.Uid);
            }
        }

        public void dropItem(int uid)
        {
            if (_hands == null)
                return;
			NetHandler.send("!+ITEM " + _hands.Uid);
			_hands = null;
        }

        public void updateNetwork(GameTime gameTime)
        {
            _netcodeTimer -= gameTime.ElapsedGameTime;
            if (_netcodeTimer.TotalMilliseconds <= 0)
            {
                _netcodeTimer = TimeSpan.FromMilliseconds(10);
                if (_pos != _netLastPos)
                {
                    NetHandler.send("?MOVE " + _pos.X + " " + _pos.Y);
                    _netLastPos = _pos;
                }
                if (_aim != _netLastAim)
                {
                    NetHandler.send("!AIM " + _aim);
                    _netLastAim = _aim;
                }
            }
        }

        //Base update call
        public void Update(GameTime gameTime, Level level, KeyboardState[] keybStates, MouseState[] mStates, Camera cam, Cursor cursor)
        {
            if (!_alive)
                return;
            if (!_inventoryOpen)
                updateAimAngle(cam, mStates);
            updateMove(gameTime, keybStates, level);
            updateHands(gameTime, keybStates, mStates);
            updateNetwork(gameTime);
            if (keybStates[1].IsKeyDown(KeyBinds.getKey("ToggleInventory")) && !keybStates[0].IsKeyDown(KeyBinds.getKey("ToggleInventory")))
                toggleInventory(cursor);
            if (keybStates[1].IsKeyDown(KeyBinds.getKey("Pickup")) && !keybStates[0].IsKeyDown(KeyBinds.getKey("Pickup")))
                pickupItem();
            if (keybStates[1].IsKeyDown(KeyBinds.getKey("Drop")) && !keybStates[0].IsKeyDown(KeyBinds.getKey("Drop")))
                dropItem(0);
            updateStates(gameTime);
            if (_states.Count > 0 && _stateTimer.TotalSeconds >= 20)
                applyStates();
            cam.Pos = _pos;
        }

        //Base draw call
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Engine.collection.Players, _pos, _skinRect, Color.White, -_aim, _origin, 1.0f, SpriteEffects.None, 0);
        }
    }

}