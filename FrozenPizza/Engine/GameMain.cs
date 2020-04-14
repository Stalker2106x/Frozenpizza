using FrozenPizza.Entities;
using FrozenPizza.Settings;
using FrozenPizza.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenPizza
{
  class GameMain
  {
    //Game
    public static Map map;
    public static MainPlayer mainPlayer { get; set; }
    public static List<Player> players;

    public static List<Projectile> projectiles;

    static Camera cam;
    public static HUD hud;

    public static void Load(GraphicsDevice graphics)
    {
      players = new List<Player>();
      projectiles = new List<Projectile>();
      cam = new Camera(graphics);
      hud = new HUD(graphics, cam);
    }

    public static void Reset()
    {
      mainPlayer = null;
      map = null;
      players.Clear();
      projectiles.Clear();
    }

    public static void Update(GameTime gameTime, DeviceState state, DeviceState prevState, Cursor cursor)
    {
      //map.Update(); //Update world
      mainPlayer.Update(gameTime, map, state, prevState, cam, cursor);
      for (int i = projectiles.Count - 1; i >= 0; i--)
      {
        if (!projectiles[i].Update(gameTime)) projectiles.RemoveAt(i);
      }
      hud.Update(state, prevState, mainPlayer);
      if (mainPlayer.active && !hud.overlayActive) //If we are ingame reset mouse each loop
        Mouse.SetPosition(cam.getViewport().Width / 2, cam.getViewport().Height / 2);
    }
    public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, GraphicsDevice graphics)
    {
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, cam.getTransformation());
      map.Draw(spriteBatch);
      mainPlayer.Draw(spriteBatch);
      foreach (var projectile in projectiles) projectile.Draw(spriteBatch);
      foreach (var player in players) player.Draw(spriteBatch); //Draw players
      spriteBatch.End();
      spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
      hud.Draw(spriteBatch, graphics, mainPlayer, cam);
      spriteBatch.End();
    }
  }
}
