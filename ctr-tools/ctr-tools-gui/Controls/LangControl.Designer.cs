using System.Windows.Forms;

namespace CTRTools.Controls
{
    partial class LangControl
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
            this.langBox = new System.Windows.Forms.TextBox();
            this.actionSave = new System.Windows.Forms.Button();
            this.actionLoad = new System.Windows.Forms.Button();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // langBox
            // 
            this.langBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.langBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.langBox.Location = new System.Drawing.Point(3, 3);
            this.langBox.Multiline = true;
            this.langBox.Name = "langBox";
            this.langBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.langBox.Size = new System.Drawing.Size(634, 444);
            this.langBox.TabIndex = 0;
            this.langBox.WordWrap = false;
            this.langBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.langBox_KeyDown);
            // 
            // actionSave
            // 
            this.actionSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionSave.Location = new System.Drawing.Point(541, 453);
            this.actionSave.Name = "actionSave";
            this.actionSave.Size = new System.Drawing.Size(96, 24);
            this.actionSave.TabIndex = 2;
            this.actionSave.Text = "Save LNG";
            this.actionSave.UseVisualStyleBackColor = true;
            this.actionSave.Click += new System.EventHandler(this.actionSave_Click);
            // 
            // actionLoad
            // 
            this.actionLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.actionLoad.Location = new System.Drawing.Point(3, 453);
            this.actionLoad.Name = "actionLoad";
            this.actionLoad.Size = new System.Drawing.Size(96, 24);
            this.actionLoad.TabIndex = 1;
            this.actionLoad.Text = "Load LNG";
            this.actionLoad.UseVisualStyleBackColor = true;
            this.actionLoad.Click += new System.EventHandler(this.actionLoad_Click);
            // 
            // sfd
            // 
            this.sfd.Filter = "Crash Team Racing LNG file(*.lng)| *.lng";
            // 
            // ofd
            // 
            this.ofd.Filter = "Crash Team Racing LNG file(*.lng)| *.lng";
            this.ofd.InitialDirectory = "ofd";
            // 
            // LangControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.actionLoad);
            this.Controls.Add(this.actionSave);
            this.Controls.Add(this.langBox);
            this.DoubleBuffered = true;
            this.Name = "LangControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.LangControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.LangControl_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox langBox;
        private Button actionSave;
        private Button actionLoad;
        private SaveFileDialog sfd;
        private OpenFileDialog ofd;
    }
}
