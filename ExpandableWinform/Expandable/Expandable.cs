using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.IO;
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
        
        public Dictionary<string, string> strRes { get; private set; }

        public Dictionary<string, string[]> comboBoxItemRes { get; private set; }

        public interface IConfig { }

        protected ExpandableForm _form { get; private set; }
        public Panel mainPanel { get; private set; }

        public string dllFileName;

        public string dataPath = "./";

        public const string LocalizationPath = "Localization/";

        public bool isConfigChanged;

        public Hotkey[] hotkeys { get; protected set; }

        public MenuStruct[] menuStructs { get; protected set; }

        public IConfig config { get; protected set; }

        public abstract string getTitle();

        public Expandable(ExpandableForm form)
        {
            mainPanel = new Panel();
            _form = form;
            hotkeys = createHotkeys();
            config = createConfig();
            strRes = createStrRes();
            menuStructs = createMenuStructs();
            if (menuStructs != null) menuStructs = menuStructs.Reverse().ToArray();
            comboBoxItemRes = createComboBoxItemRes();
        }

        public void editHotkey(string id, Keys[] keys)
        {
            for (int i = 0; i < hotkeys.Length; i++)
            {
                if (hotkeys[i].id == id)
                    hotkeys[i].keys = keys;
            }
        }

        public abstract void run();

        public abstract void quit();

        protected abstract Hotkey[] createHotkeys();

        protected abstract IConfig createConfig();

        protected abstract MenuStruct[] createMenuStructs();

        protected abstract Dictionary<string, string> createStrRes();

        public string getString(string id)
        {
            strRes.TryGetValue(id, out string str);
            return str;
        }

        public string getString(string id, params object[] args)
        {
            strRes.TryGetValue(id, out string str);
            return string.Format(str, args);
        }

        protected abstract Dictionary<string, string[]> createComboBoxItemRes();

        public void createDefaultStringResourceFile()
        {
            string dir = dataPath + LocalizationPath;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using (StreamWriter sw = File.CreateText(dir + dllFileName + "_EN.txt"))
            {
                Dictionary<string, string> origin = createStrRes();
                foreach(KeyValuePair<string,string> kv in origin.AsEnumerable())
                {
                    string key = kv.Key;
                    string value = kv.Value;
                    sw.WriteLine(key + "= " + value);
                }
            }
        }

        public bool loadStringResourceFile(string lang)
        {
            return loadStringResourceFile(dataPath + LocalizationPath, lang);
        }

        public bool loadStringResourceFile(string path, string lang)
        {
            string fileName = path + dllFileName + "_" + lang + ".txt";
            if (!Directory.Exists(path) || !File.Exists(fileName)) return false;

            FileInfo fi = new FileInfo(fileName);
            using (StreamReader sr = fi.OpenText())
            {
                string line;
                string[] kv;
                char[] splitChar = new char[] { '=' };
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    kv = line.Split(splitChar, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (kv.Length <= 1) continue;
                    if (!strRes.ContainsKey(kv[0])) continue;
                    strRes[kv[0]] = kv[1].TrimStart();
                }
            }
            return true;
        }
    }

    public delegate void HotkeyAction(params object[] args);

    public struct Hotkey
    {
        public Hotkey(string name, string id, Keys[] keys, HotkeyAction action, int sleep, bool retriggerable, params object[] args)
        {
            this.name = name;
            this.id = id;
            this.keys = keys;
            this.action = action;
            this.args = args;
            this.sleep = sleep;
            this.retriggerable = retriggerable;
        }
        public string name { get; private set; }
        public string id { get; private set; }
        public Keys[] keys;
        public HotkeyAction action { get; private set; }
        public object[] args { get; private set; }
        public bool retriggerable { get; private set; }
        public int sleep { get; private set; }
    }

    public enum MenuStripField
    {
        File, View, Tool, Option, Others, Self
    }

    public struct MenuStruct
    {
        public MenuStruct(string name, string id) : this()
        {
            this.name = name;
            this.id = id;
        }

        public MenuStruct(MenuStripField field, string name, string id) : this()
        {
            this.field = field;
            this.name = name;
            this.id = id;
        }

        public MenuStruct(string name, string id, EventHandler action) : this(name, id)
        {
            this.action = action;
        }

        public MenuStruct(string name, string id, List<MenuStruct> dropDownItems) : this()
        {
            this.name = name;
            this.id = id;
            this.dropDownItems = dropDownItems;
        }

        public MenuStruct(MenuStripField field, string name, string id, EventHandler action) : this(field, name, id)
        {
            this.action = action;
        }

        public MenuStruct(string name, string id, EventHandler action,
            List<MenuStruct> dropDownItems) : this(name, id, action)
        {
            this.dropDownItems = dropDownItems;
        }

        public MenuStruct(MenuStripField field, string name, string id, List<MenuStruct> dropDownItems) : this()
        {
            this.field = field;
            this.name = name;
            this.id = id;
            this.dropDownItems = dropDownItems;
        }

        public MenuStruct(MenuStripField field, string name,
            string id, EventHandler action, List<MenuStruct> dropDownItems) : this(field, name, id, action)
        {
            this.dropDownItems = dropDownItems;
        }

        public MenuStripField field { get; private set; }
        public string name { get; private set; }
        public string id { get; private set; }
        public EventHandler action { get; private set; }
        public List<MenuStruct> dropDownItems { get; private set; }        
    }

    public class GlobalHotkey
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys key);

        private Form myForm;

        private Thread looper;
        private bool running;

        private Keys[] allKeys;

        public string gerenalHotkey = "";
        private bool hotkeyLimit = true;
        private string _availableHotkeys;
        public string availableHotkeys
        {
            get { return _availableHotkeys; }
            set
            {
                _availableHotkeys = value;

                hotkeys.TryGetValue(_availableHotkeys, out Hotkey[] thk);
                retriggerableHotkeys.TryGetValue(_availableHotkeys, out Hotkey[] reThk);

                if (gerenalHotkey != string.Empty)
                {
                    hotkeys.TryGetValue(gerenalHotkey, out Hotkey[] gthk);
                    retriggerableHotkeys.TryGetValue(gerenalHotkey, out Hotkey[] gReThk);
                    targetHotkeys = new Hotkey[][] { thk, gthk };
                    retriggerableTargetHotkeys = new Hotkey[][] { reThk, gReThk };
                }
                else
                {
                    targetHotkeys = new Hotkey[][] { thk };
                    retriggerableTargetHotkeys = new Hotkey[][] { reThk };
                }
            }
        }

        private Hotkey[][] targetHotkeys;
        private Hotkey[][] retriggerableTargetHotkeys;

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
                if (hotkeyLimit)
                {
                    if (retriggerableTargetHotkeys != null)
                    {
                        foreach (Hotkey[] hks in retriggerableTargetHotkeys)
                        {
                            if (hks == null) continue;
                            int tmp = matchingHotkeys(hks, pressedKeys);
                            if (tmp > sleep) sleep = tmp;
                        }
                    }
                }
                else
                {
                    foreach (Hotkey[] hks in retriggerableHotkeys.Values)
                    {
                        int tmp = matchingHotkeys(hks, pressedKeys);
                        if (tmp > sleep) sleep = tmp;
                    }
                }

                //判斷按下的按鍵是否和上次一樣，若不一樣才檢查不可重複觸發的Hotkey
                if (pressedKeys.Count != lastKeys.Count || pressedKeys.Except(lastKeys).Any())
                {
                    if (hotkeyLimit)
                    {
                        if (targetHotkeys != null)
                        {
                            foreach (Hotkey[] hks in targetHotkeys)
                            {
                                if (hks == null) continue;
                                int tmp = matchingHotkeys(hks, pressedKeys);
                                if (tmp > sleep) sleep = tmp;
                            }
                        }
                    }
                    else
                    {
                        foreach (Hotkey[] hks in hotkeys.Values)
                        {
                            int tmp = matchingHotkeys(hks, pressedKeys);
                            if (tmp > sleep) sleep = tmp;
                        }
                    }
                }

                lastKeys = pressedKeys;
                Thread.Sleep(sleep + 50);
            }
        }

        private int matchingHotkeys(Hotkey[] hks, List<Keys> pressedKeys)
        {
            int sleep = 0;
            foreach (Hotkey hk in hks)
            {
                if (checkMatch(pressedKeys, hk))
                {
                    Console.WriteLine(hk.name);
                    myForm.Invoke(hk.action, new[] { hk.args });
                    if (hk.sleep > sleep) sleep = hk.sleep;
                }
            }

            return sleep;
        }

        public List<Keys> getPressedKeys()
        {
            List<Keys> pressedKeys = new List<Keys>();
            foreach (Keys k in allKeys)
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
            foreach (Keys k in hk.keys)
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
