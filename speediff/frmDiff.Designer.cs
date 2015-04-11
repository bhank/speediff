using System.Drawing;
using System.Windows.Forms;

namespace CoyneSolutions.SpeeDiff
{
    partial class frmDiff
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDiff));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUpRevision = new System.Windows.Forms.ToolStripButton();
            this.btnDownRevision = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPreviousChange = new System.Windows.Forms.ToolStripButton();
            this.btnNextChange = new System.Windows.Forms.ToolStripButton();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rtbLeft = new SynchronizedScrollRichTextBox();
            this.rtbLeftNumbers = new SynchronizedScrollRichTextBox();
            this.rtbRight = new SynchronizedScrollRichTextBox();
            this.rtbRightNumbers = new SynchronizedScrollRichTextBox();
            this.lvwRevisions = new ListViewWithSubitemTooltips();
            this.cbxPath = new System.Windows.Forms.ToolStripComboBox();
            this.lblChanges = new System.Windows.Forms.ToolStripLabel();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnFindText = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.cbxPath,
            this.toolStripSeparator1,
            this.btnFindText,
            this.toolStripSeparator3,
            this.btnDownRevision,
            this.btnUpRevision,
            this.toolStripSeparator2,
            this.btnPreviousChange,
            this.btnNextChange,
            this.lblChanges});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(944, 25);
            this.toolStrip.TabStop = true;
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(28, 22);
            this.toolStripLabel1.Text = "&File:";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnUpRevision
            // 
            this.btnUpRevision.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUpRevision.Image = ((System.Drawing.Image)(resources.GetObject("btnUpRevision.Image")));
            this.btnUpRevision.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUpRevision.Name = "btnUpRevision";
            this.btnUpRevision.Size = new System.Drawing.Size(23, 22);
            this.btnUpRevision.Text = "Go to Next Revision (Alt-Right)";
            this.btnUpRevision.Click += new System.EventHandler(this.btnUpRevision_Click);
            // 
            // btnDownRevision
            // 
            this.btnDownRevision.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDownRevision.Image = ((System.Drawing.Image)(resources.GetObject("btnDownRevision.Image")));
            this.btnDownRevision.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDownRevision.Name = "btnDownRevision";
            this.btnDownRevision.Size = new System.Drawing.Size(23, 22);
            this.btnDownRevision.Text = "Go to Previous Revision (Alt-Left)";
            this.btnDownRevision.Click += new System.EventHandler(this.btnDownRevision_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnPreviousChange
            // 
            this.btnPreviousChange.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPreviousChange.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousChange.Image")));
            this.btnPreviousChange.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPreviousChange.Name = "btnPreviousChange";
            this.btnPreviousChange.Size = new System.Drawing.Size(23, 22);
            this.btnPreviousChange.Text = "Go to Previous Change (Ctrl-Up)";
            this.btnPreviousChange.Click += new System.EventHandler(this.btnPreviousChange_Click);
            // 
            // btnNextChange
            // 
            this.btnNextChange.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNextChange.Image = ((System.Drawing.Image)(resources.GetObject("btnNextChange.Image")));
            this.btnNextChange.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNextChange.Name = "btnNextChange";
            this.btnNextChange.Size = new System.Drawing.Size(23, 22);
            this.btnNextChange.Text = "Go to Next Change (Ctrl-Down)";
            this.btnNextChange.Click += new System.EventHandler(this.btnNextChange_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 25);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lvwRevisions);
            this.splitContainer2.Size = new System.Drawing.Size(944, 571);
            this.splitContainer2.SplitterDistance = 395;
            this.splitContainer2.TabIndex = 1;
            this.splitContainer2.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rtbLeft);
            this.splitContainer1.Panel1.Controls.Add(this.rtbLeftNumbers);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rtbRight);
            this.splitContainer1.Panel2.Controls.Add(this.rtbRightNumbers);
            this.splitContainer1.Size = new System.Drawing.Size(944, 395);
            this.splitContainer1.SplitterDistance = 473;
            this.splitContainer1.TabIndex = 2;
            this.splitContainer1.TabStop = false;
            // 
            // rtbLeft
            // 
            this.rtbLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLeft.Location = new System.Drawing.Point(71, 0);
            this.rtbLeft.Name = "rtbLeft";
            this.rtbLeft.Size = new System.Drawing.Size(402, 395);
            this.rtbLeft.TabIndex = 3;
            this.rtbLeft.Text = "";
            this.rtbLeft.HideSelection = false;
            // 
            // rtbLeftNumbers
            // 
            this.rtbLeftNumbers.Dock = System.Windows.Forms.DockStyle.Left;
            this.rtbLeftNumbers.Location = new System.Drawing.Point(0, 0);
            this.rtbLeftNumbers.Name = "rtbLeftNumbers";
            this.rtbLeftNumbers.Size = new System.Drawing.Size(50, 395);
            this.rtbLeftNumbers.TabStop = false;
            this.rtbLeftNumbers.Text = "";
            this.rtbRight.HideSelection = false;
            // 
            // rtbRight
            // 
            this.rtbRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbRight.Location = new System.Drawing.Point(78, 0);
            this.rtbRight.Name = "rtbRight";
            this.rtbRight.Size = new System.Drawing.Size(389, 395);
            this.rtbRight.TabIndex = 4;
            this.rtbRight.Text = "";
            // 
            // rtbRightNumbers
            // 
            this.rtbRightNumbers.Dock = System.Windows.Forms.DockStyle.Left;
            this.rtbRightNumbers.Location = new System.Drawing.Point(0, 0);
            this.rtbRightNumbers.Name = "rtbRightNumbers";
            this.rtbRightNumbers.Size = new System.Drawing.Size(50, 395);
            this.rtbRightNumbers.TabStop = false;
            this.rtbRightNumbers.Text = "";
            // 
            // lvwRevisions
            // 
            this.lvwRevisions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwRevisions.HideSelection = false;
            this.lvwRevisions.Location = new System.Drawing.Point(0, 0);
            this.lvwRevisions.MultiSelect = false;
            this.lvwRevisions.Name = "lvwRevisions";
            this.lvwRevisions.Size = new System.Drawing.Size(944, 172);
            this.lvwRevisions.TabIndex = 5;
            this.lvwRevisions.UseCompatibleStateImageBehavior = false;
            this.lvwRevisions.View = System.Windows.Forms.View.Details;
            this.lvwRevisions.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvwRevisions_MouseClick);
            // 
            // cbxPath
            // 
            this.cbxPath.Name = "cbxPath";
            this.cbxPath.Size = new System.Drawing.Size(300, 25);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(153, 26);
            // 
            // lblChanges
            // 
            this.lblChanges.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblChanges.Name = "lblChanges";
            this.lblChanges.Size = new System.Drawing.Size(0, 22);
            // 
            // btnFindText
            // 
            this.btnFindText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFindText.Image = ((System.Drawing.Image)(resources.GetObject("btnFindText.Image")));
            this.btnFindText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFindText.Name = "btnFindText";
            this.btnFindText.Size = new System.Drawing.Size(23, 22);
            this.btnFindText.Text = "Find Text (Ctrl-F)";
            this.btnFindText.Click += new System.EventHandler(this.btnFindText_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // frmDiff
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 596);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDiff";
            this.Text = "speediff";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ListViewWithSubitemTooltips lvwRevisions;
        private SynchronizedScrollRichTextBox rtbLeft;
        private SynchronizedScrollRichTextBox rtbLeftNumbers;
        private SynchronizedScrollRichTextBox rtbRight;
        private SynchronizedScrollRichTextBox rtbRightNumbers;
        private ToolStrip toolStrip;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btnUpRevision;
        private ToolStripButton btnDownRevision;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btnPreviousChange;
        private ToolStripButton btnNextChange;
        private ToolStripLabel toolStripLabel1;
        private ToolStripComboBox cbxPath;
        private ToolStripLabel lblChanges;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripButton btnFindText;
        private ToolStripSeparator toolStripSeparator3;

    }
}