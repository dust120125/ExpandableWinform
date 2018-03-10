using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NetAudio
{
    public class Mp3Server
    {
        private TcpListener listener;
        public List<Mp3Streamer> streamerList { get; private set; }
        public bool isRunning { get; private set; }

        public delegate void ConnectedHandler(object sender, OnConnectEventArgs e);
        public event ConnectedHandler OnConnected;

        public delegate void UpdateClientListHandler(object sender, EventArgs e);
        public event UpdateClientListHandler UpdateClientList;

        private Thread mThread;
        private bool threadRunning;

        public bool publish { get; private set; }
        const string ERROR_DUPLICATE_NAME = "duplicate_name";
        const string ERROR_DUPLICATE_IP = "duplicate_ip";

        public bool Start(int port)
        {
            if (threadRunning)
                return false;
            listener = new TcpListener(IPAddress.Any, port);
            streamerList = new List<Mp3Streamer>();
            ThreadPool.QueueUserWorkItem(StartTask);
            return true;
        } 

        private void StartTask(object nothing)
        {
            mThread = Thread.CurrentThread;
            threadRunning = true;

            isRunning = true;
            listener.Start();
            while (isRunning)
            {
                CleanStreamerList();
                TcpClient tcpClient;
                try
                {
                    if (listener.Pending())
                    {
                        tcpClient = listener.AcceptTcpClient();
                        Mp3Streamer streamer = new Mp3Streamer(tcpClient);
                        streamerList.Add(streamer);

                        if (OnConnected != null)
                            OnConnected(this, new OnConnectEventArgs(streamer, tcpClient));
                    }
                    else
                        Thread.Sleep(1000);
                }
                catch (SocketException e)
                {
                    continue;
                }
            }
            threadRunning = false;
        }

        private void CleanStreamerList()
        {
            bool changed = false;
            for(int i = 0; i < streamerList.Count;)
            {
                if (streamerList[i].disposed)
                {
                    streamerList.RemoveAt(i);
                    changed = true;
                }
                else
                    i++;
            }

            if (changed)
                UpdateClientList?.Invoke(this, null);
        }

        public void Stop()
        {
            if (publish) concealServer();
            isRunning = false;
            listener.Stop();
            foreach(Mp3Streamer ms in streamerList)
            {
                ms.Dispose();
            }
            streamerList.Clear();
            UpdateClientList?.Invoke(this, null);
            while (threadRunning)
            {
                Thread.Sleep(100);
            }
        }

        public string publishServer(string name, string port, string password)
        {
            if (publish) return "Published";
            if (name.Contains(',')) return "Can't use \",\" in server name!";
            if (name.Contains('\'')) return "Can't use \" ' \" in server name!";
            if (password != null)
            {
                if (password.Contains(',')) return "Can't use \",\" in password!";
                if (password.Contains('\'')) return "Can't use \" ' \" in password!";
            }

            string response = "";
            if (name == String.Empty)
            {
                return "Name Empty!";
            }

            string url = "http://www.poipoi.idv.tw/menu/ip_publish/publish.php?";
            string urlDel = url + "del=1";
            url += "name=" + name + "&port=" + port + "&type=" + osu_viewer.Form_Server.SERVER_TYPE;
            if (password != null)
                url += "&password=" + password;

            Console.WriteLine(url);
            HttpWebRequest.Create(urlDel).GetResponse().Close();
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
            if (response.StartsWith("fail"))
            {
                publish = false;
                string errorMsg = response.Substring(response.IndexOf("error:") + 6);
                if (errorMsg == ERROR_DUPLICATE_IP)
                {
                    return "DUPLICATE IP!";
                }
                else if (errorMsg == ERROR_DUPLICATE_NAME)
                {
                    return "Duplicate Name!";
                } else
                {
                    return "Error!";
                }                
            }
            else
            {
                publish = true;
                return "Published"; 
            }
        }

        public void concealServer()
        {
            HttpWebRequest.Create(
                "http://www.poipoi.idv.tw/menu/ip_publish/publish.php?del=1").GetResponse().Close();
            publish = false;
        }

    }

    public class OnConnectEventArgs : EventArgs
    {
        public Mp3Streamer streamer;
        public TcpClient client;

        public OnConnectEventArgs(Mp3Streamer streamer, TcpClient client)
        {
            this.streamer = streamer;
            this.client = client;
        }
    }

}
