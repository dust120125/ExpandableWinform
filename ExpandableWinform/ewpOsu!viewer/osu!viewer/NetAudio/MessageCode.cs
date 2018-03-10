
namespace NetAudio
{
    public class MessageCode
    {
        public const byte requestMp3 = 1;
        public const byte mp3Info = 2;
        public const byte mp3Frame = 3;
        public const byte requestChunks = 4;

        public const byte play = 10;
        public const byte pause = 11;
        public const byte stop = 12;

        public const byte setPos = 20;

        public const byte requestOsuList = 30;
        public const byte osuList = 31;

        public const byte test = 0xFF;
    }
}
