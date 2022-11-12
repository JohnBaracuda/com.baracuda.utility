using Baracuda.Utilities.Pooling;
using System.Linq;

namespace Baracuda.Utilities
{
    public static class ArrayExtensions
    {
        #region Bitmask

        public static int GetMaxMaskIndex<T>(this T[] options)
        {
            return (1 << options.Length) - 1;
        }

        /// <summary>
        /// Returns an integer that represents the current selection based on the available options.
        /// </summary>
        public static int GetMaskIndex<T>(T[] current, T[] options)
        {
            var index = 0;
            for (var i = 0; i < options.Length; i++)
            {
                var result = 1;
                var e = i;

                while (e > 0)
                {
                    result *= 2;
                    e--;
                }

                if (current.Contains(options[i]))
                {
                    index |= result;
                }
            }

            return index;
        }

        public static T[] GetSelectionFromMask<T>(int mask, T[] options)
        {
            var selection = ListPool<T>.Get();


            for (var i = 0; i < options.Length; i++)
            {
                var index = 1;
                var e = i;

                while (e > 0)
                {
                    index *= 2;
                    e--;
                }

                if (mask.HasFlagInt(index))
                {
                    selection.Add(options[i]);
                }
            }

            var result = selection.ToArray();
            ListPool<T>.Release(selection);
            return result;
        }

        #endregion
    }
}
