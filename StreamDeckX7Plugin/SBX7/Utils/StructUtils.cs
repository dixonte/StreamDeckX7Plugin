using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StreamDeckX7Plugin.SBX7.Utils
{
    public class StructUtils
    {
        public T BytesToStruct<T>(byte[] bytes)
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T struct_;
            try
            {
                struct_ = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }

            return struct_;
        }

        public byte[] StructToByptes<T>(T struct_)
        {
            var bytes = new byte[Marshal.SizeOf<T>()];

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(struct_, handle.AddrOfPinnedObject(), false);
            }
            finally
            {
                handle.Free();
            }

            return bytes;
        }
    }
}
