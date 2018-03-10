using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace osu_viewer
{
    [Serializable()]
    public class OsuSong 
    {
        //平均一筆 OsuSong 實體占用 417 bytes (841個OsuSong的平均結果)
        enum SearchMode { AllByChar, SingleByChar, AllByWord, SingleByWord }

        public struct SearchParameters
        {
            public int Count;
            public string Title;
            public string Artist;
            public string Creator;
            public string Source;
            public string Tags;
            public string[] all;
        }

        static SearchParameters searchPars;
        static char[] stringSeparatorsDot = { ',' };
        static char[] stringSeparatorsSpace = { ' ' };
        static char[] stringSeparatorsEquals = { '=' };

        string TitleNoFound
        {
            get { return ewpOsuViewer._instance.getString("str_title_no_found"); }
        }
        string ArtistNoFound 
        {
            get { return ewpOsuViewer._instance.getString("str_artist_no_found"); }
        }        

        public String Title
        {
            get { return (m_TitleUnicode != null && m_TitleUnicode != "") ? m_TitleUnicode : m_TitleASCII; }
        }
        public String Artist
        {
            get { return (m_ArtistUnicode != null && m_ArtistUnicode != "") ? m_ArtistUnicode : m_ArtistASCII; }
        }
        public String TitleASCII
        {
            get { return m_TitleASCII == null ? TitleNoFound : m_TitleASCII; }
            private set { m_TitleASCII = value; }
        }
        public String TitleUnicode
        {
            get { return m_TitleUnicode == null ? TitleNoFound : m_TitleUnicode; }
            private set { m_TitleUnicode = value; }
        }
        public String ArtistASCII
        {
            get { return m_ArtistASCII == null ? ArtistNoFound : m_ArtistASCII; }
            private set { m_ArtistASCII = value; }
        }
        public String ArtistUnicode
        {
            get { return m_ArtistUnicode == null ? ArtistNoFound : m_ArtistUnicode; }
            private set { m_ArtistUnicode = value; }
        }

        public String OsuFilename { get; private set; }        
        private String m_TitleASCII;
        private String m_TitleUnicode;
        private String m_ArtistASCII;
        private String m_ArtistUnicode;
        public String AudioFilename { get; private set; }
        public String Creator { get; private set; }
        public String Source { get; private set; }
        public String Tags { get; private set; }
        public String BeatmapID { get; private set; }
        public String BeatmapSetID { get; private set; }
        public int PreviewTime { get; private set; }
        public WMPLib.IWMPMedia Media { get {
                return File.Exists(AudioFilename) ? ewpOsuViewer.wmp_factory.newMedia(AudioFilename) : null;                
            } }

        public static bool UNICODE = true;
        public static bool ARTIST_FRIST = false;

        public OsuSong(string title)
        {
            m_TitleASCII = title;
            AudioFilename = title;
            OsuFilename = title;
        }

        public OsuSong(FileInfo file)
        {
            OsuFilename = file.FullName;

            StreamReader sr = file.OpenText();
            bool General = false, Metadata = false;

            String tmp;
            String[] infos;
            while((tmp = sr.ReadLine()) != null)
            {
                if (!(General && Metadata))
                {
                    if (tmp == "[General]")
                        General = true;
                    else if (tmp == "[Metadata]")
                        Metadata = true;
                }
                else if (tmp.StartsWith("[")) break;

                if (General || Metadata)
                {
                    if ((infos = tmp.Split(':')).Length > 0)
                    {
                        switch (infos[0])
                        {
                            case "Title":
                                m_TitleASCII = tmp.Substring(tmp.IndexOf(':') + 1);
                                break;
                            case "TitleUnicode":
                                m_TitleUnicode = tmp.Substring(tmp.IndexOf(':') + 1);
                                break;
                            case "AudioFilename":
                                AudioFilename = file.DirectoryName + "\\" + infos[1].Trim();
                                break;
                            case "Artist":
                                m_ArtistASCII = tmp.Substring(tmp.IndexOf(':') + 1);
                                break;
                            case "ArtistUnicode":
                                m_ArtistUnicode = tmp.Substring(tmp.IndexOf(':') + 1);
                                break;
                            case "Creator":
                                Creator = tmp.Substring(tmp.IndexOf(':') + 1);
                                break;
                            case "Source":
                                Source = tmp.Substring(tmp.IndexOf(':') + 1);
                                break;
                            case "Tags":
                                Tags = tmp.Substring(tmp.IndexOf(':') + 1);
                                break;
                            case "BeatmapID":
                                BeatmapID = infos[1].Trim();
                                if (BeatmapID == string.Empty) BeatmapID = null;
                                break;
                            case "BeatmapSetID":
                                BeatmapSetID = infos[1].Trim();
                                if (BeatmapSetID == string.Empty) BeatmapSetID = null;
                                break;
                            case "PreviewTime":
                                PreviewTime = Convert.ToInt32(infos[1].Trim());
                                break;
                        }
                    }
                }
            }
            sr.Close();
        }

        private static void clearSearchPars()
        {
            searchPars.Title = null;
            searchPars.Artist = null;
            searchPars.Creator = null;
            searchPars.Source = null;
            searchPars.Tags = null;
            searchPars.all = null;
        }

        public static void setSearchPars(string str)
        {
            clearSearchPars();
            string[] cmds = str.ToLower().Split(stringSeparatorsDot, StringSplitOptions.RemoveEmptyEntries);
            int count = 0;
            string all = "";
            foreach (string s in cmds)
            {
                if (s.Trim() == "") continue;
                string[] tmp = s.Trim().Split(stringSeparatorsEquals);
                switch (tmp[0])
                {
                    case "title":
                        if (tmp.Length != 1) {
                            searchPars.Title = tmp[1].Trim();
                            count++;
                        }
                        else all += s;                        
                        break;
                    case "artist":
                        if (tmp.Length != 1)
                        {
                            searchPars.Artist = tmp[1].Trim();
                            count++;
                        }
                        else all += s;
                        break;
                    case "creator":
                        if (tmp.Length != 1)
                        {
                            searchPars.Creator = tmp[1].Trim();
                            count++;
                        }
                        else all += s;
                        break;
                    case "source":
                        if (tmp.Length != 1)
                        {
                            searchPars.Source = tmp[1].Trim();
                            count++;
                        }
                        else all += s;
                        break;
                    case "tags":
                        if (tmp.Length != 1)
                        {
                            searchPars.Tags = tmp[1].Trim();
                            count++;
                        }
                        else all += s;
                        break;
                    default:
                        all += s;
                        break;
                }

                if (all != String.Empty)
                {
                    searchPars.all = all.Split(stringSeparatorsSpace, StringSplitOptions.RemoveEmptyEntries);
                    count += 1; //searchPars.all.Length;
                }
                searchPars.Count = count;
            }
        }

        //old_None_Used
        public bool isAbout(string str)
        {
            string[] strs = str.Split(' ');
            foreach (string s in strs)
            {
                if (m_TitleASCII != null && m_TitleASCII.IndexOf(s, StringComparison.OrdinalIgnoreCase) != -1) return true;
                if (m_TitleUnicode != null && m_TitleUnicode.IndexOf(s, StringComparison.OrdinalIgnoreCase) != -1) return true;
                if (m_ArtistASCII != null && m_ArtistASCII.IndexOf(s, StringComparison.OrdinalIgnoreCase) != -1) return true;
                if (m_ArtistUnicode != null && m_ArtistUnicode.IndexOf(s, StringComparison.OrdinalIgnoreCase) != -1) return true;
                if (Creator != null && Creator.IndexOf(s, StringComparison.OrdinalIgnoreCase) != -1) return true;
                if (Source != null && Source.IndexOf(s, StringComparison.OrdinalIgnoreCase) != -1) return true;
                if (Tags != null && Tags.IndexOf(s, StringComparison.OrdinalIgnoreCase) != -1) return true;
            }
            return false;
        }
        
        //搜尋 => 符合全部條件
        public bool isAbout()
        {
            int count = 0;
            if (searchPars.Title != null && (
                (m_TitleASCII != null && compareTo(m_TitleASCII, searchPars.Title, SearchMode.SingleByChar))
                || (m_TitleUnicode != null && compareTo(m_TitleUnicode, searchPars.Title, SearchMode.SingleByChar))
                ))
                if (++count == searchPars.Count) return true;

            if (searchPars.Artist != null && (
                (m_ArtistASCII != null && compareTo(m_ArtistASCII, searchPars.Artist, SearchMode.SingleByChar))
                || (m_ArtistUnicode != null && compareTo(m_ArtistUnicode, searchPars.Artist, SearchMode.SingleByChar))
                ))
                if (++count == searchPars.Count) return true;

            if (searchPars.Creator != null && Creator != null && compareTo(Creator, searchPars.Creator, SearchMode.SingleByChar))
                if (++count == searchPars.Count) return true;

            if (searchPars.Source != null && Source != null && compareTo(Source, searchPars.Source, SearchMode.SingleByChar))
                if (++count == searchPars.Count) return true;

            if (searchPars.Tags != null && Tags != null && compareTo(Tags, searchPars.Tags, SearchMode.SingleByWord))
                if (++count == searchPars.Count) return true;

            if (searchPars.all != null)
            {
                int equals = 0;
                foreach (string str in searchPars.all)
                {
                    if (m_TitleASCII != null && m_TitleASCII.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1) equals++;
                    else if (m_TitleUnicode != null && m_TitleUnicode.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1) equals++;
                    else if (m_ArtistASCII != null && m_ArtistASCII.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1) equals++;
                    else if (m_ArtistUnicode != null && m_ArtistUnicode.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1) equals++;
                    else if (Creator != null && Creator.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1) equals++;
                    else if (Source != null && Source.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1) equals++;
                    else if (Tags != null && Tags.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1) equals++;

                    if (equals == searchPars.all.Length)
                    {
                        count++;
                        break;
                    }
                }
            }

            if (count == searchPars.Count) return true;
            return false;
        }
        
        private bool compareTo(string str, string pars, SearchMode mode)
        {
            string[] tmp = pars.Split(stringSeparatorsSpace);
            int count = 0;

            if (mode == SearchMode.AllByChar)
            {
                foreach (string s in tmp)
                {
                    if ((str).IndexOf(s, StringComparison.OrdinalIgnoreCase) != -1)
                        if (++count == tmp.Length) return true;
                }
            }
            else if (mode == SearchMode.SingleByChar)
            {
                foreach (string s in tmp)
                {
                    if ((str).IndexOf(s, StringComparison.OrdinalIgnoreCase) != -1) return true;
                }
            }
            else if (mode == SearchMode.AllByWord)
            {
                foreach (string s in tmp)
                {
                    if ((' ' + str + ' ').IndexOf(' ' + s + ' ', StringComparison.OrdinalIgnoreCase) != -1)
                        if (++count == tmp.Length) return true;
                }
            }
            else if (mode == SearchMode.SingleByWord)
            {
                foreach (string s in tmp)
                {
                    if ((' ' + str + ' ').IndexOf(' ' + s + ' ', StringComparison.OrdinalIgnoreCase) != -1) return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            string result;
            if (ARTIST_FRIST)
            {
                result = UNICODE ? "[ " + Artist : "[ " + ArtistASCII;
                result += UNICODE ? " ] " + Title : " ] " + TitleASCII;
            }
            else
            {
                result = UNICODE ? Title + " [ " : TitleASCII + " [ ";
                result += UNICODE ? Artist + " ]" : ArtistASCII + " ]";
            }

            return result;
        }
    }    
}
