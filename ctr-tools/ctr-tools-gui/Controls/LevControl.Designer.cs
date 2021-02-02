
namespace CTRTools
{
    partial class LevControl
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
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPickups = new System.Windows.Forms.TabPage();
            this.button21 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button3 = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabVertexs = new System.Windows.Forms.TabPage();
            this.button33 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button22 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button24 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabQuadz = new System.Windows.Forms.TabPage();
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
            this.tabVramz = new System.Windows.Forms.TabPage();
            this.button31 = new System.Windows.Forms.Button();
            this.button30 = new System.Windows.Forms.Button();
            this.button29 = new System.Windows.Forms.Button();
            this.button23 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button19 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.tabVisData = new System.Windows.Forms.TabPage();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.button28 = new System.Windows.Forms.Button();
            this.button27 = new System.Windows.Forms.Button();
            this.button26 = new System.Windows.Forms.Button();
            this.button32 = new System.Windows.Forms.Button();
            this.tabControl2.SuspendLayout();
            this.tabPickups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.tabVertexs.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabQuadz.SuspendLayout();
            this.tabVramz.SuspendLayout();
            this.tabVisData.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPickups);
            this.tabControl2.Controls.Add(this.tabVertexs);
            this.tabControl2.Controls.Add(this.tabQuadz);
            this.tabControl2.Controls.Add(this.tabVramz);
            this.tabControl2.Controls.Add(this.tabVisData);
            this.tabControl2.Location = new System.Drawing.Point(3, 3);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(537, 305);
            this.tabControl2.TabIndex = 15;
            // 
            // tabPickups
            // 
            this.tabPickups.Controls.Add(this.button21);
            this.tabPickups.Controls.Add(this.button4);
            this.tabPickups.Controls.Add(this.numericUpDown1);
            this.tabPickups.Controls.Add(this.button3);
            this.tabPickups.Controls.Add(this.trackBar1);
            this.tabPickups.Controls.Add(this.propertyGrid1);
            this.tabPickups.Location = new System.Drawing.Point(4, 22);
            this.tabPickups.Name = "tabPickups";
            this.tabPickups.Padding = new System.Windows.Forms.Padding(3);
            this.tabPickups.Size = new System.Drawing.Size(529, 279);
            this.tabPickups.TabIndex = 0;
            this.tabPickups.Text = "Pickup headers";
            this.tabPickups.UseVisualStyleBackColor = true;
            // 
            // button21
            // 
            this.button21.Location = new System.Drawing.Point(117, 139);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(89, 26);
            this.button21.TabIndex = 19;
            this.button21.Text = "memcard save";
            this.button21.UseVisualStyleBackColor = true;
            this.button21.Click += new System.EventHandler(this.button21_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(117, 57);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(89, 23);
            this.button4.TabIndex = 18;
            this.button4.Text = "move all down";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(117, 3);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(89, 20);
            this.numericUpDown1.TabIndex = 17;
            this.numericUpDown1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(117, 28);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(89, 23);
            this.button3.TabIndex = 16;
            this.button3.Text = "move all up";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Left;
            this.trackBar1.Location = new System.Drawing.Point(3, 3);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.trackBar1.Size = new System.Drawing.Size(45, 273);
            this.trackBar1.TabIndex = 15;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(214, 2);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(309, 226);
            this.propertyGrid1.TabIndex = 14;
            // 
            // tabVertexs
            // 
            this.tabVertexs.Controls.Add(this.button33);
            this.tabVertexs.Controls.Add(this.textBox3);
            this.tabVertexs.Controls.Add(this.button22);
            this.tabVertexs.Controls.Add(this.groupBox1);
            this.tabVertexs.Location = new System.Drawing.Point(4, 22);
            this.tabVertexs.Name = "tabVertexs";
            this.tabVertexs.Padding = new System.Windows.Forms.Padding(3);
            this.tabVertexs.Size = new System.Drawing.Size(529, 279);
            this.tabVertexs.TabIndex = 1;
            this.tabVertexs.Text = "Vertex array";
            this.tabVertexs.UseVisualStyleBackColor = true;
            // 
            // button33
            // 
            this.button33.Location = new System.Drawing.Point(15, 214);
            this.button33.Name = "button33";
            this.button33.Size = new System.Drawing.Size(234, 23);
            this.button33.TabIndex = 12;
            this.button33.Text = "Mirror X";
            this.button33.UseVisualStyleBackColor = true;
            this.button33.Click += new System.EventHandler(this.button33_Click);
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Location = new System.Drawing.Point(268, 6);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(255, 267);
            this.textBox3.TabIndex = 11;
            // 
            // button22
            // 
            this.button22.Location = new System.Drawing.Point(15, 185);
            this.button22.Name = "button22";
            this.button22.Size = new System.Drawing.Size(234, 23);
            this.button22.TabIndex = 10;
            this.button22.Text = "Randomize vertex anim";
            this.button22.UseVisualStyleBackColor = true;
            this.button22.Click += new System.EventHandler(this.button22_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button24);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 173);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Vertex colors";
            // 
            // button24
            // 
            this.button24.Location = new System.Drawing.Point(9, 43);
            this.button24.Name = "button24";
            this.button24.Size = new System.Drawing.Size(75, 23);
            this.button24.TabIndex = 12;
            this.button24.Text = "button1";
            this.button24.UseVisualStyleBackColor = true;
            this.button24.Click += new System.EventHandler(this.button24_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(6, 138);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(156, 23);
            this.button7.TabIndex = 12;
            this.button7.Text = "morph color = normal color";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Single color mode";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Night mode (click once)";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(6, 93);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 9;
            this.button6.Text = "Darken";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(87, 43);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 8;
            this.button5.Text = "morph color";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(168, 43);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "apply";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabQuadz
            // 
            this.tabQuadz.Controls.Add(this.button25);
            this.tabQuadz.Controls.Add(this.button20);
            this.tabQuadz.Controls.Add(this.button17);
            this.tabQuadz.Controls.Add(this.button16);
            this.tabQuadz.Controls.Add(this.button15);
            this.tabQuadz.Controls.Add(this.button14);
            this.tabQuadz.Controls.Add(this.button13);
            this.tabQuadz.Controls.Add(this.button12);
            this.tabQuadz.Controls.Add(this.button11);
            this.tabQuadz.Controls.Add(this.button10);
            this.tabQuadz.Controls.Add(this.button9);
            this.tabQuadz.Controls.Add(this.checkedListBox1);
            this.tabQuadz.Location = new System.Drawing.Point(4, 22);
            this.tabQuadz.Name = "tabQuadz";
            this.tabQuadz.Size = new System.Drawing.Size(529, 279);
            this.tabQuadz.TabIndex = 2;
            this.tabQuadz.Text = "Quad Blocks";
            this.tabQuadz.UseVisualStyleBackColor = true;
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
            this.button9.Location = new System.Drawing.Point(3, 208);
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
            this.checkedListBox1.Size = new System.Drawing.Size(120, 199);
            this.checkedListBox1.TabIndex = 14;
            // 
            // tabVramz
            // 
            this.tabVramz.Controls.Add(this.button31);
            this.tabVramz.Controls.Add(this.button30);
            this.tabVramz.Controls.Add(this.button29);
            this.tabVramz.Controls.Add(this.button23);
            this.tabVramz.Controls.Add(this.textBox2);
            this.tabVramz.Controls.Add(this.button19);
            this.tabVramz.Controls.Add(this.button18);
            this.tabVramz.Controls.Add(this.button8);
            this.tabVramz.Location = new System.Drawing.Point(4, 22);
            this.tabVramz.Name = "tabVramz";
            this.tabVramz.Size = new System.Drawing.Size(529, 279);
            this.tabVramz.TabIndex = 3;
            this.tabVramz.Text = "VRAM";
            this.tabVramz.UseVisualStyleBackColor = true;
            // 
            // button31
            // 
            this.button31.Location = new System.Drawing.Point(3, 234);
            this.button31.Name = "button31";
            this.button31.Size = new System.Drawing.Size(75, 23);
            this.button31.TabIndex = 27;
            this.button31.Text = "button31";
            this.button31.UseVisualStyleBackColor = true;
            this.button31.Click += new System.EventHandler(this.button31_Click);
            // 
            // button30
            // 
            this.button30.Location = new System.Drawing.Point(3, 205);
            this.button30.Name = "button30";
            this.button30.Size = new System.Drawing.Size(75, 23);
            this.button30.TabIndex = 26;
            this.button30.Text = "button30";
            this.button30.UseVisualStyleBackColor = true;
            this.button30.Click += new System.EventHandler(this.button30_Click);
            // 
            // button29
            // 
            this.button29.Location = new System.Drawing.Point(3, 176);
            this.button29.Name = "button29";
            this.button29.Size = new System.Drawing.Size(75, 23);
            this.button29.TabIndex = 25;
            this.button29.Text = "button29";
            this.button29.UseVisualStyleBackColor = true;
            // 
            // button23
            // 
            this.button23.Location = new System.Drawing.Point(3, 106);
            this.button23.Name = "button23";
            this.button23.Size = new System.Drawing.Size(117, 23);
            this.button23.TabIndex = 24;
            this.button23.Text = "export mpk textures";
            this.button23.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(131, 5);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(414, 303);
            this.textBox2.TabIndex = 23;
            // 
            // button19
            // 
            this.button19.Location = new System.Drawing.Point(3, 77);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(117, 23);
            this.button19.TabIndex = 22;
            this.button19.Text = "Load LNG and EXE";
            this.button19.UseVisualStyleBackColor = true;
            this.button19.Click += new System.EventHandler(this.button19_Click);
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(3, 32);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(117, 39);
            this.button18.TabIndex = 21;
            this.button18.Text = "export UI textures (requires ui_map file)";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.button18_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(3, 3);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(117, 23);
            this.button8.TabIndex = 20;
            this.button8.Text = "convert vram";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // tabVisData
            // 
            this.tabVisData.Controls.Add(this.checkedListBox2);
            this.tabVisData.Controls.Add(this.button28);
            this.tabVisData.Location = new System.Drawing.Point(4, 22);
            this.tabVisData.Name = "tabVisData";
            this.tabVisData.Size = new System.Drawing.Size(529, 279);
            this.tabVisData.TabIndex = 4;
            this.tabVisData.Text = "VisData";
            this.tabVisData.UseVisualStyleBackColor = true;
            // 
            // checkedListBox2
            // 
            this.checkedListBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.Location = new System.Drawing.Point(3, 3);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.Size = new System.Drawing.Size(120, 244);
            this.checkedListBox2.TabIndex = 29;
            // 
            // button28
            // 
            this.button28.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button28.Location = new System.Drawing.Point(3, 252);
            this.button28.Name = "button28";
            this.button28.Size = new System.Drawing.Size(120, 24);
            this.button28.TabIndex = 28;
            this.button28.Text = "set VisData Flags";
            this.button28.UseVisualStyleBackColor = true;
            this.button28.Click += new System.EventHandler(this.button28_Click);
            // 
            // button27
            // 
            this.button27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button27.Location = new System.Drawing.Point(420, 314);
            this.button27.Name = "button27";
            this.button27.Size = new System.Drawing.Size(116, 26);
            this.button27.TabIndex = 21;
            this.button27.Text = "Save LEV";
            this.button27.UseVisualStyleBackColor = true;
            this.button27.Click += new System.EventHandler(this.button27_Click);
            // 
            // button26
            // 
            this.button26.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button26.Location = new System.Drawing.Point(298, 314);
            this.button26.Name = "button26";
            this.button26.Size = new System.Drawing.Size(116, 26);
            this.button26.TabIndex = 20;
            this.button26.Text = "Load LEV";
            this.button26.UseVisualStyleBackColor = true;
            this.button26.Click += new System.EventHandler(this.button26_Click);
            // 
            // button32
            // 
            this.button32.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button32.Location = new System.Drawing.Point(176, 314);
            this.button32.Name = "button32";
            this.button32.Size = new System.Drawing.Size(116, 26);
            this.button32.TabIndex = 20;
            this.button32.Text = "Restore LEV";
            this.button32.UseVisualStyleBackColor = true;
            this.button32.Click += new System.EventHandler(this.button32_Click);
            // 
            // LevControl
            // 
            this.Controls.Add(this.button32);
            this.Controls.Add(this.button26);
            this.Controls.Add(this.button27);
            this.Controls.Add(this.tabControl2);
            this.Name = "LevControl";
            this.Size = new System.Drawing.Size(543, 343);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.LevControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.LevControl_DragEnter);
            this.tabControl2.ResumeLayout(false);
            this.tabPickups.ResumeLayout(false);
            this.tabPickups.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.tabVertexs.ResumeLayout(false);
            this.tabVertexs.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabQuadz.ResumeLayout(false);
            this.tabVramz.ResumeLayout(false);
            this.tabVramz.PerformLayout();
            this.tabVisData.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button actionPack;
        private System.Windows.Forms.Button actionExtract;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox pathFile;
        private System.Windows.Forms.TextBox pathFolder;
        private System.Windows.Forms.Button actionBrowseFile;
        private System.Windows.Forms.Button actionBrowseFolder;
        private System.Windows.Forms.Button actionOpenFolder;
        private System.Windows.Forms.CheckBox optionTexMed;
        private System.Windows.Forms.CheckBox optionTexLow;
        private System.Windows.Forms.CheckBox optionDebugVram;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox optionTexHigh;
        private System.Windows.Forms.Button actionRestore;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPickups;
        private System.Windows.Forms.Button button21;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TabPage tabVertexs;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button22;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabPage tabQuadz;
        private System.Windows.Forms.Button button25;
        private System.Windows.Forms.Button button20;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.TabPage tabVramz;
        private System.Windows.Forms.Button button31;
        private System.Windows.Forms.Button button30;
        private System.Windows.Forms.Button button29;
        private System.Windows.Forms.Button button23;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button24;
        private System.Windows.Forms.Button button27;
        private System.Windows.Forms.Button button26;
        private System.Windows.Forms.Button button32;
        private System.Windows.Forms.TabPage tabVisData;
        private System.Windows.Forms.CheckedListBox checkedListBox2;
        private System.Windows.Forms.Button button28;
        private System.Windows.Forms.Button button33;
    }
}
