using FrozenPizza.Entities;
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

  public enum Axis
  {
    Horizontal,
    Vertical
  }

  public class CheckResult
  {
    public bool result;
    public Vector2 vector;
    public CheckResult(Vector2 movement)
    {
      vector = movement;
    }
  }

  public class AccuracyAngle
  {
    public float min;
    public float max;

    public void add(float value)
    {
      min -= value;
      max += value;
    }
  }

  public class MainPlayer : Player
  {
    //Triggers
    public bool cooldown;
    bool _sprinting, _aimlock;

    //Inventory
    public BaseItem hands;
    Inventory _inventory;

    //Timers
    TimeSpan _stepTimer;
    TimeSpan[] _cooldownTimer;

    Utils.Timer _networkUpdateTimer;

    float _aimSensivity;


    public MainPlayer(int id, String name) : base(id, name, new Vector2(1440, 1088))
    {
      //Set triggers
      cooldown = false;
      _aimlock = false;
      _sprinting = false;

      //Set aim view
      _aimSensivity = 0.005f;

      //Init Inventory
      hands = Collection.MeleeList.Find((it) => { return (it.id == "hands"); }).Copy();
      _inventory = new Inventory();

      //Init timers
      _stepTimer = new TimeSpan();
      _cooldownTimer = new TimeSpan[2];

      _networkUpdateTimer = new Utils.Timer();
      _networkUpdateTimer.addAction(TimerDirection.Forward, 10 , TimeoutBehaviour.StartOver, () => { UpdateNetwork(); });
      _networkUpdateTimer.Start();
    }


    //Converts Cooldown time to percent for the bar
    public int getCooldownPercent(int width)
    {
      float elapsed = (float)(_cooldownTimer[1].TotalMilliseconds - _cooldownTimer[0].TotalMilliseconds);

      return ((int)((float)(elapsed / _cooldownTimer[1].TotalMilliseconds) * width));
    }

    public AccuracyAngle getAimAccuracyAngleRelative()
    {
      AccuracyAngle aimAccuracyAngle = getAimAccuracyAngle();
      aimAccuracyAngle.add(_orientation - MathHelper.PiOver2);
      return (aimAccuracyAngle);
    }

    //Returns the two angles of aim calculating weapon accuracy (0 is left, 1 is right)
    public AccuracyAngle getAimAccuracyAngle()
    {
      AccuracyAngle aimAccuracyAngle = new AccuracyAngle();


      aimAccuracyAngle.max = MathHelper.PiOver2 * 1.5f;
      aimAccuracyAngle.min = MathHelper.PiOver2 * 0.5f;
      FireWeapon weapon;
      if ((weapon = hands as FireWeapon) != null) aimAccuracyAngle.add(-(float)(Math.PI / 180f) * weapon.accuracy);
      if (_sprinting) aimAccuracyAngle.add(0.15f);
      else if (_aimlock) aimAccuracyAngle.add(-0.05f);
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

    public CheckResult checkOverflow(Axis axis, Vector2 movement)
    {
      Vector2 normPosition = new Vector2((float)(_position.X % 32), (float)(_position.Y % 32));
      CheckResult check = new CheckResult(movement);

      if (axis == Axis.Horizontal)
      {
        if (normPosition.X + check.vector.X > 31f) check.vector.X = 0;
        else if (normPosition.X + check.vector.X < 1f) check.vector.X = 0;
      }
      else if (axis == Axis.Vertical)
      {
        if (normPosition.Y + check.vector.Y > 31f) check.vector.Y = 0;
        else if (normPosition.Y + check.vector.Y < 1f) check.vector.Y = 0;
      }
      check.result = GameMain.map.isValidPosition(_position + check.vector);
      return (check);
    }

    //Code to move the player
    void updateMove(GameTime gameTime, DeviceState state, DeviceState prevState, Map map)
    {
      float speed = getSpeed();
      Vector2 movement = Vector2.Zero;

      if (Options.Config.Bindings[GameAction.Sprint].IsControlPressed(state, prevState)) _sprinting = true;
      else if (Options.Config.Bindings[GameAction.Sprint].IsControlReleased(state, prevState)) _sprinting = false;

      if (Options.Config.Bindings[GameAction.StrafeLeft].IsControlDown(state)) movement += getDirectionVector(Direction.Left, speed * 1.5f);
      if (Options.Config.Bindings[GameAction.StrafeRight].IsControlDown(state)) movement += getDirectionVector(Direction.Right, speed * 1.5f);
      if (Options.Config.Bindings[GameAction.Forward].IsControlDown(state)) movement += getDirectionVector(Direction.Forward, speed);
      if (Options.Config.Bindings[GameAction.Backward].IsControlDown(state)) movement += getDirectionVector(Direction.Backward, speed);

      speed = getSpeed(); //Reset speed
      if (Options.Config.Bindings[GameAction.Forward].IsControlDown(state))
        movement += new Vector2((float)Math.Sin(_orientation) * -speed, (float)Math.Cos(_orientation) * -speed);
      else if (Options.Config.Bindings[GameAction.Backward].IsControlDown(state))
        movement += new Vector2((float)Math.Sin(_orientation) * speed, (float)Math.Cos(_orientation) * speed);

      if (movement != Vector2.Zero)
      {
        Vector2 candidate = new Vector2((float)(movement.X * gameTime.ElapsedGameTime.TotalSeconds), (float)(movement.Y * gameTime.ElapsedGameTime.TotalSeconds));
        if (!GameMain.map.isValidPosition(_position + candidate))
        {
          CheckResult hres = checkOverflow(Axis.Horizontal, candidate);
          CheckResult vres = checkOverflow(Axis.Vertical, candidate);

          if (!hres.result && !vres.result) candidate = Vector2.Zero;
          if (hres.result) candidate = hres.vector;
          if (vres.result) candidate = vres.vector;
        }
        _position += candidate;
        stepSound(gameTime, _sprinting);
      }
    }

    //Use Hands
    public void useHands(GameAction type)
    {
      switch (type)
      {
        case GameAction.UseHands:
          hands.use(this);
          break;
        case GameAction.Reload:
          FireWeapon weapon;
          if ((weapon = hands as FireWeapon) != null) weapon.reload();
          break;
        default:
          break;
      }
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

    public void interact()
    {
      if (hands.id == "hands") //Pickup
      {
        Point gridPos = GameMain.map.WorldToGrid(_position);
        var item = GameMain.map.items.Find((it) => { return (it.position == gridPos); });
        if (item != null)
        {
          item.position = null;
          ClientSenderV2.SendItemPickup(new ItemData(item.uid, item.position));
          hands = item;
          GameMain.hud.initHands(hands);
        }
      }
    }

    public void dropItem(int uid)
    {
      if (hands.id != "hands") //drop
      {
        var item = GameMain.map.items.Find((it) => { return (it.uid == hands.uid); });
        if (item != null)
        {
          Point gridPos = GameMain.map.WorldToGrid(_position);
          item.position = gridPos;
          ClientSenderV2.SendItemPickup(new ItemData(item.uid, item.position));
          hands = Collection.MeleeList.Find((it) => { return (it.id == "hands"); }).Copy();
        }
      }
    }

    public void UpdateNetwork()
    {
      ClientSenderV2.SendPlayerData(new PlayerData(_id, _position, _orientation));
    }

    //Base update call
    public void Update(GameTime gameTime, Map map, DeviceState state, DeviceState prevState, Camera cam, Cursor cursor)
    {
      if (!_active) return;
      if (!GameMain.hud.overlayActive) updateAimAngle(cam, state, prevState);
      hands.Update(gameTime);
      updateMove(gameTime, state, prevState, map);
      if (Options.Config.Bindings[GameAction.Aim].IsControlPressed(state, prevState)) _aimlock = true;
      if (Options.Config.Bindings[GameAction.Aim].IsControlReleased(state, prevState)) _aimlock = false;
      if (Options.Config.Bindings[GameAction.Interact].IsControlPressed(state, prevState)) interact();
      if (Options.Config.Bindings[GameAction.Drop].IsControlPressed(state, prevState)) dropItem(0);
      if (Options.Config.Bindings[GameAction.UseHands].IsControlDown(state) && !cooldown) useHands(GameAction.UseHands);
      if (Options.Config.Bindings[GameAction.Reload].IsControlPressed(state, prevState) && !cooldown) useHands(GameAction.Reload);
      cam.Pos = _position;
      _networkUpdateTimer.Update(gameTime);
    }
  }

}