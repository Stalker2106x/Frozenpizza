using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace FrozenPizza
{
    public class Weapon : Item
    {
        public int Damage { get; set; }
        public int Durability { get; set; }
        public float Cooldown { get; set; }


        public Weapon(int id, String name, ItemType type, float weight, float size) : base(id, name, type, weight, size)
		{

		}

		public void SetWeaponAttributes(int damage, float cooldown)
		{
			Random rnd = new Random();

			Damage = damage;
			Cooldown = cooldown;
			Durability = rnd.Next(20, 101);
		}
	}

	public class Melee : Weapon
	{
		public Melee(int id, String name, float weight, float size) : base(id, name, ItemType.Melee, weight, size)
		{

		}
	}

    public class Firearm : Weapon
    {
		public Firearm(int id, String name, float weight, float size) : base(id, name, ItemType.Firearm, weight, size)
        {
            
        }
    }
}
