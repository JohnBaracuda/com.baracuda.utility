using System;

namespace Baracuda.Utility.Types
{
    public readonly struct OneOf<T1, T2>
    {
        public readonly bool HasValue => _index != 0;
        private readonly int _index;
        private readonly T1 _result1;
        private readonly T2 _result2;

        private OneOf(T1 result)
        {
            _index = 1;
            _result1 = result;
            _result2 = default;
        }

        private OneOf(T2 result)
        {
            _index = 2;
            _result1 = default;
            _result2 = result;
        }

        public static implicit operator OneOf<T1, T2>(T1 result)
        {
            return new OneOf<T1, T2>(result);
        }

        public static implicit operator OneOf<T1, T2>(T2 result)
        {
            return new OneOf<T1, T2>(result);
        }

        public static implicit operator T1(OneOf<T1, T2> oneOf)
        {
            return oneOf._result1;
        }

        public static implicit operator T2(OneOf<T1, T2> oneOf)
        {
            return oneOf._result2;
        }

        public bool Map<T>(Action<T> action)
        {
            switch (_index)
            {
                case 1:
                {
                    if (_result1 is T result)
                    {
                        action(result);
                        return true;
                    }
                    return false;
                }

                case 2:
                {
                    if (_result2 is T result)
                    {
                        action(result);
                        return true;
                    }
                    return false;
                }

                default:
                    return false;
            }
        }
    }
}