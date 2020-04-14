#if GAME
using FrozenPizza.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
#else
  using System.Drawing;
  using System.Numerics;
#endif
using System;

namespace FrozenPizza
{
  public class BasePlayer
  {
    //Stats
    protected int _id; //From server
    public int id { get { return (_id); } }

    protected string _name;
    public string name { get { return (_name); } }

    protected int _hp;
    public int hp { get { return (_hp); } set { _hp = value; } }

    protected bool _active;
    public bool active { get { return (_active); } set { _active = value; } }

    //Movement
    protected Vector2 _position;
    public Vector2 position { get { return (_position); } set { _position = value; } }

    protected float _orientation;
    public float orientation { get { return (_orientation); } set { _orientation = value; } }

    //Graphics
    protected Rectangle _skinRect;

#if GAME
    //Sound
    protected SoundEffect[] _sounds;
#endif

    public BasePlayer(int id, string name_, int hp, Vector2 position)
    {
      _active = true;
      _id = id;
      _name = name_;
      _hp = hp;
      _position = position;
#if GAME
      _orientation = 0;
#endif
      _skinRect = new Rectangle(0, 0, 32, 16);
    }

    public void Reset()
    {
      _skinRect = new Rectangle(0, 0, 32, 16);
    }

    public Rectangle getHitbox()
    {
      return (new Rectangle((int)_position.X - 8, (int)_position.Y - 8, 16, 16));
    }

    //Reports damage from server to player
    public virtual void addHealth(int value)
    {
      if (!_active) return;
      _hp += value;
#if GAME
      _sounds[(int)PlayerSounds.Hurt].Play(Options.Config.SoundVolume, 0f, 0f);
#endif
      if (_hp <= 0) die();
    }

    //Reports death from server to player
    public virtual void die()
    {
#if GAME
      _sounds[(int)PlayerSounds.Die].Play(Options.Config.SoundVolume, 0f, 0f);
#endif
      _active = false;
      _skinRect = new Rectangle(0, 64, 32, 64);
    }

    public float getDistanceTo(Vector2 pos)
    {
      return ((float)Math.Sqrt(Math.Pow(pos.X - _position.X, 2) + Math.Pow(pos.Y - _position.Y, 2)));
    }

  }
}
