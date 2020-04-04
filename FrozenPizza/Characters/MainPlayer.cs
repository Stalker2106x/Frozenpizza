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
    public float getSpeed()
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

    //Code to move the player
    void updateMove(GameTime gameTime, DeviceState state, DeviceState prevState, Level level)
    {
      float speed = getSpeed();
      Vector2 movement = Vector2.Zero;

      if (Options.Config.Bindings[GameAction.Sprint].IsControlUp(state))
        _sprinting = false;
      if (Options.Config.Bindings[GameAction.StrafeLeft].IsControlDown(state))
      {
        if (Options.Config.Bindings[GameAction.Forward].IsControlDown(state) || Options.Config.Bindings[GameAction.Backward].IsControlDown(state))
          speed *= 0.5f;
        movement = new Vector2((float)Math.Cos(orientation) * -speed, (float)Math.Sin(orientation) * speed);
      }
      if (Options.Config.Bindings[GameAction.StrafeRight].IsControlDown(state))
      {
        if (Options.Config.Bindings[GameAction.Forward].IsControlDown(state) || Options.Config.Bindings[GameAction.Backward].IsControlDown(state))
          speed *= 0.5f;
        movement += new Vector2((float)Math.Cos(orientation) * speed, (float)-Math.Sin(orientation) * speed);
      }

      speed = getSpeed(); //Reset speed
      if (Options.Config.Bindings[GameAction.Forward].IsControlDown(state))
        movement += new Vector2((float)Math.Sin(orientation) * -speed, (float)Math.Cos(orientation) * -speed);
      else if (Options.Config.Bindings[GameAction.Backward].IsControlDown(state))
        movement += new Vector2((float)Math.Sin(orientation) * speed, (float)Math.Cos(orientation) * speed);

      if (movement != Vector2.Zero)
      {
        Vector2 syncVector = new Vector2((float)(movement.X * gameTime.ElapsedGameTime.TotalSeconds), (float)(movement.Y * gameTime.ElapsedGameTime.TotalSeconds));
        Rectangle newhit = getHitbox();

        if (!_sprinting && Options.Config.Bindings[GameAction.Sprint].IsControlPressed(state, prevState))
          _sprinting = true;
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
    public void updateHands(GameTime gameTime, DeviceState state, DeviceState prevState)
    {
      if (cooldown)
        updateCooldown(gameTime);
      if (inventoryOpen)
        return;
      if ((hands != null && hands.type == ItemType.Firearm)
         && Options.Config.Bindings[GameAction.Reload].IsControlPressed(state, prevState))
      {
        Firearm weapon = (Firearm)hands;
        if (weapon.reload())
        {
          cooldown = true;
          _cooldownTimer[1] = TimeSpan.FromSeconds(weapon.ReloadCooldown);
          _cooldownTimer[0] = _cooldownTimer[1];
        }
      }
      if (Options.Config.Bindings[GameAction.Fire].IsControlDown(state) && !cooldown)
      {
        useHands();
        cooldown = true;
        _cooldownTimer[1] = TimeSpan.FromMilliseconds(getCooldown());
        _cooldownTimer[0] = _cooldownTimer[1];
      }
      if (Options.Config.Bindings[GameAction.Aim].IsControlDown(state))
        _aimlock = true;
      else
        _aimlock = false;
    }

    //View & Lookup Update
    void updateAimAngle(Camera cam, DeviceState state, DeviceState prevState)
    {
      if (state.mouse.X != cam.getViewport().Width / 2)
        if (prevState.mouse.X < state.mouse.X)
          orientation += (prevState.mouse.X - state.mouse.X) * _aimSensivity;
        else if (prevState.mouse.X > state.mouse.X)
          orientation -= (state.mouse.X - prevState.mouse.X) * _aimSensivity;
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
    public void Update(GameTime gameTime, Level level, DeviceState state, DeviceState prevState, Camera cam, Cursor cursor)
    {
      if (!alive) return;
      if (!inventoryOpen) updateAimAngle(cam, state, prevState);
      updateMove(gameTime, state, prevState, level);
      updateHands(gameTime, state, prevState);
      updateNetwork(gameTime);
      if (Options.Config.Bindings[GameAction.ToggleInventory].IsControlPressed(state, prevState))
        toggleInventory(cursor);
      if (Options.Config.Bindings[GameAction.Use].IsControlPressed(state, prevState))
        pickupItem();
      if (Options.Config.Bindings[GameAction.Drop].IsControlPressed(state, prevState))
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