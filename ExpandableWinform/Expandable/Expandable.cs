using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Serialization;

namespace Dust.Expandable
{
    public abstract class Expandable
    {

        [Serializable]
        [XmlType("Property")]
        public struct Property
        {
            public Property(string name, object value)
            {
                this.name = name;
                this.value = value;
            }
            public string name;
            public object value;
        }

        public interface IConfig { }

        protected Form _form { get; private set; }
        public Panel mainPanel { get; private set;}

        public string dllFileName;

        public Hotkey[] hotkeys { get; protected set; }

        public IConfig config { get; protected set; }

        public abstract string getTitle();

        public Expandable(Form form)
        {
            mainPanel = new Panel();
            _form = form;
            hotkeys = createHotkeys();
            config = createConfig();
        }

        public abstract void run();

        public abstract void quit();

        protected abstract Hotkey[] createHotkeys();

        protected abstract IConfig createConfig();
    }

    public delegate void HotkeyAction();
    
    public struct Hotkey
    {
        public Hotkey(string name, string id, Keys[] keys, HotkeyAction action, int sleep, bool retriggerable)
        {
            this.name = name;
            this.id = id;
            this.keys = keys;
            this.action = action;
            this.sleep = sleep;
            this.retriggerable = retriggerable;
        }
        public string name { get; private set; }
        public string id { get; private set; }
        public Keys[] keys;
        public HotkeyAction action { get; private set; }
        public bool retriggerable { get; private set; }
        public int sleep { get; private set; }
    }

    public class GlobalHotkey
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys key);

        private Form myForm;

        private Thread looper;
        private bool running;

        private Keys[] allKeys;
        private Dictionary<string, Hotkey[]> hotkeys;
        private Dictionary<string, Hotkey[]> retriggerableHotkeys;
        private List<Keys> lastKeys;

        public GlobalHotkey(Form form)
        {
            myForm = form;
            hotkeys = new Dictionary<string, Hotkey[]>();
            retriggerableHotkeys = new Dictionary<string, Hotkey[]>();
            lastKeys = new List<Keys>();
            List<Keys> tmp = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList();
            tmp.Remove(Keys.None);
            allKeys = tmp.ToArray();
        }

        public void start()
        {
            looper = new Thread(run);
            looper.Start();
        }

        public void stop()
        {
            looper.Abort();
            running = false;
        }

        private void run()
        {
            running = true;
            List<Keys> pressedKeys;
            int sleep = 0;
            while (running)
            {
                pressedKeys = getPressedKeys();

                //總是檢查可重複觸發的Hotkey
                foreach (Hotkey[] hks in retriggerableHotkeys.Values)
                {
                    foreach (Hotkey hk in hks)
                    {
                        if (checkMatch(pressedKeys, hk))
                        {
                            myForm.Invoke(hk.action);
                            if (hk.sleep > sleep) sleep = hk.sleep;
                        }
                    }
                }

                //判斷按下的按鍵是否和上次一樣，若不一樣才檢查不可重複觸發的Hotkey
                if (pressedKeys.Count != lastKeys.Count || pressedKeys.Except(lastKeys).Any())
                {
                    foreach (Hotkey[] hks in hotkeys.Values)
                    {
                        foreach (Hotkey hk in hks)
                        {
                            if (checkMatch(pressedKeys, hk))
                            {
                                myForm.Invoke(hk.action);
                                if (hk.sleep > sleep) sleep = hk.sleep;
                            }
                        }
                    }
                }                

                lastKeys = pressedKeys;                
                Thread.Sleep(sleep + 50);
            }            
        }

        private List<Keys> getPressedKeys()
        {
            List<Keys> pressedKeys = new List<Keys>();
            foreach(Keys k in allKeys)
            {
                if (GetAsyncKeyState(k) < 0)
                {
                    pressedKeys.Add(k);
                }
            }
            return pressedKeys;
        }

        private bool checkMatch(List<Keys> pressedKeys, Hotkey hk)
        {
            foreach(Keys k in hk.keys)
            {
                if (pressedKeys.Count != hk.keys.Length || !pressedKeys.Contains(k))
                {
                    return false;
                }
            }
            return true;
        }

        public void setHotkeys(string moudleName, Hotkey[] hks)
        {
            Hotkey[] tmp = hks.Where(_ => _.id != null && _.keys != null && _.action != null).ToArray();
            Hotkey[] hk = tmp.Where(_ => _.retriggerable == false).ToArray();
            Hotkey[] rthk = tmp.Where(_ => _.retriggerable == true).ToArray();
            hotkeys[moudleName] = hk;
            retriggerableHotkeys[moudleName] = rthk;
        }

    }
}
