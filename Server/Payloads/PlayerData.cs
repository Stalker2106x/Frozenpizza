#if GAME
using Microsoft.Xna.Framework;
#else
  using System.Drawing;
  using System.Numerics;
#endif

namespace Server.Payloads
{
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
