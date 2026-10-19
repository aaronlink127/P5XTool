namespace P5XToolGUI
{
    partial class BundleView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            splitContainer1 = new SplitContainer();
            textBox1 = new TextBox();
            listView1 = new ListView();
            name = new ColumnHeader();
            type = new ColumnHeader();
            pathId = new ColumnHeader();
            assetListContext = new ContextMenuStrip(components);
            sortByToolStripMenuItem = new ToolStripMenuItem();
            filterToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            detailsToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            largeIconToolStripMenuItem = new ToolStripMenuItem();
            extractToolStripMenuItem = new ToolStripMenuItem();
            copyNameToolStripMenuItem = new ToolStripMenuItem();
            copyToolStripMenuItem = new ToolStripMenuItem();
            objectView1 = new ObjectView();
            timer1 = new System.Windows.Forms.Timer(components);
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            assetListContext.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(textBox1);
            splitContainer1.Panel1.Controls.Add(listView1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(objectView1);
            splitContainer1.Size = new Size(675, 394);
            splitContainer1.SplitterDistance = 265;
            splitContainer1.TabIndex = 0;
            splitContainer1.SplitterMoved += splitContainer1_SplitterMoved;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(0, 0);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(265, 23);
            textBox1.TabIndex = 1;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // listView1
            // 
            listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView1.Columns.AddRange(new ColumnHeader[] { name, type, pathId });
            listView1.ContextMenuStrip = assetListContext;
            listView1.Location = new Point(0, 29);
            listView1.Name = "listView1";
            listView1.Size = new Size(265, 365);
            listView1.Sorting = SortOrder.Ascending;
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.VirtualMode = true;
            listView1.CacheVirtualItems += listView1_CacheVirtualItems;
            listView1.ColumnClick += listView1_ColumnClick;
            listView1.RetrieveVirtualItem += listView1_RetrieveVirtualItem;
            listView1.SearchForVirtualItem += listView1_SearchForVirtualItem;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // name
            // 
            name.Text = "Name";
            name.Width = 200;
            // 
            // type
            // 
            type.Text = "Type";
            type.Width = 100;
            // 
            // pathId
            // 
            pathId.Text = "Path ID";
            pathId.Width = 100;
            // 
            // assetListContext
            // 
            assetListContext.Items.AddRange(new ToolStripItem[] { sortByToolStripMenuItem, filterToolStripMenuItem, viewToolStripMenuItem, extractToolStripMenuItem, copyNameToolStripMenuItem, copyToolStripMenuItem });
            assetListContext.Name = "assetListContext";
            assetListContext.Size = new Size(138, 136);
            assetListContext.Closing += assetListContext_Closing;
            assetListContext.Opening += assetListContext_Opening;
            // 
            // sortByToolStripMenuItem
            // 
            sortByToolStripMenuItem.Name = "sortByToolStripMenuItem";
            sortByToolStripMenuItem.Size = new Size(137, 22);
            sortByToolStripMenuItem.Text = "Sort By";
            // 
            // filterToolStripMenuItem
            // 
            filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            filterToolStripMenuItem.Size = new Size(137, 22);
            filterToolStripMenuItem.Text = "Filter";
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { detailsToolStripMenuItem, toolStripMenuItem1, largeIconToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(137, 22);
            viewToolStripMenuItem.Text = "View";
            // 
            // detailsToolStripMenuItem
            // 
            detailsToolStripMenuItem.Checked = true;
            detailsToolStripMenuItem.CheckState = CheckState.Checked;
            detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            detailsToolStripMenuItem.Size = new Size(129, 22);
            detailsToolStripMenuItem.Text = "Details";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(129, 22);
            toolStripMenuItem1.Text = "Small Icon";
            // 
            // largeIconToolStripMenuItem
            // 
            largeIconToolStripMenuItem.Name = "largeIconToolStripMenuItem";
            largeIconToolStripMenuItem.Size = new Size(129, 22);
            largeIconToolStripMenuItem.Text = "Large Icon";
            // 
            // extractToolStripMenuItem
            // 
            extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            extractToolStripMenuItem.Size = new Size(137, 22);
            extractToolStripMenuItem.Text = "Extract";
            extractToolStripMenuItem.Click += extractToolStripMenuItem_Click;
            // 
            // copyNameToolStripMenuItem
            // 
            copyNameToolStripMenuItem.Name = "copyNameToolStripMenuItem";
            copyNameToolStripMenuItem.Size = new Size(137, 22);
            copyNameToolStripMenuItem.Text = "Copy Name";
            copyNameToolStripMenuItem.Click += copyNameToolStripMenuItem_Click;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new Size(137, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // objectView1
            // 
            objectView1.Dock = DockStyle.Fill;
            objectView1.Location = new Point(0, 0);
            objectView1.Name = "objectView1";
            objectView1.Size = new Size(406, 394);
            objectView1.TabIndex = 0;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            // 
            // BundleView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "BundleView";
            Size = new Size(675, 394);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            assetListContext.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private ListView listView1;
        private ColumnHeader name;
        private ColumnHeader type;
        private ColumnHeader pathId;
        private ObjectView objectView1;
        private ContextMenuStrip assetListContext;
        private ToolStripMenuItem extractToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem detailsToolStripMenuItem;
        private ToolStripMenuItem largeIconToolStripMenuItem;
        private ToolStripMenuItem filterToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem copyNameToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem sortByToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private TextBox textBox1;
    }
}
