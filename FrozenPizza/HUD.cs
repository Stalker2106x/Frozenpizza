using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenPizza
{
    public class HUD
    {

        SpriteFont _font;
        Texture2D _cursor, _hudEntities;
        Vector2 _cursorPos, _cursorOrigin;
        Rectangle _cursorRect, _hudEntRect;
        Vector2 _statsPos;
        Vector2 _foodPos;
        Vector2 _hudOffset;
        int _headsUpHeight, _headsUpWidth;

        public HUD(GraphicsDevice graphics, Camera cam)
        {
            _hudOffset = new Vector2(64, 0);
            _headsUpHeight = 64;
            _headsUpWidth = 64;
            _statsPos = new Vector2(0, cam.getViewport().Height - _headsUpHeight);
            _foodPos = new Vector2(cam.getViewport().Width - (2 * _headsUpWidth), cam.getViewport().Height - _headsUpHeight);
        }

        public bool Load(ContentManager content)
        {
            _cursorRect = new Rectangle(192, 0, 64, 64);
            _cursorOrigin = new Vector2(32, 32);
            _hudEntRect = new Rectangle(0, 0, 64, 64);
            _cursor = content.Load<Texture2D>(@"gfx/cursors");
            _hudEntities = content.Load<Texture2D>(@"gfx/hud");
            _font = content.Load<SpriteFont>(@"font/hud");
            return (true);
        }

        public void updateCursor(MouseState mState)
        {
            _cursorPos = new Vector2(mState.X, mState.Y);
        }
        public void Update(MouseState mState)
        {
            updateCursor(mState);
        }

        public Vector2 getHeadsUpHeight(int maxvalue, int value, bool offset)
        {
            Vector2 vec = new Vector2(0, (_headsUpHeight - (int)((value / maxvalue) * _headsUpHeight)));
            if (offset)
                vec += _hudOffset;
            return (vec);
        }
        public Rectangle calcHeadsUpRect(int maxvalue, int value)
        {
            return (new Rectangle(_hudEntRect.X, _hudEntRect.Y + (int)getHeadsUpHeight(maxvalue, value, false).Y,
                                  _headsUpWidth, value));
        }
        public void Draw(SpriteBatch spriteBatch, Player mainPlayer)
        {
            _hudEntRect.X = 0;
            spriteBatch.Draw(_cursor, _cursorPos, _cursorRect, Color.White, 0, _cursorOrigin, 1.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_hudEntities, _statsPos, _hudEntRect, Color.Gray, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(_hudEntities, _statsPos + getHeadsUpHeight(mainPlayer.maxHP, mainPlayer.HP, false), calcHeadsUpRect(mainPlayer.maxHP, mainPlayer.HP), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            _hudEntRect.X += 64;
            spriteBatch.Draw(_hudEntities, _statsPos + _hudOffset, _hudEntRect, Color.White);
            spriteBatch.DrawString(_font, "999", _statsPos + _hudOffset, Color.White);
            _hudEntRect.X += 64;
            spriteBatch.Draw(_hudEntities, _foodPos, _hudEntRect, Color.Gray, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(_hudEntities, _foodPos + getHeadsUpHeight(mainPlayer.maxHunger, mainPlayer.Hunger, false), calcHeadsUpRect(mainPlayer.maxHunger, mainPlayer.Hunger), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
            _hudEntRect.X += 64;
            spriteBatch.Draw(_hudEntities, _foodPos + _hudOffset, _hudEntRect, Color.Gray, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(_hudEntities, _foodPos + getHeadsUpHeight(mainPlayer.maxThirst, mainPlayer.Thirst, true), calcHeadsUpRect(mainPlayer.maxThirst, mainPlayer.Thirst), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
            _hudEntRect.X += 64;
        }
    }
}
