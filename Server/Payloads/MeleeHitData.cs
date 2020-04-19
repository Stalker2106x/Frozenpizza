using Microsoft.Xna.Framework;

namespace Server.Payloads
{
  public class MeleeHitData
  {
    public int ownerId;
    public Vector2 position;
    public int damage;

    public MeleeHitData(int ownerId_, Vector2 position_, int damage_)
    {
      ownerId = ownerId_;
      position = position_;
      damage = damage_;
    }
  }
}
