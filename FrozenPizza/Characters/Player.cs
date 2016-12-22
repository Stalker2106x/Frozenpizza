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
        //Constructor for players
        public Player(int id, String name, Vector2 pos) : base(name)
        {
            Id = id;
            _origin = new Vector2(16, 8);
            _skinRect = new Rectangle(0, 0, 32, 16);
            _pos = pos;
        }

        //Constructor for remote players
        public Player(int id, String name, Vector2 pos, int hp) : base(name)
        {
            Id = id;
            HP = hp;
            _origin = new Vector2(16, 8);
            _skinRect = new Rectangle(0, 0, 32, 16);
            _pos = pos;
        }

        //Reports damage from server to player
        public virtual void hurt(int damage)
        {
            HP -= damage;
            _sounds[(int)PlayerSounds.Hurt].Play(Options.Config.SoundVolume, 0f, 0f);
        }

        //Reports death from server to player
        public virtual void die()
        {
            HP = 0;
            _sounds[(int)PlayerSounds.Die].Play(Options.Config.SoundVolume, 0f, 0f);
            _alive = false;
            _skinRect = new Rectangle(0, 64, 32, 64);
        }

        //Update player pos
        public virtual void Update(GameTime gameTime)
        {
            Vector2 syncVector = new Vector2();

            syncVector.X = (float)(_move.X * gameTime.ElapsedGameTime.TotalSeconds);
            syncVector.Y = (float)(_move.Y * gameTime.ElapsedGameTime.TotalSeconds);
            Pos += syncVector;
        }

        //Draws the player
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
