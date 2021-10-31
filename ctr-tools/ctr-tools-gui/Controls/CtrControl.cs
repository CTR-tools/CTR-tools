using CTRFramework;
using System;
using System.IO;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class CtrControl : UserControl
    {
        CtrModel ctr;

        public CtrControl()
        {
            InitializeComponent();
        }

        public void LoadCtr(string filename)
        {
            try
            {
                ctr = CtrModel.FromFile(filename);
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadModel(string filename)
        {
            try
            {
                CtrModel model = new CtrModel();

                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".obj":
                        OBJ obj = OBJ.FromFile(filename);
                        model = CtrModel.FromObj(obj);
                        break;
                    case ".ply":
                        model = CtrModel.FromPly(filename);
                        break;
                    default:
                        MessageBox.Show("Unsupported model.");
                        break;
                }

                if (ctr != null)
                {
                    ctr.Entries.AddRange(model.Entries);
                }
                else
                {
                    ctr = model;
                }

                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void UpdateUI()
        {
            listBox1.Items.Clear();

            if (ctr != null)
            {
                foreach (var model in ctr.Entries)
                    listBox1.Items.Add(model.name);

                propertyGrid1.SelectedObject = ctr;
            }
        }

        private void CtrControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (var file in files)
            {
                switch (Path.GetExtension(file).ToLower())
                {
                    case ".ctr":
                    case ".dyn":
                        LoadCtr(file);
                        break;

                    case ".obj":
                    case ".ply":
                        LoadModel(file);
                        break;

                    default:
                        MessageBox.Show("Unsupported file.");
                        break;
                }
            }
        }
        private void CtrControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void actionImportObj_Click(object sender, EventArgs e)
        {
            if (ofdmdl.ShowDialog() == DialogResult.OK)
                LoadModel(ofdmdl.FileName);
        }

        private void actionLoadCtr_Click(object sender, EventArgs e)
        {
            if (ofdctr.ShowDialog() == DialogResult.OK)
                LoadCtr(ofdctr.FileName);
        }

        private void actionSaveCtr_Click(object sender, EventArgs e)
        {
            try
            {
                if (ctr != null)
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                        ctr.Save(fbd.SelectedPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void actionExportObj_Click(object sender, EventArgs e)
        {
            try
            {
                if (ctr != null)
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                        ctr.Export(fbd.SelectedPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void actionExportPly_Click(object sender, EventArgs e)
        {
            if (ctr != null)
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                    ctr.ExportPly(fbd.SelectedPath);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
                propertyGrid1.SelectedObject = ctr.Entries[listBox1.SelectedIndex];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ctr = null;
            propertyGrid1.SelectedObject = null;
            UpdateUI();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ctr != null)
                propertyGrid1.SelectedObject = ctr;
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            int x = listBox1.SelectedIndex;
            UpdateUI();
            listBox1.SelectedIndex = x;
        }
    }
}
