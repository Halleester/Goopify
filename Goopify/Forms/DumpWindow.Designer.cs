
namespace Goopify.Forms
{
    partial class DumpWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DumpWindow));
            this.ympLocLabel = new System.Windows.Forms.Label();
            this.saveLocLabel = new System.Windows.Forms.Label();
            this.ympLocFileBox = new System.Windows.Forms.TextBox();
            this.ympLocButton = new System.Windows.Forms.Button();
            this.saveLocButton = new System.Windows.Forms.Button();
            this.saveLocFileBox = new System.Windows.Forms.TextBox();
            this.heightmapCheckbox = new System.Windows.Forms.CheckBox();
            this.infoCheckbox = new System.Windows.Forms.CheckBox();
            this.extractButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ympLocLabel
            // 
            this.ympLocLabel.AutoSize = true;
            this.ympLocLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ympLocLabel.ForeColor = System.Drawing.Color.LightGray;
            this.ympLocLabel.Location = new System.Drawing.Point(24, 57);
            this.ympLocLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ympLocLabel.Name = "ympLocLabel";
            this.ympLocLabel.Size = new System.Drawing.Size(131, 20);
            this.ympLocLabel.TabIndex = 0;
            this.ympLocLabel.Text = "YMP Location:";
            // 
            // saveLocLabel
            // 
            this.saveLocLabel.AutoSize = true;
            this.saveLocLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveLocLabel.ForeColor = System.Drawing.Color.LightGray;
            this.saveLocLabel.Location = new System.Drawing.Point(24, 109);
            this.saveLocLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.saveLocLabel.Name = "saveLocLabel";
            this.saveLocLabel.Size = new System.Drawing.Size(134, 20);
            this.saveLocLabel.TabIndex = 1;
            this.saveLocLabel.Text = "Save Location:";
            // 
            // ympLocFileBox
            // 
            this.ympLocFileBox.Location = new System.Drawing.Point(177, 53);
            this.ympLocFileBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ympLocFileBox.Name = "ympLocFileBox";
            this.ympLocFileBox.Size = new System.Drawing.Size(339, 26);
            this.ympLocFileBox.TabIndex = 2;
            this.ympLocFileBox.TextChanged += new System.EventHandler(this.ympLocFileBox_TextChanged);
            // 
            // ympLocButton
            // 
            this.ympLocButton.Location = new System.Drawing.Point(532, 49);
            this.ympLocButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ympLocButton.Name = "ympLocButton";
            this.ympLocButton.Size = new System.Drawing.Size(44, 31);
            this.ympLocButton.TabIndex = 3;
            this.ympLocButton.Text = "...";
            this.ympLocButton.UseVisualStyleBackColor = true;
            this.ympLocButton.Click += new System.EventHandler(this.ympLocButton_Click);
            // 
            // saveLocButton
            // 
            this.saveLocButton.Location = new System.Drawing.Point(532, 103);
            this.saveLocButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.saveLocButton.Name = "saveLocButton";
            this.saveLocButton.Size = new System.Drawing.Size(44, 31);
            this.saveLocButton.TabIndex = 5;
            this.saveLocButton.Text = "...";
            this.saveLocButton.UseVisualStyleBackColor = true;
            this.saveLocButton.Click += new System.EventHandler(this.saveLocButton_Click);
            // 
            // saveLocFileBox
            // 
            this.saveLocFileBox.Location = new System.Drawing.Point(177, 105);
            this.saveLocFileBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.saveLocFileBox.Name = "saveLocFileBox";
            this.saveLocFileBox.Size = new System.Drawing.Size(339, 26);
            this.saveLocFileBox.TabIndex = 4;
            this.saveLocFileBox.TextChanged += new System.EventHandler(this.saveLocFileBox_TextChanged);
            // 
            // heightmapCheckbox
            // 
            this.heightmapCheckbox.AutoSize = true;
            this.heightmapCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.heightmapCheckbox.ForeColor = System.Drawing.Color.LightGray;
            this.heightmapCheckbox.Location = new System.Drawing.Point(50, 169);
            this.heightmapCheckbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.heightmapCheckbox.Name = "heightmapCheckbox";
            this.heightmapCheckbox.Size = new System.Drawing.Size(190, 24);
            this.heightmapCheckbox.TabIndex = 6;
            this.heightmapCheckbox.Text = "Dump Heightmaps";
            this.heightmapCheckbox.UseVisualStyleBackColor = true;
            // 
            // infoCheckbox
            // 
            this.infoCheckbox.AutoSize = true;
            this.infoCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoCheckbox.ForeColor = System.Drawing.Color.LightGray;
            this.infoCheckbox.Location = new System.Drawing.Point(350, 169);
            this.infoCheckbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.infoCheckbox.Name = "infoCheckbox";
            this.infoCheckbox.Size = new System.Drawing.Size(189, 24);
            this.infoCheckbox.TabIndex = 7;
            this.infoCheckbox.Text = "Dump Info as Text";
            this.infoCheckbox.UseVisualStyleBackColor = true;
            // 
            // extractButton
            // 
            this.extractButton.Location = new System.Drawing.Point(206, 228);
            this.extractButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.extractButton.Name = "extractButton";
            this.extractButton.Size = new System.Drawing.Size(177, 55);
            this.extractButton.TabIndex = 8;
            this.extractButton.Text = "Extract";
            this.extractButton.UseVisualStyleBackColor = true;
            this.extractButton.Click += new System.EventHandler(this.extractButton_Click);
            // 
            // DumpWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(610, 311);
            this.Controls.Add(this.extractButton);
            this.Controls.Add(this.infoCheckbox);
            this.Controls.Add(this.heightmapCheckbox);
            this.Controls.Add(this.ympLocButton);
            this.Controls.Add(this.ympLocFileBox);
            this.Controls.Add(this.ympLocLabel);
            this.Controls.Add(this.saveLocButton);
            this.Controls.Add(this.saveLocFileBox);
            this.Controls.Add(this.saveLocLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "DumpWindow";
            this.Text = "Goopify (Extract)";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DumpWindow_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DumpWindow_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ympLocLabel;
        private System.Windows.Forms.Label saveLocLabel;
        private System.Windows.Forms.TextBox ympLocFileBox;
        private System.Windows.Forms.Button ympLocButton;
        private System.Windows.Forms.Button saveLocButton;
        private System.Windows.Forms.TextBox saveLocFileBox;
        private System.Windows.Forms.CheckBox heightmapCheckbox;
        private System.Windows.Forms.CheckBox infoCheckbox;
        private System.Windows.Forms.Button extractButton;
    }
}