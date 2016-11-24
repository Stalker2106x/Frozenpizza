using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class Melee : Weapon
    {
        public Melee()
        {
            
        }

		public override bool Load(String name)
		{
			return (true);
		}
    }

    public class Firearm : Weapon
    {
		public Firearm()
        {
            
        }

		public override bool Load(String name)
		{
			return (true);
		}
    }
}
