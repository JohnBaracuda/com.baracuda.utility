using Baracuda.Utilities.Pooling.Source;

namespace Baracuda.Gameloop.Update
{
    internal class TickHandle : ITickHandle
    {
        private static readonly ObjectPool<TickHandle> pool = new ObjectPoolT<TickHandle>(() => new TickHandle(), defaultCapacity: 64);

        /*
         * State
         */

        private ITickReceiver _receiver;
        private ITicker _ticker;
        private bool _tickEnabled;
        private float? _delay;

        public bool TickEnabled
        {
            get => _tickEnabled;
            set
            {
                if (value == _tickEnabled)
                {
                    return;
                }

                _tickEnabled = value;

                if (_tickEnabled)
                {
                    if (_delay.HasValue)
                    {
                        _ticker.AddTick(_receiver, _delay.Value);
                    }
                    else
                    {
                        _ticker.AddTick(_receiver);
                    }
                }
                else
                {
                    _ticker.RemoveTickReceiver(_receiver);
                }
            }
        }

        /*
         * Factory
         */

        public static ITickHandle Get(ITickReceiver receiver, ITicker ticker)
        {
            var handle = pool.Get();
            handle._tickEnabled = false;
            handle._receiver = receiver;
            handle._delay = null;
            handle._ticker = ticker;
            return handle;
        }

        public static ITickHandle Get(ITickReceiver receiver, float secondsDelay, ITicker ticker)
        {
            var handle = pool.Get();
            handle._tickEnabled = false;
            handle._receiver = receiver;
            handle._delay = secondsDelay;
            handle._ticker = ticker;
            return handle;
        }

        public void Dispose()
        {
            _ticker.RemoveTickReceiver(_receiver);
            _ticker = null;
            _receiver = null;
            _tickEnabled = false;
            pool.Release(this);
        }
    }
}