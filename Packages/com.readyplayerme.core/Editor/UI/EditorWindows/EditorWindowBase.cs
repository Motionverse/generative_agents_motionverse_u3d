using System;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Core.Editor
{
    /// <summary>
    ///     This <c>EditorWindow</c> class is used as a base class for Ready Player Me Editor windows and contains
    ///     functionality that is shared between each child class.
    /// </summary>
    public class EditorWindowBase : EditorWindow
    {
        private const string SUPPORT_HEADING = "Support";
        private const float WIDTH = 460;
        private readonly GUILayoutOption windowWidth = GUILayout.Width(WIDTH);

        protected readonly float ButtonHeight = 30f;
        
        protected GUIStyle HeadingStyle;

        private GUIStyle webButtonStyle;

        private Header header;
        private Footer footer;

        private string editorWindowName;
        private bool windowResized;
        private string heading;

        private void LoadAssets()
        {
            header ??= new Header(heading);

            footer ??= new Footer(editorWindowName);

            HeadingStyle ??= new GUIStyle
            {
                fontSize = 14,
                richText = true,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(15, 0, 0, 8),
                normal =
                {
                    textColor = Color.white
                }
            };

            webButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fixedWidth = 149,
                fixedHeight = ButtonHeight,
                padding = new RectOffset(5, 5, 5, 5)
            };
        }

        protected void SetEditorWindowName(string editorName, string headingText)
        {
            heading = headingText;
            editorWindowName = editorName;

        }

        protected void DrawContent(Action content, bool useFooter = true)
        {
            LoadAssets();

            Layout.Horizontal(() =>
            {
                GUILayout.FlexibleSpace();
                Layout.Vertical(() =>
                {
                    header.Draw(position);
                    content?.Invoke();
                    if (useFooter)
                    {
                        Layout.Vertical(() =>
                        {
                            GUILayout.Label(SUPPORT_HEADING, HeadingStyle);
                            footer.Draw();
                        }, true);
                    }
                }, false, windowWidth);
                GUILayout.FlexibleSpace();
            });

            SetWindowSize();
        }

        private void SetWindowSize()
        {
            var height = GUILayoutUtility.GetLastRect().height;
            if (!windowResized && height > 1)
            {
                minSize = maxSize = new Vector2(WIDTH, height);
                windowResized = true;
                
            }
        }
    }
}
