using FrozenPizza.Network;
using FrozenPizza.Settings;
using FrozenPizza.Utils;
using FrozenPizza.World;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Server.Payloads;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FrozenPizza
{
  public class MainPlayer : Player
  {

    //Triggers
    public bool cooldown { get; set; }
    public bool inventoryOpen { get; set; }
    bool _sprinting, _aimlock;

    //Inventory
    public Item hands { get; set; }
    Inventory _inventory;

    //Timers
    TimeSpan _stepTimer;
    TimeSpan _stateTimer;
    TimeSpan[] _cooldownTimer;

    Utils.Timer _networkUpdateTimer;

    float _aimSensivity;


    public MainPlayer(int id, String name) : base(id, name, new Vector2(0, 0))
    {
      //Set triggers
      inventoryOpen = false;
      cooldown = false;
      _aimlock = false;
      _sprinting = false;

      //Set aim view
      _aimSensivity = 0.005f;

      //Init Inventory
      _inventory = new Inventory();

      //Init timers
      _stateTimer = new TimeSpan();
      _stepTimer = new TimeSpan();
      _cooldownTimer = new TimeSpan[2];

      _networkUpdateTimer = new Utils.Timer();
      _networkUpdateTimer.addAction(TimerDirection.Forward, 50, TimeoutBehaviour.StartOver, () => { UpdateNetwork(); });
      _networkUpdateTimer.Start();
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
      aimAccuracyAngle[0] += _orientation - MathHelper.PiOver2;
      aimAccuracyAngle[1] += _orientation - MathHelper.PiOver2;
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
    void updateMove(GameTime gameTime, DeviceState state, DeviceState prevState, Map map)
    {
      float speed = getSpeed();
      Vector2 movement = Vector2.Zero;

      if (Options.Config.Bindings[GameAction.Sprint].IsControlPressed(state, prevState)) _sprinting = true;
      else if (Options.Config.Bindings[GameAction.Sprint].IsControlReleased(state, prevState)) _sprinting = false;

      if (Options.Config.Bindings[GameAction.StrafeLeft].IsControlDown(state))
      {
        if (Options.Config.Bindings[GameAction.Forward].IsControlDown(state) || Options.Config.Bindings[GameAction.Backward].IsControlDown(state))
          speed *= 0.5f;
        movement = new Vector2((float)Math.Cos(_orientation) * -speed, (float)Math.Sin(_orientation) * speed);
      }
      if (Options.Config.Bindings[GameAction.StrafeRight].IsControlDown(state))
      {
        if (Options.Config.Bindings[GameAction.Forward].IsControlDown(state) || Options.Config.Bindings[GameAction.Backward].IsControlDown(state))
          speed *= 0.5f;
        movement += new Vector2((float)Math.Cos(_orientation) * speed, (float)-Math.Sin(_orientation) * speed);
      }

      speed = getSpeed(); //Reset speed
      if (Options.Config.Bindings[GameAction.Forward].IsControlDown(state))
        movement += new Vector2((float)Math.Sin(_orientation) * -speed, (float)Math.Cos(_orientation) * -speed);
      else if (Options.Config.Bindings[GameAction.Backward].IsControlDown(state))
        movement += new Vector2((float)Math.Sin(_orientation) * speed, (float)Math.Cos(_orientation) * speed);

      if (movement != Vector2.Zero)
      {
        _position += new Vector2((float)(movement.X * gameTime.ElapsedGameTime.TotalSeconds), (float)(movement.Y * gameTime.ElapsedGameTime.TotalSeconds));
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
        weapon.attack(_position);
      }
      else if (hands.GetType() == typeof(Firearm))
      {
        Firearm weapon = (Firearm)hands;

        weapon.fire(_position, getAimAccuracyAngleRelative());
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
          _orientation += (prevState.mouse.X - state.mouse.X) * _aimSensivity;
        else if (prevState.mouse.X > state.mouse.X)
          _orientation -= (state.mouse.X - prevState.mouse.X) * _aimSensivity;
      if (_orientation < 0)
        _orientation = MathHelper.TwoPi;
      else if (_orientation > MathHelper.TwoPi)
        _orientation = 0;
      if (cam.Rotation != _orientation)
      {
        cam.Rotation = _orientation;
      }
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
        //Item ent = GameMain.map.getEntityByPos(pos);

        //if (ent == null)
          return;
        //hands = ent;
        //NetHandler.send("!-ITEM " + ent.uid);
      }
    }

    public void dropItem(int uid)
    {
      if (hands == null)
        return;
      //NetHandler.send("!+ITEM " + hands.uid);
      hands = null;
    }

    public void UpdateNetwork()
    {
      NetDataWriter writer = new NetDataWriter();
      PlayerData payload = new PlayerData(_id, _position, _orientation);

      writer.Put(".PLAYER "+JsonConvert.SerializeObject(payload));
      Engine.networkClient.send(writer, DeliveryMethod.Unreliable);
    }

    //Base update call
    public void Update(GameTime gameTime, Map map, DeviceState state, DeviceState prevState, Camera cam, Cursor cursor)
    {
      if (!_active) return;
      if (!inventoryOpen) updateAimAngle(cam, state, prevState);
      updateMove(gameTime, state, prevState, map);
      updateHands(gameTime, state, prevState);
      if (Options.Config.Bindings[GameAction.ToggleInventory].IsControlPressed(state, prevState))
        toggleInventory(cursor);
      if (Options.Config.Bindings[GameAction.Use].IsControlPressed(state, prevState))
        pickupItem();
      if (Options.Config.Bindings[GameAction.Drop].IsControlPressed(state, prevState))
        dropItem(0);
      cam.Pos = _position;
      _networkUpdateTimer.Update(gameTime);
    }
  }

}