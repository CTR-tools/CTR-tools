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
            this.DoubleBuffered = true;
        }

        public void LoadCtr(string filename)
        {
            ctr = CtrModel.FromFile(filename);
            propertyGrid1.SelectedObject = ctr;
        }

        public void LoadObj(string filename)
        {
            OBJ obj = OBJ.FromFile(filename);
            ctr = CtrModel.FromObj(obj);
            propertyGrid1.SelectedObject = ctr;
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
                        LoadObj(files[0]);
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
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Wavefront OBJ (*.obj)|*.obj";

                if (ofd.ShowDialog() == DialogResult.OK)
                    LoadObj(ofd.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void actionLoadCtr_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Crash Team Racing CTR file|*.ctr";

                if (ofd.ShowDialog() == DialogResult.OK)
                    LoadCtr(ofd.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void actionSaveCtr_Click(object sender, EventArgs e)
        {
            try
            {
                if (ctr != null)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();

                    if (fbd.ShowDialog() == DialogResult.OK)
                        ctr.Write(fbd.SelectedPath);
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
                    FolderBrowserDialog fbd = new FolderBrowserDialog();

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
