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
        public PointF Pos { get; set; }
        public PointF Origin { get; set; }
        public float Aim { get; set; }

		public Player(PointF spawn)
        {
			Pos = spawn;
            Origin = new PointF(12, 12);
        }

        public PointF calcFirePos()
        {
            return (Pos + new SizeF((float)Math.Sin(Aim) * -15, (float)Math.Cos(Aim) * -15));
        }

        public Rectangle getHitbox()
        {
            return (new Rectangle(Point.Truncate(Pos - new SizeF(8, 8)), new Size(Point.Truncate(Pos + new SizeF(8, 8)))));
        }
    }
}
