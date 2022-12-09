#if UNITY_EDITOR
using Baracuda.Utilities;
using Baracuda.Utilities.Inspector;
using Baracuda.Utilities.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Gameloop.Update
{
    /// <summary>
    /// Editor class providing edit time tick management.
    /// </summary>
    [Description("Editor class providing a edit time tick based game-loop with delta time")]
    internal class EditorTicker : Utilities.ScriptableSingleton<EditorTicker>, ITicker
    {
        /*
         * Type definitions
         */

        [ShowInInspector] private double _lastTimeStamp;
        [ShowInInspector] private float _deltaTime;

        private readonly List<ITickReceiver> _updateReceiver = new(32);
        private readonly Dictionary<float, TickLoop> _tickLoops = new(16);

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

        public ITickHandle AddTickHandled(ITickReceiver receiver, float secondsDelay, bool tickEnabled = true)
        {
            var handle = TickHandle.Get(receiver, secondsDelay, this);
            handle.SetEnabled(tickEnabled);
            return handle;
        }

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

        public ITickHandle AddTickHandled(ITickReceiver receiver, bool tickEnabled)
        {
            var handle = TickHandle.Get(receiver, this);
            handle.SetEnabled(tickEnabled);
            return handle;
        }

        private void OnEditorUpdate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            var current = EditorApplication.timeSinceStartup;
            _deltaTime = (float) (current - _lastTimeStamp);
            _lastTimeStamp = current;

            for (var i = _updateReceiver.Count - 1; i >= 0; i--)
            {
                _updateReceiver[i].Tick(_deltaTime);
            }

            foreach (var tickLoop in _tickLoops.Values)
            {
                tickLoop.Update(_deltaTime);
            }
        }

        protected override void OnInitialized()
        {
            ITicker.Singleton = this;
            _updateReceiver.Clear();
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }
    }
}

#endif