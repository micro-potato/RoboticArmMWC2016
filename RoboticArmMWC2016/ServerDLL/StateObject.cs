using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace ServerDLL
{
    internal class StateObject
    {
        public Socket workSocket;
        public const int Buffersize = 307200;
        public byte[] buffer = new byte[Buffersize];

    }
}
