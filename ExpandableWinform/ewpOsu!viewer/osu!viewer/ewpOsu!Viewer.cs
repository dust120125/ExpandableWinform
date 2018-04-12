using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Drawing.Imaging;

using CommonAudioPlayer;
using Dust.Expandable;
using osu_viewer.Util;
using Dust.Image;

namespace osu_viewer
{
    public partial class ewpOsuViewer : Expandable
    {
        const bool DEBUG = false;

        public delegate void voidInvokeMethod();
        //delegate ListViewItem addIntoListView(string str);
        delegate void void_with_OsuSongList(List<OsuSong> list);

        //setting
        static bool SHOW_PLAYFAIL_MESSAGE = true;

        static bool PlayingCustomPlaylist = false;
        static string LoopMode = "all";
        static string savedLoopMode = null;

        private bool Loop;
        bool SINGLE_MEDIA_LOOP
        {
            get
            {
                return Loop;
            }
            set
            {
                Loop = value;
                audioPlayer.Loop = value;
            }
        }

        OsuView osuView;
        OsuSong currentOsuSong;
        List<OsuSong> mOriginList;
        private List<OsuSong> originList
        {
            get
            {
                return mOriginList;
            }
            set
            {
                mOriginList = value;
                GlobalResources.originList = mOriginList;
            }
        }
        OsuPlaylist currentList;

        static Form_Server serverForm;

        Form_PlayList Playlist_Manager;
        System.Windows.Forms.Timer progressBarController;
        private double currentMediaLenght;

        private static OsuSong currentMediaInfo;
        private bool readyNext = false, manualPlay = false;
        private Random random = new Random(DateTime.Now.Millisecond);
        public static AxWMPLib.AxWindowsMediaPlayer wmp_factory;

        private AudioPlayer audioPlayer, audioPlayer_bak;
        private List<OsuSong> originList_bak;
        private bool netClientMode;
        private bool isPlaying;

        private LRUDictionary<string, Image> transparentImageCache;
        private static readonly Font MEDIA_INFO_FONT = new Font(SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Bold);
        private bool opacitySliderClicked;

        public static Dictionary<string, string> _strRes { get; private set; }
        private bool componentsInitialized;
        public static ewpOsuViewer _instance { get; private set; }

        public ewpOsuViewer(ExpandableForm form) : base(form)
        {
            _instance = this;
        }

        public override string getTitle()
        {
            return "osu!viewer";
        }

        public enum SortOptions { Title, Artist, Creator, Source }

        private Config setting;
        private class Config : IConfig
        {
            [@Description("str_osu_path")]
            public string OsuPath;
            [@Description("str_start_up_play")]
            public bool StartupPlay = true;
            [@Description("str_start_up_refresh")]
            public bool StartupRefresh = true;
            [@Description("str_doubleclick_play")]
            public bool DoubleClickPlay = false;
            [@Description("str_media_preview")]
            public bool MediaPreview = true;
            [@Description("str_unicode_title")]
            public bool UnicodeTitle = true;
            [@Description("str_artist_first")]
            public bool ArtistFirst = false;
            [@Description("str_skip_ogg")]
            public bool SkipOgg = true;
            [@Description("str_move_index")]
            public bool MoveIndex = true;

            [@Description("str_volume")]
            [NumericOption(100, 0, 5)]
            public int Volume = 50;

            [NonSettable]
            public SortOptions SortBy = SortOptions.Title;
            [NonSettable]
            public bool showBackground = true;
            [NonSettable]
            public float backgroundOpacity = 0.7f;
            [NonSettable]
            public bool random = false;
            [NonSettable]
            public string loop = "none";

            [NonSettable]
            public int splitterDistance = 238;
        }

        protected override IConfig createConfig()
        {
            return setting = new Config();
        }

        protected override MenuStruct[] createMenuStructs()
        {
            return new MenuStruct[] {
                new MenuStruct(MenuStripField.File, "Load Osu", "load_osu", loadOsuToolStripMenuItem_Click),
                new MenuStruct(MenuStripField.File, "Reload Osu", "reload_osu", reloadOsuToolStripMenuItem_Click),
                new MenuStruct(MenuStripField.Self, "Net", "net", new List<MenuStruct>()
                {
                    new MenuStruct("Server", "server", serverToolStripMenuItem_Click),
                    new MenuStruct("Client Mode", "client_mode", clientModeToolStripMenuItem_Click),
                })
            };
        }

        protected override Dictionary<string, string> createStrRes()
        {
            return _strRes = new Dictionary<string, string>()
            {
                { "str_osu_path", "osu! Path" },
                { "str_start_up_play", "Play at start up" },
                { "str_start_up_refresh", "Refresh at start up" },
                { "str_doubleclick_play", "Double click to play" },
                { "str_media_preview", "Media preview" },
                { "str_unicode_title", "Use unicode character" },
                { "str_artist_first", "Put artist front of title" },
                { "str_skip_ogg", "Skip .ogg media files" },
                { "str_move_index", "Move index to current playing" },
                { "str_volume", "Volume" },

                { "str_toggle_play_pause", "Toggle play/pause" },
                { "str_play", "Play" },
                { "str_pause", "Pause" },
                { "str_next", "Next" },
                { "str_previous", "Previous" },
                { "str_volup", "Volume up" },
                { "str_voldown", "Volume down" },
                { "str_single_loop", "Loop play" },

                { "str_load_osu", "Load Osu" },
                { "str_reload_osu", "Reload Osu" },

                { "str_title", "Title" },
                { "str_title_ascii", "Title (ASCII)" },
                { "str_artist", "Artist" },
                { "str_artist_ascii", "Artist (ASCII)" },
                { "str_creator", "Creator" },
                { "str_source", "Source" },
                { "str_tags", "Tags" },
                { "str_beatmap_id", "BeatmapID" },
                { "str_beatmapset_id", "BeatmapSetID" },
                { "str_media_path", "MediaPath" },

                { "str_file_not_found",
                    "Cannot found file:\n\"{0}\"\n\nPlease check your disk for the file," +
                    " or press buttom \"{1}\" to rescan the Osu! Songs folder !" },
                { "str_folder_changed",
                    "Osu! Songs folder has changed.\nYou can press buttom \"Refresh\" to rescan the folder !" },

                { "str_file_broken",
                    "Oops! This \".osu\" file seems borken !\n"
                    + "File Path: {0}"},

                { "str_refresh", "Refresh" },
                { "str_refresh_msg", "Took Time: {0} sec, Remove: {1} Songs, Add: {2} Songs." },

                { "str_connect_fail", "Connect Fail" },

                { "str_title_no_found", "# Title No Found #" },
                { "str_artist_no_found", "# Artist No Found #" },

                { "str_net", "Net" },
                { "str_server", "Server" },
                { "str_client_mode", "Client mode" },

                { "str_show_all", "Show all" },
                { "str_playlist", "PlayList" },
                { "str_add_playlist", "Add to playlist" },
                { "str_artist_first_short", "Artist Frist"},
                { "str_sort_by", "Sort by"},
                { "str_background", "Background"},
                { "str_find_index", "Find index"}
            };
        }

