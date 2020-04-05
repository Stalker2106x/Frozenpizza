using FrozenPizza.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace FrozenPizza
{
  public enum PlayerSounds
  {
    Step1,
    Step2,
    Step3,
    Step4,
    RunStep1,
    RunStep2,
    RunStep3,
    RunStep4,
    Hurt,
    Die
  }

  public class Player : BasePlayer
  {

    //Sound
    protected SoundEffect[] _sounds;

    //Constructor for remote players
    public Player(int id, String name, Vector2 pos, int hp = 100) : base(id, name, hp, pos)
    {
      _skinRect = new Rectangle(0, 0, 32, 16);
      _sounds = Collection.PlayersSound;
    }

    //Reports damage from server to player
    public virtual void addHealth(int value)
    {
      _hp -= value;
      _sounds[(int)PlayerSounds.Hurt].Play(Options.Config.SoundVolume, 0f, 0f);
      if (_hp <= 0) die();
    }

    //Reports death from server to player
    public virtual void die()
    {
      _sounds[(int)PlayerSounds.Die].Play(Options.Config.SoundVolume, 0f, 0f);
      _active = false;
      _skinRect = new Rectangle(0, 64, 32, 64);
    }
    public virtual void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Collection.Players, _position, _skinRect, Color.White, -_orientation, new Vector2(16, 8), 1.0f, SpriteEffects.None, 0.3f);
    }
  }
}
