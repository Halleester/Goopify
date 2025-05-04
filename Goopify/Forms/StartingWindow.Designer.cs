
namespace Goopify
{
    partial class StartingWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartingWindow));
            this.newGoopmapButton = new System.Windows.Forms.Button();
            this.editGoopmapButton = new System.Windows.Forms.Button();
            this.extractGoopmapButton = new System.Windows.Forms.Button();
            this.toolIcon = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.helpButton = new System.Windows.Forms.Button();
            this.startingTooltip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.toolIcon)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // newGoopmapButton
            // 
            this.newGoopmapButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(66)))), ((int)(((byte)(95)))));
            this.newGoopmapButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.newGoopmapButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(66)))), ((int)(((byte)(95)))));
            this.newGoopmapButton.FlatAppearance.BorderSize = 0;
            this.newGoopmapButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newGoopmapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newGoopmapButton.ForeColor = System.Drawing.Color.LightGray;
            this.newGoopmapButton.Location = new System.Drawing.Point(273, 220);
            this.newGoopmapButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.newGoopmapButton.Name = "newGoopmapButton";
            this.newGoopmapButton.Size = new System.Drawing.Size(234, 63);
            this.newGoopmapButton.TabIndex = 0;
            this.newGoopmapButton.Text = "New GoopMap";
            this.startingTooltip.SetToolTip(this.newGoopmapButton, "Create a new goopmap for a level using a collision file.");
            this.newGoopmapButton.UseVisualStyleBackColor = false;
            this.newGoopmapButton.Click += new System.EventHandler(this.newGoopmapButton_Click);
            // 
            // editGoopmapButton
            // 
            this.editGoopmapButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(66)))), ((int)(((byte)(95)))));
            this.editGoopmapButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.editGoopmapButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(66)))), ((int)(((byte)(95)))));
            this.editGoopmapButton.FlatAppearance.BorderSize = 0;
            this.editGoopmapButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editGoopmapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editGoopmapButton.ForeColor = System.Drawing.Color.LightGray;
            this.editGoopmapButton.Location = new System.Drawing.Point(273, 308);
            this.editGoopmapButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.editGoopmapButton.Name = "editGoopmapButton";
            this.editGoopmapButton.Size = new System.Drawing.Size(234, 63);
            this.editGoopmapButton.TabIndex = 1;
            this.editGoopmapButton.Text = "Edit GoopMap";
            this.startingTooltip.SetToolTip(this.editGoopmapButton, "Load an existing Goopify goopmap file (.goop) to modify or rebuild it.");
            this.editGoopmapButton.UseVisualStyleBackColor = false;
            this.editGoopmapButton.Click += new System.EventHandler(this.editGoopmapButton_Click);
            // 
            // extractGoopmapButton
            // 
            this.extractGoopmapButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(66)))), ((int)(((byte)(95)))));
            this.extractGoopmapButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.extractGoopmapButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(66)))), ((int)(((byte)(95)))));
            this.extractGoopmapButton.FlatAppearance.BorderSize = 0;
            this.extractGoopmapButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.extractGoopmapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.extractGoopmapButton.ForeColor = System.Drawing.Color.LightGray;
            this.extractGoopmapButton.Location = new System.Drawing.Point(273, 394);
            this.extractGoopmapButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.extractGoopmapButton.Name = "extractGoopmapButton";
            this.extractGoopmapButton.Size = new System.Drawing.Size(234, 63);
            this.extractGoopmapButton.TabIndex = 2;
            this.extractGoopmapButton.Text = "Extract YMP Data";
            this.startingTooltip.SetToolTip(this.extractGoopmapButton, "Extract the data from a .ymp file to view the heightmaps and data stored within i" +
        "t.");
            this.extractGoopmapButton.UseVisualStyleBackColor = false;
            this.extractGoopmapButton.Click += new System.EventHandler(this.extractGoopmapButton_Click);
            // 
            // toolIcon
            // 
            this.toolIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.toolIcon.BackgroundImage = global::Goopify.Properties.Resources.GoopifyIcon;
            this.toolIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolIcon.Location = new System.Drawing.Point(27, 9);
            this.toolIcon.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.toolIcon.Name = "toolIcon";
            this.toolIcon.Size = new System.Drawing.Size(147, 152);
            this.toolIcon.TabIndex = 3;
            this.toolIcon.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label1.Font = new System.Drawing.Font("Lucida Sans", 50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Location = new System.Drawing.Point(188, 29);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(422, 112);
            this.label1.TabIndex = 4;
            this.label1.Text = "Goopify";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.panel1.Controls.Add(this.toolIcon);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(88, 32);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(624, 171);
            this.panel1.TabIndex = 5;
            // 
            // helpButton
            // 
            this.helpButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(66)))), ((int)(((byte)(95)))));
            this.helpButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.helpButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(66)))), ((int)(((byte)(95)))));
            this.helpButton.FlatAppearance.BorderSize = 0;
            this.helpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.helpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpButton.ForeColor = System.Drawing.Color.LightGray;
            this.helpButton.Location = new System.Drawing.Point(749, 428);
            this.helpButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(39, 36);
            this.helpButton.TabIndex = 7;
            this.helpButton.Text = "?";
            this.startingTooltip.SetToolTip(this.helpButton, "Help");
            this.helpButton.UseVisualStyleBackColor = false;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // StartingWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(801, 478);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.extractGoopmapButton);
            this.Controls.Add(this.editGoopmapButton);
            this.Controls.Add(this.newGoopmapButton);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "StartingWindow";
            this.Text = "Goopify";
            ((System.ComponentModel.ISupportInitialize)(this.toolIcon)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button newGoopmapButton;
        private System.Windows.Forms.Button editGoopmapButton;
        private System.Windows.Forms.Button extractGoopmapButton;
        private System.Windows.Forms.PictureBox toolIcon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolTip startingTooltip;
        private System.Windows.Forms.Button helpButton;
    }
}