using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Payloads
{
  public class GameData
  {
    public string mapName;
    public int clientId;

    public GameData(int id, string mapName_)
    {
      clientId = id;
      mapName = mapName_;
    }
  }
}
