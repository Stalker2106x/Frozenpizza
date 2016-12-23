using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{

    public class Player
    {
        public int Id { get; set; }
        public int HP { get; set; }
        public PointF Pos { get; set; }
        public PointF Origin { get; set; }
        public PointF Move { get; set; }
        public float Aim { get; set; }
        public bool Alive { get; set; }

		public Player(int id, PointF spawn)
        {
            Id = id;
            HP = 100;
			Pos = spawn;
            Origin = new PointF(12, 12);
            Alive = true;
        }

        public PointF calcFirePos()
        {
            return (Pos + new SizeF((float)Math.Sin(Aim) * -17f, (float)Math.Cos(Aim) * -17f));
        }

        public Rectangle getHitbox()
        {
            return (new Rectangle(Point.Truncate(Pos - new SizeF(8, 8)), new Size(16, 16)));
        }

        public void Update(TimeSpan delta)
        {
            PointF validPos = Pos;

            if (HP <= 0 && Alive)
            {
                Alive = false;
                Server.broadcast(-1, "!DIE " + Id);
            }
            if (Move.X != 0 || Move.Y != 0)
            {
                PointF syncVector = new PointF((float)(Move.X * delta.TotalSeconds), (float)(Move.Y * delta.TotalSeconds));

                Pos = new PointF(Pos.X + syncVector.X, Pos.Y + syncVector.Y);
            }
            if (Server.Level.RCollide(getHitbox()))
            {
                Move = new PointF(0, 0);
                Pos = validPos;
                Server.broadcast(-1, "!MOVE " + Id + " " + Move.X + " " + Move.Y);
                Server.broadcast(-1, "!SETPOS " + Id + " " + validPos.X + " " + validPos.Y);
            }
        }
    }
}
