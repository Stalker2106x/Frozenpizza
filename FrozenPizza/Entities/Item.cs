using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrozenPizza.Entities
{
  public class Item : BaseItem
  {
    public Texture2D texture;

    public void Draw(SpriteBatch spriteBatch, Point tileSize)
    {
      if (position == null) return; //Not on world
      spriteBatch.Draw(texture, new Rectangle(position.GetValueOrDefault().X * tileSize.X, position.GetValueOrDefault().Y * tileSize.Y, tileSize.X, tileSize.Y), Color.White);
    }
  }
}
