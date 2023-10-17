using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class SubdomainPanel : IEditorWindowComponent
    {
        private const string HEADING = "Enter your subdomain";
        private const string DESCRIPTION =
            "Please enter your subdomain here. This ensures, that you will get your avatars and the embedded avatar-creator with the configuration you set in Studio (Developer Dashboard).";
        private const string STUDIO_TEXT = "If you don't have a subdomain, you can get one by signing up in Studio.<a>(https://studio.readyplayer.me)</a>";
        private const string STUDIO_URL =
            "https://studio.readyplayer.me";
        private const string USE_DEMO_SUBDOMAIN = "I don't have an account. Use demo subdomain instead.";
        private const string DEMO_SUBDOMAIN = "demo";

        private readonly GUILayoutOption toggleWidth = GUILayout.Width(20);

        public bool IsSubdomainFieldEmpty => string.IsNullOrEmpty(subdomainField.PartnerSubdomain);

        private readonly SubdomainField subdomainField;
        private bool userDemoSubdomain;

        public SubdomainPanel()
        {
            subdomainField = new SubdomainField();
        }

        public void Draw(Rect position = new Rect())
        {
            HeadingAndDescriptionField.SetDescription(HEADING, DESCRIPTION, () =>
            {
                if (GUILayout.Button(STUDIO_TEXT, new GUIStyle(GUI.skin.label)
                    {
                        richText = true,
                        fixedWidth = 435,
                        wordWrap = true,
                        margin = new RectOffset(15, 0, 0, 0),
                        normal =
                        {
                            textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f)
                        }
                    }))
                {
                    Application.OpenURL(STUDIO_URL);
                }
                EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            });
            GUILayout.Space(20);

            subdomainField.Draw();

            GUILayout.Space(20);

            Layout.Horizontal(() =>
            {
                GUILayout.Space(15);
                userDemoSubdomain = EditorGUILayout.Toggle(userDemoSubdomain, toggleWidth);
                if (userDemoSubdomain)
                {
                    subdomainField.SetSubdomain(DEMO_SUBDOMAIN);
                }
                GUILayout.Label(USE_DEMO_SUBDOMAIN);
                GUILayout.FlexibleSpace();
            });

            GUILayout.Space(10);
        }

        public void SaveSubdomain()
        {
            subdomainField.SaveSubdomain();
        }
    }
}
