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

        public Form1()
        {
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Setting.globalHotkey = new GlobalHotkey(this);            
            loadModuleFiles();
        }        

        private void Form1_Shown(object sender, EventArgs e)
        {
            foreach (Expandable exa in Setting.loadedModules)
            {                
                if (!Setting.loadConfig(exa.dllFileName, exa.config, exa.hotkeys))
                {
                    //強制產生設定檔
                    exa.isConfigChanged = true;
                }

                Hotkey[] hks = exa.hotkeys;
                if (hks != null)
                {
                    Setting.Hotkeys.Add(exa.dllFileName, hks);
                    Setting.globalHotkey.setHotkeys(exa.dllFileName, hks);
                }
            }
            Setting.globalHotkey.start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Setting.globalHotkey.stop();
            foreach (Expandable exa in Setting.loadedModules)
            {
                exa.quit();
                Setting.saveConfig(exa, exa.config, exa.hotkeys);
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
            foreach(string f in modules)
            {                
                Assembly dll = Assembly.LoadFrom(f);
                Type t = dll.GetTypes().Where(_ => _.Name.StartsWith("ewp")).First();

                Expandable exa = (Expandable)Activator.CreateInstance(t, this);
                exa.dllFileName = f.Substring(modulePath.Length, f.Length - modulePath.Length - 4);
                Setting.loadedModules.Add(exa);

                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem()
                {
                    Text = exa.getTitle(),
                    AutoSize = true,
                    Tag = exa
                };
                toolStripMenuItem.Click += ToolStripMenuItem_Click;

                modulesToolStripMenuItem.DropDown.Items.Add(toolStripMenuItem);

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

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (item.Checked)
            {
                Expandable exa = (Expandable)item.Tag;
                Type type = exa.GetType();             
                IEnumerable<Control> ien = tabControl.Controls.Cast<Control>();
                TabPage page = (TabPage)ien.Where(_ => type.Equals(_.Tag)).First();

                Setting.saveConfig(exa, exa.config, exa.hotkeys);
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
    }
}
