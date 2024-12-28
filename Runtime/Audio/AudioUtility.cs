using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using FMOD.Studio;
using FMODUnity;
using JetBrains.Annotations;

namespace Baracuda.Utility.Audio
{
    public static class AudioUtility
    {
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetPath(this in EventReference eventReference)
        {
#if UNITY_EDITOR
            return eventReference.Path;
#else
            if (eventReference.IsNull)
            {
                return "Invalid Event Reference";
            }

            RuntimeManager.StudioSystem.getEventByID(eventReference.Guid, out var eventDescription);

            if (!eventDescription.isValid())
            {
                return "Invalid Event Description";
            }

            eventDescription.getPath(out var path);
            return path;
#endif
        }

        public static List<EventReference> GetEventReferencesFromBank(string bankName)
        {
            if (!bankName.StartsWith("bank:/"))
            {
                bankName = "bank:/" + bankName;
            }

            var eventReferences = new List<EventReference>();

            RuntimeManager.StudioSystem.getBank(bankName, out var bank);

            if (bank.isValid())
            {
                bank.getEventCount(out var eventCount);

                if (eventCount > 0)
                {
                    bank.getEventList(out var eventDescriptions);

                    foreach (var eventDescription in eventDescriptions)
                    {
                        eventDescription.getID(out var guid);
#if UNITY_EDITOR
                        eventDescription.getPath(out var path);
#endif
                        var eventRef = new EventReference
                        {
                            Guid = guid,
#if UNITY_EDITOR
                            Path = path
#endif
                        };
                        eventReferences.Add(eventRef);
                    }
                }
                else
                {
                    Debug.LogWarning("No events found in the specified bank.");
                }
            }
            else
            {
                Debug.LogWarning("Bank not found or invalid name.");
            }

            return eventReferences;
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetRemainingDuration(this ref EventInstance eventInst)
        {
            if (eventInst.isValid() is false)
            {
                return 0;
            }

            eventInst.getTimelinePosition(out var playbackPosition);

            eventInst.getDescription(out var eventDesc);
            eventDesc.getLength(out var length);

            var remainingDuration = (length - playbackPosition) / 1000f;

            return remainingDuration;
        }

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetEventName(this ref EventReference eventReference)
        {
            if (eventReference.IsNull)
            {
                return "Invalid Event Reference";
            }

            RuntimeManager.StudioSystem.getEventByID(eventReference.Guid, out var eventDescription);

            if (!eventDescription.isValid())
            {
                return "Invalid Event Description";
            }

            eventDescription.getPath(out var path);
            var eventName = Path.GetFileName(path);
            return eventName;
        }
    }
}