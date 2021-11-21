using System.Windows.Forms;

namespace CTRTools.Controls
{
    partial class BigFileControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.fileTree = new System.Windows.Forms.TreeView();
            this.fileInfo = new System.Windows.Forms.TextBox();
            this.actionLoadBig = new System.Windows.Forms.Button();
            this.actionExportAll = new System.Windows.Forms.Button();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.bigLoader = new System.ComponentModel.BackgroundWorker();
            this.expandAll = new System.Windows.Forms.Button();
            this.bigVersion = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.splitContainer1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(634, 444);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Big File";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(6, 19);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.fileTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.fileInfo);
            this.splitContainer1.Size = new System.Drawing.Size(622, 419);
            this.splitContainer1.SplitterDistance = 258;
            this.splitContainer1.TabIndex = 0;
            // 
            // fileTree
            // 
            this.fileTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fileTree.Location = new System.Drawing.Point(0, 0);
            this.fileTree.Name = "fileTree";
            this.fileTree.Size = new System.Drawing.Size(258, 419);
            this.fileTree.TabIndex = 5;
            this.fileTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.fileTree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.fileTree_MouseClick);
            // 
            // fileInfo
            // 
            this.fileInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileInfo.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fileInfo.Location = new System.Drawing.Point(0, 0);
            this.fileInfo.Multiline = true;
            this.fileInfo.Name = "fileInfo";
            this.fileInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.fileInfo.Size = new System.Drawing.Size(360, 419);
            this.fileInfo.TabIndex = 7;
            this.fileInfo.WordWrap = false;
            // 
            // actionLoadBig
            // 
            this.actionLoadBig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionLoadBig.Location = new System.Drawing.Point(439, 453);
            this.actionLoadBig.Name = "actionLoadBig";
            this.actionLoadBig.Size = new System.Drawing.Size(96, 24);
            this.actionLoadBig.TabIndex = 6;
            this.actionLoadBig.Text = "Load BIG";
            this.actionLoadBig.UseVisualStyleBackColor = true;
            this.actionLoadBig.Click += new System.EventHandler(this.actionLoadBig_Click);
            // 
            // actionExportAll
            // 
            this.actionExportAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionExportAll.Location = new System.Drawing.Point(541, 453);
            this.actionExportAll.Name = "actionExportAll";
            this.actionExportAll.Size = new System.Drawing.Size(96, 24);
            this.actionExportAll.TabIndex = 8;
            this.actionExportAll.Text = "Export";
            this.actionExportAll.UseVisualStyleBackColor = true;
            this.actionExportAll.Click += new System.EventHandler(this.actionExportAll_Click);
            // 
            // ofd
            // 
            this.ofd.Filter = "Crash Team Racing BIG file (*.big)|*.big";
            // 
            // bigLoader
            // 
            this.bigLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bigLoader_DoWork);
            this.bigLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bigLoader_RunWorkerCompleted);
            // 
            // expandAll
            // 
            this.expandAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.expandAll.Location = new System.Drawing.Point(3, 453);
            this.expandAll.Name = "expandAll";
            this.expandAll.Size = new System.Drawing.Size(96, 24);
            this.expandAll.TabIndex = 10;
            this.expandAll.Text = "Expand";
            this.expandAll.UseVisualStyleBackColor = true;
            this.expandAll.Click += new System.EventHandler(this.expandAll_Click);
            // 
            // bigVersion
            // 
            this.bigVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bigVersion.AutoSize = true;
            this.bigVersion.Location = new System.Drawing.Point(105, 459);
            this.bigVersion.Name = "bigVersion";
            this.bigVersion.Size = new System.Drawing.Size(0, 13);
            this.bigVersion.TabIndex = 11;
            // 
            // BigFileControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bigVersion);
            this.Controls.Add(this.expandAll);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.actionExportAll);
            this.Controls.Add(this.actionLoadBig);
            this.DoubleBuffered = true;
            this.Name = "BigFileControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.BigFileControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.BigFileControl_DragEnter);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox groupBox1;
        private TreeView fileTree;
        private TextBox fileInfo;
        private Button actionLoadBig;
        private Button actionExportAll;
        private SplitContainer splitContainer1;
        private OpenFileDialog ofd;
        private FolderBrowserDialog fbd;
        private System.ComponentModel.BackgroundWorker bigLoader;
        private Button expandAll;
        private Label bigVersion;
    }
}
