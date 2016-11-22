using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FrozenPizza
{
    public enum ItemType
    {
        FOOD,
        WEAPON,
        WEARABLE,
        MISC
    }

    public enum SlotType
    {
        HANDS,
        BACK,
        OUTFIT,
        BAG
    }

    public class Item
    {
        String _name;
        ItemType _type;
        List<SlotType> _slots;
        float _weight, _size;
        Texture2D _skin;
        Vector2 _pos;

        public Item(String name, ItemType type, List<SlotType> slots, float weight, float size)
        {
            _name = name;
            _type = type;
            _slots = slots;
            _weight = weight;
            _size = size;
        }

        public bool slotCompatible(SlotType dest)
        {
            for (int i = 0; i < _slots.Count; i++)
                if (_slots[i] == dest)
                    return (true);
            return (false);
        }

        public float getWeight()
        {
            return (_weight);
        }

        public float getSize()
        {
            return (_size);
        }
    }

    public class Food : Item
    {
        uint _value;
        public Food(String name, uint value, List<SlotType> slots, float weight, float size) : base(name, ItemType.FOOD, new List<SlotType> { SlotType.BAG }, weight, size)
        {
            _value = value;
        }
    }

    public class Weapon : Item
    {
        uint _condition;
        uint _damage;
        public Weapon(String name, uint damage, uint condition, List<SlotType> slots, float weight, float size) : base(name, ItemType.WEAPON, slots, weight, size)
        {
            _damage = damage;
            _condition = condition;
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

}