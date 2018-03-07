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
    public partial class Form1 : Form
    {
        const string modulePath = ".\\modules\\";
        bool quitting;

        private static Core _Core;

        public Form1()
        {
            InitializeComponent();
            _Core = Core.getInstance(this, tabControl);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Core.loadedModules.Add(_Core);
            Core.globalHotkey = new GlobalHotkey(this);
            
            loadModuleFiles();
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(500);
        }        

        private void Form1_Shown(object sender, EventArgs e)
        {
            foreach (Expandable exa in Core.loadedModules)
            {                
                if (!Core.loadConfig(exa.dllFileName, exa.config, exa.hotkeys))
                {
                    //強制產生設定檔
                    exa.isConfigChanged = true;
                }

                Hotkey[] hks = exa.hotkeys;
                if (hks != null)
                {
                    Core.reloadHotkeys(exa.dllFileName, hks);
                }
            }
            Core.globalHotkey.start();
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
            foreach (Expandable exa in Core.loadedModules)
            {
                exa.quit();
                Core.saveConfig(exa, exa.config, exa.hotkeys);
            }
        }

        private void loadModuleFiles()
        {
            if (!Directory.Exists(modulePath))
            {
                Directory.CreateDirectory(modulePath);
                return;
            }

            string[] files = Directory.GetFiles(modulePath);
            string[] modules = Directory.GetFiles(modulePath).Where(
                s => s.StartsWith(modulePath + "ewp") && s.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)).ToArray();

            List<Expandable> loadedModules = new List<Expandable>();
            foreach(string f in modules)
            {                
                Assembly dll = Assembly.LoadFrom(f);
                Type t = dll.GetTypes().Where(_ => _.Name.StartsWith("ewp")).First();

                Expandable exa = (Expandable)Activator.CreateInstance(t, this);
                exa.dllFileName = f.Substring(modulePath.Length, f.Length - modulePath.Length - 4);
                Core.loadedModules.Add(exa);

                loadMenuStripItems(exa);
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

            foreach(Expandable exa in loadedModules)
            {
                loadModule(exa);
            }
        }

        private void loadModule(Expandable exa)
        {
            TabPage tpage = new TabPage(exa.getTitle())
            {
                Padding = new Padding(3),
                TabIndex = tabControl.TabCount,
                UseVisualStyleBackColor = true,
                Tag = exa.GetType()
            };
            tpage.Controls.Add(exa.mainPanel);
            exa.mainPanel.Dock = DockStyle.Fill;

            tabControl.Controls.Add(tpage);
            ToolStripMenuItem item = modulesToolStripMenuItem.DropDown.Items.Cast<ToolStripMenuItem>()
                .Where(_ => _.Tag.GetType().Equals(exa.GetType())).First();

            exa.run();
            item.Checked = true;
        }

        private void loadMenuStripItems(Expandable exa)
        {
            MenuStruct[] menuStructs = exa.menuStructs;
            if (menuStructs == null) return;
            foreach(MenuStruct ms in menuStructs)
            {
                ToolStripMenuItem menuItem = createMenuItem(exa, ms);
                addMenuItem(ms.field, menuItem);
            }
        }

        private void addMenuItem(MenuStripField field, ToolStripMenuItem item)
        {
            IEnumerable<ToolStripMenuItem> menuItems = MainMenuStrip.Items.Cast<ToolStripMenuItem>();
            ToolStripMenuItem menu;

            switch (field)
            {
                case MenuStripField.File:
                    menu = menuItems.Where(_ => _.Text == "File").First();
                    menu.DropDownItems.Insert(0, item);
                    break;
                case MenuStripField.View:
                    menu = menuItems.Where(_ => _.Text == "View").First();
                    menu.DropDownItems.Insert(0, item);
                    break;
                case MenuStripField.Tool:
                    menu = menuItems.Where(_ => _.Text == "Tool").First();
                    menu.DropDownItems.Insert(0, item);
                    break;
                case MenuStripField.Option:
                    menu = menuItems.Where(_ => _.Text == "Option").First();
                    menu.DropDownItems.Insert(0, item);
                    break;
                case MenuStripField.Others:
                default:
                    menu = menuItems.Where(_ => _.Text == "Others").First();
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

            if(menuStruct.dropDownItems != null)
            {
                List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();
                foreach(MenuStruct ms in menuStruct.dropDownItems)
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
                Type type = exa.GetType();             
                IEnumerable<Control> ien = tabControl.Controls.Cast<Control>();
                TabPage page = (TabPage)ien.Where(_ => type.Equals(_.Tag)).First();

                Core.saveConfig(exa, exa.config, exa.hotkeys);
                tabControl.Controls.Remove(page);                
                exa.quit();

                item.Checked = false;
            }
            else
            {
                loadModule((Expandable)item.Tag);
                //item.Checked = true;
            }
        }

        private void hotkeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form_HotkeySetting().Show();
        }

        private void optionToolStripMenuItem1_Click(object sender, EventArgs e)
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
