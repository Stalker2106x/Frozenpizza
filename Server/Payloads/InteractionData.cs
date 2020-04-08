using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Payloads
{
  public enum ActionType
  {
    Fire
  }
  public class InteractionData
  {
    public int playerId;
    public ActionType action;
    public float value;

    public InteractionData(int playerId_, ActionType action_, float value_)
    {
      playerId = playerId_;
      action = action_;
      value = value_;
    }
  }
}
