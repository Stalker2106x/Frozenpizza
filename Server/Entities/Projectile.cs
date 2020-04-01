using System;
using System.Drawing;
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
        public PointF Pos { get; set; }
        public int Damage { get; }
        public float Angle { get; }
        public float Velocity { get; }

        public Projectile(ProjectileType type, PointF pos, float angle, float velocity, int damage)
        {
            Type = type;
            Pos = pos;
            Angle = angle;
            Velocity = velocity;
            Damage = damage;
        }

        public bool Update()
        {
            Pos += new SizeF((float)Math.Sin(Angle) * -Velocity, (float)Math.Cos(Angle) * -Velocity);
            for (int i = 0; i < Server.ClientList.Count; i++)
            {
                if (Server.ClientList[i] == null || Server.ClientList[i].Player == null)
                    continue;
                if (Server.ClientList[i].Player.getHitbox().Contains(Point.Truncate(Pos)))
                {
                    Server.ClientList[i].send("!HIT " + Server.ClientList[i].Id + " " + Damage);
                    Server.ClientList[i].Player.hp -= Damage;
                    return (false);
                }
                else if (Server.Level.Collide(Pos))
                    return (false);
            }
            return (true);
        }
    }
}
