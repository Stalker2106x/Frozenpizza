using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FrozenPizza.Utils
{
  public class ProbabilityRange
  {
    List<int> _steps;
    public ProbabilityRange(params int[] probabilities)
    {
      _steps = new List<int>();
    }
    public void AddRange(int probability)
    {
      int total = (_steps.Count == 0 ? 0 : _steps[_steps.Count - 1]) + probability;
      if (total > 100) throw new IndexOutOfRangeException("Probabilities exceed 100%");
      _steps.Add(total);
    }

    public int GetIndex(int probability)
    {
      for (int i = 0; i < _steps.Count; i++)
      {
        if (probability <= _steps[i]) return (i);
      }
      throw new IndexOutOfRangeException("Probability not in range");
    }
  }
}
