using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Data;
using System.Collections;

namespace CTRFramework
{

    public class CTRVRAM
    {
        public int pagesCount; //not really
        public List<MemPage> pages = new List<MemPage>();

        public void Read(BinaryReader br)
        {
            pagesCount = br.ReadInt32();

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                MemPage p = new MemPage();
                if (p.Read(br))
                    pages.Add(p);
            }
        }

        public Bitmap ToBitmap()
        {
            PsxVram pv = new PsxVram();

            foreach (MemPage p in pages)
                pv.WriteTim(p.tim);

            Bitmap bmp = pv.ToBitmap();

            int tpw = 256;
            int tph = 256;

            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 2; j++)
                {
                    Bitmap x = bmp.Clone(new Rectangle(new Point(i * tpw, j * tph), new Size(tpw, tph)), System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    x = new Bitmap(x, new Size(256, 256));
                    
                    if (i >= 8)
                        x.Save(String.Format("texpage_{0}_{1}.png", i, j));
                }

            return pv.ToBitmap();
        }
    }

    public class MemPage
    {
        public int size;
        public Tim tim;

        public bool Read(BinaryReader br)
        {
            size = br.ReadInt32();

            if (size != 0)
            {
                tim = new Tim();
                tim.Read(br);
                return true;
            }

            return false;
        }

        public Bitmap ToBitmap()
        {
            return tim.To4bitBitmap();
        }
    }






    /*

    public Bitmap ToBMP(BitMode mode, Point palpos)
    {
        int pX = 0;
        int pY = 0;

        int imgWidth = width * psxBPP / (int)mode;
        Bitmap bmp = new Bitmap(imgWidth, height);
        List<Color> pal = new List<Color>();

        switch (mode)
        {
            case BitMode.Indexed4:
                {
                    int ind = palpos.Y * (width * 2) + palpos.X * 2;

                    for (int i = 0; i < 16; i++)
                    {
                        ushort val = (ushort)((data[ind + i * 2 + 1] << 8) | data[ind + i * 2]);

                        pal.Add(Convert16(val));
                    }

                    for (int i = 0; i < data.Length; i++)
                    {
                        byte x1 = (byte)(data[i] & 0x0F);
                        byte x2 = (byte)(data[i] >> 4);

                        bmp.SetPixel(pX, pY, pal[x1]);
                        bmp.SetPixel(pX + 1, pY, pal[x2]);

                        pX += 2;

                        if (pX >= imgWidth)
                        {
                            pX = 0;
                            pY++;
                        }
                    }
                }
                break;

            case BitMode.Indexed8:
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        bmp.SetPixel(pX, pY, Color.FromArgb(data[i], data[i], data[i]));

                        pX++;

                        if (pX >= imgWidth)
                        {
                            pX = 0;
                            pY++;
                        }
                    }
                }
                break;

            case BitMode.Colored16:
                {
                        for (int i = 0; i < imgWidth * height; i++)
                        {
                            ushort val = (ushort)((data[i * 2 + 1] << 8) | data[i * 2]);

                            bmp.SetPixel(pX, pY, Convert16(val));

                            pX++;

                            if (pX >= imgWidth)
                            {
                                pX = 0;
                                pY++;
                            }
                        }
                }
                break;

            case BitMode.Colored24:
                {

                    for (int i = 0; i < imgWidth * height; i++)
                    {
                        byte r = data[i * 3 + 1];
                        byte g = data[i * 3 + 2];
                        byte b = data[i * 3 + 2];
                        byte a = 255;

                        bmp.SetPixel(pX, pY, Color.FromArgb(a, r, g, b));

                        pX++;

                        if (pX >= imgWidth)
                        {
                            pX = 0;
                            pY++;
                        }
                    }
                }
                break;

            case BitMode.Colored32:
                {

                    for (int i = 0; i < imgWidth * height; i++)
                    {
                        byte a = data[i*4];
                        byte r = data[i*4+1];
                        byte g = data[i*4+2];
                        byte b = data[i*4+3];

                        bmp.SetPixel(pX, pY, Color.FromArgb(a, r, g, b));

                        pX++;

                        if (pX >= imgWidth)
                        {
                            pX = 0;
                            pY++;
                        }
                    }
                }
                break;

            default: throw new System.Exception("unimplemented bit mode"); return null;
        }

        return bmp;
    }
     * 
     * */
}