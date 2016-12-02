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
    }
    public class Player
    {
        public int Id { get; set; }
        public Vector2 Pos { get; set; }

        public Player()
        {

        }
    }
}
