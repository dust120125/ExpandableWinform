using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonAudioPlayer;

namespace osu_viewer
{
    public class WmpAudioPlayer : AudioPlayer
    {
        CommonPlaybackState mPlayState;
        public override CommonPlaybackState currentPlayState
        {
            get { return mPlayState; }
        }

        /// <summary>
        /// 當前撥放進度(毫秒)
        /// </summary>
        public override double playbackDuration
        {
            get
            {
                if (currentPlayState == CommonPlaybackState.stopped)
                    return 0;
                return wmp_player.Ctlcontrols.currentPosition * 1000;
            }

            set
            {
                wmp_player.Ctlcontrols.currentPosition = value / 1000.0;
            }
        }

        public override bool Loop
        {
            get { return wmp_player.settings.getMode("loop"); }
            set { wmp_player.settings.setMode("loop", value); }
        }

        public override int volume
        {
            get
            {
                return wmp_player.settings.volume;
            }

            set
            {
                wmp_player.settings.volume = value;
            }
        }

        public override double mediaLength
        {
            get
            {
                return wmp_player.newMedia(wmp_player.currentMedia.sourceURL).duration * 1000;
            }
        }

        private AxWMPLib.AxWindowsMediaPlayer wmp_player;
        
        public WmpAudioPlayer(AxWMPLib.AxWindowsMediaPlayer player)
        {
            wmp_player = player;
            wmp_player.PlayStateChange += Wmp_player_PlayStateChange;
        }

        private void Wmp_player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            Console.WriteLine(wmp_player.playState.ToString());
            switch (wmp_player.playState)
            {
                case WMPLib.WMPPlayState.wmppsPlaying:
                    mPlayState = CommonPlaybackState.playing;
                    break;
                case WMPLib.WMPPlayState.wmppsPaused:
                    mPlayState = CommonPlaybackState.paused;
                    break;
                case WMPLib.WMPPlayState.wmppsStopped:
                    mPlayState = CommonPlaybackState.stopped;
                    break;
                case WMPLib.WMPPlayState.wmppsMediaEnded:
                    mPlayState = CommonPlaybackState.mediaEnded;                    
                    break;
                default:
                    mPlayState = CommonPlaybackState.other;
                    break;
            }
            playStateChanged();
        }
        
        public override void pause()
        {
            wmp_player.Ctlcontrols.pause();
        }

        public override void play()
        {
            wmp_player.Ctlcontrols.play();
        }

        public override void resume()
        {
            wmp_player.Ctlcontrols.play();
        }

        public override void setMedia(object media)
        {
            WMPLib.IWMPMedia audio = media as WMPLib.IWMPMedia;
            wmp_player.currentPlaylist.clear();
            wmp_player.currentPlaylist.insertItem(0, audio);
            wmp_player.Ctlcontrols.playItem(wmp_player.currentPlaylist.Item[0]);
        }
        
        public override void stop()
        {
            wmp_player.Ctlcontrols.stop();
        }

        public override void play(double ms)
        {
            throw new NotImplementedException();
        }
    }
}
