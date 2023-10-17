using System;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Core.Editor
{
    public static class Layout
    {
        public static void Vertical(Action content, bool isBox = false, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(isBox ? GUI.skin.box : GUIStyle.none,options);
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }

        public static void Horizontal(Action content, bool isBox = false,params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(isBox ? GUI.skin.box : GUIStyle.none, options);
            content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }
    }
}
