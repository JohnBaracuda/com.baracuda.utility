using Baracuda.Bedrock.Scenes;
using UnityEngine;
using SceneUtility = UnityEngine.SceneManagement.SceneUtility;

namespace Baracuda.Utilities.Editor.Drawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            var sceneProperty = property.FindPropertyRelative("sceneAsset");
            var pathProperty = property.FindPropertyRelative("scenePath");
            var indexProperty = property.FindPropertyRelative("buildIndex");

            var sceneAsset = sceneProperty.objectReferenceValue;
            var scenePath = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);

            var buildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
            if (buildIndex == -1)
            {
                UnityEditor.EditorGUILayout.HelpBox("The scene is not part of the build settings!", UnityEditor.MessageType.Warning);
            }

            UnityEditor.EditorGUI.BeginChangeCheck();
            var newSceneAsset = UnityEditor.EditorGUILayout.ObjectField(label, sceneAsset, typeof(UnityEditor.SceneAsset), false) as UnityEditor.SceneAsset;

            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                var newScenePath = UnityEditor.AssetDatabase.GetAssetPath(newSceneAsset);
                var newBuildIndex = SceneUtility.GetBuildIndexByScenePath(newScenePath);
                pathProperty.stringValue = newScenePath;
                indexProperty.intValue = newBuildIndex;
                sceneProperty.objectReferenceValue = newSceneAsset;
            }
            else
            {
                // Update fields in case path/index has changed
                pathProperty.stringValue = scenePath;
                indexProperty.intValue = buildIndex;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}