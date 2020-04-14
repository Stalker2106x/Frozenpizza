#if GAME
using Microsoft.Xna.Framework;
#else
  using System.Drawing;
  using System.Numerics;
#endif


namespace Server.Payloads
{
  public class ProjectileData
  {
    public int ownerId;
    public float x;
    public float y;
    public float angle;
    public float velocity;
    public int damage;

    public ProjectileData(int ownerId_, Vector2 position, float angle_, float velocity_, int damage_)
    {
      ownerId = ownerId_;
      x = position.X;
      y = position.Y;
      angle = angle_;
      velocity = velocity_;
      damage = damage_;
    }
  }
}
