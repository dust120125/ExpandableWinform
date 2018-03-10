using System;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace osu_viewer
{
    public partial class clientDialog : Form
    {

        private static List<Server> serverList = new List<Server>();
        private struct Server
        {
            public string name;
            public string ip;
            public string port;
            public bool password;
            public override string ToString()
            {                
                return name + "         " + (password ? "(Need password)" : "");
            }
        }

        public string ipAds { get; private set; }
        public int port { get; private set; }
        public bool OK { get; private set; }

        public clientDialog()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();
        }

        private void clientDialog_Load(object sender, EventArgs e)
        {            
            if (serverList.Count > 0)
            {
                foreach(Server s in serverList)
                {
                    listBox_servers.Items.Add(s);
                }
            }
            else
            {
                getServerList();
            }
            Console.WriteLine("index" + listBox_servers.SelectedIndex);
        }

        private void button_Ok_Click(object sender, EventArgs e)
        {            
            if (checkBox_custom.Checked)
            {
                ipAds = textBox_IP.Text;
                int tmp;
                if (int.TryParse(textBox_Port.Text, out tmp))
                    port = tmp;
                else
                    port = -1;
                OK = true;
                Close();
            }
            else
            {
                if (listBox_servers.SelectedIndex < 0)
                {
                    return;
                }

                if (getIP(listBox_servers.SelectedIndex))
                {
                    OK = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Worng Password!");
                }
            }
        }

        private bool getIP(int index)
        {
            Server server = serverList[index];
            if (serverList[index].password)
            {
                string response;
                string url = "http://www.poipoi.idv.tw/menu/ip_publish/index.php?";
                url += "name=" + server.name;
                url += "&password=" + textBox_password.Text;
                HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create(url);
                wr.Method = "GET";
                wr.Timeout = 5000;

                using (WebResponse res = wr.GetResponse())
                {
                    Console.WriteLine("Connected");
                    using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                    {
                        response = sr.ReadToEnd();
                    }
                }

                Console.WriteLine(response);
                if (String.IsNullOrEmpty(response)) return false;

                string[] info = response.Split(',');
                ipAds = info[0];
                int tmp;
                if (int.TryParse(info[1], out tmp))
                    port = tmp;
                else
                    port = -1;
                return true;
            }
            else
            {
                ipAds = server.ip;
                int tmp;
                if (int.TryParse(server.port, out tmp))
                    port = tmp;
                else
                    port = -1;
                return true;
            }
        }

        private void getServerList()
        {
            try
            {
                string response;
                string url = "http://www.poipoi.idv.tw/menu/ip_publish/index.php";
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                wr.Method = "GET";
                wr.Timeout = 5000;

                using (WebResponse res = wr.GetResponse())
                {
                    Console.WriteLine("Connected");
                    using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                    {
                        response = sr.ReadToEnd();
                    }
                }

                Console.WriteLine(response);
                listBox_servers.Items.Clear();
                serverList.Clear();

                listBox_servers.BeginUpdate();
                foreach (string s in response.Split('\n'))
                {
                    if (s.StartsWith("#") || String.IsNullOrEmpty(s)) continue;
                    string[] info = s.Split(',');
                    if (info[2] != Form_Server.SERVER_TYPE) continue;

                    Server tmp = new Server();
                    tmp.password = info[0] == "true" ? true : false;
                    tmp.name = info[1];
                    if (!tmp.password)
                    {
                        tmp.ip = info[3];
                        tmp.port = info[4];
                    }

                    listBox_servers.Items.Add(tmp);
                    serverList.Add(tmp);
                }
                listBox_servers.EndUpdate();
            }
            catch
            {

            }
        }

        private void listBox_servers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serverList[listBox_servers.SelectedIndex].password)
                textBox_password.Enabled = true;
        }

        private void checkBox_custom_CheckedChanged(object sender, EventArgs e)
        {
            textBox_IP.Enabled = checkBox_custom.Checked;
            textBox_Port.Enabled = checkBox_custom.Checked;
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            OK = false;
            Close();
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            getServerList();
        }
    }
}
