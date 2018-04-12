using System.Windows.Forms;

namespace osu_viewer
{
    partial class ewpOsuViewer
    {
        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ewpOsuViewer));
            this.folderBrowserDialog_Osu = new System.Windows.Forms.FolderBrowserDialog();
            this.panel_Features = new System.Windows.Forms.Panel();
            this.label_CurrentIndex = new System.Windows.Forms.Label();
            this.button_PlayList = new System.Windows.Forms.Button();
            this.button_ShowAll = new System.Windows.Forms.Button();
            this.button_RefreshList = new System.Windows.Forms.Button();
            this.textBox_SearchSongs = new System.Windows.Forms.TextBox();
            this.panel_Lists = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listView_MediaInfo = new TransparentListView();
            this.listBox_Songs = new System.Windows.Forms.ListBox();
            this.progressBar_refreshList = new System.Windows.Forms.ProgressBar();
            this.panel_PlayControls = new System.Windows.Forms.Panel();
            this.wmp_player = new AxWMPLib.AxWindowsMediaPlayer();
            this.progressBar_mediaPosition = new System.Windows.Forms.ProgressBar();
            this.label_MediaName = new System.Windows.Forms.Label();
            this.label_PlayingProgress = new System.Windows.Forms.Label();
            this.button_RandomPlay = new System.Windows.Forms.Button();
            this.button_LoopPlay = new System.Windows.Forms.Button();
            this.button_Previous = new System.Windows.Forms.Button();
            this.button_Next = new System.Windows.Forms.Button();
            this.label_ProgressTime = new System.Windows.Forms.Label();
            this.button_Play = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            this.slider_Volume = new osu_viewer.Slider();
            this.panel_Features2 = new System.Windows.Forms.Panel();
            this.checkBox_Unicode = new System.Windows.Forms.CheckBox();
            this.checkBox_Background = new System.Windows.Forms.CheckBox();
            this.trackBar_BgOpacity = new System.Windows.Forms.TrackBar();
            this.button_AddtoPlaylist = new System.Windows.Forms.Button();
            this.checkBox_ArtistFirst = new System.Windows.Forms.CheckBox();
            this.comboBox_SortBy = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_FindIndex = new System.Windows.Forms.Button();
            this.panel_Features.SuspendLayout();
            this.panel_Lists.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel_PlayControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.wmp_player)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_Volume)).BeginInit();
            this.panel_Features2.SuspendLayout();
            this.mainPanel.SuspendLayout();
            // 
            // folderBrowserDialog_Osu
            // 
            this.folderBrowserDialog_Osu.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog_Osu.ShowNewFolderButton = false;
            // 
            // panel_Features
            // 
            this.panel_Features.Controls.Add(this.textBox_SearchSongs);
            this.panel_Features.Controls.Add(this.button_RefreshList);
            this.panel_Features.Controls.Add(this.button_ShowAll);
            this.panel_Features.Controls.Add(this.button_PlayList);
            this.panel_Features.Controls.Add(this.label_CurrentIndex);
            this.panel_Features.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Features.Location = new System.Drawing.Point(3, 3);
            this.panel_Features.Name = "panel_Features";
            this.panel_Features.Size = new System.Drawing.Size(452, 28);
            this.panel_Features.TabIndex = 25;
            // 
            // label_CurrentIndex
            // 
            this.label_CurrentIndex.BackColor = System.Drawing.SystemColors.Control;
            this.label_CurrentIndex.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_CurrentIndex.Location = new System.Drawing.Point(166, 8);
            this.label_CurrentIndex.Name = "label_CurrentIndex";
            this.label_CurrentIndex.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label_CurrentIndex.Size = new System.Drawing.Size(70, 12);
            this.label_CurrentIndex.TabIndex = 21;
            this.label_CurrentIndex.Text = "0000";
            this.label_CurrentIndex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_PlayList
            // 
            this.button_PlayList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_PlayList.Location = new System.Drawing.Point(387, 3);
            this.button_PlayList.Name = "button_PlayList";
            this.button_PlayList.Size = new System.Drawing.Size(65, 23);
            this.button_PlayList.TabIndex = 6;
            this.button_PlayList.Text = getString("str_playlist");
            this.button_PlayList.UseVisualStyleBackColor = true;
            this.button_PlayList.Click += new System.EventHandler(this.button_PlayList_Click);
            // 
            // button_ShowAll
            // 
            this.button_ShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ShowAll.Location = new System.Drawing.Point(316, 3);
            this.button_ShowAll.Name = "button_ShowAll";
            this.button_ShowAll.Size = new System.Drawing.Size(65, 23);
            this.button_ShowAll.TabIndex = 5;
            this.button_ShowAll.Text = getString("str_show_all");
            this.button_ShowAll.UseVisualStyleBackColor = true;
            this.button_ShowAll.Click += new System.EventHandler(this.button_ShowAll_Click);
            // 
            // button_RefreshList
            // 
            this.button_RefreshList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_RefreshList.Location = new System.Drawing.Point(245, 3);
            this.button_RefreshList.Name = "button_RefreshList";
            this.button_RefreshList.Size = new System.Drawing.Size(65, 23);
            this.button_RefreshList.TabIndex = 4;
            this.button_RefreshList.Text = getString("str_refresh");
            this.button_RefreshList.UseVisualStyleBackColor = true;
            this.button_RefreshList.Click += new System.EventHandler(this.button_Refresh_Click);
            // 
            // textBox_SearchSongs
            // 
            this.textBox_SearchSongs.Location = new System.Drawing.Point(0, 3);
            this.textBox_SearchSongs.Name = "textBox_SearchSongs";
            this.textBox_SearchSongs.Size = new System.Drawing.Size(160, 22);
            this.textBox_SearchSongs.TabIndex = 1;
            this.textBox_SearchSongs.TextChanged += new System.EventHandler(this.textBox_SearchSongs_TextChanged);
            // 
            // panel_Lists
            // 
            this.panel_Lists.Controls.Add(this.splitContainer1);
            this.panel_Lists.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Lists.Location = new System.Drawing.Point(3, 31);
            this.panel_Lists.Name = "panel_Lists";
            this.panel_Lists.Size = new System.Drawing.Size(452, 231);
            this.panel_Lists.TabIndex = 23;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.progressBar_refreshList);
            this.splitContainer1.Panel1.Controls.Add(this.listBox_Songs);
            // 
            // splitContainer1.Panel2
            // 
            //this.splitContainer1.Panel2.Controls.Add(this.pictureBox_Bg);
            this.splitContainer1.Panel2.Controls.Add(this.listView_MediaInfo);
            this.splitContainer1.Size = new System.Drawing.Size(452, 231);
            this.splitContainer1.SplitterDistance = 238;
            this.splitContainer1.TabIndex = 22;
            // 
            // listView_MediaInfo
            // 
            this.listView_MediaInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_MediaInfo.Location = new System.Drawing.Point(0, 0);
            this.listView_MediaInfo.Name = "listView_MediaInfo";
            this.listView_MediaInfo.Size = new System.Drawing.Size(206, 227);
            this.listView_MediaInfo.TabIndex = 8;
            this.listView_MediaInfo.TileSize = new System.Drawing.Size(260, 28);
            this.listView_MediaInfo.UseCompatibleStateImageBehavior = false;
            this.listView_MediaInfo.View = System.Windows.Forms.View.Tile;
            // 
            // listBox_Songs
            // 
            this.listBox_Songs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Songs.FormattingEnabled = true;
            this.listBox_Songs.HorizontalScrollbar = true;
            this.listBox_Songs.ItemHeight = 12;
            this.listBox_Songs.Location = new System.Drawing.Point(0, 0);
            this.listBox_Songs.Name = "listBox_Songs";
            this.listBox_Songs.Size = new System.Drawing.Size(234, 227);
            this.listBox_Songs.TabIndex = 2;
            // 
            // progressBar_refreshList
            // 
            this.progressBar_refreshList.Location = new System.Drawing.Point(43, 95);
            this.progressBar_refreshList.Name = "progressBar_refreshList";
            this.progressBar_refreshList.Size = new System.Drawing.Size(137, 23);
            this.progressBar_refreshList.Step = 1;
            this.progressBar_refreshList.TabIndex = 2;
            this.progressBar_refreshList.Visible = false;
            // 
            // panel_PlayControls
            // 
            this.panel_PlayControls.Controls.Add(this.slider_Volume);
            this.panel_PlayControls.Controls.Add(this.button_Stop);
            this.panel_PlayControls.Controls.Add(this.button_Play);
            this.panel_PlayControls.Controls.Add(this.label_ProgressTime);
            this.panel_PlayControls.Controls.Add(this.button_Next);
            this.panel_PlayControls.Controls.Add(this.button_Previous);
            this.panel_PlayControls.Controls.Add(this.button_LoopPlay);
            this.panel_PlayControls.Controls.Add(this.button_RandomPlay);
            this.panel_PlayControls.Controls.Add(this.label_PlayingProgress);
            this.panel_PlayControls.Controls.Add(this.label_MediaName);
            this.panel_PlayControls.Controls.Add(this.progressBar_mediaPosition);
            this.panel_PlayControls.Controls.Add(this.wmp_player);
            this.panel_PlayControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_PlayControls.Location = new System.Drawing.Point(3, 291);
            this.panel_PlayControls.Margin = new System.Windows.Forms.Padding(6);
            this.panel_PlayControls.Name = "panel_PlayControls";
            this.panel_PlayControls.Size = new System.Drawing.Size(452, 63);
            this.panel_PlayControls.TabIndex = 10;
            // 
            // wmp_player
            // 
            this.wmp_player.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.wmp_player.Enabled = true;
            this.wmp_player.Location = new System.Drawing.Point(0, 32);
            this.wmp_player.Name = "wmp_player";
            this.wmp_player.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("wmp_player.OcxState")));
            this.wmp_player.Size = new System.Drawing.Size(452, 31);
            this.wmp_player.TabIndex = 13;
            this.wmp_player.TabStop = false;
            // 
            // progressBar_mediaPosition
            // 
            this.progressBar_mediaPosition.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar_mediaPosition.Location = new System.Drawing.Point(0, 22);
            this.progressBar_mediaPosition.Maximum = 1200;
            this.progressBar_mediaPosition.Name = "progressBar_mediaPosition";
            this.progressBar_mediaPosition.Size = new System.Drawing.Size(452, 10);
            this.progressBar_mediaPosition.TabIndex = 4;
            this.progressBar_mediaPosition.Click += new System.EventHandler(this.progressBar_mediaPosition_Click);
            // 
            // label_MediaName
            // 
            this.label_MediaName.AutoSize = true;
            this.label_MediaName.Location = new System.Drawing.Point(0, 4);
            this.label_MediaName.Name = "label_MediaName";
            this.label_MediaName.Size = new System.Drawing.Size(60, 12);
            this.label_MediaName.TabIndex = 9;
            this.label_MediaName.Text = "mediaName";
            // 
            // label_PlayingProgress
            // 
            this.label_PlayingProgress.AutoSize = true;
            this.label_PlayingProgress.Location = new System.Drawing.Point(404, 40);
            this.label_PlayingProgress.Name = "label_PlayingProgress";
            this.label_PlayingProgress.Size = new System.Drawing.Size(25, 12);
            this.label_PlayingProgress.TabIndex = 5;
            this.label_PlayingProgress.Text = "time";
            // 
            // button_RandomPlay
            // 
            this.button_RandomPlay.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_RandomPlay.BackgroundImage")));
            this.button_RandomPlay.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.button_RandomPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_RandomPlay.Location = new System.Drawing.Point(338, 39);
            this.button_RandomPlay.Name = "button_RandomPlay";
            this.button_RandomPlay.Size = new System.Drawing.Size(15, 13);
            this.button_RandomPlay.TabIndex = 11;
            this.button_RandomPlay.UseVisualStyleBackColor = true;
            this.button_RandomPlay.Click += new System.EventHandler(this.button_RandomPlay_Click);
            // 
            // button_LoopPlay
            // 
            this.button_LoopPlay.BackgroundImage = global::osu_viewer.Properties.Resources.lp_d_all;
            this.button_LoopPlay.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.button_LoopPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_LoopPlay.Location = new System.Drawing.Point(359, 39);
            this.button_LoopPlay.Name = "button_LoopPlay";
            this.button_LoopPlay.Size = new System.Drawing.Size(15, 13);
            this.button_LoopPlay.TabIndex = 12;
            this.button_LoopPlay.UseVisualStyleBackColor = true;
            this.button_LoopPlay.Click += new System.EventHandler(this.button_LoopPlay_Click);
            // 
            // button_Previous
            // 
            this.button_Previous.BackgroundImage = global::osu_viewer.Properties.Resources.previous;
            this.button_Previous.FlatAppearance.BorderSize = 0;
            this.button_Previous.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_Previous.Location = new System.Drawing.Point(65, 38);
            this.button_Previous.Name = "button_Previous";
            this.button_Previous.Size = new System.Drawing.Size(16, 16);
            this.button_Previous.TabIndex = 9;
            this.button_Previous.UseVisualStyleBackColor = true;
            this.button_Previous.Click += new System.EventHandler(this.button_Previous_Click);
            // 
            // button_Next
            // 
            this.button_Next.BackgroundImage = global::osu_viewer.Properties.Resources.next;
            this.button_Next.FlatAppearance.BorderSize = 0;
            this.button_Next.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_Next.Location = new System.Drawing.Point(87, 38);
            this.button_Next.Name = "button_Next";
            this.button_Next.Size = new System.Drawing.Size(16, 16);
            this.button_Next.TabIndex = 10;
            this.button_Next.UseVisualStyleBackColor = true;
            this.button_Next.Click += new System.EventHandler(this.button_Next_Click);
            // 
            // label_ProgressTime
            // 
            this.label_ProgressTime.AutoSize = true;
            this.label_ProgressTime.Location = new System.Drawing.Point(129, 4);
            this.label_ProgressTime.Name = "label_ProgressTime";
            this.label_ProgressTime.Size = new System.Drawing.Size(53, 12);
            this.label_ProgressTime.TabIndex = 6;
            this.label_ProgressTime.Text = "pointTime";
            this.label_ProgressTime.Visible = false;
            // 
            // button_Play
            // 
            this.button_Play.BackColor = System.Drawing.Color.Transparent;
            this.button_Play.BackgroundImage = global::osu_viewer.Properties.Resources.play;
            this.button_Play.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button_Play.FlatAppearance.BorderSize = 0;
            this.button_Play.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_Play.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_Play.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Play.Location = new System.Drawing.Point(4, 33);
            this.button_Play.Name = "button_Play";
            this.button_Play.Size = new System.Drawing.Size(28, 27);
            this.button_Play.TabIndex = 14;
            this.button_Play.UseVisualStyleBackColor = false;
            this.button_Play.MouseClick += new System.Windows.Forms.MouseEventHandler(this.button_play_MouseClick);
            this.button_Play.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_play_MouseDown);
            this.button_Play.MouseEnter += new System.EventHandler(this.button_Play_MouseEnter);
            this.button_Play.MouseLeave += new System.EventHandler(this.button_play_MouseLeave);
            // 
            // button_Stop
            // 
            this.button_Stop.BackColor = System.Drawing.Color.Transparent;
            this.button_Stop.BackgroundImage = global::osu_viewer.Properties.Resources.stop;
            this.button_Stop.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button_Stop.FlatAppearance.BorderSize = 0;
            this.button_Stop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_Stop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Stop.Location = new System.Drawing.Point(34, 36);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(22, 22);
            this.button_Stop.TabIndex = 15;
            this.button_Stop.UseVisualStyleBackColor = false;
            this.button_Stop.MouseClick += new System.Windows.Forms.MouseEventHandler(this.button_Stop_MouseClick);
            this.button_Stop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_Stop_MouseDown);
            this.button_Stop.MouseEnter += new System.EventHandler(this.button_Stop_MouseEnter);
            this.button_Stop.MouseLeave += new System.EventHandler(this.button_Stop_MouseLeave);
            // 
            // slider_Volume
            // 
            this.slider_Volume.AutoSize = false;
            this.slider_Volume.BaseBarColor = System.Drawing.Color.Silver;
            this.slider_Volume.Location = new System.Drawing.Point(107, 34);
            this.slider_Volume.Maximum = 100;
            this.slider_Volume.MinimumSize = new System.Drawing.Size(60, 25);
            this.slider_Volume.Name = "slider_Volume";
            this.slider_Volume.PinColor = System.Drawing.Color.RoyalBlue;
            this.slider_Volume.Size = new System.Drawing.Size(106, 25);
            this.slider_Volume.TabIndex = 10;
            this.slider_Volume.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.slider_Volume.TrackBarColor = System.Drawing.Color.CornflowerBlue;
            this.slider_Volume.Value = 50;
            // 
            // panel_Features2
            // 
            this.panel_Features2.Controls.Add(this.button_FindIndex);
            this.panel_Features2.Controls.Add(this.label1);
            this.panel_Features2.Controls.Add(this.comboBox_SortBy);
            this.panel_Features2.Controls.Add(this.checkBox_ArtistFirst);
            this.panel_Features2.Controls.Add(this.button_AddtoPlaylist);
            this.panel_Features2.Controls.Add(this.checkBox_Unicode);
            this.panel_Features2.Controls.Add(this.checkBox_Background);
            this.panel_Features2.Controls.Add(this.trackBar_BgOpacity);
            this.panel_Features2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_Features2.Location = new System.Drawing.Point(3, 262);
            this.panel_Features2.Name = "panel_Features2";
            this.panel_Features2.Size = new System.Drawing.Size(452, 29);
            this.panel_Features2.TabIndex = 25;
            // 
            // checkBox_Unicode
            // 
            this.checkBox_Unicode.AutoSize = true;
            this.checkBox_Unicode.Checked = true;
            this.checkBox_Unicode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Unicode.Location = new System.Drawing.Point(189, 7);
            this.checkBox_Unicode.Name = "checkBox_Unicode";
            this.checkBox_Unicode.Size = new System.Drawing.Size(66, 16);
            this.checkBox_Unicode.TabIndex = 7;
            this.checkBox_Unicode.Text = "Unicode ";
            this.checkBox_Unicode.UseVisualStyleBackColor = true;
            this.checkBox_Unicode.CheckedChanged += new System.EventHandler(this.checkBox_Unicode_CheckedChanged);
            // 
            // button_AddtoPlaylist
            // 
            this.button_AddtoPlaylist.Location = new System.Drawing.Point(0, 3);
            this.button_AddtoPlaylist.Name = "button_AddtoPlaylist";
            this.button_AddtoPlaylist.Size = new System.Drawing.Size(87, 23);
            this.button_AddtoPlaylist.TabIndex = 3;
            this.button_AddtoPlaylist.Text = getString("str_add_playlist");
            this.button_AddtoPlaylist.UseVisualStyleBackColor = true;
            this.button_AddtoPlaylist.Click += new System.EventHandler(this.button_AddtoPlaylist_Click);
            // 
            // checkBox_ArtistFrist
            // 
            this.checkBox_ArtistFirst.AutoSize = true;
            this.checkBox_ArtistFirst.Location = new System.Drawing.Point(261, 7);
            this.checkBox_ArtistFirst.Name = "checkBox_ArtistFirst";
            this.checkBox_ArtistFirst.Size = new System.Drawing.Size(72, 16);
            this.checkBox_ArtistFirst.TabIndex = 8;
            this.checkBox_ArtistFirst.Text = getString("str_artist_first_short");
            this.checkBox_ArtistFirst.UseVisualStyleBackColor = true;
            this.checkBox_ArtistFirst.CheckedChanged += new System.EventHandler(this.checkBox_ArtistFrist_CheckedChanged);
            // 
            // comboBox_SortBy
            // 
            this.comboBox_SortBy.FormattingEnabled = true;
            this.comboBox_SortBy.Items.AddRange(new object[] {
            getString("str_title"),
            getString("str_artist"),
            getString("str_creator"),
            getString("str_source")});
            this.comboBox_SortBy.Name = "comboBox_SortBy";
            this.comboBox_SortBy.Size = new System.Drawing.Size(62, 20);
            int x = mainPanel.Size.Width - comboBox_SortBy.Size.Width - 8;
            this.comboBox_SortBy.Location = new System.Drawing.Point(x, 5);
            this.comboBox_SortBy.TabIndex = 10;
            this.comboBox_SortBy.SelectedIndexChanged += new System.EventHandler(this.comboBox_SortBy_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 12);
            this.label1.Text = getString("str_sort_by");
            x = comboBox_SortBy.Location.X - this.label1.Size.Width - 6;
            this.label1.Location = new System.Drawing.Point(x, 8);
            this.label1.TabIndex = 11;
            //
            // trackBar_BgOpacity
            //
            this.trackBar_BgOpacity.AutoSize = true;
            this.trackBar_BgOpacity.Name = "trackBar_BgOpacity";
            this.trackBar_BgOpacity.TickStyle = TickStyle.None;
            this.trackBar_BgOpacity.Minimum = 0;
            this.trackBar_BgOpacity.Maximum = 100;
            x = label1.Location.X - this.trackBar_BgOpacity.Size.Width - 6;
            this.trackBar_BgOpacity.Location = new System.Drawing.Point(x, 8);
            this.trackBar_BgOpacity.ValueChanged += TrackBar_BgOpacity_ValueChanged;
            // 
            // checkBox_Background
            // 
            this.checkBox_Background.AutoSize = true;
            this.checkBox_Background.Checked = true;
            this.checkBox_Background.CheckState = System.Windows.Forms.CheckState.Checked;            
            this.checkBox_Background.Name = "checkBox_Background";
            this.checkBox_Background.Size = new System.Drawing.Size(66, 16);
            this.checkBox_Background.Text = getString("str_background");
            this.checkBox_Background.UseVisualStyleBackColor = true;
            x = trackBar_BgOpacity.Location.X - this.checkBox_Background.Size.Width - 6;
            this.checkBox_Background.Location = new System.Drawing.Point(x, 8);
            this.checkBox_Background.CheckedChanged += new System.EventHandler(this.checkBox_Background_CheckedChanged);
            // 
            // button_FindIndex
            // 
            this.button_FindIndex.Location = new System.Drawing.Point(93, 3);
            this.button_FindIndex.Name = "button_FindIndex";
            this.button_FindIndex.Size = new System.Drawing.Size(87, 23);
            this.button_FindIndex.TabIndex = 12;
            this.button_FindIndex.Text = getString("str_find_index");
            this.button_FindIndex.UseVisualStyleBackColor = true;
            this.button_FindIndex.Click += new System.EventHandler(this.button_FindIndex_Click);
            // 
            // panel_Main
            // 
            this.mainPanel.Controls.Add(this.panel_Features2);
            this.mainPanel.Controls.Add(this.panel_PlayControls);
            this.mainPanel.Controls.Add(this.panel_Lists);
            this.mainPanel.Controls.Add(this.panel_Features);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(3, 27);
            this.mainPanel.Name = "panel_Main";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(3);
            this.mainPanel.Size = new System.Drawing.Size(458, 357);
            this.mainPanel.TabIndex = 24;
            // 
            // Form_Main
            // 
            this.panel_Features.ResumeLayout(false);
            this.panel_Features.PerformLayout();
            this.panel_Lists.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel_PlayControls.ResumeLayout(false);
            this.panel_PlayControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.wmp_player)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slider_Volume)).EndInit();
            this.panel_Features2.ResumeLayout(false);
            this.panel_Features2.PerformLayout();
            this.mainPanel.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog_Osu;
        private System.Windows.Forms.Panel panel_Features;
        private System.Windows.Forms.TextBox textBox_SearchSongs;
        private System.Windows.Forms.Button button_RefreshList;
        private System.Windows.Forms.Button button_ShowAll;
        private System.Windows.Forms.Button button_PlayList;
        private System.Windows.Forms.Label label_CurrentIndex;
        private System.Windows.Forms.Panel panel_Lists;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ProgressBar progressBar_refreshList;
        private System.Windows.Forms.ListBox listBox_Songs;
        private TransparentListView listView_MediaInfo;
        private System.Windows.Forms.Panel panel_PlayControls;
        private Slider slider_Volume;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.Button button_Play;
        private System.Windows.Forms.Label label_ProgressTime;
        private System.Windows.Forms.Button button_Next;
        private System.Windows.Forms.Button button_Previous;
        private System.Windows.Forms.Button button_LoopPlay;
        private System.Windows.Forms.Button button_RandomPlay;
        private System.Windows.Forms.Label label_PlayingProgress;
        private System.Windows.Forms.Label label_MediaName;
        private System.Windows.Forms.ProgressBar progressBar_mediaPosition;
        private AxWMPLib.AxWindowsMediaPlayer wmp_player;
        private System.Windows.Forms.Panel panel_Features2;
        private System.Windows.Forms.Button button_FindIndex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_SortBy;
        private System.Windows.Forms.CheckBox checkBox_ArtistFirst;
        private System.Windows.Forms.Button button_AddtoPlaylist;
        private System.Windows.Forms.CheckBox checkBox_Unicode;
        private System.Windows.Forms.CheckBox checkBox_Background;
        private System.Windows.Forms.TrackBar trackBar_BgOpacity;
    }
}

