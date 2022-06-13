using StreamDeckX7Plugin.SBX7.Attributes;
using StreamDeckX7Plugin.SBX7.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StreamDeckX7Plugin.SBX7.Packets
{
    [PacketId(PACKET_ID)]
    public class SpeakerConfigurationPacket : AbstractPacket
    {
        public const byte PACKET_ID = 41;

        public enum X7SpeakerConfiguration : Int32
        {
            // -(Math.Pow(2, 31))
            // This is actually Interger.MIN_VALUE in Java. This allows changing output
            // to speakers without actually knowing the current SpeakerConfiguration
            TOGGLE_TO_SPEAKER = Int32.MinValue,
            HEADPHONES = 1,
            STEREO_2_0 = 2,
            MULTI_CHANNEL_5_1 = 4
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PacketStruct
        {
            [MarshalAs(UnmanagedType.I1)]
            public GetSet GetSet;
            [MarshalAs(UnmanagedType.I4)]
            public X7SpeakerConfiguration SpeakerConfiguration;
        }

        public X7SpeakerConfiguration SpeakerConfiguration { get; set; }

        protected override void LoadPayload(byte[] payload)
        {
            GetSet = (GetSet)payload[0];
            if (payload.Length == 5 && GetSet == GetSet.Get)
            {
                SpeakerConfiguration = (X7SpeakerConfiguration)BitConverter.ToInt32(payload, 1);
            }
        }

        public override byte[] GetPayload()
        {
            if (GetSet == GetSet.Get)
            {
                return new byte[] { (byte)GetSet.Get };
            }
            else
            {
                var bytes = new byte[5];

                bytes[0] = (byte)GetSet;
                Array.Copy(BitConverter.GetBytes((Int32)SpeakerConfiguration), 0, bytes, 1, 4);

                return bytes;
            }
        }
    }
}
