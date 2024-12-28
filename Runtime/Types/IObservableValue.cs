using System;
using JetBrains.Annotations;

namespace Baracuda.Utility.Types
{
    public interface IObservableValue<TValue>
    {
        [PublicAPI]
        public TValue Value { get; set; }

        [PublicAPI]
        public void AddObserver(Action<TValue> observer);

        [PublicAPI]
        public void RemoveObserver(Action<TValue> observer);
    }
}