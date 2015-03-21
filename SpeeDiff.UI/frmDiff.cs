using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoyneSolutions.SpeeDiff
{
    public partial class frmDiff : Form
    {
        private SvnRevisionProvider svnRevisionProvider;

        public frmDiff()
        {
            InitializeComponent();

            rtbLeft.vScroll += (msg) =>
            {
                msg.HWnd = rtbRight.Handle;
                rtbRight.PubWndProc(ref msg);
            };
            rtbRight.vScroll += (msg) =>
            {
                msg.HWnd = rtbLeft.Handle;
                rtbLeft.PubWndProc(ref msg);
            };

            lvwRevisions.FullRowSelect = true;
            lvwRevisions.Columns.AddRange(
                new[]
                {
                    new ColumnHeader {Text = "Revision", Width = 50},
                    new ColumnHeader {Text = "Time", Width = 100},
                    new ColumnHeader {Text = "Author", Width = 50},
                    new ColumnHeader {Text = "Message", Width = 750},
                }
            );
            //lvwRevisions.Items.Add(new ListViewItem(new[] {"1", "2", "3", "4"}));
            //lvwRevisions.Items.Add(new ListViewItem(new[] {"1", "2", "3", "4"}));
            //lvwRevisions.Items.Add(new ListViewItem(new[] {"1", "2", "3", "4"}));
            //lvwRevisions.Items.Add(new ListViewItem(new[] {"1", "2", "3", "4"}));
            //lvwRevisions.Items.Add(new ListViewItem(new[] {"1", "2", "3", "4"}));
            lvwRevisions.ItemSelectionChanged += lvwRevisions_ItemSelectionChanged;
            Load += frmDiff_Load;
            //Test1();
        }

        async void frmDiff_Load(object sender, EventArgs e)
        {
            svnRevisionProvider = new SvnRevisionProvider();
            svnRevisionProvider.RevisionLoaded += (s, args) =>
            {
                Invoke(new Action(() =>
                {
                    lvwRevisions.Items.Add(new ListViewItem(new[] {args.Revision.RevisionId, args.Revision.RevisionTime.ToString(), args.Revision.Author, args.Revision.Message}));
                }));
            };
            await svnRevisionProvider.Initialize(Environment.GetCommandLineArgs()[1]);
        }

        void lvwRevisions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                var index = e.ItemIndex;
                if (index == lvwRevisions.Items.Count - 1)
                {
                    index--; // -1 now if it's the only item.
                }
                if (index == -1)
                {
                    rtbLeft.Text = rtbRight.Text = svnRevisionProvider.Revisions[0].GetContent();
                }
                else
                {
                    rtbRight.Text = svnRevisionProvider.Revisions[index].GetContent();
                    rtbLeft.Text = svnRevisionProvider.Revisions[index + 1].GetContent();
                }
            }
        }
    }
}
