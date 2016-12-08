using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{
    public class Projectile
    {
        public Vector2 Pos { get; set; }
        public int Damage { get; }
        public float Angle { get; }
        public float Velocity { get; }

        public Projectile(Vector2 pos, float angle, float velocity, int damage)
        {
            Pos = pos;
            Angle = angle;
            Velocity = velocity;
            Damage = damage;
        }

        public void Update()
        {
            Pos += new Vector2((float)Math.Cos(Angle) * -Velocity, (float)Math.Sin(Angle) * Velocity);
        }
    }
}
