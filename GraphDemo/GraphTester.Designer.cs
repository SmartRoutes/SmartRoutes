namespace SmartRoutes.GraphDemo
{
    partial class GraphTester
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
            this.ResultsBox = new System.Windows.Forms.TextBox();
            this.ResultsTextLabel = new System.Windows.Forms.Label();
            this.BuildGraphBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ResultsBox
            // 
            this.ResultsBox.Location = new System.Drawing.Point(25, 294);
            this.ResultsBox.Multiline = true;
            this.ResultsBox.Name = "ResultsBox";
            this.ResultsBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ResultsBox.Size = new System.Drawing.Size(620, 392);
            this.ResultsBox.TabIndex = 0;
            this.ResultsBox.TextChanged += new System.EventHandler(this.ResultsBox_TextChanged);
            // 
            // ResultsTextLabel
            // 
            this.ResultsTextLabel.AutoSize = true;
            this.ResultsTextLabel.Location = new System.Drawing.Point(25, 275);
            this.ResultsTextLabel.Name = "ResultsTextLabel";
            this.ResultsTextLabel.Size = new System.Drawing.Size(42, 13);
            this.ResultsTextLabel.TabIndex = 1;
            this.ResultsTextLabel.Text = "Results";
            this.ResultsTextLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // BuildGraphBtn
            // 
            this.BuildGraphBtn.Location = new System.Drawing.Point(229, 84);
            this.BuildGraphBtn.Name = "BuildGraphBtn";
            this.BuildGraphBtn.Size = new System.Drawing.Size(75, 23);
            this.BuildGraphBtn.TabIndex = 2;
            this.BuildGraphBtn.Text = "Build Graph";
            this.BuildGraphBtn.UseVisualStyleBackColor = true;
            this.BuildGraphBtn.Click += new System.EventHandler(this.BuildGraphBtn_Click);
            // 
            // GraphTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 712);
            this.Controls.Add(this.BuildGraphBtn);
            this.Controls.Add(this.ResultsTextLabel);
            this.Controls.Add(this.ResultsBox);
            this.Name = "GraphTester";
            this.Text = "GraphTester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ResultsBox;
        private System.Windows.Forms.Label ResultsTextLabel;
        private System.Windows.Forms.Button BuildGraphBtn;
    }
}