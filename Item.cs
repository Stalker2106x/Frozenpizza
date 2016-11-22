using System;

namespace FrozenPizza
{
    enum ItemType
    {
        Food,
        Weapon,
        Wearable,
        Misc
    }

    enum SlotType
    {
        Head,
        Chest,
        Legs,
        Hands,
        Back,
        Bag
    }

    public class Item
    {
        String _name;
        ItemType _type;
        List<SlotType> _slots;
        unsigned int _weight, _size;

        public Item(String name, ItemType type, List<SlotType> slots, uint weight, uint size)
        {
            _name = name;
            _type = type;
            _slots = slots;
            _weight = weight;
            _size = size;
        }
    }

    public class Food : Item
    {
        public Food(String name, uint weight, uint size)
        {
            Item(name, Food, List < SlotType >{ Bag };, weight, size);
        }
    }

    public class Weapon : Item
    {
        unsigned int _condition;
        unsigned int _damage;
        public Weapon(String name, uint damage, List<SlotType> slots, uint weight, uint size)
        {
            Item(name, Weapon, slots, weight, size);
            _damage = damage;
        }

        public uint getDamage()
        {
            return (_damage * (_condition / 100));
        }

        public bool isBroken()
        {
            return (_condition >= 50 ? false : true);
        }
    }

    public class Wearable : Item
    {
        unsigned int _condition, _def;
        public Wearable(String name, SlotType slot, uint def, uint weight, uint size)
        {
            Item(name, Wearable, List < SlotType >{ slot };, weight, size);
            _def = def;
        }

        public uint getArmour()
        {
            return (_def * (_condition / 100));
        }
        public bool isBroken()
        {
            return (_condition >= 50 ? false : true);
        }
    }
}