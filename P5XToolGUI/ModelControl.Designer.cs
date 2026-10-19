namespace P5XToolGUI
{
    partial class ModelControl
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
            prefabCheckList = new CheckedListBox();
            saveBtn = new Button();
            tabControl1 = new TabControl();
            modelListTab = new TabPage();
            previewTab = new TabPage();
            resourcesTab = new TabPage();
            resourceControl1 = new ResourceControl();
            tabControl1.SuspendLayout();
            modelListTab.SuspendLayout();
            resourcesTab.SuspendLayout();
            SuspendLayout();
            // 
            // prefabCheckList
            // 
            prefabCheckList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            prefabCheckList.FormattingEnabled = true;
            prefabCheckList.IntegralHeight = false;
            prefabCheckList.Location = new Point(3, 6);
            prefabCheckList.Name = "prefabCheckList";
            prefabCheckList.Size = new Size(290, 172);
            prefabCheckList.TabIndex = 1;
            // 
            // saveBtn
            // 
            saveBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveBtn.Location = new Point(178, 184);
            saveBtn.Name = "saveBtn";
            saveBtn.Size = new Size(115, 23);
            saveBtn.TabIndex = 2;
            saveBtn.Text = "Save Bundles";
            saveBtn.UseVisualStyleBackColor = true;
            saveBtn.Click += saveBtn_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(modelListTab);
            tabControl1.Controls.Add(previewTab);
            tabControl1.Controls.Add(resourcesTab);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(307, 241);
            tabControl1.TabIndex = 3;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // modelListTab
            // 
            modelListTab.Controls.Add(prefabCheckList);
            modelListTab.Controls.Add(saveBtn);
            modelListTab.Location = new Point(4, 24);
            modelListTab.Name = "modelListTab";
            modelListTab.Padding = new Padding(3);
            modelListTab.Size = new Size(299, 213);
            modelListTab.TabIndex = 0;
            modelListTab.Text = "Models List";
            // 
            // previewTab
            // 
            previewTab.Location = new Point(4, 24);
            previewTab.Name = "previewTab";
            previewTab.Padding = new Padding(3);
            previewTab.Size = new Size(299, 213);
            previewTab.TabIndex = 1;
            previewTab.Text = "Preview";
            // 
            // resourcesTab
            // 
            resourcesTab.Controls.Add(resourceControl1);
            resourcesTab.Location = new Point(4, 24);
            resourcesTab.Name = "resourcesTab";
            resourcesTab.Size = new Size(299, 213);
            resourcesTab.TabIndex = 2;
            resourcesTab.Text = "Resources";
            // 
            // resourceControl1
            // 
            resourceControl1.Dock = DockStyle.Fill;
            resourceControl1.Location = new Point(0, 0);
            resourceControl1.Name = "resourceControl1";
            resourceControl1.Size = new Size(299, 213);
            resourceControl1.TabIndex = 0;
            // 
            // ModelControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl1);
            Name = "ModelControl";
            Size = new Size(307, 241);
            tabControl1.ResumeLayout(false);
            modelListTab.ResumeLayout(false);
            resourcesTab.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private CheckedListBox prefabCheckList;
        private Button saveBtn;
        private TabControl tabControl1;
        private TabPage modelListTab;
        private TabPage previewTab;
        private TabPage resourcesTab;
        private ResourceControl resourceControl1;
    }
}
