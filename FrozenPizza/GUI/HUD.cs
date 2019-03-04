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
		//Text
        SpriteFont _font;

		//Stats
		Texture2D _hudEntities;
		Rectangle _hudEntRect;

		//StatsPanel
        Vector2 _statsPos;

		//HandsPanel
		Vector2 _handsPos;
		Rectangle _handsPanel;

		//FoodPanel
		Vector2 _foodPos;
		Color[] _foodBackground;

		//Common
		int _headsUpHeight, _headsUpWidth;
		Vector2 _hudOffset;
		Texture2D _colorRect;

		//Inventory
		Rectangle _inventoryPanel;

        //Bars
        Rectangle _cooldownBar;

        public HUD(GraphicsDevice graphics, Camera cam)
        {
			//Common
            _hudOffset = new Vector2(64, 0);
            _headsUpHeight = 64;
            _headsUpWidth = 64;
			_colorRect = new Texture2D(graphics, 1, 1);
			_colorRect.SetData(new[] { Color.White });
			//HandsPanel
			_handsPanel = new Rectangle((cam.getViewport().Width / 2) - 125, cam.getViewport().Height - 90, 250, 80);
			_handsPos = new Vector2(_handsPanel.X, _handsPanel.Y);
			//Bars
            _cooldownBar = new Rectangle((cam.getViewport().Width / 2) - 125, cam.getViewport().Height - 13, 0, 3);
			//Inventory
            _inventoryPanel = new Rectangle(cam.getViewport().Width / 2, 20, cam.getViewport().Width / 2, cam.getViewport().Height - 140);
			//StatsPanel
            _statsPos = new Vector2(0, cam.getViewport().Height - _headsUpHeight);
			//FoodPanel
            _foodPos = new Vector2(cam.getViewport().Width - (2 * _headsUpWidth), cam.getViewport().Height - _headsUpHeight);
        }

		public bool Load(ContentManager content)
        {
            _hudEntRect = new Rectangle(0, 0, 64, 64);
            _hudEntities = content.Load<Texture2D>(@"gfx/hud");
            _font = content.Load<SpriteFont>(@"font/army");
			_foodBackground = new Color[2] { Color.White, Color.White };
            return (true);
        }

		public void Update(MouseState[] mStates, MainPlayer mainPlayer)
        {
            if (mainPlayer.cooldown)
                _cooldownBar.Width = mainPlayer.getCooldownPercent(_handsPanel.Width);
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

		private void DrawInventory(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, MainPlayer mainPlayer)
		{
			//Background Panel
			DrawHudPanel(spriteBatch, graphicsDevice, _inventoryPanel, Color.Gray, 0.9f);

		}

		private void DrawHudPanel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Rectangle panel, Color color, float opacity)
		{
			spriteBatch.Draw(_colorRect, panel, null, color * opacity, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
		}

		public void drawHandsPanel(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, MainPlayer mainPlayer, Collection collection)
		{
			DrawHudPanel(spriteBatch, graphicsDevice, _handsPanel, Color.LightGray, 0.5f);
			if (mainPlayer.cooldown)
				DrawHudPanel(spriteBatch, graphicsDevice, _cooldownBar, Color.White, 0.75f);
            if (mainPlayer.hands == null)
            {
                spriteBatch.DrawString(_font, "Hands", _handsPanel.Location.ToVector2(), Color.White);
                spriteBatch.Draw(collection.Tilesets[(int)ItemType.Melee], new Vector2(_handsPanel.X + _handsPanel.Width / 2, _handsPanel.Y + _handsPanel.Height / 2), new Rectangle(0, 0, 32, 32), Color.White, 0f, new Vector2(16, 16), 1.0f, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.DrawString(_font, mainPlayer.hands.name, _handsPanel.Location.ToVector2(), Color.White);
                spriteBatch.Draw(collection.Tilesets[(int)mainPlayer.hands.type], new Vector2(_handsPanel.X + _handsPanel.Width / 2, _handsPanel.Y + _handsPanel.Height / 2), mainPlayer.hands.skinRect, Color.White, 0f, new Vector2(16, 16), 1.0f, SpriteEffects.None, 0f);
                if (mainPlayer.hands.type == ItemType.Firearm)
                {
                    Firearm weapon = (Firearm)mainPlayer.hands;

                    spriteBatch.DrawString(_font, weapon.LoadedAmmo.ToString(), _handsPanel.Location.ToVector2() + new Vector2(_handsPanel.Width - _font.MeasureString(weapon.LoadedAmmo.ToString()).X, _handsPanel.Height - _font.MeasureString(weapon.LoadedAmmo.ToString()).Y), Color.White);
                }
            }
		}

		public void drawAimLines(SpriteBatch spriteBatch, MainPlayer mainPlayer, Camera cam)
		{
			float[] aimAccuracyAngle = mainPlayer.getAimAccuracyAngle();
			Vector2[] leftLine = new Vector2[2];
			Vector2[] rightLine = new Vector2[2];

			leftLine[0] = cam.getViewportCenter() + new Vector2((float)Math.Cos(aimAccuracyAngle[0]) * 100, (float)-Math.Sin(aimAccuracyAngle[0]) * 100);
			leftLine[1] = cam.getViewportCenter() + new Vector2((float)Math.Cos(aimAccuracyAngle[0]) * 120, (float)-Math.Sin(aimAccuracyAngle[0]) * 120);
			rightLine[0] = cam.getViewportCenter() + new Vector2((float)Math.Cos(aimAccuracyAngle[1]) * 100, (float)-Math.Sin(aimAccuracyAngle[1]) * 100);
			rightLine[1] = cam.getViewportCenter() + new Vector2((float)Math.Cos(aimAccuracyAngle[1]) * 120, (float)-Math.Sin(aimAccuracyAngle[1]) * 120);
			Engine.DrawLine(spriteBatch, _colorRect, leftLine[0], leftLine[1], Color.Yellow, 1);
			Engine.DrawLine(spriteBatch, _colorRect, rightLine[0], rightLine[1], Color.Yellow, 1);
		}

		public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, MainPlayer mainPlayer, Collection collection, Camera cam)
        {
			_hudEntRect.X = 0;
            //Health
            spriteBatch.Draw(_hudEntities, _statsPos, _hudEntRect, Color.Gray, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(_hudEntities, _statsPos + getHeadsUpHeight(mainPlayer.hp.max, mainPlayer.hp.get(), false), calcHeadsUpRect(mainPlayer.hp.max, mainPlayer.hp.get()), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
			//Armor
			spriteBatch.Draw(_hudEntities, _statsPos + _hudOffset, _hudEntRect, Color.Gray, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
			spriteBatch.DrawString(_font, "999", _statsPos + _hudOffset, Color.White);
            _hudEntRect.X += 64;
			//Hunger
			spriteBatch.Draw(_hudEntities, _foodPos, _hudEntRect, Color.Gray, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(_hudEntities, _foodPos + getHeadsUpHeight(mainPlayer.hunger.max, mainPlayer.hunger.get(), false), calcHeadsUpRect(mainPlayer.hunger.max, mainPlayer.hunger.get()), _foodBackground[0], 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
			//Thirst
			spriteBatch.Draw(_hudEntities, _foodPos + _hudOffset, _hudEntRect, Color.Gray, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(_hudEntities, _foodPos + getHeadsUpHeight(mainPlayer.thirst.max, mainPlayer.thirst.get(), true), calcHeadsUpRect(mainPlayer.thirst.max, mainPlayer.thirst.get()), _foodBackground[1], 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
			//Hands Panel
			drawHandsPanel(spriteBatch, graphicsDevice, mainPlayer, collection);
			//Inventory
            if (mainPlayer.inventoryOpen)
				DrawInventory(spriteBatch, graphicsDevice, mainPlayer);
			//Cursor & AimLines
			drawAimLines(spriteBatch, mainPlayer, cam);
		}
    }
}
