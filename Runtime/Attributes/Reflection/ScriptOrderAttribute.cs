namespace Baracuda.Utilities.Reflection
{
    public enum Order : int
    {
        CoreServiceRegistration = short.MinValue,
        ServiceRegistration = short.MinValue + 10,
        Default = 0,
        Last = short.MaxValue,
    }
}