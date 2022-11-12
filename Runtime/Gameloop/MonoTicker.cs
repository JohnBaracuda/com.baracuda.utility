using Baracuda.Utilities.Singleton;
using System.Collections.Generic;
using UnityEngine;

namespace Baracuda.Gameloop.Update
{
    /// <summary>
    /// GameLoop manager used to register <see cref="ITickReceiver"/> for custom tick events.
    /// </summary>
    [AddComponentMenu("Gameloop/Ticker")]
    internal class MonoTicker : MonoSingleton<MonoTicker>, ITicker
    {
        #region Fields

        private readonly List<ITickReceiver> _updateReceiver = new(64);

        private readonly Dictionary<float, TickLoop> _tickLoops = new(16);

        #endregion

        #region Add

        public void AddTick(ITickReceiver receiver)
        {
            _updateReceiver.Add(receiver);
        }

        public void AddTick(ITickReceiver receiver, float secondsDelay)
        {
            if (_tickLoops.TryGetValue(secondsDelay, out var loop))
            {
                loop.Add(receiver);
            }
            else
            {
                _tickLoops.Add(secondsDelay, new TickLoop(receiver, secondsDelay));
            }
        }

        public ITickHandle AddTickHandled(ITickReceiver receiver, bool tickEnabled = true)
        {
            var handle = TickHandle.Get(receiver, this);
            handle.SetEnabled(tickEnabled);
            return handle;
        }

        public ITickHandle AddTickHandled(ITickReceiver receiver, float secondsDelay, bool tickEnabled = true)
        {
            var handle = TickHandle.Get(receiver, secondsDelay, this);
            handle.SetEnabled(tickEnabled);
            return handle;
        }

        #endregion

        #region Remove

        public void RemoveTickReceiver(ITickReceiver receiver)
        {
            if (_updateReceiver.Remove(receiver))
            {
                return;
            }

            foreach (var tickLoop in _tickLoops.Values)
            {
                if (!tickLoop.Remove(receiver))
                {
                    continue;
                }

                if (tickLoop.Count <= 0)
                {
                    _tickLoops.Remove(tickLoop.Delay);
                }
                return;
            }
        }

        #endregion

        #region Update & Disposal

        protected override void Awake()
        {
            base.Awake();
            ITicker.Singleton = this;
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;

            for (var i = _updateReceiver.Count - 1; i >= 0; i--)
            {
                _updateReceiver[i].Tick(deltaTime);
            }

            foreach (var tickLoop in _tickLoops.Values)
            {
                tickLoop.Update(deltaTime);
            }
        }

        private void OnDestroy()
        {
            _updateReceiver.Clear();
            _tickLoops.Clear();
        }

        #endregion
    }
}