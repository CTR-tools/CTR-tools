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
                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".obj":
                        OBJ obj = OBJ.FromFile(filename);
                        ctr = CtrModel.FromObj(obj);
                        break;
                    case ".ply":
                        ctr = CtrModel.FromPly(filename);
                        break;
                    default:
                        MessageBox.Show("Unsupported model.");
                        break;
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
            if (ctr != null)
            {
                listBox1.Items.Clear();
                foreach (var c in ctr.Entries)
                    listBox1.Items.Add(c.name);

                propertyGrid1.SelectedObject = ctr;
            }
        }

        private void CtrControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                switch (Path.GetExtension(files[0]).ToLower())
                {
                    case ".ctr":
                    case ".dyn":
                        LoadCtr(files[0]);
                        break;

                    case ".obj":
                        LoadModel(files[0]);
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
    }
}
