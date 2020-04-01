using FrozenPizza.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Myra;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;

namespace FrozenPizza
{
  public class Engine : Game
  {
    private static DeviceState deviceState;
    private static DeviceState prevDeviceState;
    private static GameState gameState;
    public static bool quit;
    public static Options options;

    //Base Monogame Graphics
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    //GAME MECHANICS
    bool hasFocus;

    //Menu & Cursor
    private Cursor _cursor;

    public Engine()
    {
      graphics = new GraphicsDeviceManager(this);
      options = new Options(graphics, GraphicsAdapter.DefaultAdapter);
      TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 90.0f);
      Content.RootDirectory = "Data";
      setState(GameState.Menu);
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
        IsMouseVisible = true;
        _cursor = new Cursor();
        base.Initialize();
    }

	  /// <summary>
	  /// When losing and getting back focus of window
	  /// </summary>
	  protected override void OnDeactivated(Object sender, EventArgs args)
    {
        hasFocus = false;
        IsMouseVisible = true;
        //call the base method and fire the event
        base.OnDeactivated(sender, args);
    }

    protected override void OnActivated(object sender, EventArgs args)
    {          
        base.OnActivated(sender, args);
        IsMouseVisible = false;
        hasFocus = true;
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      _cursor.Load(Content);
      spriteBatch = new SpriteBatch(GraphicsDevice);
      Resources.Load(Content);

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
    public static void setState(GameState state)
    {
      gameState = state;
      switch (state)
      {
        case GameState.Playing:
         // toggleMouseVisible();
         // GameMain.hud.activate();
          break;
        case GameState.Menu:
          //MediaPlayer.Play(Resources.menuTheme);
          break;
        case GameState.Splashscreen:
          SplashScreen.Trigger();
          break;
      }
    }

    public static DeviceState getDeviceState()
    {
      return (deviceState);
    }

    public void toggleMouseVisible()
    {
        _cursor.Show = _cursor.Show == true ? false : true;
    }

	  /// <summary>
	  /// Allows the game to run logic such as updating the world,
	  /// checking for collisions, gathering input, and playing audio.
	  /// </summary>
	  /// <param name="gameTime">Provides a snapshot of timing values.</param>
	  protected override void Update(GameTime gameTime)
	  {
      deviceState = new DeviceState(Mouse.GetState(), Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
      if (_cursor.Show) _cursor.Update(deviceState, prevDeviceState);
      switch (gameState)
      {
          case GameState.Menu:
              if (!hasFocus)
                  return;
              break;
          case GameState.Playing:
              GameMain.Update(gameTime, deviceState, prevDeviceState);
              break;
      }
      prevDeviceState = deviceState;
      base.Update(gameTime);
    }

    protected void DrawGame(GameTime gameTime)
    {
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
            case GameState.Menu:
                //DrawGame(gameTime); //Draw Game Anyway behind
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                spriteBatch.End();
                break;
            case GameState.Playing:
                DrawGame(gameTime);
                break;
        }
        if (_cursor.Show)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            _cursor.Draw(spriteBatch);
            spriteBatch.End();
      }
      base.Draw(gameTime);
      Desktop.Render();
    }

	  public static void DrawLine(SpriteBatch spriteBatch, Texture2D text, Vector2 begin, Vector2 end, Color color, int width)
	  {
		  Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
		  Vector2 v = Vector2.Normalize(begin - end);
		  float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
		  if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
		  spriteBatch.Draw(text, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
	  }

    public void exit()
    {
      if (GameMain.netHandle != null)
      {
          NetHandler.disconnect();
          GameMain.netHandle = null;
      }
      Exit();
    }
  }
}
