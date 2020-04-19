using FrozenPizza.Entities;
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
    VerticalStackPanel _deathPanel;

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
      _deathPanel = GameMenu.DeathPanel();

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

      Stylesheet.Current.VerticalProgressBarStyle.Background = new ColoredRegion(DefaultAssets.WhiteRegion, Color.Gray);
      Stylesheet.Current.VerticalProgressBarStyle.Filler = new ColoredRegion(DefaultAssets.WhiteRegion, Color.Red);
      _healthIndicator = new VerticalProgressBar();
      _healthIndicator.Width = 64;
      _healthIndicator.Height = 64;
      _healthIndicator.Minimum = 0;
      _healthIndicator.Maximum = 100;
      _healthIndicator.Value = 100;
      bottomLeftPanel.Widgets.Add(_healthIndicator);

      Stylesheet.Current.VerticalProgressBarStyle.Background = new ColoredRegion(DefaultAssets.WhiteRegion, Color.Transparent);
      Stylesheet.Current.VerticalProgressBarStyle.Filler = new ColoredRegion(DefaultAssets.WhiteRegion, Color.Blue);
      _kevlarIndicator = new VerticalProgressBar();
      _kevlarIndicator.Width = 64;
      _kevlarIndicator.Height = 64;
      _kevlarIndicator.Minimum = 0;
      _kevlarIndicator.Maximum = 100;
      _kevlarIndicator.Value = 0;
      bottomLeftPanel.Widgets.Add(_kevlarIndicator);

      _ui.Widgets.Add(bottomLeftPanel);
      // Bottom Panel
      VerticalStackPanel bottomPanel = new VerticalStackPanel();
      bottomPanel.HorizontalAlignment = HorizontalAlignment.Center;
      bottomPanel.VerticalAlignment = VerticalAlignment.Bottom;

      //bottomPanel.Background = new TextureRegion(Resources.MenuBackground);

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

    public void initHands(Item hands)
    {
      _handsLabel.Text = hands.name;
      _handsImage.Renderable = new TextureRegion(hands.textures["world"], new Rectangle(0, 0, hands.textures["world"].Width, hands.textures["world"].Height));
      _handsProgressIndicator.Minimum = 0;
      RangedWeapon weapon;
      if ((weapon = hands as RangedWeapon) != null) _handsProgressIndicator.Maximum = weapon.reloadDelay;
      else _handsAmount.Text = "";
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

      leftLine[0] = cam.getViewportCenter() + Player.getDirectionVector(Direction.Forward, aimAccuracyAngle.left, 100);
      leftLine[1] = cam.getViewportCenter() + Player.getDirectionVector(Direction.Forward, aimAccuracyAngle.left, 120);
      rightLine[0] = cam.getViewportCenter() + Player.getDirectionVector(Direction.Forward, aimAccuracyAngle.right, 100);
      rightLine[1] = cam.getViewportCenter() + Player.getDirectionVector(Direction.Forward, aimAccuracyAngle.right, 120);
      DrawLine(spriteBatch, Resources.Pixel, leftLine[0], leftLine[1], Color.Yellow, 1);
      DrawLine(spriteBatch, Resources.Pixel, rightLine[0], rightLine[1], Color.Yellow, 1);
    }
    public void toggleDeathPanel()
    {
      if (_ui.Widgets.Contains(_deathPanel))
      {
        Engine.cursor.Show = false;
        _ui.Widgets.Remove(_deathPanel);
      }
      else
      {
        Engine.cursor.Show = true;
        _ui.Widgets.Add(_deathPanel);
      }
    }

    void togglePlayersPanel()
    {
      if (_ui.Widgets.Contains(_playersPanel))
      {
        Engine.cursor.Show = false;
        _ui.Widgets.Remove(_playersPanel);
      }
      else
      {
        Engine.cursor.Show = true;
        _ui.Widgets.Add(_playersPanel);
      }
    }

    public void Update(DeviceState state, DeviceState prevState, MainPlayer mainPlayer)
    {
      if (Options.Config.Bindings[GameAction.TogglePlayersPanel].IsControlPressed(state, prevState))
      {
        overlayActive = !overlayActive;
        togglePlayersPanel();
      }
    }

    public static void DrawLine(SpriteBatch spriteBatch, Texture2D text, Vector2 begin, Vector2 end, Color color, int width)
    {
      Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
      Vector2 v = Vector2.Normalize(begin - end);
      float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
      if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
      spriteBatch.Draw(text, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
    }

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, MainPlayer mainPlayer, Camera cam)
    {
      //Cursor & AimLines
      drawAimLines(spriteBatch, mainPlayer, cam);
    }
  }
}
