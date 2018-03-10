using System;

namespace NetAudio
{
    [Serializable]
    class Mp3Info
    {
        public string name;
        public int totalFrames;
        public double totalMilliseconds;
        public int sampleRate;
    }
}
