using FrozenPizza.Network;
using FrozenPizza.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;

namespace FrozenPizza
{
  //Enum for state managment
  public enum GameState
  {
    Splashscreen,
    Menu,
    Playing
  }

  public class Engine : Game
  {
    private static DeviceState deviceState;
    private static DeviceState prevDeviceState;
    private static GameState gameState;
    public static bool quit;
    public static Options options;

    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    //Database, Config and Netcode
    //public static NetHandler netHandle;

    //Menu & Cursor
    public static Cursor cursor;

    public static ClientV2 networkClient;

    public Engine()
    {
      quit = false;
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Data";
      options = new Options(graphics, GraphicsAdapter.DefaultAdapter);
      graphics.IsFullScreen = Options.Config.Fullscreen;
      graphics.PreferredBackBufferWidth = Options.Config.Width;
      graphics.PreferredBackBufferHeight = Options.Config.Height;
      graphics.PreferMultiSampling = true;
      graphics.GraphicsProfile = GraphicsProfile.HiDef;
      graphics.SynchronizeWithVerticalRetrace = false;
      GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
      GraphicsDevice.RasterizerState = new RasterizerState()
      {
        CullMode = CullMode.None,
        MultiSampleAntiAlias = true,
      };
      IsFixedTimeStep = true;
      TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 90.0); //Set FPS
      graphics.ApplyChanges();
      gameState = GameState.Menu;
      cursor = new Cursor();
      MyraEnvironment.Game = this;
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      ClientHandlerV2.Initialize();
      IsMouseVisible = false;
      cursor.Show = true;
      base.Initialize();
    }

    /// <summary>
    /// When losing and getting back focus of window
    /// </summary>
    protected override void OnDeactivated(Object sender, EventArgs args)
    {
      IsMouseVisible = true;
      cursor.Show = false;
      //call the base method and fire the event
      base.OnDeactivated(sender, args);
    }

    protected override void OnActivated(object sender, EventArgs args)
    {
      IsMouseVisible = false;
      switch (gameState)
      {
        case GameState.Splashscreen:
        case GameState.Menu:
          cursor.Show = true;
          break;
        case GameState.Playing:
          cursor.Show = false;
          break;
      }
      base.OnActivated(sender, args);
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      cursor.Load(this.Content);
      spriteBatch = new SpriteBatch(GraphicsDevice);
      Collection.Load(this.Content, GraphicsDevice);
      GameMain.Load(GraphicsDevice);
      Menu.MainMenu();
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    public static void setCursor(bool state)
    {
      cursor.Show = state;
    }
    public static DeviceState getPrevDeviceState()
    {
      return (prevDeviceState);
    }
    public static DeviceState getDeviceState()
    {
      return (deviceState);
    }

    public static void setState(GameState state)
    {
      gameState = state;
      switch (state)
      {
        case GameState.Playing:
          //MediaPlayer.Stop();
          setCursor(false);
          GameMain.hud.activate();
          break;
        case GameState.Menu:
          setCursor(true);
          //MediaPlayer.Play(Resources.menuTheme);
          break;
        case GameState.Splashscreen:
          //SplashScreen.Trigger();
          break;
      }
    }


    void updateGame(GameTime gameTime)
    {
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      if (quit) exit();
      deviceState = new DeviceState(Mouse.GetState(), Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
      if (cursor.Show) cursor.Update(deviceState, prevDeviceState);
      switch (gameState)
      {
        case GameState.Menu:
          break;
        case GameState.Playing:
          if (Options.Config.Bindings[GameAction.Menu].IsControlPressed(deviceState, prevDeviceState)) //Pause
          {

            setCursor(true);
            Menu.GameMenu();
            setState(GameState.Menu);
          }
          GameMain.Update(gameTime, deviceState, prevDeviceState, cursor);
          break;
      }
      prevDeviceState = deviceState;
      prevDeviceState.mouse = Mouse.GetState(); //Recatch state
      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.Black);
      switch (gameState)
      {
        case GameState.Playing:
          GameMain.Draw(spriteBatch, gameTime, GraphicsDevice);
          break;
      }
      Desktop.Render();
      spriteBatch.Begin();
      cursor.Draw(spriteBatch);
      spriteBatch.End();
      base.Draw(gameTime);
    }

    public void exit()
    {
      if (networkClient != null) networkClient.disconnect();
      if (ClientV2.activeProcess != null) ClientV2.activeProcess.Kill();
      Exit();
    }
  }
}
