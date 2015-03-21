using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace CoyneSolutions.SpeeDiff
{
    public partial class frmDiff : Form
    {
        private SvnRevisionProvider svnRevisionProvider;

        public frmDiff()
        {
            InitializeComponent();

            rtbLeft.ReadOnly = rtbRight.ReadOnly = true;
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

        private async void frmDiff_Load(object sender, EventArgs e)
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

        private void lvwRevisions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                var index = e.ItemIndex;
                if (index == lvwRevisions.Items.Count - 1)
                {
                    index--; // -1 now if it's the only item.
                }
                string right, left;
                if (index == -1)
                {
                    left = right = svnRevisionProvider.Revisions[0].GetContent();
                }
                else
                {
                    right = svnRevisionProvider.Revisions[index].GetContent();
                    left = svnRevisionProvider.Revisions[index + 1].GetContent();
                }
                var builder = new SideBySideDiffBuilder(new Differ());
                var model = builder.BuildDiffModel(left, right);

                ModelToTextBox(model.OldText, rtbLeft);
                ModelToTextBox(model.NewText, rtbRight);

            }
        }

        private static void ModelToTextBox(DiffPaneModel model, RichTextBox textBox)
        {
            foreach (var line in model.Lines)
            {
                if (line.SubPieces.Count == 0)
                {
                    AppendText(textBox, line.Text + Environment.NewLine, GetPieceColor(line.Type));
                }
                else
                {
                    foreach (var piece in line.SubPieces)
                    {
                        if (piece.Text != null)
                        {
                            AppendText(textBox, piece.Text, GetPieceColor(piece.Type));
                        }
                    }
                    textBox.AppendText(Environment.NewLine);
                }
            }
        }

        private static Color GetPieceColor(ChangeType changeType)
        {
            switch (changeType)
            {
                case ChangeType.Deleted:
                    return Color.Red;
                case ChangeType.Imaginary:
                    return Color.Blue;
                case ChangeType.Inserted:
                    return Color.Green;
                case ChangeType.Modified:
                    return Color.Orange;
                case ChangeType.Unchanged:
                    return Color.Black;
            }
            return Color.Aqua; // ?
        }

        private static void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
