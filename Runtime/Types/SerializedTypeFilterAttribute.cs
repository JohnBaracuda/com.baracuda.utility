using System;
using UnityEngine;

namespace Baracuda.Utilities.Types
{
    public class SerializedTypeFilterAttribute : PropertyAttribute
    {
        public Func<Type, bool> Filter { get; }

        public SerializedTypeFilterAttribute(Type filterType)
        {
            Filter = type => !type.IsAbstract &&
                             !type.IsInterface &&
                             !type.IsGenericType &&
                             type.InheritsOrImplements(filterType);
        }
    }
}