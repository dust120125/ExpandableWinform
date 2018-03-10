using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetAudio
{
    class MessageManager
    {
        private TcpClient client;
        private NetworkStream netStream;
        private BinaryFormatter formatter = new BinaryFormatter();
        public bool CONNECT_DEAD;

        private byte[] inputBuffer = new byte[40960];
        private byte[] headerBuffer = new byte[4];

        private const int MAX_INPUT_BUFFER = 67108864;
        private static readonly byte[] HEADER = { 0x58, 0x58, 0x5A, 0x5A }; //"XXZZ" to byte

        public MessageManager(TcpClient client)
        {
            this.client = client;
            netStream = client.GetStream();
            CONNECT_DEAD = false;
        }

        public bool SendMessage(Message msg)
        {
            if (msg.msgCode != MessageCode.test)
                Console.WriteLine("Send {0}", msg.msgCode);

            try
            {                
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, msg);
                    netStream.Write(HEADER, 0, 4);
                    netStream.Write(BitConverter.GetBytes((int)ms.Length), 0, 4);
                    netStream.Write(ms.ToArray(), 0, (int)ms.Length);
                }
                formatter.Serialize(netStream, msg);
                netStream.Flush();
            }
            catch (Exception e)
            {
                SocketException se = e.InnerException as SocketException;
                if (se != null)
                {
                    if (se.SocketErrorCode != SocketError.TimedOut)
                    {
                        Console.WriteLine(se);
                    }
                }
                else
                {
                    Console.WriteLine(e);
                }
                return false;
            }

            if (msg.msgCode != MessageCode.test)
                Console.WriteLine("Send {0}", msg.msgCode);

            return true;
        }

        public Message ReceiveMessage()
        {
            while (true)
            {
                /*   
                while (!stream.DataAvailable)
                {
                    System.Threading.Thread.Yield();
                    System.Threading.Thread.Sleep(10);
                }
                */
                Message tmp;
                bool callGC = false;
                try
                {
                    /*
                    byte[] lenB = new byte[4];
                    netStream.Read(lenB, 0, 4);
                    int len = BitConverter.ToInt32(lenB, 0);
                    if (len > MAX_INPUT_BUFFER)
                        throw new IOException("Receive Data Too Large: " + len + "bytes");
                        */

                    int len = ReadHeader();
                    byte[] buff;
                    if (len > inputBuffer.Length)
                    {
                        buff = new byte[len];
                        callGC = true;
                    }
                    else
                        buff = inputBuffer;

                    while (len > 0)
                    {
                        int readed = netStream.Read(buff, 0, len);
                        len -= readed;
                    }

                    tmp = formatter.Deserialize(netStream) as Message;
                    return tmp;
                }
                catch (Exception e)
                {
                    SocketException se = e.InnerException as SocketException;
                    if (se != null)
                    {
                        if (se.SocketErrorCode != SocketError.TimedOut)
                        {
                            Console.WriteLine(se);
                        }
                    }
                    else
                    {
                        Console.WriteLine(e);
                    }
                    return null;
                }
                finally
                {
                    if (callGC)
                        System.GC.Collect();
                }
            }
        }

        private int ReadHeader()
        {
            netStream.Read(headerBuffer, 0, 4);
            while (true)
            {
                if (headerBuffer[0] == HEADER[0] &&
                    headerBuffer[1] == HEADER[1] &&
                    headerBuffer[2] == HEADER[2] &&
                    headerBuffer[3] == HEADER[3])
                {
                    netStream.Read(headerBuffer, 0, 4);
                    return BitConverter.ToInt32(headerBuffer, 0);
                }
                else
                {
                    headerBuffer[0] = headerBuffer[1];
                    headerBuffer[1] = headerBuffer[2];
                    headerBuffer[2] = headerBuffer[3];
                    headerBuffer[3] = (byte)netStream.ReadByte();
                }
            }
        }

    }
}
