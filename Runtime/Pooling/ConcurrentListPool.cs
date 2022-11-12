// Copyright (c) 2022 Jonathan Lang

using Baracuda.Utilities.Pooling.Source;
using System.Collections.Generic;

namespace Baracuda.Utilities.Pooling
{
    public class ConcurrentListPool<T> : ConcurrentCollectionPool<List<T>, T>
    {
    }
}