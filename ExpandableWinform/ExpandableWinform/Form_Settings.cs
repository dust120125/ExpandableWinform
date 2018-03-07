using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using Dust.Expandable;
using System.Collections;

namespace ExpandableWinform
{
    public partial class Form_Settings : Form
    {

        Type[] NumericTypes = new Type[]
        { typeof(Int16), typeof(UInt16), typeof(Int32), typeof(UInt32),
            typeof(Int64), typeof(UInt64), typeof(float), typeof(double) };

        public Form_Settings()
        {
            InitializeComponent();
            InitPages();
        }

        private void InitPages()
        {
            foreach (Expandable exa in Core.loadedModules)
            {
                Expandable.IConfig config = exa.config;
                if (config == null) continue;

                string module = exa.dllFileName;
                string text = module == Core.CORE_ID ? "General" : exa.getTitle();                
                Dictionary<string, string> strRes = exa.strRes;

                TabPage page = new TabPage
                {
                    Text = text,
                    AutoScroll = true,
                    Location = new Point(4, 22),
                    Padding = new Padding(3),
                    Size = new Size(290, 294),
                    TabIndex = tabControl1.TabCount,
                    UseVisualStyleBackColor = true,
                    Name = module
                };

                Type type = config.GetType();
                FieldInfo[] fields = type.GetFields();
                List<Control> controls = new List<Control>();
                int index = 0;
                foreach (FieldInfo fi in fields)
                {
                    dynamic value = Convert.ChangeType(fi.GetValue(config), fi.FieldType);
                    Control control = createControl(ref index, page, fi, exa, value);
                    if (control == null) continue;
                    controls.Add(control);
                }

                page.Tag = new object[] { exa, controls.ToArray(), config };
                tabControl1.Controls.Add(page);
            }
        }

        int valueBox_x = 135;
        private Control createControl
            (ref int index, TabPage page, FieldInfo field, Expandable exa, dynamic value)
        {
            Dictionary<string, string> strRes = exa.strRes;
            Dictionary<string, string[]> comboBoxRes = exa.comboBoxItemRes;

            string desc = field.Name;
            Type type = field.FieldType;

            string[] comboBoxItems = null;
            decimal maximun = 100, minimun = 0, increment = 1;
            int decimalPlaces = 0;

            if (field.GetCustomAttributes(typeof(NonSettable), false).Length != 0) return null;

            object[] oAttr = field.GetCustomAttributes(typeof(Description), false);
            if (oAttr.Length != 0)
            {
                Description attr = (Description)oAttr[0];
                desc = strRes[attr.strResId];
            }

            oAttr = field.GetCustomAttributes(typeof(NumericOption), false);
            if (oAttr.Length != 0)
            {
                NumericOption attr = (NumericOption)oAttr[0];
                maximun = attr.maximun;
                minimun = attr.minimun;
                increment = attr.increment;
                decimalPlaces = attr.decimalPlaces;
            }

            oAttr = field.GetCustomAttributes(typeof(ComboBoxOption), false);
            if (oAttr.Length != 0)
            {
                ComboBoxOption attr = (ComboBoxOption)oAttr[0];
                comboBoxItems = comboBoxRes[attr.key];
            }

            Label lab = new Label
            {
                AutoSize = true,
                Size = new Size(33, 12),
                Text = desc
            };

            Control control = null;
            if (type.Equals(typeof(string)) && comboBoxItems != null)
            {
                ComboBox tmp = new ComboBox
                {
                    Anchor = (AnchorStyles.Top | AnchorStyles.Right),
                    Text = value,
                    Location = new Point(valueBox_x, 6 + index * 28),
                    Size = new Size(150, 22),
                };
                tmp.Items.AddRange(comboBoxItems);
                control = tmp;
                lab.Location = new Point(8, 6 + index * 28 + 3);
            }
            else if (type.Equals(typeof(string)))
            {
                control = new TextBox
                {
                    Anchor = (AnchorStyles.Top | AnchorStyles.Right),
                    Text = value,
                    Location = new Point(valueBox_x, 6 + index * 28),
                    Size = new Size(150, 22),
                };
                lab.Location = new Point(8, 6 + index * 28 + 5);
            }
            else if (type.Equals(typeof(bool)))
            {
                control = new CheckBox()
                {
                    Anchor = (AnchorStyles.Top | AnchorStyles.Right),
                    Checked = value,
                    Location = new Point(valueBox_x, 6 + index * 28),
                    Size = new Size(15, 14),
                    Text = ""
                };
                lab.Location = new Point(8, 6 + index * 28 + 1);
            }
            else if (NumericTypes.Contains(type))
            {
                control = new NumericUpDown()
                {
                    Anchor = (AnchorStyles.Top | AnchorStyles.Right),
                    Value = Convert.ToDecimal(value),
                    DecimalPlaces = decimalPlaces,
                    Maximum = maximun,
                    Minimum = minimun,
                    Increment = increment,
                    TextAlign = HorizontalAlignment.Right,
                    Location = new Point(valueBox_x, 6 + index * 28),
                    Size = new Size(100, 22),
                };
                lab.Location = new Point(8, 6 + index * 28 + 2);
            }

            if (control != null)
            {
                control.Tag = field;
            }
            page.Controls.Add(control);
            page.Controls.Add(lab);
            index++;

            return control;
        }

        private void saveSettings()
        {
            foreach (TabPage page in tabControl1.TabPages)
            {
                if (page.Tag == null) continue;                
                object[] tags = (object[])page.Tag;

                Expandable exa = (Expandable)tags[0];
                exa.isConfigChanged = true;

                Control[] controls = (Control[])tags[1];
                Expandable.IConfig config = (Expandable.IConfig)tags[2];

                saveSetting(controls, config);
            }
        }

        private void saveSetting(Control[] controls, Expandable.IConfig config)
        {
            foreach (Control control in controls)
            {
                FieldInfo field = (FieldInfo)control.Tag;
                Type type = field.FieldType;
                object value = null;

                if (type.Equals(typeof(string)))
                {
                    value = control.Text;
                }
                else if (type.Equals(typeof(bool)))
                {
                    value = ((CheckBox)control).Checked;
                }
                else if (NumericTypes.Contains(type))
                {
                    value = ((NumericUpDown)control).Value;
                    if (type.Equals(typeof(double)))
                    {
                        value = Convert.ToDouble(value);
                    }
                    else if (type.Equals(typeof(float)))
                    {
                        value = Convert.ToSingle(value);
                    }
                }

                if (value == null) return;
                field.SetValue(config, value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveSettings();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
