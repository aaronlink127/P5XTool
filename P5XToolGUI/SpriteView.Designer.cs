namespace P5XToolGUI
{
    partial class SpriteView
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
            centerImgView1 = new CenterImgView();
            metaLabel = new Label();
            SuspendLayout();
            // 
            // centerImgView1
            // 
            centerImgView1.Dock = DockStyle.Fill;
            centerImgView1.Location = new Point(0, 0);
            centerImgView1.Name = "centerImgView1";
            centerImgView1.Size = new Size(150, 150);
            centerImgView1.TabIndex = 0;
            // 
            // metaLabel
            // 
            metaLabel.AutoSize = true;
            metaLabel.Location = new Point(3, 3);
            metaLabel.Name = "metaLabel";
            metaLabel.Size = new Size(28, 15);
            metaLabel.TabIndex = 1;
            metaLabel.Text = "Text";
            // 
            // SpriteView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(metaLabel);
            Controls.Add(centerImgView1);
            Name = "SpriteView";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CenterImgView centerImgView1;
        private Label metaLabel;
    }
}
