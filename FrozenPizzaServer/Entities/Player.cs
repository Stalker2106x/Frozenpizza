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
        public int HP { get; set; }
        public PointF Pos { get; set; }
        public PointF Origin { get; set; }
        public float Aim { get; set; }
        public bool Alive { get; set; }

		public Player(PointF spawn)
        {
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
            return (new Rectangle(Point.Truncate(Pos - new SizeF(8, 8)), new Size(Point.Truncate(new PointF(16, 16)))));
        }
    }
}
