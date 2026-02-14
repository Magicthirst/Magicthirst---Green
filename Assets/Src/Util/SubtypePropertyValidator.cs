#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using static System.Reflection.BindingFlags;

namespace Util
{
    public class SubtypePropertyValidator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled);

            foreach (var scene in scenes)
            {
                var openedScene = EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                ValidateScene(openedScene);
                EditorSceneManager.CloseScene(openedScene, true);
            }
        }

        private void ValidateScene(UnityEngine.SceneManagement.Scene scene)
        {
            var behaviours = scene
                .GetRootGameObjects()
                .SelectMany(g => g.GetComponentsInChildren<MonoBehaviour>(true))
                .WhereNotNull();

            foreach (var behaviour in behaviours)
            {
                var fields = behaviour.GetType()
                    .GetFields(Instance | Public | NonPublic)
                    .Where(f => Attribute.IsDefined(f, typeof(SubtypePropertyAttribute)));

                foreach (var field in fields)
                {
                    var value = field.GetValue(behaviour) as string;

                    if (string.IsNullOrEmpty(value) || Type.GetType(value) == null)
                    {
                        throw new BuildFailedException(
                            $"Build blocked.\n" +
                            $"Scene: {scene.path}\n" +
                            $"GameObject: {behaviour.gameObject.name}\n" +
                            $"Component: {behaviour.GetType().Name}\n" +
                            $"Field: {field.Name}\n\n" +
                            $"Subtype is not specified or invalid.");
                    }
                }
            }
        }
    }
}
#endif