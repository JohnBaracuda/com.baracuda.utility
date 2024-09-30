using FMODUnity;
using UnityEngine;

namespace Baracuda.Bedrock.FMOD
{
    public static class SpacialAudioUtility
    {
        private static global::FMOD.Studio.System fmodSystem;

        public static global::FMOD.Studio.System FmodSystem()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return RuntimeManager.StudioSystem;
            }

            EditorUtils.LoadPreviewBanks();
            return EditorUtils.System;
#else
            return RuntimeManager.StudioSystem;
#endif
        }

        public static SpacialAudioSettings GetSpacialAudioSettings(EventReference eventReference, ref SpacialAudioSettings settings)
        {
            var studioSystem = FmodSystem();
            studioSystem.getEventByID(eventReference.Guid, out var eventDescription);

            if (!eventDescription.isValid())
            {
                return settings;
            }

            eventDescription.getMinMaxDistance(out var minDistance, out var maxDistance);
            eventDescription.is3D(out var is3D);

            if (!is3D)
            {
                return settings;
            }

            settings.radius = maxDistance;

            settings.falloff = new AnimationCurve(
                new Keyframe(0, 1), // At distance 0, volume is 1
                new Keyframe(minDistance, 1), // No falloff within the minDistance
                new Keyframe(maxDistance, 0) // Volume falls to 0 at maxDistance
            );

            return settings;
        }
    }
}