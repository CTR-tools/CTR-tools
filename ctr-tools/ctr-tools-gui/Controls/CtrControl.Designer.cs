﻿using System.Windows.Forms;

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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.actionLoadObj = new System.Windows.Forms.Button();
            this.actionExportObj = new System.Windows.Forms.Button();
            this.actionLoadCtr = new System.Windows.Forms.Button();
            this.actionSaveCtr = new System.Windows.Forms.Button();
            this.ofdctr = new System.Windows.Forms.OpenFileDialog();
            this.ofdmdl = new System.Windows.Forms.OpenFileDialog();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.optionTwoSided = new System.Windows.Forms.CheckBox();
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
            this.groupBox1.Size = new System.Drawing.Size(628, 423);
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
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            this.splitContainer1.Panel1.Controls.Add(this.button2);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(616, 398);
            this.splitContainer1.SplitterDistance = 253;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 19);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(247, 329);
            this.listBox1.TabIndex = 7;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(175, 369);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Model info";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(3, 369);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(359, 398);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // actionLoadObj
            // 
            this.actionLoadObj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.actionLoadObj.Location = new System.Drawing.Point(105, 432);
            this.actionLoadObj.Name = "actionLoadObj";
            this.actionLoadObj.Size = new System.Drawing.Size(96, 24);
            this.actionLoadObj.TabIndex = 6;
            this.actionLoadObj.Text = "Import OBJ/PLY";
            this.actionLoadObj.UseVisualStyleBackColor = true;
            this.actionLoadObj.Click += new System.EventHandler(this.actionImportObj_Click);
            // 
            // actionExportObj
            // 
            this.actionExportObj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionExportObj.Location = new System.Drawing.Point(433, 432);
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
            this.actionLoadCtr.Location = new System.Drawing.Point(3, 432);
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
            this.actionSaveCtr.Location = new System.Drawing.Point(535, 432);
            this.actionSaveCtr.Name = "actionSaveCtr";
            this.actionSaveCtr.Size = new System.Drawing.Size(96, 24);
            this.actionSaveCtr.TabIndex = 11;
            this.actionSaveCtr.Text = "Save CTR";
            this.actionSaveCtr.UseVisualStyleBackColor = true;
            this.actionSaveCtr.Click += new System.EventHandler(this.actionSaveCtr_Click);
            // 
            // ofdctr
            // 
            this.ofdctr.Filter = "Crash Team Racing CTR file (*.ctr)|*.ctr";
            // 
            // ofdmdl
            // 
            this.ofdmdl.Filter = "All supported models (*.ply, *.obj)|*.ply;*.obj|Wavefront OBJ (*.obj)|*.obj|Stanf" +
    "ord PLY (*.ply)|*.ply";
            // 
            // optionTwoSided
            // 
            this.optionTwoSided.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.optionTwoSided.AutoSize = true;
            this.optionTwoSided.Location = new System.Drawing.Point(207, 435);
            this.optionTwoSided.Name = "optionTwoSided";
            this.optionTwoSided.Size = new System.Drawing.Size(115, 19);
            this.optionTwoSided.TabIndex = 1;
            this.optionTwoSided.Text = "Force two sided";
            this.optionTwoSided.UseVisualStyleBackColor = true;
            // 
            // CtrControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.optionTwoSided);
            this.Controls.Add(this.actionSaveCtr);
            this.Controls.Add(this.actionLoadCtr);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.actionExportObj);
            this.Controls.Add(this.actionLoadObj);
            this.DoubleBuffered = true;
            this.Name = "CtrControl";
            this.Size = new System.Drawing.Size(634, 459);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.CtrControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.CtrControl_DragEnter);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox groupBox1;
        private Button actionLoadObj;
        private Button actionExportObj;
        private SplitContainer splitContainer1;
        private Button actionLoadCtr;
        private Button actionSaveCtr;
        private OpenFileDialog ofdctr;
        private OpenFileDialog ofdmdl;
        private FolderBrowserDialog fbd;
        private CheckBox optionTwoSided;
        private PropertyGrid propertyGrid1;
        private Button button2;
        private Button button1;
        private ListBox listBox1;
    }
}
