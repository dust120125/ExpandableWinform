using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using NAudio.Wave;

namespace NetAudio
{
    public class Mp3Streamer : IDisposable
    {
        public delegate void ReceivedHandler(object sender, ReceivedEventArgs e);
        public event ReceivedHandler OnReveived;

        BinaryFormatter formatter;
        private TcpClient client;
        private NetworkStream netStream;
        private BufferedStream buffNetStream;
        private MessageManager mm;

        private IMp3FileReader mp3FileReader;
        private FileStream mp3FileStream;

        private int requestStart, requestEnd;
        private bool stopStream;
        private object streamingLock = new object();

        private Thread streamer;
        private PlaybackState playState;
        public bool disposed { get; private set; }

        private StreamWriter logFile;

        public Mp3Streamer(TcpClient client)
        {
            if (osu_viewer.GlobalResources.DEBUG)
            {
                initLogFile();
            }

            this.client = client;
            this.client.GetStream().ReadTimeout = 1000;
            this.netStream = client.GetStream();
            this.buffNetStream = new BufferedStream(client.GetStream());
            mm = new MessageManager(client);
            formatter = new BinaryFormatter();
            playState = PlaybackState.Stopped;
            streamer = new Thread(streamingMp3);
            streamer.Start();
            run();
        }

        private void initLogFile()
        {
            if (!Directory.Exists("log"))
            {
                Directory.CreateDirectory("log");
            }
            FileStream fs = new FileStream("log\\" + (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds + ".txt", FileMode.Create);
            logFile = new StreamWriter(fs);
            logFile.WriteLine(client.Client.RemoteEndPoint.ToString() + "=> Connected");
            logFile.Flush();
        }

        private void run()
        {
            ThreadPool.QueueUserWorkItem(callBack =>
            {
                Message msg;
                while (!disposed)
                {
                    try
                    {
                        msg = mm.ReceiveMessage();
                        if (msg == null)
                        {
                            if (!client.Connected)
                            {
                                Dispose();
                            }
                            continue;
                        }

                        if (msg.msgCode == MessageCode.test) continue;

                        logStr(client.Client.RemoteEndPoint.ToString() + "=> Got Message: {0}", msg.msgCode);
                        
                        ReceivedEventArgs rea = new ReceivedEventArgs(msg);
                        if (OnReveived != null)
                            OnReveived(this, rea);
                        if (!rea.done)
                        {
                            dealRequest(msg);
                        }
                    }
                    catch (IOException)
                    {
                        
                    }
                }
            });
        }

        public void SendMessage(Message msg)
        {
            if (msg.msgCode == MessageCode.osuList)
            {
                logStr(client.Client.RemoteEndPoint.ToString() + "=> Sending {0}\n", msg.msgCode);
                if (mm.SendMessage(msg))
                {
                    logStr(client.Client.RemoteEndPoint.ToString() + "=> Send Done {0}\n", msg.msgCode);                    
                }
            }
        }

        DateTime logTime = DateTime.Now;
        private void logStr(string log, params object[] arg)
        {
            if (!osu_viewer.GlobalResources.DEBUG) return;

            logFile.WriteLine(log, arg);

            if ((DateTime.Now - logTime).Milliseconds > 2000)
            {
                logTime = DateTime.Now;
                logFile.Flush();
            }            
        }

        private void dealRequest(Message msg)
        {
            byte mode = msg.msgCode;
            switch (mode)
            {
                case MessageCode.requestMp3:
                    logStr(client.Client.RemoteEndPoint.ToString() + "=> request mp3 {0}\n", msg.data[0] as string);
                    responseMp3(msg.data[0] as string);
                    break;
                case MessageCode.requestChunks:
                    logStr(client.Client.RemoteEndPoint.ToString() + "=> request chunks {0} to {1}\n", (int)msg.data[0], (int)msg.data[1]);
                    Console.WriteLine("request chunks {0} to {1}", msg.data[0], msg.data[1]);
                    responseChunks((int)msg.data[0], (int)msg.data[1]);
                    break;                    
            }
        }

        private void responseMp3(string fileName)
        {
            stopStream = true;
            lock (streamingLock)
            {
                long len = setMp3File(fileName);
                var mp3Info = new Mp3Info();
                mp3Info.name = fileName;
                mp3Info.totalFrames = mp3FileReader.totalFrames;
                mp3Info.totalMilliseconds = mp3FileReader.totalSeconds * 1000;
                mp3Info.sampleRate = mp3FileReader.getSampleRate();
                mm.SendMessage(new Message(MessageCode.mp3Info, mp3Info));
                Monitor.PulseAll(streamingLock);
            }
            stopStream = false;
        }

        private long setMp3File(string fileName)
        {
            if (mp3FileReader != null)
            {
                mp3FileReader.Close();
                mp3FileStream.Close();
            }
            mp3FileStream =
                new FileStream(fileName, FileMode.Open);
            mp3FileReader = new IMp3FileReader(mp3FileStream);
            return mp3FileStream.Length;
        }

        private void responseChunks(int start, int end)
        {            
            stopStream = true;
            lock (streamingLock)
            {
                requestStart = start;
                requestEnd = end;
                stopStream = false;
                Monitor.PulseAll(streamingLock);
            }
        }

        public void streamingMp3()
        {
            byte[] buff = new byte[4096];
            byte[] frame;
            int index;
            int cc;

            lock (streamingLock)
            {
                while (!disposed)
                {
                    Monitor.PulseAll(streamingLock);
                    Monitor.Wait(streamingLock);
                    if (mp3FileReader != null && !disposed)
                    {
                        cc = requestStart;
                        mp3FileReader.setIndex(requestStart);
                        for (int i = requestStart; i <= requestEnd && !stopStream; i++)
                        {
                            frame = mp3FileReader.ReadNextFrameToBytes(out index);
                            if (frame == null) continue;
                            if (!mm.SendMessage(new Message(MessageCode.mp3Frame, index, frame)))
                            {
                                if (!client.Connected) break;
                            }
                            Console.WriteLine("{0}/{1}, {2}", cc++, requestStart, requestEnd);
                        }
                    }
                }
            }
            Console.WriteLine("Stream finished");
        }

        public void streamingMp3_()
        {
            if (mp3FileStream != null)
            {
                byte[] buff = new byte[4096];
                int len;
                while (true)
                {
                    if (playState == PlaybackState.Playing)
                    {
                        lock (mp3FileStream)
                        {
                            len = mp3FileStream.Read(buff, 0, buff.Length);
                            if (len == -1) break;
                            netStream.Write(buff, 0, len);
                            netStream.Flush();
                            Console.WriteLine(len + "");
                        }
                    }
                }
                Console.WriteLine("Stream finished");
            }
        }

        public void setStreamPosition(long pos)
        {
            playState = PlaybackState.Paused;
            lock (mp3FileStream)
            {
                mp3FileStream.Position = pos;                
            }
            Console.WriteLine("www");
            Console.ReadKey();
            Console.WriteLine("sss");
            playState = PlaybackState.Playing;
        }

        public void Dispose()
        {
            disposed = true;
            stopStream = true;
            client.Close();
            if (mp3FileReader != null)
                mp3FileReader.Close();
            if (mp3FileStream != null)
                mp3FileStream.Close();
            mp3FileReader = null;
            try
            {
                logFile.Close();
            }
            catch (Exception e)
            {

            }

            lock(streamingLock)
                Monitor.PulseAll(streamingLock);
        }

        public override string ToString()
        {
            return client.Client.RemoteEndPoint.ToString();
        }

    }

    public class ReceivedEventArgs : EventArgs
    {

        public Message msg { get; private set; }
        public bool done;

        public ReceivedEventArgs(Message msg)
        {
            this.msg = msg;
            this.done = false;
        }

    }

}
