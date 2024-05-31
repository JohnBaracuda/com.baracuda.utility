namespace Baracuda.Utilities
{
    public interface INative<out T> where T : struct
    {
        public T ToNative();
    }
}