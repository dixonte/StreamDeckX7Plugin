using StreamDeckX7Plugin.SBX7.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamDeckX7Plugin.SBX7.Packets
{
    [PacketId(PACKET_ID)]
    public class HardwareButtonStatePacket : AbstractPacket
    {
        public const byte PACKET_ID = 38;

        internal enum X7HardwareButtons : Int32
        {
            SBX = 1,
            MUTE = 8,
            CRYSTAL_VOICE = 17,

            // Not available on X7
            VOICE = 4,
            MICROPHONE = 5,
            PHONE = 7,
            NOISE_REDUCTION = 9,

            // Back Buttons (BP = Bluetooth Player?), Not Available on X7
            BP_PLAY = 10,
            BP_PREV_TRACK = 11,
            BP_NEXT_TRACK = 12,
            BP_PREV_FOLDER = 13,
            BP_NEXT_FOLDER = 14,
            BP_PLAY_RECORDING = 15,
            BP_RECORD_RECORDING = 16,
        }

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
