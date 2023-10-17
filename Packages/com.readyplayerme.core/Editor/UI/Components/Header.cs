using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Core.Editor
{
    public class Header : IEditorWindowComponent
    {
        private readonly string heading;
        private const int WIDTH = 460;
        private const int HEIGHT = 100;
        private const int FONT_SIZE = 20;

        private readonly Texture2D logo;
        private readonly GUIStyle textStyle;

        public Header(string heading)
        {
            this.heading = heading;
            logo = Resources.Load<Texture2D>("rpm_logo");
            textStyle = new GUIStyle();
            textStyle.fontSize = FONT_SIZE;
            textStyle.richText = true;
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.normal.textColor = Color.white;
            textStyle.alignment = TextAnchor.MiddleLeft;
        }

        public void Draw(Rect position)
        {
            var startPos = new Vector2((position.size.x - WIDTH) / 2, 0);
            
            var rect = new Rect(startPos.x, startPos.y, WIDTH, HEIGHT);
            EditorGUI.DrawRect(rect, Color.black);

            var versionText = new Rect(startPos.x + 15, startPos.y + 35, 40, 40);
            EditorGUI.LabelField(versionText, heading, textStyle);

            GUI.DrawTexture(new Rect(startPos.x + WIDTH - 100, startPos.y + 35, 80, 40), logo);
            GUILayout.Space(HEIGHT + 20);
        }
    }
}
