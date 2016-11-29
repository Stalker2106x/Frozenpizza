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

        public Weapon(int id, String name, ItemType type, float weight, float size) : base(id, name, type, weight, size)
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
		public Melee(int id, String name, float weight, float size) : base(id, name, ItemType.Melee, weight, size)
		{
            Sounds = new SoundEffect[1];
		}
        public void LoadSounds(ContentManager content)
        {
            Sounds[0] = content.Load<SoundEffect>("sounds/weapon/" + ResourceId + "/attack");
        }
        public void attack()
        {
            Sounds[0].Play();
        }
	}

    public class Firearm : Weapon
    {

		public int Accuracy { get; set; }
        public int LoadedAmmo { get; set; }
        public int ClipSize { get; set; }

		public Firearm(int id, String name, float weight, float size) : base(id, name, ItemType.Firearm, weight, size)
        {
        }

        public float getFireAngle(float[] angle)
        {
            Random rnd = new Random();
            int max = (int)(angle[0] * 100);
            int min = (int)(angle[1] * 100);

            return ((float)(rnd.Next(min, max + 1) / 100f));
        }

        public void fire(List<Projectile> projectiles, Vector2 pos, float[] aimAccuracyAngle)
        {
            float angle = getFireAngle(aimAccuracyAngle);

            if (LoadedAmmo > 0)
            {
                Sounds[(int)FirearmActions.Fire].Play();
                projectiles.Add(new Projectile(ProjectileType.Bullet, pos, angle, 5f));
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

		public void SetFirearmAttributes(int accuracy, int clipsize)
		{
			Accuracy = accuracy;
            ClipSize = accuracy;
            LoadedAmmo = ClipSize;
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
