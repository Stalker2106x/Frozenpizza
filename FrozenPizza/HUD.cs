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
		Rectangle _handsPanel, _inventoryPanel;
        Vector2 _statsPos;
        Vector2 _foodPos;
		Vector2 _handsPos;
        Vector2 _hudOffset;
		Color[] _foodBackground;
        int _headsUpHeight, _headsUpWidth;

        //Bars
        Rectangle _cooldownBar;

        public HUD(GraphicsDevice graphics, Camera cam)
        {
            _hudOffset = new Vector2(64, 0);
            _headsUpHeight = 64;
            _headsUpWidth = 64;
			_cursorPos = new Vector2(cam.getViewport().Width / 2, cam.getViewport().Height / 4);
			_handsPanel = new Rectangle((cam.getViewport().Width / 2) - 125, cam.getViewport().Height - 90, 250, 80);
            _cooldownBar = new Rectangle((cam.getViewport().Width / 2) - 125, cam.getViewport().Height - 13, 0, 3);
            _inventoryPanel = new Rectangle(cam.getViewport().Width / 2, 20, cam.getViewport().Width / 2, cam.getViewport().Height - 140);
			_handsPos = new Vector2(_handsPanel.X, _handsPanel.Y);
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
			_foodBackground = new Color[2] { Color.White, Color.White };
            return (true);
        }

		public void Update(MouseState[] mStates, Player mainPlayer)
        {
			if (mainPlayer.checkState(PlayerStates.Hungry) && _foodBackground[0] != Color.Orange)
				_foodBackground[0] = Color.Orange;
			else if (mainPlayer.checkState(PlayerStates.Starving) && _foodBackground[0] != Color.Red)
				_foodBackground[0] = Color.Red;
			if (mainPlayer.checkState(PlayerStates.Thirsty) && _foodBackground[1] != Color.Orange)
				_foodBackground[1] = Color.Orange;
			else if (mainPlayer.checkState(PlayerStates.Dehydrated) && _foodBackground[1] != Color.Red)
				_foodBackground[1] = Color.Red;				
        }

        public Vector2 getHeadsUpHeight(int maxvalue, int value, bool offset)
        {
			Vector2 vec = new Vector2(0, (_headsUpHeight - (int)(((float)value / (float)maxvalue) * (float)_headsUpHeight)));
            if (offset)
                vec += _hudOffset;
            return (vec);
        }
        public Rectangle calcHeadsUpRect(int maxvalue, int value)
        {
			Rectangle rect = new Rectangle(_hudEntRect.X, _hudEntRect.Y + (int)getHeadsUpHeight(maxvalue, value, false).Y,
										   _headsUpWidth, value);
			
            _hudEntRect.X += 64;
        	return (rect);
		}

		private void DrawInventory(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Player mainPlayer)
		{
			//Background Panel
			DrawHudPanel(spriteBatch, graphicsDevice, _inventoryPanel, Color.Gray, 0.9f);

		}

		private void DrawHudPanel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Rectangle panel, Color color, float opacity)
		{
			var rect = new Texture2D(graphicsDevice, 1, 1);
			rect.SetData(new[] { color });
			spriteBatch.Draw(rect, panel, color * opacity);
		}

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Player mainPlayer)
        {
			_hudEntRect.X = 0;
            //Health
            spriteBatch.Draw(_hudEntities, _statsPos, _hudEntRect, Color.Gray, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(_hudEntities, _statsPos + getHeadsUpHeight(mainPlayer.maxHP, mainPlayer.HP, false), calcHeadsUpRect(mainPlayer.maxHP, mainPlayer.HP), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
			//Armor
			spriteBatch.Draw(_hudEntities, _statsPos + _hudOffset, _hudEntRect, Color.Gray, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
			spriteBatch.DrawString(_font, "999", _statsPos + _hudOffset, Color.White);
            _hudEntRect.X += 64;
			//Hunger
			spriteBatch.Draw(_hudEntities, _foodPos, _hudEntRect, Color.Gray, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(_hudEntities, _foodPos + getHeadsUpHeight(mainPlayer.maxHunger, mainPlayer.Hunger, false), calcHeadsUpRect(mainPlayer.maxHunger, mainPlayer.Hunger), _foodBackground[0], 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
			//Thirst
			spriteBatch.Draw(_hudEntities, _foodPos + _hudOffset, _hudEntRect, Color.Gray, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(_hudEntities, _foodPos + getHeadsUpHeight(mainPlayer.maxThirst, mainPlayer.Thirst, true), calcHeadsUpRect(mainPlayer.maxThirst, mainPlayer.Thirst), _foodBackground[1], 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
			//Hands Panel
			DrawHudPanel(spriteBatch, graphicsDevice, _handsPanel, Color.LightGray, 0.5f);
            _cooldownBar.Width = mainPlayer.Cooldown;
            DrawHudPanel(spriteBatch, graphicsDevice, _cooldownBar, Color.White, 0.75f);
            if (mainPlayer.Hands == null)
			    spriteBatch.DrawString(_font, "Hands", _handsPanel.Location.ToVector2(), Color.White);
            else
                spriteBatch.DrawString(_font, mainPlayer.Hands.Name, _handsPanel.Location.ToVector2(), Color.White);
            if (mainPlayer.InventoryOpen)
				DrawInventory(spriteBatch, graphicsDevice, mainPlayer);
			spriteBatch.Draw(_cursor, _cursorPos, _cursorRect, Color.White, 0, _cursorOrigin, 1.0f, SpriteEffects.None, 0);
		}
    }
}
