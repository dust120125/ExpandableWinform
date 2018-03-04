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
        public Form_HotkeySetting()
        {
            InitializeComponent();
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
                Hotkey[] hks = Setting.Hotkeys[module];
                if (hks == null || hks.Length == 0) continue;

                Hotkey[] tmpHks = hks.ToArray();
                TabPage page = new TabPage();
                page.Text = Setting.loadedModules.Where(_ => _.dllFileName == module).First().getTitle();
                page.AutoScroll = true;
                page.Location = new Point(4, 22);
                page.Padding = new Padding(3);
                page.Size = new Size(290, 294);
                page.TabIndex = tabControl1.TabCount;
                page.UseVisualStyleBackColor = true;
                page.Name = module;

                for(int i = 0; i < tmpHks.Length; i++)
                {
                    TextBox tb = new TextBox();
                    tb.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                    tb.Location = new Point(101, 6 + i * 28);
                    tb.Size = new Size(155, 22);
                    //tb.TabIndex = i * 2;
                    tb.Tag = tmpHks[i];

                    Button bt = new Button();
                    bt.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                    bt.Location = new Point(262, 6 + i * 28);
                    bt.Size = new Size(22, 22);
                    //bt.TabIndex = i * 2 + 1;
                    bt.Text = "☓";
                    bt.UseVisualStyleBackColor = true;

                    Label lab = new Label();
                    lab.AutoSize = true;
                    lab.Location = new Point(8, 11 + i * 28);
                    lab.Size = new Size(33, 12);
                    lab.Text = tmpHks[i].name;

                    page.Controls.Add(tb);
                    page.Controls.Add(bt);
                    page.Controls.Add(lab);
                }

                tabControl1.Controls.Add(page);
            }
        }

    }
}
