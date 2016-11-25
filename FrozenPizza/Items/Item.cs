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

	public class Item
	{
		public int Id { get; set; }
		public String Name { get; set; }
		public ItemType Type { get; set; }
		public List<SlotType> Slots { get; set; }
		public float Weight { get; set; }
		public float Size { get; set; }

		public Item(int id, String name, ItemType type, float weight, float size)
		{
			Id = id;
			Name = name;
			Type = type;
			Weight = weight;
			Size = size;
		}
	}
}