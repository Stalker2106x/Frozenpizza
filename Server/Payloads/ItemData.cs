using Microsoft.Xna.Framework;

namespace Server.Payloads
{
  /// <summary>
  /// Declarative payload
  /// </summary>
  public class NewItemData
  {
    public int uid;
    public string id;
    public Point? position;

    public NewItemData(int uid_, string id_, Point? position_)
    {
      uid = uid_;
      id = id_;
      position = position_;
    }
  }
  /// <summary>
  /// Update payload
  /// </summary>
  public class ItemData
  {
    public int uid;
    public Point? position;

    public ItemData(int uid_, Point? position_)
    {
      uid = uid_;
      position = position_;
    }
  }
}
