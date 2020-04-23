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
  public enum Direction
  {
    Left,
    Right,
    Forward,
    Backward
  }

  public class Player : Actor
  {
    protected int _uid; //From server
    public int uid { get { return (_uid); } }

    //Constructor for remote players
    public Player(int uid, String name, Vector2 pos, int hp = 100) : base(name, hp, pos)
    {
      _uid = uid;
#if GAME
      _skinRect = new Rectangle(0, 0, 32, 16);
      _sounds = Resources.PlayersSound;
#endif
    }

  }
}