        protected override Dictionary<string, string[]> createComboBoxItemRes()
        {
            return new Dictionary<string, string[]>() { };
        }

        protected override Hotkey[] createHotkeys()
        {
            return new Hotkey[]
            {
                new Hotkey("Toggle play/pause", "toggle_play_pause", null, togglePlayPause, 0, false),
                new Hotkey("Play", "play", new Keys[]{ Keys.Z, Keys.X, Keys.I}, play, 0, false),
                new Hotkey("Pause", "pause", new Keys[]{ Keys.Z, Keys.X, Keys.K}, pause, 0, false),
                new Hotkey("Next", "next", new Keys[]{ Keys.Z, Keys.X, Keys.P}, Next, 0, false),
                new Hotkey("Previous", "previous", new Keys[]{ Keys.Z, Keys.X, Keys.O}, Previous, 0, false),
                new Hotkey("Volume up", "volup", new Keys[]{ Keys.Z, Keys.X, Keys.Oemplus}, volumeUp, 100, true),
                new Hotkey("Volume down", "voldown", new Keys[]{ Keys.Z, Keys.X, Keys.OemMinus}, volumeDown, 100, true),
                new Hotkey("Single loop", "single_loop", new Keys[]{ Keys.Z, Keys.X, Keys.L}, toggleLoop, 150, false),
            };
        }

        public Form getParentForm()
        {
            return _form;
        }

