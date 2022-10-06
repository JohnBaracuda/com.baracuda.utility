using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Utilities.Extensions
{
    public static class RectExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithX(this Rect rect, float x)
        {
            return new Rect(x, rect.y, rect.width, rect.height);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithY(this Rect rect, float y)
        {
            return new Rect(rect.x, y, rect.width, rect.height);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithXY(this Rect rect, float x, float y)
        {
            return new Rect(x, y, rect.width, rect.height);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithWidth(this Rect rect, float width)
        {
            return new Rect(rect.x, rect.y, width, rect.height);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithHeight(this Rect rect, float height)
        {
            return new Rect(rect.x, rect.y, rect.width, height);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithWidthAndHeight(this Rect rect, float width, float height)
        {
            return new Rect(rect.x, rect.y, width, height);
        }

        /*
         * Offsets   
         */
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithOffset(this Rect rect, float x, float y = 0, float width = 0, float height = 0)
        {
            return new Rect(rect.x + x, rect.y + y, rect.width + width, rect.height + height);
        }
    }
}
