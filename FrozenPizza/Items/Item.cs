using Microsoft.Xna.Framework;
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
        Ammo,
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
		public Int64 Uid { get; set; }
		public int Id { get; set; }
		public Vector2 Pos { get; set; }
		public String Name { get; set; }
		public ItemType Type { get; set; }
		public List<SlotType> Slots { get; set; }
		public float Weight { get; set; }
		public float Size { get; set; }
        public Rectangle SkinRect { get; set; }
        public Vector2 Origin { get; set; }

        public Item(Int64 uid, int id, String name, ItemType type, float weight, float size)
		{
            Uid = uid;
			Id = id;
			Name = name;
			Type = type;
			Weight = weight;
			Size = size;
            setSkin();
		}

        public void use()
        {

        }

        //Set correct skinRect with itemId
        public virtual void setSkin()
        {
            int id = Id;

            switch(Type)
            {
                case ItemType.Firearm:
                    id -= (int)ItemIds.Pistol;
                    break;
            }
            SkinRect = new Rectangle(id * 32, 0, 32, 32);
            Origin = new Vector2(16, 16);
        }
	}
}