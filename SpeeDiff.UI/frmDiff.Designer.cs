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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lvwRevisions = new System.Windows.Forms.ListView();
            this.rtbLeftNumbers = new SynchronizedScrollRichTextBox();
            this.rtbLeft = new SynchronizedScrollRichTextBox();
            this.rtbRightNumbers = new SynchronizedScrollRichTextBox();
            this.rtbRight = new SynchronizedScrollRichTextBox();
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
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
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
            this.splitContainer2.Size = new System.Drawing.Size(944, 596);
            this.splitContainer2.SplitterDistance = 413;
            this.splitContainer2.TabIndex = 4;
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
            this.splitContainer1.Size = new System.Drawing.Size(944, 413);
            this.splitContainer1.SplitterDistance = 473;
            this.splitContainer1.TabIndex = 4;
            // 
            // lvwRevisions
            // 
            this.lvwRevisions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwRevisions.Location = new System.Drawing.Point(0, 0);
            this.lvwRevisions.Name = "lvwRevisions";
            this.lvwRevisions.Size = new System.Drawing.Size(944, 179);
            this.lvwRevisions.TabIndex = 3;
            this.lvwRevisions.UseCompatibleStateImageBehavior = false;
            this.lvwRevisions.View = System.Windows.Forms.View.Details;
            this.lvwRevisions.HideSelection = false;
            // 
            // rtbLeftNumbers
            // 
            this.rtbLeftNumbers.Dock = System.Windows.Forms.DockStyle.Left;
            this.rtbLeftNumbers.Location = new System.Drawing.Point(0, 0);
            this.rtbLeftNumbers.Name = "rtbLeftNumbers";
            this.rtbLeftNumbers.Size = new System.Drawing.Size(71, 413);
            this.rtbLeftNumbers.TabIndex = 0;
            this.rtbLeftNumbers.Text = "";
            // 
            // rtbLeft
            // 
            this.rtbLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLeft.Location = new System.Drawing.Point(71, 0);
            this.rtbLeft.Name = "rtbLeft";
            this.rtbLeft.Size = new System.Drawing.Size(402, 413);
            this.rtbLeft.TabIndex = 2;
            this.rtbLeft.Text = "";
            // 
            // rtbRightNumbers
            // 
            this.rtbRightNumbers.Dock = System.Windows.Forms.DockStyle.Left;
            this.rtbRightNumbers.Location = new System.Drawing.Point(0, 0);
            this.rtbRightNumbers.Name = "rtbRightNumbers";
            this.rtbRightNumbers.Size = new System.Drawing.Size(78, 413);
            this.rtbRightNumbers.TabIndex = 0;
            this.rtbRightNumbers.Text = "";
            // 
            // rtbRight
            // 
            this.rtbRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbRight.Location = new System.Drawing.Point(78, 0);
            this.rtbRight.Name = "rtbRight";
            this.rtbRight.Size = new System.Drawing.Size(389, 413);
            this.rtbRight.TabIndex = 3;
            this.rtbRight.Text = "";
            // 
            // frmDiff
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 596);
            this.Controls.Add(this.splitContainer2);
            this.Name = "frmDiff";
            this.Text = "frmDiff";
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView lvwRevisions;
        private SynchronizedScrollRichTextBox rtbLeft;
        private SynchronizedScrollRichTextBox rtbLeftNumbers;
        private SynchronizedScrollRichTextBox rtbRight;
        private SynchronizedScrollRichTextBox rtbRightNumbers;

    }
}