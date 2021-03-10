
namespace CTRTools.Controls
{
    partial class LangControl
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
            this.langBox.Size = new System.Drawing.Size(634, 445);
            this.langBox.TabIndex = 0;
            this.langBox.WordWrap = false;
            this.langBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.langBox_KeyDown);
            // 
            // actionSave
            // 
            this.actionSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.actionSave.Location = new System.Drawing.Point(562, 454);
            this.actionSave.Name = "actionSave";
            this.actionSave.Size = new System.Drawing.Size(75, 23);
            this.actionSave.TabIndex = 1;
            this.actionSave.Text = "Save LNG";
            this.actionSave.UseVisualStyleBackColor = true;
            this.actionSave.Click += new System.EventHandler(this.actionSave_Click);
            // 
            // actionLoad
            // 
            this.actionLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.actionLoad.Location = new System.Drawing.Point(3, 454);
            this.actionLoad.Name = "actionLoad";
            this.actionLoad.Size = new System.Drawing.Size(75, 23);
            this.actionLoad.TabIndex = 2;
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

        private System.Windows.Forms.TextBox langBox;
        private System.Windows.Forms.Button actionSave;
        private System.Windows.Forms.Button actionLoad;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.OpenFileDialog ofd;
    }
}
