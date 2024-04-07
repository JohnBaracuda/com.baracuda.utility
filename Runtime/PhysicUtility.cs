using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Utilities
{
    public static class PhysicUtility
    {
        private static readonly Collider[] buffer = new Collider[128];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckBoxOverlap(BoxCollider boxCollider, int layerMask)
        {
            var transform = boxCollider.transform;
            var worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);
            var worldSize = boxCollider.bounds.size;
            var halfExtents = new Vector3(worldSize.x / 2, worldSize.y / 2, worldSize.z / 2);
            var worldRotation = transform.rotation;

            var size = Physics.OverlapBoxNonAlloc(worldCenter, halfExtents, buffer, worldRotation, layerMask);

            return size > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckBoxOverlap(IEnumerable<BoxCollider> boxCollider, int layerMask)
        {
            foreach (var collider in boxCollider)
            {
                if (CheckBoxOverlap(collider, layerMask))
                {
                    return true;
                }
            }

            return false;
        }
    }
}