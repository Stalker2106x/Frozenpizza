using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Security.Cryptography;
using System.Text;

namespace FrozenPizza.Utils
{
  public static class Tools
  {
    public static string Sha256(string value)
    {
      StringBuilder Sb = new StringBuilder();

      using (var hash = SHA256.Create())
      {
        Encoding enc = Encoding.UTF8;
        byte[] result = hash.ComputeHash(enc.GetBytes(value));

        foreach (byte b in result) Sb.Append(b.ToString("x2"));
      }

      return Sb.ToString();
    }
    public static Vector2 RotateAroundOrigin(Vector2 point, Vector2 origin, float rotation)
    {
      return Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
    }
    public static Rectangle GetDrawRect(int x, int y, Point size)
    {
      return (new Rectangle(new Point(x - (size.X / 2), y - (size.Y / 2)), size));
    }
    public static Rectangle GetDrawRect(int x, int y, Texture2D texture)
    {
      return (new Rectangle(new Point(x - (texture.Width / 2), y - (texture.Height / 2)), new Point(texture.Width, texture.Height)));
    }
    public static Vector2 GetDrawPoint(Vector2 position, Texture2D texture)
    {
      return (position - new Vector2(texture.Width / 2, texture.Height / 2));
    }
    public static Vector2 GetDrawPoint(Vector2 position, Rectangle rect)
    {
      return (position - new Vector2(rect.Width / 2, rect.Height / 2));
    }
  }
}
