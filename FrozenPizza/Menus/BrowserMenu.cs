using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    class BrowserMenu : Menu
    {
        public BrowserMenu(Engine engine) : base(engine, "BrowseMenu")
        {

        }

        public override void itemClicked(int index)
        {
            switch (index)
            {
                case 0:
                    break;
                case 1:
                    _engine.setMenu(new DirectConnectMenu(_engine, this));
                    break;
                case 3:
                    _engine.setMenu(new MainMenu(_engine));
                    break;
            }
        }
    }

    class DirectConnectMenu : Menu
    {
        BrowserMenu _bmenu;
        public DirectConnectMenu(Engine engine, BrowserMenu menu) : base(engine, "DirectConnectMenu")
        {
            _bmenu = menu;
        }

        public override void itemClicked(int index)
        {
            switch(index)
            {
                case 0:
                    //Connect
                    break;
                case 1:
                    _engine.setMenu(_bmenu);
                    break;
            }
        }
    }
}
