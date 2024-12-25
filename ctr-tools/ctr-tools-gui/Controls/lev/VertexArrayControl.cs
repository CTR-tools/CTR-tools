using CTRFramework;
using CTRFramework.Shared;
using System;
using System.Windows.Forms;

namespace CTRTools.Controls
{
    public partial class VertexArrayControl : UserControl
    {
        public CtrScene Scene;

        public VertexArrayControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void applyColorsButton_Click(object sender, EventArgs e)
        {
            foreach (var vertex in Scene.verts)
            {
                vertex.SetColor(new Vector4b(setMainColorButton.BackColor), Vcolor.Default);
                vertex.SetColor(new Vector4b(setMorphColorButton.BackColor), Vcolor.Morph);
            }
        }

        private void setMainColorButton_Click(object sender, EventArgs e)
        {
            if (cd.ShowDialog() == DialogResult.OK)
                setMainColorButton.BackColor = cd.Color;
        }

        private void setMorphColorButton_Click(object sender, EventArgs e)
        {
            if (cd.ShowDialog() == DialogResult.OK)
                setMorphColorButton.BackColor = cd.Color;
        }

        private void darkenButton_Click(object sender, EventArgs e)
        {
            float red = redSlider.Value / 255f;
            float green = greenSlider.Value / 255f;
            float blue = blueSlider.Value / 255f;

            foreach (var vertex in Scene.verts)
            {
                vertex.Color.Scale(red, green, blue, 1f);
                vertex.MorphColor.Scale(red, green, blue, 1f);
            }
        }

        private void mainToMorphButton_Click(object sender, EventArgs e)
        {
            foreach (var vertex in Scene.verts)
                vertex.MorphColor = vertex.Color;
        }

        private void rainbowButton_Click(object sender, EventArgs e)
        {
            foreach (var vertex in Scene.verts)
            {
                vertex.Color = new Vector4b((byte)Helpers.Random.Next(255), (byte)Helpers.Random.Next(255), (byte)Helpers.Random.Next(255), 0);
                vertex.MorphColor = vertex.Color;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Light map PNG image (*.png)|*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Scene.ApplyLightMap(ofd.FileName);
            }
        }
    }
}