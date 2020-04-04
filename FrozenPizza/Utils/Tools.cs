using FrozenPizza.Settings;
using Microsoft.Xna.Framework;
using System.Security.Cryptography;
using System.Text;

namespace FrozenPizza.Utils
{
  public class Gauge
  {
    private int _value;
    public int min { get; set; }
    public int max { get; set; }

    public Gauge(int value, int vmin, int vmax)
    {
      _value = value;
      min = vmin;
      max = vmax;
    }

    public int get()
    {
      return (_value);
    }

    public void set(int value)
    {
      _value = value;
      if (_value < min) _value = min;
      if (_value > max) _value = max;
    }
  }

  class Tools
  {
    public static bool IsOnScreen(Point p)
    {
      return ((p.X > 0 && p.X < Options.Config.Width) && (p.Y > 0 && p.Y < Options.Config.Height));
    }
    public static bool IsOnScreen(Rectangle r)
    {
      return ((r.X + r.Width > 0 && r.X < Options.Config.Width) && (r.Y + r.Height > 0 && r.Y < Options.Config.Height));
    }
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
  }
}
