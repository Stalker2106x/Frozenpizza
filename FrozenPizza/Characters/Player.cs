using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace FrozenPizza
{
    public enum PlayerSounds
    {
        Step1,
        Step2,
        Step3,
        Step4,
        RunStep1,
        RunStep2,
        RunStep3,
        RunStep4,
        Hurt,
        Die
    }

    public class Player : Character
    {
        public String name { get; set; }
        public int id { get; set; }

        //Constructor for remote players
        public Player(int vid, String vname, Vector2 vpos, int vhp = 100) : base(vhp)
        {
            name = vname;
            id = vid;
            _skinRect = new Rectangle(0, 0, 32, 16);
            pos = vpos;
        }

        //Reports damage from server to player
        public virtual void hurt(int damage)
        {
            hp.set(hp.get() - damage);
            _sounds[(int)PlayerSounds.Hurt].Play(Options.Config.SoundVolume, 0f, 0f);
            if (hp.get() == hp.min) die();
        }

        //Reports death from server to player
        public virtual void die()
        {
            _sounds[(int)PlayerSounds.Die].Play(Options.Config.SoundVolume, 0f, 0f);
            alive = false;
            _skinRect = new Rectangle(0, 64, 32, 64);
        }

        //Update player pos
        public virtual void Update(GameTime gameTime)
        {
        }

        //Draws the player
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
