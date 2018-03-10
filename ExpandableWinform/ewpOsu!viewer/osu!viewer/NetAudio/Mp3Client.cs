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
using CommonAudioPlayer;

namespace NetAudio
{
    public class Mp3Client : IDisposable
    {
        public enum PlayState { playing, stopped, paused, mediaEnded }

        public delegate void PlayStateChangedHandler(object sender, PlayStateChangedEventArgs e);
        public event PlayStateChangedHandler PlayStateChanged;

        public delegate void ReceivedHandler(object sender, ReceivedEventArgs e);
        public event ReceivedHandler OnReceivedMessage;

        public delegate void DisposeHandler(object sender, EventArgs e);
        public event DisposeHandler Disposing;

        BinaryFormatter formatter;
        private MessageManager mm;
        private TcpClient tcpClient;
        private NetworkStream netStream;
        private BufferedStream buffNetStream;
        private MemoryStream frameInputStream;
        private object frameIndexLock = new object();
        private object frameListLock = new object();        

        //private IWavePlayer waveOut = new WaveOut();
        //private WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
        private WaveOutEvent waveOut = new WaveOutEvent();
        //private WaveOut waveOut;
        private ReadFullyStream readFullyStream;
        private PlayState mPlayState;
        public PlayState playState
        { 
            get { return mPlayState; }
            private set {                
                mPlayState = value;
                PlayStateChanged?.Invoke(this, new PlayStateChangedEventArgs(mPlayState));
            }
        }

        private bool paused, waveoutInited, playAfterGetInfo;
        private double playStartAt;

        private object setMediaLock = new object();

        private struct Mp3Request
        {
            public DateTime requestTime;
            public string mp3Name;
            public bool done;
        }
        private Mp3Request latestRequest;

        //Mp3 Info
        private string currentMp3;
        private byte[][] frameList;
        private int totalFrames
        {
            get { return frameList == null ? 0 : frameList.Length; }
        }
        private int frameIndex;
        private int frameCheckPoint;
        private int sampleRate;
        public double totalMilliseconds { get; private set; }
        private int latestStartFrame;

        //Bind Mp3File Once
        private bool decompressDone;
        private BufferedWaveProvider bufferedWaveProvider;        
        private VolumeWaveProvider16 volumeProvider;
        private IMp3FrameDecompressor decompressor;

        private float mVolume;
        public int volume
        {
            get { return (int)(mVolume * 100); }
            set
            {
                mVolume = value;
                mVolume = mVolume > 100 ? 100 : mVolume;
                mVolume = mVolume < 0 ? 0 : mVolume;
                mVolume = mVolume / 100f;
                if (waveOut != null)
                    waveOut.Volume = mVolume;
            }
        }
        public bool connected { get; private set; }
        public bool disposed { get; private set; }

        public Mp3Client(String host, int port)
            : this(new TcpClient(host, port))
        {

        }

        public Mp3Client(TcpClient client)
        {
            //ThreadPool.QueueUserWorkItem(initWaveout);
            tcpClient = client;
            this.netStream = tcpClient.GetStream();
            this.buffNetStream = new BufferedStream(tcpClient.GetStream());
            netStream.ReadTimeout = 1000;
            mm = new MessageManager(tcpClient);
            playState = PlayState.stopped;
            formatter = new BinaryFormatter();
            waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
            connect();
        }

        private void initWaveout(Object nothing)
        {
            WaveOut waveOut = new WaveOut(WaveCallbackInfo.NewWindow());
    }

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            playState = PlayState.stopped;
        }

        private void connect()
        {
            if (tcpClient.Connected)
            {
                connected = true;
                ThreadPool.QueueUserWorkItem(ReceiveMessage, netStream);
                ThreadPool.QueueUserWorkItem(StreamMp3);
            }
        }
        
        public bool SendMessage(Message msg)
        {
            if (mm != null)
                return mm.SendMessage(msg);
            else
                return false;
        }

        public void requestMp3(string fileName)
        {
            if (Monitor.TryEnter(setMediaLock, 3000))
            {
                latestRequest.mp3Name = fileName;
                latestRequest.done = false;

                if (playState == PlayState.playing)
                {
                    stop();
                }

                if (waveoutInited)
                {
                    bufferedWaveProvider = null;
                    volumeProvider = null;
                    decompressor = null;
                    waveoutInited = false;
                    decompressDone = false;

                    if (waveOut != null)
                    {
                        waveOut.Dispose();
                        waveOut = new WaveOutEvent();
                    }
                }

                lock (frameListLock)
                {
                    frameList = null;
                    Monitor.PulseAll(frameListLock);
                }
                mm.SendMessage(new Message(MessageCode.requestMp3, fileName));
                latestRequest.requestTime = DateTime.Now;
                currentMp3 = fileName;
                playStartAt = -1;
                Monitor.PulseAll(setMediaLock);
                Monitor.Exit(setMediaLock);
            }
        }

