#if GAME
using FrozenPizza.Utils;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FrozenPizza.Entities
{
  public class Item
  {
    public int uid;
    public string name;
    public string id;
    public float weight;
    public Point size;

    public Point? position;

    public Item()
    {
#if GAME
    textures = new Dictionary<string, Texture2D>();
    sounds = new Dictionary<string, SoundEffect>();
#endif
    }

    public virtual void Copy(Item toCopy)
    {
      name = toCopy.name;
      id = toCopy.id;
      weight = toCopy.weight;
      size = toCopy.size;
#if GAME
      textures = toCopy.textures;
      sounds = toCopy.sounds;
#endif
    }

    public virtual void Init()
    {
    }
    
    /// <summary>
    /// Game specific logic
    /// </summary>
#if GAME
    public Dictionary<string, Texture2D> textures;
    public Dictionary<string, SoundEffect> sounds;

    public virtual bool use(Player player) { throw new Exception("BaseItem should be overriden"); }
    public virtual void drop() { }


    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Draw(SpriteBatch spriteBatch, Player player)
    {
      if (position == null) return; //Not on world
      spriteBatch.Draw(textures["world"], Tools.GetDrawRect(position.GetValueOrDefault().X * GameMain.map.tileSize.X, position.GetValueOrDefault().Y * GameMain.map.tileSize.Y, GameMain.map.tileSize),
                                          new Rectangle(0, 0, GameMain.map.tileSize.X, GameMain.map.tileSize.Y), Color.White);
    }
#endif
  }

}