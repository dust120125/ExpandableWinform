using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

using Dust.Expandable;

namespace ExpandableWinform
{
    public class Core : Expandable
    {
        public const string CORE_ID = "#mwin_core!";
        public const string CORE_CONFIG_PATH = "./config.ini";

        public const string CONFIG_PATH = "./config/";

        public static List<Expandable> loadedModules = new List<Expandable>();
        public static Dictionary<string, Hotkey[]> ModuleHotkeys = new Dictionary<string, Hotkey[]>();
        public static GlobalHotkey globalHotkey;

        private static TabControl tabControl;

        private static Core _Core;

        public static Core getInstance(Form form, TabControl tabControl)
        {
            if (_Core != null) return _Core;
            return _Core = new Core(form, tabControl);
        }

        public Core(Form form, TabControl _tabControl) : base(form)
        {
            this.dllFileName = CORE_ID;
            tabControl = _tabControl;
        }

        [Serializable]
        public struct HotkeyInfo
        {
            public HotkeyInfo(string id, Keys[] keys)
            {
                this.id = id;
                this.keys = keys;
            }

            public string id;
            public Keys[] keys;
        }

        [Serializable]
        public struct Config
        {
            public Config(List<Expandable.Property> property, List<HotkeyInfo> hotkey)
            {
                this.Properties = property;
                this.Hotkeys = hotkey;
            }

            public List<Expandable.Property> Properties;
            public List<HotkeyInfo> Hotkeys;
        }

        public static void saveConfig(Expandable exa, Expandable.IConfig config, Hotkey[] hks)
        {
            if (!exa.isConfigChanged) return;
            if (!Directory.Exists(CONFIG_PATH)) Directory.CreateDirectory(CONFIG_PATH);

            string module = exa.dllFileName;
            string file = module == CORE_ID ? CORE_CONFIG_PATH : CONFIG_PATH + module + ".ini";

            List<Expandable.Property> property = convertConfig(config);
            List<HotkeyInfo> hotkeyInfos = getHotkeyInfo(hks);
            Config targetConfig = new Config(property, hotkeyInfos);

            using (Stream str = File.Create(file))
            {
                try
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Config));
                    xs.Serialize(str, targetConfig);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public static bool loadConfig(string module, Expandable.IConfig config, Hotkey[] hks)
        {
            string file = module == CORE_ID ? CORE_CONFIG_PATH : CONFIG_PATH + module + ".ini";
            if (!File.Exists(file)) return false;

            Config tmp = new Config();
            using (Stream str = File.OpenRead(file))
            {
                try
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Config));
                    tmp = (Config)xs.Deserialize(str);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            if (tmp.Properties.Count == 0 && tmp.Hotkeys.Count == 0) return false;

            if (tmp.Hotkeys.Count > 0)
            {
                for (int i = 0; i < hks.Length; i++)
                {
                    HotkeyInfo hki = tmp.Hotkeys.FirstOrDefault(_ => _.id == hks[i].id);
                    if (hki.id != null) hks[i].keys = hki.keys;
                }
            }

            if (tmp.Properties.Count > 0)
            {
                Type confType = config.GetType();
                FieldInfo[] fields = confType.GetFields();
                foreach (FieldInfo fi in fields)
                {
                    object value = tmp.Properties.Where(_ => _.name == fi.Name).First().value;
                    if (value.GetType().AssemblyQualifiedName == fi.FieldType.AssemblyQualifiedName)
                    {
                        fi.SetValue(config, value);
                    }
                }
            }

            return true;
        }

        private static List<Expandable.Property> convertConfig(Expandable.IConfig config)
        {
            if (config == null) return null;

            List<Expandable.Property> fieldList = new List<Expandable.Property>();
            Type type = config.GetType();
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo fi in fields)
            {
                object value = fi.GetValue(config);
                //Console.WriteLine(fi.Name + ", as " + fi.FieldType);
                Expandable.Property property = new Expandable.Property(fi.Name, value);
                fieldList.Add(property);
            }
            return fieldList;
        }

        private static List<HotkeyInfo> getHotkeyInfo(Hotkey[] hks)
        {
            if (hks == null) return null;
            List<HotkeyInfo> hotkeyInfo = new List<HotkeyInfo>();
            foreach (Hotkey hk in hks)
            {
                hotkeyInfo.Add(new HotkeyInfo(hk.id, hk.keys));
            }
            return hotkeyInfo;
        }

        public override string getTitle()
        {
            return "MWinCore";
        }

        public override void run()
        {

        }

        public override void quit()
        {

        }
        
        protected override Hotkey[] createHotkeys()
        {
            return new Hotkey[] { new Hotkey("Switch page", "switch_tab", null, switchPage, 0, false) };
        }

        protected override Dictionary<string, string[]> createComboBoxItemRes()
        {
            return new Dictionary<string, string[]>()
            {
                { "languages", new string[]{"English", "Chinese"} }
            };
        }

        public static Setting setting { get; private set; }
        public class Setting : IConfig
        {
            [Description("str_lang")]
            [ComboBoxOption("languages")]
            public string language = "English";

            [Description("str_hide")]
            public bool hideWhenClose = true;
        }

        protected override IConfig createConfig()
        {
            setting = new Setting();
            return setting;
        }

        protected override MenuStruct[] createMenuStructs()
        {
            return null;
        }

        protected override Dictionary<string, string> createStrRes()
        {
            return new Dictionary<string, string>()
            {
                { "str_lang", "Language" },
                { "str_hide", "Hide when window close" },
                { "str_switch_tab", "Switch to next page" }
            };
        }

        public static void reloadHotkeys(string module, Hotkey[] hks)
        {
            ModuleHotkeys[module] = hks;
            globalHotkey.setHotkeys(module, hks);
        }

        public void addSwitchPageHotkey(params Expandable[] exas)
        {
            List<Hotkey> hks = new List<Hotkey>();
            foreach(Expandable exa in exas)
            {
                string name = exa.getTitle();
                string id = "#sw_" + exa.dllFileName + "_!";
                hks.Add(new Hotkey(name, id, null, switchToPage, 0, false, exa));
            }
            hotkeys = hotkeys.Concat(hks).ToArray();
            reloadHotkeys(CORE_ID, hotkeys);
        }

        private void switchToPage(params object[] args)
        {            
            Expandable exa = (Expandable)args[0];
            Type type = exa.GetType();
            TabPage page = tabControl.TabPages.Cast<TabPage>().First(_ => _.Tag.Equals(type));
            tabControl.SelectedTab = page;
        }

        private void switchPage(params object[] args)
        {
            int count = tabControl.TabCount;
            int index = tabControl.SelectedIndex + 1 >= count ? 0 : tabControl.SelectedIndex + 1;
            tabControl.SelectTab(index);
        }

    }
}
