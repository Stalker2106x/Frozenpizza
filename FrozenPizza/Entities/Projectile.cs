#if GAME
  using FrozenPizza.Utils;
#else
using FPServer;
  using Server;
  using Server.Payloads;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrozenPizza.Entities
{
  public class Projectile
  {
    public int ownerId;
    public Vector2 position;
    public float angle;
    public float velocity;
    public int damage;

    //Creates a projectile from network
    public Projectile(int ownerId_, Vector2 position_, float angle_, float velocity_, int damage_)
    {
      ownerId = ownerId_;
      position = position_;
      angle = angle_;
      velocity = velocity_;
      damage = damage_;
    }


    ///
    /// Game Logic
    ///
#if GAME
    public bool Update(GameTime gameTime)
    {
      position += new Vector2((float)(Math.Sin(angle) * -velocity * gameTime.ElapsedGameTime.TotalSeconds), (float)(Math.Cos(angle) * -velocity * gameTime.ElapsedGameTime.TotalSeconds));
      if (!GameMain.map.isValidPosition(position)) return (false);
      else if (GameMain.mainPlayer.getHitbox().Contains(position))
      {
        return (false);
      }
      foreach (var player in GameMain.players)
      {
        if (!player.active) continue;
        if (player.getHitbox().Contains(position))
        {
          return (false);
        }
      }
      return (true);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Resources.projectile, Tools.GetDrawPoint(position, Resources.projectile), new Rectangle(0, 0, Resources.projectile.Width, Resources.projectile.Height), Color.White, -angle, new Vector2(16, 16), 1.0f, SpriteEffects.None, 0.3f);
    }
#else
    public bool Update(ServerTime gameTime)
    {
      position += new Vector2((float)(Math.Sin(angle) * -velocity * gameTime.ElapsedGameTime.TotalSeconds), (float)(Math.Cos(angle) * -velocity * gameTime.ElapsedGameTime.TotalSeconds));
      if (!ServerV2.map.isValidPosition(position)) return (false);
      foreach (var entry in ServerV2.players)
      {
        Player player = entry.Value;
        if (!player.active) continue;
        if (player.getHitbox().Contains(new Point((int)position.X, (int)position.Y)))
        {
          player.addHealth(-damage);
          FullPlayerData payload = new FullPlayerData(player.name, player.active, player.hp, new PlayerData(player.uid, player.position, player.orientation));
          ServerSenderV2.SendFullPlayerData(payload);
          return (false);
        }
      }
      return (true);
    }
#endif
  }
}
