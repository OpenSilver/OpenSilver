namespace DotNetForHtml5.VisualStudioExtension.Editor.SplittedXamlEditor
{
    partial class EditorPane
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


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.xamlDesignerContainer = new System.Windows.Forms.Panel();
            this.xamlTextEditorContainer = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.xamlDesignerContainer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.xamlTextEditorContainer);
            this.splitContainer1.Size = new System.Drawing.Size(505, 424);
            this.splitContainer1.SplitterDistance = 212;
            this.splitContainer1.TabIndex = 4;
            // 
            // xamlDesignerContainer
            // 
            this.xamlDesignerContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.xamlDesignerContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.xamlDesignerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xamlDesignerContainer.Location = new System.Drawing.Point(0, 0);
            this.xamlDesignerContainer.Name = "xamlDesignerContainer";
            this.xamlDesignerContainer.Size = new System.Drawing.Size(503, 210);
            this.xamlDesignerContainer.TabIndex = 5;
            // 
            // xamlTextEditorContainer
            // 
            this.xamlTextEditorContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.xamlTextEditorContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.xamlTextEditorContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xamlTextEditorContainer.Location = new System.Drawing.Point(0, 0);
            this.xamlTextEditorContainer.Name = "xamlTextEditorContainer";
            this.xamlTextEditorContainer.Size = new System.Drawing.Size(503, 206);
            this.xamlTextEditorContainer.TabIndex = 4;
            this.xamlTextEditorContainer.Resize += new System.EventHandler(this.xamlTextEditorContainer_Resize);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Please reopen the document to refresh the view.";
            // 
            // EditorPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Name = "EditorPane";
            this.Size = new System.Drawing.Size(505, 424);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel xamlTextEditorContainer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel xamlDesignerContainer;
    }
}
