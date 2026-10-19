namespace P5XToolGUI
{
    partial class BundleForm
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
            components = new System.ComponentModel.Container();
            addName = new TextBox();
            label1 = new Label();
            addBtn = new Button();
            treeView1 = new TreeView();
            cancelBtn = new Button();
            button1 = new Button();
            depCheck = new CheckBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            deleteToolStripMenuItem = new ToolStripMenuItem();
            listCopyBtn = new Button();
            openToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // addName
            // 
            addName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            addName.Location = new Point(12, 32);
            addName.Name = "addName";
            addName.Size = new Size(287, 23);
            addName.TabIndex = 0;
            addName.TextChanged += addName_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(112, 15);
            label1.TabIndex = 1;
            label1.Text = "Asset/Bundle Name";
            // 
            // addBtn
            // 
            addBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            addBtn.Enabled = false;
            addBtn.Location = new Point(305, 32);
            addBtn.Name = "addBtn";
            addBtn.Size = new Size(23, 23);
            addBtn.TabIndex = 2;
            addBtn.Text = "+";
            addBtn.UseVisualStyleBackColor = true;
            addBtn.Click += addBtn_Click;
            // 
            // treeView1
            // 
            treeView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeView1.Location = new Point(12, 86);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(316, 257);
            treeView1.TabIndex = 3;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            // 
            // cancelBtn
            // 
            cancelBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelBtn.Location = new Point(253, 349);
            cancelBtn.Name = "cancelBtn";
            cancelBtn.Size = new Size(75, 23);
            cancelBtn.TabIndex = 4;
            cancelBtn.Text = "Cancel";
            cancelBtn.UseVisualStyleBackColor = true;
            cancelBtn.Click += cancelBtn_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Location = new Point(172, 349);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 5;
            button1.Text = "Save To...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // depCheck
            // 
            depCheck.AutoSize = true;
            depCheck.Checked = true;
            depCheck.CheckState = CheckState.Checked;
            depCheck.Location = new Point(203, 61);
            depCheck.Name = "depCheck";
            depCheck.Size = new Size(125, 19);
            depCheck.TabIndex = 6;
            depCheck.Text = "Add Dependencies";
            depCheck.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { openToolStripMenuItem, deleteToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(181, 70);
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(180, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // listCopyBtn
            // 
            listCopyBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            listCopyBtn.Location = new Point(91, 349);
            listCopyBtn.Name = "listCopyBtn";
            listCopyBtn.Size = new Size(75, 23);
            listCopyBtn.TabIndex = 7;
            listCopyBtn.Text = "Copy List";
            listCopyBtn.UseVisualStyleBackColor = true;
            listCopyBtn.Click += listCopyBtn_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(180, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // BundleForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(340, 384);
            Controls.Add(listCopyBtn);
            Controls.Add(depCheck);
            Controls.Add(button1);
            Controls.Add(cancelBtn);
            Controls.Add(treeView1);
            Controls.Add(addBtn);
            Controls.Add(label1);
            Controls.Add(addName);
            Name = "BundleForm";
            Text = "Bundle Tool";
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox addName;
        private Label label1;
        private Button addBtn;
        private TreeView treeView1;
        private Button cancelBtn;
        private Button button1;
        private CheckBox depCheck;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private Button listCopyBtn;
        private ToolStripMenuItem openToolStripMenuItem;
    }
}