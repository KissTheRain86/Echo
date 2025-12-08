using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace EchoServer
{
    class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
    }
}
