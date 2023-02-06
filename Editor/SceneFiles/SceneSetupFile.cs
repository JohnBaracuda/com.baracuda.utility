using Baracuda.Utilities.Inspector;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Baracuda.Utilities.SceneFiles
{
    /// <summary>
    /// Editor file storing scene setups files that can be opened like scenes.
    /// </summary>
    [CreateAssetMenu(menuName = "Scenes/Multi Scene Setup", fileName = "SceneSetup")]
    public class SceneSetupFile : ScriptableObject
    {
        #region Fields

        [Foldout("Settings")]
        [SerializeField] [HideInInspector] private SceneSetup[] sceneSetups;

        #endregion

        #region GUI

        [ShowInInspector][DrawSpace]
        private string[] Paths => DrawScenePaths();

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Save & Load

        [Button]
        [Foldout("Controls")]
        public void SaveCurrentLayout()
        {
            sceneSetups = EditorSceneManager.GetSceneManagerSetup();
        }

        [Button]
        [Foldout("Controls")]
        [ConditionalShow(nameof(IsSetupAvailable))]
        public void LoadSavedLayout()
        {
            try
            {
                if (sceneSetups != null && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.RestoreSceneManagerSetup(sceneSetups);
                }
            }
            catch (ArgumentException exception)
            {
               Debug.LogWarning($"Warning: Did you move the scene files? \nException was thrown: {exception}");
            }
        }

        #endregion

        #region Open Scene Template

        [OnOpenAsset]
        public static bool OpenSceneTemplate(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is SceneSetupFile sceneTemplate)
            {
                sceneTemplate.LoadSavedLayout();
            }

            return false;
        }

        #endregion

        #region Editor GUI

        private bool ValidateLayout()
        {
            if (sceneSetups == null)
            {
                return true;
            }

            for (var i = 0; i < sceneSetups.Length; i++)
            {
                var sceneSetup = sceneSetups[i];
                if (!File.Exists(sceneSetup.path))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsSetupAvailable() => sceneSetups is {Length: > 0};

        private string[] DrawScenePaths()
        {
            if (sceneSetups == null)
            {
                return Array.Empty<string>();
            }

            var paths = new string[sceneSetups.Length];
            for (var i = 0; i < sceneSetups.Length; i++)
            {
                paths[i] = sceneSetups[i].path;
            }

            return paths;
        }

        #endregion
    }
}