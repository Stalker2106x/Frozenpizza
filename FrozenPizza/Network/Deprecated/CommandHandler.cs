using FrozenPizza;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
  public class CommandHandler
  {
    Dictionary<String, Func<String[], bool>> _commands;

    public CommandHandler()
    {
      _commands = new Dictionary<String, Func<String[], bool>>();
      _commands.Add(".WELCOME", acknowledge);
      _commands.Add("?VERSION", sendVersion);
      _commands.Add("?WHOIS", sendWhois);
      _commands.Add("!MAP", setMap);
      _commands.Add("!PLAYER", setMainPlayer);
      _commands.Add("!+PLAYER", addNewPlayer);
      _commands.Add("!STATE", playerState);
      _commands.Add("!+FIRE", createProjectile);
      _commands.Add("!HIT", playerHit);
      _commands.Add("!++ITEM", spawnItem);
      _commands.Add("!+ITEM", dropItem);
      _commands.Add("!-ITEM", pickItem);
      _commands.Add(".HANDSHAKE", handShake);
      _commands.Add(".READY", ready);
      _commands.Add(".OK", accept);
      _commands.Add(".KO", error);
    }

    public static String getCmd(String msg)
    {
      if (msg.IndexOf(' ') == -1)
        return (msg);
      return (msg.Substring(0, msg.IndexOf(' ')));
    }

    public static String[] getArgs(String msg)
    {
      String[] args;
      int argc;

      argc = msg.Split(' ').Length - 1;
      args = new String[argc];
      msg = msg.Remove(0, msg.IndexOf(' ') + 1);
      for (int i = 0; i < argc; i++)
      {
        int nextSpace = msg.IndexOf(' ');

        if (nextSpace > 0)
        {
          args[i] = msg.Substring(0, nextSpace);
          msg = msg.Remove(0, nextSpace + 1);
        }
        else
          args[i] = msg;
      }
      return (args);
    }

    public bool ParseCmd(String msg)
    {
      String cmd;
      String[] args;

      if (msg == null)
        return (false);
      cmd = getCmd(msg);
      args = getArgs(msg);
      if (!_commands.ContainsKey(cmd))
        return (false);
      _commands[cmd](args);
      return (true);
    }

    public bool ParseExpectedCmd(String msg, String expected)
    {
      if (msg == null)
        return (false);
      if (getCmd(msg) != expected)
        return (false);
      return (ParseCmd(msg));
    }

    //Handshake
    bool sendVersion(String[] args)
    {
      NetHandler.send("!VERSION " + Assembly.GetEntryAssembly().GetName().Version.ToString());
      NetHandler.ConnectionStatus = "Checking version...";
      return (true);
    }

    bool sendWhois(String[] args)
    {
      NetHandler.send("!WHOIS");
      NetHandler.ConnectionStatus = "Sending client info...";
      return (true);
    }

    bool handShake(String[] args)
    {
      NetHandler.send(".HANDSHAKE");
      NetHandler.ConnectionStatus = "Handshake success!";
      NetHandler.HandshakeCallback();
      //Engine.netHandle.Handshake = true;
      return (true);
    }

    //Player
    bool setMainPlayer(String[] args)
    {
      Vector2 pos;
      int id;

      Int32.TryParse(args[0], out id);
      //GameMain.mainPlayer.id = id;
      float.TryParse(args[1], out pos.X);
      float.TryParse(args[2], out pos.Y);
      //GameMain.mainPlayer.pos = pos;
      acknowledge(null);
      return (true);
    }

    bool setMap(String[] args)
    {
      List<String> levels = Level.getAvailableLevels();
      int index = args[0].LastIndexOf('/') + 1;

      for (int i = 0; i < levels.Count; i++)
      {
        if (levels[i] == args[0].Substring(index, args[0].Length - (index + 4)))
        {
          Level.MapName = "Data/maps/" + levels[i] + ".tmx";
          acknowledge(null);
          return (true);
        }
      }
      NetHandler.ConnectionStatus = args[0].Substring(index, args[0].Length - (index + 4)) + ": Missing map.";
      nacknowledge(null);
      return (false);
    }

    bool addNewPlayer(String[] args)
    {
      int id, hp;
      Vector2 pos;

      Int32.TryParse(args[0], out id);
      Int32.TryParse(args[1], out hp);
      float.TryParse(args[2], out pos.X);
      float.TryParse(args[3], out pos.Y);
      GameMain.players.Add(new Player(id, "RP", pos, hp));
      acknowledge(null);
      return (true);
    }

    bool playerState(String[] args)
    {
      int id;
      Vector2 pos;
      float orientation;

      Int32.TryParse(args[0], out id);
      float.TryParse(args[1], out pos.X);
      float.TryParse(args[2], out pos.Y);
      float.TryParse(args[3], out orientation);
      /*if (GameMain.mainPlayer != null && id == GameMain.mainPlayer.id)
      {
        GameMain.mainPlayer.pos = pos;
        GameMain.mainPlayer.orientation = orientation;
      }
      else if (GameMain.getPlayerById(id) != null)
      {
        GameMain.getPlayerById(id).pos = pos;
        GameMain.getPlayerById(id).orientation = orientation;
      }*/
      return (true);
    }

    bool createProjectile(String[] args)
    {
      int id, type, damage;
      float angle, velocity;
      Vector2 pos;

      Int32.TryParse(args[0], out id);
      Int32.TryParse(args[1], out type);
      float.TryParse(args[2], out pos.X);
      float.TryParse(args[3], out pos.Y);
      float.TryParse(args[4], out angle);
      float.TryParse(args[5], out velocity);
      Int32.TryParse(args[6], out damage);
      //GameMain.map.Projectiles.Add(new Projectile(id, ProjectileType.Bullet, pos, angle, velocity, damage));
      acknowledge(null);
      return (true);
    }

    bool playerHit(String[] args)
    {
      int id, damage;

      Int32.TryParse(args[0], out id);
      Int32.TryParse(args[1], out damage);
      //if (id == GameMain.mainPlayer.id)
        //GameMain.mainPlayer.hurt(damage);
      //else
        //GameMain.getPlayerById(id).hurt(damage);
      acknowledge(null);
      return (true);
    }

    //World data
    bool spawnItem(String[] args)
    {
      Vector2 pos;
      int id;
      Int64 uid;

      Int64.TryParse(args[0], out uid);
      Int32.TryParse(args[1], out id);
      float.TryParse(args[2], out pos.X);
      float.TryParse(args[3], out pos.Y);
      //GameMain.map.Entities.Add(Collection.getNewItemById(uid, id));
      //GameMain.map.Entities.Last().pos = pos;
      acknowledge(null);
      return (true);
    }

    bool dropItem(String[] args)
    {
      Vector2 pos;
      Int64 uid;

      Int64.TryParse(args[0], out uid);
      float.TryParse(args[1], out pos.X);
      float.TryParse(args[2], out pos.Y);
      //GameMain.map.Entities[GameMain.map.getEntityIndex(uid)].pos = pos;
      acknowledge(null);
      return (true);
    }

    bool pickItem(String[] args)
    {
      Int64 uid;

      Int64.TryParse(args[0], out uid);
      //GameMain.map.Entities[GameMain.map.getEntityIndex(uid)].pos = -Vector2.One;
      acknowledge(null);
      return (true);
    }

    //ready switch
    bool ready(String[] args)
    {
      NetHandler.send(".READY");
      //Engine.netHandle.Ready = true;
      return (true);
    }

    //Ack & Nack switches
    bool accept(String[] args)
    {
      return (true);
    }
    bool error(String[] args)
    {
      return (false);
    }
    bool acknowledge(String[] args)
    {
      NetHandler.send(".ACK");
      return (true);
    }

    bool nacknowledge(String[] args)
    {
      NetHandler.send(".NACK");
      return (true);
    }
  }
}
