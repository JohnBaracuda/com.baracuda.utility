namespace Baracuda.Utilities.Collections
{
    public class ValueStack<TValue>
    {
        private readonly StackList<TValue> _values = new();
        private readonly StackList<object> _provider = new();
        private readonly TValue _defaultValue;

        public TValue Value => _values.TryPeek(out var result) ? result : _defaultValue;

        public void Add(TValue value, object provider)
        {
            if (_provider.Contains(provider))
            {
                return;
            }

            _values.Push(value);
            _provider.Push(provider);
        }

        public void Remove(object provider)
        {
            if (_provider.Count <= 0)
            {
                return;
            }

            if (_provider.Peek() == provider)
            {
                _provider.Pop();
                _values.Pop();
            }
            else
            {
                var index = _provider.List.IndexOf(provider);
                if (index == -1)
                {
                    return;
                }
                _provider.List.RemoveAt(index);
                _values.List.RemoveAt(index);
            }
        }

        public static implicit operator TValue(ValueStack<TValue> stack)
        {
            return stack.Value;
        }
    }
}