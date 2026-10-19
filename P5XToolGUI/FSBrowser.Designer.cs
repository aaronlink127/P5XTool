namespace P5XToolGUI
{
    partial class FSBrowser
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
            splitContainer1 = new SplitContainer();
            listView1 = new ListView();
            name = new ColumnHeader();
            filterTextBox = new TextBox();
            swapTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.SuspendLayout();
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
            splitContainer1.Panel1.Controls.Add(listView1);
            splitContainer1.Panel1.Controls.Add(filterTextBox);
            splitContainer1.Size = new Size(878, 477);
            splitContainer1.SplitterDistance = 200;
            splitContainer1.TabIndex = 0;
            // 
            // listView1
            // 
            listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView1.Columns.AddRange(new ColumnHeader[] { name });
            listView1.Location = new Point(0, 35);
            listView1.Name = "listView1";
            listView1.Size = new Size(197, 442);
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.VirtualMode = true;
            listView1.RetrieveVirtualItem += listView1_RetrieveVirtualItem;
            listView1.SearchForVirtualItem += listView1_SearchForVirtualItem;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // name
            // 
            name.Text = "Name";
            name.Width = 180;
            // 
            // filterTextBox
            // 
            filterTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            filterTextBox.Location = new Point(0, 6);
            filterTextBox.Name = "filterTextBox";
            filterTextBox.Size = new Size(197, 23);
            filterTextBox.TabIndex = 1;
            filterTextBox.TextChanged += filterTextBox_TextChanged;
            // 
            // swapTimer
            // 
            swapTimer.Interval = 300;
            swapTimer.Tick += swapTimer_Tick;
            // 
            // FSBrowser
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(878, 477);
            Controls.Add(splitContainer1);
            Name = "FSBrowser";
            Text = "FSBrowser";
            FormClosing += FSBrowser_FormClosing;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private TextBox filterTextBox;
        private System.Windows.Forms.Timer swapTimer;
        private ListView listView1;
        private ColumnHeader name;
    }
}