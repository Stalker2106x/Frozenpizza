#if GAME
  using Microsoft.Xna.Framework;
#else
  using System.Drawing;
  using System.Numerics;
#endif

namespace Server.Payloads
{
  public class GameData
  {
    public string mapName;
    public int clientId;
    public int spawnX;
    public int spawnY;

    public GameData(int id, string mapName_, Point spawn)
    {
      clientId = id;
      mapName = mapName_;
      spawnX = spawn.X;
      spawnY = spawn.Y;
    }
  }
}
