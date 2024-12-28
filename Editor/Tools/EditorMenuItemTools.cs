using UnityEngine;

namespace Baracuda.Utility.Editor.Tools
{
    public static class EditorMenuItemTools
    {
        [UnityEditor.MenuItem("Tools/HideFlags/Show Hidden GameObjects", priority = 2300)]
        private static void ShowAllHiddenObjects()
        {
            HideFlagsUtility.ShowAllHiddenObjects();
        }

        [UnityEditor.MenuItem("Tools/HideFlags/Show Hidden Inspector", priority = 2300)]
        private static void ShowAllHiddenInspector()
        {
            HideFlagsUtility.ShowAllHiddenInspector();
        }

        [UnityEditor.MenuItem("Tools/EditorPrefs/Clear All EditorPrefs", priority = 280)]
        private static void ClearEditorPrefs()
        {
            UnityEditor.EditorPrefs.DeleteAll();
        }

        [UnityEditor.MenuItem("Tools/PlayerPrefs/Clear All EditorPrefs", priority = 280)]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [UnityEditor.MenuItem("Tools/Delete Missing Scripts", priority = 2500)]
        private static void DeleteMissingScripts()
        {
            var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                var rootGameObject = stage.prefabContentsRoot;
                if (rootGameObject != null)
                {
                    RemoveMissingScriptsRecursively(rootGameObject);
                }
            }
            else
            {
                foreach (var gameObject in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include,
                             FindObjectsSortMode.None))
                {
                    RemoveMissingScriptsRecursively(gameObject);
                }
            }
        }

        private static void RemoveMissingScriptsRecursively(GameObject gameObject)
        {
            foreach (var component in gameObject.GetComponents<Component>())
            {
                if (component == null)
                {
                    UnityEditor.GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
                    break;
                }
            }

            // Recursively check and remove missing scripts in child gameObjects
            foreach (Transform child in gameObject.transform)
            {
                RemoveMissingScriptsRecursively(child.gameObject);
            }
        }

        [UnityEditor.MenuItem("Tools/Reload Domain", priority = -10000)]
        private static void ReloadDomain()
        {
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        [UnityEditor.MenuItem("Tools/Editor/Force Reserialize Assets", priority = 2400)]
        private static void ForceReserializeAssets()
        {
            UnityEditor.AssetDatabase.ForceReserializeAssets();
        }
    }
}