using System;
using Baracuda.Utility.Types;
using FMOD;
using FMOD.Studio;
using FMODUnity;

namespace Baracuda.Utility.Audio
{
    public class BusHandle : IDisposable
    {
        private readonly IObservableValue<int> _volume;
        private Bus _bus;

        public BusHandle(IObservableValue<int> volume, string path)
        {
            _volume = volume;
            _bus = RuntimeManager.GetBus(path);
            volume.AddObserver(SetVolume);
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
            _volume.RemoveObserver(SetVolume);
        }

        private void SetVolume(int volume)
        {
            _bus.setVolume(volume / 100f);
        }
    }
}