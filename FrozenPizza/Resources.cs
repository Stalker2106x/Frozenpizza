using FrozenPizza.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace FrozenPizza
{
  public static class Resources
  {
    public static Texture2D GameLogo;
    public static Texture2D MenuBackground;

    public static SoundEffect[] MenuSounds;
    public static SoundEffect Dryfire;
    //public static Texture2D[] Tilesets;
    public static Texture2D Projectiles;

    public static Texture2D Pixel;
    public static Texture2D projectile;
    public static Texture2D Players;
    public static SoundEffect[] PlayersSound;

    public static Dictionary<string, Texture2D> ItemTextures;

    //HUD
    public static Texture2D hudEntities;
    public static SpriteFont regularFont;
    public static SpriteFont titleFont;

    private static ContentManager _content;

    public static bool Load(ContentManager content, GraphicsDevice graphics)
    {
      _content = content;
      MenuSounds = new SoundEffect[2];
      //Game
      Pixel = new Texture2D(graphics, 1, 1, false, SurfaceFormat.Color);
      Pixel.SetData(new[] { Color.White });

      regularFont = content.Load<SpriteFont>("font/regular");
      titleFont = content.Load<SpriteFont>("font/title");

      GameLogo = content.Load<Texture2D>("gfx/logo");
      MenuBackground = content.Load<Texture2D>("gfx/bg/main");
      MenuSounds[0] = content.Load<SoundEffect>("sounds/menu/hover");
      MenuSounds[1] = content.Load<SoundEffect>("sounds/menu/click");

      //HUD
      hudEntities = content.Load<Texture2D>(@"gfx/hud");

      //Game
      projectile = content.Load<Texture2D>(@"gfx/projectile");
      LoadPlayers(content);
      EntityStore.LoadGameMelee(content);
      EntityStore.LoadGameRanged(content);

      return (true);
    }

    public static bool LoadPlayers(ContentManager content)
    {
      Dryfire = content.Load<SoundEffect>("sounds/weapons/dryfire");
      Players = content.Load<Texture2D>("gfx/players");
      PlayersSound = new SoundEffect[Enum.GetNames(typeof(PlayerSounds)).Length];
      PlayersSound[(int)PlayerSounds.Step1] = content.Load<SoundEffect>("sounds/player/step1");
      PlayersSound[(int)PlayerSounds.Step2] = content.Load<SoundEffect>("sounds/player/step2");
      PlayersSound[(int)PlayerSounds.Step3] = content.Load<SoundEffect>("sounds/player/step3");
      PlayersSound[(int)PlayerSounds.Step4] = content.Load<SoundEffect>("sounds/player/step4");
      PlayersSound[(int)PlayerSounds.RunStep1] = content.Load<SoundEffect>("sounds/player/rstep1");
      PlayersSound[(int)PlayerSounds.RunStep2] = content.Load<SoundEffect>("sounds/player/rstep2");
      PlayersSound[(int)PlayerSounds.RunStep3] = content.Load<SoundEffect>("sounds/player/rstep3");
      PlayersSound[(int)PlayerSounds.RunStep4] = content.Load<SoundEffect>("sounds/player/rstep4");
      PlayersSound[(int)PlayerSounds.Hurt] = content.Load<SoundEffect>("sounds/player/hurt");
      PlayersSound[(int)PlayerSounds.Die] = content.Load<SoundEffect>("sounds/player/die");
      return (true);
    }

    public static Texture2D LoadTileset(string tilesetName)
    {
      return (_content.Load<Texture2D>("maps/"+tilesetName));
    }
  }
}
