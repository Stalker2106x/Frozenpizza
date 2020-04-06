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

    static Camera cam;
    public static HUD hud;

    public static void Load(GraphicsDevice graphics)
    {
      cam = new Camera(graphics);
      hud = new HUD(graphics, cam);
    }

    public static void Unload()
    {
      map = null;
      mainPlayer = null;
      players = null;
    }

    public static void Update(GameTime gameTime, DeviceState state, DeviceState prevState, Cursor cursor)
    {
      //map.Update(); //Update world
      mainPlayer.Update(gameTime, map, state, prevState, cam, cursor);
      hud.Update(state, prevState, mainPlayer);
      if (mainPlayer.active && !mainPlayer.inventoryOpen) //If we are ingame reset mouse each loop
        Mouse.SetPosition(cam.getViewport().Width / 2, cam.getViewport().Height / 2);
    }
    public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, GraphicsDevice graphics)
    {
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, cam.getTransformation());
      map.Draw(spriteBatch);
      mainPlayer.Draw(spriteBatch);
      foreach (var player in players) player.Draw(spriteBatch); //Draw players
      spriteBatch.End();
      spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
      hud.Draw(spriteBatch, graphics, mainPlayer, cam);
      spriteBatch.End();
    }
  }
}
