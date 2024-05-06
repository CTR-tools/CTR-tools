using CTRFramework.Audio;
using CTRTools.Controls;

using System;
using System.Collections.Generic;

using System.Windows.Forms;

namespace CTRTools
{
    public partial class InstrumentSelectionForm : Form
    {
        Howl Howl = null;
        public SpuInstrument SelectedInstrument = null;

        public InstrumentSelectionForm(Howl howl)
        {
            InitializeComponent();
            Howl = howl;

            categoryBox.SelectedIndex = 0;
        }


        private List<SpuInstrument> SelectedPool;
        private List<SpuInstrument> Cache = new List<SpuInstrument>();

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cache.Clear();

            if (categoryBox.SelectedIndex == 0)
                SelectedPool = Howl.Context.InstrumentPool;

            if (categoryBox.SelectedIndex == 1)
                SelectedPool = Howl.Context.PercussionPool;

            if (categoryBox.SelectedIndex == 2)
                SelectedPool = Howl.Context.EffectsPool;

            Filter(searchBox.Text);
        }

        private void Filter(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();

            sampleTableListBox.BeginUpdate();

            Cache.Clear();

            foreach (var entry in SelectedPool)
            {
                if (searchTerm is null || searchTerm == "")
                {
                    Cache.Add(entry);
                }
                else
                {
                    // TODO: this should use instrument name, but atm i dont generate a proper one
                    if (entry.Sample == null)
                    {
                        Cache.Add(entry);
                    }
                    else if (entry.Sample.Name.ToLower().Contains(searchTerm.ToLower()))
                    {
                        Cache.Add(entry);
                    }
                }
            }

            sampleTableListBox.Items.Clear();

            foreach (var inst in Cache)
                sampleTableListBox.Items.Add(inst.Sample == null ? "<null (fix engine)> " + inst.Name : inst.Sample.Name);

            sampleTableListBox.EndUpdate();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            Filter(searchBox.Text.Trim());
        }

        private void sampleTableListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Howl is null) return;

            if (sampleTableListBox.SelectedIndex != -1)
            {
                SelectedInstrument = Cache[sampleTableListBox.SelectedIndex];

                if (SelectedInstrument.Sample != null)
                    HowlPlayer.Play(SelectedInstrument);
            }
        }
    }
}
