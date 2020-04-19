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

    public Vector2 getDirectionVector(Direction direction, float move)
    {
      return (getDirectionVector(direction, _orientation, move));
    }

    public static Vector2 getDirectionVector(Direction direction, float angle, float move)
    {
      if (direction == Direction.Left) return (new Vector2((float)Math.Cos(angle) * -move, (float)Math.Sin(angle) * move));
      if (direction == Direction.Right) return (new Vector2((float)Math.Cos(angle) * move, (float)-Math.Sin(angle) * move));
      if (direction == Direction.Forward) return (new Vector2((float)Math.Sin(angle) * -move, (float)Math.Cos(angle) * -move));
      if (direction == Direction.Backward) return (new Vector2((float)Math.Sin(angle) * move, (float)Math.Cos(angle) * move));
      return (Vector2.Zero);
    }

  }
}
