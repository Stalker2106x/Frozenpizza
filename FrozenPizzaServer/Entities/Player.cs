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

        public static Vector2 operator +(Vector2 vec, Vector2 vec2)
        {
            return (new Vector2(vec.X + vec2.X, vec.Y + vec2.Y));
        }
    }

    public struct Rectangle
    {
        public Vector2 Origin;
        public Vector2 Size;

        public Rectangle(float x, float y, float width, float height)
        {
            Origin = new Vector2(x, y);
            Size = new Vector2(width, height);
        }
    }
    public class Player
    {
        public Vector2 Pos { get; set; }
        public float Aim { get; set; }

		public Player(Vector2 spawn)
        {
			Pos = spawn;
        }
    }
}
