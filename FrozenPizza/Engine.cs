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
            Menu,
            Playing,
            Paused
        }

        //GAME MECHANICS
        bool hasFocus;
        GameState gstate;
        Camera cam;
        KeyBinds keybinds;
        HUD hud;

        //Database
        public static Collection collection;
        //Game
        Level level;     
        Player mainPlayer;
        List<Projectile> projectiles;

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
			level = new Level("Data/maps/world.tmx");
			cam = new Camera(GraphicsDevice);
			hud = new HUD(GraphicsDevice, cam);
			keybStates = new KeyboardState[2];
			mouseStates = new MouseState[2];
            projectiles = new List<Projectile>();
			mainPlayer = new Player("Bernie", level.getSpawnPoint());
			collection = new Collection();
            base.Initialize();
            gstate = GameState.Playing;
        }
        protected override void OnDeactivated(Object sender, EventArgs args)
        {
            hasFocus = false;
            //call the base method and fire the event
            base.OnDeactivated(sender, args);
        }

        protected override void OnActivated(object sender, EventArgs args)
        {          
            base.OnActivated(sender, args);
            hasFocus = true;
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

        void updateGame(GameTime gameTime)
        {
            updateTimeEvents(gameTime);
            mainPlayer.Update(gameTime, level, keybStates, mouseStates, cam, projectiles);
            hud.Update(mouseStates, mainPlayer);
            for (int p = 0; p < projectiles.Count; p++)
                if (!projectiles[p].Update(level))
                    projectiles.RemoveAt(p);
            IsMouseVisible = mainPlayer.InventoryOpen;
            base.Update(gameTime);
        }

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (!hasFocus)
				return;
			keybStates[1] = Keyboard.GetState();
			mouseStates[1] = Mouse.GetState();

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
            updateGame(gameTime);
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
            for (int p = 0; p < projectiles.Count; p++)
                projectiles[p].Draw(spriteBatch, collection);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
			hud.Draw(spriteBatch, GraphicsDevice, mainPlayer, collection, cam);
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
                case GameState.Menu:
                    break;
                case GameState.Playing:
                    DrawGame(gameTime);
                    break;
            }
            base.Draw(gameTime);
        }

		public static void DrawLine(SpriteBatch spriteBatch, Texture2D text, Vector2 begin, Vector2 end, Color color, int width)
		{
			Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
			Vector2 v = Vector2.Normalize(begin - end);
			float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
			if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
			spriteBatch.Draw(text, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
		}
    }
}
