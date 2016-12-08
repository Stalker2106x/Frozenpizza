using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrozenPizzaServer
{
    public class NetCli
    {
        TcpClient _client;
        Thread _cThread;
        NetworkStream _stream;
        CommandHandler _cmdHandle;
        bool _worldSent, _dataSent;
        int _id;
        String _name;
        Player _player;

        //Stack
        Queue<String> _receiveStack;

        public NetCli(TcpClient inClientSocket, int cliId)
        {
            _receiveStack = new Queue<string>();
            _client = inClientSocket;
            _id = cliId;
            _worldSent = false;
            Ready = false;
        }

        public bool Ready { get; set; }

        public int Id
        {
            get { return (_id); }
        }

        public Player Player
        {
            get { return (_player); }
        }

        public void startClient()
        {
            _stream = _client.GetStream();
            _cmdHandle = new CommandHandler(this);
            _cThread = new Thread(Update);
            _cThread.Start();
        }

        public void terminateClient()
        {
            _cThread.Abort();
        }

        private bool isConnected()
        {
            if (_client.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] buff = new byte[1];
                if (_client.Client.Receive(buff, SocketFlags.Peek) == 0)
                {
                    // Client disconnected
                    return (false);
                }
            }
            return (true);
        }

        public bool handShake()
        {
            send(".WELCOME");
            if (receive() != ".ACK")
                return (false);
            send("?VERSION");
            if (!_cmdHandle.ParseExpectedCmd(receive(), "!VERSION"))
                return (false);
			_player = new Player(Server.Level.getSpawnLocation());
            send("?WHOIS");
            if (!_cmdHandle.ParseExpectedCmd(receive(), "!WHOIS"))
                return (false);
            send(".HANDSHAKE");
            if (!_cmdHandle.ParseExpectedCmd(receive(), ".HANDSHAKE"))
                return (false);
            return (true); //HandShake success!
        }

        public bool clientInfo()
        {
            send("!PLAYER " + Id + " " + _player.Pos.X + " " + _player.Pos.Y);
            if (!_cmdHandle.ParseExpectedCmd(receive(), ".ACK"))
                return (false);
            Server.broadcast(Id, "!+PLAYER " + Id + " " + _player.Pos.X + " " + _player.Pos.Y);
            Ready = true;
            return (true);
        }

        private void Update()
        {
            handShake();
            Thread.Sleep(1000); // Timeout for client to generate all placeholders
            clientInfo();
            while (isConnected())
            {
                String msg = receive();

                if (msg != null)
                {
                    _cmdHandle.ParseCmd(msg);
                }
            }
        }

        public void send(String msg)
        {
            byte[] buffer = new byte[msg.Length];

            Console.Write(">[" + _id + "] " + msg + "\n");
            buffer = Encoding.UTF8.GetBytes(msg+"\r\n");
            try
            {
                _stream.Write(buffer, 0, buffer.Length);
            }
            catch (System.IO.IOException e)
            {
                Console.Write(" >> Client ID " + _id + " disconnected.");
                terminateClient();
            }
        }

        public String receive()
        {
            int receivingBufferSize = (int)_client.ReceiveBufferSize;
            byte[] buffer = new byte[receivingBufferSize];
            int readCount = 0;
            String msg, msgprocess;

            if (_receiveStack.Count != 0) //If we have loads of messages to treat
                return (_receiveStack.Dequeue());
            try
            {
                readCount = _stream.Read(buffer, 0, receivingBufferSize);
            }
            catch (System.Exception e)
            {
                Console.Write(" >> Client ID " + _id + " disconnected.");
                terminateClient();
            }
            msg = Encoding.UTF8.GetString(buffer, 0, readCount);
            if (msg.IndexOf("\r\n") != -1)
            {
                msgprocess = msg;
                msg = msgprocess.Substring(0, msg.IndexOf("\r\n"));
                while (msgprocess.IndexOf("\r\n") != -1)
                {
                    msgprocess = msgprocess.Substring(msg.IndexOf("\r\n"), msg.Length - msg.IndexOf("\r\n"));
                    _receiveStack.Enqueue(msgprocess.Substring(0, msg.IndexOf("\r\n")));
                }
            }
            return (msg);
        }
    }
}
