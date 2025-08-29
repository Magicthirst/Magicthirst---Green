using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Levels.Util
{
    public class TitleInspectorDecorum : MonoBehaviour
    {
        [SerializeField] private string text = "";

        public string Text
        {
            get => text;
            set => text = value;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TitleInspectorDecorum))]
    public class TitleInspectorDecorumEditor : Editor
    {
        private TitleInspectorDecorum _title;
        private GUIStyle _style;

        private void OnEnable()
        {
            _title = (TitleInspectorDecorum)target;
            _style = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18
            };
        }

        public override void OnInspectorGUI()
        {
            if (!_title) return;

            try
            {
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    _title.Text = EditorGUILayout.TextField(_title.Text) ?? "";
                    EditorGUILayout.SelectableLabel(_title.Text ?? "", _style);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
#endif
}