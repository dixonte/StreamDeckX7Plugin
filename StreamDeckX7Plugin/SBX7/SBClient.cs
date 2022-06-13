using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using StreamDeckX7Plugin.SBX7.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDeckX7Plugin.SBX7
{
    public class SBClient
    {
        public const Int32 COMM_CHANNEL_ID = 1;

        private BluetoothClient _bluetoothClient;
        private Thread _readThread;

        #region Singleton
        private SBClient()
        {
            PacketReceived += SBClient_PacketReceived;
        }

        private static SBClient _singleton;
        public static SBClient Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new SBClient();

                return _singleton;
            }
        }
        #endregion

        private bool _enabled = false;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;

                if (value)
                {
                    _bluetoothClient = new BluetoothClient();
                    var sb = _bluetoothClient.PairedDevices.Where(x => x.DeviceName.Contains("Sound Blaster") && x.DeviceName.Contains("X7")).FirstOrDefault();

                    if (sb == null || !sb.Connected)
                    {
                        _enabled = false;

                        return; // TODO: Throw exception?
                    }

                    _bluetoothClient.Connect(new BluetoothEndPoint(sb.DeviceAddress, BluetoothService.SerialPort, COMM_CHANNEL_ID));
                    var stream = _bluetoothClient.GetStream();

                    _readThread = new Thread(new ThreadStart(async () =>
                    {
                        try
                        {
                            while (true)
                            {
                                await ReadPacket(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine(ex.ToString());
                        }
                    }));
                    _readThread.Start();

                    // Query the speaker state immediately
                    new Thread(new ThreadStart(async () => { await QuerySpeakerConfigurationAsync(); })).Start();
                }
                else
                {
                    if (_readThread != null)
                    {
                        _readThread.Interrupt();
                        _readThread.Join();
                        _readThread = null;
                    }

                    if (_bluetoothClient != null)
                    {
                        _bluetoothClient.Dispose();
                        _bluetoothClient = null;
                    }
                }

                _enabled = value;
            }
        }

        public async Task ReadPacket(NetworkStream stream)
        {
            var startbyte = new byte[1];
            await stream.ReadAsync(startbyte, 0, startbyte.Length);

            if (startbyte[0] != AbstractPacket.STARTBYTEID)
            {
                Console.Error.WriteLine($"Invalid start byte: {startbyte[0].ToString("x2")}");
                return;
            }

            var header = new byte[2];
            await stream.ReadAsync(header, 0, header.Length);

            var packetId = header[0];
            var packetLen = header[1];

            var payload = new byte[packetLen];
            await stream.ReadAsync(payload, 0, payload.Length);

            var packet = AbstractPacket.FromBytes(packetId, payload);

            if (packet != null)
                PacketReceived?.Invoke(this, new PacketEventArgs { Packet = packet });
        }

        public class PacketEventArgs : EventArgs
        {
            public AbstractPacket Packet { get; set; }
        }

        public event EventHandler<PacketEventArgs> PacketReceived;

        private async void SBClient_PacketReceived(object sender, PacketEventArgs e)
        {
            if (e.Packet is SpeakerConfigurationPacket speakerConfigurationPacket)
            {
                if (e.Packet.GetSet == Enums.GetSet.Get && CurrentSpeakerConfiguration != speakerConfigurationPacket.SpeakerConfiguration)
                {
                    CurrentSpeakerConfiguration = speakerConfigurationPacket.SpeakerConfiguration;
                    SpeakerConfigurationChanged?.Invoke(this, new EventArgs());
                }
            }
            else if (e.Packet is AckPacket)
            {
                await QuerySpeakerConfigurationAsync();
            }
        }

        public SpeakerConfigurationPacket.X7SpeakerConfiguration CurrentSpeakerConfiguration { get; set; }

        public event EventHandler SpeakerConfigurationChanged;

        public async Task QuerySpeakerConfigurationAsync()
        {
            await _bluetoothClient.GetStream().WriteAsync((new SpeakerConfigurationPacket { GetSet = Enums.GetSet.Get }).ToBytes());
        }

        public async Task SetSpeakersLastConfigAsync()
        {
            await _bluetoothClient.GetStream().WriteAsync((new SpeakerConfigurationPacket { GetSet = Enums.GetSet.Set, SpeakerConfiguration = SpeakerConfigurationPacket.X7SpeakerConfiguration.TOGGLE_TO_SPEAKER }).ToBytes());
        }

        public async Task SetHeadphonesAsync()
        {
            await _bluetoothClient.GetStream().WriteAsync((new SpeakerConfigurationPacket { GetSet = Enums.GetSet.Set, SpeakerConfiguration = SpeakerConfigurationPacket.X7SpeakerConfiguration.HEADPHONES }).ToBytes());
        }

        public async Task SetSpeakers2_0Async()
        {
            await _bluetoothClient.GetStream().WriteAsync((new SpeakerConfigurationPacket { GetSet = Enums.GetSet.Set, SpeakerConfiguration = SpeakerConfigurationPacket.X7SpeakerConfiguration.STEREO_2_0 }).ToBytes());
            await _bluetoothClient.GetStream().WriteAsync((new SpeakerConfigurationPacket { GetSet = Enums.GetSet.Set, SpeakerConfiguration = SpeakerConfigurationPacket.X7SpeakerConfiguration.TOGGLE_TO_SPEAKER }).ToBytes());
        }

        public async Task SetSpeakers5_1Async()
        {
            await _bluetoothClient.GetStream().WriteAsync((new SpeakerConfigurationPacket { GetSet = Enums.GetSet.Set, SpeakerConfiguration = SpeakerConfigurationPacket.X7SpeakerConfiguration.MULTI_CHANNEL_5_1 }).ToBytes());
            await _bluetoothClient.GetStream().WriteAsync((new SpeakerConfigurationPacket { GetSet = Enums.GetSet.Set, SpeakerConfiguration = SpeakerConfigurationPacket.X7SpeakerConfiguration.TOGGLE_TO_SPEAKER }).ToBytes());
        }
    }
}
