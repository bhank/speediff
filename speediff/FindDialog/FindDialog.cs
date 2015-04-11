using System;
using System.Windows.Forms;

namespace CoyneSolutions.SpeeDiff
{
    public partial class FindDialog : Form
    {
        public FindDialog()
        {
            InitializeComponent();
        }

        public delegate void FindEventHandler(FindEventArgs e);
        public event FindEventHandler Find;

        private void OnFind(FindEventArgs e)
        {
            var find = Find;
            if (find != null)
            {
                find(e);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            OnFind(new FindEventArgs
            {
                Text = txtFindString.Text,
                UseRegularExpressions = chkUseRegularExpressions.Checked,
                SearchLeft = rbLeft.Checked || rbBoth.Checked,
                SearchRight = rbRight.Checked || rbBoth.Checked,
                CaseSensitive = chkCaseSensitive.Checked,
            });
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
