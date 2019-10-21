using System.Collections.Generic;

namespace CTRFramework.Shared
{
    public class InstanceList<T> : List<T> where T : IRead, new()
    {
        public static List<T> ReadFrom(BinaryReaderEx br, uint pos, uint count)
        {
            List<T> list = new List<T>();

            br.Jump(pos);

            for (int i = 0; i < count; i++)
            {
                T t = new T();
                t.Read(br);
                list.Add(t);
            }

            return list;
        }
    }

    public class Instance<T> where T : IRead, new()
    {
        public static T ReadFrom(BinaryReaderEx br, uint pos)
        {
            br.Jump(pos);
            T t = new T();
            t.Read(br);
            return t;
        }
    }
}
