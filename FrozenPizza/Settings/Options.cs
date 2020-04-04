using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FrozenPizza.Settings
{
  /// <summary>
  /// Game actions that needs to be bound by the player
  /// </summary>
  public enum GameAction
  {
    Fire,
    Aim,
    Reload,
    Forward,
    Backward,
    StrafeLeft,
    StrafeRight,
    Sprint,
    Use,
    Drop,
    ToggleInventory,
    Menu
  }

  /// <summary>
  /// Holds all the user system settings
  /// </summary>
  public class GameSettings
  {
    const string ConfigPath = "Content/Config.cfg";
    public bool Fullscreen { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public float MouseSensivity { get; set; }
    public float ControllerSensivity { get; set; }
    public float MusicVolume { get; set; }
    public float SoundVolume { get; set; }

    public Dictionary<GameAction, ControlPair> Bindings;
    public GameSettings()
    {
      DisplayMode defaultRes;
      try
      {
        defaultRes = Options.Resolutions.First((it) => { return (it.Width == 1280 && it.Height == 800); });
      }
      catch (Exception)
      {
        defaultRes = Options.Resolutions[0];
      }

      Fullscreen = false;
      Width = defaultRes.Width;
      Height = defaultRes.Height;
      MouseSensivity = 1.0f;
      ControllerSensivity = 8.0f;
      MusicVolume = 1.0f;
      SoundVolume = 1.0f;
      DefaultBindings();
    }

    public void DefaultBindings()
    {
      Bindings = new Dictionary<GameAction, ControlPair>();
      Bindings.Add(GameAction.Fire, new ControlPair(new Control(MouseButton.Left)));
      Bindings.Add(GameAction.Aim, new ControlPair(new Control(MouseButton.Right)));
      Bindings.Add(GameAction.Reload, new ControlPair(new Control(Keys.R)));
      Bindings.Add(GameAction.Use, new ControlPair(new Control(Keys.E)));
      Bindings.Add(GameAction.Forward, new ControlPair(new Control(Keys.Z)));
      Bindings.Add(GameAction.Backward, new ControlPair(new Control(Keys.S)));
      Bindings.Add(GameAction.StrafeLeft, new ControlPair(new Control(Keys.Q)));
      Bindings.Add(GameAction.StrafeRight, new ControlPair(new Control(Keys.D)));
      Bindings.Add(GameAction.Sprint, new ControlPair(new Control(Keys.LeftShift)));
      Bindings.Add(GameAction.Drop, new ControlPair(new Control(Keys.G)));
      Bindings.Add(GameAction.ToggleInventory, new ControlPair(new Control(Keys.Tab)));
      Bindings.Add(GameAction.Menu, new ControlPair(new Control(Keys.Escape)));
    }

    public static GameSettings Load()
    {
      if (!File.Exists(ConfigPath)) return (new GameSettings());
      return (JsonConvert.DeserializeObject<GameSettings>(File.ReadAllText(ConfigPath)));
    }
    public void Save()
    {
      File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(this));
    }
  }

  /// <summary>
  /// Holds all the user system settings
  /// </summary>
  public class Options
  {
    public static GraphicsDeviceManager GDevice { get; set; }
    public static GraphicsAdapter GAdapter { get; set; }
    public static List<DisplayMode> Resolutions { get; set; }

    public static GameSettings Config { get; set; }

    public Options(GraphicsDeviceManager gdevice, GraphicsAdapter gadapter)
    {
      GDevice = gdevice;
      GAdapter = gadapter;
      LoadResolutions();
      Config = GameSettings.Load();
      applyConfig();
    }

    public void LoadResolutions()
    {
      Resolutions = new List<DisplayMode>();
      foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
      {
        Resolutions.Add(mode);
      }
    }

    public static void applyConfig()
    {
      GDevice.PreferredBackBufferWidth = Config.Width;
      GDevice.PreferredBackBufferHeight = Config.Height;
      GDevice.ApplyChanges();
      GDevice.IsFullScreen = Config.Fullscreen;
      GDevice.ApplyChanges();
      MediaPlayer.Volume = Options.Config.MusicVolume;
    }

    public static List<String> getResolutions()
    {
      List<String> resolutions = new List<String>();

      for (int i = 0; i < Resolutions.Count; i++)
        resolutions.Add(Resolutions[i].Width + "x" + Resolutions[i].Height);
      return (resolutions);
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
  }
}
