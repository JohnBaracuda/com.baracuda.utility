using System;
using System.Collections.Generic;

namespace Baracuda.Bedrock.Timing
{
    public interface ICountdown
    {
        /// <summary>
        ///     Returns true if the cooldown is active and running.
        /// </summary>
        public bool IsRunning { get; }

        /// <summary>
        ///     Returns true if the cooldown is active but paused.
        /// </summary>
        public bool IsPaused { get; }

        /// <summary>
        ///     Returns true if the cooldown is active. The cooldown could either be paused or running.
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        ///     How long is the cooldown.
        /// </summary>
        public float TotalDurationInSeconds { get; }

        /// <summary>
        ///     How long is the cooldown without any modifications.
        /// </summary>
        public float Value { get; }

        /// <summary>
        ///     How many seconds remain until the cooldown has completed.
        /// </summary>
        public float RemainingDurationInSeconds { get; }

        /// <summary>
        ///     How many seconds have passed since the cooldown was started.
        /// </summary>
        public float PassedDurationInSeconds { get; }

        /// <summary>
        ///     Percentage completion value between 0 and 100.
        /// </summary>
        public float PercentageCompleted { get; }

        /// <summary>
        ///     Percentage completion value between 0 and 1.
        /// </summary>
        public float FactorCompleted { get; }

        /// <summary>
        ///     Raised when the cooldown has completed.
        /// </summary>
        public event Action Completed;

        /// <summary>
        ///     Raised when the cooldown was cancelled.
        /// </summary>
        public event Action Cancelled;

        /// <summary>
        ///     Raised when the cooldown was started.
        /// </summary>
        public event Action Started;

        /// <summary>
        ///     Raised when the cooldown was restarted.
        /// </summary>
        public event Action Restarted;

        /// <summary>
        ///     Raised when the cooldown is paused.
        /// </summary>
        public event Action Paused;

        /// <summary>
        ///     Raised when the cooldown was paused and is resumed.
        /// </summary>
        public event Action Resumed;

        /// <summary>
        ///     Raised when the cooldown was reduced.
        /// </summary>
        public event Action<float> Reduced;

        /// <summary>
        ///     Start the cooldown.
        /// </summary>
        /// <returns>True if the cooldown was started</returns>
        public bool Start();

        /// <summary>
        ///     Force the cooldown to end without calling completed callbacks.
        /// </summary>
        /// <returns>True if the cooldown was active</returns>
        public bool Cancel();

        /// <summary>
        ///     Force the cooldown to end calling completed callbacks.
        /// </summary>
        /// <returns>True if the cooldown was active</returns>
        public bool Complete();

        /// <summary>
        ///     Restart the cooldown.
        /// </summary>
        /// <returns>True if the cooldown was running</returns>
        public bool Restart(bool startIfInactive = true);

        /// <summary>
        ///     Reduce the cooldown by a given duration.
        /// </summary>
        /// <param name="durationInSeconds">How many seconds should be subtracted from the cooldown</param>
        /// <returns>True if the cooldown was active</returns>
        public bool Reduce(float durationInSeconds);

        /// <summary>
        ///     Pause the cooldown if it is running.
        /// </summary>
        /// <returns>True if the cooldown was running and can be paused</returns>
        public bool Pause();

        /// <summary>
        ///     Resume the cooldown if it is paused.
        /// </summary>
        /// <returns>True if the cooldown was paused and is running again</returns>
        public bool Resume();

        /// <summary>
        ///     List of modifiers that influence the total duration of the cooldown.
        /// </summary>
        public IList<ICountdownDurationModifier> CooldownDurationModifiers { get; }

        internal void UpdateCountdown(float deltaTime);
    }
}