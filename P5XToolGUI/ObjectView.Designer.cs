namespace P5XToolGUI
{
    partial class ObjectView
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
            previewTab = new TabPage();
            dumpTab = new TabPage();
            textBox1 = new TextBox();
            tabControl1.SuspendLayout();
            dumpTab.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(previewTab);
            tabControl1.Controls.Add(dumpTab);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(150, 150);
            tabControl1.TabIndex = 0;
            // 
            // previewTab
            // 
            previewTab.BackColor = Color.Silver;
            previewTab.Location = new Point(4, 24);
            previewTab.Name = "previewTab";
            previewTab.Padding = new Padding(3);
            previewTab.Size = new Size(142, 122);
            previewTab.TabIndex = 0;
            previewTab.Text = "Preview";
            // 
            // dumpTab
            // 
            dumpTab.Controls.Add(textBox1);
            dumpTab.Location = new Point(4, 24);
            dumpTab.Name = "dumpTab";
            dumpTab.Padding = new Padding(3);
            dumpTab.Size = new Size(142, 122);
            dumpTab.TabIndex = 1;
            dumpTab.Text = "Dump";
            dumpTab.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Fill;
            textBox1.Location = new Point(3, 3);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Both;
            textBox1.Size = new Size(136, 116);
            textBox1.TabIndex = 0;
            // 
            // ObjectView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl1);
            Name = "ObjectView";
            tabControl1.ResumeLayout(false);
            dumpTab.ResumeLayout(false);
            dumpTab.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage previewTab;
        private TabPage dumpTab;
        private TextBox textBox1;
    }
}
