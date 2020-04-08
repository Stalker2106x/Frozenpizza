#if GAME
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;
  using System;
#else
  using System.Drawing;
  using System.Numerics;
#endif

namespace FrozenPizza.Entities
{
  public class Projectile
  {
    public Vector2 position;
    public float angle;
    public float velocity;
    public int damage;

    //Creates a projectile from network
    public Projectile(Vector2 position_, float angle_, float velocity_, int damage_)
    {
      position = position_;
      angle = angle_;
      velocity = velocity_;
      damage = damage_;
    }

    //Update locally
    public bool Update(GameTime gameTime)
    {
      position += new Vector2((float)(Math.Sin(angle) * -velocity * gameTime.ElapsedGameTime.TotalSeconds), (float)(Math.Cos(angle) * -velocity * gameTime.ElapsedGameTime.TotalSeconds));
      if (!GameMain.map.isValidPosition(position)) return (false);
      else if (GameMain.mainPlayer.getHitbox().Contains(position))
        return (false);
      for (int i = 0; i < GameMain.players.Count; i++)
      {
        if (GameMain.players[i].getHitbox().Contains(position))
          return (false);
      }
      return (true);
    }

    //Draw projectile
    public void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Collection.projectile, position, new Rectangle(0, 0, 15, 20), Color.White, -angle, new Vector2(16, 16), 1.0f, SpriteEffects.None, 0.3f);
    }
  }
}
