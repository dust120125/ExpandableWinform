using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetAudio
{
    [Serializable]
    public class Message
    {
        public byte msgCode { get; private set; }
        public object[] data { get; private set; }

        public Message(byte msgCode, params object[] data)
        {
            this.msgCode = msgCode;
            this.data = data;
        }

    }
}
