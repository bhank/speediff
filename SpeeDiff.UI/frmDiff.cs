using System;
using System.Collections.Generic;
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

            rtbLeft.AddPeers(rtbRight, rtbLeftNumbers, rtbRightNumbers);

            foreach (ISynchronizedScrollTextBox box in new[] {rtbLeft, rtbRight, rtbLeftNumbers, rtbRightNumbers})
            {
                box.ReadOnly = true;
                box.Font = new Font(FontFamily.GenericMonospace, 10);
                box.WordWrap = false;
            }

            foreach (ISynchronizedScrollTextBox box in new[] {rtbLeftNumbers, rtbRightNumbers})
            {
                box.ShowScrollBars = false;
                box.BackColor = Color.Cyan;
                box.ReadOnly = true;
                //box.Enabled = false;// Disabling the line-number boxes kills their background color.
                box.Cursor = Cursors.Default;
            }
            // Disabling the line-number boxes kills their background color... but hooking up these methods makes everything freeze!
            //rtbLeftNumbers.Enter += FocusListView;
            //rtbRightNumbers.Enter += FocusListView;

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

        private void GotoChange(bool next)
        {
            var position = rtbLeft.SelectionStart;
            var newPosition = -1;

            foreach (var i in ChangeStartPositions)
            {
                if (!next)
                {
                    if (i < position)
                    {
                        newPosition = i; // for previous only
                    }
                    else
                    {
                        break;
                    }
                }
                else if (i > position)
                {
                        newPosition = i;
                        break;
                }
            }
            if (newPosition > -1)
            {
                rtbLeft.SelectionStart = newPosition;
                rtbLeft.ScrollToCaret();
            }
        }

        private void frmDiff_Load(object sender, EventArgs e)
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                txtPath.Text = Environment.GetCommandLineArgs()[1];
                btnLoad.PerformClick();
            }
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
            ChangeStartPositions = ModelToTextBox(model.OldText, rtbLeft, rtbLeftNumbers);
            ModelToTextBox(model.NewText, rtbRight, rtbRightNumbers);
        }

        private static List<int> ModelToTextBox(DiffPaneModel model, ISynchronizedScrollTextBox textBox, ISynchronizedScrollTextBox lineNumbersTextBox)
        {
            var changeStartPositions = new List<int>();

            foreach (var box in new[] {textBox, lineNumbersTextBox})
            {
                box.StopRepaint();
                box.SavePosition();
                box.Clear();
            }

            var lineNumbersText = new StringBuilder();
            var inChange = false;

            foreach (var line in model.Lines)
            {
                var lineNumber = line.Position.HasValue ? line.Position.ToString() : string.Empty;
                lineNumbersText.AppendLine(lineNumber);
                //AppendText(lineNumbersTextBox, lineNumber + Environment.NewLine, Color.Empty);
                if (line.Type == ChangeType.Unchanged)
                {
                    inChange = false;
                }
                else
                {
                    if (!inChange)
                    {
                        inChange = true;
                        changeStartPositions.Add(textBox.SelectionStart);
                    }
                }

                if (line.Type == ChangeType.Deleted || line.Type == ChangeType.Inserted || line.Type == ChangeType.Unchanged)
                {
                    textBox.AppendText(line.Text + Environment.NewLine, GetPieceColor(line.Type));
                }
                else
                {
                    foreach (var piece in line.SubPieces)
                    {
                        if (piece.Type != ChangeType.Imaginary)
                        {
                            textBox.AppendText(piece.Text, GetPieceColor(piece.Type));
                        }
                    }
                    textBox.AppendText(Environment.NewLine);
                }
            }

            lineNumbersTextBox.Text = lineNumbersText.ToString();

            foreach (var box in new[] {textBox, lineNumbersTextBox})
            {
                box.RestorePosition();
                box.StartRepaint();
            }

            return changeStartPositions;
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

        private List<int> ChangeStartPositions { get; set; }

        // https://social.msdn.microsoft.com/Forums/windows/en-US/2995a8cf-62af-446e-87ab-75045d670942/how-to-assign-a-shortcut-key-to-a-toolstrip-button?forum=winforms
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.Up:
                    btnPreviousChange.PerformClick();
                    break;
                case Keys.Control | Keys.Down:
                    btnNextChange.PerformClick();
                    break;
                case Keys.Alt | Keys.Left:
                    btnDownRevision.PerformClick();
                    break;
                case Keys.Alt | Keys.Right:
                    btnUpRevision.PerformClick();
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
            return true; // Prevent default handling, like for Ctrl-Up and down in textboxes: http://stackoverflow.com/questions/18366787/stop-a-key-from-firing-an-event-in-c-sharp-using-processcmdkey
            
        }

        private void btnPreviousChange_Click(object sender, EventArgs e)
        {
            GotoChange(false);
        }

        private void btnNextChange_Click(object sender, EventArgs e)
        {
            GotoChange(true);
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            //lvwRevisions.Clear();
            lvwRevisions.Items.Clear();

            revisionProvider = RevisionProvider.GetRevisionProvider(txtPath.Text);
            var revisions = await revisionProvider.LoadRevisions();
            lvwRevisions.Items.AddRange(revisions.Select(r => new ListViewItem(new[] {r.RevisionId, r.RevisionTime.ToString(), r.Author, r.Message})).ToArray());
            lvwRevisions.Items[0].Selected = true;
        }

        private void btnUpRevision_Click(object sender, EventArgs e)
        {
            GotoRevision(true);
        }

        private void btnDownRevision_Click(object sender, EventArgs e)
        {
            GotoRevision(false);
        }

        private void GotoRevision(bool next)
        {
            var currentIndex = -1;
            var newIndex = -1;
            if (lvwRevisions.SelectedItems.Count == 1)
            {
                currentIndex = lvwRevisions.SelectedItems[0].Index;
            }
            if (next)
            {
                if (currentIndex < 1)
                {
                    newIndex = 0;
                }
                else if (currentIndex == 0)
                {
                    return;
                }
                else
                {
                    newIndex = currentIndex - 1;
                }
            }
            else
            {
                if (currentIndex == lvwRevisions.Items.Count - 1)
                {
                    return;
                }
                else
                {
                    newIndex = currentIndex + 1;
                }
            }
            lvwRevisions.Items[newIndex].Selected = true;
        }
    }
}
