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
        Vector2 _origin;


        public Projectile(ProjectileType type, Vector2 pos, float angle, float velocity, int damage)
        {
            Type = type;
            Pos = pos;
            Angle = angle;
            Velocity = velocity;
            Damage = damage;
            _skinRect = new Rectangle(0, 0, 32, 32);
            _origin = new Vector2(16, 16);
        }

        public bool Update()
        {
            Pos += new Vector2((float)Math.Sin(Angle) * -Velocity, (float)Math.Cos(Angle) * -Velocity);
            return (true);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Engine.collection.Projectiles, Pos, _skinRect, Color.White, -Angle, _origin, 1.0f, SpriteEffects.None, 0);
        }
    }
    public class Ammo : Item
    {
        public Ammo(Int64 uid, int id, String name, float weight, float size) : base(uid, id, name, ItemType.Ammo, weight, size)
        {

        }
    }
}
