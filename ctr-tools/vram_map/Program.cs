using CTRFramework.Vram;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace vram_map
{
    enum TimData
    {
        Name = 0,
        Bpp = 1,
        PageX = 2,
        PageY = 3,
        X = 4,
        Y = 5,
        Width = 6,
        Height = 7,
        PalX = 8,
        PalY = 9
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            string[] lines = File.ReadAllLines(args[0]);

            StringBuilder sb = new StringBuilder();
            int i = 0;

            sb.AppendLine("[TIM Files]");

            foreach (var line in lines)
            {
                if (line.Trim() == "")
                    continue;

                string[] values = line.Split(',');

                string path = Path.Combine(Path.GetDirectoryName(args[0]), values[(int)TimData.Name]);

                Bitmap bmp = (Bitmap)Bitmap.FromFile(path);

                Tim tim = new Tim(new Rectangle(
                    Int32.Parse(values[(int)TimData.PageX]) * 64 + Int32.Parse(values[(int)TimData.X]) / 4,
                    Int32.Parse(values[(int)TimData.PageY]) * 256 + Int32.Parse(values[(int)TimData.Y]),
                    bmp.Width / 4, bmp.Height));

                Console.WriteLine(tim.region);

                tim.flags = 1 << 3;

                tim.clutregion = new Rectangle(
                    Int32.Parse(values[(int)TimData.PalX]) * 16,
                    Int32.Parse(values[(int)TimData.PalY]),
                    16, 1);

                tim.LoadDataFromBitmap(path);

                string timpath = Path.ChangeExtension(path, ".tim");

                tim.Write(timpath);

                sb.AppendLine($"File_{i}={timpath}");

                i++;
            }

            sb.AppendLine("[Graphics Mode]\r\nWidth = 320\r\nHeight = 256");

            File.WriteAllText("_shared.psx", sb.ToString());
        }
    }
}
