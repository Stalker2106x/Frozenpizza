using Microsoft.Xna.Framework;


namespace Server.Payloads
{
  public class ProjectileData
  {
    public int ownerId;
    public Vector2 position;
    public float angle;
    public float velocity;
    public int damage;

    public ProjectileData(int ownerId_, Vector2 position_, float angle_, float velocity_, int damage_)
    {
      ownerId = ownerId_;
      position = position_;
      angle = angle_;
      velocity = velocity_;
      damage = damage_;
    }
  }
}
