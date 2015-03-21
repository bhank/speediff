using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private IRevisionProvider revisionProvider;

        public frmDiff()
        {
            InitializeComponent();

            rtbLeft.AddPeer(rtbRight, true);

            rtbLeft.ReadOnly = rtbRight.ReadOnly = true;
            rtbLeft.Font = rtbRight.Font = new Font(FontFamily.GenericMonospace, 10);
            //rtbLeft.ScrollBars = rtbRight.ScrollBars = RichTextBoxScrollBars.Both;
            rtbLeft.WordWrap = rtbRight.WordWrap = false;


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
            lvwRevisions.ItemSelectionChanged += lvwRevisions_ItemSelectionChanged;
            Load += frmDiff_Load;
        }

        private async void frmDiff_Load(object sender, EventArgs e)
        {
            revisionProvider = RevisionProvider.GetRevisionProvider(Environment.GetCommandLineArgs()[1]);
            revisionProvider.RevisionLoaded += (s, args) =>
            {
                Invoke(new Action(() =>
                {
                    lvwRevisions.Items.Add(new ListViewItem(new[] {args.Revision.RevisionId, args.Revision.RevisionTime.ToString(), args.Revision.Author, args.Revision.Message}));
                }));
            };
            await revisionProvider.Initialize();
        }

        private async void lvwRevisions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                var index = e.ItemIndex;
                if (index == lvwRevisions.Items.Count - 1)
                {
                    index--; // -1 now if it's the only item.
                }
                int leftIndex, rightIndex;
                if (index == -1)
                {
                    leftIndex = rightIndex = 0;
                }
                else
                {
                    leftIndex = index;
                    rightIndex = index + 1;
                }
                await Task.Run(() => ShowRevisions(leftIndex, rightIndex));
                Debug.WriteLine("Done awaiting.");
            }
        }

        private void ShowRevisions(int leftIndex, int rightIndex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int,int>(ShowRevisions), leftIndex, rightIndex);
                return;
            }
            var left = revisionProvider.Revisions[leftIndex].GetContent();
            var right = revisionProvider.Revisions[rightIndex].GetContent();
            var builder = new SideBySideDiffBuilder(new Differ());
            var model = builder.BuildDiffModel(left, right);
            ModelToTextBox(model.OldText, rtbLeft);
            ModelToTextBox(model.NewText, rtbRight);
        }

        private static void ModelToTextBox(DiffPaneModel model, SynchronizedScrollRichTextBox textBox)
        {
            textBox.StopRepaint();
            textBox.Clear();
            foreach (var line in model.Lines)
            {
                var lineNumber = line.Position.HasValue ? line.Position.ToString() : string.Empty;
                AppendText(textBox, lineNumber + "\t" , Color.Cyan);

                if (line.Type == ChangeType.Deleted || line.Type == ChangeType.Inserted || line.Type == ChangeType.Unchanged)
                {
                    AppendText(textBox, line.Text + Environment.NewLine, GetPieceColor(line.Type));
                }
                else
                {
                    foreach (var piece in line.SubPieces)
                    {
                        if (piece.Type != ChangeType.Imaginary)
                        {
                            AppendText(textBox, piece.Text, GetPieceColor(piece.Type));
                        }
                    }
                    textBox.AppendText(Environment.NewLine);
                }
            }
            textBox.StartRepaint();
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
                    return Color.Empty;
            }
            return Color.Aqua; // ?
        }

        private static void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            if (color != Color.Empty)
            {
                box.SelectionBackColor = color;
            }
            box.AppendText(text);
            box.SelectionBackColor = box.BackColor;
        }
    }
}
