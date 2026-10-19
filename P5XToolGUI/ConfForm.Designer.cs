namespace P5XToolGUI
{
    partial class ConfForm
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
            okButton = new Button();
            cancelButton = new Button();
            saveFileDialog1 = new SaveFileDialog();
            button1 = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            dataGridView1 = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // confSel
            // 
            confSel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            confSel.FormattingEnabled = true;
            confSel.Location = new Point(12, 12);
            confSel.Name = "confSel";
            confSel.Size = new Size(1085, 23);
            confSel.TabIndex = 0;
            confSel.SelectedIndexChanged += confSel_SelectedIndexChanged;
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.Location = new Point(941, 521);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 1;
            okButton.Text = "Save As";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Location = new Point(1022, 521);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "Close";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Location = new Point(860, 521);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 3;
            button1.Text = "Save All";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 41);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(1085, 474);
            dataGridView1.TabIndex = 6;
            dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;
            // 
            // ConfForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1109, 556);
            Controls.Add(dataGridView1);
            Controls.Add(button1);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(confSel);
            Name = "ConfForm";
            Text = "Config Export";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ComboBox confSel;
        private Button okButton;
        private Button cancelButton;
        private SaveFileDialog saveFileDialog1;
        private Button button1;
        private FolderBrowserDialog folderBrowserDialog1;
        private DataGridView dataGridView1;
    }
}