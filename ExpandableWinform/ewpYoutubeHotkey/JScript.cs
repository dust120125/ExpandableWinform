
namespace ewpYoutubeHotkey
{
    public class JScript
    {
        public const string INIT_VIDEO_PAGE = "var lockTimes=0;var _maxVolume=1.0;var _volume=1.0;var vid=document.getElementsByTagName('video')[0];var mb=document.getElementsByClassName('ytp-mute-button ytp-button')[0];var vsl=document.getElementsByClassName('ytp-volume-slider')[0];";
        public const string VIDEO_PAGE_FUNCTIONS = "function lockVolume(){mb.onmouseenter=function(){vid.onvolumechange=saveVolume};mb.onmouseleave=function(){vid.onvolumechange=volumeLock};vsl.onmouseenter=function(){vid.onvolumechange=saveVolume};vsl.onmouseleave=function(){vid.onvolumechange=volumeLock}}function getMaxVolume(){var v=document.getElementsByTagName('video')[0];var vs=document.getElementsByClassName('ytp-volume-slider-handle')[0];var vol=v.volume;var att=vs.getAttribute('style');if(!att.includes('left'))return false;var percent=parseFloat(att.split(' ')[1].replace('px',''))/40;return vol/percent}function setVolume(vol){var v=document.getElementsByTagName('video')[0];var vb=document.getElementsByClassName('ytp-mute-button ytp-button')[0];var muted=v.muted;if(vol>0){if(muted)vb.click();var res=getMaxVolume();if(res)_maxVolume=res;v.volume=vol;_volume=vol;var vs=document.getElementsByClassName('ytp-volume-slider-handle')[0];var pos=40*vol/_maxVolume;vs.style='left: '+pos+'px'}else{if(!muted)vb.click();v.volume=0}}function saveVolume(){_volume=vid.volume}function volumeLock(){var v=document.getElementsByTagName('video')[0];v.onvolumechange=null;setVolume(_volume);v.onvolumechange=volumeLock;lockTimes++}";

    }
}
