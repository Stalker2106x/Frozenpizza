#if GAME
using Microsoft.Xna.Framework;
#else
  using System.Drawing;
  using System.Numerics;
#endif

namespace Server.Payloads
{
  public class NewPlayerData
  {
    public string name;

    public int hp;

    public PlayerData data;

    public NewPlayerData(string name_, int hp_, PlayerData data_)
    {
      name = name_;
      hp = hp_;
      data = data_;
    }
  }
}
