using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenPizza.Entities
{

  public class Weapon : Item
  {
    public float cooldown;
    public float damage;

    public Weapon() : base()
    {

    }
  }

  /// <summary>
  /// Melee Wepaon
  /// </summary>
  public class MeleeWeapon : Weapon
  {
    public MeleeWeapon() : base()
    {

    }

    public MeleeWeapon Copy()
    {
      return (MeleeWeapon)this.MemberwiseClone();
    }

    public void use()
    {

    }
  }

  public enum FireMode
  {
    FullAuto,
    SemiAuto
  }

  /// <summary>
  /// Firearm
  /// </summary>
  public class FireWeapon : Weapon
  {
    public int ammo;
    public int magazineSize;
    public float reloadDelay;

    public FireMode fireMode;

    public float accuracy;

    public FireWeapon() : base()
    {

    }
    public FireWeapon Copy()
    {
      return (FireWeapon)this.MemberwiseClone();
    }

    public void use()
    {

    }

    public void reload()
    {

    }
  }
}
