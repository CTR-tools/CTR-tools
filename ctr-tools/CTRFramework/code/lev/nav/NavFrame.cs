using System;
using System.Collections.Generic;
using CTRFramework.Shared;

namespace CTRFramework
{
    public class NavFrame : IRead
    {
        public Vector3s position;
        public List<short> data = new List<short>();

        public NavFrame()
        {

        }

        public NavFrame(BinaryReaderEx br)
        {
            Read(br);
        }

        public void Read(BinaryReaderEx br)
        {
            position = new Vector3s(br);
            for (int i = 0; i < 7; i++)
            {
                data.Add(br.ReadInt16());
            }
        }
    }
}