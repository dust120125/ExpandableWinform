using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using System.Reflection;
using System.Runtime.InteropServices;
using Dust.Expandable;

namespace ExpandableWinform
{
    public partial class Form1 : ExpandableForm
    {
        bool quitting;

        private static Core _Core;

        public Form1()
        {
            InitializeComponent();
            _Core = Core.getInstance(this, tabControl);
        }

        protected override NotifyIcon createNotifyIcon()
        {
            return notifyIcon1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Core.loadedModules.Add(_Core);
            Core.globalHotkey = new GlobalHotkey(this);
            Core.globalHotkey.gerenalHotkey = Core.CORE_ID;

            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            loadModuleFiles();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.TabPages.Count == 0) return;
            Expandable exa = (Expandable)tabControl.SelectedTab.Tag;
            Core.globalHotkey.availableHotkeys = exa.dllFileName;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            SuspendLayout();
            foreach (Expandable exa in Core.loadedModules)
            {
                Expandable.Property property = Core.setting.enabledModules.FirstOrDefault(_ => _.name == exa.dllFileName);
                if (property.name != null && !(bool)property.value) continue;
                loadModule(exa);
            }

            switchLanguage();

            foreach (Expandable exa in Core.loadedModules)
            {
                Hotkey[] hks = exa.hotkeys;
                if (hks != null)
                {
                    Core.reloadHotkeys(exa.dllFileName, hks);
                }
            }

            tabControl.SelectedIndex = 0;

            ResumeLayout();
            Core.globalHotkey.start();
        }

        private void switchLanguage()
        {
            fileToolStripMenuItem.Text = _Core.getString("str_file");
            viewToolStripMenuItem.Text = _Core.getString("str_view");
            toolToolStripMenuItem.Text = _Core.getString("str_tool");
            optionToolStripMenuItem.Text = _Core.getString("str_option");
            modulesToolStripMenuItem.Text = _Core.getString("str_modules");
            othersToolStripMenuItem.Text = _Core.getString("str_others");
            exitToolStripMenuItem.Text = _Core.getString("str_exit");
            hotkeysToolStripMenuItem.Text = _Core.getString("str_hotkeys");
            exportstrToolStripMenuItem.Text = _Core.getString("str_export_str");
            settingToolStripMenuItem.Text = _Core.getString("str_setting");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Core.setting.hideWhenClose)
            {
                Quit();
                return;
            }

