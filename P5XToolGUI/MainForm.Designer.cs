namespace P5XToolGUI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            gameFolderBrowser = new FolderBrowserDialog();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            pickGameFolderToolStripMenuItem = new ToolStripMenuItem();
            recentFoldersToolStripMenuItem = new ToolStripMenuItem();
            closeGameToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            configDataExportToolStripMenuItem = new ToolStripMenuItem();
            genConfigCClassToolStripMenuItem = new ToolStripMenuItem();
            assetBundleExtractorToolStripMenuItem = new ToolStripMenuItem();
            extractLuaScriptsToolStripMenuItem = new ToolStripMenuItem();
            dialogSpritesToolStripMenuItem = new ToolStripMenuItem();
            spriteExtractToolStripMenuItem = new ToolStripMenuItem();
            fSBrowserToolStripMenuItem = new ToolStripMenuItem();
            spriteBrowserToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            setGameLanguageToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem1 = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            treeView1 = new TreeView();
            contextMenuStrip1 = new ContextMenuStrip(components);
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            timer1 = new System.Windows.Forms.Timer(components);
            sceneViewerToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolsToolStripMenuItem, optionsToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { pickGameFolderToolStripMenuItem, recentFoldersToolStripMenuItem, closeGameToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // pickGameFolderToolStripMenuItem
            // 
            pickGameFolderToolStripMenuItem.Name = "pickGameFolderToolStripMenuItem";
            pickGameFolderToolStripMenuItem.Size = new Size(173, 22);
            pickGameFolderToolStripMenuItem.Text = "Pick Game Folder..";
            pickGameFolderToolStripMenuItem.Click += pickGameFolderToolStripMenuItem_Click;
            // 
            // recentFoldersToolStripMenuItem
            // 
            recentFoldersToolStripMenuItem.Enabled = false;
            recentFoldersToolStripMenuItem.Name = "recentFoldersToolStripMenuItem";
            recentFoldersToolStripMenuItem.Size = new Size(173, 22);
            recentFoldersToolStripMenuItem.Text = "Recent Folders";
            // 
            // closeGameToolStripMenuItem
            // 
            closeGameToolStripMenuItem.Enabled = false;
            closeGameToolStripMenuItem.Name = "closeGameToolStripMenuItem";
            closeGameToolStripMenuItem.Size = new Size(173, 22);
            closeGameToolStripMenuItem.Text = "Close Game Folder";
            closeGameToolStripMenuItem.Click += closeGameToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { configDataExportToolStripMenuItem, genConfigCClassToolStripMenuItem, assetBundleExtractorToolStripMenuItem, extractLuaScriptsToolStripMenuItem, dialogSpritesToolStripMenuItem, spriteExtractToolStripMenuItem, fSBrowserToolStripMenuItem, spriteBrowserToolStripMenuItem, sceneViewerToolStripMenuItem });
            toolsToolStripMenuItem.Enabled = false;
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // configDataExportToolStripMenuItem
            // 
            configDataExportToolStripMenuItem.Name = "configDataExportToolStripMenuItem";
            configDataExportToolStripMenuItem.Size = new Size(208, 22);
            configDataExportToolStripMenuItem.Text = "Config Data Export";
            configDataExportToolStripMenuItem.Click += configDataExportToolStripMenuItem_Click;
            // 
            // genConfigCClassToolStripMenuItem
            // 
            genConfigCClassToolStripMenuItem.Name = "genConfigCClassToolStripMenuItem";
            genConfigCClassToolStripMenuItem.Size = new Size(208, 22);
            genConfigCClassToolStripMenuItem.Text = "Generate Config C# Class";
            genConfigCClassToolStripMenuItem.Click += genConfigCClassToolStripMenuItem_Click;
            // 
            // assetBundleExtractorToolStripMenuItem
            // 
            assetBundleExtractorToolStripMenuItem.Name = "assetBundleExtractorToolStripMenuItem";
            assetBundleExtractorToolStripMenuItem.Size = new Size(208, 22);
            assetBundleExtractorToolStripMenuItem.Text = "Asset Bundle Extractor";
            assetBundleExtractorToolStripMenuItem.Click += assetBundleExtractorToolStripMenuItem_Click;
            // 
            // extractLuaScriptsToolStripMenuItem
            // 
            extractLuaScriptsToolStripMenuItem.Name = "extractLuaScriptsToolStripMenuItem";
            extractLuaScriptsToolStripMenuItem.Size = new Size(208, 22);
            extractLuaScriptsToolStripMenuItem.Text = "Extract Lua Scripts";
            extractLuaScriptsToolStripMenuItem.Click += extractLuaScriptsToolStripMenuItem_Click;
            // 
            // dialogSpritesToolStripMenuItem
            // 
            dialogSpritesToolStripMenuItem.Name = "dialogSpritesToolStripMenuItem";
            dialogSpritesToolStripMenuItem.Size = new Size(208, 22);
            dialogSpritesToolStripMenuItem.Text = "Dialog Sprites";
            dialogSpritesToolStripMenuItem.Click += dialogSpritesToolStripMenuItem_Click;
            // 
            // spriteExtractToolStripMenuItem
            // 
            spriteExtractToolStripMenuItem.Name = "spriteExtractToolStripMenuItem";
            spriteExtractToolStripMenuItem.Size = new Size(208, 22);
            spriteExtractToolStripMenuItem.Text = "Sprite Extract";
            spriteExtractToolStripMenuItem.Click += spriteExtractToolStripMenuItem_Click;
            // 
            // fSBrowserToolStripMenuItem
            // 
            fSBrowserToolStripMenuItem.Name = "fSBrowserToolStripMenuItem";
            fSBrowserToolStripMenuItem.Size = new Size(208, 22);
            fSBrowserToolStripMenuItem.Text = "FS Browser";
            fSBrowserToolStripMenuItem.Click += fSToolStripMenuItem_Click;
            // 
            // spriteBrowserToolStripMenuItem
            // 
            spriteBrowserToolStripMenuItem.Name = "spriteBrowserToolStripMenuItem";
            spriteBrowserToolStripMenuItem.Size = new Size(208, 22);
            spriteBrowserToolStripMenuItem.Text = "Sprite Browser";
            spriteBrowserToolStripMenuItem.Click += spriteBrowserToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { setGameLanguageToolStripMenuItem, optionsToolStripMenuItem1 });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(61, 20);
            optionsToolStripMenuItem.Text = "Options";
            // 
            // setGameLanguageToolStripMenuItem
            // 
            setGameLanguageToolStripMenuItem.Enabled = false;
            setGameLanguageToolStripMenuItem.Name = "setGameLanguageToolStripMenuItem";
            setGameLanguageToolStripMenuItem.Size = new Size(179, 22);
            setGameLanguageToolStripMenuItem.Text = "Set Game Language";
            // 
            // optionsToolStripMenuItem1
            // 
            optionsToolStripMenuItem1.Name = "optionsToolStripMenuItem1";
            optionsToolStripMenuItem1.Size = new Size(179, 22);
            optionsToolStripMenuItem1.Text = "Options";
            optionsToolStripMenuItem1.Click += optionsToolStripMenuItem1_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(treeView1);
            splitContainer1.Size = new Size(800, 401);
            splitContainer1.SplitterDistance = 202;
            splitContainer1.TabIndex = 1;
            // 
            // treeView1
            // 
            treeView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeView1.HideSelection = false;
            treeView1.Location = new Point(3, 3);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(196, 395);
            treeView1.TabIndex = 0;
            treeView1.AfterSelect += treeView1_AfterSelect;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(0, 17);
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // sceneViewerToolStripMenuItem
            // 
            sceneViewerToolStripMenuItem.Name = "sceneViewerToolStripMenuItem";
            sceneViewerToolStripMenuItem.Size = new Size(208, 22);
            sceneViewerToolStripMenuItem.Text = "Scene Viewer";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(statusStrip1);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            FormClosed += MainForm_FormClosed;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FolderBrowserDialog gameFolderBrowser;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem pickGameFolderToolStripMenuItem;
        private SplitContainer splitContainer1;
        private TreeView treeView1;
        private ToolStripMenuItem recentFoldersToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem closeGameToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem configDataExportToolStripMenuItem;
        private ToolStripMenuItem assetBundleExtractorToolStripMenuItem;
        private ToolStripMenuItem genConfigCClassToolStripMenuItem;
        private ToolStripMenuItem dialogSpritesToolStripMenuItem;
        private ToolStripMenuItem spriteExtractToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem setGameLanguageToolStripMenuItem;
        private ToolStripMenuItem extractLuaScriptsToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Timer timer1;
        private ToolStripMenuItem fSBrowserToolStripMenuItem;
        private ToolStripMenuItem spriteBrowserToolStripMenuItem;
        private ToolStripMenuItem sceneViewerToolStripMenuItem;
    }
}
