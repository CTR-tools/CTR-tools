using CTRFramework.Sound;
using System;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class InstrumentControl : UserControl
    {
        public HowlContext Context;
        private Instrument instrument;

        public Instrument Instrument
        {
            get { return instrument; }
            set
            {
                instrument = value;
                UpdateForm();
            }
        }

        public InstrumentControl()
        {
            InitializeComponent();
        }

        public void UpdateForm()
        {
            SetButtonsState(instrument != null);

            if (instrument != null)
            {
                instrumentProperties.SelectedObject = instrument;
            }
        }

        public void SetButtonsState(bool state)
        {
            exportVagButton.Enabled = state;
            exportWavButton.Enabled = state;
            wipeButton.Enabled = state;
            replaceSampleButton.Enabled = state;
            addToSfxBank.Enabled = state;
            findFreeIndexButton.Enabled = state;
            octaveUpButton.Enabled = state;
            octaveDownButton.Enabled = state;
        }

        private void replaceSampleButton_Click(object sender, EventArgs e)
        {
            if (Context is null) return;
            if (Context.howl is null) return;
            if (Instrument is null) return;

            var ofd = new OpenFileDialog();
            ofd.Filter = "PSX VAG sample file (*.vag)|*.vag";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Context.howl.ReplaceVagSample(Instrument.SampleID, ofd.FileName, Instrument);
            }
        }

        private void wipeButton_Click(object sender, EventArgs e)
        {
            if (Context is null) return;
            if (Context.howl is null) return;

            var index = Instrument.SampleID;
            Context.Samples[index].Data = new byte[0];
            Context.SpuPtrTable[index] = new SpuAddr() { Size = 0 };
        }

        private void findFreeIndexButton_Click(object sender, EventArgs e)
        {
            if (Context is null) return;
            if (Context.howl is null) return;

            int index = 0;

            do
            {
                if (Context.Samples.ContainsKey(index))
                {
                    index++;
                }
                else
                {
                    MessageBox.Show("found free index: " + index);

                    //add new empty sample with the given index 
                    Context.Samples.Add(index, new Sample() { ID = index });

                    //update sample index
                    Instrument.SampleID = (ushort)index;

                    instrumentProperties.SelectedObject = Instrument;

                    return;
                }
            }
            while (index < 2048);

            if (index >= 2048)
            {
                MessageBox.Show("Index search overflow!");
                return;
            }
        }

        private void addToSfxBank_Click(object sender, EventArgs e)
        {
            if (Context is null) return;
            if (Context.howl is null) return;

            var index = Instrument.SampleID;

            if (Context.howl.Banks.Count == 0) return;

            if (!Context.howl.Banks[0].Entries.ContainsKey(index))
            {
                Context.howl.Banks[0].Entries.Add(index, Context.Samples[index]);
            }
            else
            {
                MessageBox.Show("Already in the SFX bank!");
            }
        }

        public void Clear()
        {
            Instrument = null;
            instrumentProperties.SelectedObject = null;
        }

        private void exportWavButton_Click(object sender, EventArgs e)
        {
            if (Context is null) return;
            if (Context.howl is null) return;
            if (Instrument is null) return;

            var index = Instrument.SampleID;

            var sfd = new SaveFileDialog();
            sfd.Filter = "WAV sound file (*.wav)|*.wav";
            sfd.FileName = Instrument.Sample.Name + ".wav";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Context.Samples[index].SaveWav(sfd.FileName, Instrument.Frequency);
            }
        }

        private void exportVagButton_Click(object sender, EventArgs e)
        {
            if (Context is null) return;
            if (Context.howl is null) return;
            if (Instrument is null) return;

            var index = Instrument.SampleID;

            var sfd = new SaveFileDialog();
            sfd.Filter = "VAG sound file (*.vag)|*.vag";
            sfd.FileName = Instrument.Sample.Name + ".vag";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Context.Samples[index].SaveVag(sfd.FileName, Instrument.Frequency);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Instrument == null) return;

            Instrument.Frequency *= 2;
            instrumentProperties.Refresh();

            HowlPlayer.Play(Instrument);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Instrument == null) return;

            Instrument.Frequency /= 2;
            instrumentProperties.Refresh();

            HowlPlayer.Play(Instrument);
        }
    }
}