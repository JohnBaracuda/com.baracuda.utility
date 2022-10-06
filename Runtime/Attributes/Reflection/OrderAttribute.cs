using System;
using Baracuda.Utilities.Extensions;

namespace Baracuda.Utilities.Reflection
{
    /// <summary>
    /// Multi purpose order attribute
    /// </summary>
    public class OrderAttribute : Attribute
    {
        public int Order { get; }

        public OrderAttribute(int order)
        {
            Order = order;
        }

        public static Comparison<T> ObjectComparison<T>() where T : class
        {
            return (lhs, rhs) =>
            {
                var lhsOrder = 0;
                var rhsOrder = 0;

                if (lhs.GetType().TryGetCustomAttribute<OrderAttribute>(out var lhsAttribute))
                {
                    lhsOrder = lhsAttribute.Order;
                }

                if (rhs.GetType().TryGetCustomAttribute<OrderAttribute>(out var rhsAttribute))
                {
                    rhsOrder = rhsAttribute.Order;
                }

                return rhsOrder.CompareTo(lhsOrder);
            };
        }
    }
}
