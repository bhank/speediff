using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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

            //rtbLeft.AddPeers(rtbRight, rtbLeftNumbers, rtbRightNumbers);

            foreach (var box in new[] {rtbLeft, rtbRight, rtbLeftNumbers, rtbRightNumbers})
            {
                box.ReadOnly = true;
                box.Font = new Font(FontFamily.GenericMonospace, 10);
                box.WordWrap = false;
            }

            foreach (var box in new[] {rtbLeftNumbers, rtbRightNumbers})
            {
                box.ScrollBars = RichTextBoxScrollBars.None;
                box.BackColor = Color.Cyan;
                box.ReadOnly = true;
                //box.Enabled = false;
                box.Cursor = Cursors.Default;
            }
            // Disabling the line-number boxes kills their background color... try this instead.
            rtbLeftNumbers.Enter += (sender, args) => lvwRevisions.Focus();
            rtbRightNumbers.Enter += (sender, args) => lvwRevisions.Focus();

            lvwRevisions.FullRowSelect = true;
            lvwRevisions.Columns.AddRange(
                new[]
                {
                    new ColumnHeader {Text = "Revision", Width = 70},
                    new ColumnHeader {Text = "Time", Width = 125},
                    new ColumnHeader {Text = "Author", Width = 75},
                    new ColumnHeader {Text = "Message", Width = 650},
                }
                );
            lvwRevisions.ItemSelectionChanged += lvwRevisions_ItemSelectionChanged;
            Load += frmDiff_Load;
        }

        private async void frmDiff_Load(object sender, EventArgs e)
        {
            revisionProvider = RevisionProvider.GetRevisionProvider(Environment.GetCommandLineArgs()[1]);
            var revisions = await revisionProvider.LoadRevisions();
            lvwRevisions.Items.AddRange(revisions.Select(r => new ListViewItem(new[] {r.RevisionId, r.RevisionTime.ToString(), r.Author, r.Message})).ToArray());
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
                    leftIndex = index + 1;
                    rightIndex = index;
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
            ModelToTextBox(model.OldText, rtbLeft, rtbLeftNumbers);
            ModelToTextBox(model.NewText, rtbRight, rtbRightNumbers);
        }

        private static void ModelToTextBox(DiffPaneModel model, SynchronizedScrollRichTextBox textBox, SynchronizedScrollRichTextBox lineNumbersTextBox)
        {
            foreach (var box in new[] {textBox, lineNumbersTextBox})
            {
                box.StopRepaint();
                box.SaveScrollPosition();
                box.Clear();
            }

            var lineNumbersText = new StringBuilder();

            foreach (var line in model.Lines)
            {
                var lineNumber = line.Position.HasValue ? line.Position.ToString() : string.Empty;
                lineNumbersText.AppendLine(lineNumber);
                //AppendText(lineNumbersTextBox, lineNumber + Environment.NewLine, Color.Empty);

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

            lineNumbersTextBox.Text = lineNumbersText.ToString();

            foreach (var box in new[] {textBox, lineNumbersTextBox})
            {
                box.RestoreScrollPosition();
                box.StartRepaint();
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
