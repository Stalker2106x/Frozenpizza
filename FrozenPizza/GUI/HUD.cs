﻿using FrozenPizza.Entities;
using FrozenPizza.GUI;
using FrozenPizza.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenPizza
{
  public class HUD
  { 
    // Specific panels
    Panel _playersPanel;

    public bool overlayActive;

    /// NEW
    private Panel _ui;

    private VerticalProgressBar _healthIndicator;
    private VerticalProgressBar _kevlarIndicator;
    private Label _handsLabel;
    private Image _handsImage;
    private Label _handsAmount;
    private HorizontalProgressBar _handsProgressIndicator;

    public HUD(GraphicsDevice graphics, Camera cam)
    {
      _playersPanel = GameMenu.PlayersPlanel();
      Desktop.Widgets.Add(_playersPanel);

      overlayActive = false;
      _ui = new Panel();
      addIndicators();
    }

    public void activate()
    {
      Desktop.Root = _ui;
    }

    public void addIndicators()
    {
      // Bottom Left Panel
      HorizontalStackPanel bottomLeftPanel = new HorizontalStackPanel();
      bottomLeftPanel.HorizontalAlignment = HorizontalAlignment.Left;
      bottomLeftPanel.VerticalAlignment = VerticalAlignment.Bottom;
      bottomLeftPanel.Spacing = 8;

      Stylesheet.Current.HorizontalProgressBarStyle.Background = new ColoredRegion(DefaultAssets.WhiteRegion, Color.Gray);
      Stylesheet.Current.HorizontalProgressBarStyle.Filler = new ColoredRegion(DefaultAssets.WhiteRegion, Color.Red);
      _healthIndicator = new VerticalProgressBar();
      _healthIndicator.Width = 64;
      _healthIndicator.Height = 64;
      _healthIndicator.Minimum = 0;
      _healthIndicator.Maximum = 100;
      _healthIndicator.Value = 100;
      bottomLeftPanel.Widgets.Add(_healthIndicator);

      Stylesheet.Current.HorizontalProgressBarStyle.Background = new ColoredRegion(DefaultAssets.WhiteRegion, Color.Transparent);
      Stylesheet.Current.HorizontalProgressBarStyle.Filler = new ColoredRegion(DefaultAssets.WhiteRegion, Color.Blue);
      _kevlarIndicator = new VerticalProgressBar();
      _kevlarIndicator.Width = 64;
      _kevlarIndicator.Height = 64;
      _kevlarIndicator.Minimum = 0;
      _kevlarIndicator.Maximum = 100;
      _kevlarIndicator.Value = 100;
      bottomLeftPanel.Widgets.Add(_kevlarIndicator);

      _ui.Widgets.Add(bottomLeftPanel);
      // Bottom Panel
      VerticalStackPanel bottomPanel = new VerticalStackPanel();
      bottomPanel.HorizontalAlignment = HorizontalAlignment.Center;
      bottomPanel.VerticalAlignment = VerticalAlignment.Bottom;

      //bottomPanel.Background = new TextureRegion(Collection.MenuBackground);

      _handsLabel = new Label();
      _handsLabel.Text = "Hands";
      bottomPanel.Widgets.Add(_handsLabel);

      _handsImage = new Image();
      bottomPanel.Widgets.Add(_handsImage);

      _handsAmount = new Label();
      _handsAmount.Text = "";
      bottomPanel.Widgets.Add(_handsAmount);

      Stylesheet.Current.HorizontalProgressBarStyle.Background = new ColoredRegion(DefaultAssets.WhiteRegion, Color.Transparent);
      Stylesheet.Current.HorizontalProgressBarStyle.Filler = new ColoredRegion(DefaultAssets.WhiteRegion, Color.LimeGreen);
      _handsProgressIndicator = new HorizontalProgressBar();
      _handsProgressIndicator.Height = 20;
      _handsProgressIndicator.Width = 100;
      bottomPanel.Widgets.Add(_handsProgressIndicator);

      _ui.Widgets.Add(bottomPanel);
    }

    public void initHands(BaseItem hands)
    {
      _handsLabel.Text = hands.name;
      _handsImage.Renderable = new TextureRegion(hands.textures["world"], new Rectangle(0, 0, hands.textures["world"].Width, hands.textures["world"].Height));
      _handsProgressIndicator.Minimum = 0;
      FireWeapon weapon;
      if ((weapon = hands as FireWeapon) != null) _handsProgressIndicator.Maximum = weapon.reloadDelay;
    }

    public void updatePlayer(int health, int armor)
    {
      _healthIndicator.Value = health;
      _kevlarIndicator.Value = armor;
    }

    public void updateWeapon(int ammo, int magazineSize, float reloadTime)
    {
      _handsAmount.Text = ammo + " / " + magazineSize;
      _handsProgressIndicator.Value = reloadTime;
    }

    public void drawAimLines(SpriteBatch spriteBatch, MainPlayer mainPlayer, Camera cam)
    {
      AccuracyAngle aimAccuracyAngle = mainPlayer.getAimAccuracyAngle();
      Vector2[] leftLine = new Vector2[2];
      Vector2[] rightLine = new Vector2[2];

      leftLine[0] = cam.getViewportCenter() + new Vector2((float)Math.Cos(aimAccuracyAngle.max) * 100, (float)-Math.Sin(aimAccuracyAngle.max) * 100);
      leftLine[1] = cam.getViewportCenter() + new Vector2((float)Math.Cos(aimAccuracyAngle.max) * 120, (float)-Math.Sin(aimAccuracyAngle.max) * 120);
      rightLine[0] = cam.getViewportCenter() + new Vector2((float)Math.Cos(aimAccuracyAngle.min) * 100, (float)-Math.Sin(aimAccuracyAngle.min) * 100);
      rightLine[1] = cam.getViewportCenter() + new Vector2((float)Math.Cos(aimAccuracyAngle.min) * 120, (float)-Math.Sin(aimAccuracyAngle.min) * 120);
      Engine.DrawLine(spriteBatch, Collection.Pixel, leftLine[0], leftLine[1], Color.Yellow, 1);
      Engine.DrawLine(spriteBatch, Collection.Pixel, rightLine[0], rightLine[1], Color.Yellow, 1);
    }

    public void Update(DeviceState state, DeviceState prevState, MainPlayer mainPlayer)
    {
      if (Options.Config.Bindings[GameAction.ToggleInventory].IsControlPressed(state, prevState))
      {
        overlayActive = !overlayActive;
        _playersPanel.Enabled = true;
      }
    }

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, MainPlayer mainPlayer, Camera cam)
    {
      //Cursor & AimLines
      drawAimLines(spriteBatch, mainPlayer, cam);
    }
  }
}
