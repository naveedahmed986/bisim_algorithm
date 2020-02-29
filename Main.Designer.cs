namespace Bisimulation_Desktop
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.pathLabel = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.pathLabel2 = new System.Windows.Forms.Label();
            this.parseSelection = new System.Windows.Forms.ComboBox();
            this.picLoader = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picLoader)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 118);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(1248, 821);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 9);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(261, 32);
            this.button2.TabIndex = 2;
            this.button2.Text = "Browse (first model)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.browse_Button_Click);
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Location = new System.Drawing.Point(280, 17);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(0, 17);
            this.pathLabel.TabIndex = 3;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1029, 17);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(231, 95);
            this.button3.TabIndex = 4;
            this.button3.Text = "Transform Model";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.model_transform_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(13, 51);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(261, 30);
            this.button4.TabIndex = 5;
            this.button4.Text = "Browse (second model)";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.browse_button2_Click);
            // 
            // pathLabel2
            // 
            this.pathLabel2.AutoSize = true;
            this.pathLabel2.Location = new System.Drawing.Point(280, 58);
            this.pathLabel2.Name = "pathLabel2";
            this.pathLabel2.Size = new System.Drawing.Size(0, 17);
            this.pathLabel2.TabIndex = 6;
            // 
            // parseSelection
            // 
            this.parseSelection.FormattingEnabled = true;
            this.parseSelection.Location = new System.Drawing.Point(12, 88);
            this.parseSelection.Name = "parseSelection";
            this.parseSelection.Size = new System.Drawing.Size(261, 24);
            this.parseSelection.TabIndex = 7;
            this.parseSelection.SelectedIndexChanged += new System.EventHandler(this.parseSelection_SelectedIndexChange);
            // 
            // picLoader
            // 
            this.picLoader.Image = ((System.Drawing.Image)(resources.GetObject("picLoader.Image")));
            this.picLoader.InitialImage = ((System.Drawing.Image)(resources.GetObject("picLoader.InitialImage")));
            this.picLoader.Location = new System.Drawing.Point(591, 129);
            this.picLoader.Name = "picLoader";
            this.picLoader.Size = new System.Drawing.Size(57, 55);
            this.picLoader.TabIndex = 8;
            this.picLoader.TabStop = false;
            this.picLoader.Visible = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1272, 951);
            this.Controls.Add(this.picLoader);
            this.Controls.Add(this.parseSelection);
            this.Controls.Add(this.pathLabel2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Main";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLoader)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label pathLabel2;
        private System.Windows.Forms.ComboBox parseSelection;
        private System.Windows.Forms.PictureBox picLoader;
    }
}

