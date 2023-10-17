using ReadyPlayerMe.Core.Analytics;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class AnalyticsPanel : IEditorWindowComponent
    {
        private const string HEADING = "Help us improve Ready Player Me SDK";
        private const string DESCRIPTION =
            "We are constantly adding new features and improvements to our SDK. Enable analytics and help us in building even better free tools for more developers. This data is used for internal purposes only and is not shared with third parties.";
        private const string ANALYTICS_PRIVACY_TEXT = "Read our Privacy Policy and learn how we use the data <a>here</a>";
        private const string ANALYTICS_PRIVACY_URL =
            "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/help-us-improve-the-unity-sdk";

        private const string ENABLE_ANALYTICS_LABEL = "Analytics Enabled";

        private static bool enableAnalytics;
        private static bool previousAnalyticsState;

        private readonly GUILayoutOption toggleWidth = GUILayout.Width(20);

        public AnalyticsPanel()
        {
            enableAnalytics = AnalyticsEditorLogger.IsEnabled;
        }

        public void Draw(Rect position = new Rect())
        {
            HeadingAndDescriptionField.SetDescription(HEADING, DESCRIPTION, () =>
            {
                GUILayout.Space(20);
                if (GUILayout.Button(ANALYTICS_PRIVACY_TEXT, new GUIStyle(GUI.skin.label)
                    {
                        richText = true,
                        fixedWidth = 435,
                        margin = new RectOffset(15, 15, 0, 0),
                        normal =
                        {
                            textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f)
                        }
                    }))
                {
                    Application.OpenURL(ANALYTICS_PRIVACY_URL);
                }
                EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            });

            GUILayout.FlexibleSpace();

            Layout.Horizontal(() =>
            {
                GUILayout.Space(15);
                enableAnalytics = EditorGUILayout.Toggle(enableAnalytics, toggleWidth);
                switch (enableAnalytics)
                {
                    case true when !previousAnalyticsState:
                        AnalyticsEditorLogger.Enable();
                        break;
                    case false when previousAnalyticsState:
                        AnalyticsEditorLogger.Disable();
                        break;
                }
                previousAnalyticsState = enableAnalytics;

                GUILayout.Label(ENABLE_ANALYTICS_LABEL);
                GUILayout.FlexibleSpace();
                ProjectPrefs.SetBool(SetupGuideWindow.FIRST_TIME_SETUP_DONE, enableAnalytics);
            });

            GUILayout.Space(10);
        }
    }
}
