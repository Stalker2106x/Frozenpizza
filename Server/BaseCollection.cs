using FrozenPizza.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FPServer
{
  /// <summary>
  /// Holds basic representation of items
  /// </summary>
  public static class BaseCollection
  {
    public static List<MeleeWeapon> MeleeList;
    public static List<FireWeapon> FirearmList;
    public static void Load()
    {
      LoadMelee();
      LoadFirearm();
    }

    public static void LoadMelee()
    {
      string meleeData = File.ReadAllText("Data/items/melee.json");
      MeleeList = JsonConvert.DeserializeObject<List<MeleeWeapon>>(meleeData);
    }

    public static void LoadFirearm()
    {
      string fireData = File.ReadAllText("Data/items/weapons.json");
      FirearmList = JsonConvert.DeserializeObject<List<FireWeapon>>(fireData);
    }

  }
}
