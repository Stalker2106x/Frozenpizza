using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace FrozenPizza
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        enum GameState
        {
            Playing,
            Paused
        }

        //GAME
        GameState gstate;
        Camera cam;
        Level level;
        KeyBinds keybinds;
        Player mainPlayer;
        HUD hud;

        //Timers
        TimeSpan tMinute;

        public Engine()
        {
            IsMouseVisible = false;
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 90.0f);
            Content.RootDirectory = "Data";
            tMinute = new TimeSpan(0);
            gstate = GameState.Paused;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            level = new Level("Data/maps/world.tmx");
            cam = new Camera(GraphicsDevice);
            hud = new HUD(GraphicsDevice, cam);
            mainPlayer = new Player("Bernie");
            base.Initialize();
            gstate = GameState.Playing;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            level.Load(this.Content);
            hud.Load(this.Content);
            mainPlayer.Load(this.Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected void updateTimeEvents(GameTime gameTime)
        {
            tMinute += gameTime.ElapsedGameTime;
            //TODO: hunger and thirst over time
            if (tMinute.TotalMinutes >= 1)
            {
                mainPlayer.Hunger -= 2;
                mainPlayer.Thirst -= 1;
                tMinute = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gstate == GameState.Paused)
                return;
            KeyboardState keybState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            updateTimeEvents(gameTime);
            mainPlayer.Update(gameTime, level, keybState, mState, cam);
			hud.Update(mState, mainPlayer);
            base.Update(gameTime);
        }

        protected void DrawGame(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cam.getTransformation());
            level.Draw(spriteBatch, cam, mainPlayer);
            mainPlayer.Draw(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            hud.Draw(spriteBatch, mainPlayer);
            spriteBatch.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            switch (gstate)
            {
                case GameState.Playing:
                    DrawGame(gameTime);
                    break;
                case GameState.Paused:
                    break;
            }
            base.Draw(gameTime);
        }
    }
}
