using System.Collections.Generic;

namespace Baracuda.Gameloop.Update
{
    /// <summary>
    /// Internal class managing a collection of tick receiver with a specific time period
    /// </summary>
    internal class TickLoop
    {
        /*
         * State
         */

        public int Count => _listener.Count;
        private float _timer;
        public readonly float Delay;
        private readonly List<ITickReceiver> _listener;

        /*
         * API
         */

        public void Add(ITickReceiver ticker)
        {
            _listener.Add(ticker);
        }

        public bool Remove(ITickReceiver ticker)
        {
            return _listener.Remove(ticker);
        }

        /*
         * Ctor
         */

        public TickLoop(ITickReceiver receiver, float delay, int capacity = 64)
        {
            Delay = delay;
            _timer = 0;
            _listener = new List<ITickReceiver>(capacity) {receiver};
        }

        /*
         * Loop
         */

        public void Update(float deltaTime)
        {
            if ((_timer += deltaTime) < Delay)
            {
                return;
            }

            for (var i = _listener.Count - 1; i >= 0; i--)
            {
                _listener[i].Tick(_timer);
            }

            _timer = 0;
        }
    }
}