using System;
using ReadyPlayerMe.Core.Editor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public static class HeadingAndDescriptionField
    {
        private static GUIStyle descriptionStyle;
        private static GUIStyle headingStyle;

        private static void LoadStyles()
        {
            headingStyle ??= new GUIStyle
            {
                fontSize = 14,
                richText = true,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(15, 0, 0, 0),
                normal =
                {
                    textColor = Color.white
                }
            };

            descriptionStyle ??= new GUIStyle(GUI.skin.box)
            {
                fontSize = 12,
                richText = true,
                wordWrap = true,
                fixedWidth = 435,
                margin = new RectOffset(15, 15, 0, 0),
                alignment = TextAnchor.UpperLeft,
                normal =
                {
                    textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f)
                }
            };
        }

        public static void SetDescription(string heading, string description, Action extraDescription = null)
        {
            LoadStyles();
            Layout.Vertical(() =>
            {
                GUILayout.Label(heading, headingStyle);
                GUILayout.Space(10);
                GUILayout.Label(description, descriptionStyle);
                extraDescription?.Invoke();
            });
        }
    }
}
