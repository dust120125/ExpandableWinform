using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace osu_viewer
{
    class SongListChangedEventArgs : EventArgs
    {
        public List<OsuSong> SongList { get; private set; }
        public SongListChangedEventArgs(List<OsuSong> osuSongs)
        {
            this.SongList = osuSongs;
        }
    }

    class SongListRefreshedEventArgs : EventArgs
    {
        public List<OsuSong> SongList { get; private set; }
        public int remove { get; private set; }
        public int add { get; private set; }
        public float tookTime { get; private set; }
        public SongListRefreshedEventArgs(List<OsuSong> osuSongs, int remove, int add, float tookTime)
        {
            this.SongList = osuSongs;
            this.remove = remove;
            this.add = add;
            this.tookTime = tookTime;
        }
    }

    class OsuView
    {
        public static string _dataPath;
        public static string dataPath {
            get { return _dataPath; }
            set
            {
                _dataPath = value;
                cacheFile = _dataPath + SONGLIST_CACHE;
            }
        }

        public const String SONGLIST_CACHE = "cache.los";
        public static string cacheFile = SONGLIST_CACHE;

        public delegate void void_SingleInt_InvokeMethod(int value);
        public delegate void SongListChangedHandler(object sender, SongListChangedEventArgs e);
        public delegate void SongListRefreshedHandler(object sender, SongListRefreshedEventArgs e);

        public event SongListChangedHandler SongListChanged;
        public event SongListRefreshedHandler SongListRefreshed;

        private string path;
        private DirectoryInfo songDir;

        public List<OsuSong> osuSongs { get; private set; }
        public Dictionary<string, OsuSong> SongDictionary { get; private set; }
        static Thread myThread;
        static bool threadEnd = false;

        private static OsuView _osuView;

        public static OsuView getInstance()
        {
            if (_osuView == null) return null;
            return _osuView;
        }

        public static OsuView getInstance(String path)
        {
            if (_osuView == null) _osuView = new OsuView(path);
            return _osuView;
        }

        public OsuView(String path)
        {
            this.path = path;
            songDir = new DirectoryInfo(path + "\\songs");
            SongDictionary = new Dictionary<string, OsuSong>();
        }

        public List<OsuSong> getSongsList(Form parent, void_SingleInt_InvokeMethod progressBarUpdate)
        {
            if (myThread != null && myThread.ThreadState == ThreadState.Running)
            {
                threadEnd = true;
                myThread.Abort();
            }
            
            DirectoryInfo[] dirs = songDir.GetDirectories();
            double count = 0, length = dirs.Length;
            foreach (DirectoryInfo di in dirs)
            {
                foreach (FileInfo fi in di.GetFiles()){
                    if (fi.Extension == ".osu")
                    {
                        count++;
                        OsuSong os = new OsuSong(fi);
                        addToDictionary(os);                        
                        if (parent != null && progressBarUpdate != null)
                            parent.Invoke(progressBarUpdate, (int)(count / length * 100));
                        break;
                    }
                }
            }
            osuSongs = SongDictionary.Values.ToList();
            CreateSongListCache();            

            if (parent != null && progressBarUpdate != null)
                parent.Invoke(progressBarUpdate, 100);

            osuSongs.Sort((x, y) => { return x.TitleASCII.CompareTo(y.TitleASCII); });

            ActiveSongListChangedEvent();
            return osuSongs;
        }

        private void addToDictionary(OsuSong os)
        {
            string id = os.BeatmapID;
            if (id == null) id = os.OsuFilename;
            SongDictionary[id] = os;
        }

        private void removeFromDictionary(OsuSong os)
        {
            string id = os.BeatmapID;
            if (id == null) id = os.OsuFilename;
            SongDictionary.Remove(id);
        }

        private void ActiveSongListChangedEvent()
        {
            CreateSongListCache();
            SongListChanged(this, new SongListChangedEventArgs(osuSongs));
        }

        public int getSongsCount()
        {
            return songDir.GetDirectories().Length;
        }

        public void setOsuDir(String dir)
        {
            path = dir;
            songDir = new DirectoryInfo(dir + "\\songs");
        }

        public OsuSong getOsuSongByID(string id)
        {            
            SongDictionary.TryGetValue(id, out OsuSong os);
            return os;
        }

        public bool CreateSongListCache()
        {
             return CreateSongListCache(cacheFile);
        }

        public bool CreateSongListCache(string file)
        {
            try
            {
                Stream fs = File.Create(file);
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(fs, osuSongs);
                fs.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool ReadinSongListCache()
        {
            return ReadinSongListCache(cacheFile);
        }

        public bool ReadinSongListCache(string file)
        {
            try
            {
                Stream fs = File.OpenRead(file);
                BinaryFormatter deserializer = new BinaryFormatter();
                osuSongs = (List<OsuSong>)(deserializer.Deserialize(fs));
                osuSongs.Sort((x, y) => { return x.TitleASCII.CompareTo(y.TitleASCII); });
                fs.Close();
            }
            catch
            {
                return false;
            }
            foreach(OsuSong os in osuSongs)
            {
                addToDictionary(os);
            }
            //(myThread = new Thread(CheckIf_CacheUptoDate)).Start();
            //StartRefresh();
            ActiveSongListChangedEvent();
            return true;
        }
        
        private void CheckIf_CacheUptoDate()
        {
            threadEnd = false;
            int count = 0;
            foreach (DirectoryInfo di in songDir.GetDirectories())
            {
                foreach (FileInfo fi in di.GetFiles())
                {
                    if (fi.Extension == ".osu")
                    {
                        count++;
                        break;
                    }
                }
            }
            Thread.Sleep(1000);
            if (!threadEnd && count != osuSongs.Count)
            {
                string msg = ewpOsuViewer._instance.getString("str_folder_changed");
                MessageBox.Show(msg, "Notice.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void StartRefresh()
        {
            (myThread = new Thread(Refresh)).Start();
        }

        //載入cache後檢查硬碟與cache中的osuSongs差異，並修正
        private void Refresh()
        {
            int remove;
            int add;
            float tookTime;

            DateTime beforeTime = DateTime.Now;
            //currentDirs = 當前硬碟上所有譜面資料夾
            List<string> currentDirs = new List<string>();
            foreach (DirectoryInfo di in songDir.GetDirectories())
            {
                foreach (FileInfo fi in di.GetFiles())
                {
                    if (fi.Extension == ".osu")
                    {
                        currentDirs.Add(fi.DirectoryName.ToLower());
                        break;
                    }
                }
            }

            //紀錄稍後需從osuSongs中移除的索引值
            Stack<int> needToRemove = new Stack<int>();
            for (int i = 0; i < osuSongs.Count; i++)
            {
                string dir = osuSongs[i].OsuFilename;
                int index = dir.LastIndexOf('\\');
                dir = dir.Remove(index, dir.Length - index).ToLower();

                //當前osuSongs中的osu file資料夾是否存在於 currentDirs
                //存在則從 currentDirs 中除名
                //不存在則紀錄index，並在稍後從osuSongs中移除
                //最後留下的 currentDirs 為目前osuSongs中沒有但硬碟上存在的osu file資料夾清單
                index = currentDirs.IndexOf(dir);
                if (index != -1)
                {
                    currentDirs.RemoveAt(index);
                }
                else
                {
                    needToRemove.Push(i);
                }
            }

            remove = needToRemove.Count;
            add = currentDirs.Count;

            //songDir 資料夾無發生任何變化
            if (needToRemove.Count == 0 && currentDirs.Count == 0)
            {
                tookTime = (DateTime.Now - beforeTime).Milliseconds / (float)1000;
                SongListRefreshed(this, new SongListRefreshedEventArgs(osuSongs, remove, add, tookTime));
                return;
            }

            foreach (int index in needToRemove)
            {
                removeFromDictionary(osuSongs[index]);
                osuSongs.RemoveAt(index);                
            }
            currentDirs.ForEach(path =>
            {
                foreach (FileInfo fi in new DirectoryInfo(path).GetFiles())
                {
                    if (fi.Extension == ".osu")
                    {
                        OsuSong os = new OsuSong(fi);
                        osuSongs.Add(os);
                        addToDictionary(os);
                        break;
                    }
                }
            });

            tookTime = (DateTime.Now - beforeTime).Milliseconds / (float)1000;

            ActiveSongListChangedEvent();
            SongListRefreshed(this, new SongListRefreshedEventArgs(osuSongs, remove, add, tookTime));
        }
    }
}
