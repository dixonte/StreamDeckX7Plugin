using StreamDeckLib;
using StreamDeckLib.Messages;
using StreamDeckX7Plugin.SBX7;
using StreamDeckX7Plugin.SBX7.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StreamDeckX7Plugin.actions
{
    [ActionUuid(Uuid = "com.heavybell.x7plugin.SpeakerHeadphoneToggle.action")]
    public class SpeakerHeadphoneToggle : BaseStreamDeckActionWithSettingsModel<Models.CounterSettingsModel>
    {
        private string _context;
        private SpeakerConfigurationPacket.X7SpeakerConfiguration _intendedConfig;

        private async Task UpdateImageAsync()
        {
            var transition = _intendedConfig != SBClient.Singleton.CurrentSpeakerConfiguration;
            var speakers = _intendedConfig != SpeakerConfigurationPacket.X7SpeakerConfiguration.HEADPHONES;

            if (speakers)
            {
                await Manager.SetImageAsync(_context, $"images/SpeakerMode{(transition ? "Grey" : "")}.png");
            }
            else
            {
                await Manager.SetImageAsync(_context, $"images/HeadphoneMode{(transition ? "Grey" : "")}.png");
            }
        }

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            // Do nothing if we're already changing
            if (_intendedConfig != SBClient.Singleton.CurrentSpeakerConfiguration)
                return;

            if (SBClient.Singleton.CurrentSpeakerConfiguration != SpeakerConfigurationPacket.X7SpeakerConfiguration.HEADPHONES)
            {
                _intendedConfig = SpeakerConfigurationPacket.X7SpeakerConfiguration.HEADPHONES;
                await SBClient.Singleton.SetHeadphonesAsync();
            }
            else
            {
                _intendedConfig = SpeakerConfigurationPacket.X7SpeakerConfiguration.TOGGLE_TO_SPEAKER;
                await SBClient.Singleton.SetSpeakersLastConfigAsync();
            }

            await UpdateImageAsync();
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            _context = args.context;
            _intendedConfig = SBClient.Singleton.CurrentSpeakerConfiguration;

            await base.OnWillAppear(args);
            await UpdateImageAsync();

            SBClient.Singleton.SpeakerConfigurationChanged += Singleton_SpeakerConfigurationChanged;
        }

        public override async Task OnWillDisappear(StreamDeckEventPayload args)
        {
            SBClient.Singleton.SpeakerConfigurationChanged -= Singleton_SpeakerConfigurationChanged;

            await base.OnWillDisappear(args);
        }

        private async void Singleton_SpeakerConfigurationChanged(object sender, EventArgs e)
        {
            _intendedConfig = SBClient.Singleton.CurrentSpeakerConfiguration;
            await UpdateImageAsync();
        }
    }
}
