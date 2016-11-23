using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    
    public enum WearableSlot
    {
        HEAD,
        CHEST,
        HANDS,
        PANTS,
        FEETS
    }
    public abstract class Wearable : Item
    {
        int _condition, _def;
        WearableSlot _empl;
        public Wearable(String name, WearableSlot empl, int def, int condition, float weight, float size) : base(name, ItemType.WEARABLE, new List<SlotType> { SlotType.BAG, SlotType.OUTFIT }, weight, size)
        {
            _empl = empl;
            _def = def;
            _condition = condition;
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
