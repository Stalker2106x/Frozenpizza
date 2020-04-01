using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{
  public class Program
  {
    static int Main(string[] args)
    {

      if (args.Length < 1)
        return (-1);
      Server.run(args[0]);
      return (0);
    }
  }
}
