#if GAME
  using Microsoft.Xna.Framework;
#else
  using System.Drawing;
  using System.Numerics;
#endif

namespace Server.Payloads
{
  /// <summary>
  /// Declarative payload
  /// </summary>
  public class NewItemData
  {
    public int uid;
    public string id;
    public bool onmap;
    public int x;
    public int y;

    public NewItemData(int uid_, string id_, Point? position)
    {
      uid = uid_;
      id = id_;
      if (position == null) onmap = false;
      else
      {
        onmap = true;
        x = position.GetValueOrDefault().X;
        y = position.GetValueOrDefault().Y;
      }
    }
  }
  /// <summary>
  /// Update payload
  /// </summary>
  public class ItemData
  {
    public int uid;
    public bool onmap;
    public int x;
    public int y;

    public ItemData(int uid_, Point? position)
    {
      if (position == null) onmap = false;
      else
      {
        onmap = true;
        x = position.GetValueOrDefault().X;
        y = position.GetValueOrDefault().Y;
      }
    }
  }
}
