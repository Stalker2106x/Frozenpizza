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
        int _id;
        String _name;

        public NetCli(TcpClient inClientSocket, int cliId)
        {
            _client = inClientSocket;
            _id = cliId;
        }

        public int Id
        {
            get { return (_id); }
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
            send("?WHOIS");
            if (!_cmdHandle.ParseExpectedCmd(receive(), "!WHOIS"))
                return (false);
            send(".HANDSHAKE");
            if (!_cmdHandle.ParseExpectedCmd(receive(), ".HANDSHAKE"))
                return (false);
            return (true); //HandShake success!
        }

        private void Update()
        {
            handShake();
            while (isConnected())
            {
                String msg = receive();

                Console.WriteLine(msg);
            }
        }

        public void send(String msg)
        {
            byte[] buffer = new byte[msg.Length];

            Console.Write(">[" + _id + "] " + msg + "\n");
            buffer = Encoding.UTF8.GetBytes(msg);
            _stream.Write(buffer, 0, buffer.Length);
        }

        public String receive()
        {
            int receivingBufferSize = (int)_client.ReceiveBufferSize;
            byte[] buffer = new byte[receivingBufferSize];
            int readCount = _stream.Read(buffer, 0, receivingBufferSize);
            String msg;

            if (readCount <= 0)
                return ("");
            msg = Encoding.UTF8.GetString(buffer, 0, readCount);
            Console.Write("[" + _id + "] " + msg + "\n");
            return (msg);
        }
    }
}
