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
            _settingSelectedValue[0] = (Options.Config.Fullscreen ? 1 : 0);
            _settingValues[1] = Options.getResolutions();
            _settingSelectedValue[1] = Options.getDisplayMode();
            _settingValues[2] = new List<String>() { "0%", "25%", "50%", "75%", "100%" };
            _settingSelectedValue[2] = (int)(Options.Config.MusicVolume / 0.25f);
            _settingValues[3] = new List<String>() { "0%", "25%", "50%", "75%", "100%" };
            _settingSelectedValue[3] = (int)(Options.Config.MusicVolume / 0.25f);
        }

        public override void itemClicked(int index)
        {
            switch (index)
            {
                case 0:
                    Options.Config.Fullscreen = _settingSelectedValue[0] == 1 ? true : false;
                    Options.Config.Width = Options.getDisplayModeById(_settingSelectedValue[1]).Width;
                    Options.Config.Height = Options.getDisplayModeById(_settingSelectedValue[1]).Height;
                    Options.Config.MusicVolume = 0.25f * _settingSelectedValue[2];
                    Options.Config.SoundVolume = 0.25f * _settingSelectedValue[2];
                    Options.applyConfig();
                    initItems();
                    initSettings();
                    Options.SetConfigFile();
                    break;
                case 1:

                    break;
                case 2:
                    Options.Config = new EngineSettings();
                    Options.applyConfig();
                    Options.SetConfigFile();
                    initItems();
                    initSettings();
                    break;
                case 3:
                    _engine.setMenu(prevMenu);
                    break;
            }
        }

    }
}
