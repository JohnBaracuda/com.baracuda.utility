using System;

namespace Baracuda.Gameloop.Update
{
    /// <summary>
    /// Handle for an <see cref="ITickReceiver"/> to dynamically enable / disable its tick state.
    /// </summary>
    public interface ITickHandle : IDisposable
    {
        public bool TickEnabled { get; set; }

        /// <summary>
        /// Enable the handles <see cref="ITickReceiver"/> if it is not already enabled.
        /// </summary>
        public void Enable() => TickEnabled = true;

        /// <summary>
        /// Disable the handles <see cref="ITickReceiver"/> if it is not already disabled.
        /// </summary>
        public void Disable() => TickEnabled = false;

        /// <summary>
        /// Toggle the enabled state of the handles <see cref="ITickReceiver"/>.
        /// </summary>
        /// <returns></returns>
        public bool Toggle() => TickEnabled = !TickEnabled;

        /// <summary>
        /// Set the enabled state of the handles <see cref="ITickReceiver"/>.
        /// </summary>
        public void SetEnabled(bool enabled) => TickEnabled = enabled;
    }
}