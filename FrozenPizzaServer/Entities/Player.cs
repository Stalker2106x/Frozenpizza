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
        public int id { get; set; }
        public int hp { get; set; }
        public PointF pos { get; set; }
        public float orientation { get; set; }
        public bool alive { get; set; }

		public Player(int vid, PointF spawn)
        {
            id = vid;
            hp = 100;
			pos = spawn;
            alive = true;
        }

        public PointF calcFirePos()
        {
            return (pos + new SizeF((float)Math.Sin(orientation) * -17f, (float)Math.Cos(orientation) * -17f));
        }

        public Rectangle getHitbox()
        {
            return (new Rectangle(Point.Truncate(pos - new SizeF(8, 8)), new Size(16, 16)));
        }

        public void Update(TimeSpan delta)
        {
        }
    }
}
