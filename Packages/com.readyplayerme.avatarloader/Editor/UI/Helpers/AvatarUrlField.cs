using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class AvatarUrlField
    {
        private const string AVATAR_HEADING = "Download Avatar into Scene";
        private const string URL_SHORTCODE_ERROR = "<color=red>Please enter a valid Avatar URL or Shortcode.<b>Read more.</b></color>";
        private const string ERROR_HELP_URL = "https://docs.readyplayer.me/ready-player-me/avatars/avatar-creator#avatar-url-and-data-format";
        private const string LOAD_AVATAR_DOCS = "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/load-avatars#save-avatars-as-npcs-in-your-project";

        private const string URL_SAVE_KEY = "UrlSaveKey";
        private const int LEFT_MARGIN = 15;

        private readonly GUILayoutOption fieldHeight = GUILayout.Height(20);
        private readonly GUILayoutOption fieldWidth = GUILayout.Width(285);
        private readonly GUILayoutOption labelWidth = GUILayout.Width(140);

        public bool IsValidUrlShortCode => isValidUrlShortcode;
        public string Url => url;

        private bool isValidUrlShortcode;
        private GUIStyle headingStyle;
        private string url;

        public AvatarUrlField()
        {
            url = EditorPrefs.GetString(URL_SAVE_KEY);
            isValidUrlShortcode = EditorUtilities.IsUrlShortcodeValid(url);
        }

        private void LoadStyles()
        {
            headingStyle ??= new GUIStyle
            {
                fontSize = 14,
                richText = true,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(LEFT_MARGIN, 0, 0, 8),
                normal =
                {
                    textColor = Color.white
                }
            };
        }


        public void Draw()
        {
            LoadStyles();
            Layout.Vertical(() =>
            {
                Layout.Horizontal(() =>
                {
                    GUILayout.Label(AVATAR_HEADING, headingStyle);
                    DocumentationButton.Draw(LOAD_AVATAR_DOCS);
                    GUILayout.FlexibleSpace();
                });

                Layout.Horizontal(() =>
                {
                    GUILayout.Space(LEFT_MARGIN);

                    EditorGUILayout.LabelField(
                        new GUIContent("Avatar URL or Shortcode", "Paste the avatar URL or shortcode received from Ready Player Me here."),labelWidth);

                    Layout.Vertical(() =>
                    {
                        var tempText = EditorUtilities.TextFieldWithPlaceholder(url, " Paste Avatar URL or shortcode here", fieldHeight, fieldWidth);

                        if (tempText != url)
                        {
                            url = tempText.Split('?')[0];
                            isValidUrlShortcode = EditorUtilities.IsUrlShortcodeValid(url);
                        }

                        if (!isValidUrlShortcode)
                        {
                            if (GUILayout.Button(URL_SHORTCODE_ERROR, new GUIStyle(GUI.skin.label)
                                {
                                    richText = true,
                                    fontSize = 10,
                                    fixedHeight = 20,
                                    normal =
                                    {
                                        textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f)
                                    }
                                }, fieldWidth))
                            {
                                Application.OpenURL(ERROR_HELP_URL);
                            }
                            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
                        }
                        else
                        {
                            GUILayout.Space(20);
                            EditorPrefs.SetString(URL_SAVE_KEY, url);
                        }
                    });
                });
            });
        }

        public void ValidateUrl()
        {
            isValidUrlShortcode = EditorUtilities.IsUrlShortcodeValid(url);
        }

    }
}
