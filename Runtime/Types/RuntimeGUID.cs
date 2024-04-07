using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Types
{
    [Serializable]
    public struct RuntimeGUID
    {
        #region Fields & Properties

        public string Value => value;
        public int Hash => hash == 0 ? hash = value.GetHashCode() : hash;

        [SerializeField] private string value;
        [SerializeField] private int hash;

        #endregion


        #region Ctor & Creation

        public RuntimeGUID(string value)
        {
            this.value = value;
            hash = value.GetHashCode();
        }

        /// <summary>
        ///     Updates a RuntimeGUID if it has no value.
        /// </summary>
        /// <param name="unityObject">The guid target object</param>
        /// <param name="guid">Reference to the runtime guid field</param>
        public static void Create(Object unityObject, ref RuntimeGUID guid)
        {
#if UNITY_EDITOR
            if (guid.value.IsNotNullOrWhitespace())
            {
                return;
            }

            if (UnityEditor.PrefabUtility.IsPartOfAnyPrefab(unityObject))
            {
                var prefabPath = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(unityObject);
                var prefabGuid = UnityEditor.AssetDatabase.AssetPathToGUID(prefabPath);
                guid = new RuntimeGUID(prefabGuid);
            }

            var path = UnityEditor.AssetDatabase.GetAssetPath(unityObject);
            var assetGuid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            guid = new RuntimeGUID(assetGuid);
#endif
        }

        #endregion


        #region Operator

        public static implicit operator RuntimeGUID(string value)
        {
            return new RuntimeGUID(value);
        }

        public static implicit operator string(RuntimeGUID guid)
        {
            return guid.value;
        }

#if UNITY_EDITOR
        public static explicit operator RuntimeGUID(UnityEditor.GUID value)
        {
            return new RuntimeGUID(value.ToString());
        }

#endif

        #endregion


        #region ToString

        public override string ToString()
        {
            return value;
        }

        #endregion
    }
}