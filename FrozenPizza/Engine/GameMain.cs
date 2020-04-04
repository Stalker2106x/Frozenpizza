using FrozenPizza.Settings;
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
    public static Level level;
    public static MainPlayer mainPlayer { get; set; }
    public static List<Player> players;

    static Camera cam;
    public static HUD hud;

    //Static accessors
    public static Player getPlayerById(int id)
    {
      if (players == null)
        return (null);
      for (int i = 0; i < players.Count; i++)
        if (players[i].id == id)
          return (players[i]);
      return (null);
    }

    public static void Initialize(GraphicsDevice graphics)
    {
      players = new List<Player>();
      cam = new Camera(graphics);
      hud = new HUD(graphics, cam);
      mainPlayer = new MainPlayer("Bernie");
    }

    public static void Load(ContentManager content)
    {
      level = new Level();
      level.Load(content);
      hud.Load(content);
      mainPlayer.Load();
    }

    public static void Unload()
    {
      level = null;
      hud = null;
      mainPlayer = null;
    }

    public static void Update(GameTime gameTime, DeviceState state, DeviceState prevState, Cursor cursor)
    {
      level.Update(); //Update world
      mainPlayer.Update(gameTime, level, state, prevState, cam, cursor);
      for (int i = 0; i < players.Count; i++)
        players[i].Update(gameTime);
      hud.Update(state, prevState, mainPlayer);
      if (mainPlayer.alive && !mainPlayer.inventoryOpen) //If we are ingame reset mouse each loop
        Mouse.SetPosition(cam.getViewport().Width / 2, cam.getViewport().Height / 2);
    }
    public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, GraphicsDevice graphics)
    {
      spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cam.getTransformation());
      level.Draw(spriteBatch, cam, mainPlayer);
      mainPlayer.Draw(spriteBatch);
      for (int i = 0; i < players.Count; i++) //Draw players
      {
        players[i].Draw(spriteBatch);
      }
      spriteBatch.End();
      spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
      hud.Draw(spriteBatch, graphics, mainPlayer, cam);
      spriteBatch.End();
    }
  }
}
