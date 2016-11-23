using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class Melee : Weapon
    {
        public Melee(String name, uint damage, uint condition, List<SlotType> slots, uint weight, uint size) : base(name, damage, condition, slots, weight, size)
        {
            
        }

		public override bool Load(String name)
		{
			return (true);
		}
    }

    public class Gun : Weapon
    {
        public Gun(String name, uint damage, uint condition, List<SlotType> slots, uint weight, uint size) : base(name, damage, condition, slots, weight, size)
        {
            
        }

		public override bool Load(String name)
		{
			return (true);
		}
    }
}
