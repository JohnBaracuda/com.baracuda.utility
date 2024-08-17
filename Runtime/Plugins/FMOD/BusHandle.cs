using System;
using Baracuda.Serialization;
using FMOD;
using FMOD.Studio;
using FMODUnity;

namespace Baracuda.Bedrock.FMOD
{
    public class BusHandle : IDisposable
    {
        private readonly SaveDataInt _asset;
        private Bus _bus;

        public BusHandle(SaveDataInt asset, string path)
        {
            _asset = asset;
            _bus = RuntimeManager.GetBus(path);
            _asset.Changed += SetVolume;
            SetVolume(_asset.Value);
        }

        /// <summary>
        ///     Get or set the muted state of the channel
        /// </summary>
        public bool Muted
        {
            get => _bus.getMute(out var result) is RESULT.OK && result;
            set => _bus.setMute(value);
        }

        public void Dispose()
        {
            _asset.Changed -= SetVolume;
        }

        private void SetVolume(int volume)
        {
            _bus.setVolume(volume / 100f);
        }
    }
}