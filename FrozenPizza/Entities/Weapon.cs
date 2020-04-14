#if GAME
  using FrozenPizza.Network;
  using FrozenPizza.Settings;
  using FrozenPizza.Utils;
  using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
  [Serializable]
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
    public override void Copy(BaseItem toCopy)
    {
      base.Copy(toCopy);
      BaseWeapon rToCopy;
      if ((rToCopy = toCopy as BaseWeapon) == null) return;
      cooldown = rToCopy.cooldown;
      damage = rToCopy.damage;
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
    protected Timer useCooldown;

    public override void Update(GameTime gameTime)
    {
      useCooldown.Update(gameTime);
    }

    public override bool use(Player player)
    {
      if (useCooldown.isActive()) return (false);
      useCooldown.Start();
      return (true);
    }
#endif
  }

  /// <summary>
  /// Melee Wepaon
  /// </summary>
  [Serializable]
  public class MeleeWeapon : BaseWeapon
  {
    public MeleeWeapon() : base()
    {
#if GAME
      attackEffectTimer = new Timer();
#endif
    }
    public override void Copy(BaseItem toCopy)
    {
      base.Copy(toCopy);
      MeleeWeapon rToCopy;
      if ((rToCopy = toCopy as MeleeWeapon) == null) return;
#if GAME
      attackEffectTimer = new Timer();
#endif
    }

    public override void Init()
    {
      base.Init();
#if GAME
      attackEffectTimer.addAction(TimerDirection.Forward, 150, TimeoutBehaviour.Reset, () => { });
#endif
    }

    /// <summary>
    /// Game specific logic
    /// </summary>
#if GAME
    protected Timer attackEffectTimer;

    public override bool use(Player player)
    {
      if (!base.use(player)) return (false);
      if (Options.Config.Bindings[GameAction.UseHands].IsControlHeld(Engine.getDeviceState(), Engine.getPrevDeviceState())) return (false);
      sounds["use"].Play(Options.Config.SoundVolume, 0f, 0f);
      attackEffectTimer.Start();
      Vector2 hitPos = player.position + player.getDirectionVector(Direction.Forward, 15);
      ClientSenderV2.SendMeleeHit(new MeleeHitData(player.id, hitPos, damage));
      return (true);
    }
    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
      attackEffectTimer.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch, Player player)
    {
      base.Draw(spriteBatch, player);
      if (attackEffectTimer.isActive())
      {
        spriteBatch.Draw(textures["attackEffect"], player.position + player.getDirectionVector(Direction.Forward, 15),
          new Rectangle(0, 0, textures["attackEffect"].Width, textures["attackEffect"].Height), Color.White, -player.orientation, Vector2.Zero, 1.0f, SpriteEffects.None, 0.3f);
      }
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

  [Serializable]
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
      muzzleFlashTimer = new Timer();
#endif
    }
    public override void Copy(BaseItem toCopy)
    {
      base.Copy(toCopy);
      FireWeapon rToCopy;
      if ((rToCopy = toCopy as FireWeapon) == null) return;
      ammo = rToCopy.ammo;
      magazineSize = rToCopy.magazineSize;
      reloadDelay = rToCopy.reloadDelay;
      accuracy = rToCopy.accuracy;
#if GAME
      fireMode = rToCopy.fireMode;
      reloadTimer = new Timer();
      muzzleFlashTimer = new Timer();
#endif
    }

    public override void Init()
    {
      base.Init();
#if GAME
      reloadTimer.addAction(TimerDirection.Forward, reloadDelay, TimeoutBehaviour.Reset, () => { });
      muzzleFlashTimer.addAction(TimerDirection.Forward, 150, TimeoutBehaviour.Reset, () => { });
#endif
    }

    /// <summary>
    /// Game specific logic
    /// </summary>
#if GAME
    protected FireMode fireMode;
    protected Timer reloadTimer;
    protected Timer muzzleFlashTimer;

    public override bool use(Player player)
    {
      if (!base.use(player)) return (false);
      if (reloadTimer.isActive()
      || (fireMode == FireMode.SemiAuto && Options.Config.Bindings[GameAction.UseHands].IsControlHeld(Engine.getDeviceState(), Engine.getPrevDeviceState()))) return (false);
      if (ammo <= 0)
      {
        Collection.Dryfire.Play(Options.Config.SoundVolume, 0f, 0f);
        return (true);
      }
      ammo--;
      muzzleFlashTimer.Start();
      AccuracyAngle angle = GameMain.mainPlayer.getAimAccuracyAngle(true);
      angle.EnforceComparison();
      float bulletAngle = (float)(_randomGenerator.Next((int)(angle.right * 1000000f), (int)(angle.left * 1000000f)) / 1000000f);
      Vector2 bulletPos = player.position + player.getDirectionVector(Direction.Forward, 15);
      float bulletVelocity = 400f;

      sounds["use"].Play(Options.Config.SoundVolume, 0f, 0f);
      GameMain.projectiles.Add(new Projectile(player.id, bulletPos, bulletAngle, bulletVelocity, damage));
      ClientSenderV2.SendProjectile(new ProjectileData(player.id, bulletPos, bulletAngle, bulletVelocity, damage));
      return (true);
    }

    public void reload()
    {
      if (reloadTimer.isActive() || ammo == magazineSize) return;
      sounds["reload"].Play(Options.Config.SoundVolume, 0f, 0f);
      reloadTimer.Start();
      ammo = magazineSize;
    }
    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
      reloadTimer.Update(gameTime);
      muzzleFlashTimer.Update(gameTime);
      GameMain.hud.updateWeapon(ammo, magazineSize, (float)reloadTimer.getDuration());
    }
    public override void Draw(SpriteBatch spriteBatch, Player player)
    {
      base.Draw(spriteBatch, player);
      if (muzzleFlashTimer.isActive())
      {
        spriteBatch.Draw(textures["muzzleFlashEffect"], player.position + player.getDirectionVector(Direction.Forward, 15),
          new Rectangle(0, 0, textures["muzzleFlashEffect"].Width, textures["muzzleFlashEffect"].Height), Color.White, -player.orientation,
          new Vector2(16, 8), 1.0f, SpriteEffects.None, 0.3f);
      }
    }
#endif
  }
}
