namespace P5XToolGUI
{
    partial class SceneControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            saveBtn = new Button();
            resourceControl1 = new ResourceControl();
            SuspendLayout();
            // 
            // saveBtn
            // 
            saveBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            saveBtn.Location = new Point(3, 523);
            saveBtn.Name = "saveBtn";
            saveBtn.Size = new Size(120, 23);
            saveBtn.TabIndex = 0;
            saveBtn.Text = "Save Bundles";
            saveBtn.UseVisualStyleBackColor = true;
            saveBtn.Click += saveBtn_Click;
            // 
            // resourceControl1
            // 
            resourceControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            resourceControl1.AutoScroll = true;
            resourceControl1.Location = new Point(3, 3);
            resourceControl1.Name = "resourceControl1";
            resourceControl1.Size = new Size(493, 514);
            resourceControl1.TabIndex = 1;
            // 
            // SceneControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(resourceControl1);
            Controls.Add(saveBtn);
            Name = "SceneControl";
            Size = new Size(499, 549);
            ResumeLayout(false);
        }

        #endregion

        private Button saveBtn;
        private ResourceControl resourceControl1;
    }
}
