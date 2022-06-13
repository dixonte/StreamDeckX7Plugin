using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamDeckX7Plugin.SBX7.Attributes
{
    public class PacketIdAttribute : Attribute
    {
        public byte PacketId { get; private set; }
        public PacketIdAttribute(byte PacketId) : base()
        {
            this.PacketId = PacketId;
        }
    }
}
