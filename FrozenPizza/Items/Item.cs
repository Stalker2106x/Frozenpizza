using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FrozenPizza
{
    public enum ItemType
    {
		Consumable,
		Melee,
		Firearm,
		Wearable,
		Misc
    }

    public enum SlotType
    {
        Hands,
		Bag,
		Outfit,
		Back
    }

    public abstract class Item
    {
        String _name;
        ItemType _type;
        List<SlotType> _slots;
        float _weight, _size;
        Texture2D _skin;
		int _resourceID;

		public Item()
		{
        }

		public abstract bool Load(String name);

        public bool slotCompatible(SlotType dest)
        {
            for (int i = 0; i < _slots.Count; i++)
                if (_slots[i] == dest)
                    return (true);
            return (false);
        }

		public List<SlotType> Slots
		{
			get { return (_slots); }
			set { _slots = value; }
		}

		public String Name
		{
			get { return (_name); }
			set { _name = value; }
		}

        public float Weight
        {
			get { return (_weight); }
			set { _weight = value; }
        }

		public float Size
		{
			get { return (_size); }
			set { _size = value; }
		}

		public int ResourceID
		{
			get { return (_resourceID); }
			set { _weight = value; }
		}

        public float getSize()
        {
            return (_size);
        }
    }

    public abstract class Consumable : Item
    {
	public Consumable()
	{

    }

		public override abstract bool Load(String name);
    }

    public abstract class Weapon : Item
    {
        uint _condition;
        uint _damage;

		public Weapon()
		{
        }

		public override abstract bool Load(string name);

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