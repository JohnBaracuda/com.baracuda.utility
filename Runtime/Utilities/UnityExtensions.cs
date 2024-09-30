using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Baracuda.Bedrock.Collections;
using Baracuda.Bedrock.Types;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Baracuda.Bedrock.Utilities
{
    public static class UnityExtensions
    {
        #region Transform

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform[] GetChildren(this Transform transform)
        {
            var buffer = ListPool<Transform>.Get();
            foreach (Transform child in transform)
            {
                buffer.Add(child);
            }

            var result = buffer.ToArray();
            ListPool<Transform>.Release(buffer);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AnchorX(this RectTransform rectTransform)
        {
            return rectTransform.anchoredPosition.x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AnchorY(this RectTransform rectTransform)
        {
            return rectTransform.anchoredPosition.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void KillChildObjects(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SamplePositionInCircle(this Transform transform, float radius, float coneDegree = 80)
        {
            var center = transform.position;

            // Flatten the forward vector onto the X-Z plane
            var forward = transform.forward;
            var flatForward = new Vector3(forward.x, 0f, forward.z).normalized;

            // Get the angle of the flattened forward vector in radians
            var forwardAngle = Mathf.Atan2(flatForward.z, flatForward.x);

            // Convert the cone degree to radians and get the half-angle
            var halfCone = Mathf.Deg2Rad * coneDegree / 2f;

            // Generate random angle in radians within the restricted cone range
            var angle = Random.Range(forwardAngle - halfCone, forwardAngle + halfCone);

            var x = center.x + radius * Mathf.Cos(angle);
            var z = center.z + radius * Mathf.Sin(angle);

            var newPos = new Vector3(x, center.y, z);

            return newPos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndRotation(this Transform transform, Transform target)
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetLocalPositionAndRotation(this Transform transform, Transform target)
        {
            transform.localPosition = target.localPosition;
            transform.localRotation = target.localRotation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndViewDirection(this Transform transform, Vector3 position,
            Vector3 viewDirection)
        {
            if (viewDirection == Vector3.zero)
            {
                transform.position = position;
            }
            else
            {
                transform.SetPositionAndRotation(position, Quaternion.LookRotation(viewDirection));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveTowards(this Transform transform, Transform target, float maxDistanceDelta,
            float maxDegreesDelta)
        {
            var position = Vector3.MoveTowards(transform.position, target.position, maxDistanceDelta);
            var rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, maxDegreesDelta);
            transform.SetPositionAndRotation(position, rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale(this Transform transform, float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale(this Transform transform, Vector3 scale)
        {
            transform.localScale = scale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVirtualParent(this Transform transform, MonoBehaviour parent, float duration)
        {
            var parentTransform = parent.transform;
            parent.StartCoroutine(VirtualParentCoroutine());
            return;

            IEnumerator VirtualParentCoroutine()
            {
                var timer = 0f;
                while (timer < duration)
                {
                    yield return null;
                    timer += Time.deltaTime;
                    transform.SetPositionAndRotation(parentTransform);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SlerpLocalRotationTo(this Transform transform, Quaternion targetRotation, float sharpness)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, sharpness * Time.deltaTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LerpLocalRotationTo(this Transform transform, Quaternion targetRotation, float sharpness)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, sharpness * Time.deltaTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LerpLocalPositionTo(this Transform transform, Vector3 targetRotation, float sharpness)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetRotation, sharpness * Time.deltaTime);
        }

        #endregion


        #region Rect Transform

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRectInsideScreen(this RectTransform rectTransform, float tolerance)
        {
            var inside = true;
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            var displayRect = new Rect(-tolerance, -tolerance, Screen.width + tolerance * 2,
                Screen.height + tolerance * 2);

            foreach (var corner in corners)
            {
                if (!displayRect.Contains(corner))
                {
                    inside = false;
                }
            }

            return inside;
        }

        public static Rect GetScreenCoordinatesOfCorners(this RectTransform uiElement)
        {
            var worldCorners = new Vector3[4];
            uiElement.GetWorldCorners(worldCorners);
            var result = new Rect(
                worldCorners[0].x,
                worldCorners[0].y,
                worldCorners[2].x - worldCorners[0].x,
                worldCorners[2].y - worldCorners[0].y);

            return result;
        }

        public static Vector2 GetPixelPositionOfRect(this RectTransform uiElement)
        {
            var screenRect = GetScreenCoordinatesOfCorners(uiElement);

            return new Vector2(screenRect.center.x, screenRect.center.y);
        }

        private static void KeepChildInScrollViewPort(ScrollRect scrollRect, RectTransform child, Margin margin)
        {
            Canvas.ForceUpdateCanvases();

            // Get min and max of the viewport and child in local space to the viewport so we can compare them.
            // NOTE: use viewport instead of the scrollRect as viewport doesn't include the scrollbars in it.
            var viewPosMin = scrollRect.viewport.rect.min;
            var viewPosMax = scrollRect.viewport.rect.max;

            Vector2 childPosMin = scrollRect.viewport.InverseTransformPoint(child.TransformPoint(child.rect.min));
            Vector2 childPosMax = scrollRect.viewport.InverseTransformPoint(child.TransformPoint(child.rect.max));

            // Apply the custom margins
            childPosMin.x -= margin.left;
            childPosMin.y -= margin.bottom;
            childPosMax.x += margin.right;
            childPosMax.y += margin.top;

            var move = Vector2.zero;

            // Check if one (or more) of the child bounding edges goes outside the viewport and
            // calculate move vector for the content rect so it can keep it visible.
            if (childPosMax.y > viewPosMax.y)
            {
                move.y = childPosMax.y - viewPosMax.y;
            }

            if (childPosMin.x < viewPosMin.x)
            {
                move.x = childPosMin.x - viewPosMin.x;
            }

            if (childPosMax.x > viewPosMax.x)
            {
                move.x = childPosMax.x - viewPosMax.x;
            }

            if (childPosMin.y < viewPosMin.y)
            {
                move.y = childPosMin.y - viewPosMin.y;
            }

            // Transform the move vector to world space, then to content local space (in case of scaling or rotation?) and apply it.
            var worldMove = scrollRect.viewport.TransformDirection(move);
            var inverse = scrollRect.content.InverseTransformDirection(worldMove);
            var targetPosition = scrollRect.content.localPosition - inverse;

            if (CanPerformAutoScroll(scrollRect, targetPosition) is false)
            {
                return;
            }

            scrollRect.content.localPosition = targetPosition;
        }

        private static bool CanPerformAutoScroll(ScrollRect scrollRect, Vector3 targetPosition)
        {
            if (targetPosition.ApproximatelyEquals(scrollRect.content.localPosition))
            {
                return false;
            }

            var contentRect = scrollRect.content.rect;
            var viewportRect = scrollRect.viewport.rect;

            if (scrollRect.horizontal)
            {
                var minX = Mathf.Min(viewportRect.width - contentRect.width, 0);
                if (targetPosition.x > 0 || targetPosition.x < minX)
                {
                    return false;
                }
            }

            if (scrollRect.vertical)
            {
                var minY = Mathf.Min(viewportRect.height - contentRect.height, 0);
                if (targetPosition.y > 0 || targetPosition.y < minY)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion


        #region Component

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPosition<TComponent>(this TComponent component, Vector3 position)
            where TComponent : Component
        {
            component.transform.position = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndRotation<TComponent>(this TComponent component, Vector3 position,
            Quaternion rotation) where TComponent : Component
        {
            component.transform.SetPositionAndRotation(position, rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndRotation<TComponent>(this TComponent component, Transform target)
            where TComponent : Component
        {
            component.transform.SetPositionAndRotation(target.position, target.rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndRotation(this GameObject gameObject, Transform target)
        {
            gameObject.transform.SetPositionAndRotation(target.position, target.rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale<TComponent>(this TComponent component, float scale) where TComponent : Component
        {
            component.transform.localScale = Vector3.one * scale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale<TComponent>(this TComponent component, Vector3 scale) where TComponent : Component
        {
            component.transform.localScale = scale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetActive<TComponent>(this TComponent component, bool activeState) where TComponent : Component
        {
            component.gameObject.SetActive(activeState);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent<TComponent>(this TComponent component, Transform parent, bool worldPositionStays = true)
            where TComponent : Component
        {
            component.transform.SetParent(parent, worldPositionStays);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DontDestroyOnLoad<TComponent>(this TComponent component)
            where TComponent : Component
        {
            component.SetParent(null);
            Object.DontDestroyOnLoad(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActive<TComponent>(this TComponent component) where TComponent : Component
        {
            return component.gameObject.activeSelf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActiveInHierarchy<TComponent>(this TComponent component) where TComponent : Component
        {
            return component != null && component.gameObject.activeInHierarchy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotActive<TComponent>(this TComponent component) where TComponent : Component
        {
            return !component.gameObject.activeSelf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotActiveInHierarchy<TComponent>(this TComponent component) where TComponent : Component
        {
            return component == null || !component.gameObject.activeInHierarchy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyGameObject<TComponent>(this TComponent component) where TComponent : Component
        {
            Object.Destroy(component.gameObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyGameObject<TComponent>(this TComponent component, float secondsDelay)
            where TComponent : Component
        {
            Object.Destroy(component.gameObject, secondsDelay);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponents<TComponent>(this GameObject gameObject, out TComponent[] components)
            where TComponent : Component
        {
            components = gameObject.GetComponents<TComponent>();
            return components.IsNotNullOrEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponents(this GameObject gameObject, Type componentType, out Component[] components)
        {
            components = gameObject.GetComponents(componentType);
            return components.IsNotNullOrEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponentInChildren<T>(this Component target, out T component,
            bool includeInactive = false)
        {
            component = target.GetComponentInChildren<T>(includeInactive);
            return component != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponentInParent<T>(this Component target, out T component,
            bool includeInactive = false)
        {
            component = target.GetComponentInParent<T>(includeInactive);
            return component != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrAddComponent<T>(this Component target) where T : Component
        {
            return target.gameObject.GetOrAddComponent<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform GetParent<TComponent>(this TComponent component) where TComponent : Component
        {
            return component.transform.parent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<TComponent>(this Component target) where TComponent : Component
        {
            return target.GetComponent<TComponent>() is not null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<TComponent>(this GameObject gameObject) where TComponent : Component
        {
            return gameObject.GetComponent<TComponent>() is not null;
        }

        #endregion


        #region GameObject

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DontDestroyOnLoad(this GameObject gameObject, bool setParent = true)
        {
            if (setParent)
            {
                gameObject.transform.SetParent(null);
            }

            Object.DontDestroyOnLoad(gameObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrefab(this GameObject gameObject)
        {
            return gameObject.scene.rootCount == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrAddComponent<T>(this GameObject target) where T : Component
        {
            if (!target.TryGetComponent(out T component))
            {
                component = target.AddComponent<T>();
            }

            return component;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LayerMaskToIndex(this LayerMask layerMask)
        {
            return Mathf.RoundToInt(Mathf.Log(layerMask.value, 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        #endregion


        #region Layer

        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMatch(this LayerMask layerMask, int layer)
        {
            return (layerMask.value & (1 << layer)) != 0;
        }

        #endregion


        #region Object

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSelected(this Object obj)
        {
#if UNITY_EDITOR
            return UnityEditor.Selection.activeObject == obj;
#else
            return false;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void SetObjectDirty(this Object target)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(target);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetPosition(this Object obj, Vector3 position)
        {
            switch (obj)
            {
                case Component component:
                    component.transform.position = position;
                    return true;

                case GameObject gameObject:
                    gameObject.transform.position = position;
                    return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetActive(this GameObject obj, bool activeState)
        {
            if (obj != null)
            {
                obj.SetActive(activeState);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPosition(this GameObject gameObject, Vector3 position)
        {
            gameObject.transform.position = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale(this GameObject gameObject, float scale)
        {
            gameObject.transform.localScale = new Vector3(scale, scale, scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrefab(this Object obj)
        {
#if UNITY_EDITOR
            return UnityEditor.PrefabUtility.GetPrefabInstanceStatus(obj) !=
                   UnityEditor.PrefabInstanceStatus.NotAPrefab;
#else
        return false;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGameObjectInScene(this Object obj)
        {
            if (obj is GameObject gameObject)
            {
                return gameObject.scene.name != null;
            }

            return false;
        }

        #endregion


        #region Rect

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithOffset(this Rect rect, float x = 0, float y = 0, float width = 0, float height = 0)
        {
            return new Rect(rect.x + x, rect.y + y, rect.width + width, rect.height + height);
        }

        public static bool Contains(this Rect rect1, Rect rect2)
        {
            if (rect1.position.x <= rect2.position.x &&
                rect1.position.x + rect1.size.x >= rect2.position.x + rect2.size.x &&
                rect1.position.y <= rect2.position.y &&
                rect1.position.y + rect1.size.y >= rect2.position.y + rect2.size.y)
            {
                return true;
            }

            return false;
        }

        public static bool Overlaps(this RectTransform a, RectTransform b)
        {
            var corners = new Vector3[4];
            a.GetWorldCorners(corners);
            var rec = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);

            b.GetWorldCorners(corners);
            var rec2 = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);

            return rec.Overlaps(rec2);
        }

        public static bool Contains(this RectTransform a, RectTransform b)
        {
            var corners = new Vector3[4];
            a.GetWorldCorners(corners);
            var rec = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);

            b.GetWorldCorners(corners);
            var rec2 = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);

            return rec.Contains(rec2);
        }

        #endregion


        #region Color

        public static Color WithAlpha(this Color color, float alpha)
        {
            ref var c = ref color;
            c.a = alpha;
            return color;
        }

        #endregion
    }
}