
using FrozenPizza;
using Server;

namespace FPServer
{
  public class Program
  {
    public static ServerTime gameTime;
    public static ServerV2 server;
    static int Main(string[] args)
    {
      ServerHandlerV2.Initialize();
      EntityStore.LoadMelee();
      EntityStore.LoadRanged();
      gameTime = new ServerTime();

      if (args.Length < 1)
        return (-1);
      server = new ServerV2(args[0]);
      server.start(27015, 5);
      while (true) ;
    }
  }
}
