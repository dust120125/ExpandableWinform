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
    public class Setting
    {
        public const string CONFIG_PATH = "./config/";

        public static List<Expandable> loadedModules = new List<Expandable>();
        public static Dictionary<string, Hotkey[]> Hotkeys = new Dictionary<string, Hotkey[]>();
        public static GlobalHotkey globalHotkey;

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
            List<Expandable.Property> property = convertConfig(config);
            List<HotkeyInfo> hotkeyInfos = getHotkeyInfo(hks);
            Config targetConfig = new Config(property, hotkeyInfos);

            using (Stream str = File.Create(CONFIG_PATH + module + ".ini"))
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
            if (!File.Exists(CONFIG_PATH + module + ".ini")) return false;

            Config tmp = new Config();
            using (Stream str = File.OpenRead(CONFIG_PATH + module + ".ini"))
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
            foreach(FieldInfo fi in fields)
            {
                object value = fi.GetValue(config);
                Console.WriteLine(fi.Name + ", as " + fi.FieldType);
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
    }
}
