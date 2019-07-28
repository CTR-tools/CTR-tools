using System.Collections.Generic;
using System.IO;

namespace CTRFramework.Shared
{
    public class InstanceList<T> : List<T> where T : IRead, new()
    {
        public static List<T> ReadFrom(BinaryReader br, int pos, int count)
        {
            List<T> list = new List<T>();

            br.BaseStream.Position = pos;

            for (int i = 0; i < count; i++)
            {
                T t = new T();
                t.Read(br);
                list.Add(t);
            }

            return list;
        }
    }

    public class Instance<T> where T: IRead, new()
    {
        public static T ReadFrom(BinaryReader br, int pos)
        {
            br.BaseStream.Position = pos;
            T t = new T();
            t.Read(br);
            return t;
        }
    }
}
