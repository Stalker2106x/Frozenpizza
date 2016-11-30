using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    class MainMenu : Menu
    {
        Rectangle _logoRect;

        public MainMenu(Engine engine) : base(engine, "MainMenu")
        {
            _logoRect = new Rectangle((_engine.GraphicsDevice.Viewport.Width - Engine.collection.GameLogo.Width) / 2,
                            (int)(_engine.GraphicsDevice.Viewport.Height * 0.25f),
                            Engine.collection.GameLogo.Width, Engine.collection.GameLogo.Height);
        }

        public override void itemClicked(int index)
        {
            switch (index)
            {
                case 0:
                    _engine.InitializeGame();
                    _engine.LoadGame();
                    _engine.gstate = Engine.GameState.Playing;
                    _engine.toggleMouseVisible();
                    _engine.setMenu(new GameMenu(_engine));
                    break;
                case 1:
                    _engine.setMenu(new BrowserMenu(_engine));
                    break;
                case 2:
                    break;
                case 3:
                    _engine.exit();
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            spriteBatch.Draw(Engine.collection.GameLogo, _logoRect, Color.White);
            base.Draw(spriteBatch, graphics);
        }

    }

    class GameMenu : Menu
    {
        public GameMenu(Engine engine) : base(engine, "GameMenu")
        {

        }

        public override void itemClicked(int index)
        {
            switch (index)
            {
                case 0:
                    _engine.toggleMouseVisible();
                    _engine.gstate = Engine.GameState.Playing;
                    break;
                case 1:
                    _engine.UnloadGame();
                    _engine.setMenu(new MainMenu(_engine));
                    break;
                case 2:
                    break;
                case 3:
                    _engine.exit();
                    break;
            }
        }
    }
}
