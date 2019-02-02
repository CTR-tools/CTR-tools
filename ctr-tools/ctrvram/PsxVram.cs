using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApplication1
{
    enum BitMode
    {
        Indexed2 = 2,
        Indexed4 = 4,
        Indexed8 = 8,
        Colored16 = 16,
        Colored24 = 24,
        Colored32 = 32
    }

    class PsxVram
    {
        public Tim tim = new Tim(new Rectangle(0, 0, 1024, 512));
        public string fn = "";

        private BitMode bitMode;
        public BitMode BitMode
        {
            get { return bitMode; }
            set { bitMode = value; }
        }

        public void WriteTim(Tim t)
        {
            int pX = 0;
            int pY = 0;

            foreach (ushort u in t.data)
            {
                //holy cow
                tim.data[tim.region.Width * (t.region.Y + pY) + t.region.X + pX] = u;

                pX++;

                if (pX >= t.region.Width)
                {
                    pX = 0;
                    pY++;
                }
            }
        }

        public static Color Convert16(ushort col, bool useAlpha)
        {
            byte r = (byte)(((col >> 0) & 0x1F) << 3);
            byte g = (byte)(((col >> 5) & 0x1F) << 3);
            byte b = (byte)(((col >> 10) & 0x1F) << 3);
            byte a = (byte)((col >> 15) * 255);

            return Color.FromArgb((useAlpha ? a : 255), r, g, b);
        }

        public Bitmap ToBitmap()
        {
            Bitmap bmp = tim.ToBitmap();

            Graphics g = Graphics.FromImage(bmp);

            g.FillRectangle(Brushes.Blue, new Rectangle(0, 0, 512, 240));
            g.FillRectangle(Brushes.Blue, new Rectangle(0, tim.region.Height - 240, 512, 240));

            g.DrawString("Screen Buffer 1", new Font("Courier New", 10), Brushes.White, new Point(10, 10));
            g.DrawString("Screen Buffer 2", new Font("Courier New", 10), Brushes.White, new Point(10, tim.region.Height - 230));

            return bmp;
        }


        public void Clear()
        {
            tim.data = new ushort[1024*512];
        }

        public static int[,] RotateMatrix(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);

            int[,] ret = new int[m, n];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < m; ++j)
                {
                    ret[i, j] = matrix[j, i];
                }
            }

            return ret;
        }

    }


}
