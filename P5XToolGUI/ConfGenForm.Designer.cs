namespace P5XToolGUI
{
    partial class ConfGenForm
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
            confSel = new ComboBox();
            textBox1 = new TextBox();
            explicitColumnCb = new CheckBox();
            titleCaseCb = new CheckBox();
            SuspendLayout();
            // 
            // confSel
            // 
            confSel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            confSel.FormattingEnabled = true;
            confSel.Location = new Point(12, 12);
            confSel.Name = "confSel";
            confSel.Size = new Size(778, 23);
            confSel.TabIndex = 0;
            confSel.SelectedIndexChanged += confSel_SelectedIndexChanged;
            // 
            // textBox1
            // 
            textBox1.AcceptsReturn = true;
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Location = new Point(12, 66);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(778, 380);
            textBox1.TabIndex = 1;
            // 
            // explicitColumnCb
            // 
            explicitColumnCb.AutoSize = true;
            explicitColumnCb.Checked = true;
            explicitColumnCb.CheckState = CheckState.Checked;
            explicitColumnCb.Location = new Point(12, 41);
            explicitColumnCb.Name = "explicitColumnCb";
            explicitColumnCb.Size = new Size(150, 19);
            explicitColumnCb.TabIndex = 2;
            explicitColumnCb.Text = "Specify Column Names";
            explicitColumnCb.UseVisualStyleBackColor = true;
            explicitColumnCb.CheckedChanged += explicitColumnCb_CheckedChanged;
            // 
            // titleCaseCb
            // 
            titleCaseCb.AutoSize = true;
            titleCaseCb.Checked = true;
            titleCaseCb.CheckState = CheckState.Checked;
            titleCaseCb.Location = new Point(168, 41);
            titleCaseCb.Name = "titleCaseCb";
            titleCaseCb.Size = new Size(129, 19);
            titleCaseCb.TabIndex = 3;
            titleCaseCb.Text = "Title Case Members";
            titleCaseCb.UseVisualStyleBackColor = true;
            titleCaseCb.CheckedChanged += titleCaseCb_CheckedChanged;
            // 
            // ConfGenForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(802, 458);
            Controls.Add(titleCaseCb);
            Controls.Add(explicitColumnCb);
            Controls.Add(textBox1);
            Controls.Add(confSel);
            Name = "ConfGenForm";
            Text = "Config Class Generator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox confSel;
        private TextBox textBox1;
        private CheckBox explicitColumnCb;
        private CheckBox titleCaseCb;
    }
}