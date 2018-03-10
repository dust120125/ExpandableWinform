using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_viewer
{
    public class OsuPlaylist
    {
        public const string Header = "osu!viewer savefile";
        static Random randomer = new Random(DateTime.Now.Millisecond);

        public string name;
        private List<OsuSong> originList;
        public List<OsuSong> randomList;

        public static bool random { get; private set; }
        public static bool loop { get; private set; }

        public List<OsuSong> Items { get { return originList; } }

        static OsuPlaylist()
        {
            random = true;
            loop = true;
        }

        public OsuPlaylist(string name, List<OsuSong> list)
        {
            this.name = name;
            originList = list;
            randomList = originList.ToList();
        }

        public OsuPlaylist(string name)
        {
            this.name = name;
            originList = new List<OsuSong>();
        }

        public void Add(OsuSong os)
        {
            if (originList == null) originList = new List<OsuSong>();
            originList.Add(os);
        }

        public string getFormatString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Header);
            sb.AppendLine(name);
            originList.ForEach(o =>
            {
                string id = o.BeatmapID;
                if (id == null) id = o.OsuFilename;
                sb.AppendLine(id);
            });
            return sb.ToString();
        }

        public OsuSong next(int index)
        {
            if (originList.Count == 0) return null;

            if (randomList == null)
            {
                reSet();
            }
            else if (randomList.Count == 0)
            {
                if (loop)
                {
                    reSet();
                }
                else return null;
            }
            if (!random && index == originList.Count - 1)
            {
                if (loop)
                    index = 0;
                else return null;
            }
            else index++;


            OsuSong result;

            if (random)
            {
                index = randomer.Next(randomList.Count);
                result = randomList[index];
                randomList.RemoveAt(index);
            }
            else
            {
                result = originList[index];
            }

            return result;
        }

        public static void setMode(string mode, bool set)
        {
            switch (mode)
            {
                case "shuffle":
                    random = set;
                    break;
                case "loop":
                    loop = set;
                    break;
            }
        }

        public void reSet()
        {
            randomList = originList.ToList();
        }

        public override string ToString()
        {
            return name;
        }

    }
}
