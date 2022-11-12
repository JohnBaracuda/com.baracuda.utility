using System.Diagnostics.Contracts;

namespace Baracuda.Gameloop.Update
{
    /// <summary>
    /// GameLoop manager used to register <see cref="ITickReceiver"/> for custom tick events.
    /// </summary>
    public interface ITicker
    {
        public static ITicker Singleton { get; set; }

        /// <summary>
        /// Pass in an object that will receive a custom tick update.
        /// </summary>
        void AddTick(ITickReceiver receiver);

        /// <summary>
        /// Pass in an object that will receive a custom tick update with an optional minimal delay in between updates.
        /// </summary>
        void AddTick(ITickReceiver receiver, float secondsDelay);

        /// <summary>
        /// Pass in an object that will receive a custom tick update and return a handle that can be used to
        /// enable and disable the receivers tick loop.
        /// </summary>
        [Pure] ITickHandle AddTickHandled(ITickReceiver receiver, bool tickEnabled = true);

        /// <summary>
        /// Pass in an object that will receive a custom tick update with an optional minimal delay in between updates
        /// and return a handle that can be used to enable and disable the receivers tick loop.
        /// </summary>
        [Pure] ITickHandle AddTickHandled(ITickReceiver receiver, float secondsDelay, bool tickEnabled = true);

        /// <summary>
        /// Pass in an object to remove it from receiving custom tick updates.
        /// </summary>
        void RemoveTickReceiver(ITickReceiver receiver);
    }
}
