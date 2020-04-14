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
  public class FullPlayerData
  {
    public string name;

    public bool active;

    public int hp;

    public PlayerData data;

    public FullPlayerData(string name_, bool active_, int hp_, PlayerData data_)
    {
      name = name_;
      active = active_;
      hp = hp_;
      data = data_;
    }
  }

  /// <summary>
  /// Update payload
  /// </summary>
  public class PlayerData
  {
    public int id;

    public int x;
    public int y;

    public float orientation;

    public PlayerData(int id_, Vector2 position, float orientation_)
    {
      id = id_;
      orientation = orientation_;
      x = (int)position.X;
      y = (int)position.Y;
    }
  }
}
