using System.Windows.Forms;

namespace CTRTools.Controls
{
    partial class CtrControl
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
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.actionLoadObj = new System.Windows.Forms.Button();
            this.actionExportObj = new System.Windows.Forms.Button();
            this.actionLoadCtr = new System.Windows.Forms.Button();
            this.actionSaveCtr = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
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
            this.groupBox1.Text = "Ctr Model";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(6, 19);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(622, 419);
            this.splitContainer1.SplitterDistance = 256;
            this.splitContainer1.TabIndex = 0;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(362, 419);
            this.propertyGrid1.TabIndex = 0;
            // 
            // actionLoadObj
            // 
            this.actionLoadObj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.actionLoadObj.Location = new System.Drawing.Point(111, 453);
            this.actionLoadObj.Name = "actionLoadObj";
            this.actionLoadObj.Size = new System.Drawing.Size(96, 24);
            this.actionLoadObj.TabIndex = 6;
            this.actionLoadObj.Text = "Import OBJ";
            this.actionLoadObj.UseVisualStyleBackColor = true;
            this.actionLoadObj.Click += new System.EventHandler(this.actionImportObj_Click);
            // 
            // actionExportObj
            // 
            this.actionExportObj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionExportObj.Location = new System.Drawing.Point(541, 453);
            this.actionExportObj.Name = "actionExportObj";
            this.actionExportObj.Size = new System.Drawing.Size(96, 24);
            this.actionExportObj.TabIndex = 8;
            this.actionExportObj.Text = "Export OBJ";
            this.actionExportObj.UseVisualStyleBackColor = true;
            this.actionExportObj.Click += new System.EventHandler(this.actionExportObj_Click);
            // 
            // actionLoadCtr
            // 
            this.actionLoadCtr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.actionLoadCtr.Location = new System.Drawing.Point(9, 453);
            this.actionLoadCtr.Name = "actionLoadCtr";
            this.actionLoadCtr.Size = new System.Drawing.Size(96, 24);
            this.actionLoadCtr.TabIndex = 10;
            this.actionLoadCtr.Text = "Load CTR";
            this.actionLoadCtr.UseVisualStyleBackColor = true;
            this.actionLoadCtr.Click += new System.EventHandler(this.actionLoadCtr_Click);
            // 
            // actionSaveCtr
            // 
            this.actionSaveCtr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionSaveCtr.Location = new System.Drawing.Point(439, 453);
            this.actionSaveCtr.Name = "actionSaveCtr";
            this.actionSaveCtr.Size = new System.Drawing.Size(96, 24);
            this.actionSaveCtr.TabIndex = 11;
            this.actionSaveCtr.Text = "Save CTR";
            this.actionSaveCtr.UseVisualStyleBackColor = true;
            this.actionSaveCtr.Click += new System.EventHandler(this.actionSaveCtr_Click);
            // 
            // CtrControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.actionSaveCtr);
            this.Controls.Add(this.actionLoadCtr);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.actionExportObj);
            this.Controls.Add(this.actionLoadObj);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "CtrControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.CtrControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.CtrControl_DragEnter);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox1;
        private Button actionLoadObj;
        private Button actionExportObj;
        private SplitContainer splitContainer1;
        private PropertyGrid propertyGrid1;
        private Button actionLoadCtr;
        private Button actionSaveCtr;
    }
}
