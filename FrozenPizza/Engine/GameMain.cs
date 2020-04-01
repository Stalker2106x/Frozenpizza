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
  public enum GameState
  {
    Splashscreen,
    Menu,
    Playing
  }

  public static class GameMain
  {
    private static Camera cam;
    public static HUD hud;
    public static Level level;
    public static MainPlayer mainPlayer;
    public static List<Player> players;

    //Netcode
    public static NetHandler netHandle;

    public static void InitializeGame()
    {
      cam = new Camera();
      hud = new HUD(cam);
      mainPlayer = new MainPlayer("Bernie");
      players = new List<Player>();
    }

    public static void LoadGame(ContentManager content)
    {
      level = new Level();
      level.Load(content);
      hud.Load(content);
      mainPlayer.Load(content);
      //level.GenerateItems(collection);
    }

    public static void UnloadGame()
    {
      level = null;
      hud = null;
      mainPlayer = null;
    }
    public static void resetMousePos()
    {
      Mouse.SetPosition(cam.getViewport().Width / 2, cam.getViewport().Height / 2);
    }

    public static void Update(GameTime gameTime, DeviceState state, DeviceState prevState)
    {
      if (Options.Config.Bindings[GameAction.Menu].IsControlPressed(state, prevState))
      {
        mainPlayer.inventoryOpen = false;
        Menu.GameMenu();
        Engine.setState(GameState.Menu);
        //if (_cursor.Show == false) toggleMouseVisible();
      }
      if (!false) //Player not dead
      {
        level.Update(); //Update world
        mainPlayer.Update(gameTime, level, state, prevState, cam);
        for (int i = 0; i < players.Count; i++)
          players[i].Update(gameTime);
        hud.Update(mainPlayer);
        if (mainPlayer.alive && !mainPlayer.inventoryOpen) //If we are ingame reset mouse each loop
          resetMousePos();
      }
    }

    public static void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cam.getTransformation());
      level.Draw(spriteBatch, cam, mainPlayer);
      mainPlayer.Draw(spriteBatch);
      for (int i = 0; i < players.Count; i++) //Draw players
      {
        players[i].Draw(spriteBatch);
      }
      //IF PLAYING
      spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
      hud.Draw(spriteBatch, mainPlayer, cam);
      spriteBatch.End();
    }
  }
}
