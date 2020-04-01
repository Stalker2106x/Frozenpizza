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
		public Int64 uid { get; set; }
		public int id { get; set; }
		public Vector2 pos { get; set; }
		public String name { get; set; }
		public ItemType type { get; set; }
		public List<SlotType> slots { get; set; }
		public float weight { get; set; }
		public float size { get; set; }
        public Rectangle skinRect { get; set; }
        public Vector2 origin { get; set; }

        public Item(Int64 vuid, int vid, String vname, ItemType vtype, float vweight, float vsize)
		{
            uid = vuid;
            id = vid;
            name = vname;
            type = vtype;
            weight = vweight;
            size = vsize;
            setSkin();
		}

    public Item Copy()
    {
      return (Item)this.MemberwiseClone();
    }

    public void use()
        {

        }

        //Set correct skinRect with itemId
        public virtual void setSkin()
        {
            int rId = id;

            switch(type)
            {
                case ItemType.Firearm:
                    rId -= (int)ItemIds.Pistol;
                    break;
            }
            skinRect = new Rectangle(rId * 32, 0, 32, 32);
            origin = new Vector2(16, 16);
        }
	}
}