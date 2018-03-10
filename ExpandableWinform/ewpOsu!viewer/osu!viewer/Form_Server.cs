using System;
using System.Windows.Forms;
using NetAudio;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.IO;

namespace osu_viewer
{
    public partial class Form_Server : Form
    {
        static bool running, publish;
        public const string SERVER_TYPE = "osu_viewer";        

        //List<>
        public Form_Server()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();
            if (GlobalResources.server == null)
            {
                GlobalResources.server = new NetAudio.Mp3Server();
                GlobalResources.server.OnConnected += Server_OnConnected;
                GlobalResources.server.UpdateClientList += Server_UpdateClientList;
                if (GlobalResources.server.streamerList != null)
                    UpdateClientList();
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }
        private void Server_UpdateClientList(object sender, EventArgs e)
        {
            try
            {
                Invoke(new updateListHandler(UpdateClientList));
            }
            catch (Exception ex) { }
        }

        private void Server_OnConnected(object sender, OnConnectEventArgs e)
        {
            Mp3Streamer streamer = e.streamer as Mp3Streamer;            
            streamer.OnReveived += Streamer_OnReveived;
            try
            {
                Invoke(new updateListHandler(UpdateClientList));
            }
            finally { }
        }

        private delegate void updateListHandler();
        private void UpdateClientList()
        {
            listBox_clients.Items.Clear();
            foreach(Mp3Streamer ms in GlobalResources.server.streamerList)
            {
                listBox_clients.Items.Add(ms);
            }
        }

        private void Streamer_OnReveived(object sender, ReceivedEventArgs e)
        {
            Console.WriteLine("############## Got Msg {0}", e.msg.msgCode);
            if (e.msg.msgCode == MessageCode.requestOsuList)
            {
                ThreadPool.QueueUserWorkItem(UploadSongList, sender);
            }
        }

        private void UploadSongList(object streamer)
        {
            (streamer as Mp3Streamer).SendMessage(
                    new NetAudio.Message(MessageCode.osuList, GlobalResources.originList));
        }

        private void button_serverSwitch_Click(object sender, EventArgs e)
        {
            if (running)
            {
                StopServer();
            }
            else
            {
                int port;
                if (int.TryParse(textBox_port.Text, out port))
                    StartServer(port);
            }
        }

        private void StartServer(int port)
        {
            if (GlobalResources.server.Start(port))
            {
                running = true;
                label_state.Text = "Running";
                button_serverSwitch.Text = "Stop";
            }
        }

        private void StopServer()
        {
            GlobalResources.server.Stop();
            running = false;
            label_state.Text = "Stopped";
            button_serverSwitch.Text = "Start";
            button_publish.Text = "Publish";
            publishMessage("None");
        }

        private void publishMessage(string msg)
        {
            label_publishStatus.Text = msg;
        }

        private void button_publish_Click(object sender, EventArgs e)
        {
            if (!running) return;
            if (GlobalResources.server.publish)
            {
                GlobalResources.server.concealServer();
                button_publish.Text = "Publish";
                publishMessage("None");
            }
            else
            {
                string name = textBox_name.Text;
                string port = textBox_port.Text;
                string password = checkBox_password.Checked ? textBox_password.Text : null;
                publishMessage(GlobalResources.server.publishServer(name, port, password));
                if (GlobalResources.server.publish)
                {
                    button_publish.Text = "Conceal";
                }
            }
        }

        

        private void checkBox_password_CheckedChanged(object sender, EventArgs e)
        {
            textBox_password.Enabled = checkBox_password.Checked;
        }
    }
}
