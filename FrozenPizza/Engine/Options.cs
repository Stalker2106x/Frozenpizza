using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class EngineSettings
    {
        public bool Fullscreen { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float MusicVolume { get; set; }
        public float SoundVolume { get; set; }

        public EngineSettings()
        {
            Fullscreen = false;
            Width = Options.Resolutions[0].Width;
            Height = Options.Resolutions[0].Height;
            MusicVolume = 1.0f;
            SoundVolume = 1.0f;
        }

        public EngineSettings(bool fullscreen, int width, int height, float musicVolume, float soundVolume)
        {
            Fullscreen = fullscreen;
            Width = width;
            Height = height;
            MusicVolume = musicVolume;
            SoundVolume = soundVolume;
        }
    }

    public class Options
    {
        static String _configfile;
        public static Engine Engine { get; set; }
        public static GraphicsDeviceManager GDevice { get; set; }
        public static GraphicsAdapter GAdapter { get; set; }
        public static List<DisplayMode> Resolutions { get; set; }

        public static EngineSettings Config { get; set; }

        public Options(Engine engine, GraphicsDeviceManager gdevice, String configFile)
        {
            _configfile = configFile;
            Engine = engine;
            GDevice = gdevice;
            Resolutions = new List<DisplayMode>();
        }

        public bool Load(GraphicsAdapter gadapter)
        {
            GAdapter = gadapter;
            LoadResolutions();
            Config = new EngineSettings();
            LoadConfigFile();
            return (false);
        }

        public void LoadConfigFile()
        {
            StreamReader file;
            String line;

            try { file = new StreamReader(_configfile); }
            catch(System.IO.IOException e)
            {
                return;
            }
            while ((line = file.ReadLine()) != null)
            {
                setSetting(line.Substring(0, line.IndexOf('=')), line.Substring(line.IndexOf('=') + 1, line.Length - line.IndexOf('=') - 1));
            }
            file.Close();
            applyConfig();
        }

        public static void SetConfigFile()
        {
            FileStream file;
            String[] lines;

            lines = File.ReadAllLines(_configfile);
            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Contains("fullscreen"))
                    lines[i] = "fullscreen=" + (Config.Fullscreen == true ? "true" : "false");
                else if (lines[i].Contains("width"))
                    lines[i] = "width=" + Config.Width;
                else if (lines[i].Contains("height"))
                    lines[i] = "width=" + Config.Height;
                else if (lines[i].Contains("musicvolume"))
                    lines[i] = "musicvolume=" + Config.MusicVolume;
                else if (lines[i].Contains("soundvolume"))
                    lines[i] = "soundvolume=" + Config.SoundVolume;
            }
            File.WriteAllLines(_configfile, lines);
        }

        public void setSetting(String setting, String value)
        {
            switch(setting)
            {
                case "fullscreen":
                    Config.Fullscreen = (value == "true" ? true : false);
                    break;
                case "width":
                    int width;

                    Int32.TryParse(value, out width);
                    Config.Width = width;
                    break;
                case "height":
                    int height;

                    Int32.TryParse(value, out height);
                    Config.Height = height;
                    break;
                case "soundvolume":
                    float soundvol;

                    float.TryParse(value, out soundvol);
                    Config.SoundVolume = soundvol;
                    break;
                case "musicvolume":
                    float musicvol;

                    float.TryParse(value, out musicvol);
                    Config.MusicVolume = musicvol;
                    break;
            }
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

            setResolution();
            GDevice.ApplyChanges();
            GDevice.IsFullScreen = Config.Fullscreen;
            GDevice.ApplyChanges();
        }

        public static List<String> getResolutions()
        {
            List<String> resolutions = new List<String>();

            for (int i = 0; i < Resolutions.Count; i++)
                resolutions.Add(Resolutions[i].Width + "x" + Resolutions[i].Height);
            return (resolutions);
        }

        public static DisplayMode getDisplayModeById(int id)
        {
            return (Resolutions[id]);
        }

        public static int getDisplayMode()
        {
            for (int i = 0; i < Resolutions.Count; i++)
            {
                if (Resolutions[i].Width == Config.Width && Resolutions[i].Height == Config.Height)
                    return (i);
            }
            return (0);
        }

        public static void setResolution()
        {
            GDevice.PreferredBackBufferWidth = Config.Width;
            GDevice.PreferredBackBufferHeight = Config.Height;
        }

        public static void setFullscreen(bool active)
        {
            GDevice.IsFullScreen = active;
        }
    }
}
