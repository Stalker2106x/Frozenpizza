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
    }

    public class Pistol : Weapon
    {
        public Pistol(String name, uint damage, uint condition, List<SlotType> slots, uint weight, uint size) : base(name, damage, condition, slots, weight, size)
        {
            
        }
    }
}
