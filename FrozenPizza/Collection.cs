using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FrozenPizza
{
    public enum ItemIds
    {
        Melee = 0,
        Pistol = 1000,
        Rifle = 2000,
        Consumable = 3000,
        Wearable = 4000
    }
	public class Collection
	{
        public Texture2D GameLogo { get; set; }
        public SoundEffect[] MenuSounds { get; set; }
		public List<Melee> MeleeList { get; }
		public List<Firearm> PistolsList { get; }
		public Texture2D[] Tilesets { get; set; }
        public Texture2D Projectiles { get; set; }
        public Texture2D Players { get; set; }

        public Collection()
		{
            MenuSounds = new SoundEffect[2];
            //Game
			MeleeList = new List<Melee>();
            PistolsList = new List<Firearm>();
			Tilesets = new Texture2D[Enum.GetNames(typeof(ItemType)).Length];
		}

		public bool Load(ContentManager content)
		{
            GameLogo = content.Load<Texture2D>("gfx/logo");
            MenuSounds[0] = content.Load<SoundEffect>("sounds/menu/hover");
            MenuSounds[1] = content.Load<SoundEffect>("sounds/menu/click");
            //Game
            LoadMelee(content);
            LoadPistols(content);
            LoadPlayers(content);
            LoadProjectiles(content);
			return (true);
		}

        public Item getItemById(int id)
        {
            if (id < (int)ItemIds.Pistol) //Melee
                return (MeleeList[id]);
            else if (id < (int)ItemIds.Rifle) //Pistol
                return (PistolsList[id - (int)ItemIds.Pistol]);
            return (null);
        }

		public bool LoadMelee(ContentManager content)
		{
			XElement bundle = XElement.Load("Data/items/melee.xml");

			Tilesets[(int)ItemType.Melee] = content.Load<Texture2D>("gfx/melee");
			foreach (var item in bundle.Elements("Item"))
			{
				MeleeList.Add(new Melee((int)item.Element("Id"), item.Element("Name").Value, (float)item.Element("Weight"), (float)item.Element("Size")));
				MeleeList.Last().SetWeaponAttributes(item.Element("ResourceId").Value.ToString(), (int)item.Element("Damage"), (float)item.Element("Cooldown"));
                MeleeList.Last().LoadSounds(content);
            }
            return (true);
		}

        public bool LoadPistols(ContentManager content)
        {
            XElement bundle = XElement.Load("Data/items/firearms.xml");

			Tilesets[(int)ItemType.Firearm] = content.Load<Texture2D>("gfx/firearms");
            foreach (var item in bundle.Elements("Item"))
            {
                PistolsList.Add(new Firearm((int)item.Element("Id"), item.Element("Name").Value, (float)item.Element("Weight"), (float)item.Element("Size")));
                PistolsList.Last().SetWeaponAttributes(item.Element("ResourceId").Value.ToString(), (int)item.Element("Damage"), (float)item.Element("Cooldown"));
                PistolsList.Last().SetFirearmAttributes((int)item.Element("Accuracy"), (int)item.Element("ClipSize"));
                PistolsList.Last().LoadSounds(content);
            }
            return (true);
        }
        public bool LoadPlayers(ContentManager content)
        {
            Players = content.Load<Texture2D>("gfx/players");
            return (true);
        }
        public bool LoadProjectiles(ContentManager content)
        {
            Projectiles = content.Load<Texture2D>("gfx/projectiles");
            return (true);
        }
    }
}