            if (!quitting)
            {
                this.Hide();
                e.Cancel = true;
            }
            else Quit();
        }

        private void Quit()
        {
            Core.globalHotkey.stop();
            foreach (Expandable exa in Core.runningModules)
            {
                try
                {
                    exa.quit();
                    Core.saveConfig(exa, exa.config, exa.hotkeys);
                }
                catch (Exception e)
                {
                    string err = e.Message + "; StackTrace: " + e.StackTrace;
                    Core.errLog(err);
                }
            }
        }

        private Assembly loadAssembly(Assembly baseAssembly, string name)
        {
            using (var stream = baseAssembly.GetManifestResourceStream(name))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                return Assembly.Load(bytes);
            }
        }

        private void loadModuleFiles()
        {
            if (!Directory.Exists(Core.modulePath))
            {
                Directory.CreateDirectory(Core.modulePath);
                return;
            }

            string[] files = Directory.GetFiles(Core.modulePath);
            string[] modules = Directory.GetFiles(Core.modulePath).Where(
                s => s.StartsWith(Core.modulePath + "ewp") && s.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)).ToArray();

            List<Expandable> loadedModules = new List<Expandable>();
            foreach (string f in modules)
            {
                Assembly assembly = Assembly.LoadFrom(f);

                string[] needDlls = assembly.GetManifestResourceNames().Where(_ => _.EndsWith(".dll")).ToArray();
                foreach (string name in needDlls)
                {
                    Assembly tmp = loadAssembly(assembly, name);
                    Program.loadedAssembly[tmp.FullName] = tmp;
                }

                Type t = assembly.GetTypes().Where(_ => _.Name.StartsWith("ewp")).First();

                Expandable exa = (Expandable)Activator.CreateInstance(t, this);
                exa.dllFileName = f.Substring(Core.modulePath.Length, f.Length - Core.modulePath.Length - 4);
                //exa.dllFileName = f.Substring(Core.modulePath.Length);
                exa.dataPath = Core.moduleDataPath + exa.dllFileName + "/";
                if (!Directory.Exists(exa.dataPath)) Directory.CreateDirectory(exa.dataPath);

                Core.loadedModules.Add(exa);

                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem()
                {
                    Text = exa.getTitle(),
                    AutoSize = true,
                    Tag = exa
                };
                toolStripMenuItem.Click += ToolStripMenuItem_Click;

                modulesToolStripMenuItem.DropDown.Items.Add(toolStripMenuItem);

                loadedModules.Add(exa);
            }

            _Core.addSwitchPageHotkey(loadedModules.ToArray());

            foreach (Expandable exa in Core.loadedModules)
            {
                if (!Core.loadConfig(exa.dllFileName, exa.config, exa.hotkeys))
                {
                    //無法讀取設定檔，則強制產生設定檔
                    exa.isConfigChanged = true;
                }
            }

        }

        private void loadModule(Expandable exa)
        {
            SuspendLayout();

            if (exa.dllFileName == Core.CORE_ID)
            {
                exa.run();
                return;
            }

            TabPage tpage = new TabPage(exa.getTitle())
            {
                Padding = new Padding(3),
                TabIndex = tabControl.TabCount,
                UseVisualStyleBackColor = true,
                Tag = exa
            };
            tpage.Controls.Add(exa.mainPanel);


            tabControl.Controls.Add(tpage);
            ToolStripMenuItem item = modulesToolStripMenuItem.DropDown.Items.Cast<ToolStripMenuItem>()
                .Where(_ => _.Tag.GetType().Equals(exa.GetType())).First();

            loadMenuStripItems(exa);

            exa.run();
            Core.runningModules.Add(exa);

            exa.mainPanel.Size = new Size(1, 1);
            exa.mainPanel.Dock = DockStyle.Fill;

            if (exa.hotkeys != null) Core.enableHotkeys(exa.dllFileName, exa.hotkeys);
            if (Core.globalHotkey.availableHotkeys == null
                || Core.globalHotkey.availableHotkeys == string.Empty)
            {
                Core.globalHotkey.availableHotkeys = exa.dllFileName;
            }

            Core.setModuleEnabled(exa.dllFileName, true);
            item.Checked = true;

            ResumeLayout();
        }

        private void unloadModule(Expandable exa)
        {
            Type type = exa.GetType();
            IEnumerable<Control> ien = tabControl.Controls.Cast<Control>();
            TabPage page = (TabPage)ien.Where(_ => type.Equals(_.Tag.GetType())).First();
            removeMenuStripItems(exa);

            Core.saveConfig(exa, exa.config, exa.hotkeys);
            tabControl.Controls.Remove(page);
            exa.quit();

            Core.runningModules.Remove(exa);
            Core.setModuleEnabled(exa.dllFileName, false);
        }

        private void removeMenuStripItems(Expandable exa)
        {
            foreach (ToolStripMenuItem bItem in MainMenuStrip.Items)
            {
                if (bItem.Text == _Core.getString("str_modules")) continue;
                List<ToolStripMenuItem> removes = new List<ToolStripMenuItem>();

                foreach (ToolStripMenuItem item in bItem.DropDownItems)
                {
                    if (item.Tag == null) continue;
                    if ((string)item.Tag == exa.dllFileName)
                    {
                        removes.Add(item);
                    }
                }
                foreach (ToolStripMenuItem item in removes)
                {
                    bItem.DropDownItems.Remove(item);
                }

                if (bItem.DropDownItems.Count == 0) bItem.Visible = false;
            }
        }

        private void loadMenuStripItems(Expandable exa)
        {
            MenuStruct[] menuStructs = exa.menuStructs;
            if (menuStructs == null) return;
            foreach (MenuStruct ms in menuStructs)
            {
                ToolStripMenuItem menuItem = createMenuItem(exa, ms);
                menuItem.Tag = exa.dllFileName;
                addMenuItem(ms.field, exa, menuItem);
            }
        }

        private void addMenuItem(MenuStripField field, Expandable exa, ToolStripMenuItem item)
        {
            ToolStripMenuItem menu;

            switch (field)
            {
                case MenuStripField.File:
                    menu = fileToolStripMenuItem;
                    menu.DropDownItems.Insert(0, item);
                    break;
                case MenuStripField.View:
                    menu = viewToolStripMenuItem;
                    menu.DropDownItems.Insert(0, item);
                    break;
                case MenuStripField.Tool:
                    menu = toolToolStripMenuItem;
                    menu.DropDownItems.Insert(0, item);
                    break;
                case MenuStripField.Option:
                    menu = optionToolStripMenuItem;
                    menu.DropDownItems.Insert(0, item);
                    break;
                case MenuStripField.Self:
                    IEnumerable<ToolStripMenuItem> menuItems
                        = modulesToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>();
                    menu = menuItems.First(_ => ((Expandable)_.Tag).dllFileName == exa.dllFileName);
                    menu.DropDownItems.Insert(0, item);
                    break;
                case MenuStripField.Others:
                default:
                    menu = othersToolStripMenuItem;
                    menu.DropDownItems.Insert(0, item);
                    break;
            }
            menu.Visible = true;

        }

        private ToolStripMenuItem createMenuItem(Expandable exa, MenuStruct menuStruct)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            exa.strRes.TryGetValue("str_" + menuStruct.id, out string name);
            menuItem.Text = name != null ? name : menuStruct.name;
            menuItem.Name = exa.dllFileName + menuStruct.id;
            menuItem.AutoSize = true;

            if (menuStruct.action != null)
            {
                menuItem.Click += menuStruct.action;
            }

            if (menuStruct.dropDownItems != null)
            {
                List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();
                foreach (MenuStruct ms in menuStruct.dropDownItems)
                {
                    items.Add(createMenuItem(exa, ms));
                }
                menuItem.DropDownItems.AddRange(items.ToArray());
            }

            return menuItem;
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (item.Checked)
            {
                Expandable exa = (Expandable)item.Tag;
                unloadModule(exa);
                item.Checked = false;
                item.DropDown.Enabled = false;
            }
            else
            {
                loadModule((Expandable)item.Tag);
                item.DropDown.Enabled = true;
                //item.Checked = true;
            }
        }

        private void hotkeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form_HotkeySetting().Show();
        }

        private void exportstrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Expandable exa in Core.loadedModules)
            {
                if (exa.strRes != null) exa.createDefaultStringResourceFile();
            }
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form_Settings().Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            quitting = true;
            Close();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }
    }
}
