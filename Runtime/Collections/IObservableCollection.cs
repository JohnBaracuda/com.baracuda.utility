using Baracuda.Utility.Types;

namespace Baracuda.Utility.Collections
{
    public interface IObservableCollection<T>
    {
        public Broadcast Changed { get; }
        public Broadcast<T> Added { get; }
        public Broadcast<T> Removed { get; }
    }
}