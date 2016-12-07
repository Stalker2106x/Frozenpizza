using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace FrozenPizza
{
    public class Engine : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public enum GameState
        {
            Menu,
            Playing,
        }

        //GAME MECHANICS
        bool hasFocus;
        public GameState gstate { get; set; }
        Camera cam;
        KeyBinds keybinds;
        HUD hud;

        //Database & Netcode
        public static NetHandler netHandle;
        public static Collection collection;

        //Menu
        Cursor _cursor;
        Menu _menu;
        bool _gameLoaded;

        //Game
        static Level level;
        static MainPlayer mainPlayer;
        static Dictionary<int, Player> players;

        //Input
        KeyboardState[] keybStates;
        MouseState[] mouseStates;

        //Timers
        TimeSpan tMinute;

		//Static accessors
        public static MainPlayer MainPlayer { get { return (mainPlayer); } }
        public static Dictionary<int, Player> Players { get { return (players); } }
        public static Level Level { get { return (level); } }

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 90.0f);
            Content.RootDirectory = "Data";
            tMinute = new TimeSpan(0);
            gstate = GameState.Menu;
            _gameLoaded = false;
        }

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
            IsMouseVisible = false;
            _cursor = new Cursor();
            keybStates = new KeyboardState[2];
			mouseStates = new MouseState[2];
			collection = new Collection();
            base.Initialize();
        }

        public void InitializeGame()
        {
            level = new Level("Data/maps/world.tmx");
            cam = new Camera(GraphicsDevice);
            hud = new HUD(GraphicsDevice, cam);
            mainPlayer = new MainPlayer("Bernie");
        }

		/// <summary>
		/// When losing and getting back focus of window
		/// </summary>
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
            _cursor.Load(this.Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            collection.Load(this.Content);
            setMenu(new MainMenu(this));
            _menu.Load(this.Content);
        }

        public void LoadGame()
        {
            level.Load(this.Content);
            hud.Load(this.Content);
            mainPlayer.Load(this.Content);
            //level.GenerateItems(collection);
            _gameLoaded = true;
        }

        public void UnloadGame()
        {
            level = null;
            hud = null;
            mainPlayer = null;
            _gameLoaded = false;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void setMenu(Menu menu)
        {
            _menu = menu;
            _menu.Load(this.Content);
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

        public void toggleMouseVisible()
        {
            _cursor.Show = _cursor.Show == true ? false : true;
        }
		void resetMousePos()
		{
			Mouse.SetPosition(cam.getViewport().Width / 2, cam.getViewport().Height / 2);
		}

        void updateGame(GameTime gameTime)
        {
            if (keybStates[0].IsKeyUp(Keys.Escape) && keybStates[1].IsKeyDown(Keys.Escape))
            {
                mainPlayer.InventoryOpen = false;
                toggleMouseVisible();
                gstate = GameState.Menu;
            }
            updateTimeEvents(gameTime);
            mainPlayer.Update(gameTime, level, keybStates, mouseStates, cam, _cursor);
            hud.Update(mouseStates, mainPlayer);
            if (!mainPlayer.InventoryOpen)
                resetMousePos();
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
            if (_cursor.Show)
                _cursor.Update(mouseStates);
            switch (gstate)
            {
                case GameState.Menu:
                    _menu.Update(keybStates, mouseStates);
                    break;
                case GameState.Playing:
                    updateGame(gameTime);
                    break;
            }
            base.Update(gameTime);
            keybStates[0] = keybStates[1];
			mouseStates[0] = Mouse.GetState();
        }

        protected void DrawGame(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cam.getTransformation());
            level.Draw(spriteBatch, cam, mainPlayer, collection);
            mainPlayer.Draw(spriteBatch);
            spriteBatch.End();
			//dummy
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
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    _menu.Draw(spriteBatch, GraphicsDevice);
                    spriteBatch.End();
                    break;
                case GameState.Playing:
                    DrawGame(gameTime);
                    break;
            }
            base.Draw(gameTime);
            if (_cursor.Show)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                _cursor.Draw(spriteBatch);
                spriteBatch.End();
            }
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
            Exit();
        }
    }
}
