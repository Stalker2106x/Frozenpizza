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
    public enum FirearmActions
    {
        Fire,
        DryFire,
        Reload
    }
    public class Weapon : Item
    {
        public int Damage { get; set; }
        public int Durability { get; set; }
        public float Cooldown { get; set; }
        public String ResourceId { get; set; }
        public SoundEffect[] Sounds { get; set; }

        public Weapon(Int64 uid, int id, String name, ItemType type, float weight, float size) : base(uid, id, name, type, weight, size)
		{

		}

		public void SetWeaponAttributes(String resourceId, int damage, float cooldown)
		{
			Random rnd = new Random();

            ResourceId = resourceId;
			Damage = damage;
			Cooldown = cooldown;
			Durability = rnd.Next(20, 101);
		}
	}

	public class Melee : Weapon
	{
		public Melee(Int64 uid, int id, String name, float weight, float size) : base(uid, id, name, ItemType.Melee, weight, size)
		{
            Sounds = new SoundEffect[1];
		}
        public void LoadSounds(ContentManager content)
        {
            Sounds[0] = content.Load<SoundEffect>("sounds/weapon/" + ResourceId + "/attack");
        }
        public void attack(Vector2 pos)
        {
            Sounds[0].Play();
            NetHandler.send("!MELEE " + Size + " " + Damage);
        }
	}

    public class Firearm : Weapon
    {

		public int Accuracy { get; set; }
        public int LoadedAmmo { get; set; }
        public int ClipSize { get; set; }
        public float ReloadCooldown { get; set; }

		public Firearm(Int64 uid, int id, String name, float weight, float size) : base(uid, id, name, ItemType.Firearm, weight, size)
        {
        }

        public float getFireAngle(float[] angle)
        {
            Random rnd = new Random();
            int max = (int)(angle[0] * 100);
            int min = (int)(angle[1] * 100);

            return ((float)(rnd.Next(min, max + 1) / 100f));
        }

        public void fire(Vector2 pos, float[] aimAccuracyAngle)
        {
            float angle = getFireAngle(aimAccuracyAngle);

            if (LoadedAmmo > 0)
            {
                Sounds[(int)FirearmActions.Fire].Play();
                NetHandler.send("!FIRE " + (int)ProjectileType.Bullet + " " + angle + " " + 7f + " " + Damage);
                LoadedAmmo -= 1;
            }
            else
            {
                Sounds[(int)FirearmActions.DryFire].Play();
            }
        }

        public bool reload()
        {
            if (LoadedAmmo == ClipSize)
                return (false);
            Sounds[(int)FirearmActions.Reload].Play();
            LoadedAmmo = ClipSize;
            return (true);
        }

		public void SetFirearmAttributes(int accuracy, int clipsize, float reloadCooldown)
		{
			Accuracy = accuracy;
            ClipSize = clipsize;
            LoadedAmmo = ClipSize;
            ReloadCooldown = reloadCooldown;
        }

        public void LoadSounds(ContentManager content)
        {
            Sounds = new SoundEffect[Enum.GetNames(typeof(FirearmActions)).Length];
            Sounds[(int)FirearmActions.Fire] = content.Load<SoundEffect>("sounds/weapon/" + ResourceId + "/fire");
            Sounds[(int)FirearmActions.DryFire] = content.Load<SoundEffect>("sounds/weapon/dryfire");
            Sounds[(int)FirearmActions.Reload] = content.Load<SoundEffect>("sounds/weapon/" + ResourceId + "/reload");

        }
    }
}
