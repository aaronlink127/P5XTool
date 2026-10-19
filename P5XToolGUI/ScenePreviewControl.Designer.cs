namespace P5XToolGUI
{
    partial class ScenePreviewControl
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
            components = new System.ComponentModel.Container();
            updTimer = new System.Windows.Forms.Timer(components);
            splitContainer1 = new SplitContainer();
            popOutBtn = new Button();
            boneName = new TextBox();
            prefabName = new TextBox();
            addPartBtn = new Button();
            partSel = new ComboBox();
            treeView1 = new TreeView();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // updTimer
            // 
            updTimer.Interval = 10;
            updTimer.Tick += updTimer_Tick;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(popOutBtn);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(boneName);
            splitContainer1.Panel2.Controls.Add(prefabName);
            splitContainer1.Panel2.Controls.Add(addPartBtn);
            splitContainer1.Panel2.Controls.Add(partSel);
            splitContainer1.Panel2.Controls.Add(treeView1);
            splitContainer1.Size = new Size(712, 455);
            splitContainer1.SplitterDistance = 515;
            splitContainer1.TabIndex = 1;
            // 
            // popOutBtn
            // 
            popOutBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            popOutBtn.Location = new Point(3, 429);
            popOutBtn.Name = "popOutBtn";
            popOutBtn.Size = new Size(75, 23);
            popOutBtn.TabIndex = 5;
            popOutBtn.Text = "Pop Out";
            popOutBtn.UseVisualStyleBackColor = true;
            popOutBtn.Click += popOutBtn_Click;
            // 
            // boneName
            // 
            boneName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            boneName.Location = new Point(0, 403);
            boneName.Name = "boneName";
            boneName.PlaceholderText = "Bone name (or empty for none)";
            boneName.Size = new Size(193, 23);
            boneName.TabIndex = 4;
            // 
            // prefabName
            // 
            prefabName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            prefabName.Location = new Point(0, 374);
            prefabName.Name = "prefabName";
            prefabName.PlaceholderText = "Prefab name";
            prefabName.Size = new Size(193, 23);
            prefabName.TabIndex = 3;
            prefabName.TextChanged += prefabName_TextChanged;
            // 
            // addPartBtn
            // 
            addPartBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            addPartBtn.Enabled = false;
            addPartBtn.Location = new Point(0, 432);
            addPartBtn.Name = "addPartBtn";
            addPartBtn.Size = new Size(193, 23);
            addPartBtn.TabIndex = 2;
            addPartBtn.Text = "Add Part";
            addPartBtn.UseVisualStyleBackColor = true;
            addPartBtn.Click += addPartBtn_Click;
            // 
            // partSel
            // 
            partSel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            partSel.FormattingEnabled = true;
            partSel.Location = new Point(0, 345);
            partSel.Name = "partSel";
            partSel.Size = new Size(193, 23);
            partSel.TabIndex = 1;
            partSel.SelectedIndexChanged += partSel_SelectedIndexChanged;
            // 
            // treeView1
            // 
            treeView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeView1.CheckBoxes = true;
            treeView1.HideSelection = false;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(193, 339);
            treeView1.TabIndex = 0;
            treeView1.AfterCheck += treeView1_AfterCheck;
            treeView1.AfterCollapse += treeView1_AfterCollapse;
            treeView1.AfterExpand += treeView1_AfterExpand;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            // 
            // ScenePreviewControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "ScenePreviewControl";
            Size = new Size(712, 455);
            Leave += ScenePreview_Leave;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Timer updTimer;
        private SplitContainer splitContainer1;
        private TreeView treeView1;
        private Button addPartBtn;
        private ComboBox partSel;
        private TextBox boneName;
        private TextBox prefabName;
        private Button popOutBtn;
    }
}
