namespace Baracuda.Utilities.Events
{
    public delegate void InAction<T>(in T arg);

    public delegate void InAction<T1, T2>(in T1 arg1, in T2 arg2);

    public delegate void InAction<T1, T2, T3>(in T1 arg1, in T2 arg2, in T3 arg3);

    public delegate void InAction<T1, T2, T3, T4>(in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4);
}