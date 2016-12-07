using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{
    public struct Vector2
    {
        public float X;
        public float Y;

		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}
    }
    public class Player
    {
        public Vector2 Pos { get; set; }

		public Player(Vector2 spawn)
        {
			Pos = spawn;
        }
    }
}
