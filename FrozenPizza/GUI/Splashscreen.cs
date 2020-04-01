using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using FrozenPizza.Settings;
using FrozenPizza.Utils;

namespace FrozenPizza
{
  /// <summary>
  /// Helper class for launch splashscreen
  /// </summary>
  public static class SplashScreen
  {
    private static Timer _splashTimer = new Timer();
    private static Timer _stayTimer = new Timer();
    private static Image _image;

    public static void Init()
    {
      _splashTimer.addAction(TimerDirection.Forward, 2000, TimeoutBehaviour.Stop, () => { _splashTimer.Reverse(); _stayTimer.Start(); });
      _splashTimer.addAction(TimerDirection.Backward, -1, TimeoutBehaviour.Stop,
        () => { Desktop.Widgets.Remove(_image); Engine.setState(GameState.Menu); Menu.MainMenu(); });
      _stayTimer.addAction(TimerDirection.Forward, 2000, TimeoutBehaviour.Stop, () => { _splashTimer.Start(); });
      _image = new Image();
      _image.Opacity = 0;
      //_image.Renderable = new TextureRegion(Resources.splashScreen);
      Desktop.Widgets.Add(_image);
    }

    public static void Trigger()
    {
      _image.Opacity = 1;
      _splashTimer.Start();
    }

    public static void Skip()
    {
      _stayTimer.setDuration(2001);
      if (_splashTimer.getDirection() != TimerDirection.Backward) _splashTimer.Reverse();
      _splashTimer.Start();
    }

    public static void Update(GameTime gameTime, DeviceState state)
    {
      if (Control.GetAnyPressedKey(state) != null) Skip();
      _splashTimer.Update(gameTime);
      _stayTimer.Update(gameTime);
      float value = (float)(_splashTimer.getDuration() / 2000);
      if (value > 1) value = 1;
      else if (value < 0) value = 0;
      _image.Opacity = value;
    }
  }
}
