using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Management;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WKeys = System.Windows.Forms.Keys;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Chrome;

using Dust.Expandable;

namespace ewpYoutubeHotkey
{
    public class ewpYoutubeHotkey : Expandable
    {

        IWebDriver webDriver;
        EventFiringWebDriver eWebDriver;
        IJavaScriptExecutor javaScriptExecutor;

        Timer timer;

        string lastUrl;
        bool isVideoPage;
        bool documentCompleted;
        double volume, maxVolume = 1;

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern int GetMenuItemCount(int hMenu);

        [DllImport("user32.dll")]
        static extern int GetSystemMenu(IntPtr hMenu, bool bRevert);

        [DllImport("user32.dll")]
        static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool RemoveMenu(int hMenu, uint uPosition, uint uFlags);

        public ewpYoutubeHotkey(Form form) : base(form)
        {

        }

        public override string getTitle()
        {
            return "YoutubeHotkey";
        }

        public override void run()
        {
            init();
        }

        protected override Hotkey[] createHotkeys()
        {
            return new Hotkey[]
            {
                new Hotkey("Play", "play", new WKeys[] { WKeys.Z, WKeys.X }, new HotkeyAction(play), 0, false),
                new Hotkey("Pause", "pause", new WKeys[] { WKeys.Z, WKeys.C }, new HotkeyAction(pause), 0, false),
                new Hotkey("Next", "next", new WKeys[] { WKeys.Z, WKeys.W }, new HotkeyAction(next), 0, false),
                new Hotkey("Previous", "previous", new WKeys[] { WKeys.Z, WKeys.E }, new HotkeyAction(back), 0, false),
                new Hotkey("Volume Up", "volup", new WKeys[] { WKeys.X, WKeys.C }, new HotkeyAction(addVolume), 0, false),
                new Hotkey("Volume Down", "voldown", new WKeys[] { WKeys.X, WKeys.V }, new HotkeyAction(reduceVolume), 0, false)
            };
        }

        Config myConfig;
        private class Config : IConfig
        {
            public string home = "https://www.youtube.com";
            public double volume = 0.5;
            public bool skipAd = true;
        }
        protected override IConfig createConfig()
        {
            myConfig = new Config();
            return myConfig;
        }

        private void play()
        {
            string script =
                "var mplayer = document.getElementsByTagName(\"video\");" +
                "var iplayer = mplayer[0];" +
                "iplayer.play();";
            javaScriptExecutor.ExecuteScript(script);
        }

        private void pause()
        {
            string script =
                "var mplayer = document.getElementsByTagName(\"video\");" +
                "var iplayer = mplayer[0];" +
                "iplayer.pause();";
            javaScriptExecutor.ExecuteScript(script);
        }

        private void next()
        {
            string script =
                "var btm = document.getElementsByClassName(\"ytp-next-button ytp-button\")[0];" +
                "if (btm != null){" +
                "var url = btm.getAttribute(\"href\");" +
                "if (url != null) btm.click();" +
                "else return \"no-next\";" +
                "} else return \"no-next\";";
            string result = (string)javaScriptExecutor.ExecuteScript(script);
            if (result != null && result == "no-next")
            {
                webDriver.Navigate().Forward();
            }
        }

        private void back()
        {
            string script =
                "var btm = document.getElementsByClassName(\"ytp-prev-button ytp-button\")[0];" +
                "if (btm != null){" +
                "var url = btm.getAttribute(\"href\");" +
                "if (url != null) btm.click();" +
                "else return \"no-pre\";" +
                "} else return \"no-pre\";";
            string result = (string)javaScriptExecutor.ExecuteScript(script);
            if (result != null && result == "no-pre")
            {
                webDriver.Navigate().Back();
            }
        }

        //去除系統選單
        public static void WindowsReStyle(Process proc)
        {
            //get menu
            int HMENU = GetSystemMenu(proc.MainWindowHandle, false);
            int count = GetMenuItemCount(HMENU);
            RemoveMenu(HMENU, 6, (0x00000400 | 0x00001000));

            //force a redraw
            DrawMenuBar(proc.MainWindowHandle);
        }

        public override void quit()
        {
            timer.Stop();
            webDriver.Quit();
        }

        public static Process GetChildProcess(Process parent, string childName)
        {
            int pid = parent.Id;
            var processes = Process.GetProcessesByName(childName);
            foreach (var process in processes)
            {
                var parentId = GetParentProcess(process.Id);
                if (parentId == pid)
                {
                    return process;
                }

            }
            return null;
        }

        private static int GetParentProcess(int Id)
        {
            int parentPid = 0;
            using (ManagementObject mo = new ManagementObject($"win32_process.handle='{Id}'"))
            {
                mo.Get();
                parentPid = Convert.ToInt32(mo["ParentProcessId"]);
            }
            return parentPid;
        }

