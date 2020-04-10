#if GAME
  using FrozenPizza.Network;
  using FrozenPizza.Settings;
  using FrozenPizza.Utils;
  using Microsoft.Xna.Framework;
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

    public BaseWeapon() : base()
    {
#if GAME
      useCooldown = new Timer();
#endif
    }

    public override void Init()
    {
      base.Init();
#if GAME
      useCooldown.addAction(TimerDirection.Forward, cooldown, TimeoutBehaviour.Reset, () => { });
#endif
    }

    /// <summary>
    /// Game specific logic
    /// </summary>
#if GAME
    public Timer useCooldown;

    public override void Update(GameTime gameTime)
    {
      useCooldown.Update(gameTime);
    }

    public override void use(Player player)
    {
      if (useCooldown.getDuration() != 0) return;
      useCooldown.Start();
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

    public float accuracy;


    public FireWeapon() : base()
    {
#if GAME
      reloadTimer = new Timer();
#endif
    }

    public override void Init()
    {
      base.Init();
#if GAME
      reloadTimer.addAction(TimerDirection.Forward, reloadDelay, TimeoutBehaviour.Reset, () => { });
#endif
    }

    public new FireWeapon Copy()
    {
      return (FireWeapon)this.MemberwiseClone();
    }

    /// <summary>
    /// Game specific logic
    /// </summary>
#if GAME
    public FireMode fireMode;
    public Timer reloadTimer;

    public override void use(Player player)
    {
      if (reloadTimer.getDuration() != 0
      || (fireMode == FireMode.SemiAuto && Options.Config.Bindings[GameAction.UseHands].IsControlHeld(Engine.getDeviceState(), Engine.getPrevDeviceState()))) return;
      if (ammo <= 0)
      {
        Collection.Dryfire.Play(Options.Config.SoundVolume, 0f, 0f);
        return;
      }
      base.use(player);
      ammo--;
      AccuracyAngle angle = GameMain.mainPlayer.getAimAccuracyAngleRelative();
      sounds["use"].Play(Options.Config.SoundVolume, 0f, 0f);
      GameMain.projectiles.Add(new Projectile(player.position + player.getDirectionVector(Direction.Forward, 10), _randomGenerator.Next( (int)angle.min * 1000, (int)angle.max * 1000) / 1000, 200.0f, damage));
      ClientSenderV2.SendProjectile(new InteractionData(player.id, ActionType.Fire, damage));
    }

    public void reload()
    {
      if (reloadTimer.getDuration() != 0 || ammo == magazineSize) return;
      sounds["reload"].Play(Options.Config.SoundVolume, 0f, 0f);
      reloadTimer.Start();
      ammo = magazineSize;
    }
    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
      reloadTimer.Update(gameTime);
      GameMain.hud.updateWeapon(ammo, magazineSize, (float)reloadTimer.getDuration());
    }
#endif
  }
}
