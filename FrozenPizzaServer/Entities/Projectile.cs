using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{
    public enum ProjectileType
    {
        Bullet,
        Arrow
    }
    public class Projectile
    {
        ProjectileType Type { get; set; }
        public Vector2 Pos { get; set; }
        public int Damage { get; }
        public float Angle { get; }
        public float Velocity { get; }

        public Projectile(ProjectileType type, Vector2 pos, float angle, float velocity, int damage)
        {
            Type = type;
            Pos = pos;
            Angle = angle;
            Velocity = velocity;
            Damage = damage;
        }

        public bool Update()
        {
            Pos += new Vector2((float)Math.Cos(Angle) * -Velocity, (float)Math.Sin(Angle) * Velocity);
            for (int i = 0; i < Server.ClientList.Count; i++)
            {
                if (Server.ClientList[i].Player.getHitbox().Contains(Pos))
                {
                    //give damage!!
                    return (false);
                }
            }
            return (true);
        }
    }
}
