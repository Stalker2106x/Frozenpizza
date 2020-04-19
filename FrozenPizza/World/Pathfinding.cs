using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenPizza.World
{
  public enum NodeState {
    Untested,
    Open,
    Closed
  }
  public class Node
  {
    public Point Location { get; set; }
    public bool IsWalkable { get; set; }
    public float G { get; set; }
    public float H { get; set; }
    public float F { get { return (G + H); } }
    public NodeState State { get; set; }
    public Node ParentNode { get; set; }

    public Node(Point pos, Point target)
    {
      Location = pos;
      H = Math.Abs(target.X - pos.X) + Math.Abs(target.Y - pos.Y); ;
    }
  }

  public static class Pathfinding
  {
    public static List<Node> GetClosestNodes(List<Node> openList, Node current, Point target)
    {
      List<Node> nodes = new List<Node>();

      for (int x = current.Location.X - 1; x < current.Location.X + 1; x++)
      {
        if (x < 0 || x > GameMain.map.size.X) continue;
        for (int y = current.Location.Y - 1; y < current.Location.Y + 1; y++)
        {
          if (y < 0 || y > GameMain.map.size.Y) continue;
          if (openList.FirstOrDefault((it) => { return (it.Location == new Point(x, y)); }) != null) continue;
          if (GameMain.map.grid[x, y] == 0)
          {
            var node = new Node(new Point(x, y), target);
            if ((x < current.Location.X || x > current.Location.X) && (y < current.Location.Y || y > current.Location.Y)) node.G = current.G + 1.5f;
            else node.G = current.G + 1f;
            nodes.Add(node);
          }
        }
      }
      return (nodes);
    }

    public static List<Node> FindBestPath(Point start, Point target)
    {
      List<Node> openList = new List<Node>();
      List<Node> closedList = new List<Node>();
      Node current = new Node(start, target);

      current.G = 0;
      openList.Add(current);
      while (openList.Count > 0)
      {
        GetClosestNodes(openList, current, target);
        var lowest = openList.Min(l => l.F);
        current = openList.First(l => l.F == lowest);
        if (current.Location == target) break; //DONE
        openList.Remove(current);
        closedList.Add(current);
      }
      return (closedList);
    }
  }
}
