using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{

  public class Item
  {
    public Int64 Uid { get; set; }
    public int Id { get; set; }
    public PointF Pos { get; set; }

    public Item(Int64 uid, int id, PointF pos)
    {
      Uid = uid;
      Id = id;
      Pos = pos;
    }
  }
}
