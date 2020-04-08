#if GAME
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;
#else
  using System.Drawing;
  using System.Numerics;
#endif
using System;

namespace FrozenPizza.Entities
{
  public class BaseItem
  {
    public int uid;
    public string name;
    public string id;
    public float weight;
    public Point size;

    public Point? position;

    public BaseItem Copy()
    {
      return (BaseItem)this.MemberwiseClone();
    }

    /// <summary>
    /// Game specific logic
    /// </summary>
#if GAME
    public Texture2D texture;
    public virtual void use(Player player) { throw new Exception("BaseItem should be overriden"); }

    public void Draw(SpriteBatch spriteBatch, Point tileSize)
    {
      if (position == null) return; //Not on world
      spriteBatch.Draw(texture, new Rectangle(position.GetValueOrDefault().X * tileSize.X, position.GetValueOrDefault().Y * tileSize.Y, tileSize.X, tileSize.Y), Color.White);
    }
#endif
  }

}