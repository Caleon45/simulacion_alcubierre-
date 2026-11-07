#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace Alcubierre.Editor
{
    [InitializeOnLoad]
    public static class DefaultSceneLoader
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";

        static DefaultSceneLoader()
        {
            EditorApplication.delayCall += OnEditorLoaded;
        }

        private static void OnEditorLoaded()
        {
            // Garantiza que al presionar Play siempre se inicie la escena de la simulación
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            if (sceneAsset != null)
            {
                EditorSceneManager.playModeStartScene = sceneAsset;
            }

            // Si al abrir Unity no hay escena cargada o es una escena nueva sin guardar, abre la simulación
            if (UnityEngine.SceneManagement.SceneManager.loadedSceneCount == 0 || 
                string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))
            {
                if (File.Exists(ScenePath))
                {
                    EditorSceneManager.OpenScene(ScenePath);
                }
            }
        }
    }
}
#endif
