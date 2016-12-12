using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    class OptionsMenu : SettingMenu
    {
        Menu prevMenu;
        public OptionsMenu(Engine engine, Menu prevmenu) : base(engine, "OptionsMenu")
        {
            prevMenu = prevmenu;
        }

        public override void setSettingValues()
        {
            _settingValues[0] = new List<String>() { "Off", "On" };
            _settingValues[1] = Options.getResolutions();
            _settingValues[2] = new List<String>() { "0%", "25%", "50%", "75%", "100%" };
            _settingValues[3] = new List<String>() { "0%", "25%", "50%", "75%", "100%" };
        }

        public override void itemClicked(int index)
        {
            switch (index)
            {
                case 0:
                    Options.Config.Fullscreen = _settingSelectedValue[0] == 1 ? true : false;
                    Options.Config.DisplayMode = Options.getDisplayMode(_settingSelectedValue[1]);
                    Options.Config.MusicVolume = 0.25f * _settingSelectedValue[2];
                    Options.Config.SoundVolume = 0.25f * _settingSelectedValue[2];
                    Options.applyConfig();
                    initItems();
                    initSettings();
                    break;
                case 1:
                    Options.Config = new EngineSettings();
                    Options.applyConfig();
                    initItems();
                    initSettings();
                    break;
                case 2:
                    _engine.setMenu(prevMenu);
                    break;
            }
        }

    }
}
