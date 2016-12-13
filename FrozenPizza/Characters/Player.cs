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
        public Player(int id, String name, Vector2 pos) : base(name)
        {
            Id = id;
            _origin = new Vector2(16, 8);
            _skinRect = new Rectangle(0, 0, 32, 16);
            _pos = pos;
        }
        public Player(int id, String name, Vector2 pos, int hp) : base(name)
        {
            Id = id;
            HP = hp;
            _origin = new Vector2(16, 8);
            _skinRect = new Rectangle(0, 0, 32, 16);
            _pos = pos;
        }

        public virtual void hurt(int damage)
        {
            HP -= damage;
            _sounds[(int)PlayerSounds.Hurt].Play(Options.Config.SoundVolume, 0f, 0f);
        }

        public virtual void die()
        {
            HP = 0;
            _sounds[(int)PlayerSounds.Die].Play(Options.Config.SoundVolume, 0f, 0f);
            _alive = false;
            _skinRect = new Rectangle(0, 64, 32, 64);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
