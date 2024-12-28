using System;
using FMODUnity;
using UnityEngine;

namespace Baracuda.Utility.Audio
{
    [Serializable]
    public struct SpacialAudio
    {
        [SerializeField] private EventReference eventReference;
        [SerializeField] private SpacialAudioSettings spacialSettings;

        public EventReference EventReference => eventReference;
        public SpacialAudioSettings SpacialSettings => spacialSettings;

        public SpacialAudio(EventReference eventReference, SpacialAudioSettings spacialSettings)
        {
            this.eventReference = eventReference;
            this.spacialSettings = spacialSettings;
        }
    }
}