#if GAME
  using FrozenPizza.Settings;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrozenPizza
{
  public class Actor
  {
    //Stats
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

    //Sound
    protected SoundEffect[] _sounds;

    public Actor(string name_, int hp, Vector2 position)
    {
      _active = true;
      _name = name_;
      _hp = hp;
      _position = position;
      _orientation = 0;
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

    /// <summary>
    /// Game Logic
    /// </summary>
#if GAME
    public virtual void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Resources.Players, _position, _skinRect, Color.White, -_orientation, new Vector2(_skinRect.Width / 2, _skinRect.Height / 2), 1.0f, SpriteEffects.None, 0.3f);
    }
#endif

  }
}
