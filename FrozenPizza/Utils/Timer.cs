using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenPizza.Utils
{
  public enum TimerDirection
  {
    Forward,
    Backward
  }
  public enum TimeoutBehaviour
  {
    None,
    Stop,
    Reset,
    StartOver,
    Destroy
  }
  public struct TimeoutAction
  {
    public bool triggered;
    public double timeout;
    public TimeoutBehaviour timeoutBehaviour;
    public Action action;
    public TimerDirection direction;
    public TimeoutAction(TimerDirection direction_, double timeout_, TimeoutBehaviour timeoutBehaviour_, Action action_)
    {
      triggered = false;
      direction = direction_;
      timeout = timeout_;
      timeoutBehaviour = timeoutBehaviour_;
      action = action_;
    }

    public void addTimeout(double toAdd)
    {
      timeout += toAdd;
    }

    public void trigger()
    {
      action();
      triggered = true;
    }
  }

  public class Timer
  {
    private bool _active;
    private double _duration;
    private double _multiplier;
    private TimerDirection _direction;
    public List<TimeoutAction> actions;
    private List<int> _destroyBuffer;

    public Timer()
    {
      Stop();
      _multiplier = 1;
      _duration = 0;
      _destroyBuffer = new List<int>();
      actions = new List<TimeoutAction>();
    }

    public double getDuration()
    {
      return (_duration);
    }
    public TimerDirection getDirection()
    {
      return (_direction);
    }

    public bool isActive()
    {
      return (_active);
    }
    public void setTimeScale(double multiplier)
    {
      _multiplier = multiplier;
    }

    public void setDuration(double duration)
    {
      _duration = duration;
    }
    public void setDirection(TimerDirection direction)
    {
      _direction = direction;
    }

    //Controls

    public void Reset()
    {
      Stop();
      _duration = 0;
      actions.ForEach((it) => { it.triggered = false; });
    }
    public void StartOver()
    {
      Reset();
      Start();
    }

    public void Toggle()
    {
      _active = !_active;
    }

    public void Stop()
    {
      _active = false;
    }

    public void Start()
    {
      _active = true;
    }

    public void Reverse()
    {
      _direction = (_direction == TimerDirection.Forward ? TimerDirection.Backward : TimerDirection.Forward);
    }
    //Setters
    public int addAction(TimerDirection direction, double timeout, TimeoutBehaviour timeoutBehaviour, Action action)
    {
      actions.Add(new TimeoutAction(direction, timeout, timeoutBehaviour, action));
      return (actions.Count - 1);
    }

    public void addMilliseconds(double time)
    {
      _duration += time;
    }

    //Update
    public void Update(GameTime gameTime)
    {
      if (!_active) return; //Only when active
      if (_direction == TimerDirection.Forward) _duration += gameTime.ElapsedGameTime.TotalMilliseconds * _multiplier;
      else if (_direction == TimerDirection.Backward) _duration -= gameTime.ElapsedGameTime.TotalMilliseconds * _multiplier;
      for (int i = 0; i < actions.Count; i++)
      {
        if (!actions[i].triggered && _direction == actions[i].direction
          && ((_direction == TimerDirection.Forward && _duration >= actions[i].timeout)
          || (_direction == TimerDirection.Backward && _duration <= actions[i].timeout)))
        {
          actions[i].trigger();
          switch (actions[i].timeoutBehaviour)
          {
            case TimeoutBehaviour.Stop:
              Stop();
              break;
            case TimeoutBehaviour.Reset:
              Reset();
              break;
            case TimeoutBehaviour.StartOver:
              StartOver();
              break;
            case TimeoutBehaviour.Destroy:
              _active = false;
              _destroyBuffer.Add(i);
              break;
            default:
              break;
          }
        }
      }
      //Clear actions triggered
      if (_destroyBuffer.Count > 0)
      {
        _destroyBuffer.ForEach((idx) => { actions.RemoveAt(idx); });
        _destroyBuffer.Clear();
      }
    }
  }
}
