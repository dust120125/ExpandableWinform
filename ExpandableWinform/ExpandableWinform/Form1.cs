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
                
        GlobalHotkey globalHotkey;        
        

        public Form1()
        {
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            globalHotkey = new GlobalHotkey(this);            
            loadEwpModules();
        }        

        private void Form1_Shown(object sender, EventArgs e)
        {
            foreach (Expandable exa in Setting.loadedModules)
            {
                Hotkey[] hks = exa.hotkeys;
                if (hks != null)
                {
                    Setting.Hotkeys.Add(exa.dllFileName, hks);                    
                    globalHotkey.setHotkeys(exa.dllFileName, hks);
                }
                Setting.loadConfig(exa.dllFileName, exa.config, exa.hotkeys);
                exa.run();
            }
            globalHotkey.start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            globalHotkey.stop();
            foreach (Expandable exa in Setting.loadedModules)
            {
                exa.quit();
                Setting.saveConfig(exa.dllFileName, exa.config, exa.hotkeys);
            }            
        }

        private void loadEwpModules()
        {
            string[] fff = Directory.GetFiles(modulePath);
            string[] modules = Directory.GetFiles(modulePath).Where(
                s => s.StartsWith(modulePath + "ewp") && s.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach(string f in modules)
            {                
                Assembly dll = Assembly.LoadFrom(f);
                Type t = dll.GetTypes().Where(_ => _.Name.StartsWith("ewp")).First();
                Expandable exa = (Expandable)Activator.CreateInstance(t, this);
                exa.dllFileName = f.Substring(modulePath.Length, f.Length - modulePath.Length - 4);

                TabPage tpage = new TabPage(exa.getTitle());
                tpage.Controls.Add(exa.mainPanel);
                tpage.Padding = new Padding(3);
                tpage.TabIndex = tabControl.TabCount;
                tpage.UseVisualStyleBackColor = true;
                tabControl.Controls.Add(tpage);

                exa.mainPanel.Dock = DockStyle.Fill;
                Setting.loadedModules.Add(exa);
            }
        }

        private void hotkeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form_HotkeySetting().Show();
        }
    }
}
