using System;
using System.Collections.Generic;

namespace CTRFramework.Shared
{
    public class PtrWrap<T> where T : IRead, new()
    {
        public UIntPtr Pointer = UIntPtr.Zero;

        public PtrWrap(BinaryReaderEx br)
        {
            Pointer = br.ReadUIntPtr();
        }

        public T Get(BinaryReaderEx br)
        {
            if (Pointer == UIntPtr.Zero)
                return default(T);

            int pos = (int)br.BaseStream.Position;

            T t = Instance<T>.FromReader(br, Pointer.ToUInt32());

            br.BaseStream.Position = pos;

            return t;
        }

        public List<T> GetList(BinaryReaderEx br, uint count)
        {
            if (Pointer == UIntPtr.Zero)
                return new List<T>();

            int pos = (int)br.BaseStream.Position;

            List<T> t = InstanceList<T>.FromReader(br, Pointer.ToUInt32(), count);

            br.BaseStream.Position = pos;

            return t;
        }
    }

    public class InstanceList<T> : List<T> where T : IRead, new()
    {
        public static List<T> FromReader(BinaryReaderEx br, uint pos, uint count)
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
        public static T FromReader(BinaryReaderEx br, uint pos)
        {
            br.Jump(pos);
            T t = new T();
            t.Read(br);
            return t;
        }
    }
}
