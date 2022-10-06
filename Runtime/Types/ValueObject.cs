namespace Baracuda.Utilities.Types
{
    public class ValueObject<T>
    {
        public T Value;
        
        public ValueObject(T value)
        {
            Value = value;
        }

        public static implicit operator ValueObject<T>(T value)
        {
            return new ValueObject<T>(value);
        }
        
        public static implicit operator T(ValueObject<T> valueObject)
        {
            return valueObject.Value;
        }
    }
}
