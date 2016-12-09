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
        public static Vector2 operator -(Vector2 vec, Vector2 vec2)
        {
            return (new Vector2(vec.X - vec2.X, vec.Y - vec2.Y));
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

        public Rectangle(Vector2 origin, Vector2 end)
        {
            Origin = origin;
            Size = end;
        }

        public bool Contains(Vector2 pos)
        {
            if ((pos.X >= Origin.X && pos.X <= (Origin.X + Size.X))
                && (pos.Y >= Origin.Y && pos.Y <= (Origin.Y + Size.Y)))
                    return (true);
            return (false);
        }
    }
    public class Player
    {
        public Vector2 Pos { get; set; }
        public Vector2 Origin { get; set; }
        public float Aim { get; set; }

		public Player(Vector2 spawn)
        {
			Pos = spawn;
            Origin = new Vector2(12, 12);
        }

        public Rectangle getHitbox()
        {
            return (new Rectangle(Pos - Origin, Origin + Origin));
        }
    }
}
