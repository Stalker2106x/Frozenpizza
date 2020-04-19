using FrozenPizza.Entities;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FrozenPizza
{
  class EntityStore
  {
    public static List<MeleeWeapon> MeleeWeapons;
    public static List<RangedWeapon> RangedWeapons;

    public static void LoadMelee()
    {
      string meleeData = File.ReadAllText("Data/items/melee.json");
      MeleeWeapons = JsonConvert.DeserializeObject<List<MeleeWeapon>>(meleeData);
    }

    public static void LoadRanged()
    {
      string rangedData = File.ReadAllText("Data/items/weapons.json");
      RangedWeapons = JsonConvert.DeserializeObject<List<RangedWeapon>>(rangedData);
    }

    public static Item GetItemWithId(string id)
    {
      foreach (var melee in MeleeWeapons)
      {
        if (melee.id == id)
        {
          var copy = new MeleeWeapon();
          copy.Copy(melee);
          copy.Init();
          return (copy);
        }
      }
      foreach (var firearm in RangedWeapons)
      {
        if (firearm.id == id)
        {
          var copy = new RangedWeapon();
          copy.Copy(firearm);
          copy.Init();
          return (copy);
        }
      }
      return (null);
    }

    /// <summary>
    /// Game Logic
    /// </summary>
#if GAME
    public static void LoadGameMelee(ContentManager content)
    {
      LoadMelee();
      foreach (var melee in MeleeWeapons)
      {
        melee.textures["world"] = content.Load<Texture2D>("gfx/weapons/" + melee.id);
        melee.textures["attackEffect"] = content.Load<Texture2D>("gfx/swoosh");
        melee.sounds["use"] = content.Load<SoundEffect>("sounds/weapons/melee");
        melee.Init();
      }
    }
    public static void LoadGameRanged(ContentManager content)
    {
      LoadRanged();
      foreach (var weapon in RangedWeapons)
      {
        weapon.textures["world"] = content.Load<Texture2D>("gfx/weapons/" + weapon.id);
        weapon.textures["muzzleFlashEffect"] = content.Load<Texture2D>("gfx/muzzleFlash");
        weapon.sounds["use"] = content.Load<SoundEffect>("sounds/weapons/" + weapon.id + "/fire");
        weapon.sounds["reload"] = content.Load<SoundEffect>("sounds/weapons/" + weapon.id + "/reload");
        weapon.Init();
      }
    }
#endif
  }
}