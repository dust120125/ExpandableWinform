using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dust.Expandable;

namespace TestLib
{
    public class TestPlugin : Expandable
    {
        public override void run()
        {
            MessageBox.Show("Loaded");
        }

        public override void quit()
        {
            
        }

    }
}
