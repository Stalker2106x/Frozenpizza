﻿using FrozenPizza.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
  public enum ProjectileType
  {
    Bullet,
    Arrow
  }
  public class Projectile
  {
    Vector2 Pos { get; set; }
    float Angle { get; set; }
    float Velocity { get; set; }
    int Damage { get; set; }
    public ProjectileType Type { get; set; }

    Rectangle _skinRect;

    //Creates a projectile from network
    public Projectile(int id, ProjectileType type, Vector2 pos, float angle, float velocity, int damage)
    {
      Type = type;
      Pos = pos;
      Angle = angle;
      Velocity = velocity;
      Damage = damage;
      Weapon weapon = (Weapon)Engine.collection.getItemById(id);
      if (Engine.mainPlayer.getDistanceTo(pos) <= (Engine.Level.Map.TileWidth * 40))
      {
        float distVolume = (Engine.mainPlayer.getDistanceTo(pos) / (Engine.Level.Map.TileWidth * 40));

        distVolume = (Options.Config.SoundVolume - distVolume) / Options.Config.SoundVolume;
        weapon.Sounds[(int)FirearmActions.Fire].Play(distVolume > 0 ? distVolume : 0, 0f, 0f);
      }
      _skinRect = new Rectangle(0, 0, 32, 32);
    }

    //Update locally
    public bool Update()
    {
      Pos += new Vector2((float)Math.Sin(Angle) * -Velocity, (float)Math.Cos(Angle) * -Velocity);
      if (Pos.X < 0 || Pos.Y < 0
          || (Pos.X >= Engine.Level.Map.Width * Engine.Level.Map.TileWidth)
          || (Pos.X >= Engine.Level.Map.Height * Engine.Level.Map.TileHeight))
        return (false);
      else if (Engine.Level.Collide(Pos))
        return (false);
      else if (Engine.mainPlayer.getHitbox().Contains(Pos))
        return (false);
      for (int i = 0; i < Engine.Players.Count; i++)
      {
        if (Engine.Players[i].getHitbox().Contains(Pos))
          return (false);
      }
      return (true);
    }

    //Draw projectile
    public void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Engine.collection.Projectiles, Pos, _skinRect, Color.White, -Angle, new Vector2(16, 16), 1.0f, SpriteEffects.None, 0.3f);
    }
  }

  public class Ammo : Item
  {
    public Ammo(Int64 uid, int id, String name, float weight, float size) : base(uid, id, name, ItemType.Ammo, weight, size)
    {

    }
  }
}
