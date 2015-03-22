using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
        private readonly ISynchronizedScrollTextBox[] allTextBoxes;
        private readonly ISynchronizedScrollTextBox[] numberTextBoxes;
        private readonly ListViewColumnSorter listViewColumnSorter;


        public frmDiff()
        {
            InitializeComponent();

            allTextBoxes = new[]{rtbLeft, rtbRight, rtbLeftNumbers, rtbRightNumbers};
            numberTextBoxes = new[] {rtbLeftNumbers, rtbRightNumbers};
            ChangeStartPositions = new List<int>();

            rtbLeft.AddPeers(rtbRight, rtbLeftNumbers, rtbRightNumbers);

            foreach (ISynchronizedScrollTextBox box in allTextBoxes)
            {
                box.ReadOnly = true;
                box.Font = new Font(FontFamily.GenericMonospace, 10);
                box.WordWrap = false;
            }

            foreach (ISynchronizedScrollTextBox box in numberTextBoxes)
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
            // These column widths are just placeholders which will be overwritten from Settings
            lvwRevisions.Columns.AddRange(
                new[]
                {
                    new ColumnHeader {Text = "Order", Width = 35},
                    new ColumnHeader {Text = "Revision", Width = 70},
                    new ColumnHeader {Text = "Time", Width = 125},
                    new ColumnHeader {Text = "Author", Width = 75},
                    new ColumnHeader {Text = "Message", Width = 650},
                }
                );
            lvwRevisions.ItemSelectionChanged += lvwRevisions_ItemSelectionChanged;
            listViewColumnSorter = new ListViewColumnSorter(lvwRevisions);
            txtPath.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.Enter && args.Modifiers == Keys.None)
                {
                    btnLoad.PerformClick();
                    args.Handled = true;
                }
            };
            Load += frmDiff_Load;
            Closing += frmDiff_Closing;
        }

        void frmDiff_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveWindowPosition();
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
            LoadWindowPosition();

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
                var index = int.Parse(e.Item.Text);
                int leftIndex, rightIndex;
                if (index == lvwRevisions.Items.Count - 1)
                {
                    leftIndex = -1;
                    rightIndex = index;
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
            var left = leftIndex == -1 ? string.Empty : revisionProvider.Revisions[leftIndex].GetContent();
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
                box.DisableScrollSync = true;
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
                box.DisableScrollSync = false;
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
                    return Color.LightGreen;
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
            ChangeStartPositions.Clear();
            lvwRevisions.Items.Clear();
            foreach (var box in allTextBoxes)
            {
                box.Clear();
            }

            txtPath.Text = txtPath.Text.Trim('"');

            try
            {
                revisionProvider = RevisionProvider.GetRevisionProvider(txtPath.Text);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Path is not in a Git or SVN repository.\n\n" + txtPath.Text, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            listViewColumnSorter.ColumnSortOptions[0].Numeric = true; // Original order column
            listViewColumnSorter.ColumnSortOptions[1].Numeric = revisionProvider is SvnRevisionProvider; // Revision number, vs. hash for git

            var revisions = await revisionProvider.LoadRevisions();
            lvwRevisions.Items.AddRange(revisions.Select((r, i) => new ListViewItem(new[] {i.ToString(CultureInfo.InvariantCulture), r.RevisionId, r.RevisionTime.ToString("s"), r.Author, r.Message})).ToArray());
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

        // http://stackoverflow.com/questions/92540/save-and-restore-form-position-and-size
        private void SaveWindowPosition()
        {
            var bounds = WindowState == FormWindowState.Normal ? DesktopBounds : RestoreBounds;
            Properties.Settings.Default.WindowLocation = bounds.Location;
            Properties.Settings.Default.WindowSize = bounds.Size;
            Properties.Settings.Default.WindowMaximized = WindowState == FormWindowState.Maximized;

            Properties.Settings.Default.ListviewColumnWidths = (from ColumnHeader columnHeader in lvwRevisions.Columns select columnHeader.Width).ToArray();

            Properties.Settings.Default.Save();
        }

        private void LoadWindowPosition()
        {
            DesktopBounds = new Rectangle(Properties.Settings.Default.WindowLocation, Properties.Settings.Default.WindowSize);
            WindowState = Properties.Settings.Default.WindowMaximized ? FormWindowState.Maximized : FormWindowState.Normal;

            for (var i = 0; i < lvwRevisions.Columns.Count && i < Properties.Settings.Default.ListviewColumnWidths.Length; i++)
            {
                lvwRevisions.Columns[i].Width = Properties.Settings.Default.ListviewColumnWidths[i];
            }
        }
    }
}
