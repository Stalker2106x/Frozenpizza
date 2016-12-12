using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class CreateServerMenu : SettingMenu
    {
        public CreateServerMenu(Engine engine) : base(engine, "CreateServerMenu")
        {

        }

        public override void setSettingValues()
        {
            _settingValues[0] = new List<String>() { "Default FrozenPizza Server", "HELP!!!", "I'm too young to die", "Too late." };
            _settingValues[1] = new List<String>() { "1", "2", "5", "10", "25", "50" };
            _settingValues[2] = Level.getAvailableLevels();
        }

        public override void itemClicked(int index)
        {
            switch (index)
            {
                case 0:
                    NetHandler.startServer(_settingValues[2][_settingSelectedValue[2]]);
                    break;
                case 1:
                    _engine.setMenu(new MainMenu(_engine));
                    break;
            }
        }
    }
}
