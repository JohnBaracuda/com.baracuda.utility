using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utility.Scenes
{
    [Serializable]
    public struct SceneReference
    {
        [SerializeField] private int buildIndex;
        [SerializeField] private string scenePath;
        [SerializeField] private Object sceneAsset;

        public int BuildIndex => GetBuildIndex();
        public string ScenePath => GetScenePath();

#if UNITY_EDITOR
        public UnityEditor.SceneAsset SceneAsset => sceneAsset as UnityEditor.SceneAsset;
#endif

        private string GetScenePath()
        {
            if (UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath(scenePath) == -1)
            {
                Debug.Log("SceneReference", "Updating scene path because the path has changed!");
                scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(buildIndex);
            }

            return scenePath;
        }

        private int GetBuildIndex()
        {
            var path = GetScenePath();
            var buildIndexOfPath = UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath(path);
            if (buildIndex != buildIndexOfPath)
            {
                Debug.Log("SceneReference", "Updating scene index because the path has changed!");
                buildIndex = buildIndexOfPath;
            }

            return buildIndex;
        }

        public bool IsLoaded()
        {
            return SceneUtility.IsSceneLoaded(buildIndex);
        }
    }
}