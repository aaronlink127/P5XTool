namespace P5XToolGUI
{
    partial class DialogHeadForm
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
            dialogHeadSelect = new ComboBox();
            eyeAnimFrame = new ComboBox();
            mouthAnimFrame = new ComboBox();
            expressionAnimFrame = new ComboBox();
            simToggle = new CheckBox();
            splitContainer1 = new SplitContainer();
            centerImgView1 = new CenterImgView();
            tableLayoutPanel1 = new TableLayoutPanel();
            label3 = new Label();
            label4 = new Label();
            label2 = new Label();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // dialogHeadSelect
            // 
            dialogHeadSelect.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dialogHeadSelect.FormattingEnabled = true;
            dialogHeadSelect.Location = new Point(6, 12);
            dialogHeadSelect.Name = "dialogHeadSelect";
            dialogHeadSelect.Size = new Size(744, 23);
            dialogHeadSelect.TabIndex = 0;
            dialogHeadSelect.Text = "Select a portrait";
            dialogHeadSelect.SelectedIndexChanged += dialogHeadSelect_SelectedIndexChanged;
            // 
            // eyeAnimFrame
            // 
            eyeAnimFrame.Dock = DockStyle.Fill;
            eyeAnimFrame.DropDownStyle = ComboBoxStyle.DropDownList;
            eyeAnimFrame.Enabled = false;
            eyeAnimFrame.FormattingEnabled = true;
            eyeAnimFrame.Location = new Point(0, 15);
            eyeAnimFrame.Margin = new Padding(0, 0, 2, 0);
            eyeAnimFrame.Name = "eyeAnimFrame";
            eyeAnimFrame.Size = new Size(246, 23);
            eyeAnimFrame.TabIndex = 2;
            eyeAnimFrame.SelectionChangeCommitted += eyeAnimFrame_SelectionChangeCommitted;
            // 
            // mouthAnimFrame
            // 
            mouthAnimFrame.Dock = DockStyle.Fill;
            mouthAnimFrame.DropDownStyle = ComboBoxStyle.DropDownList;
            mouthAnimFrame.Enabled = false;
            mouthAnimFrame.FormattingEnabled = true;
            mouthAnimFrame.Location = new Point(249, 15);
            mouthAnimFrame.Margin = new Padding(1, 0, 1, 0);
            mouthAnimFrame.Name = "mouthAnimFrame";
            mouthAnimFrame.Size = new Size(246, 23);
            mouthAnimFrame.TabIndex = 3;
            mouthAnimFrame.SelectionChangeCommitted += mouthAnimFrame_SelectionChangeCommitted;
            // 
            // expressionAnimFrame
            // 
            expressionAnimFrame.Dock = DockStyle.Fill;
            expressionAnimFrame.DropDownStyle = ComboBoxStyle.DropDownList;
            expressionAnimFrame.Enabled = false;
            expressionAnimFrame.FormattingEnabled = true;
            expressionAnimFrame.Location = new Point(498, 15);
            expressionAnimFrame.Margin = new Padding(2, 0, 0, 0);
            expressionAnimFrame.Name = "expressionAnimFrame";
            expressionAnimFrame.Size = new Size(246, 23);
            expressionAnimFrame.TabIndex = 4;
            expressionAnimFrame.SelectionChangeCommitted += expressionAnimFrame_SelectionChangeCommitted;
            // 
            // simToggle
            // 
            simToggle.AutoSize = true;
            simToggle.Checked = true;
            simToggle.CheckState = CheckState.Checked;
            simToggle.Location = new Point(3, 3);
            simToggle.Margin = new Padding(0);
            simToggle.Name = "simToggle";
            simToggle.Size = new Size(140, 19);
            simToggle.TabIndex = 5;
            simToggle.Text = "Load Material Params";
            simToggle.UseVisualStyleBackColor = true;
            simToggle.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(6, 79);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(centerImgView1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.AutoScroll = true;
            splitContainer1.Panel2.BackColor = SystemColors.Window;
            splitContainer1.Panel2.Controls.Add(simToggle);
            splitContainer1.Size = new Size(744, 359);
            splitContainer1.SplitterDistance = 510;
            splitContainer1.TabIndex = 6;
            // 
            // centerImgView1
            // 
            centerImgView1.Dock = DockStyle.Fill;
            centerImgView1.Location = new Point(0, 0);
            centerImgView1.Name = "centerImgView1";
            centerImgView1.Size = new Size(510, 359);
            centerImgView1.TabIndex = 7;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Controls.Add(eyeAnimFrame, 0, 1);
            tableLayoutPanel1.Controls.Add(mouthAnimFrame, 1, 1);
            tableLayoutPanel1.Controls.Add(expressionAnimFrame, 2, 1);
            tableLayoutPanel1.Controls.Add(label3, 1, 0);
            tableLayoutPanel1.Controls.Add(label4, 2, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 0);
            tableLayoutPanel1.Location = new Point(6, 38);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(744, 40);
            tableLayoutPanel1.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoEllipsis = true;
            label3.Location = new Point(248, 0);
            label3.Margin = new Padding(0);
            label3.Name = "label3";
            label3.Size = new Size(79, 15);
            label3.TabIndex = 6;
            label3.Text = "Mouth Frame";
            // 
            // label4
            // 
            label4.AutoEllipsis = true;
            label4.Location = new Point(496, 0);
            label4.Margin = new Padding(0);
            label4.Name = "label4";
            label4.Size = new Size(99, 15);
            label4.TabIndex = 7;
            label4.Text = "Expression Frame";
            // 
            // label2
            // 
            label2.AutoEllipsis = true;
            label2.Location = new Point(0, 0);
            label2.Margin = new Padding(0);
            label2.Name = "label2";
            label2.Size = new Size(61, 15);
            label2.TabIndex = 5;
            label2.Text = "Eye Frame";
            // 
            // DialogHeadForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(759, 450);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(splitContainer1);
            Controls.Add(dialogHeadSelect);
            Name = "DialogHeadForm";
            Text = "Dialog Sprite Preview";
            FormClosed += DialogHeadForm_FormClosed;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ComboBox dialogHeadSelect;
        private ComboBox eyeAnimFrame;
        private ComboBox mouthAnimFrame;
        private ComboBox expressionAnimFrame;
        private CheckBox simToggle;
        private SplitContainer splitContainer1;
        private CenterImgView centerImgView1;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label2;
        private Label label3;
        private Label label4;
        private ToolTip toolTip1;
    }
}