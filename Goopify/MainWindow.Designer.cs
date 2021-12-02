namespace Goopify
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.overallLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.sidebarContentFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.openCollisionButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.addRegionButton = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.goopRegionListBox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.preview3dCheckBox = new System.Windows.Forms.CheckBox();
            this.glControl1 = new OpenTK.GLControl();
            this.mainWindowBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.menuStrip1.SuspendLayout();
            this.overallLayoutPanel.SuspendLayout();
            this.sidebarContentFlowLayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainWindowBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1108, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // overallLayoutPanel
            // 
            this.overallLayoutPanel.ColumnCount = 2;
            this.overallLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.overallLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.overallLayoutPanel.Controls.Add(this.sidebarContentFlowLayoutPanel, 0, 0);
            this.overallLayoutPanel.Controls.Add(this.panel1, 1, 0);
            this.overallLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overallLayoutPanel.Location = new System.Drawing.Point(0, 24);
            this.overallLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.overallLayoutPanel.Name = "overallLayoutPanel";
            this.overallLayoutPanel.RowCount = 1;
            this.overallLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.overallLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 595F));
            this.overallLayoutPanel.Size = new System.Drawing.Size(1108, 595);
            this.overallLayoutPanel.TabIndex = 1;
            // 
            // sidebarContentFlowLayoutPanel
            // 
            this.sidebarContentFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.sidebarContentFlowLayoutPanel.AutoScroll = true;
            this.sidebarContentFlowLayoutPanel.AutoSize = true;
            this.sidebarContentFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.sidebarContentFlowLayoutPanel.Controls.Add(this.panel3);
            this.sidebarContentFlowLayoutPanel.Controls.Add(this.openCollisionButton);
            this.sidebarContentFlowLayoutPanel.Controls.Add(this.panel2);
            this.sidebarContentFlowLayoutPanel.Controls.Add(this.addRegionButton);
            this.sidebarContentFlowLayoutPanel.Controls.Add(this.panel4);
            this.sidebarContentFlowLayoutPanel.Controls.Add(this.goopRegionListBox);
            this.sidebarContentFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.sidebarContentFlowLayoutPanel.Location = new System.Drawing.Point(5, 0);
            this.sidebarContentFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.sidebarContentFlowLayoutPanel.Name = "sidebarContentFlowLayoutPanel";
            this.sidebarContentFlowLayoutPanel.Size = new System.Drawing.Size(210, 595);
            this.sidebarContentFlowLayoutPanel.TabIndex = 5;
            this.sidebarContentFlowLayoutPanel.WrapContents = false;
            // 
            // panel3
            // 
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 10);
            this.panel3.TabIndex = 5;
            // 
            // openCollisionButton
            // 
            this.openCollisionButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.openCollisionButton.Location = new System.Drawing.Point(0, 10);
            this.openCollisionButton.Margin = new System.Windows.Forms.Padding(0);
            this.openCollisionButton.Name = "openCollisionButton";
            this.openCollisionButton.Size = new System.Drawing.Size(210, 23);
            this.openCollisionButton.TabIndex = 1;
            this.openCollisionButton.Text = "Open Collision File";
            this.openCollisionButton.UseVisualStyleBackColor = true;
            this.openCollisionButton.Click += new System.EventHandler(this.openCollisionButton_Click);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(0, 33);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 23);
            this.panel2.TabIndex = 6;
            // 
            // addRegionButton
            // 
            this.addRegionButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.addRegionButton.Location = new System.Drawing.Point(15, 56);
            this.addRegionButton.Margin = new System.Windows.Forms.Padding(0);
            this.addRegionButton.Name = "addRegionButton";
            this.addRegionButton.Size = new System.Drawing.Size(179, 34);
            this.addRegionButton.TabIndex = 3;
            this.addRegionButton.Text = "Insert Goop Region";
            this.addRegionButton.UseVisualStyleBackColor = true;
            this.addRegionButton.Click += new System.EventHandler(this.addRegionButton_Click);
            // 
            // panel4
            // 
            this.panel4.Location = new System.Drawing.Point(0, 90);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(200, 10);
            this.panel4.TabIndex = 6;
            // 
            // goopRegionListBox
            // 
            this.goopRegionListBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.goopRegionListBox.FormattingEnabled = true;
            this.goopRegionListBox.Location = new System.Drawing.Point(4, 100);
            this.goopRegionListBox.Margin = new System.Windows.Forms.Padding(0);
            this.goopRegionListBox.Name = "goopRegionListBox";
            this.goopRegionListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.goopRegionListBox.Size = new System.Drawing.Size(202, 329);
            this.goopRegionListBox.TabIndex = 0;
            this.goopRegionListBox.SelectedIndexChanged += new System.EventHandler(this.goopRegionListBox_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.preview3dCheckBox);
            this.panel1.Controls.Add(this.glControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(220, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(888, 595);
            this.panel1.TabIndex = 2;
            // 
            // preview3dCheckBox
            // 
            this.preview3dCheckBox.AutoSize = true;
            this.preview3dCheckBox.Location = new System.Drawing.Point(14, 12);
            this.preview3dCheckBox.Name = "preview3dCheckBox";
            this.preview3dCheckBox.Size = new System.Drawing.Size(81, 17);
            this.preview3dCheckBox.TabIndex = 2;
            this.preview3dCheckBox.Text = "Preview 3D";
            this.preview3dCheckBox.UseVisualStyleBackColor = true;
            this.preview3dCheckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // glControl1
            // 
            this.glControl1.AllowDrop = true;
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl1.Location = new System.Drawing.Point(0, 0);
            this.glControl1.Margin = new System.Windows.Forms.Padding(0);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(888, 595);
            this.glControl1.TabIndex = 1;
            this.glControl1.VSync = false;
            this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyDown);
            this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyUp);
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
            this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
            this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
            this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
            // 
            // mainWindowBindingSource
            // 
            this.mainWindowBindingSource.DataSource = typeof(Goopify.MainWindow);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(1108, 619);
            this.Controls.Add(this.overallLayoutPanel);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "MainWindow";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Goopify";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.overallLayoutPanel.ResumeLayout(false);
            this.overallLayoutPanel.PerformLayout();
            this.sidebarContentFlowLayoutPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainWindowBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TableLayoutPanel overallLayoutPanel;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox preview3dCheckBox;
        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.BindingSource mainWindowBindingSource;
        private System.Windows.Forms.FlowLayoutPanel sidebarContentFlowLayoutPanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button openCollisionButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button addRegionButton;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ListBox goopRegionListBox;
    }
}

