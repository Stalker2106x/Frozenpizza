using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    
    public enum WearableSlot
    {
        Head,
		Chest,
		Hands,
		Pants,
		Feets
    }
    public abstract class Wearable : Item
    {
        int _condition, _def;
        WearableSlot _empl;

        public Wearable()
        {
			
        }

		public abstract override bool Load(string name);

        public int getArmor()
        {
            return (_def * (_condition / 100));
        }
        public bool isBroken()
        {
            return (_condition >= 50 ? false : true);
        }
    }
}
