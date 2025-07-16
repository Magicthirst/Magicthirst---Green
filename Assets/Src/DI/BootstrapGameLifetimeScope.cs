using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace DI
{
    [InitializeOnLoad]
    public static class BootstrapGameLifetimeScope
    {
        private const string LastPlayedSceneKey = nameof(BootstrapGameLifetimeScope) + nameof(_LastPlayedScene);

        [CanBeNull]
        private static string _LastPlayedScene
        {
            get => SessionState.GetString(LastPlayedSceneKey, null);
            set
            {
                if (value != null)
                {
                    SessionState.SetString(LastPlayedSceneKey, value);
                }
                else
                {
                    SessionState.EraseString(LastPlayedSceneKey);
                }
            }
        }

        static BootstrapGameLifetimeScope()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            string scenePath;
            
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                scenePath = SceneManager.GetActiveScene().path;
                _LastPlayedScene = scenePath;
                EditorSceneManager.OpenScene("Assets/Scenes/Bootstrap.unity");
                return;
            }

            scenePath = _LastPlayedScene;
            if (string.IsNullOrEmpty(scenePath)) return;

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                SceneManager.LoadScene(scenePath, LoadSceneMode.Additive);
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorSceneManager.OpenScene(scenePath);
                SessionState.EraseString("LastPlayedScene");
            }
        }
    }
}
