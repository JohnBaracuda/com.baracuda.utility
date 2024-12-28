using System.Runtime.CompilerServices;
using Baracuda.Utility.Timing;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

namespace Baracuda.Utility.Audio
{
    public static class AudioExtensions
    {
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Play(this in EventReference eventReference, ref UnscaledTimer blockTimer, int milliseconds)
        {
            if (blockTimer.IsNotRunning)
            {
                blockTimer = UnscaledTimer.FromMilliseconds(milliseconds);
                RuntimeManager.PlayOneShot(eventReference);
            }
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Play(this in EventReference eventReference, bool predicate)
        {
            if (predicate)
            {
                RuntimeManager.PlayOneShot(eventReference);
            }
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Play(this in EventReference eventReference)
        {
            RuntimeManager.PlayOneShot(eventReference);
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Play(this in EventReference eventReference, Transform target)
        {
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.set3DAttributes(target.To3DAttributes());
            eventInstance.start();
            eventInstance.release();
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Play(this in EventReference eventReference, in Vector3 position)
        {
            RuntimeManager.PlayOneShot(eventReference, position);
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Play(this in EventReference eventReference, in Vector3 position, in FmodParameter parameter)
        {
            var instance = RuntimeManager.CreateInstance(eventReference);
            instance.set3DAttributes(position.To3DAttributes());
            instance.start();
            instance.setParameterByID(parameter.Id, parameter.Value);
            instance.release();
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Play(this in EventReference eventReference, in FmodParameter parameter)
        {
            var instance = RuntimeManager.CreateInstance(eventReference);
            instance.start();
            instance.setParameterByID(parameter.Id, parameter.Value);
            instance.release();
        }
    }
}