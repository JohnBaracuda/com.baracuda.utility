using System;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    public static class ConditionalShowValidator
    {
        public static bool ValidateComparison(object lhs, Func<object> getRhs, bool negationCondition)
        {
            var convertedRhs = TryConvert(getRhs(), lhs);

            var valueType = getRhs().GetType();

            return valueType.IsFlagsEnum()
                ? negationCondition
                    ? !convertedRhs.As<int>().HasFlagInt(lhs.As<int>())
                    : convertedRhs.As<int>().HasFlagInt(lhs.As<int>())
                : negationCondition
                    ? !convertedRhs.Equals(lhs)
                    : convertedRhs.Equals(lhs);
        }

        private static object TryConvert(object from, object to)
        {
            try
            {
                return Convert.ChangeType(from, to.GetType());
            }
            catch (Exception)
            {
                return from;
            }
        }
    }
}