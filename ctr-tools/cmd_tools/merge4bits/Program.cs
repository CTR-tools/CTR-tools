using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace merge4bits
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //check for 2 arguments

            if (args.Length != 2)
            {
                Console.WriteLine("Merges two 2bpp images into 4bpp");
                Console.WriteLine("Expects two image filenames as the input");
                return;
            }


            //check if files provided exist

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"File not found: {args[0]}");
                return;
            }

            if (!File.Exists(args[1]))
            {
                Console.WriteLine($"File not found: {args[1]}");
                return;
            }


            //read bitmaps
            //catch exception, maybe permissions problem, etc etc.

            Bitmap src1;
            Bitmap src2;

            try
            {
                src1 = (Bitmap)Bitmap.FromFile(args[0]);
                src2 = (Bitmap)Bitmap.FromFile(args[1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read bitmaps...\r\n{ex.Message}");
                return;
            }


            //check if size is same

            if (src1.Width != src2.Width || src1.Height != src2.Height)
            {
                Console.WriteLine($"Size mismatch, please make sure sizes are same! {src1.Width}x{src1.Height} vs {src2.Width}x{src2.Height}");
                return;
            }

            Console.WriteLine($"Image size: {src1.Width}x{src1.Height}");


            //start benchmark

            Stopwatch sw = new Stopwatch();
            sw.Start();


            //get palettes of both images

            var pal1 = GetColors(src1);
            var pal2 = GetColors(src2);

            Console.WriteLine($"Image 1: {pal1.Count} colors");
            Console.WriteLine($"Image 2: {pal2.Count} colors");


            //only accept less than 4 colors 

            if (pal1.Count > 4 || pal2.Count > 4)
            {
                Console.WriteLine("Make sure your images only use 4 colors!");
                return;
            }


            // generate new index data

            var newdata = new byte[src1.Width * src1.Height];

            for (int j = 0; j < src1.Height; j++)
                for (int i = 0; i < src1.Width; i++)
                    //naive getpixel approach, can speedup by using bitmapdata here
                    newdata[i + j * src1.Width] = (byte)(pal1.IndexOf(src1.GetPixel(i, j)) * 4 + pal2.IndexOf(src2.GetPixel(i, j)));

            //copy new index data to bitmap

            var result = new Bitmap(src1.Width, src1.Height, PixelFormat.Format8bppIndexed);
            var rect = new Rectangle(0, 0, src1.Width, src1.Height);
            var bmpdata = result.LockBits(rect, ImageLockMode.ReadWrite, result.PixelFormat);
            Marshal.Copy(newdata, 0, bmpdata.Scan0, newdata.Length);
            result.UnlockBits(bmpdata);


            //bake src1 new palette

            var palette1 = result.Palette;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    palette1.Entries[i + j * 4] = pal1[i];


            //bake src2 new palette

            var palette2 = result.Palette;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    palette2.Entries[i * 4 + j] = pal2[i];


            //bake grayscale palette

            var grayscale = result.Palette;

            for (int i = 0; i < 16; i++)
                grayscale.Entries[i] = Color.FromArgb(255, (byte)(i / 16f * 255f), (byte)(i / 16f * 255f), (byte)(i / 16f * 255f));

            try
            {
                //save images

                result.Palette = palette1;
                result.Save("merged_result_pal1.png");

                result.Palette = palette2;
                result.Save("merged_result_pal2.png");

                result.Palette = grayscale;
                result.Save("merged_result_gray.png");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save: {ex.Message}");
            }

            sw.Stop();

            Console.WriteLine($"Spent: {sw.ElapsedMilliseconds}ms");
        }

        private static List<Color> GetColors(Bitmap bitmap)
        {
            var palette = new List<Color>();

            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var color = bitmap.GetPixel(i, j);

                    if (!palette.Contains(color))
                        palette.Add(color);
                }

            return palette;
        }
    }
}