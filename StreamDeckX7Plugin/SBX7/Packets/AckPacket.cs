using StreamDeckX7Plugin.SBX7.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamDeckX7Plugin.SBX7.Packets
{
    [PacketId(PACKET_ID)]
    public class AckPacket : AbstractPacket
    {
        public const byte PACKET_ID = 2;

        protected override void LoadPayload(byte[] payload)
        {
            // Do nothing
        }

        public override byte[] GetPayload()
        {
            throw new NotImplementedException();
        }
    }
}
