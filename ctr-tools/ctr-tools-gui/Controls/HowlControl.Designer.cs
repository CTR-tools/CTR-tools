
namespace CTRTools.Controls
{
    partial class HowlControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.actionLoad = new System.Windows.Forms.Button();
            this.actionExport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // actionLoad
            // 
            this.actionLoad.Location = new System.Drawing.Point(3, 3);
            this.actionLoad.Name = "actionLoad";
            this.actionLoad.Size = new System.Drawing.Size(128, 23);
            this.actionLoad.TabIndex = 0;
            this.actionLoad.Text = "Load HOWL";
            this.actionLoad.UseVisualStyleBackColor = true;
            this.actionLoad.Click += new System.EventHandler(this.actionLoad_Click);
            // 
            // actionExport
            // 
            this.actionExport.Location = new System.Drawing.Point(3, 32);
            this.actionExport.Name = "actionExport";
            this.actionExport.Size = new System.Drawing.Size(128, 23);
            this.actionExport.TabIndex = 1;
            this.actionExport.Text = "Export CSEQ";
            this.actionExport.UseVisualStyleBackColor = true;
            this.actionExport.Click += new System.EventHandler(this.actionExport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(137, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(137, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            // 
            // ofd
            // 
            this.ofd.Filter = "Crash Team Racing HWL file (*.hwl)|*.hwl";
            // 
            // HowlControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.actionExport);
            this.Controls.Add(this.actionLoad);
            this.Name = "HowlControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HowlControl_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button actionLoad;
        private System.Windows.Forms.Button actionExport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog fbd;
        private System.Windows.Forms.OpenFileDialog ofd;
    }
}
