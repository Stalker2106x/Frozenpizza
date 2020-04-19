using FrozenPizza.Utils;
using FrozenPizza.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenPizza.Entities.Actors
{
  class Creature : Actor
  {
    Timer timer;
    public Creature(string name_, int hp_, Vector2 position) : base(name_, hp_, position)
    {
      timer = new Timer();
      timer.addAction(TimerDirection.Forward, 1000, TimeoutBehaviour.StartOver, () =>
      {
        var astar = Pathfinding.FindBestPath(GameMain.map.WorldToGrid(position), GameMain.map.WorldToGrid(GameMain.mainPlayer.position));
        if (astar.Count > 1) position = GameMain.map.GridToWorld(astar[1].Location);
      });
    }

#if GAME
    public void Update(GameTime gameTime)
    {
      timer.Update(gameTime);
    }
#endif
  }
}
