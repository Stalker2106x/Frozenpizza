using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FrozenPizza
{
	public class Collection
	{
		public List<Melee> MeleeList { get; }
		public List<Firearm> FirearmList { get; }
		public Texture2D[] Tilesets { get; set; }

        public Collection()
		{
			MeleeList = new List<Melee>();
            FirearmList = new List<Firearm>();
			Tilesets = new Texture2D[Enum.GetNames(typeof(ItemType)).Length];
		}

		public bool Load(ContentManager content)
		{
			LoadMelee(content);
            LoadFirearm(content);
			return (true);
		}

		public bool LoadMelee(ContentManager content)
		{
			XElement bundle = XElement.Load("Data/items/melee.xml");

			Tilesets[(int)ItemType.Melee] = content.Load<Texture2D>("gfx/melee");
			foreach (var item in bundle.Elements("Item"))
			{
				MeleeList.Add(new Melee((int)item.Element("Id"), item.Element("Name").Value, (float)item.Element("Weight"), (float)item.Element("Size")));
				MeleeList.Last().SetWeaponAttributes((int)item.Element("Damage"), (float)item.Element("Cooldown"));
				//MeleeList.Last().SetMeleeAttributes();
			}
			return (true);
		}

        public bool LoadFirearm(ContentManager content)
        {
            XElement bundle = XElement.Load("Data/items/firearms.xml");

			Tilesets[(int)ItemType.Firearm] = content.Load<Texture2D>("gfx/firearms");
            foreach (var item in bundle.Elements("Item"))
            {
                FirearmList.Add(new Firearm((int)item.Element("Id"), item.Element("Name").Value, (float)item.Element("Weight"), (float)item.Element("Size")));
				FirearmList.Last().SetWeaponAttributes((int)item.Element("Damage"), (float)item.Element("Cooldown"));
				FirearmList.Last().SetFirearmAttributes((int)item.Element("Accuracy"));
            }
            return (true);
        }
    }
}
