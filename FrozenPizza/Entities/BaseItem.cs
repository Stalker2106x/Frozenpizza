#if GAME
  using Microsoft.Xna.Framework;
#else
  using System.Drawing;
  using System.Numerics;
#endif
using System;

namespace FrozenPizza.Entities
{
  public enum ItemType
  {
    MeleeWeapon,
    FireWeapon
  }

  public class BaseItem
  {
    public int uid;
    public string name;
    public string id;
    public ItemType type;
    public float weight;
    public Point size;

    public Point? position;

    public BaseItem Copy()
    {
      return (BaseItem)this.MemberwiseClone();
    }
  }
}