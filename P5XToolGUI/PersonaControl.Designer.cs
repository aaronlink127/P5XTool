namespace P5XToolGUI
{
    partial class PersonaControl
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
            tabControl1 = new TabControl();
            modelTab = new TabPage();
            resourceTab = new TabPage();
            resourceControl1 = new ResourceControl();
            tabControl1.SuspendLayout();
            resourceTab.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(modelTab);
            tabControl1.Controls.Add(resourceTab);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(458, 351);
            tabControl1.TabIndex = 0;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // modelTab
            // 
            modelTab.Location = new Point(4, 24);
            modelTab.Name = "modelTab";
            modelTab.Padding = new Padding(3);
            modelTab.Size = new Size(450, 323);
            modelTab.TabIndex = 0;
            modelTab.Text = "Model";
            modelTab.UseVisualStyleBackColor = true;
            // 
            // resourceTab
            // 
            resourceTab.Controls.Add(resourceControl1);
            resourceTab.Location = new Point(4, 24);
            resourceTab.Name = "resourceTab";
            resourceTab.Padding = new Padding(3);
            resourceTab.Size = new Size(450, 323);
            resourceTab.TabIndex = 1;
            resourceTab.Text = "Resources";
            resourceTab.UseVisualStyleBackColor = true;
            // 
            // resourceControl1
            // 
            resourceControl1.Dock = DockStyle.Fill;
            resourceControl1.Location = new Point(3, 3);
            resourceControl1.Name = "resourceControl1";
            resourceControl1.Size = new Size(444, 317);
            resourceControl1.TabIndex = 0;
            // 
            // PersonaControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl1);
            Name = "PersonaControl";
            Size = new Size(458, 351);
            tabControl1.ResumeLayout(false);
            resourceTab.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage modelTab;
        private TabPage resourceTab;
        private ResourceControl resourceControl1;
    }
}
