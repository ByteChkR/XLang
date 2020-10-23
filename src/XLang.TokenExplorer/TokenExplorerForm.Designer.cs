namespace XLang.TokenExplorer
{
    partial class TokenExplorerForm
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
            this.tvNodeView = new System.Windows.Forms.TreeView();
            this.panelSide = new System.Windows.Forms.Panel();
            this.gbContent = new System.Windows.Forms.GroupBox();
            this.panelSideTop = new System.Windows.Forms.Panel();
            this.panelCustomData = new System.Windows.Forms.Panel();
            this.rtbCustomData = new System.Windows.Forms.RichTextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.panelNodeView = new System.Windows.Forms.Panel();
            this.btnOpenLiveEditor = new System.Windows.Forms.Button();
            this.panelSide.SuspendLayout();
            this.gbContent.SuspendLayout();
            this.panelSideTop.SuspendLayout();
            this.panelCustomData.SuspendLayout();
            this.panelNodeView.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvNodeView
            // 
            this.tvNodeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvNodeView.Location = new System.Drawing.Point(0, 0);
            this.tvNodeView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tvNodeView.Name = "tvNodeView";
            this.tvNodeView.Size = new System.Drawing.Size(461, 635);
            this.tvNodeView.TabIndex = 0;
            // 
            // panelSide
            // 
            this.panelSide.Controls.Add(this.gbContent);
            this.panelSide.Controls.Add(this.panelSideTop);
            this.panelSide.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSide.Location = new System.Drawing.Point(461, 0);
            this.panelSide.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelSide.Name = "panelSide";
            this.panelSide.Size = new System.Drawing.Size(540, 635);
            this.panelSide.TabIndex = 1;
            // 
            // gbContent
            // 
            this.gbContent.Controls.Add(this.btnOpenLiveEditor);
            this.gbContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbContent.Location = new System.Drawing.Point(0, 278);
            this.gbContent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbContent.Name = "gbContent";
            this.gbContent.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbContent.Size = new System.Drawing.Size(540, 357);
            this.gbContent.TabIndex = 3;
            this.gbContent.TabStop = false;
            this.gbContent.Text = "Content";
            // 
            // panelSideTop
            // 
            this.panelSideTop.Controls.Add(this.panelCustomData);
            this.panelSideTop.Controls.Add(this.lblName);
            this.panelSideTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSideTop.Location = new System.Drawing.Point(0, 0);
            this.panelSideTop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelSideTop.Name = "panelSideTop";
            this.panelSideTop.Size = new System.Drawing.Size(540, 278);
            this.panelSideTop.TabIndex = 2;
            // 
            // panelCustomData
            // 
            this.panelCustomData.Controls.Add(this.rtbCustomData);
            this.panelCustomData.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelCustomData.Location = new System.Drawing.Point(0, 43);
            this.panelCustomData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelCustomData.Name = "panelCustomData";
            this.panelCustomData.Size = new System.Drawing.Size(540, 235);
            this.panelCustomData.TabIndex = 1;
            // 
            // rtbCustomData
            // 
            this.rtbCustomData.DetectUrls = false;
            this.rtbCustomData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbCustomData.Location = new System.Drawing.Point(0, 0);
            this.rtbCustomData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rtbCustomData.Name = "rtbCustomData";
            this.rtbCustomData.Size = new System.Drawing.Size(540, 235);
            this.rtbCustomData.TabIndex = 0;
            this.rtbCustomData.Text = "";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(4, 15);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(142, 25);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "TokenName: ";
            // 
            // panelNodeView
            // 
            this.panelNodeView.Controls.Add(this.tvNodeView);
            this.panelNodeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelNodeView.Location = new System.Drawing.Point(0, 0);
            this.panelNodeView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelNodeView.Name = "panelNodeView";
            this.panelNodeView.Size = new System.Drawing.Size(461, 635);
            this.panelNodeView.TabIndex = 2;
            // 
            // btnOpenLiveEditor
            // 
            this.btnOpenLiveEditor.Location = new System.Drawing.Point(9, 22);
            this.btnOpenLiveEditor.Name = "btnOpenLiveEditor";
            this.btnOpenLiveEditor.Size = new System.Drawing.Size(124, 23);
            this.btnOpenLiveEditor.TabIndex = 0;
            this.btnOpenLiveEditor.Text = "Write Script";
            this.btnOpenLiveEditor.UseVisualStyleBackColor = true;
            this.btnOpenLiveEditor.Click += new System.EventHandler(this.btnOpenLiveEditor_Click);
            // 
            // TokenExplorerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 635);
            this.Controls.Add(this.panelNodeView);
            this.Controls.Add(this.panelSide);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "TokenExplorerForm";
            this.Text = "Form1";
            this.panelSide.ResumeLayout(false);
            this.gbContent.ResumeLayout(false);
            this.panelSideTop.ResumeLayout(false);
            this.panelSideTop.PerformLayout();
            this.panelCustomData.ResumeLayout(false);
            this.panelNodeView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvNodeView;
        private System.Windows.Forms.Panel panelSide;
        private System.Windows.Forms.Panel panelNodeView;
        private System.Windows.Forms.GroupBox gbContent;
        private System.Windows.Forms.Panel panelSideTop;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Panel panelCustomData;
        private System.Windows.Forms.RichTextBox rtbCustomData;
        private System.Windows.Forms.Button btnOpenLiveEditor;
    }
}

