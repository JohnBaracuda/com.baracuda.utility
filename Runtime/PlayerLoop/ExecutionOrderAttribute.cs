using System;

namespace Baracuda.Bedrock.PlayerLoop
{
    /// <summary>
    ///     Apply this attribute to a MonoScript to set its script execution order to the order value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExecutionOrderAttribute : Attribute
    {
        public int Order { get; }

        public ExecutionOrderAttribute(int order)
        {
            Order = order;
        }
    }
}