using Dust.Expandable;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WKeys = System.Windows.Forms.Keys;

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
        bool loop;
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

        public ewpYoutubeHotkey(ExpandableForm form) : base(form)
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
                new Hotkey("Toggle Play/Pause", "toggle", new WKeys[] { WKeys.Z, WKeys.Q }, new HotkeyAction(togglePlayPause), 0, false),
                new Hotkey("Play", "play", new WKeys[] { WKeys.Z, WKeys.X }, new HotkeyAction(play), 0, false),
                new Hotkey("Pause", "pause", new WKeys[] { WKeys.Z, WKeys.C }, new HotkeyAction(pause), 0, false),
                new Hotkey("Next", "next", new WKeys[] { WKeys.Z, WKeys.W }, new HotkeyAction(next), 0, false),
                new Hotkey("Previous", "previous", new WKeys[] { WKeys.Z, WKeys.E }, new HotkeyAction(back), 0, false),
                new Hotkey("Volume Up", "volup", new WKeys[] { WKeys.X, WKeys.C }, new HotkeyAction(addVolume), 100, true),
                new Hotkey("Volume Down", "voldown", new WKeys[] { WKeys.X, WKeys.V }, new HotkeyAction(reduceVolume), 100, true),
                new Hotkey("Loop", "loop", new WKeys[] { WKeys.Z, WKeys.R }, new HotkeyAction(clickLoop), 0, false)
            };
        }

        Config myConfig;
        private class Config : IConfig
        {
            //[NonSettable]
            [Description("str_home")]
            public string home = "https://www.youtube.com";

            [Description("str_volume")]
            [NumericOption(1.0, 0, 0.1, 3)]
            public double volume = 0.5;

            [Description("str_skip_ad")]
            public bool skipAd = true;
        }

        protected override IConfig createConfig()
        {
            myConfig = new Config();
            return myConfig;
        }

        protected override Dictionary<string, string> createStrRes()
        {
            return new Dictionary<string, string>
            {
                {"str_toggle", "Toggle Play/Pause" },
                {"str_play", "Play" },
                {"str_pause", "Pause" },
                {"str_next", "Next" },
                {"str_previous", "Previous" },
                {"str_volup", "Volume up" },
                {"str_voldown", "Volume down" },
                {"str_home", "Homepage" },
                {"str_volume", "Video volume" },
                {"str_skip_ad", "Auto skip ads" },
                {"str_loop", "Loop" }
            };
        }

        protected override Dictionary<string, string[]> createComboBoxItemRes()
        {
            return null;
        }

        protected override MenuStruct[] createMenuStructs()
        {
            return null;
        }

        private void play(params object[] args)
        {
            string script =
                "var mplayer = document.getElementsByTagName(\"video\");" +
                "var iplayer = mplayer[0];" +
                "iplayer.play();";
            javaScriptExecutor.ExecuteScript(script);
        }

        private void pause(params object[] args)
        {
            string script =
                "var mplayer = document.getElementsByTagName(\"video\");" +
                "var iplayer = mplayer[0];" +
                "iplayer.pause();";
            javaScriptExecutor.ExecuteScript(script);
        }


        private bool isPlaying(params object[] args)
        {
            string script =
                "var mplayer = document.getElementsByTagName(\"video\");" +
                "var iplayer = mplayer[0];" +
                "return iplayer.paused;";
            bool paused = Convert.ToBoolean(javaScriptExecutor.ExecuteScript(script));
            return !paused;
        }

        private void togglePlayPause(params object[] args)
        {
            string script =
                "var mplayer = document.getElementsByTagName(\"video\");" +
                "var iplayer = mplayer[0];" +
                "if (iplayer.paused) {" +
                "iplayer.play();" +
                "} else {" +
                "iplayer.pause();" +
                "}";
            javaScriptExecutor.ExecuteScript(script);
        }

        private void next(params object[] args)
        {
            //showToolbar();
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

        private void back(params object[] args)
        {
            //showToolbar();
            string script =
                "var btm = document.getElementsByClassName(\"ytp-prev-button ytp-button\")[0];" +
                "if (btm != null){" +
                "btm.click();" +
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
            timer = null;

            webDriver.Quit();
            webDriver = null;

            javaScriptExecutor = null;
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

        private void createDriverExec(string fileName)
        {
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("ewpYoutubeHotkey.chromedriver.exe"))
            {
                using(var file = File.Create(fileName))
                {
                    s.CopyTo(file);
                }
            }
        }

        private void init()
        {
            string file = dataPath + "chromedriver.exe";
            if (!File.Exists(file)) createDriverExec(file);

            ChromeDriverService service = ChromeDriverService.CreateDefaultService(dataPath);
            service.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            string home = myConfig.home;
            string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            options.AddArgument("user-data-dir=" + local + "/Google/Chrome/User Data/Default");
            options.AddArgument("app=https://www.youtube.com");
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

            mainPanel.SizeChanged += MainPanel_SizeChanged;

            WindowsReStyle(webProcess);
            int w = mainPanel.Width, h = mainPanel.Height;
            wind.Size = new Size(w, h);
            SetParent(webProcess.MainWindowHandle, mainPanel.Handle);
            wind.Position = new Point(0, 0);

            lastUrl = "";
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void MainPanel_SizeChanged(object sender, EventArgs e)
        {
            IWindow wind = webDriver.Manage().Window;
            int w = mainPanel.Width, h = mainPanel.Height;
            wind.Size = new Size(w, h);
            wind.Position = new Point(0, 0);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
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
            catch { }
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
                initVideoPageScripts();

                maxVolume = getMaxVolume();
                Console.WriteLine("maxVolume = " + maxVolume);

                if (myConfig.skipAd) skipAd();

                getVolume();  
                
                //先讓右鍵選單顯示一次，之後才能抓到循環撥放按鍵
                showContextMenu();
                //按下空白處關閉右鍵選單
                clickEmpty();

                if (isLoop() != loop) clickLoop();
                //firstOnVolumechangeEvent();                
                setVolume2(myConfig.volume * maxVolume);
                for (int i = 0; i < 10; i++)
                {
                    System.Threading.Thread.Sleep(125);
                    setVolume2(myConfig.volume * maxVolume);
                }
            }
        }

        private void embedJavaScript(string script)
        {
            javaScriptExecutor.ExecuteScript(
                "var s=window.document.createElement('script');" +
                "s.text = \"" + script + "\";" +
                "window.document.head.appendChild(s);" +
                "console.log(s);"
                );
        }

        private void initVideoPageScripts()
        {
            embedJavaScript(JScript.INIT_VIDEO_PAGE);
            embedJavaScript(JScript.VIDEO_PAGE_FUNCTIONS);
            javaScriptExecutor.ExecuteScript("_maxVolume = getMaxVolume(); lockVolume()");
        }

        private void firstOnVolumechangeEvent()
        {
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
            string onvolumechange =
                "var s = document.getElementsByTagName('video')[0];" +
                "var i = 0;" +
                "s.onvolumechange = function(){" +
                "setVolume(" + myConfig.volume * maxVolume + ");" +
                "i++;" +
                "if (i >= 2){" +
                "s.onvolumechange = null;" +
                "}" +
                "};";

            javaScriptExecutor.ExecuteScript(setVolume + onvolumechange);
        }

        private void showToolbar()
        {
            //讓撥放器狀態欄顯示出來
            string script =
                "var element = document.getElementById('movie_player');" +
                "if (window.CustomEvent)" +
                "{" +
                    "element.dispatchEvent(new CustomEvent('touchstart'));" +
                "}" +
                "else if (document.createEvent)" +
                "{" +
                    "var ev = document.createEvent('HTMLEvents');" +
                    "ev.initEvent('touchstart', true, false);" +
                    "element.dispatchEvent(ev);" +
                "}";
            javaScriptExecutor.ExecuteScript(script);
        }

        private void showContextMenu()
        {
            string script =
                "var element = document.getElementById('movie_player');" +
                "if (window.CustomEvent)" +
                "{" +
                    "element.dispatchEvent(new CustomEvent('contextmenu'));" +
                "}" +
                "else if (document.createEvent)" +
                "{" +
                    "var ev = document.createEvent('HTMLEvents');" +
                    "ev.initEvent('contextmenu', true, false);" +
                    "element.dispatchEvent(ev);" +
                "}";
            javaScriptExecutor.ExecuteScript(script);
        }

        private void clickEmpty()
        {
            string script =
                "var b = document.getElementsByTagName('body')[0];" +
                "b.click();";
            javaScriptExecutor.ExecuteScript(script);
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
            myConfig.volume = vol / maxVolume;
            isConfigChanged = true;
        }

        private void setVolume2(double vol)
        {
            if (vol > maxVolume) vol = maxVolume;
            else if (vol < 0) vol = 0;            
            javaScriptExecutor.ExecuteScript("_volume = " + vol + "; vid.onvolumechange = null; setVolume(_volume); vid.onvolumechange = volumeLock;");
            myConfig.volume = vol / maxVolume;
            isConfigChanged = true;
        }

        private void addVolume(params object[] args)
        {
            if (isVideoPage && documentCompleted)
                setVolume2(getVolume() + maxVolume / 15);
        }

        private void reduceVolume(params object[] args)
        {
            if (isVideoPage && documentCompleted)
                setVolume2(getVolume() - maxVolume / 15);
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
                    "setInterval(checkAd, 2000);";
            javaScriptExecutor.ExecuteScript(script);
        }

        private bool isLoop()
        {
            string script =
                "var mplayer = document.getElementsByTagName(\"video\");" +
                "var iplayer = mplayer[0];" +
                "return iplayer.loop;";
            return Convert.ToBoolean(javaScriptExecutor.ExecuteScript(script));
        }

        private void clickLoop(params object[] args)
        {
            string script =
                "var menu = document.getElementsByClassName(\"ytp-panel-menu\")[1];" +
                "for(var i = 0; i < menu.childElementCount; i++) { " +
                    "var element = menu.children[i];" +
                    "if (element.getAttribute(\"role\") === \"menuitemcheckbox\")" +
                    "{" +
                        "element.click();" +
                    "}" +
                "}";
            javaScriptExecutor.ExecuteScript(script);
            loop = isLoop();
        }
    }
}
