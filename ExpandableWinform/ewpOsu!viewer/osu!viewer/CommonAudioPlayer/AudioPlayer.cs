using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonAudioPlayer
{
    public abstract class AudioPlayer : AudioPlayerInterface
    {
        public abstract CommonPlaybackState currentPlayState { get; }
        public abstract double playbackDuration { get; set; }
        public abstract int volume { get; set; }
        public abstract double mediaLength { get; }

        public delegate void PlayStateChangedHandler(object sender, PlayStateChangedEventArgs e);
        public event PlayStateChangedHandler PlayStateChanged;

        public delegate void DisposeHandler(object sender, EventArgs e);
        public event DisposeHandler Disposing;

        public bool disposed;

        public virtual bool Loop { get; set; }
        
        protected void playStateChanged()
        {
            PlayStateChanged?.Invoke(this, new PlayStateChangedEventArgs(currentPlayState));
        }

        public abstract void setMedia(object media);
        public abstract void play();
        public abstract void pause();
        public abstract void resume();
        public abstract void stop();

        public string currentDurationString()
        {
            return SecondToString((int)(playbackDuration / 1000));
        }

        public virtual void Dispose()
        {
            disposed = true;
            Disposing?.Invoke(this, null);
        }

        public static string SecondToString(int second)
        {
            int sec = second;
            int hr = sec / 3600; sec %= 3600;
            int min = sec / 60; sec %= 60;
            StringBuilder sb = new StringBuilder();
            if (hr > 0)
            {
                if (hr < 10)
                    sb.Append('0');
                sb.Append(hr + ":");
            }
            if (min > 0)
            {
                if (min < 10)
                    sb.Append('0');
                sb.Append(min + ":");
            }
            else
                sb.Append("00:");
            if (sec > 0)
            {
                if (sec < 10)
                    sb.Append('0');
                sb.Append(sec);
            }
            else
                sb.Append("00");
            return sb.ToString();
        }

        public abstract void play(double ms);
    }

    public class PlayStateChangedEventArgs : EventArgs
    {
        public CommonPlaybackState currentState;

        public PlayStateChangedEventArgs(CommonPlaybackState state)
        {
            currentState = state;
        }
    }
}
