using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoyneSolutions.SpeeDiff.Properties;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace CoyneSolutions.SpeeDiff
{
    public partial class frmDiff : Form
    {
        private string formTitle;
        private IRevisionProvider revisionProvider;
        private readonly ISynchronizedScrollTextBox[] allTextBoxes;
        private readonly ISynchronizedScrollTextBox[] numberTextBoxes;
        private readonly ListViewColumnSorter listViewColumnSorter;
        private string loadedFileName;

        public frmDiff()
        {
            formTitle = "speediff " + GetVersion();
            InitializeComponent();

            Text = formTitle;

            allTextBoxes = new[]{rtbLeft, rtbRight, rtbLeftNumbers, rtbRightNumbers};
            numberTextBoxes = new[] {rtbLeftNumbers, rtbRightNumbers};
            ChangeStartPositions = new List<int>();

            rtbLeft.LanguageOption = rtbRight.LanguageOption = RichTextBoxLanguageOptions.DualFont; // Prevent CJK characters from switching the line to SimSun font and making it taller, so it doesn't line up with the line numbers. http://stackoverflow.com/questions/10118117/pasting-cjk-characters-to-a-richtextbox-adds-an-unwanted-second-font/12168388#12168388

            rtbLeft.AddVerticalScrollPeers(rtbRight, rtbLeftNumbers, rtbRightNumbers);
            rtbLeft.AddHorizontalScrollPeers(rtbRight);

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
            lvwRevisions.ShowItemToolTips = true;
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
            lvwRevisions.KeyUp += lvwRevisions_KeyUp;
            listViewColumnSorter = new ListViewColumnSorter(lvwRevisions);

            var skipNextSelectionChangeCommitted = false;
            cbxPath.ComboBox.SelectionChangeCommitted += (sender, args) =>
            {
                if (skipNextSelectionChangeCommitted)
                {
                    skipNextSelectionChangeCommitted = false;
                }
                else if (cbxPath.DroppedDown)
                {
                    ComboBoxItemSelected();
                }
            };
            cbxPath.ComboBox.KeyDown += (sender, args) =>
            {
                if (args.KeyCode == Keys.Enter && args.Modifiers == Keys.None)
                {
                    args.Handled = true;
                    args.SuppressKeyPress = true; // Stop the ding
                    if (cbxPath.DroppedDown)
                    {
                        // Prevent ComboBoxItemSelected from running again in SelectionChangeCommitted
                        skipNextSelectionChangeCommitted = true;
                    }
                    ComboBoxItemSelected();
                }
            };
            Load += frmDiff_Load;
            Closing += frmDiff_Closing;

            SetUpContextMenu();
        }

        private string GetVersion()
        {
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            return string.Format("{0}.{1}", version.FileMajorPart, version.FileMinorPart);
        }

        void lvwRevisions_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Apps && lvwRevisions.SelectedItems.Count == 1)
            {
                contextMenuStrip.Show(lvwRevisions, lvwRevisions.SelectedItems[0].Position);
            }
        }

        private void SetUpContextMenu()
        {
            const string settingPrefix = "ExternalApp";
            var i = 1;
            while (true)
            {
                var name = ConfigurationManager.AppSettings[settingPrefix + i + "Name"];
                var type = ConfigurationManager.AppSettings[settingPrefix + i + "Type"];
                var program = ConfigurationManager.AppSettings[settingPrefix + i + "Program"];
                var parameters = ConfigurationManager.AppSettings[settingPrefix+ i + "Parameters"];
                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(program))
                {
                    contextMenuStrip.Items.Add(new ToolStripMenuItem(name, null, (sender, args) => RunExternalApp(program, parameters)) {Tag = type});
                }
                else
                {
                    break;
                }
                i++;
            }
            if (TortoiseSvnHelper.Exists)
            {
                contextMenuStrip.Items.Add(new ToolStripMenuItem("View diff in TortoiseMerge", null, (sender, args) => RunExternalApp(TortoiseSvnHelper.TortoiseMergeProgram, TortoiseSvnHelper.TortoiseMergeParameters)));
                contextMenuStrip.Items.Add(new ToolStripSeparator {Tag = "svn"});
                contextMenuStrip.Items.Add(new ToolStripMenuItem("TortoiseSVN Diff", null, (sender, args) => RunExternalApp(TortoiseSvnHelper.TortoiseProcProgram, TortoiseSvnHelper.DiffParameters)) { Tag="svn" });
                contextMenuStrip.Items.Add(new ToolStripMenuItem("TortoiseSVN Log", null, (sender, args) => RunExternalApp(TortoiseSvnHelper.TortoiseProcProgram, TortoiseSvnHelper.LogParameters)) { Tag="svn" });
            }
        }

        private async void RunExternalApp(string program, string parameters)
        {
            int leftIndex, rightIndex;
            if (GetSelectedRevisionIndexes(out leftIndex, out rightIndex))
            {
                var leftRevision = leftIndex == -1 ? null : revisionProvider.Revisions[leftIndex];
                var rightRevision = revisionProvider.Revisions[rightIndex];

                string leftFile = null, rightFile = null;
                if (parameters == null)
                {
                    parameters = string.Empty;
                }
                if (parameters.Contains("{left}") || parameters.Contains("{right}"))
                {
                    leftFile = Path.GetTempFileName();
                    rightFile = Path.GetTempFileName();

                    if(leftRevision != null)
                    {
                        File.WriteAllText(leftFile, leftRevision.GetContent());
                    }
                    File.WriteAllText(rightFile, rightRevision.GetContent());

                    parameters = parameters
                        .Replace("{left}", leftFile)
                        .Replace("{right}", rightFile);
                }

                var revisionPrefix = revisionProvider is SvnRevisionProvider ? "r" : string.Empty;
                var title = string.Format("{0} {1}", Path.GetFileName(loadedFileName), revisionPrefix);
                var leftTitle = " ";
                if (leftRevision != null)
                {
                    leftTitle = title + leftRevision.RevisionId + " " + leftRevision.RevisionTime;
                }
                var rightTitle = title + rightRevision.RevisionId + " " + rightRevision.RevisionTime;

                parameters = parameters
                    .Replace("{file}", loadedFileName)
                    .Replace("{leftrevisionid}", leftRevision == null ? string.Empty : leftRevision.RevisionId)
                    .Replace("{rightrevisionid}", rightRevision.RevisionId)
                    .Replace("{lefttitle}", leftTitle)
                    .Replace("{righttitle}", rightTitle);

                await Task.Run(() =>
                {
                    var process = Process.Start(program, parameters);
                    process.WaitForExit();
                    if(File.Exists(leftFile)) File.Delete(leftFile);
                    if(File.Exists(rightFile)) File.Delete(rightFile);
                });
            }
        }

        private void ComboBoxItemSelected()
        {
            if (cbxPath.SelectedItem == null)
            {
                LoadFile(cbxPath.Text);
            }
            else
            {
                var comboBoxCommand = cbxPath.SelectedItem as ComboBoxCommand;
                if (comboBoxCommand != null)
                {
                    //cbxPath.SelectedIndex = -1;
                    comboBoxCommand.Action();
                    // Hmm. Whether I use SelectionChangeCommitted or SelectedIndexChanged, the change will cause the combobox's text to be changed after the action runs, changing the text away from the filename.
                    // What if I don't change it, and rely on the Action to change it? No good.. it will still get changed to this afterward.
                }
                else
                {
                    LoadFile(cbxPath.SelectedItem.ToString());
                }
            }
        }

        void frmDiff_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var settings = Properties.Settings.Default;
            SaveWindowPosition(settings);
            SaveMostRecentlyUsedFiles(settings);
            settings.Save();
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
                
                var lineNumber = rtbLeft.GetLineFromCharIndex(newPosition);
                var rightPosition = rtbRight.GetFirstCharIndexFromLine(lineNumber);
                rtbRight.SelectionStart = rightPosition;
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void frmDiff_Load(object sender, EventArgs e)
        {
            var settings = Properties.Settings.Default;
            LoadWindowPosition(settings);
            LoadMostRecentlyUsedFiles(settings);
            SetDropdownWidth();
            AddComboBoxCommands();
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                LoadFile(Environment.GetCommandLineArgs()[1]);
            }
        }

        private void SetDropdownWidth()
        {
            var maxWidth = 0;
            foreach (var item in cbxPath.Items)
            {
                var width = TextRenderer.MeasureText(item.ToString(), cbxPath.Font).Width;
                if (width > maxWidth)
                {
                    maxWidth = width;
                }
            }
            if (maxWidth > 0)
            {
                cbxPath.DropDownWidth = maxWidth;
            }
        }

        private async void lvwRevisions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int leftIndex, rightIndex;
            if(GetSelectedRevisionIndexes(out leftIndex, out rightIndex))
            {
                await Task.Run(() => ShowChanges(leftIndex, rightIndex));
                Debug.WriteLine("Done awaiting.");
            }
        }

        private bool GetSelectedRevisionIndexes(out int leftIndex, out int rightIndex)
        {
            leftIndex = rightIndex = -1;
            if (lvwRevisions.SelectedItems.Count == 1)
            {
                var index = int.Parse(lvwRevisions.SelectedItems[0].Text);
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
                return true;
            }
            return false;
        }

        private void ShowChanges(int leftIndex, int rightIndex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int,int>(ShowChanges), leftIndex, rightIndex);
                return;
            }
            var left = leftIndex == -1 ? string.Empty : revisionProvider.Revisions[leftIndex].GetContent();
            var right = revisionProvider.Revisions[rightIndex].GetContent();
            var builder = new SideBySideDiffBuilder(new Differ());
            var model = builder.BuildDiffModel(left, right);
            ChangeStartPositions = ModelToTextBox(model.OldText, rtbLeft, rtbLeftNumbers);
            ModelToTextBox(model.NewText, rtbRight, rtbRightNumbers);
            lblChanges.Text = ChangeStartPositions.Count + " change" + (ChangeStartPositions.Count == 1 ? string.Empty : "s");
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
                var isLongLine = line.Text != null && line.Text.Length > 3500; // TODO: use a number based on the current font size
                var oldPosition = -1;
                if (isLongLine)
                {
                    oldPosition = textBox.GetLastCharTopPosition();
                }
                var lineNumber = line.Position.HasValue ? line.Position.ToString() : string.Empty;
                lineNumbersText.AppendLine(lineNumber);
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

                if (isLongLine)
                {
                    // If the textbox wrapped (as it does at a certain width, after about 3511 characters in this font), add space in the line numbers column.
                    var newPosition = textBox.GetLastCharTopPosition();
                    var fontHeight = textBox.Font.Height;
                    for (var i = oldPosition + fontHeight; i < newPosition; i += fontHeight)
                    {
                        lineNumbersText.AppendLine();
                    }
                }
            }

            lineNumbersTextBox.Text = lineNumbersText.ToString();

            foreach (var box in new[] {textBox, lineNumbersTextBox})
            {
                box.CalculateScrollOffset();
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
                case Keys.Escape:
                    if (Config.ExitOnEscapeKey)
                    {
                        Close();
                    }
                    break;
                case Keys.Control | Keys.Cancel:
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                    else
                    {
                        goto default;
                    }
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

        private async void LoadFile(string filename)
        {
            ChangeStartPositions.Clear();
            lvwRevisions.Items.Clear();
            foreach (var box in allTextBoxes)
            {
                box.Clear();
            }
            Text = formTitle;
            lblChanges.Text = string.Empty;
            loadedFileName = null;

            if (string.IsNullOrEmpty(filename))
            {
                BeginInvoke(new Action(() => cbxPath.Text = string.Empty)); // Need to invoke in case the combo box selection is changing, since in that case any text I set here will get overwritten.
                return;
            }

            try
            {
                revisionProvider = RevisionProvider.GetRevisionProvider(filename);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Path is not in a Git or SVN repository.\n\n" + filename, formTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var isSvn = revisionProvider is SvnRevisionProvider;
            var isGit = revisionProvider is GitRevisionProvider;

            listViewColumnSorter.ColumnSortOptions[0].Numeric = true; // Original order column
            listViewColumnSorter.ColumnSortOptions[1].Numeric = isSvn; // Revision number, vs. hash for git

            foreach (ToolStripItem item in contextMenuStrip.Items)
            {
                var tag = item.Tag as string;
                if (string.IsNullOrWhiteSpace(tag))
                {
                    item.Visible = true;
                }
                else
                {
                    if (isSvn)
                    {
                        item.Visible = tag.Contains("svn");
                    }
                    if (isGit)
                    {
                        item.Visible = tag.Contains("git");
                    }
                }
            }

            Text = string.Format("{0} - {1}", filename, formTitle);

            var revisions = await revisionProvider.LoadRevisions();
            lvwRevisions.Items.AddRange(revisions.Select((r, i) => new ListViewItem(new[]
            {
                i.ToString(CultureInfo.InvariantCulture),
                r.RevisionId,
                r.RevisionTime.ToString("s"),
                r.Author,
                r.Message,
            })
            {
                ToolTipText = r.ToString(),
            }).ToArray());
            lvwRevisions.Items[0].Selected = true;
            AddMostRecentlyUsedFilename(filename);
            loadedFileName = filename;
            rtbRight.Select();
        }

        private void AddMostRecentlyUsedFilename(string filename)
        {
            cbxPath.Items.Remove(filename);
            cbxPath.Items.Insert(0, filename);
            cbxPath.Text = filename;
            cbxPath.SelectAll();
            SetDropdownWidth();
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
                if (currentIndex < 0)
                {
                    newIndex = 0;
                }
                else if (currentIndex == 0)
                {
                    System.Media.SystemSounds.Beep.Play();
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
                    System.Media.SystemSounds.Beep.Play();
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
        private void SaveWindowPosition(Settings settings)
        {
            var bounds = WindowState == FormWindowState.Normal ? DesktopBounds : RestoreBounds;
            settings.WindowLocation = bounds.Location;
            settings.WindowSize = bounds.Size;
            settings.WindowMaximized = WindowState == FormWindowState.Maximized;
            settings.ListviewColumnWidths = (from ColumnHeader columnHeader in lvwRevisions.Columns select columnHeader.Width).ToArray();
        }

        private void LoadWindowPosition(Settings settings)
        {
            DesktopBounds = new Rectangle(settings.WindowLocation, settings.WindowSize);
            WindowState = settings.WindowMaximized ? FormWindowState.Maximized : FormWindowState.Normal;

            for (var i = 0; i < lvwRevisions.Columns.Count && i < settings.ListviewColumnWidths.Length; i++)
            {
                lvwRevisions.Columns[i].Width = settings.ListviewColumnWidths[i];
            }
        }

        private void LoadMostRecentlyUsedFiles(Settings settings)
        {
            var mostRecentlyUsedFiles = settings.MostRecentlyUsedFiles.Cast<object>().ToArray();
            cbxPath.Items.AddRange(mostRecentlyUsedFiles);
        }

        private void AddComboBoxCommands()
        {
            cbxPath.Items.Add(new ComboBoxCommand("", () => {}));
            cbxPath.Items.Add(new ComboBoxCommand("Browse...", Browse));
            if (TortoiseSvnHelper.Exists)
            {
                cbxPath.Items.Add(new ComboBoxCommand("TortoiseSVN Repo Browser...", RepoBrowse));
            }
        }

        private void SaveMostRecentlyUsedFiles(Settings settings)
        {
            settings.MostRecentlyUsedFiles.Clear();
            settings.MostRecentlyUsedFiles.AddRange(cbxPath.Items.OfType<string>().Where(s => !string.IsNullOrWhiteSpace(s)).Take(Config.MaxMostRecentlyUsedFiles).ToArray());
        }

        private void Browse()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadFile(dialog.FileName);
            }
            else
            {
                LoadFile(string.Empty);
            }
        }

        private void RepoBrowse()
        {
            var url = TortoiseSvnHelper.GetUrlFromRepoBrowser();
            LoadFile(url);
        }

        // http://stackoverflow.com/questions/13437889/showing-a-context-menu-for-an-item-in-a-listview
        private void lvwRevisions_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (lvwRevisions.FocusedItem.Bounds.Contains(e.Location))
                {
                    if (contextMenuStrip.Items.Count > 0)
                    {
                        contextMenuStrip.Show(lvwRevisions, e.Location);
                    }
                }
            }
        }
    }
}
