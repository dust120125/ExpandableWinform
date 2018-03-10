using System;

namespace CommonAudioPlayer
{
    public enum CommonPlaybackState { playing, paused, stopped, mediaEnded, other }
    interface AudioPlayerInterface : IDisposable
    {
        
        void setMedia(object media);

        void play();

        void play(double ms);

        void pause();

        void resume();

        void stop();

        double playbackDuration
        {
            get; set;
        }        

        /// <summary>
        /// 音量 100最大，0靜音
        /// </summary>
        int volume
        {
            get; set;
        }

        double mediaLength
        {
            get;
        }
               
        CommonPlaybackState currentPlayState
        {
            get; 
        }
    }
}
