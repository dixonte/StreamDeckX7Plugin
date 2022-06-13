using StreamDeckX7Plugin.SBX7.Attributes;
using StreamDeckX7Plugin.SBX7.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamDeckX7Plugin.SBX7.Packets
{
    public abstract class AbstractPacket
    {
        public const Int32 STARTBYTEID = 90;

        private static Dictionary<byte, Func<AbstractPacket>> _packetConstructors;
        private static Dictionary<Type, byte> _packetTypeIds;
        static AbstractPacket()
        {
            // Compile packet list
            _packetConstructors = typeof(AbstractPacket).Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(AbstractPacket)) && t.CustomAttributes.Any(a => a.AttributeType == typeof(PacketIdAttribute)))
                .ToDictionary(
                    f => f.GetCustomAttributes(typeof(PacketIdAttribute), false).Cast<PacketIdAttribute>().FirstOrDefault().PacketId,
                    f => new Func<AbstractPacket>(() =>
                    {
                        return (AbstractPacket)Activator.CreateInstance(f);
                    }));

            _packetTypeIds = typeof(AbstractPacket).Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(AbstractPacket)) && t.CustomAttributes.Any(a => a.AttributeType == typeof(PacketIdAttribute)))
                .ToDictionary(
                    f => f,
                    f => f.GetCustomAttributes(typeof(PacketIdAttribute), false).Cast<PacketIdAttribute>().FirstOrDefault().PacketId);
        }

        public GetSet GetSet { get; set; }

        protected abstract void LoadPayload(byte[] payload);
        public abstract byte[] GetPayload();

        public static AbstractPacket FromBytes(byte packetId, byte[] payload)
        {
            if (_packetConstructors.ContainsKey(packetId))
            {
                var packet = _packetConstructors[packetId]();

                packet.LoadPayload(payload);

                return packet;
            }
            else
            {
                return null;
            }
        }

        public byte[] ToBytes()
        {
            var payload = GetPayload();

            var bytes = new byte[payload.Length + 3];
            Array.Copy(payload, 0, bytes, 3, payload.Length);

            bytes[0] = STARTBYTEID;
            bytes[1] = _packetTypeIds[GetType()];
            bytes[2] = (byte)payload.Length;

            return bytes;
        }
    }
}
