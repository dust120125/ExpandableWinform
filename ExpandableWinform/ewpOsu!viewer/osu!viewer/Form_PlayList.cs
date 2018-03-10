using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace osu_viewer
{
    public partial class Form_PlayList : Form
    {
        public const string DEFALUT_PATH = "playlist/";
        private static string dataPath
        {
            get { return rootDir + DEFALUT_PATH; }
        }

        public static string rootDir = "./";

        private List<OsuPlaylist> Playlists;
        private OsuPlaylist currentPlaylist;
        private Form parentForm;
        private ewpOsuViewer parent;

        public Form_PlayList(ewpOsuViewer parent)
        {
            this.parent = parent;
            parentForm = parent.getParentForm();
            InitializeComponent();            
        }

        private void Form_PlayList_Load(object sender, EventArgs e)
        {
            this.Location = new Point(parentForm.Location.X + parentForm.Width - 15, parentForm.Location.Y);
            if (this.Location.X > Screen.PrimaryScreen.Bounds.Width - 100)
            {
                this.Location = new Point (parentForm.Location.X - this.Width + 15, parentForm.Location.Y);
            }

            Playlists = new List<OsuPlaylist>();

            listBox_MediaList.DoubleClick += ListBox_MediaList_DoubleClick;
            textBox_RenamePlaylist.LostFocus += TextBox_RenamePlaylist_LostFocus;

            this.Disposed += Form_PlayList_Disposed;
            LoadLists();
        }

        private void Form_PlayList_Disposed(object sender, EventArgs e)
        {
            SaveLists();
            parent.Close_PlaylistManager();
        }

        private void SaveLists()
        {
            Playlists.ForEach(p =>
            {
                try
                {
                    StreamWriter sw = File.CreateText(dataPath + p.name + ".xml");
                    sw.WriteLine(p.getFormatString());
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void LoadLists()
        {
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            IEnumerable<string> files = Directory.GetFiles(dataPath).Where(s => s.EndsWith(".xml"));
            foreach(string file in files)
            {
                StreamReader sr = File.OpenText(file);
                if (sr.ReadLine() == OsuPlaylist.Header)
                {
                    OsuPlaylist tmp = new OsuPlaylist(sr.ReadLine());
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        if (s == "") continue;
                        OsuSong os = OsuView.getInstance().getOsuSongByID(s);
                        if (os != null)
                            tmp.Items.Add(os);
                        else
                        {
                            tmp.Items.Add(new OsuSong(s));
                        }
                    }

                    Playlists.Add(tmp);
                }
                sr.Close();
            }
            UpdatePlaylists();
        }

        private void TextBox_RenamePlaylist_LostFocus(object sender, EventArgs e)
        {
            if (Playlists.FindAll(o => o.name == textBox_RenamePlaylist.Text).Count > 1)
            {
                MessageBox.Show("Has Duplicate Name!");
                textBox_RenamePlaylist.Focus();
            }
            else
            {                
                if (File.Exists(dataPath + currentPlaylist.name + ".xml"))
                {
                    (new FileInfo(dataPath + currentPlaylist.name + ".xml")).MoveTo(dataPath + textBox_RenamePlaylist.Text + ".xml");
                }
                currentPlaylist.name = textBox_RenamePlaylist.Text;
            }
            UpdatePlaylists();
        }

        private void ListBox_MediaList_DoubleClick(object sender, EventArgs e)
        {
            if (currentPlaylist.Items.Count > 0)
            {
                currentPlaylist.Items.RemoveAt(listBox_MediaList.SelectedIndex);
                UpdateMediaList();
            }
        }

        private void button_NewList_Click(object sender, EventArgs e)
        {
            int index = 1;

            List<OsuPlaylist> tmp = Playlists.FindAll(o => o.name.Split('_')[0] == "New Playlist");
            if (tmp.Count > 0)
            {
                int[] nums = new int[tmp.Count];
                for(int i = 0; i < tmp.Count; i++)
                {
                    try
                    {
                        nums[i] = Convert.ToInt32(tmp[i].name.Split('_')[1]);
                    }
                    catch
                    {
                        continue;
                    }
                }
                while (nums.Contains(index))
                {
                    index++;
                }
            }

            Playlists.Add(new OsuPlaylist("New Playlist_" + index));
            UpdatePlaylists();
            comboBox_Playlists.SelectedIndex = comboBox_Playlists.Items.Count - 1;            
        }

        public void AddtoCurrentList(OsuSong os)
        {
            if (currentPlaylist != null)
            {
                currentPlaylist.Items.Add(os);
                UpdateMediaList();
                listBox_MediaList.SetSelected(listBox_MediaList.Items.Count - 1, true);
            }
        }

        private void UpdatePlaylists()
        {
            int index = comboBox_Playlists.SelectedIndex;
            comboBox_Playlists.DataSource = null;
            comboBox_Playlists.DataSource = Playlists;

            if (index > comboBox_Playlists.Items.Count - 1)
                comboBox_Playlists.SelectedIndex = comboBox_Playlists.Items.Count - 1;
            else if (index != -1)
                comboBox_Playlists.SelectedIndex = index;
            
        }

        public void UpdateMediaList()
        {
            listBox_MediaList.DataSource = null;
            if (currentPlaylist != null)
                listBox_MediaList.DataSource = currentPlaylist.Items;
        }

        private void button_Play_Click(object sender, EventArgs e)
        {
            if (currentPlaylist != null)
            {
                parent.ImportPlaylist(currentPlaylist.Items.ToList());
            }            
        }

        private void textBox_RenamePlaylist_TextChanged(object sender, EventArgs e)
        {            
            comboBox_Playlists.Text = textBox_RenamePlaylist.Text;
        }

        private void comboBox_Playlists_SelectedIndexChanged(object sender, EventArgs e)
        {            
            if (comboBox_Playlists.SelectedIndex != -1 && Playlists[comboBox_Playlists.SelectedIndex] != null)
            {
                currentPlaylist = Playlists[comboBox_Playlists.SelectedIndex];
                UpdateMediaList();
                textBox_RenamePlaylist.Text = currentPlaylist.name;
            }
            else
            {
                currentPlaylist = null;
                textBox_RenamePlaylist.Text = null;
                UpdateMediaList();
            }
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            if (currentPlaylist != null)
            {
                if (File.Exists(dataPath + currentPlaylist.name + ".xml"))
                    File.Delete(dataPath + currentPlaylist.name + ".xml");

                Playlists.RemoveAt(comboBox_Playlists.SelectedIndex);
                UpdatePlaylists();
            }
        }

        private void moveItem(int i)
        {
            if (listBox_MediaList.SelectedIndex >= 0 && currentPlaylist != null)
            {
                OsuSong tmp = currentPlaylist.Items[listBox_MediaList.SelectedIndex];
                currentPlaylist.Items.RemoveAt(listBox_MediaList.SelectedIndex);
                int index = listBox_MediaList.SelectedIndex + i;

                if (index < 0) index = 0;
                else if (index > currentPlaylist.Items.Count - 1) index = currentPlaylist.Items.Count;

                currentPlaylist.Items.Insert(index, tmp);                
                UpdatePlaylists();
                listBox_MediaList.SetSelected(index, true);
            }
        }

        private void button_Up_Click(object sender, EventArgs e)
        {
            moveItem(-1);
        }

        private void button_Down_Click(object sender, EventArgs e)
        {
            moveItem(1);
        }

        private void buttonDoubleUp_Click(object sender, EventArgs e)
        {
            moveItem(-5);
        }

        private void button_DoubleDown_Click(object sender, EventArgs e)
        {
            moveItem(5);
        }
    }
}
