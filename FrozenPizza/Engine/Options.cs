using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class EngineSettings
    {
        public bool Fullscreen { get; set; }
        public DisplayMode DisplayMode { get; set; }
        public float MusicVolume { get; set; }
        public float SoundVolume { get; set; }

        public EngineSettings()
        {
            Fullscreen = false;
            DisplayMode = Options.Resolutions[0];
            MusicVolume = 1.0f;
            SoundVolume = 1.0f;
        }
    }

    public class Options
    {
        public static Engine Engine { get; set; }
        public static GraphicsDeviceManager GDevice { get; set; }
        public static GraphicsAdapter GAdapter { get; set; }
        public static List<DisplayMode> Resolutions { get; set; }

        public static EngineSettings Config { get; set; }

        public Options(Engine engine, GraphicsDeviceManager gdevice)
        {
            Engine = engine;
            GDevice = gdevice;
            Resolutions = new List<DisplayMode>();
        }

        public bool Load(GraphicsAdapter gadapter)
        {
            GAdapter = gadapter;
            LoadResolutions();
            Config = new EngineSettings();
            return (false);
        }

        public void LoadResolutions()
        {
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                Resolutions.Add(mode);
            }
        }

        public static void applyConfig()
        {
            GDevice.IsFullScreen = Config.Fullscreen;
            setResolution(Config.DisplayMode);
            GDevice.ApplyChanges();
        }

        public static List<String> getResolutions()
        {
            List<String> resolutions = new List<String>();

            for (int i = 0; i < Resolutions.Count; i++)
                resolutions.Add(Resolutions[i].Width + "x" + Resolutions[i].Height);
            return (resolutions);
        }

        public static DisplayMode getDisplayMode(int index)
        {
            return (Resolutions[index]);
        }

        public static void setResolution(DisplayMode mode)
        {
            GDevice.PreferredBackBufferWidth = mode.Width;
            GDevice.PreferredBackBufferHeight = mode.Height;
        }

        public static void setFullscreen(bool active)
        {
            GDevice.IsFullScreen = active;
        }
    }
}
