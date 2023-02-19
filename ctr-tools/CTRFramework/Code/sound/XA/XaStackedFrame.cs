using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTRFramework.Sound
{
    public class XaStackedFrame
    {
        private byte[] header;
        private byte[] data;

        public XaStackedFrame() { }

        public XaStackedFrame(BinaryReaderEx br) => Read(br);

        public static XaStackedFrame FromReader(BinaryReaderEx br) => new XaStackedFrame(br);

        public void Read(BinaryReaderEx br)
        {
            byte[] top = br.ReadBytes(4);
            header = br.ReadBytes(8);
            byte[] bottom = br.ReadBytes(4);
            data = br.ReadBytes(28*4);

            byte[] check = new byte[8];

            Array.Copy(top, 0, check, 0, 4);
            Array.Copy(bottom, 0, check, 4, 4);

            for (int i = 0; i < 8; i++)
            {
                if (header[i] != check[i])
                {
                    Console.WriteLine($"{header[i]} {check[i]} warning! at {br.BaseStream.Position.ToString("X8")}");
                    Console.ReadKey();
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var b in header)
                sb.Append(b.ToString("X2") + " ");

            return sb.ToString();
        }

        public XaFrame GetXaFrame(int channel)
        {
            var frame = new XaFrame();

            frame.shift = (byte)(header[channel] & 7);
            frame.filter = (byte)(header[channel] >> 3 & 3);

            //

            return frame;
        }
    }
}


//https://psx.arthus.net/code/XA/XA ADPCM documentation.txt
//https://trac.ffmpeg.org/ticket/6690

/*

    static int coeff1, coeff2, shift, adpcmHistory1 = 0, adpcmHistory2 = 0;

    static readonly int[] EA_XA_TABLE = new int[] {
        0,    0,
      240,    0,
      460, -208,
      392, -220,
    };

    static void DecodeSingleFrame(BinaryReader stream, BinaryWriter outbuf)
    {
      int frameInfo = stream.ReadByte();

      coeff1 = EA_XA_TABLE[((frameInfo >> 4) & 15) * 2];
      coeff2 = EA_XA_TABLE[((frameInfo >> 4) & 15) * 2 + 1];
      shift = (frameInfo & 15) + 8;

      for (int i = 0; i < 14; i++)
      {
        int sample_byte = stream.ReadByte();

        int[] nibbles = { sample_byte >> 4, sample_byte & 15 };

        foreach (int nibble in nibbles)
        {
          int sample = GetSample(nibble);

          outbuf.Write(Clamp16(sample));
        }
      }
    }

    private static int GetSample(int nibble)
    {
      int sample = ((nibble << 28 >> shift) + (coeff1 * adpcmHistory1) + (coeff2 * adpcmHistory2)) >> 8;

      adpcmHistory2 = adpcmHistory1;
      adpcmHistory1 = sample;

      return sample;
    }

    static private short Clamp16(int sample)
    {
      if (sample > 32767)
      {
        return 32767;
      }

      if (sample < -32768)
      {
        return -32768;
      }

      return (short) sample;
    }

    static int Main(string[] args)
    {
      if (args.Length != 1)
      {
        Console.WriteLine("This program takes only one argument, input file");
        return 1;
      }

      string inputFileName = args[0], outputFileName = Path.ChangeExtension(inputFileName, "raw");
      Stream inputFile = File.OpenRead(inputFileName), outputFile = File.OpenWrite(outputFileName);
      BinaryReader inputFileReader = new BinaryReader(inputFile);
      BinaryWriter outputFileWriter = new BinaryWriter(outputFile);

      while (inputFile.Length - inputFile.Position >= 15)
      {
        DecodeSingleFrame(inputFileReader, outputFileWriter);
      }

      if (inputFile.Length - inputFile.Position > 0)
      {
        Console.WriteLine("File has {0} spare bytes.", inputFile.Length - inputFile.Position);
      }

      return 0;
    }
  }
} 
*/