using System;
using System.Collections.Generic;

namespace CTRFramework.Shared
{
    /// <summary>
    /// PtrWrap class is meant to wrap a C style pointer, read and return data from binaryreader and jump back to the pointer.
    /// This allows to parse some header sequentially and obtain the data it points to immediately.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PtrWrap<T> where T : IRead, new()
    {
        public UIntPtr Pointer = UIntPtr.Zero;

        public PtrWrap(UIntPtr ptr)
        {
            Pointer = ptr;
        }

        public PtrWrap(BinaryReaderEx br)
        {
            Pointer = br.ReadUIntPtr();
        }

        public T Get(BinaryReaderEx br)
        {
            if (Pointer == UIntPtr.Zero)
                return default(T);

            int pos = (int)br.Position;

            var t = Instance<T>.FromReader(br, Pointer);
            br.Jump(pos);

            return t;
        }

        public List<T> GetList(BinaryReaderEx br, uint count)
        {
            if (Pointer == UIntPtr.Zero)
                return new List<T>();

            int pos = (int)br.Position;

            var t = InstanceList<T>.FromReader(br, Pointer, count);

            br.Jump(pos);

            return t;
        }
    }

    /// <summary>
    /// Generic class, returns a list of class instances (IRead) given the address and number of entries.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InstanceList<T> where T : IRead, new()
    {
        public static List<T> FromReader(BinaryReaderEx br, UIntPtr pos, uint count)
        {
            var list = new List<T>();

            br.Jump(pos);

            for (int i = 0; i < count; i++)
            {
                var t = new T();
                t.Read(br);
                list.Add(t);
            }

            return list;
        }
    }

    /// <summary>
    /// Generic class, returns a class instance (IRead) given the address.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Instance<T> where T : IRead, new()
    {
        public static T FromReader(BinaryReaderEx br, uint pos = 0)
        {
            if (pos != 0)
                br.Jump(pos);

            var t = new T();
            t.Read(br);

            return t;
        }

        public static T FromReader(BinaryReaderEx br, UIntPtr pos) => FromReader(br, pos.ToUInt32());
    }
}