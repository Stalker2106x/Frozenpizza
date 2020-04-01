using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenPizza.Settings
{
  //Mouse helper
  public enum MouseButton
  {
    None,
    Left,
    Middle,
    Right,
    XButton1,
    XButton2
  }
  static class MouseHelper
  {
    public static bool ButtonClicked(MouseState state, MouseButton button)
    {
      switch (button)
      {
        case MouseButton.Left:
          return (state.LeftButton == ButtonState.Pressed);
        case MouseButton.Middle:
          return (state.MiddleButton == ButtonState.Pressed);
        case MouseButton.Right:
          return (state.RightButton == ButtonState.Pressed);
        case MouseButton.XButton1:
          return (state.XButton1 == ButtonState.Pressed);
        case MouseButton.XButton2:
          return (state.XButton2 == ButtonState.Pressed);
        default:
          return (false);
      }
    }
  }

}
