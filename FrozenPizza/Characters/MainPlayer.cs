using FrozenPizza.Settings;
using FrozenPizza.Utils;
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
    public Gauge hunger { get; set; }
    public Gauge thirst { get; set; }

    //Triggers
    public bool cooldown { get; set; }
    public bool inventoryOpen { get; set; }
    bool _sprinting, _aimlock;

    //Inventory
    public Item hands { get; set; }
    Inventory _inventory;
    List<PlayerStates> _states;


    //Timers
    TimeSpan _stepTimer;
    TimeSpan _stateTimer;
    TimeSpan[] _cooldownTimer;

    float _aimSensivity;


    public MainPlayer(String name) : base(-1, name, new Vector2(0, 0))
    {
      //Set Stats
      hunger = new Gauge(50, 0, 100);
      thirst = new Gauge(50, 0, 100);
      pos = Vector2.Zero;

      _states = new List<PlayerStates>();
      //Set triggers
      inventoryOpen = false;
      cooldown = false;
      _aimlock = false;
      _sprinting = false;

      //Set aim view
      orientation = 0;
      _aimSensivity = 0.005f;

      //Init Inventory
      _inventory = new Inventory();

      //Init timers
      _stateTimer = new TimeSpan();
      _stepTimer = new TimeSpan();
      _cooldownTimer = new TimeSpan[2];
    }


    //Converts Cooldown time to percent for the bar
    public int getCooldownPercent(int width)
    {
      float elapsed = (float)(_cooldownTimer[1].TotalMilliseconds - _cooldownTimer[0].TotalMilliseconds);

      return ((int)((float)(elapsed / _cooldownTimer[1].TotalMilliseconds) * width));
    }

    //Get the 'use' (or fire) cooldown from currently held item
    public float getCooldown()
    {
      Weapon whands = (Weapon)hands;
      if (whands != null)
        return ((float)TimeSpan.FromSeconds(whands.Cooldown).TotalMilliseconds);
      else
        return ((float)TimeSpan.FromSeconds(Collection.MeleeList[0].Cooldown).TotalMilliseconds);
    }

    //Placeholder
    public int getArmor()
    {
      return (0);
    }

    //Placeholder
    public float getWeight()
    {
      float weight = 0;

      return (weight);
    }

    public float[] getAimAccuracyAngleRelative()
    {
      float[] aimAccuracyAngle = getAimAccuracyAngle();
      aimAccuracyAngle[0] += orientation - MathHelper.PiOver2;
      aimAccuracyAngle[1] += orientation - MathHelper.PiOver2;
      return (aimAccuracyAngle);
    }

    //Returns the two angles of aim calculating weapon accuracy (0 is left, 1 is right)
    public float[] getAimAccuracyAngle()
    {
      float[] aimAccuracyAngle = new float[2];


      aimAccuracyAngle[0] = MathHelper.PiOver2 * 1.5f;
      aimAccuracyAngle[1] = MathHelper.PiOver2 * 0.5f;
      if (hands != null && hands.type == ItemType.Firearm)
      {
        Firearm weapon = (Firearm)hands;

        aimAccuracyAngle[0] -= (float)(Math.PI / 180) * weapon.Accuracy;
        aimAccuracyAngle[1] += (float)(Math.PI / 180) * weapon.Accuracy;
      }
      if (_sprinting)
      {
        aimAccuracyAngle[0] += 0.15f;
        aimAccuracyAngle[1] -= 0.15f;
      }
      else
      {
        if (_aimlock)
        {
          aimAccuracyAngle[0] -= 0.05f;
          aimAccuracyAngle[1] += 0.05f;
        }
      }
      return (aimAccuracyAngle);
    }

    //Returns speed accordint to weight and sprint states
    public float getSpeed(KeyboardState keybState)
    {
      float speed = 120f;

      if (_sprinting)
        speed += 30f;
      return (speed);
    }

    //Make player drop item on death & closes inventory
    public override void die()
    {
      base.die();
      dropItem(0);
      inventoryOpen = false;
    }

    //Plays if needed the correct stepsound
    void stepSound(GameTime gameTime, bool run)
    {
      _stepTimer += gameTime.ElapsedGameTime;
      if (run && _stepTimer >= _sounds[(int)PlayerSounds.RunStep1].Duration)
      {
        Random rnd = new Random();

        _sounds[rnd.Next(4, 8)].Play(Options.Config.SoundVolume, 0f, 0f);
        _stepTimer = TimeSpan.Zero;
      }
      else if (!run && _stepTimer >= _sounds[(int)PlayerSounds.Step1].Duration)
      {
        Random rnd = new Random();

        _sounds[rnd.Next(0, 4)].Play(Options.Config.SoundVolume, 0f, 0f);
        _stepTimer = TimeSpan.Zero;
      }
    }

    //check wether the user is sprinting or not [DEPRECATED]
    void setSprinting(KeyboardState[] keybStates)
    {
      if (!_sprinting && keybStates[1].IsKeyDown(KeyBinds.getKey("Sprint")))
        _sprinting = true;
    }

    //Code to move the player
    void updateMove(GameTime gameTime, KeyboardState[] keybStates, Level level)
    {
      float speed = getSpeed(keybStates[1]);
      Vector2 movement = Vector2.Zero;

      if (keybStates[1].IsKeyUp(KeyBinds.getKey("Sprint")))
        _sprinting = false;
      if (keybStates[1].IsKeyDown(KeyBinds.getKey("StrafeLeft")))
      {
        if (keybStates[1].IsKeyDown(KeyBinds.getKey("Forward")) || keybStates[1].IsKeyDown(KeyBinds.getKey("Backward")))
          speed *= 0.5f;
        movement = new Vector2((float)Math.Cos(orientation) * -speed, (float)Math.Sin(orientation) * speed);
      }
      else if (keybStates[1].IsKeyDown(KeyBinds.getKey("StrafeRight")))
      {
        if (keybStates[1].IsKeyDown(KeyBinds.getKey("Forward")) || keybStates[1].IsKeyDown(KeyBinds.getKey("Backward")))
          speed *= 0.5f;
        movement += new Vector2((float)Math.Cos(orientation) * speed, (float)-Math.Sin(orientation) * speed);
      }

      speed = getSpeed(keybStates[1]); //Reset speed
      if (keybStates[1].IsKeyDown(KeyBinds.getKey("Forward")))
        movement += new Vector2((float)Math.Sin(orientation) * -speed, (float)Math.Cos(orientation) * -speed);
      else if (keybStates[1].IsKeyDown(KeyBinds.getKey("Backward")))
        movement += new Vector2((float)Math.Sin(orientation) * speed, (float)Math.Cos(orientation) * speed);

      if (movement != Vector2.Zero)
      {
        Vector2 syncVector = new Vector2((float)(movement.X * gameTime.ElapsedGameTime.TotalSeconds), (float)(movement.Y * gameTime.ElapsedGameTime.TotalSeconds));
        Rectangle newhit = getHitbox();

        setSprinting(keybStates);
        newhit.X += (int)syncVector.X;
        newhit.Y += (int)syncVector.Y;
        if (GameMain.level.RCollide(newhit))
        {
          return;
        }
        pos += syncVector;
        stepSound(gameTime, _sprinting);
      }
    }

    //Attack & Cooldown
    public void updateCooldown(GameTime gameTime)
    {

      _cooldownTimer[0] -= gameTime.ElapsedGameTime;
      if (_cooldownTimer[0].TotalMilliseconds <= 0)
        cooldown = false;
    }

    //Use Hands
    public void useHands()
    {
      if (hands == null || hands.GetType() == typeof(Melee))
      {
        Melee weapon;

        if (hands == null)
          weapon = Collection.MeleeList[0];
        else
          weapon = (Melee)hands;
        weapon.attack(pos);
      }
      else if (hands.GetType() == typeof(Firearm))
      {
        Firearm weapon = (Firearm)hands;

        weapon.fire(pos, getAimAccuracyAngleRelative());
      }
    }

    //Check Hands events (Reload, fire, ...)
    public void updateHands(GameTime gameTime, KeyboardState[] keybStates, MouseState[] mStates)
    {
      if (cooldown)
        updateCooldown(gameTime);
      if (inventoryOpen)
        return;
      if ((hands != null && hands.type == ItemType.Firearm)
          && (keybStates[0].IsKeyUp(KeyBinds.getKey("Reload")) && keybStates[1].IsKeyDown(KeyBinds.getKey("Reload"))))
      {
        Firearm weapon = (Firearm)hands;
        if (weapon.reload())
        {
          cooldown = true;
          _cooldownTimer[1] = TimeSpan.FromSeconds(weapon.ReloadCooldown);
          _cooldownTimer[0] = _cooldownTimer[1];
        }
      }
      if (mStates[1].LeftButton == ButtonState.Pressed && mStates[0].LeftButton == ButtonState.Released && !cooldown)
      {
        useHands();
        cooldown = true;
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
          orientation += (mStates[0].X - mStates[1].X) * _aimSensivity;
        else if (mStates[0].X > mStates[1].X)
          orientation -= (mStates[1].X - mStates[0].X) * _aimSensivity;
      if (orientation < 0)
        orientation = MathHelper.TwoPi;
      else if (orientation > MathHelper.TwoPi)
        orientation = 0;
      if (cam.Rotation != orientation)
      {
        cam.Rotation = orientation;
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
      if (hunger.get() > 0 && hunger.get() < 50 && !checkState(PlayerStates.Hungry))
        _states.Add(PlayerStates.Hungry);
      else if (hunger.get() == hunger.min && !checkState(PlayerStates.Starving))
        _states.Add(PlayerStates.Starving);
      if (thirst.get() > 0 && thirst.get() < 50 && !checkState(PlayerStates.Thirsty))
        _states.Add(PlayerStates.Thirsty);
      else if (thirst.get() <= thirst.min && !checkState(PlayerStates.Dehydrated))
        _states.Add(PlayerStates.Dehydrated);
    }

    //Used to hurt player
    void applyStates()
    {
      _stateTimer = TimeSpan.Zero;
      if (checkState(PlayerStates.Starving))
        hp.set(hp.get() - 1);
    }

    //Inventory & Item Management
    void toggleInventory(Cursor cursor)
    {
      inventoryOpen = !inventoryOpen;
      cursor.Show = inventoryOpen;
    }

    public void pickupItem()
    {
      if (hands == null)
      {
        Item ent = GameMain.level.getEntityByPos(pos);

        if (ent == null)
          return;
        hands = ent;
        NetHandler.send("!-ITEM " + ent.uid);
      }
    }

    public void dropItem(int uid)
    {
      if (hands == null)
        return;
      NetHandler.send("!+ITEM " + hands.uid);
      hands = null;
    }

    public void updateNetwork(GameTime gameTime)
    {
      NetHandler.send("!STATE " + id + " " + pos.X + " " + pos.Y + " " + orientation);
    }

    //Base update call
    public void Update(GameTime gameTime, Level level, KeyboardState[] keybStates, MouseState[] mStates, Camera cam, Cursor cursor)
    {
      if (!alive) return;
      if (!inventoryOpen) updateAimAngle(cam, mStates);
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
      cam.Pos = pos;
    }

    //Base draw call
    public override void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Collection.Players, pos, _skinRect, Color.White, -orientation, new Vector2(16, 8), 1.0f, SpriteEffects.None, 0.3f);
    }
  }

}