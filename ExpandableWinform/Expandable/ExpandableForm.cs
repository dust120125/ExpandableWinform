using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Dust.Expandable
{
    public abstract class ExpandableForm : Form
    {
        private NotifyIcon _notifyIcon;
        public NotifyIcon notifyIcon
        {
            get
            {
                if (_notifyIcon == null) _notifyIcon = createNotifyIcon();
                return _notifyIcon;
            }
        }

        protected abstract NotifyIcon createNotifyIcon();

    }
}
