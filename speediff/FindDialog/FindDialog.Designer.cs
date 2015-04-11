namespace CoyneSolutions.SpeeDiff
{
    partial class FindDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtFindString = new System.Windows.Forms.TextBox();
            this.rbLeft = new System.Windows.Forms.RadioButton();
            this.chkUseRegularExpressions = new System.Windows.Forms.CheckBox();
            this.rbRight = new System.Windows.Forms.RadioButton();
            this.rbBoth = new System.Windows.Forms.RadioButton();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCaseSensitive = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Search for:";
            // 
            // txtFindString
            // 
            this.txtFindString.Location = new System.Drawing.Point(77, 6);
            this.txtFindString.Name = "txtFindString";
            this.txtFindString.Size = new System.Drawing.Size(190, 20);
            this.txtFindString.TabIndex = 1;
            // 
            // rbLeft
            // 
            this.rbLeft.AutoSize = true;
            this.rbLeft.Location = new System.Drawing.Point(12, 55);
            this.rbLeft.Name = "rbLeft";
            this.rbLeft.Size = new System.Drawing.Size(80, 17);
            this.rbLeft.TabIndex = 4;
            this.rbLeft.Text = "Search &Left";
            this.rbLeft.UseVisualStyleBackColor = true;
            // 
            // chkUseRegularExpressions
            // 
            this.chkUseRegularExpressions.AutoSize = true;
            this.chkUseRegularExpressions.Location = new System.Drawing.Point(12, 32);
            this.chkUseRegularExpressions.Name = "chkUseRegularExpressions";
            this.chkUseRegularExpressions.Size = new System.Drawing.Size(138, 17);
            this.chkUseRegularExpressions.TabIndex = 2;
            this.chkUseRegularExpressions.Text = "Use regular e&xpressions";
            this.chkUseRegularExpressions.UseVisualStyleBackColor = true;
            // 
            // rbRight
            // 
            this.rbRight.AutoSize = true;
            this.rbRight.Location = new System.Drawing.Point(98, 55);
            this.rbRight.Name = "rbRight";
            this.rbRight.Size = new System.Drawing.Size(87, 17);
            this.rbRight.TabIndex = 5;
            this.rbRight.Text = "Search &Right";
            this.rbRight.UseVisualStyleBackColor = true;
            // 
            // rbBoth
            // 
            this.rbBoth.AutoSize = true;
            this.rbBoth.Checked = true;
            this.rbBoth.Location = new System.Drawing.Point(191, 55);
            this.rbBoth.Name = "rbBoth";
            this.rbBoth.Size = new System.Drawing.Size(84, 17);
            this.rbBoth.TabIndex = 6;
            this.rbBoth.TabStop = true;
            this.rbBoth.Text = "Search &Both";
            this.rbBoth.UseVisualStyleBackColor = true;
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(273, 6);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(75, 23);
            this.btnFind.TabIndex = 7;
            this.btnFind.Text = "&Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(273, 35);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkCaseSensitive
            // 
            this.chkCaseSensitive.AutoSize = true;
            this.chkCaseSensitive.Location = new System.Drawing.Point(150, 33);
            this.chkCaseSensitive.Name = "chkCaseSensitive";
            this.chkCaseSensitive.Size = new System.Drawing.Size(82, 17);
            this.chkCaseSensitive.TabIndex = 3;
            this.chkCaseSensitive.Text = "Match &case";
            this.chkCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // FindDialog
            // 
            this.AcceptButton = this.btnFind;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(356, 83);
            this.Controls.Add(this.chkCaseSensitive);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.rbBoth);
            this.Controls.Add(this.rbRight);
            this.Controls.Add(this.chkUseRegularExpressions);
            this.Controls.Add(this.rbLeft);
            this.Controls.Add(this.txtFindString);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindDialog";
            this.Text = "Find Text";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFindString;
        private System.Windows.Forms.RadioButton rbLeft;
        private System.Windows.Forms.CheckBox chkUseRegularExpressions;
        private System.Windows.Forms.RadioButton rbRight;
        private System.Windows.Forms.RadioButton rbBoth;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkCaseSensitive;
    }
}