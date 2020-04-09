#if GAME
using FrozenPizza.Network;
using FrozenPizza.Settings;
using Server.Payloads;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenPizza.Entities
{
  /// <summary>
  /// Common base class for weapons
  /// </summary>
  public class BaseWeapon : BaseItem
  {
    public float cooldown;
    public int damage;

    /// <summary>
    /// Game specific logic
    /// </summary>
#if GAME
    public override void use(Player player)
    {
      throw new Exception("Unknown Base Weapon");
    }
#endif
  }

  /// <summary>
  /// Melee Wepaon
  /// </summary>
  public class MeleeWeapon : BaseWeapon
  {
    public MeleeWeapon() : base()
    {

    }

    public new MeleeWeapon Copy()
    {
      return (MeleeWeapon)this.MemberwiseClone();
    }

    /// <summary>
    /// Game specific logic
    /// </summary>
#if GAME
    public override void use(Player player)
    {
    }
#endif
  }

  /// <summary>
  /// Firearm
  /// </summary>
  public enum FireMode
  {
    FullAuto,
    SemiAuto
  }

  public class FireWeapon : BaseWeapon
  {
    private static Random _randomGenerator = new Random();

    public int ammo;
    public int magazineSize;
    public float reloadDelay;

    public FireMode fireMode;

    public float accuracy;

    public new FireWeapon Copy()
    {
      return (FireWeapon)this.MemberwiseClone();
    }

    /// <summary>
    /// Game specific logic
    /// </summary>
#if GAME
    public override void use(Player player)
    {
      if (fireMode == FireMode.SemiAuto && Options.Config.Bindings[GameAction.UseHands].IsControlHeld(Engine.getDeviceState(), Engine.getPrevDeviceState())) return;
      if (ammo <= 0)
      {
        Collection.Dryfire.Play(Options.Config.SoundVolume, 0f, 0f);
        return;
      }
      ammo--;
      float[] angle = GameMain.mainPlayer.getAimAccuracyAngle();
      sounds["use"].Play(Options.Config.SoundVolume, 0f, 0f);
      GameMain.projectiles.Add(new Projectile(player.position + player.getDirectionVector(Direction.Forward, 10), (int)(player.orientation + (_randomGenerator.Next((int)angle[1] * 10, (int)angle[0] * 10) / 10) ), 200.0f, damage));
      ClientSenderV2.SendProjectile(new InteractionData(player.id, ActionType.Fire, damage));
    }

    public void reload()
    {
      sounds["reload"].Play(Options.Config.SoundVolume, 0f, 0f);
      ammo = magazineSize;
    }
#endif
  }
}
