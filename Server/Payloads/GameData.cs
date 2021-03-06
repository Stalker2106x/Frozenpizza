﻿using Microsoft.Xna.Framework;

namespace Server.Payloads
{
  public class GameData
  {
    public string mapName;
    public int clientId;
    public int spawnX;
    public int spawnY;

    public GameData(int id, string mapName_, Vector2 spawn)
    {
      clientId = id;
      mapName = mapName_;
      spawnX = (int)spawn.X;
      spawnY = (int)spawn.Y;
    }
  }
}
