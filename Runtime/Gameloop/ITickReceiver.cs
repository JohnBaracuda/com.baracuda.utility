namespace Baracuda.Gameloop.Update
{
    /// <summary>
    /// Interface for objects that can receive custom tick events.
    /// Instance must be registered to an active <see cref="ITicker"/>.
    /// </summary>
    public interface ITickReceiver
    {
        /// <summary>
        /// Called every frame or in custom intervals.
        /// </summary>
        /// <param name="deltaTime">The time since the last received tick</param>
        void Tick(float deltaTime);
    }
}