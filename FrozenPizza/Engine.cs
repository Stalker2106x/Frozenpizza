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
		Collection collection;
        HUD hud;

		//Input
		KeyboardState[] keybStates;
		MouseState[] mouseStates;

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
			level = new Level("Data/maps/dev.tmx");
			cam = new Camera(GraphicsDevice);
			hud = new HUD(GraphicsDevice, cam);
			keybStates = new KeyboardState[2];
			mouseStates = new MouseState[2];
			mainPlayer = new Player("Bernie", level.getSpawnPoint());
			collection = new Collection();
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
            collection.Load(this.Content);
            level.Load(this.Content);
            hud.Load(this.Content);
            mainPlayer.Load(this.Content);
            level.GenerateItems(collection);
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


		void resetMousePos()
		{
			Mouse.SetPosition(cam.getViewport().Width / 2, cam.getViewport().Height / 2);
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
			keybStates[1] = Keyboard.GetState();
			mouseStates[1] = Mouse.GetState();

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
			updateTimeEvents(gameTime);
			mainPlayer.Update(gameTime, level, keybStates, mouseStates, cam);
            hud.Update(mouseStates, mainPlayer);
            IsMouseVisible = mainPlayer.InventoryOpen;
			base.Update(gameTime);
			keybStates[0] = keybStates[1];
            if (!mainPlayer.InventoryOpen)
			    resetMousePos();
			mouseStates[0] = Mouse.GetState();
        }

        protected void DrawGame(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cam.getTransformation());
            level.Draw(spriteBatch, cam, mainPlayer, collection);
            mainPlayer.Draw(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            hud.Draw(spriteBatch, GraphicsDevice, mainPlayer, collection);
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