        private void Init()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(ewpOsuViewer));
            ewpOsuViewer.wmp_factory = new AxWMPLib.AxWindowsMediaPlayer();
            ((ISupportInitialize)(ewpOsuViewer.wmp_factory)).BeginInit();
            this.panel_PlayControls.Controls.Add(ewpOsuViewer.wmp_factory);
            ewpOsuViewer.wmp_factory.Enabled = true;
            ewpOsuViewer.wmp_factory.Location = new Point(0, 31);
            ewpOsuViewer.wmp_factory.Name = "wmp_player";
            ewpOsuViewer.wmp_factory.OcxState = ((AxHost.State)(resources.GetObject("wmp_player.OcxState")));
            ewpOsuViewer.wmp_factory.Size = new Size(398, 36);
            ((ISupportInitialize)(ewpOsuViewer.wmp_factory)).EndInit();
        }

        public override void run()
        {
            InitializeComponent();
            Init();

            OsuView.dataPath = dataPath;
            Form_PlayList.rootDir = dataPath;
            transparentImageCache = new LRUDictionary<string, Image>(10);

            audioPlayer = new WmpAudioPlayer(wmp_player);
            wmp_player.settings.volume = setting.Volume;
            wmp_player.uiMode = "full";
            //wmp_player.PlayStateChange += Wmp_player_PlayStateChange;  
            audioPlayer.PlayStateChanged += AudioPlayer_PlayStateChanged;

            progressBarController = new System.Windows.Forms.Timer();
            progressBarController.Interval = 100;
            progressBarController.Tick += ProgressBarController_Tick;
            progressBarController.Start();

            progressBar_mediaPosition.MouseMove += ProgressBar_mediaPosition_MouseMove;
            progressBar_mediaPosition.MouseLeave += ProgressBar_mediaPosition_MouseLeave;

            slider_Volume.Value = setting.Volume;
            slider_Volume.ValueChanged += Slider_Volume_ValueChanged;

            label_PlayingProgress.Text = null;
            label_MediaName.Text = null;

            listBox_Songs.DoubleClick += listBox_Songs_DoubleClick;
            listBox_Songs.Click += ListBox_Songs_Click;
            listBox_Songs.KeyDown += ListBox_Songs_KeyDown;
            listBox_Songs.KeyUp += ListBox_Songs_KeyUp;
            listBox_Songs.SelectedIndexChanged += ListBox_Songs_SelectedIndexChanged;

            listView_MediaInfo.DoubleClick += ListView_MediaInfo_DoubleClick;
            listView_MediaInfo.SizeChanged += ListView_MediaInfo_SizeChanged;

            splitContainer1.SplitterDistance = setting.splitterDistance;
            splitContainer1.SplitterMoved += setMediaInfoTitleSize;
            mainPanel.SizeChanged += ResizeControls;

            listBox_Songs.Location = new Point(0, 0);
            listView_MediaInfo.Location = new Point(0, 0);

            if (setting.OsuPath != null) TryLoadCache();

            checkBox_Unicode.Checked = setting.UnicodeTitle;
            checkBox_ArtistFirst.Checked = setting.ArtistFirst;
            checkBox_Background.Checked = setting.showBackground;

            trackBar_BgOpacity.Value = (int)(setting.backgroundOpacity * 100);
            trackBar_BgOpacity.MouseDown += TrackBar_BgOpacity_MouseDown;
            trackBar_BgOpacity.MouseUp += TrackBar_BgOpacity_MouseUp;

            switch (setting.SortBy)
            {
                case SortOptions.Title:
                    comboBox_SortBy.SelectedIndex = 0;
                    break;
                case SortOptions.Artist:
                    comboBox_SortBy.SelectedIndex = 1;
                    break;
                case SortOptions.Creator:
                    comboBox_SortBy.SelectedIndex = 2;
                    break;
                case SortOptions.Source:
                    comboBox_SortBy.SelectedIndex = 3;
                    break;
            }

            setLoopMode(setting.loop);
            setRandomPlay(setting.random);

            ResizeControls(null, null);
        }

        private void ListView_MediaInfo_SizeChanged(object sender, EventArgs e)
        {
            if (!setting.showBackground) return;
            updateMediaBackgroundImage();
        }

        private void updateMediaBackgroundImage()
        {
            if (currentList == null || listBox_Songs.SelectedIndex < 0) return;

            OsuSong os = currentList.Items[listBox_Songs.SelectedIndex];
            if (os == null || os.BackgroundFilenameWithoutPath == null) return;
            listView_MediaInfo.BackgroundImage = getTransparentImage(os,
                    listView_MediaInfo.ClientSize.Width, listView_MediaInfo.ClientSize.Height, Color.White, setting.backgroundOpacity);
        }

        private void TrackBar_BgOpacity_MouseUp(object sender, MouseEventArgs e)
        {
            opacitySliderClicked = false;
            transparentImageCache.Clear();

            if (!setting.showBackground) return;
            updateMediaBackgroundImage();
        }

        private void TrackBar_BgOpacity_MouseDown(object sender, MouseEventArgs e)
        {
            opacitySliderClicked = true;
        }

        private void TrackBar_BgOpacity_ValueChanged(object sender, EventArgs e)
        {
            setting.backgroundOpacity = trackBar_BgOpacity.Value / 100f;

            if (opacitySliderClicked) return;
            transparentImageCache.Clear();

            if (!setting.showBackground) return;
            updateMediaBackgroundImage();
        }

        private void Slider_Volume_ValueChanged(object sender, EventArgs e)
        {
            audioPlayer.volume = slider_Volume.Value;
        }

        private void setMediaInfoTitleSize(object sender, SplitterEventArgs e)
        {
            int width = listView_MediaInfo.Width - 21 < 120 ? 120 : listView_MediaInfo.Width - 21;
            listView_MediaInfo.TileSize = new Size(width, listView_MediaInfo.TileSize.Height);
            setting.splitterDistance = splitContainer1.SplitterDistance;
            isConfigChanged = true;
        }

        private void ResizeControls(object sender, EventArgs e)
        {
            panel_Lists.Size = new Size(panel_Lists.Width, this.panel_Features2.Location.Y - this.panel_Lists.Location.Y);

            int width = listView_MediaInfo.Width - 21 < 180 ? 180 : listView_MediaInfo.Width - 21;
            listView_MediaInfo.TileSize = new Size(width, listView_MediaInfo.TileSize.Height);

            int x = mainPanel.Size.Width - comboBox_SortBy.Size.Width - 8;
            comboBox_SortBy.Location = new Point(x, 5);

            x = comboBox_SortBy.Location.X - label1.Size.Width - 6;
            label1.Location = new Point(x, 8);

            x = label1.Location.X - this.trackBar_BgOpacity.Size.Width - 6;
            this.trackBar_BgOpacity.Location = new System.Drawing.Point(x, 8);

            x = trackBar_BgOpacity.Location.X - checkBox_Background.Size.Width - 6;
            checkBox_Background.Location = new Point(x, 8);

            x = panel_PlayControls.ClientSize.Width - button_LoopPlay.Size.Width - 25;
            button_LoopPlay.Location = new Point(x, 39);

            x = button_LoopPlay.Location.X - button_RandomPlay.Size.Width - 5;
            button_RandomPlay.Location = new Point(x, 39);

            progressBar_refreshList.Width = (int)(listBox_Songs.Width * 0.6);
            progressBar_refreshList.Location = new Point((listBox_Songs.Width - progressBar_refreshList.Width) / 2, (listBox_Songs.Height - progressBar_refreshList.Height) / 2);
        }

        public override void quit()
        {
            wmp_player.close();
            wmp_player.Dispose();
            wmp_player = null;

            wmp_factory.close();
            wmp_factory = null;

            audioPlayer.Dispose();
            audioPlayer = null;

            progressBarController.Stop();
            progressBarController = null;

            if (serverForm != null)
                serverForm.Dispose();
            if (GlobalResources.server != null && GlobalResources.server.isRunning)
                GlobalResources.server.Stop();
            setting.Volume = slider_Volume.Value;
            isConfigChanged = true;

            currentList = null;
            currentOsuSong = null;

            transparentImageCache = null;

            mainPanel.Controls.Clear();
        }

        private void ListBox_Songs_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (currentList != null)
                    setCurrentMedia(currentList.Items[listBox_Songs.SelectedIndex], 0);
            }
        }

        private void ListBox_Songs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_Songs.SelectedIndex != -1)
                if (currentList != null && listBox_Songs.SelectedIndex < currentList.Items.Count)
                {
                    setMediaInfoListView(currentList.Items[listBox_Songs.SelectedIndex]);
                }
            UpdateCountLabel();
        }

        private void ListView_MediaInfo_DoubleClick(object sender, EventArgs e)
        {
            if (listView_MediaInfo.SelectedItems.Count > 0)
            {
                string[] infos = listView_MediaInfo.SelectedItems[0].Text.Split(':');
                StringBuilder sb = new StringBuilder();
                sb.Append(infos[1]);
                for (int i = 2; i < infos.Length; i++)
                {
                    sb.Append(":" + infos[i]);
                }
                //MessageBox.Show(sb.ToString());
                Clipboard.SetText(sb.ToString());
            }
        }

        private void ListBox_Songs_KeyDown(object sender, KeyEventArgs e)
        {
            //攔截鍵盤輸入
            e.Handled = true;
            e.SuppressKeyPress = true;

            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)
            {
                int index = listBox_Songs.SelectedIndex + 1 > listBox_Songs.Items.Count - 1 ? 0 : listBox_Songs.SelectedIndex + 1;
                if (!setting.DoubleClickPlay) setCurrentMedia(index, 1);
                else listBox_Songs.SetSelected(index, true);
            }
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
            {
                int index = listBox_Songs.SelectedIndex - 1 < 0 ? listBox_Songs.Items.Count - 1 : listBox_Songs.SelectedIndex - 1;
                if (!setting.DoubleClickPlay) setCurrentMedia(index, 1);
                else listBox_Songs.SetSelected(index, true);
            }
        }

        private void ListBox_Songs_Click(object sender, EventArgs e)
        {
            if (!setting.DoubleClickPlay && currentList != null)
            {
                currentList.reSet();
                if (listBox_Songs.SelectedIndex >= 0)
                    setCurrentMedia(listBox_Songs.SelectedIndex, 1);
            }
        }

        private void listBox_Songs_DoubleClick(object sender, EventArgs e)
        {
            if (setting.DoubleClickPlay && currentList != null)
            {
                currentList.reSet();
                if (listBox_Songs.SelectedIndex >= 0)
                    setCurrentMedia(listBox_Songs.SelectedIndex, 1);
            }
        }

        private void ProgressBar_mediaPosition_MouseLeave(object sender, EventArgs e)
        {
            label_ProgressTime.Visible = false;
        }

        private void ProgressBar_mediaPosition_MouseMove(object sender, EventArgs e)
        {
            if (!label_ProgressTime.Visible) label_ProgressTime.Visible = true;

            double x = progressBar_mediaPosition.PointToClient(Cursor.Position).X;
            double progress = x / progressBar_mediaPosition.Width;
            int time = (int)(progress * currentMediaLenght);

            label_ProgressTime.Text = (time / 60) + ":" + (time % 60);
            label_ProgressTime.Location = new Point((int)x - 10, 5);
        }

        private void AudioPlayer_PlayStateChanged(object sender, PlayStateChangedEventArgs e)
        {
            _form.Invoke(new playStateChangeHandler(PlayStateChange_Task), e.currentState);
        }

        private delegate void playStateChangeHandler(CommonPlaybackState state);
        private void PlayStateChange_Task(CommonPlaybackState state)
        {
            if (state == CommonPlaybackState.playing)
            {
                isPlaying = true;
                button_Play.BackgroundImage = Properties.Resources.pause;
                progressBarController.Start();
                UpdateMediaInfo();
                UpdatePlayingProgressBar();
            }
            else if (state == CommonPlaybackState.mediaEnded)
            {
                if (!SINGLE_MEDIA_LOOP)
                    readyNext = true;
            }
            else if (state == CommonPlaybackState.stopped)
            {
                isPlaying = false;
                button_Play.BackgroundImage = Properties.Resources.play;
                UpdatePlayingProgressBar();
                progressBarController.Stop();
                audioPlayer.playbackDuration = audioPlayer.playbackDuration;
                if (readyNext)
                {
                    Next();
                    readyNext = false;
                    manualPlay = true;
                }
                else if (manualPlay)
                {
                    audioPlayer.play();
                    manualPlay = false;
                }
            }
            if (state == CommonPlaybackState.paused)
            {
                isPlaying = false;
                button_Play.BackgroundImage = Properties.Resources.play;
                UpdatePlayingProgressBar();
                progressBarController.Stop();
                if (!netClientMode)
                    audioPlayer.playbackDuration = audioPlayer.playbackDuration;
            }
        }

        private void ProgressBarController_Tick(object sender, EventArgs e)
        {
            UpdatePlayingProgressBar();
        }

        private void UpdateMediaInfo()
        {
            if (currentOsuSong != null)
            {
                currentMediaLenght = audioPlayer.mediaLength / 1000;
                label_MediaName.Text = checkBox_Unicode.Checked ? currentOsuSong.Title : currentOsuSong.TitleASCII;
            }
            else label_MediaName.Text = null;

            //setMediaInfoListView(currentOsuSong);
        }

        private void setMediaInfoListView(OsuSong os)
        {
            if (os == null)
            {
                listView_MediaInfo.Items.Clear();
                return;
            }
            else if (currentMediaInfo != null && os.OsuFilename == currentMediaInfo.OsuFilename)
            {
                return;
            }
            listView_MediaInfo.BeginUpdate();
            listView_MediaInfo.Items.Clear();
            listView_MediaInfo.Items.Add(getString("str_title") + ": " + os.Title);
            listView_MediaInfo.Items.Add(getString("str_title_ascii") + ": " + os.TitleASCII);
            listView_MediaInfo.Items.Add(getString("str_artist") + ": " + os.Artist);
            listView_MediaInfo.Items.Add(getString("str_artist_ascii") + ": " + os.ArtistASCII);
            listView_MediaInfo.Items.Add(getString("str_creator") + ": " + os.Creator);
            listView_MediaInfo.Items.Add(getString("str_source") + ": " + os.Source);
            listView_MediaInfo.Items.Add(getString("str_tags") + ": " + os.Tags);
            listView_MediaInfo.Items.Add(getString("str_beatmap_id") + ": " + os.BeatmapID);
            listView_MediaInfo.Items.Add(getString("str_beatmapset_id") + ": " + os.BeatmapSetID);
            listView_MediaInfo.Items.Add(getString("str_media_path") + ": " + os.AudioFilename);
            currentMediaInfo = os;

            foreach (ListViewItem item in listView_MediaInfo.Items)
            {
                item.Font = MEDIA_INFO_FONT;
            }

            if (setting.showBackground && os.BackgroundFilename != null)
            {
                listView_MediaInfo.BackgroundImage = getTransparentImage(os,
                    listView_MediaInfo.ClientSize.Width, listView_MediaInfo.ClientSize.Height, Color.White, setting.backgroundOpacity);
            }

            listView_MediaInfo.EndUpdate();
        }

        private Image getTransparentImage(OsuSong os, int width, int height, Color backCol, float opacity)
        {
            Image image;
            if (!transparentImageCache.ContainsKey(os.BackgroundFilename))
            {
                image = getTransparentImage(os.BackgroundImage, Color.White, opacity);
                transparentImageCache.Add(os.BackgroundFilename, image);
            }
            else
            {
                image = transparentImageCache[os.BackgroundFilename];
            }

            return ConvertImageSize(image, width, height);
        }

        private Image getTransparentImage(Image image, int width, int height, Color backCol, float opacity)
        {
            Image result = ImageTransparentSimulator.getTransparentImage(image, backCol, opacity);
            return ConvertImageSize(result, width, height);
        }

        private Image getTransparentImage(Image image, Color backCol, float opacity)
        {
            return ImageTransparentSimulator.getTransparentImage(image, backCol, opacity);
        }

        private Image ConvertImageSize(Image image, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.DrawImage(image, 0, 0, width, height);
            }

            return bmp;
        }

        private void UpdatePlayingProgressBar()
        {
            try
            {
                int value = (int)(audioPlayer.playbackDuration / 1000.0 / currentMediaLenght * progressBar_mediaPosition.Maximum);
                if (value < 0) return;
                progressBar_mediaPosition.Value = value > progressBar_mediaPosition.Maximum ? progressBar_mediaPosition.Maximum : value;
                label_PlayingProgress.Text = audioPlayer.currentDurationString();
            }
            catch { }
        }

        private void ReloadOsu()
        {
            if (Directory.Exists(setting.OsuPath + "\\songs"))
            {
                Thread thread = new Thread(LoadSonglist);
                progressBar_refreshList.Value = 0;
                progressBar_refreshList.Visible = true;
                thread.Start();
            }
        }

        private void LoadSonglist()
        {
            osuView.getSongsList(_form, new OsuView.void_SingleInt_InvokeMethod(setLoadSonglist_ProgressValue));
            Thread.Sleep(600);
            _form.Invoke(new voidInvokeMethod(Invoke_LoadSonglist));
        }

        private void Invoke_LoadSonglist()
        {
            UpdateSongsList(originList);
            textBox_SearchSongs.Text = null;
            if (currentList.Items.Count != 0)
                setCurrentMedia(random.Next(currentList.Items.Count), 1);
            closeProgressBar();
        }

        private void setLoadSonglist_ProgressValue(int value)
        {
            progressBar_refreshList.Value = value;
        }

        private void closeProgressBar()
        {
            progressBar_refreshList.Visible = false;
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            RefreshSongList();
        }

        private void RefreshSongList()
        {
            if (osuView != null)
                osuView.StartRefresh();
        }

        private void setCurrentMedia(int index, int mode) //mode 1 = from PreviewTime, 0 = from Head
        {
            if (currentList.Items.Count > 0)
            {
                if (currentOsuSong != currentList.Items[index])
                {
                    if (listBox_Songs.Items.Count != currentList.Items.Count)
                    {
                        UpdateListBox();
                    }

                    if (setting.MoveIndex) listBox_Songs.SelectedIndex = index;

                    currentOsuSong = currentList.Items[index];
                    if (currentOsuSong.AudioFilename == null || currentOsuSong.AudioFilename == String.Empty)
                        PlayMediaFail(currentOsuSong, PlayFail.OsuFileBroken);

                    if (setting.SkipOgg && currentOsuSong.AudioFilename.EndsWith(".ogg"))
                        PlayMediaFail(currentOsuSong, PlayFail.NoMessage);

                    WMPLib.IWMPMedia audio = null;
                    if (!netClientMode)
                        audio = currentOsuSong.Media;

                    if (audio != null || netClientMode)
                    {
                        if (netClientMode)
                        {
                            audioPlayer.setMedia(currentOsuSong.AudioFilename);
                            if (setting.MediaPreview && mode == 1)
                                audioPlayer.play(currentOsuSong.PreviewTime);
                            else
                                audioPlayer.play();

                        }
                        else
                        {
                            audioPlayer.setMedia(audio);
                            if (setting.MediaPreview && mode == 1)
                                audioPlayer.playbackDuration = currentOsuSong.PreviewTime;

                        }

                        UpdateMediaInfo();

                    }
                    else
                    {
                        PlayMediaFail(currentOsuSong, PlayFail.AudioFileNoFound);
                    }
                }
            }
            else
            {
                currentOsuSong = null;
                UpdateMediaInfo();
            }
        }

        private void setCurrentMedia(OsuSong media, int mode) //mode 1 = from PreviewTime, 0 = from Head
        {
            int tmp = currentList.Items.FindIndex(o => o.AudioFilename == media.AudioFilename);
            setCurrentMedia(tmp, mode);
        }

        enum PlayFail { AudioFileNoFound, OsuFileBroken, NoMessage }

        private void PlayMediaFail(OsuSong failMedia, PlayFail mode)
        {
            if (SHOW_PLAYFAIL_MESSAGE)
            {
                string msg;
                switch (mode)
                {
                    case PlayFail.AudioFileNoFound:
                        msg = string.Format(getString("str_file_not_fount"), currentOsuSong.AudioFilename, getString("str_refresh"));
                        MessageBox.Show(msg, "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case PlayFail.OsuFileBroken:
                        msg = string.Format(getString("str_file_broken"), currentOsuSong.OsuFilename);
                        MessageBox.Show(msg, "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }

            if (currentList.Items.Count > 1)
            {
                currentOsuSong = null;
                UpdateMediaInfo();
                Next();
            }
            else
            {
                UpdateMediaInfo();
                audioPlayer.stop();
            }
        }

        List<OsuSong> previousList = new List<OsuSong>();
        List<OsuSong> nextList = new List<OsuSong>();

        private void Next(params object[] args)
        {
            if (SINGLE_MEDIA_LOOP) return;

            if (OsuPlaylist.random)
            {
                if (currentOsuSong != null && currentList.Items.Contains(currentOsuSong))
                {
                    previousList.Insert(0, currentOsuSong);
                    if (previousList.Count > 10) previousList.RemoveAt(9);
                }

                if (nextList.Count > 0)
                {
                    setCurrentMedia(nextList[0], 0);
                    nextList.RemoveAt(0);
                }
                else
                {
                    PlayNextMedia();
                }
            }
            else
            {
                int index = listBox_Songs.SelectedIndex + 1;
                index = index > listBox_Songs.Items.Count - 1 ? 0 : index;
                if (currentList.Items[index] != null)
                    setCurrentMedia(index, 0);
            }
        }

        private void Previous(params object[] args)
        {
            if (OsuPlaylist.random)
            {
                if (currentOsuSong != null && currentList.Items.Contains(currentOsuSong))
                {
                    nextList.Insert(0, currentOsuSong);
                    if (nextList.Count > 10) nextList.RemoveAt(9);
                }

                if (previousList.Count > 0)
                {
                    setCurrentMedia(previousList[0], 0);
                    previousList.RemoveAt(0);
                }
                else
                {
                    PlayNextMedia();
                }
            }
            else
            {
                int index = listBox_Songs.SelectedIndex - 1;
                index = index < 0 ? listBox_Songs.Items.Count - 1 : index;
                if (currentList.Items[index] != null)
                    setCurrentMedia(index, 0);
            }
        }

        private void PlayNextMedia()
        {
            OsuSong tmp;
            if ((tmp = currentList.next(listBox_Songs.SelectedIndex)) != null)
            {
                if (!OsuPlaylist.random)
                {
                    int index = listBox_Songs.SelectedIndex + 1;
                    index = index > listBox_Songs.Items.Count - 1 ? 0 : index;
                }

                setCurrentMedia(tmp, 0);
            }
            else
            {
                audioPlayer.stop();
            }
        }

        private void progressBar_mediaPosition_Click(object sender, EventArgs e)
        {
            double x = progressBar_mediaPosition.PointToClient(Cursor.Position).X;
            double progress = x / progressBar_mediaPosition.Width;
            audioPlayer.playbackDuration = progress * currentMediaLenght * 1000;
            UpdatePlayingProgressBar();
        }

        private void LoadOsuFolder(String path)
        {
            setting.OsuPath = path;
            osuView = newOsuView(setting.OsuPath);
            ReloadOsu();
        }

        private void TryLoadCache()
        {
            Console.WriteLine("Try Load Cache");
            if (Directory.Exists(setting.OsuPath))
            {
                if (File.Exists(OsuView.cacheFile))
                {
                    osuView = newOsuView(setting.OsuPath);
                    if (osuView.ReadinSongListCache())
                    {
                        if (setting.StartupRefresh) osuView.StartRefresh();
                        //originList = osuView.osuSongs;
                        //UpdateSongsList(originList);
                        if (setting.StartupPlay && currentList.Items.Count != 0)
                            setCurrentMedia(random.Next(currentList.Items.Count), 1);
                        return;
                    }
                }
                LoadOsuFolder(setting.OsuPath);
            }
        }

        private OsuView newOsuView(string path)
        {
            OsuView tmp = OsuView.getInstance(setting.OsuPath);
            tmp.SongListChanged += UpdateOriginList;
            tmp.SongListRefreshed += Tmp_SongListRefreshed;
            return tmp;
        }

        private void Tmp_SongListRefreshed(object sender, SongListRefreshedEventArgs e)
        {
            /*
            DateTime now = DateTime.Now;
            StreamWriter sw = File.CreateText(now.ToString("yyyy-MM-dd.hhmmss") + ".txt");
            sw.WriteLine("[" + now.ToString("yyyy/MM/dd hh:mm:ss") + "]");
            sw.WriteLine("Took Time: " + e.tookTime + " sec, Remove: " + e.remove + " Songs, Add: " + e.add + " Songs.");
            sw.Close();
            */

            _form.notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _form.notifyIcon.BalloonTipTitle = getString("str_refresh");
            string msg = string.Format(getString("str_refresh_msg"), e.tookTime, e.remove, e.add);
            _form.notifyIcon.BalloonTipText = msg;
            _form.notifyIcon.ShowBalloonTip(2000);
        }

        private void UpdateOriginList(object sender, SongListChangedEventArgs e)
        {
            if (!PlayingCustomPlaylist)
            {
                originList = e.SongList;
                UpdateSongsList(originList);
            }
        }

        private void checkBox_ArtistFrist_CheckedChanged(object sender, EventArgs e)
        {
            OsuSong.ARTIST_FRIST = checkBox_ArtistFirst.Checked;
            setting.ArtistFirst = checkBox_ArtistFirst.Checked;
            resetMediaListText();
        }

        private void checkBox_Unicode_CheckedChanged(object sender, EventArgs e)
        {
            OsuSong.UNICODE = checkBox_Unicode.Checked;
            setting.UnicodeTitle = checkBox_Unicode.Checked;
            resetMediaListText();
        }

        private void checkBox_Background_CheckedChanged(object sender, EventArgs e)
        {
            setting.showBackground = checkBox_Background.Checked;
            isConfigChanged = true;
            if (checkBox_Background.Checked)
            {
                updateMediaBackgroundImage();
            }
            else
            {
                listView_MediaInfo.BackgroundImage = null;
            }
        }

        private void resetMediaListText()
        {
            listBox_Songs.BeginUpdate();
            for (int i = 0; i < listBox_Songs.Items.Count; i++)
            {
                listBox_Songs.Items[i] = currentList.Items[i].ToString();
            }
            listBox_Songs.EndUpdate();
            UpdateMediaInfo();
            if (Playlist_Manager != null) Playlist_Manager.UpdateMediaList();
        }

        private void comboBox_SortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_SortBy.SelectedIndex)
            {
                case 0:
                    setting.SortBy = SortOptions.Title;
                    break;
                case 1:
                    setting.SortBy = SortOptions.Artist;
                    break;
                case 2:
                    setting.SortBy = SortOptions.Creator;
                    break;
                case 3:
                    setting.SortBy = SortOptions.Source;
                    break;
            }
            SortMediaList();
        }

        private void SortMediaList()
        {
            if (currentList != null && currentList.Items.Count > 1)
            {
                switch (setting.SortBy)
                {
                    case SortOptions.Title:
                        currentList.Items.Sort((x, y) => { return x.TitleASCII.CompareTo(y.TitleASCII); });
                        break;
                    case SortOptions.Artist:
                        currentList.Items.Sort((x, y) => { return x.ArtistASCII.CompareTo(y.ArtistASCII); });
                        break;
                    case SortOptions.Creator:
                        currentList.Items.Sort((x, y) => { return x.Creator.CompareTo(y.Creator); });
                        break;
                    case SortOptions.Source:
                        currentList.Items.Sort((x, y) =>
                        {
                            if (x.Source == null || x.Source == "") return 1;
                            if (y.Source == null || y.Source == "") return -1;
                            return x.Source.CompareTo(y.Source);
                        });
                        break;
                }
                resetMediaListText();
                if (currentOsuSong != null)
                {
                    int index = currentList.Items.IndexOf(currentOsuSong);
                    if (index != -1) listBox_Songs.SetSelected(index, true);
                }
            }
        }

        private void UpdateCountLabel()
        {
            int index = listBox_Songs.SelectedIndex + 1;
            int count = listBox_Songs.Items.Count;
            label_CurrentIndex.Text = index + "/" + count;
        }

        private void UpdateListBox()
        {
            listBox_Songs.Items.Clear();
            listBox_Songs.BeginUpdate();
            for (int i = 0; i < currentList.Items.Count; i++)
            {
                string title = currentList.Items[i].ToString();
                //if (title == null) title = "# Title No Found #";
                listBox_Songs.Items.Add(title);
            }
            listBox_Songs.EndUpdate();
            UpdateCountLabel();
        }

        private void UpdateSongsList(List<OsuSong> list)
        {
            currentList = new OsuPlaylist("current", list);
            _form.Invoke(new voidInvokeMethod(UpdateListBox));
        }

        private void SearchSongs(string str)
        {
            OsuSong.setSearchPars(str);
            IEnumerable<OsuSong> list = originList.Where<OsuSong>(e => e.isAbout());
            UpdateSongsList(list.ToList<OsuSong>());
        }

        private void textBox_SearchSongs_TextChanged(object sender, EventArgs e)
        {
            if (textBox_SearchSongs.Text != "" && textBox_SearchSongs.Text != null)
                SearchSongs(textBox_SearchSongs.Text);
            else if (originList != null)
            {
                UpdateSongsList(originList);
            }

            if (currentOsuSong != null)
            {
                int index = currentList.Items.IndexOf(currentOsuSong);
                if (index != -1) listBox_Songs.SetSelected(index, true);
            }
            previousList.Clear();
            nextList.Clear();
            //setCurrentMedia(0, 1);
        }

        private void button_PlayList_Click(object sender, EventArgs e)
        {
            Start_PlaylistManager();
        }

        private void setRandomPlay(bool value)
        {
            nextList.Clear();
            previousList.Clear();
            OsuPlaylist.setMode("shuffle", value);
            setting.random = value;
            if (value) button_RandomPlay.BackgroundImage = Properties.Resources.rp_d;
            else button_RandomPlay.BackgroundImage = Properties.Resources.rp_u;
        }

        private void button_RandomPlay_Click(object sender, EventArgs e)
        {
            if (OsuPlaylist.random)
            {
                setRandomPlay(false);
            }
            else
            {
                setRandomPlay(true);
            }
        }

        private void button_LoopPlay_Click(object sender, EventArgs e)
        {
            //nextList.Clear();
            //previousList.Clear();
            savedLoopMode = null;
            if (OsuPlaylist.loop)
            {
                setLoopMode("single");
            }
            else if (SINGLE_MEDIA_LOOP)
            {
                setLoopMode("none");
            }
            else
            {
                setLoopMode("all");
            }
        }

        //==========Play, Pause, Stop Buttons==========

        private void button_Play_MouseEnter(object sender, EventArgs e)
        {
            if (audioPlayer.currentPlayState == CommonPlaybackState.playing)
                button_Play.BackgroundImage = Properties.Resources.pause_mso;
            else
                button_Play.BackgroundImage = Properties.Resources.play_mso;
        }

        private void button_play_MouseLeave(object sender, EventArgs e)
        {
            if (audioPlayer.currentPlayState == CommonPlaybackState.playing)
                button_Play.BackgroundImage = Properties.Resources.pause;
            else
                button_Play.BackgroundImage = Properties.Resources.play;
        }

        private void button_play_MouseDown(object sender, MouseEventArgs e)
        {
            if (audioPlayer.currentPlayState == CommonPlaybackState.playing)
                button_Play.BackgroundImage = Properties.Resources.pause_msd;
            else
                button_Play.BackgroundImage = Properties.Resources.play_msd;
        }

        private void button_play_MouseClick(object sender, MouseEventArgs e)
        {
            if (audioPlayer.currentPlayState == CommonPlaybackState.playing)
            {
                audioPlayer.pause();
                button_Play.BackgroundImage = Properties.Resources.play_mso;
            }
            else
            {
                audioPlayer.play();
                button_Play.BackgroundImage = Properties.Resources.pause_mso;
            }
        }

        //==========Play, Pause, Stop Buttons==========

        private void setLoopMode(string mode)
        {
            setting.loop = mode;
            switch (mode)
            {
                case "none":
                    OsuPlaylist.setMode("loop", false);
                    SINGLE_MEDIA_LOOP = false;
                    button_LoopPlay.BackgroundImage = Properties.Resources.lp_u;
                    LoopMode = mode;
                    audioPlayer.Loop = false;
                    break;
                case "all":
                    OsuPlaylist.setMode("loop", true);
                    SINGLE_MEDIA_LOOP = false;
                    button_LoopPlay.BackgroundImage = Properties.Resources.lp_d_all;
                    LoopMode = mode;
                    audioPlayer.Loop = false;
                    break;
                case "single":
                    OsuPlaylist.setMode("loop", false);
                    SINGLE_MEDIA_LOOP = true;
                    button_LoopPlay.BackgroundImage = Properties.Resources.lp_d_single;
                    LoopMode = mode;
                    audioPlayer.Loop = true;
                    break;
            }
        }

        private void button_ShowAll_Click(object sender, EventArgs e)
        {
            if (osuView != null && osuView.osuSongs != null)
            {
                textBox_SearchSongs.Text = null;
                originList = osuView.osuSongs;
                UpdateSongsList(originList);
            }
        }

        private void button_FindIndex_Click(object sender, EventArgs e)
        {
            int tmp = currentList.Items.FindIndex(o => o.AudioFilename == currentOsuSong.AudioFilename);
            if (tmp != -1) listBox_Songs.SetSelected(tmp, true);
        }

        private void button_Previous_Click(object sender, EventArgs e)
        {
            if (currentList != null) currentList.reSet();
            Previous();
        }

        private void button_Next_Click(object sender, EventArgs e)
        {
            if (currentList != null) currentList.reSet();
            Next();
        }

        private void button_AddtoPlaylist_Click(object sender, EventArgs e)
        {
            if (Playlist_Manager != null && currentList != null && currentList.Items.Count > 0)
                Playlist_Manager.AddtoCurrentList(currentList.Items[listBox_Songs.SelectedIndex]);
        }

        private void loadOsuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog_Osu.ShowDialog() == DialogResult.OK)
            {
                LoadOsuFolder(folderBrowserDialog_Osu.SelectedPath);
            }
        }

        private void reloadOsuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReloadOsu();
        }

        //=============Player_Control=============


        //=============PlayList_Manager=============

        private void Start_PlaylistManager()
        {
            if (Playlist_Manager == null)
            {
                Playlist_Manager = new Form_PlayList(this);
                Playlist_Manager.Show();
            }
        }

        public void Close_PlaylistManager()
        {
            Playlist_Manager = null;
        }

        public void ImportPlaylist(List<OsuSong> list)
        {
            PlayingCustomPlaylist = true;
            originList = list;
            UpdateSongsList(list);
            textBox_SearchSongs.Text = null;
            setCurrentMedia(0, 0);
        }

        //=============NotifyIcon=============


        private void playlistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Start_PlaylistManager();
        }

        //=============HotKey_Action=============        


        public void togglePlayPause(object[] args)
        {
            if (audioPlayer.currentPlayState == CommonPlaybackState.playing)
            {
                pause();
            }
            else
            {
                play();
            }
        }

        public void play(params object[] args)
        {
            audioPlayer.play();
        }

        public void pause(params object[] args)
        {
            audioPlayer.pause();
        }

        public void volumeUp(params object[] args)
        {
            int i = audioPlayer.volume;
            i = i > 95 ? 100 : i + 5;
            audioPlayer.volume = i;
            _form.Invoke(new voidInvokeMethod(updateVolumeSlider));
            Thread.Sleep(150);
        }

        public void volumeDown(params object[] args)
        {
            int i = audioPlayer.volume;
            i = i < 5 ? 0 : i;
            audioPlayer.volume = i - 5;
            _form.Invoke(new voidInvokeMethod(updateVolumeSlider));
            Thread.Sleep(150);
        }

        public void toggleLoop(params object[] args)
        {
            if (savedLoopMode == null)
            {
                if (LoopMode != "single")
                {
                    savedLoopMode = LoopMode;
                    setLoopMode("single");
                }
            }
            else
            {
                setLoopMode(savedLoopMode);
                savedLoopMode = null;
            }
        }

        private void updateVolumeSlider()
        {
            slider_Volume.Value = audioPlayer.volume;
        }

        //=============Net - Socket Server/Client Mp3Stream Playback Service=============
        private void serverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (serverForm == null)
            {
                serverForm = new Form_Server();
                serverForm.Show();
            }
            else
                serverForm.Visible = true;

        }

        private void clientModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (!netClientMode)
            {
                clientDialog cd = new clientDialog();
                cd.ShowDialog();
                if (cd.OK)
                {
                    if (StartNetMode(cd.ipAds, cd.port))
                        item.Checked = true;
                }
            }
            else
            {
                EndNetMode();
                item.Checked = false;
            }
        }

        private bool StartNetMode(String host, int port)
        {
            int volume = audioPlayer.volume;
            if (audioPlayer != null)
                audioPlayer.stop();

            NetAudio.NetStreamMp3Player tmp;
            try
            {
                tmp = new NetAudio.NetStreamMp3Player(host, port);
            }
            catch (Exception e)
            {
                MessageBox.Show(getString("str_connect_fail") + ": \n" + e.Message);
                return false;
            }
            netClientMode = true;

            List<OsuSong> list = new List<OsuSong>();
            originList = list;
            UpdateSongsList(list);
            textBox_SearchSongs.Text = null;

            tmp.ReceivedMessageEvent += Tmp_ReceivedMessageEvent;
            tmp.SendMessage(new NetAudio.Message(NetAudio.MessageCode.requestOsuList));

            audioPlayer_bak = audioPlayer;

            if (osuView != null)
                originList_bak = osuView.osuSongs;

            audioPlayer = tmp;
            audioPlayer.volume = volume;
            setLoopMode(LoopMode);
            audioPlayer.PlayStateChanged += AudioPlayer_PlayStateChanged;
            audioPlayer.Disposing += AudioPlayer_Disposing;

            return true;
            //audioPlayer.setMedia(@"D:\Downloads\Adele - Rolling in the Deep.mp3");
            //audioPlayer.play();
        }

        private void AudioPlayer_Disposing(object sender, EventArgs e)
        {
            _form.Invoke(new voidInvokeMethod(EndNetMode));
        }

        private void EndNetMode()
        {
            netClientMode = false;
            if (audioPlayer != null)
            {
                audioPlayer.stop();

                if (!audioPlayer.disposed)
                    audioPlayer.Dispose();

                if (audioPlayer_bak != null)
                {
                    audioPlayer = audioPlayer_bak;
                    if (originList_bak != null)
                    {
                        originList = originList_bak;
                        UpdateSongsList(originList);
                        textBox_SearchSongs.Text = null;
                        setCurrentMedia(0, 0);
                        audioPlayer.stop();
                    }
                    else
                    {
                        UpdateSongsList(new List<OsuSong>());
                        textBox_SearchSongs.Text = null;
                    }
                }
            }
        }

        private void Tmp_ReceivedMessageEvent(object sender, NetAudio.ReceivedEventArgs e)
        {
            if (e.msg.msgCode == NetAudio.MessageCode.osuList)
            {
                e.done = true;
                _form.Invoke(new GetNetSongListHandler(GetNetSongList), e.msg);
            }
            else if (e.msg.msgCode == NetAudio.MessageCode.mp3Info)
            {
                e.done = false;
                NetAudio.Mp3Info info = e.msg.data[0] as NetAudio.Mp3Info;
                currentMediaLenght = info.totalMilliseconds / 1000;
            }
        }

        private delegate void GetNetSongListHandler(NetAudio.Message msg);

        private void button_Stop_MouseEnter(object sender, EventArgs e)
        {
            button_Stop.BackgroundImage = Properties.Resources.stop_mso;
        }

        private void button_Stop_MouseLeave(object sender, EventArgs e)
        {
            button_Stop.BackgroundImage = Properties.Resources.stop;
        }

        private void button_Stop_MouseDown(object sender, MouseEventArgs e)
        {
            button_Stop.BackgroundImage = Properties.Resources.stop_msd;
        }

        private void button_Stop_MouseClick(object sender, MouseEventArgs e)
        {
            audioPlayer.stop();
            button_Stop.BackgroundImage = Properties.Resources.stop_mso;
        }

        private void GetNetSongList(NetAudio.Message msg)
        {
            List<OsuSong> list = msg.data[0] as List<OsuSong>;
            originList = list;
            UpdateSongsList(list);
            textBox_SearchSongs.Text = null;
            setCurrentMedia(0, 1);
        }


        public class TransparentListView : ListView
        {
            public TransparentListView()
            {
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            }
        }

    }
}
