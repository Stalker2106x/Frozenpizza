using Microsoft.Xna.Framework;

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

    public Vector2 position;

    public float orientation;

    public PlayerData(int id_, Vector2 position_, float orientation_)
    {
      id = id_;
      orientation = orientation_;
      position = position_;
    }
  }
}
