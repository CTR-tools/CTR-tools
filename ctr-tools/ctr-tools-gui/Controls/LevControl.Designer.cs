using System.Windows.Forms;

namespace CTRTools.Controls
{
    partial class LevControl
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPickups = new System.Windows.Forms.TabPage();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.scaleButton = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.moveAllButton = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabVerts = new System.Windows.Forms.TabPage();
            this.vertexArrayControl1 = new CTRTools.Controls.VertexArrayControl();
            this.tabQuads = new System.Windows.Forms.TabPage();
            this.button25 = new System.Windows.Forms.Button();
            this.button20 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.tabVisData = new System.Windows.Forms.TabPage();
            this.button18 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.button28 = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button8 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button27 = new System.Windows.Forms.Button();
            this.button26 = new System.Windows.Forms.Button();
            this.button32 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPickups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.tabVerts.SuspendLayout();
            this.tabQuads.SuspendLayout();
            this.tabVisData.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPickups);
            this.tabControl1.Controls.Add(this.tabVerts);
            this.tabControl1.Controls.Add(this.tabQuads);
            this.tabControl1.Controls.Add(this.tabVisData);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(634, 444);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPickups
            // 
            this.tabPickups.Controls.Add(this.propertyGrid1);
            this.tabPickups.Controls.Add(this.label1);
            this.tabPickups.Controls.Add(this.trackBar2);
            this.tabPickups.Controls.Add(this.scaleButton);
            this.tabPickups.Controls.Add(this.numericUpDown1);
            this.tabPickups.Controls.Add(this.moveAllButton);
            this.tabPickups.Controls.Add(this.trackBar1);
            this.tabPickups.Location = new System.Drawing.Point(4, 22);
            this.tabPickups.Name = "tabPickups";
            this.tabPickups.Padding = new System.Windows.Forms.Padding(3);
            this.tabPickups.Size = new System.Drawing.Size(626, 418);
            this.tabPickups.TabIndex = 0;
            this.tabPickups.Text = "Pickup headers";
            this.tabPickups.UseVisualStyleBackColor = true;
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(56, 74);
            this.trackBar2.Maximum = 200;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(152, 45);
            this.trackBar2.TabIndex = 19;
            this.trackBar2.TickFrequency = 25;
            this.trackBar2.Value = 100;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // scaleButton
            // 
            this.scaleButton.Location = new System.Drawing.Point(56, 45);
            this.scaleButton.Name = "scaleButton";
            this.scaleButton.Size = new System.Drawing.Size(152, 23);
            this.scaleButton.TabIndex = 18;
            this.scaleButton.Text = "Scale by 100%";
            this.scaleButton.UseVisualStyleBackColor = true;
            this.scaleButton.Click += new System.EventHandler(this.scaleButton_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(56, 9);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(59, 20);
            this.numericUpDown1.TabIndex = 17;
            this.numericUpDown1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // moveAllButton
            // 
            this.moveAllButton.Location = new System.Drawing.Point(119, 6);
            this.moveAllButton.Name = "moveAllButton";
            this.moveAllButton.Size = new System.Drawing.Size(89, 23);
            this.moveAllButton.TabIndex = 16;
            this.moveAllButton.Text = "Move all";
            this.moveAllButton.UseVisualStyleBackColor = true;
            this.moveAllButton.Click += new System.EventHandler(this.moveAllButton_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.trackBar1.Location = new System.Drawing.Point(3, 3);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.trackBar1.Size = new System.Drawing.Size(45, 412);
            this.trackBar1.TabIndex = 15;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(214, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(409, 412);
            this.propertyGrid1.TabIndex = 14;
            // 
            // tabVerts
            // 
            this.tabVerts.Controls.Add(this.vertexArrayControl1);
            this.tabVerts.Location = new System.Drawing.Point(4, 22);
            this.tabVerts.Name = "tabVerts";
            this.tabVerts.Padding = new System.Windows.Forms.Padding(3);
            this.tabVerts.Size = new System.Drawing.Size(626, 418);
            this.tabVerts.TabIndex = 1;
            this.tabVerts.Text = "Vertex array";
            this.tabVerts.UseVisualStyleBackColor = true;
            // 
            // vertexArrayControl1
            // 
            this.vertexArrayControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vertexArrayControl1.BackColor = System.Drawing.Color.Transparent;
            this.vertexArrayControl1.Location = new System.Drawing.Point(6, 6);
            this.vertexArrayControl1.Name = "vertexArrayControl1";
            this.vertexArrayControl1.Size = new System.Drawing.Size(614, 409);
            this.vertexArrayControl1.TabIndex = 0;
            // 
            // tabQuads
            // 
            this.tabQuads.Controls.Add(this.button25);
            this.tabQuads.Controls.Add(this.button20);
            this.tabQuads.Controls.Add(this.button17);
            this.tabQuads.Controls.Add(this.button16);
            this.tabQuads.Controls.Add(this.button15);
            this.tabQuads.Controls.Add(this.button14);
            this.tabQuads.Controls.Add(this.button13);
            this.tabQuads.Controls.Add(this.button12);
            this.tabQuads.Controls.Add(this.button11);
            this.tabQuads.Controls.Add(this.button10);
            this.tabQuads.Controls.Add(this.button9);
            this.tabQuads.Controls.Add(this.checkedListBox1);
            this.tabQuads.Location = new System.Drawing.Point(4, 22);
            this.tabQuads.Name = "tabQuads";
            this.tabQuads.Size = new System.Drawing.Size(626, 418);
            this.tabQuads.TabIndex = 2;
            this.tabQuads.Text = "Quad Blocks";
            this.tabQuads.UseVisualStyleBackColor = true;
            // 
            // button25
            // 
            this.button25.Location = new System.Drawing.Point(210, 82);
            this.button25.Name = "button25";
            this.button25.Size = new System.Drawing.Size(75, 23);
            this.button25.TabIndex = 26;
            this.button25.Text = "reverse track";
            this.button25.UseVisualStyleBackColor = true;
            this.button25.Click += new System.EventHandler(this.button25_Click);
            // 
            // button20
            // 
            this.button20.Location = new System.Drawing.Point(210, 32);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(75, 44);
            this.button20.TabIndex = 25;
            this.button20.Text = "show nav data";
            this.button20.UseVisualStyleBackColor = true;
            this.button20.Click += new System.EventHandler(this.button20_Click);
            // 
            // button17
            // 
            this.button17.Location = new System.Drawing.Point(210, 3);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(75, 23);
            this.button17.TabIndex = 24;
            this.button17.Text = "midflags 2 disable";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.button17_Click);
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(291, 57);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(75, 43);
            this.button16.TabIndex = 23;
            this.button16.Text = "random weather";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(129, 305);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(75, 47);
            this.button15.TabIndex = 22;
            this.button15.Text = "disable weather";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(291, 3);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(75, 48);
            this.button14.TabIndex = 21;
            this.button14.Text = "intense weather";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(129, 153);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(75, 43);
            this.button13.TabIndex = 20;
            this.button13.Text = "randomize terrain";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(129, 111);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(75, 36);
            this.button12.TabIndex = 19;
            this.button12.Text = "list terrain bytes";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(129, 82);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(75, 23);
            this.button11.TabIndex = 18;
            this.button11.Text = "list coldata";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(129, 32);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(75, 44);
            this.button10.TabIndex = 16;
            this.button10.Text = "remove texture";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button9
            // 
            this.button9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button9.Location = new System.Drawing.Point(3, 347);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(120, 23);
            this.button9.TabIndex = 15;
            this.button9.Text = "Set quad flags";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(3, 3);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(120, 334);
            this.checkedListBox1.TabIndex = 14;
            // 
            // tabVisData
            // 
            this.tabVisData.Controls.Add(this.button18);
            this.tabVisData.Controls.Add(this.textBox2);
            this.tabVisData.Controls.Add(this.checkedListBox2);
            this.tabVisData.Controls.Add(this.button28);
            this.tabVisData.Location = new System.Drawing.Point(4, 22);
            this.tabVisData.Name = "tabVisData";
            this.tabVisData.Size = new System.Drawing.Size(626, 418);
            this.tabVisData.TabIndex = 4;
            this.tabVisData.Text = "VisData";
            this.tabVisData.UseVisualStyleBackColor = true;
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(138, 12);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(75, 23);
            this.button18.TabIndex = 33;
            this.button18.Text = "button18";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.button18_Click_1);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(129, 3);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(494, 410);
            this.textBox2.TabIndex = 32;
            // 
            // checkedListBox2
            // 
            this.checkedListBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.Location = new System.Drawing.Point(3, 3);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.Size = new System.Drawing.Size(120, 379);
            this.checkedListBox2.TabIndex = 29;
            // 
            // button28
            // 
            this.button28.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button28.Location = new System.Drawing.Point(3, 391);
            this.button28.Name = "button28";
            this.button28.Size = new System.Drawing.Size(120, 24);
            this.button28.TabIndex = 28;
            this.button28.Text = "set VisData Flags";
            this.button28.UseVisualStyleBackColor = true;
            this.button28.Click += new System.EventHandler(this.button28_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button8);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(626, 418);
            this.tabPage1.TabIndex = 5;
            this.tabPage1.Text = "skyTab";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(286, 35);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(118, 23);
            this.button8.TabIndex = 2;
            this.button8.Text = "save image";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(286, 6);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(118, 23);
            this.button4.TabIndex = 1;
            this.button4.Text = "load gradient";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(6, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(274, 212);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button27
            // 
            this.button27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button27.Location = new System.Drawing.Point(541, 453);
            this.button27.Name = "button27";
            this.button27.Size = new System.Drawing.Size(96, 24);
            this.button27.TabIndex = 21;
            this.button27.Text = "Save LEV";
            this.button27.UseVisualStyleBackColor = true;
            this.button27.Click += new System.EventHandler(this.actionSaveLev);
            // 
            // button26
            // 
            this.button26.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button26.Location = new System.Drawing.Point(3, 453);
            this.button26.Name = "button26";
            this.button26.Size = new System.Drawing.Size(96, 24);
            this.button26.TabIndex = 20;
            this.button26.Text = "Load LEV";
            this.button26.UseVisualStyleBackColor = true;
            this.button26.Click += new System.EventHandler(this.actionLoadLev);
            // 
            // button32
            // 
            this.button32.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button32.Location = new System.Drawing.Point(105, 453);
            this.button32.Name = "button32";
            this.button32.Size = new System.Drawing.Size(96, 24);
            this.button32.TabIndex = 20;
            this.button32.Text = "Restore LEV";
            this.button32.UseVisualStyleBackColor = true;
            this.button32.Click += new System.EventHandler(this.actionRestoreLev);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(439, 453);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 24);
            this.button1.TabIndex = 22;
            this.button1.Text = "Export OBJ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.actionExportObj);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 39);
            this.label1.TabIndex = 20;
            this.label1.Text = "Please note:\r\nscaling too much will cause\r\nsevere visibility/collision issues";
            // 
            // LevControl
            // 
            this.AllowDrop = true;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button32);
            this.Controls.Add(this.button26);
            this.Controls.Add(this.button27);
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.Name = "LevControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.LevControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.LevControl_DragEnter);
            this.tabControl1.ResumeLayout(false);
            this.tabPickups.ResumeLayout(false);
            this.tabPickups.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.tabVerts.ResumeLayout(false);
            this.tabQuads.ResumeLayout(false);
            this.tabVisData.ResumeLayout(false);
            this.tabVisData.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion


        private TabControl tabControl1;
        private TabPage tabPickups;
        private NumericUpDown numericUpDown1;
        private Button moveAllButton;
        private TrackBar trackBar1;
        private PropertyGrid propertyGrid1;
        private TabPage tabVerts;
        private TabPage tabQuads;
        private Button button25;
        private Button button20;
        private Button button17;
        private Button button16;
        private Button button15;
        private Button button14;
        private Button button13;
        private Button button12;
        private Button button11;
        private Button button10;
        private Button button9;
        private CheckedListBox checkedListBox1;
        private Button button27;
        private Button button26;
        private Button button32;
        private TabPage tabVisData;
        private CheckedListBox checkedListBox2;
        private Button button28;
        private TextBox textBox2;
        private Button button1;
        private TabPage tabPage1;
        private Button button4;
        private PictureBox pictureBox1;
        private Button button8;
        private Button button18;
        private VertexArrayControl vertexArrayControl1;
        private Button scaleButton;
        private TrackBar trackBar2;
        private Label label1;
    }
}
