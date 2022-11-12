// Copyright (c) 2022 Jonathan Lang

using Baracuda.Utilities.Pooling.Source;
using System.Collections.Generic;

namespace Baracuda.Utilities.Pooling
{
    public class DictionaryPool<TKey, TValue> : CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
    {
    }
}