using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Dust.Expandable;

namespace ExpandableWinform
{
    public partial class Form_HotkeySetting : Form
    {
        private static StringBuilder stringBuilder;

        public Form_HotkeySetting()
        {
            InitializeComponent();

            stringBuilder = new StringBuilder();
            InitPages();
        }

        private void InitGeneralPage()
        {
            for (int i = 0; i < Setting.loadedModules.Count; i++)
            {
                TextBox tb = new TextBox();
                tb.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                tb.Location = new Point(101, 34 + i * 28);
                tb.Size = new Size(155, 22);
                //tb.TabIndex = i * 2;
                tb.Tag = Setting.loadedModules[i];

                Button bt = new Button();
                bt.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                bt.Location = new Point(262, 34 + i * 28);
                bt.Size = new Size(22, 22);
                //bt.TabIndex = i * 2 + 1;
                bt.Text = "☓";
                bt.UseVisualStyleBackColor = true;

                Label lab = new Label();
                lab.AutoSize = true;
                lab.Location = new Point(8, 39 + i * 28);
                lab.Size = new Size(33, 12);
                lab.Text = Setting.loadedModules[i].getTitle();

                tabPage1.Controls.Add(tb);
                tabPage1.Controls.Add(bt);
                tabPage1.Controls.Add(lab);

            }
        }

        private void InitPages()
        {
            InitGeneralPage();

            foreach (string module in Setting.Hotkeys.Keys)
            {
                Expandable exa = Setting.loadedModules.Where(_ => _.dllFileName == module).First();
                Hotkey[] hks = Setting.Hotkeys[module];
                if (hks == null || hks.Length == 0) continue;

                Hotkey[] tmpHks = hks.ToArray();
                TabPage page = new TabPage
                {
                    Text = exa.getTitle(),
                    AutoScroll = true,
                    Location = new Point(4, 22),
                    Padding = new Padding(3),
                    Size = new Size(290, 294),
                    TabIndex = tabControl1.TabCount,
                    UseVisualStyleBackColor = true,
                    Name = module
                };

                List<TextBox> textBoxes = new List<TextBox>();

                for (int i = 0; i < tmpHks.Length; i++)
                {                    
                    TextBox tb = new TextBox
                    {
                        Anchor = (AnchorStyles.Top | AnchorStyles.Right),                        
                        Location = new Point(101, 6 + i * 28),
                        Size = new Size(155, 22),
                        //tb.TabIndex = i * 2;
                        Tag = tmpHks[i]
                    };
                    tb.KeyDown += HotkeyBox_KeyDown;
                    textBoxes.Add(tb);
                    if (tmpHks[i].keys != null)
                    {
                        tb.Text = getHotKeyString(tmpHks[i].keys.ToList());
                    }

                    Button bt = new Button
                    {
                        Anchor = (AnchorStyles.Top | AnchorStyles.Right),
                        Location = new Point(262, 6 + i * 28),
                        Size = new Size(22, 22),
                        //bt.TabIndex = i * 2 + 1;
                        Text = "☓",
                        UseVisualStyleBackColor = true,
                        Tag = tb
                    };
                    bt.Click += Delete_Click;
                                        
                    Label lab = new Label
                    {
                        AutoSize = true,
                        Location = new Point(8, 11 + i * 28),
                        Size = new Size(33, 12)
                    };
                    bool got = exa.strRes.TryGetValue("str_" + tmpHks[i].id, out string desc);
                    lab.Text = got ? desc : tmpHks[i].name;

                    page.Controls.Add(tb);
                    page.Controls.Add(bt);
                    page.Controls.Add(lab);
                }

                page.Tag = new object[] { exa, textBoxes };
                tabControl1.Controls.Add(page);
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            Button btn = ((Button)sender);
            TextBox textb = ((TextBox)btn.Tag);
            textb.Text = "";
            Hotkey hk = (Hotkey)textb.Tag;
            hk.keys = null;
            textb.Tag = hk;
        }

        private void HotkeyBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            List<Keys> pressedKeys = Setting.globalHotkey.getPressedKeys();
            TextBox textb = ((TextBox)sender);
            textb.Text = getHotKeyString(pressedKeys);
            Hotkey hk = (Hotkey)textb.Tag;
            hk.keys = pressedKeys.ToArray();
            textb.Tag = hk;
        }

        private void saveHotkeys()
        {
            foreach(TabPage page in tabControl1.TabPages)
            {
                if (page.Tag == null) continue;
                object[] tags = (object[])page.Tag;
                Expandable exa = (Expandable)tags[0];
                exa.isConfigChanged = true;

                List<TextBox> textBoxes = (List<TextBox>)tags[1];

                foreach(TextBox tb in textBoxes)
                {
                    Hotkey hk = (Hotkey)tb.Tag;
                    exa.editHotkey(hk.id, hk.keys);
                }
                Setting.Hotkeys[exa.dllFileName] = exa.hotkeys;
                Setting.globalHotkey.setHotkeys(exa.dllFileName, exa.hotkeys);
            }
        }

        private static string getHotKeyString(List<Keys> keys)
        {
            stringBuilder.Clear();
            keys.ForEach(k => stringBuilder.Append(k.ToString() + " + "));

            if (stringBuilder.Length == 0) return "";
            stringBuilder.Remove(stringBuilder.Length - 3, 3);
            return stringBuilder.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveHotkeys();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
