using FrozenPizza.Settings;
using Microsoft.Xna.Framework.Input;
using System;

namespace TargetGame.Settings
{
  public struct DeviceState
  {
    public MouseState mouse;
    public KeyboardState keyboard;
    public GamePadState gamepad;

    public DeviceState(MouseState mouse_, KeyboardState keyboard_, GamePadState gamepad_)
    {
      mouse = mouse_;
      keyboard = keyboard_;
      gamepad = gamepad_;
    }
  }

  //Pair helper
  public struct ControlPair
  {
    public Control? primary, secondary;
    public ControlPair(Control primary_)
    {
      primary = primary_;
      secondary = null;
    }
    public ControlPair(Control primary_, Control secondary_)
    {
      primary = primary_;
      secondary = secondary_;
    }
    public bool IsControlHeld(DeviceState state, DeviceState prevState)
    {
      return ((primary != null && primary.GetValueOrDefault().IsControlHeld(state, prevState))
           || (secondary != null && secondary.GetValueOrDefault().IsControlHeld(state, prevState)));
    }

    public bool IsControlPressed(DeviceState state, DeviceState prevState)
    {
      return ((primary != null && primary.GetValueOrDefault().IsControlPressed(state, prevState))
           || (secondary != null && secondary.GetValueOrDefault().IsControlPressed(state, prevState)));
    }

    public bool IsControlReleased(DeviceState state, DeviceState prevState)
    {
      return ((primary != null && primary.GetValueOrDefault().IsControlReleased(state, prevState))
           || (secondary != null && secondary.GetValueOrDefault().IsControlReleased(state, prevState)));
    }

    public bool IsControlDown(DeviceState state)
    {
      return ((primary != null && primary.GetValueOrDefault().IsControlDown(state))
           || (secondary != null && secondary.GetValueOrDefault().IsControlDown(state)));
    }

    public bool IsControlUp(DeviceState state)
    {
      return ((primary != null && primary.GetValueOrDefault().IsControlUp(state))
           || (secondary != null && secondary.GetValueOrDefault().IsControlUp(state)));
    }
  }
  //Single control
  public struct Control
  {
    public Keys? key;
    public Buttons? button;
    public MouseButton? mouse;

    public Control(Keys key_) { key = key_; button = null; mouse = null; }
    public Control(Buttons button_) { key = null; button = button_; mouse = null; }
    public Control(MouseButton mouse_) { key = null; button = null; mouse = mouse_; }

    public bool IsBound()
    {
      return (key != null || button != null || mouse != null);
    }
    public string GetInput()
    {
      if (key != null) return (key.GetValueOrDefault().ToString());
      if (button != null) return (button.GetValueOrDefault().ToString());
      if (mouse != null) return (mouse.GetValueOrDefault().ToString());
      return ("Unbound");
    }

    public bool IsControlHeld(DeviceState state, DeviceState prevState)
    {
      return (IsControlDown(state) && IsControlDown(prevState));
    }

    public bool IsControlPressed(DeviceState state, DeviceState prevState)
    {
      return (IsControlDown(state) && IsControlUp(prevState));
    }

    public bool IsControlReleased(DeviceState state, DeviceState prevState)
    {
      return (IsControlUp(state) && IsControlDown(prevState));
    }

    public bool IsControlDown(DeviceState state)
    {
      return ((key != null && state.keyboard.IsKeyDown(key.GetValueOrDefault()))
           || (button != null && state.gamepad.IsButtonDown(button.GetValueOrDefault()))
           || (mouse != null && MouseHelper.ButtonClicked(state.mouse, mouse.GetValueOrDefault())));
    }

    public bool IsControlUp(DeviceState state)
    {
      return ((key != null && state.keyboard.IsKeyUp(key.GetValueOrDefault()))
           || (button != null && state.gamepad.IsButtonUp(button.GetValueOrDefault()))
           || (mouse != null && !MouseHelper.ButtonClicked(state.mouse, mouse.GetValueOrDefault())));
    }

    public static Control? GetAnyPressedKey(DeviceState state)
    {
      Control control = new Control();
      //Scan Keyboard
      foreach (Keys key in Enum.GetValues(typeof(Keys)))
      {
        if (state.keyboard.IsKeyDown(key))
        {
          control.key = key;
          return (control);
        }
      }
      //Scan gamepad
      foreach (Buttons btn in Enum.GetValues(typeof(Buttons)))
      {
        if (state.gamepad.IsButtonDown(btn))
        {
          control.button = btn;
          return (control);
        }
      }
      //Scan mouse
      foreach (MouseButton mbtn in Enum.GetValues(typeof(MouseButton)))
      {
        if (MouseHelper.ButtonClicked(state.mouse, mbtn))
        {
          control.mouse = mbtn;
          return (control);
        }
      }
      return (null);
    }
  }
}