        object setPosLock = new object();
        public void setPosition(double milliseconds)
        {
            if (frameList == null)
                return;
            lock (setMediaLock)
            {
                lock (setPosLock)
                {
                    if (milliseconds > totalMilliseconds) return;
                    double percent = milliseconds / totalMilliseconds;
                    int index;
                    lock (frameIndexLock)
                    {
                        index = (int)Math.Round(totalFrames * percent);
                        frameIndex = index;
                        latestStartFrame = index;
                        if (bufferedWaveProvider != null)
                        {
                            if (waveOut.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                            {
                                pause();
                                waveOut.Stop(); //reset position                            
                                bufferedWaveProvider.ClearBuffer();
                                play();
                            }
                            else
                            {
                                waveOut.Stop(); //reset position
                                bufferedWaveProvider.ClearBuffer();
                            }
                        }
                        Monitor.PulseAll(frameIndexLock);
                    }
                    decompressDone = false;
                    checkChunk(index, out frameCheckPoint);
                    Monitor.PulseAll(setPosLock);
                }
                Monitor.PulseAll(setMediaLock);
            }
        }

        //回傳遞第一個Frame空格，out endIndex => 最後一個Frame空格
        private int checkChunk(int startIndex, out int endIndex)
        {
            Console.WriteLine("Check chunk: {0}", startIndex);

            if (frameList == null)
            {
                endIndex = -1;
                return -1;
            }

            int start = -1, end = -1;
            for(int i = startIndex; i < totalFrames; i++)
            {
                if (start == -1)
                {
                    if (frameList[i] == null)
                    {
                        start = i;
                    }
                }
                else
                {
                    if (frameList[i] != null)
                    {
                        end = i - 1;
                        break;
                    }
                }
            }
            if (start != -1)
            {
                if (end == -1)
                    end = totalFrames - 1;
                requestChunks(start, end);                
            }
            endIndex = end;
            Console.WriteLine("Check done, set checkpoint = {0}", frameCheckPoint);
            return start;
        }

        private void requestChunks(int start, int end)
        {
            Console.WriteLine("request chunks {0} to {1}", start, end);
            mm.SendMessage(new Message(MessageCode.requestChunks, start, end));
            lastGotFrameTime = DateTime.Now;
        }

        private bool IsBufferNearlyFull
        {
            get
            {
                return bufferedWaveProvider != null &&
                       bufferedWaveProvider.BufferLength - bufferedWaveProvider.BufferedBytes
                       < bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 4;
            }
        }

        private void ReceiveMessage(object stream)
        {
            latestRequest.done = true;
            NetworkStream nStream = stream as NetworkStream;
            Message msg;
            while (!disposed)
            {
                if (!latestRequest.done && (DateTime.Now - latestRequest.requestTime).TotalMilliseconds > 3000)
                    requestMp3(latestRequest.mp3Name);

                msg = mm.ReceiveMessage();
                if (msg == null)
                {
                    if (!tcpClient.Connected) break;
                    SendMessage(new Message(MessageCode.test));
                    continue;
                }

                ReceivedEventArgs rea = new ReceivedEventArgs(msg);
                OnReceivedMessage?.Invoke(this, rea);
                if (rea.done)
                {
                    continue;
                }

                switch (msg.msgCode)
                {
                    case MessageCode.mp3Frame :
                        int index = (int) msg.data[0];
                        byte[] frame = msg.data[1] as byte[];
                        putMp3Frame(index, frame);
                        break;
                    case MessageCode.mp3Info:
                        Mp3Info mp3Info = msg.data[0] as Mp3Info;
                        if (!latestRequest.done && mp3Info.name == latestRequest.mp3Name)
                        {
                            frameList = new byte[mp3Info.totalFrames][];
                            totalMilliseconds = mp3Info.totalMilliseconds;
                            sampleRate = mp3Info.sampleRate;
                            setPosition(0);
                            latestRequest.done = true;
                            if (playAfterGetInfo)
                                play();
                        }
                        break;
                }
            }
            Dispose();
            Console.WriteLine("done");
        }

        /// <summary>
        /// 取得撥放進度(秒)
        /// </summary>
        public double playDuration
        {
            get {
                if (playState == PlayState.stopped)
                    return 0;
                try
                {
                    return 1152.0 / sampleRate * latestStartFrame +
                        (double)waveOut.GetPosition() / (double)waveOut.OutputWaveFormat.AverageBytesPerSecond;
                }
                catch (Exception) {
                    return 0;
                }
            }
        }

        private void putMp3Frame(int index, byte[] frameData)
        {            
            lock (frameListLock)
            {
                if (frameList != null)
                {
                    frameList[index] = frameData;
                    lastGotFrameTime = DateTime.Now;
                    if (index == frameCheckPoint && index < totalFrames - 1)
                    {
                        checkChunk(frameCheckPoint + 1, out frameCheckPoint);
                    }
                }
            }
        }

        DateTime lastGotFrameTime = DateTime.Now;
        private void StreamMp3(object nothing)
        {
            if (frameInputStream == null)
            {
                frameInputStream = new MemoryStream(1024 * 1024); //1MB
            }
            readFullyStream = new ReadFullyStream(frameInputStream);            
            Mp3Frame frame;
            byte[] buff = new byte[16384 * 4];

            while(!disposed)
            {
                if (playState == PlayState.playing && !decompressDone)
                {

                    try
                    {
                        if (frameIndex >= frameList.Length)
                        {
                            decompressDone = true;
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }

                    if (IsBufferNearlyFull)
                    {
                        Thread.Sleep(500);
                    }
                    else
                    {
                        lock (setMediaLock)
                        {
                            try
                            {
                                if (Monitor.TryEnter(frameIndexLock, 500))
                                {
                                    if (frameList == null)
                                    {
                                        continue;
                                    }

                                    if (frameList[frameIndex] == null)
                                    {
                                        if ((DateTime.Now - lastGotFrameTime).TotalMilliseconds > 3000)
                                        {
                                            Console.WriteLine("@@@@@@@@@@ Recourse Data @@@@@@@@@@");
                                            checkChunk(frameIndex, out frameCheckPoint);
                                        }
                                        Thread.Sleep(100);
                                        continue;
                                    }
                                    frameInputStream.Position = 0;
                                    frameInputStream.Write(frameList[frameIndex], 0, frameList[frameIndex].Length);
                                    frameInputStream.Position = 0;
                                    frameIndex++;
                                }
                            }
                            finally
                            {
                                Monitor.PulseAll(frameIndexLock);
                                Monitor.Exit(frameIndexLock);
                            }


                            try
                            {
                                frame = Mp3Frame.LoadFromStream(frameInputStream);
                                if (frame == null) continue;
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                            if (decompressor == null)
                            {
                                WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate, frame.ChannelMode == ChannelMode.Mono ? 1 : 2,
                                    frame.FrameLength, frame.BitRate);
                                decompressor = new AcmMp3FrameDecompressor(waveFormat);
                                bufferedWaveProvider = new BufferedWaveProvider(decompressor.OutputFormat);
                                bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(20);
                                volumeProvider = new VolumeWaveProvider16(bufferedWaveProvider);
                                volumeProvider.Volume = mVolume;
                                waveOut.Init(volumeProvider);
                                waveoutInited = true;
                            }

                            try
                            {
                                int decompressed = decompressor.DecompressFrame(frame, buff, 0);
                                bufferedWaveProvider.AddSamples(buff, 0, decompressed);
                            }
                            catch (NAudio.MmException e)
                            {
                                Console.WriteLine(e);
                            }

                            Monitor.PulseAll(setMediaLock);
                        }
                    }
                }
                else
                {                
                    if (playState == PlayState.playing && playDuration >= totalMilliseconds / 1000)
                    {
                        playState = PlayState.mediaEnded;
                        stop();                        
                    }
                    Thread.Sleep(50);
                }
            }
        }

        public void play(double ms)
        {
            playStartAt = ms;
            play();
        }

        public void play()
        {
            if (frameList == null)
            {
                playAfterGetInfo = true;
                return;
            }
            if (playAfterGetInfo)
                playAfterGetInfo = false;
            playState = PlayState.playing;
            ThreadPool.QueueUserWorkItem(playTask);
        }

        object wavePlayLock = new object();
        private void playTask(object nothing)
        {
            while (!waveoutInited)
            {
                if (paused)
                {
                    paused = false;
                    return;
                }
                Thread.Sleep(5);
            }
            if (paused)
            {
                paused = false;
                return;
            }

            if (playStartAt != -1)
            {
                setPosition(playStartAt);
                playStartAt = -1;
            }
            if (waveOut.PlaybackState != NAudio.Wave.PlaybackState.Playing)
            {
                try
                {
                    waveOut.Play();
                }
                catch (NAudio.MmException e)
                {
                    Console.WriteLine(e);
                }
                Console.WriteLine("Playing");
            }
        }

        public void pause()
        {
            paused = true;
            playAfterGetInfo = false;
            if (playState == PlayState.playing)
            {
                waveOut.Pause();
                playState = PlayState.paused;
            }
            Console.WriteLine("Paused");
            paused = false;
        }

        public void stop()
        {
            playAfterGetInfo = false;
            waveOut.Stop();
            playState = PlayState.stopped;
            frameIndex = 0;
            decompressDone = false;
            latestStartFrame = 0;
            Console.WriteLine("Stopped");
        }

        public void Dispose()
        {
            stop();
            disposed = true;
            Disposing?.Invoke(this, null);
            tcpClient.Close();
            lock (frameIndexLock)
            {
                if (readFullyStream != null)
                    readFullyStream.Close();
                if (frameInputStream != null)
                    frameInputStream.Close();
                //if (waveOut != null)
                    //waveOut.Dispose();
            }
        }
    }

    public class PlayStateChangedEventArgs : EventArgs
    {
        public Mp3Client.PlayState currentState;

        public PlayStateChangedEventArgs(Mp3Client.PlayState state)
        {
            currentState = state;
        }
    }

}
