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
        Menu,
        Playing,
    }

    public class Engine : Game
    {
        //Base Monogame Graphics
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //GAME MECHANICS
        bool hasFocus;
        public GameState gstate { get; set; }
        Camera cam;
        HUD hud;

        //Database, Config and Netcode
        public static NetHandler netHandle;
        public static Collection collection;
        public static Options options;

        //Menu & Cursor
        private Cursor _cursor;
        //Menu _menu;

        //Game
        static Level level;
        public static MainPlayer mainPlayer { get; set; }
        static List<Player> players;

        //Input management
        KeyBinds keybinds;
        KeyboardState[] keybStates;
        MouseState[] mouseStates;

		//Static accessors
        public static List<Player> Players { get { return (players); } }
        public static Player getPlayerById(int id)
        {
            if (players == null)
                return (null);
            for (int i = 0; i < players.Count; i++)
                if (players[i].id == id)
                    return (players[i]);
            return (null);
        }
        public static Level Level { get { return (level); } }

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            options = new Options(graphics, GraphicsAdapter.DefaultAdapter);
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 90.0f);
            Content.RootDirectory = "Data";
            gstate = GameState.Menu;
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
            IsMouseVisible = false;
            _cursor = new Cursor();
            InitializeGraphics();
            keybinds = new KeyBinds();
            keybStates = new KeyboardState[2];
			      mouseStates = new MouseState[2];
			      collection = new Collection();
            base.Initialize();
        }

        public void InitializeGraphics()
        {
            //desktop.Bounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        public void InitializeGame()
        {
            cam = new Camera(GraphicsDevice);
            hud = new HUD(GraphicsDevice, cam);
            mainPlayer = new MainPlayer("Bernie");
            players = new List<Player>();
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
            _cursor.Load(this.Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            collection.Load(this.Content);
            Menu.Init(this.Content);
            Menu.MainMenu(this);
        }

        public void LoadGame()
        {
            level = new Level();
            level.Load(this.Content);
            hud.Load(this.Content);
            mainPlayer.Load(this.Content);
            //level.GenerateItems(collection);
        }

        public void UnloadGame()
        {
            level = null;
            hud = null;
            mainPlayer = null;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
            if (keybStates[0].IsKeyUp(Keys.Escape) && keybStates[1].IsKeyDown(Keys.Escape)) //Pause
            {
                mainPlayer.inventoryOpen = false;
                if (_cursor.Show == false)
                    toggleMouseVisible();
                gstate = GameState.Menu;
            }
            level.Update(); //Update world
            mainPlayer.Update(gameTime, level, keybStates, mouseStates, cam, _cursor);
            for (int i = 0; i < players.Count; i++)
                players[i].Update(gameTime);
            hud.Update(mouseStates, mainPlayer);
            if (mainPlayer.alive && !mainPlayer.inventoryOpen) //If we are ingame reset mouse each loop
                resetMousePos();
        }

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			keybStates[1] = Keyboard.GetState();
			mouseStates[1] = Mouse.GetState();
            if (_cursor.Show)
                _cursor.Update(mouseStates);
            switch (gstate)
            {
                case GameState.Menu:
                    if (!hasFocus)
                        return;
                    //_menu.Update(keybStates, mouseStates);
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
            for (int i = 0; i < players.Count; i++) //Draw players
            {
                players[i].Draw(spriteBatch);
            }
            spriteBatch.End();
            if (gstate == GameState.Playing) //Draw HUD
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                hud.Draw(spriteBatch, GraphicsDevice, mainPlayer, collection, cam);
                spriteBatch.End();
            }
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
                    //DrawGame(gameTime); //Draw Game Anyway behind
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    Desktop.Render();
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
            if (netHandle != null)
            {
                NetHandler.disconnect();
                netHandle = null;
            }
            Exit();
        }
    }
}
