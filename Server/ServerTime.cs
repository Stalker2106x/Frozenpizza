using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Server
{
  public class ServerTime
  {
    private Stopwatch _timer;
    private TimeSpan _lastTick;
    public ServerTime()
    {
      _timer = new Stopwatch();
      _timer.Start();
    }

    public void Update()
    {
      _lastTick = _timer.Elapsed;
    }

    public TimeSpan TotalGameTime {
      get {
        return (_timer.Elapsed);
      }
    }

    public TimeSpan ElapsedGameTime {
      get
      {
        return (_timer.Elapsed - _lastTick);
      }
    }
  }
}
