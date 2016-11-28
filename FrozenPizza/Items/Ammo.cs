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
        public ProjectileType Type { get; set; }

        Rectangle _skinRect;
        Vector2 _origin;


        public Projectile(ProjectileType type, Vector2 pos, float angle, float velocity)
        {
            Type = type;
            Pos = pos;
            Angle = angle;
            Velocity = velocity;
            _skinRect = new Rectangle(0, 0, 32, 32);
            _origin = new Vector2(16, 16);
        }

        public bool Update(Level level)
        {
            Pos += new Vector2((float)Math.Sin(Angle) * -Velocity, (float)Math.Cos(Angle) * -Velocity);
            if (level.Collide(Pos))
                return (false);
            return (true);
        }

        public void Draw(SpriteBatch spriteBatch, Collection collection)
        {
            spriteBatch.Draw(collection.Projectiles, Pos, _skinRect, Color.White, -Angle, _origin, 1.0f, SpriteEffects.None, 0);
        }
    }
    public class Ammo : Item
    {
        public Ammo(int id, String name, float weight, float size) : base(id, name, ItemType.Ammo, weight, size)
        {

        }
    }
}
