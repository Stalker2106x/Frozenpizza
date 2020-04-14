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
using System.Linq;
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

  public static class Orientation
  {
    public static float North = (float)Math.PI / 2;
    public static float NorthEast = (float)Math.PI / 4;
    public static float East = 0;
    public static float SouthEast = (float)(7 * Math.PI) / 4;
    public static float South = (float)(3 * Math.PI) / 2;
    public static float SouthWest = (float)(5 * Math.PI) / 4;
    public static float West = (float)Math.PI;
    public static float NorthWest = (float)( 3 * Math.PI) / 4;
    public static float MaxAngle = NorthWest - North;
  }

  public class AccuracyAngle
  {
    public float left;
    public float right;

    public AccuracyAngle()
    {
      left = GameMain.mainPlayer.orientation + MathHelper.PiOver4;
      right = GameMain.mainPlayer.orientation - MathHelper.PiOver4;
    }

    public void EnforceComparison()
    {
      if (left < right) right -= MathHelper.TwoPi;
    }
    public void EnforceConsistency()
    {
      if (left < 0) left += MathHelper.TwoPi;
      else if (left > MathHelper.TwoPi) left -= MathHelper.TwoPi;
      if (right < 0) right += MathHelper.TwoPi;
      else if (right > MathHelper.TwoPi) right -= MathHelper.TwoPi;
    }

    public bool Contains(float value)
    {
      if (right > left)
      {
        return ((value <= left && value >= 0) || (value <= MathHelper.TwoPi && value >= right));
      }
      else
      {
        return ((value <= left && value >= right));
      }
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

    Utils.Timer _networkUpdateTimer;



    public MainPlayer(int id, String name, Vector2 position) : base(id, name, position)
    {
      //Set triggers
      _aimlock = false;
      _sprinting = false;

      //Init Inventory
      hands = new MeleeWeapon();
      hands.Copy(Collection.MeleeList.First((it) => { return (it.id == "hands"); }));
      hands.Init();
      _inventory = new Inventory();
      
      _networkUpdateTimer = new Utils.Timer();
      _networkUpdateTimer.addAction(TimerDirection.Forward, 10 , TimeoutBehaviour.StartOver, () => { UpdateNetwork(); });
      _networkUpdateTimer.Start();
    }

    //Returns the two angles of aim calculating weapon accuracy (0 is left, 1 is right)
    public AccuracyAngle getAimAccuracyAngle(bool relative = false)
    {
      AccuracyAngle aimAccuracyAngle = new AccuracyAngle();
      float fov = (float)Math.PI / 4f; //half view

      FireWeapon weapon;
      if ((weapon = hands as FireWeapon) != null)
      {
        fov -= (weapon.accuracy / 100) * Orientation.MaxAngle;
      }
      if (_sprinting) fov += 0.15f;
      else if (_aimlock) fov -= 0.05f;

      aimAccuracyAngle.left = fov;
      aimAccuracyAngle.right = -fov;
      if (relative)
      {
        aimAccuracyAngle.left += (_orientation);
        aimAccuracyAngle.right += (_orientation);
      }
      aimAccuracyAngle.EnforceConsistency();
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

    public override void addHealth(int value)
    {
      base.addHealth(value);
      GameMain.hud.updatePlayer(_hp, 0);
    }

    //Make player drop item on death & closes inventory
    public override void die()
    {
      base.die();
      dropItem(0);
      GameMain.hud.toggleDeathPanel();
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
      Vector2 normPosition = new Vector2(((_position.X + (GameMain.map.tileSize.X / 2)) % GameMain.map.tileSize.X),
                                         ((_position.Y + (GameMain.map.tileSize.Y / 2)) % GameMain.map.tileSize.Y));
      Rectangle tileBounds = new Rectangle(1, 1, GameMain.map.tileSize.X - 1, GameMain.map.tileSize.Y - 1);
      CheckResult check = new CheckResult(movement);

      if (axis == Axis.Horizontal)
      {
        if (normPosition.X + check.vector.X > tileBounds.Width) check.vector.X = 0;
        else if (normPosition.X + check.vector.X < tileBounds.X) check.vector.X = 0;
      }
      else if (axis == Axis.Vertical)
      {
        if (normPosition.Y + check.vector.Y > tileBounds.Height) check.vector.Y = 0;
        else if (normPosition.Y + check.vector.Y < tileBounds.Y) check.vector.Y = 0;
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
      if (state.mouse.X != Options.Config.Width / 2)
      {
        if (prevState.mouse.X < state.mouse.X)
          _orientation += (prevState.mouse.X - state.mouse.X) * (Options.Config.MouseSensivity / 100);
        else if (prevState.mouse.X > state.mouse.X)
          _orientation -= (state.mouse.X - prevState.mouse.X) * (Options.Config.MouseSensivity / 100);
      }
      if (_orientation < 0) _orientation += MathHelper.TwoPi;
      else if (_orientation > MathHelper.TwoPi) _orientation -= MathHelper.TwoPi;
      cam.Rotation = _orientation;
    }

    public void interact()
    {
      if (hands.id == "hands") //Pickup
      {
        Point gridPos = GameMain.map.WorldToGrid(_position);
        var item = GameMain.map.items.FirstOrDefault((it) => { return (it.position == gridPos); });
        if (item != null)
        {
          GameMain.map.items.Remove(item);
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
        ClientSenderV2.SendItemPickup(new ItemData(hands.uid, hands.position));
        hands.position = GameMain.map.WorldToGrid(_position);
        hands.drop();
        GameMain.map.items.Add(hands);
        hands = new MeleeWeapon();
        hands.Copy(Collection.MeleeList.First((it) => { return (it.id == "hands"); }));
        hands.Init();
        GameMain.hud.initHands(hands);
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

    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);
      hands.Draw(spriteBatch, this);
    }
  }

}