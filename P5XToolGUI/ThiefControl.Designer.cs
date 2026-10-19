namespace P5XToolGUI
{
    partial class ThiefControl
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
            comboBox1 = new ComboBox();
            groupBox1 = new GroupBox();
            tabControl = new TabControl();
            modelTab = new TabPage();
            resTab = new TabPage();
            resourceControl1 = new ResourceControl();
            tabControl.SuspendLayout();
            resTab.SuspendLayout();
            SuspendLayout();
            // 
            // comboBox1
            // 
            comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(9, 18);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(307, 23);
            comboBox1.TabIndex = 0;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(319, 45);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Thief Model";
            // 
            // tabControl
            // 
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(modelTab);
            tabControl.Controls.Add(resTab);
            tabControl.Location = new Point(0, 54);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(325, 200);
            tabControl.TabIndex = 4;
            tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;
            // 
            // modelTab
            // 
            modelTab.BackColor = Color.Transparent;
            modelTab.Location = new Point(4, 24);
            modelTab.Name = "modelTab";
            modelTab.Padding = new Padding(3);
            modelTab.Size = new Size(317, 172);
            modelTab.TabIndex = 0;
            modelTab.Text = "Models";
            // 
            // resTab
            // 
            resTab.Controls.Add(resourceControl1);
            resTab.Location = new Point(4, 24);
            resTab.Name = "resTab";
            resTab.Padding = new Padding(3);
            resTab.Size = new Size(317, 172);
            resTab.TabIndex = 1;
            resTab.Text = "Resources";
            resTab.UseVisualStyleBackColor = true;
            // 
            // resourceControl1
            // 
            resourceControl1.Dock = DockStyle.Fill;
            resourceControl1.Location = new Point(3, 3);
            resourceControl1.Name = "resourceControl1";
            resourceControl1.Size = new Size(311, 166);
            resourceControl1.TabIndex = 0;
            // 
            // ThiefControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl);
            Controls.Add(comboBox1);
            Controls.Add(groupBox1);
            Name = "ThiefControl";
            Size = new Size(325, 254);
            tabControl.ResumeLayout(false);
            resTab.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ComboBox comboBox1;
        private GroupBox groupBox1;
        private TabControl tabControl;
        private TabPage modelTab;
        private TabPage resTab;
        private ResourceControl resourceControl1;
    }
}
