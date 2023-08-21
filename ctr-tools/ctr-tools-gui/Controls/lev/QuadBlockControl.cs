using CTRFramework;
using System;
using System.Windows.Forms;


namespace CTRTools.Controls.lev
{
    public partial class QuadBlockControl : UserControl
    {
        QuadBlock quadBlock;
        QuadBlock QuadBlock
        {
            get
            {
                return quadBlock;
            }

            set
            {
                quadBlock = value;
                UpdateUI();
            }
        }

        public QuadBlockControl()
        {
            InitializeComponent();

            checkedListBox1.Items.AddRange(Enum.GetNames(typeof(QuadFlags)));
        }

        public void UpdateUI()
        {
            if (quadBlock is null) return;
        }
    }
}
