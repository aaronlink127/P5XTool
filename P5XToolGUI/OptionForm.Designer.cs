namespace P5XToolGUI
{
    partial class OptionForm
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
            autoPathText = new TextBox();
            autoLoadCheck = new CheckBox();
            saveBtn = new Button();
            label1 = new Label();
            dirSelect = new Button();
            closeBtn = new Button();
            SuspendLayout();
            // 
            // autoPathText
            // 
            autoPathText.AutoCompleteMode = AutoCompleteMode.Suggest;
            autoPathText.AutoCompleteSource = AutoCompleteSource.CustomSource;
            autoPathText.Location = new Point(12, 27);
            autoPathText.Name = "autoPathText";
            autoPathText.Size = new Size(188, 23);
            autoPathText.TabIndex = 1;
            autoPathText.TextChanged += autoPathText_TextChanged;
            // 
            // autoLoadCheck
            // 
            autoLoadCheck.AutoSize = true;
            autoLoadCheck.Location = new Point(187, 11);
            autoLoadCheck.Name = "autoLoadCheck";
            autoLoadCheck.Size = new Size(15, 14);
            autoLoadCheck.TabIndex = 0;
            autoLoadCheck.UseVisualStyleBackColor = true;
            autoLoadCheck.CheckedChanged += autoLoadCheck_CheckedChanged;
            // 
            // saveBtn
            // 
            saveBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveBtn.Location = new Point(291, 352);
            saveBtn.Name = "saveBtn";
            saveBtn.Size = new Size(75, 23);
            saveBtn.TabIndex = 5;
            saveBtn.Text = "Save";
            saveBtn.UseVisualStyleBackColor = true;
            saveBtn.Click += saveBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(89, 15);
            label1.TabIndex = 3;
            label1.Text = "Auto Load Path";
            // 
            // dirSelect
            // 
            dirSelect.Location = new Point(125, 56);
            dirSelect.Name = "dirSelect";
            dirSelect.Size = new Size(75, 23);
            dirSelect.TabIndex = 2;
            dirSelect.Text = "Select...";
            dirSelect.UseVisualStyleBackColor = true;
            dirSelect.Click += dirSelect_Click;
            // 
            // closeBtn
            // 
            closeBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            closeBtn.Location = new Point(372, 352);
            closeBtn.Name = "closeBtn";
            closeBtn.Size = new Size(75, 23);
            closeBtn.TabIndex = 6;
            closeBtn.Text = "Close";
            closeBtn.UseVisualStyleBackColor = true;
            closeBtn.Click += closeBtn_Click;
            // 
            // OptionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(459, 387);
            Controls.Add(closeBtn);
            Controls.Add(dirSelect);
            Controls.Add(label1);
            Controls.Add(autoLoadCheck);
            Controls.Add(autoPathText);
            Controls.Add(saveBtn);
            Name = "OptionForm";
            Text = "OptionForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox autoPathText;
        private CheckBox autoLoadCheck;
        private Button saveBtn;
        private Label label1;
        private Button dirSelect;
        private Button closeBtn;
    }
}