using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dust.Expandable;

namespace TestLib
{
    public class ewpTestPlugin : Expandable
    {
        public ewpTestPlugin(Form form) : base(form)
        {
        }

        public override void run()
        {
            //MessageBox.Show("Loaded");
        }

        public override void quit()
        {
            
        }

        public override string getTitle()
        {
            return "Test";
        }

        protected override Hotkey[] createHotkeys()
        {
            return null;
        }

        protected override IConfig createConfig()
        {
            return null;
        }

        protected override MenuStruct[] createMenuStructs()
        {
            return null;
        }

        protected override Dictionary<string, string> createStrRes()
        {
            return null;
        }

        protected override Dictionary<string, string[]> createComboBoxItemRes()
        {
            return null;
        }
    }
}
