#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
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
            var anyProblems = false;
            var problems = scene
                .GetRootGameObjects()
                .SelectMany(g => g.GetComponentsInChildren<MonoBehaviour>(true))
                .WhereNotNull()
                .SelectMany(behaviour => behaviour.GetType()
                    .GetFields(Instance | Public | NonPublic)
                    .Where(field => GetAttribute(field)?.Required == true)
                    .Where(field =>
                    {
                        var value = field.GetValue(behaviour) as string;
                        return string.IsNullOrEmpty(value) || Type.GetType(value) == null;
                    })
                    .Select(field => (behaviour, field)))
                .Select(p =>
                    $"\tScene: {scene.path}\n" +
                    $"\tGameObject: {p.behaviour.gameObject.name}\n" +
                    $"\tComponent: {p.behaviour.GetType().Name}\n" +
                    $"\tField: {p.field.Name}\n\n" +
                    $"\tSubtype is not specified or invalid.")
                .Where(_ => anyProblems = true);

            if (!anyProblems)
            {
                var errors = string.Join("\n\n", problems);
                throw new BuildFailedException($"Build blocked due to SubtypePropertyAttribute validation errors:\n{errors}");
            }

            SubtypePropertyAttribute GetAttribute(FieldInfo f)
            {
                return (SubtypePropertyAttribute) f.GetCustomAttribute(typeof(SubtypePropertyAttribute));
            }
        }
    }
}
#endif