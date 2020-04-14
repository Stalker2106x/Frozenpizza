#if GAME
  using Microsoft.Xna.Framework;
#else
  using System.Drawing;
  using System.Numerics;
#endif
namespace Server.Payloads
{
  public class MeleeHitData
  {
    public int ownerId;
    public float x;
    public float y;
    public int damage;

    public MeleeHitData(int ownerId_, Vector2 position, int damage_)
    {
      ownerId = ownerId_;
      x = position.X;
      y = position.Y;
      damage = damage_;
    }
  }
}