        private void init()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            options.AddArgument("user-data-dir=" + local + "/Google/Chrome/User Data/Default");
            options.AddArgument("app=https://www.youtube.com/watch?v=-tKVN2mAKRI");
            options.AddArgument("--window-size=1,1");
            options.AddArgument("--window-position=0,0");
            //options.AddArgument("headless");
            webDriver = new ChromeDriver(service, options);
            //eWebDriver = new EventFiringWebDriver(webDriver);
            javaScriptExecutor = (IJavaScriptExecutor)webDriver;
            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
            wait.Until((d) =>
            {
                try
                {
                    string readyState = javaScriptExecutor.ExecuteScript(
                        "if (document.readyState) return document.readyState;").ToString();
                    return readyState.ToLower() == "complete";
                }
                catch (Exception)
                {
                    return false;
                }
            });

            IWindow wind = webDriver.Manage().Window;
            Process driverProcess = GetChildProcess(Process.GetCurrentProcess(), "chromedriver");
            Process webProcess = GetChildProcess(driverProcess, "chrome");

            WindowsReStyle(webProcess);
            wind.Size = new Size(600, 500);
            SetParent(webProcess.MainWindowHandle, mainPanel.Handle);
            wind.Position = new Point(0, 0);

            lastUrl = "";
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {            
            string currentUrl = webDriver.Url;
            if (lastUrl != currentUrl)
            {
                Console.WriteLine("Url Changed");
                documentCompleted = false;
                lastUrl = currentUrl;
                onUrlChanged(currentUrl);
                WebDriverWait waiter = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));
                try
                {
                    waiter.Until(w => javaScriptExecutor.ExecuteScript(
                        "return document.readyState").ToString() == "complete");
                    Console.WriteLine("Document Completed");                    
                    onDocumentCompleted(currentUrl);
                    documentCompleted = true;
                }
                catch (WebDriverTimeoutException) { }                
            }
        }

        private void onUrlChanged(string url)
        {
            if (url.StartsWith("https://www.youtube.com/watch?")) isVideoPage = true;
            else isVideoPage = false;
        }

        private void onDocumentCompleted(string url)
        {
            if (isVideoPage)
            {
                maxVolume = getMaxVolume();
                Console.WriteLine("maxVolume = " + maxVolume);
                
                if (myConfig.skipAd) skipAd();

                getVolume();
                //setVolume(myConfig.volume * maxVolume);
            }            
        }

        private double getMaxVolume()
        {
            string script =
                "var v = document.getElementsByTagName(\"video\")[0];" +
                "var vs = document.getElementsByClassName(\"ytp-volume-slider-handle\")[0];" +
                "var vol = v.volume;" +
                "var percent = parseFloat(vs.getAttribute(\"style\").split(\" \")[1].replace(\"px\", \"\")) / 40;" +
                "return vol / percent;";
            object tmp = javaScriptExecutor.ExecuteScript(script);
            return Convert.ToDouble(tmp);
        }

        private double getVolume()
        {
            string script =
                "var v = document.getElementsByTagName(\"video\")[0];" +
                "return v.volume;";
            object volume = javaScriptExecutor.ExecuteScript(script);
            Console.WriteLine("o = " + volume + ", co = " + Convert.ToSingle(volume));
            this.volume = Convert.ToDouble(volume);
            return this.volume;
        }

        private void setVolume(double vol)
        {
            if (vol > maxVolume) vol = maxVolume;
            else if (vol < 0) vol = 0;
            string setVolume =
                "function setVolume(vol){" +
                "var v = document.getElementsByTagName(\"video\")[0];" +
                "var vb = document.getElementsByClassName(\"ytp-mute-button ytp-button\")[0];" +
                "var muted = v.muted;" +
                "if (vol > 0)" +
                "{" +
                "if (muted) vb.click();" +
                "v.volume = vol;" +
                "var vs = document.getElementsByClassName(\"ytp-volume-slider-handle\")[0];" +
                "var pos = 40 * vol / " + maxVolume + ";" +
                "vs.style = \"left: \" + pos + \"px\";" +
                "} else {" +
                "if (!muted) vb.click();" +
                "v.volume = 0;" +
                "}" +
                "}";
            javaScriptExecutor.ExecuteScript(setVolume + "setVolume(" + vol + ");");
        }

        private void addVolume()
        {
            if (isVideoPage && documentCompleted)
                setVolume(getVolume() + maxVolume / 15);
        }

        private void reduceVolume()
        {
            if (isVideoPage && documentCompleted)
                setVolume(getVolume() - maxVolume / 15);
        }

        private void skipAd()
        {
            string script =
                    "var skd;" +
                    "function skipAd(){" +
                    "var skip = document.getElementsByClassName(\"videoAdUiSkipButton videoAdUiAction videoAdUiFixedPaddingSkipButton\")[0];" +
                    "if (skip != null){" +
                    "skip.click();" +
                    "} else {" +
                    "clearInterval(skd);" +
                    "}" +
                    "}" +
                    "function checkAd(){" +
                    "var ad = document.getElementsByClassName(\"videoAdUiAttributionContainer videoAdUiWtaClickable\")[0];" +
                    "if (ad === null) return;" +
                    "skd = setInterval(skipAd, 1000);" +
                    "}" +
                    "setTimeout(checkAd, 1000);";
            javaScriptExecutor.ExecuteScript(script);
        }

    }
}
