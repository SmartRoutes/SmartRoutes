namespace SmartRoutes.GraphVisualizer
{
    partial class GraphVisualizerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphVisualizerForm));
            this.OuterPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ControlsLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.LoadGraphButton = new System.Windows.Forms.Button();
            this.NodeCountLabel = new System.Windows.Forms.Label();
            this.NodeCount = new System.Windows.Forms.NumericUpDown();
            this.Scene = new ILNumerics.Drawing.ILPanel();
            this.OuterPanel.SuspendLayout();
            this.ControlsLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NodeCount)).BeginInit();
            this.SuspendLayout();
            // 
            // OuterPanel
            // 
            this.OuterPanel.ColumnCount = 1;
            this.OuterPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.OuterPanel.Controls.Add(this.Scene, 0, 0);
            this.OuterPanel.Controls.Add(this.ControlsLayout, 0, 1);
            this.OuterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OuterPanel.Location = new System.Drawing.Point(0, 0);
            this.OuterPanel.Name = "OuterPanel";
            this.OuterPanel.RowCount = 2;
            this.OuterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.OuterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.OuterPanel.Size = new System.Drawing.Size(384, 261);
            this.OuterPanel.TabIndex = 0;
            // 
            // ControlsLayout
            // 
            this.ControlsLayout.Controls.Add(this.LoadGraphButton);
            this.ControlsLayout.Controls.Add(this.NodeCountLabel);
            this.ControlsLayout.Controls.Add(this.NodeCount);
            this.ControlsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ControlsLayout.Location = new System.Drawing.Point(3, 230);
            this.ControlsLayout.Name = "ControlsLayout";
            this.ControlsLayout.Size = new System.Drawing.Size(378, 28);
            this.ControlsLayout.TabIndex = 1;
            // 
            // LoadGraphButton
            // 
            this.LoadGraphButton.Location = new System.Drawing.Point(3, 3);
            this.LoadGraphButton.Name = "LoadGraphButton";
            this.LoadGraphButton.Size = new System.Drawing.Size(151, 23);
            this.LoadGraphButton.TabIndex = 0;
            this.LoadGraphButton.Text = "Load Graph";
            this.LoadGraphButton.UseVisualStyleBackColor = true;
            this.LoadGraphButton.Click += new System.EventHandler(this.LoadGraphClick);
            // 
            // NodeCountLabel
            // 
            this.NodeCountLabel.AutoSize = true;
            this.NodeCountLabel.Location = new System.Drawing.Point(160, 8);
            this.NodeCountLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.NodeCountLabel.Name = "NodeCountLabel";
            this.NodeCountLabel.Size = new System.Drawing.Size(67, 13);
            this.NodeCountLabel.TabIndex = 1;
            this.NodeCountLabel.Text = "Node Count:";
            this.NodeCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NodeCount
            // 
            this.NodeCount.Location = new System.Drawing.Point(233, 5);
            this.NodeCount.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.NodeCount.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.NodeCount.Name = "NodeCount";
            this.NodeCount.Size = new System.Drawing.Size(100, 20);
            this.NodeCount.TabIndex = 2;
            this.NodeCount.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // Scene
            // 
            this.Scene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Scene.Driver = ILNumerics.Drawing.RendererTypes.OpenGL;
            this.Scene.Editor = null;
            this.Scene.Location = new System.Drawing.Point(2, 2);
            this.Scene.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Scene.Name = "Scene";
            this.Scene.Rectangle = ((System.Drawing.RectangleF)(resources.GetObject("Scene.Rectangle")));
            this.Scene.ShowUIControls = false;
            this.Scene.Size = new System.Drawing.Size(380, 223);
            this.Scene.TabIndex = 0;
            // 
            // GraphVisualizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.OuterPanel);
            this.Name = "GraphVisualizerForm";
            this.Text = "Graph Visualizer";
            this.OuterPanel.ResumeLayout(false);
            this.ControlsLayout.ResumeLayout(false);
            this.ControlsLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NodeCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel OuterPanel;
        private ILNumerics.Drawing.ILPanel Scene;
        private System.Windows.Forms.FlowLayoutPanel ControlsLayout;
        private System.Windows.Forms.Button LoadGraphButton;
        private System.Windows.Forms.Label NodeCountLabel;
        private System.Windows.Forms.NumericUpDown NodeCount;


    }
}

