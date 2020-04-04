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
  public static class Collection
  {
    public static Texture2D GameLogo;
    public static SoundEffect[] MenuSounds;
    public static List<Melee> MeleeList;
    public static List<Firearm> PistolsList;
    public static Texture2D[] Tilesets;
    public static Texture2D Projectiles;
    public static Texture2D Players;
    public static SoundEffect[] PlayersSound;

    public static bool Load(ContentManager content)
    {
      MenuSounds = new SoundEffect[2];
      //Game
      MeleeList = new List<Melee>();
      PistolsList = new List<Firearm>();
      Tilesets = new Texture2D[Enum.GetNames(typeof(ItemType)).Length];

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

    public static Item getItemById(int id)
    {
      if (id < (int)ItemIds.Pistol) //Melee
      {
        return (MeleeList[id]);
      }
      else if (id >= (int)ItemIds.Pistol && id < (int)ItemIds.Rifle) //Pistol
      {
        return (PistolsList[id - (int)ItemIds.Pistol]);
      }
      return (null);
    }

    public static Item getNewItemById(Int64 uid, int id)
    {
      if (id < (int)ItemIds.Pistol) //Melee
      {
        return (MeleeList[id].Copy());
      }
      else if (id >= (int)ItemIds.Pistol && id < (int)ItemIds.Rifle) //Pistol
      {
        return (PistolsList[id - (int)ItemIds.Pistol].Copy());
      }
      return (null);
    }

    public static bool LoadMelee(ContentManager content)
    {
      XElement bundle = XElement.Load("Data/items/melee.xml");

      Tilesets[(int)ItemType.Melee] = content.Load<Texture2D>("gfx/melee");
      foreach (var item in bundle.Elements("Item"))
      {
        MeleeList.Add(new Melee(-1, (int)item.Element("Id"), item.Element("Name").Value, (float)item.Element("Weight"), (float)item.Element("Size")));
        MeleeList.Last().SetWeaponAttributes(item.Element("ResourceId").Value.ToString(), (int)item.Element("Damage"), (float)item.Element("Cooldown"));
        MeleeList.Last().LoadSounds(content);
      }
      return (true);
    }

    public static bool LoadPistols(ContentManager content)
    {
      XElement bundle = XElement.Load("Data/items/firearms.xml");

      Tilesets[(int)ItemType.Firearm] = content.Load<Texture2D>("gfx/firearms");
      foreach (var item in bundle.Elements("Item"))
      {
        PistolsList.Add(new Firearm(-1, (int)item.Element("Id"), item.Element("Name").Value, (float)item.Element("Weight"), (float)item.Element("Size")));
        PistolsList.Last().SetWeaponAttributes(item.Element("ResourceId").Value.ToString(), (int)item.Element("Damage"), (float)item.Element("Cooldown"));
        PistolsList.Last().SetFirearmAttributes((int)item.Element("Accuracy"), (int)item.Element("ClipSize"), (float)item.Element("ReloadCooldown"));
        PistolsList.Last().LoadSounds(content);
      }
      return (true);
    }
    public static bool LoadPlayers(ContentManager content)
    {
      Players = content.Load<Texture2D>("gfx/players");
      PlayersSound = new SoundEffect[Enum.GetNames(typeof(PlayerSounds)).Length];
      PlayersSound[(int)PlayerSounds.Step1] = content.Load<SoundEffect>("sounds/player/step1");
      PlayersSound[(int)PlayerSounds.Step2] = content.Load<SoundEffect>("sounds/player/step2");
      PlayersSound[(int)PlayerSounds.Step3] = content.Load<SoundEffect>("sounds/player/step3");
      PlayersSound[(int)PlayerSounds.Step4] = content.Load<SoundEffect>("sounds/player/step4");
      PlayersSound[(int)PlayerSounds.RunStep1] = content.Load<SoundEffect>("sounds/player/rstep1");
      PlayersSound[(int)PlayerSounds.RunStep2] = content.Load<SoundEffect>("sounds/player/rstep2");
      PlayersSound[(int)PlayerSounds.RunStep3] = content.Load<SoundEffect>("sounds/player/rstep3");
      PlayersSound[(int)PlayerSounds.RunStep4] = content.Load<SoundEffect>("sounds/player/rstep4");
      PlayersSound[(int)PlayerSounds.Hurt] = content.Load<SoundEffect>("sounds/player/hurt");
      PlayersSound[(int)PlayerSounds.Die] = content.Load<SoundEffect>("sounds/player/die");
      return (true);
    }
    public static bool LoadProjectiles(ContentManager content)
    {
      Projectiles = content.Load<Texture2D>("gfx/projectiles");
      return (true);
    }
  }
}
