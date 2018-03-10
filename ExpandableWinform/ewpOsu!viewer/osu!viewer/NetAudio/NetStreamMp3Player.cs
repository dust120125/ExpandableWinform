using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonAudioPlayer;
using System.Net.Sockets;

namespace NetAudio
{
    public class NetStreamMp3Player : AudioPlayer
    {
        Mp3Client mp3Client;
        CommonPlaybackState mPlayState;
        public event Mp3Client.ReceivedHandler ReceivedMessageEvent;        

        public override CommonPlaybackState currentPlayState
        {
            get { return mPlayState; }
        }

        public override double playbackDuration
        {
            get
            {
                if (currentPlayState == CommonPlaybackState.stopped)
                    return 0;
                return mp3Client.playDuration * 1000;
            }

            set
            {
                mp3Client.setPosition(value);
            }
        }

        public override int volume
        {
            get
            {
                return mp3Client.volume;
            }

            set
            {
                mp3Client.volume = value;
            }
        }

        public override double mediaLength
        {
            get
            {
                return mp3Client.totalMilliseconds;
            }
        }

        public NetStreamMp3Player(TcpClient client)
        {
            mp3Client = new Mp3Client(client);
            mp3Client.PlayStateChanged += Mp3Client_PlayStateChanged;
            mp3Client.OnReceivedMessage += Mp3Client_OnReceivedMessage;
            mp3Client.Disposing += Mp3Client_Disposing;
        }

        public NetStreamMp3Player(String host, int port)
        {
            mp3Client = new Mp3Client(host, port);
            mp3Client.PlayStateChanged += Mp3Client_PlayStateChanged;
            mp3Client.OnReceivedMessage += Mp3Client_OnReceivedMessage;
            mp3Client.Disposing += Mp3Client_Disposing;
        }

        private void Mp3Client_Disposing(object sender, EventArgs e)
        {
            Dispose();
        }

        public bool SendMessage(Message msg)
        {
            return mp3Client.SendMessage(msg);
        }

        private void Mp3Client_OnReceivedMessage(object sender, ReceivedEventArgs e)
        {
            ReceivedMessageEvent?.Invoke(this, e);            
        }

        private void Mp3Client_PlayStateChanged(object sender, PlayStateChangedEventArgs e)
        {
            switch (e.currentState)
            {
                case Mp3Client.PlayState.playing:
                    mPlayState = CommonPlaybackState.playing;
                    break;
                case Mp3Client.PlayState.paused:
                    mPlayState = CommonPlaybackState.paused;
                    break;
                case Mp3Client.PlayState.stopped:
                    mPlayState = CommonPlaybackState.stopped;
                    break;
                case Mp3Client.PlayState.mediaEnded:
                    mPlayState = CommonPlaybackState.mediaEnded;
                    if (Loop)
                    {
                        play();
                    }
                    break;
                default:
                    mPlayState = CommonPlaybackState.other;
                    break;
            }
            playStateChanged();
        }

        public double getPlaybackDuration()
        {
            return mp3Client.playDuration;
        }

        public override void pause()
        {
            mp3Client.pause();
        }

        public override void play()
        {
            mp3Client.play();
        }

        public override void resume()
        {
            mp3Client.play();
        }

        public override void setMedia(object media)
        {
            string fileName = media as string;
            mp3Client.requestMp3(fileName);
            mp3Client.play();
        }

        public void setPlaybackDuration(double milliseconds)
        {
            mp3Client.setPosition(milliseconds);
        }

        public override void stop()
        {
            mp3Client.stop();
        }
                
        public override void Dispose()
        {
            if (!mp3Client.disposed)
                mp3Client.Dispose();
            base.Dispose();
        }

        public override void play(double ms)
        {
            mp3Client.play(ms);
        }
    }
}
