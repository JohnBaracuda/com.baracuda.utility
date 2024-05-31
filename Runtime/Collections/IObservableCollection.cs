using System;

namespace Baracuda.Utilities.Collections
{
    public interface IObservableCollection<out T>
    {
        event Action Changed;
        event Action<T> Added;
        event Action<T> Removed;
    }
}