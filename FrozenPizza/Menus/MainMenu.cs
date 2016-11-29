using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    class MainMenu : Menu
    {
        public MainMenu(Engine engine) : base(engine, "MainMenu")
        {

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
                    break;
                case 2:
                    break;
                case 3:
                    _engine.exit();
                    break;
            }
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
                    _engine.gstate = Engine.GameState.Playing;
                    break;
                case 1:
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
