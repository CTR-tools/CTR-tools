using CTRFramework;
using CTRFramework.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ThreeDeeBear.Models.Ply
{
    public enum PlyFaceParseMode
    {
        VertexCountVertexIndex // todo: see if other modes exist
    }

    public enum PlyFormat
    {
        Ascii,
        BinaryBigEndian,
        BinaryLittleEndian,
        Unknown
    }

    public class PlyHeader
    {
        public PlyFormat Format;
        public int VertexCount;
        public int FaceCount;
        public int XIndex;
        public int YIndex;
        public int ZIndex;
        public int? RedIndex;
        public int? GreenIndex;
        public int? BlueIndex;
        public int? AlphaIndex;
        public PlyFaceParseMode FaceParseMode;
        public List<string> RawHeader;

        public PlyHeader()
        {

        }

        public PlyHeader(List<string> headerUnparsed)
        {
            Format = GetFormat(headerUnparsed.FirstOrDefault(x => x.Contains("format")).Split(' ')[1]);
            var elementVertexIndex = headerUnparsed.IndexOf(headerUnparsed.FirstOrDefault(x => x.Contains("element vertex")));
            VertexCount = Convert.ToInt32(headerUnparsed[elementVertexIndex].Split(' ')[2]);
            var elementFaceIndex = headerUnparsed.IndexOf(headerUnparsed.FirstOrDefault(x => x.Contains("element face")));
            FaceCount = Convert.ToInt32(headerUnparsed[elementFaceIndex].Split(' ')[2]);
            SetVertexProperties(GetProperties(headerUnparsed, elementVertexIndex));
            SetFaceProperties(GetProperties(headerUnparsed, elementFaceIndex));
            RawHeader = headerUnparsed;
        }

        private PlyFormat GetFormat(string formatLine)
        {
            switch (formatLine)
            {
                case "binary_little_endian":
                    return PlyFormat.BinaryLittleEndian;
                case "binary_big_endian":
                    return PlyFormat.BinaryBigEndian;
                case "ascii":
                    return PlyFormat.Ascii;
                default:
                    return PlyFormat.Unknown;
            }
        }

        private void SetVertexProperties(IList<string> properties)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                var split = properties[i].Split(' ');
                var propertyType = split.Last();
                switch (propertyType)
                {
                    case "x":
                        XIndex = i;
                        break;
                    case "y":
                        YIndex = i;
                        break;
                    case "z":
                        ZIndex = i;
                        break;
                    case "red":
                        RedIndex = i;
                        break;
                    case "green":
                        GreenIndex = i;
                        break;
                    case "blue":
                        BlueIndex = i;
                        break;
                    case "alpha":
                        AlphaIndex = i;
                        break;
                }
            }
        }

        private void SetFaceProperties(IList<string> properties)
        {
            foreach (var property in properties)
            {
                switch (property)
                {
                    case "property list uchar int vertex_index":
                    case "property list uchar int vertex_indices":
                    case "property list uchar uint vertex_indices":
                        FaceParseMode = PlyFaceParseMode.VertexCountVertexIndex;
                        break;
                    default:
                        Helpers.Panic(this, PanicType.Warning, $"{property}\r\nUnknown face property, will not know how to read faces from the mesh");
                        break;
                }
            }
        }

        private List<string> GetProperties(IList<string> header, int elementIndex)
        {
            var properties = new List<string>();
            for (int i = elementIndex + 1; i < header.Count; i++)
            {
                var property = header[i];

                if (!property.Contains("property"))
                    break;

                properties.Add(property);
            }
            return properties;
        }
    }
}