using CTRFramework.Sound;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class SampleControl : UserControl
    {
        public HowlContext Context;
        private Instrument instrument;
        public Instrument Instrument
        {
            get { return instrument; }
            set { 
                instrument = value;
                UpdateForm(); 
            }
        }

        public SampleControl()
        {
            InitializeComponent();
        }

        public void UpdateForm()
        {
            instrumentProperties.SelectedObject = instrument;
        }
    }
}