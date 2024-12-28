using System;

namespace Baracuda.Utility.Types
{
    public class ObservableBridge : IDisposable
    {
        public Broadcast Changed { get; } = new();
        private Action _dispose;

        public static ObservableBridge Create<T>(IObservableValue<T> observableValue)
        {
            var bridge = new ObservableBridge();
            Action<T> raise = _ => { bridge.Changed.Raise(); };
            observableValue.AddObserver(raise);
            bridge._dispose = () => { observableValue.RemoveObserver(raise); };
            return bridge;
        }

        public void Dispose()
        {
            _dispose();
        }
    }
}