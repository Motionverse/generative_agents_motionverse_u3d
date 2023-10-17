using UnityEngine;
using UnityEditor;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Editor;
using ReadyPlayerMe.Core.Analytics;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class SettingsEditorWindow : EditorWindowBase
    {
        private const string EDITOR_WINDOW_NAME = "rpm settings";
        private const string WINDOW_HEADING = "Settings";
        private const string PARTNER_SETTINGS_HEADING = "Partner Settings";
        private const string HELP_TEXT =
            "If you are a Ready Player Me partner, please enter your subdomain here to apply your configuration to the WebView.";
        private const string OTHER_SECTION_HEADING = "Other";
        private const string ANALYTICS_LOGGING_DESCRIPTION =
            "We are constantly adding new features and improvements to our SDK. Enable analytics and help us in building even better free tools for more developers. This data is used for internal purposes only and is not shared with third parties.";
        private const string ANALYTICS_PRIVACY_TOOLTIP = "Click to read our Privacy Policy.";
        private const string LOGGING_ENABLED_TOOLTIP = "Enable for detailed console logging of RPM Unity SDK at Runtime and in Editor.";
        private const string ANALYTICS_PRIVACY_URL =
            "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/help-us-improve-the-unity-sdk";
        private const string CACHING_DOCS_LINK = "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/optimize/avatar-caching";

        private bool initialized;
        private bool analyticsEnabled;
        private bool sdkLoggingEnabled;

        private GUIStyle saveButtonStyle;
        private GUIStyle privacyPolicyStyle;

        private AvatarConfig avatarConfig;

        private SubdomainField subdomainField;
        private AvatarConfigFields avatarConfigFields;

        [MenuItem("Ready Player Me/Settings", priority = 1)]
        public static void ShowWindowMenu()
        {
            var window = (SettingsEditorWindow) GetWindow(typeof(SettingsEditorWindow));
            window.titleContent = new GUIContent(WINDOW_HEADING);
            window.ShowUtility();

            AnalyticsEditorLogger.EventLogger.LogOpenDialog(EDITOR_WINDOW_NAME);
        }

        private void Initialize()
        {
            SetEditorWindowName(EDITOR_WINDOW_NAME, WINDOW_HEADING);

            subdomainField = new SubdomainField();
            avatarConfigFields = new AvatarConfigFields();

            analyticsEnabled = AnalyticsEditorLogger.IsEnabled;

            initialized = true;
            sdkLoggingEnabled = SDKLogger.IsLoggingEnabled();
        }

        private void OnFocus()
        {
            avatarConfigFields?.SetIsCacheEmpty();
        }

        private void OnGUI()
        {
            if (!initialized) Initialize();
            if (subdomainField == null)
            {
                Initialize();
            }

            LoadStyles();
            DrawContent(DrawContent);
        }

        private void LoadStyles()
        {
            saveButtonStyle ??= new GUIStyle(GUI.skin.button);
            saveButtonStyle.fontSize = 14;
            saveButtonStyle.fontStyle = FontStyle.Bold;
            saveButtonStyle.fixedWidth = 449;
            saveButtonStyle.fixedHeight = ButtonHeight;
            saveButtonStyle.padding = new RectOffset(5, 5, 5, 5);

            privacyPolicyStyle ??= new GUIStyle(GUI.skin.label);
            privacyPolicyStyle.fontStyle = FontStyle.Bold;
            privacyPolicyStyle.fontSize = 12;
            privacyPolicyStyle.fixedWidth = 100;
        }

        private void DrawContent()
        {
            Layout.Vertical(() =>
            {
                DrawPartnerSettings();
                DrawAvatarSettings();
                DrawAvatarCaching();
                DrawOtherSection();
            });
        }

        private void DrawPartnerSettings()
        {
            Layout.Vertical(() =>
            {
                GUILayout.Label(new GUIContent(PARTNER_SETTINGS_HEADING, HELP_TEXT), HeadingStyle);
                GUILayout.Space(2);
                subdomainField?.Draw();
            }, true);
        }

        private void DrawAvatarSettings()
        {
            Layout.Vertical(() =>
            {
                GUILayout.Label(new GUIContent("Avatar Settings"), HeadingStyle);

                avatarConfigFields?.DrawAvatarConfig();
                EditorGUILayout.Space(3);
                avatarConfigFields?.DrawGltfDeferAgent();

            }, true);
        }

        private void DrawAvatarCaching()
        {
            Layout.Vertical(() =>
            {
                Layout.Horizontal(() =>
                {
                    GUILayout.Label("Avatar Caching", HeadingStyle);
                    DocumentationButton.Draw(CACHING_DOCS_LINK);
                    GUILayout.FlexibleSpace();
                });

                avatarConfigFields?.DrawAvatarCaching();
            }, true);
        }

        private void DrawOtherSection()
        {
            Layout.Vertical(() =>
            {
                GUILayout.Label(OTHER_SECTION_HEADING, HeadingStyle);

                Layout.Horizontal(() =>
                {
                    GUILayout.Space(15);
                    analyticsEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Analytics enabled", ANALYTICS_LOGGING_DESCRIPTION), analyticsEnabled, GUILayout.Width(125));

                    if (GUILayout.Button(new GUIContent("(Privacy Policy)", ANALYTICS_PRIVACY_TOOLTIP), privacyPolicyStyle))
                    {
                        Application.OpenURL(ANALYTICS_PRIVACY_URL);
                    }

                    if (AnalyticsEditorLogger.IsEnabled != analyticsEnabled)
                    {
                        if (analyticsEnabled)
                        {
                            AnalyticsEditorLogger.Enable();
                        }
                        else
                        {
                            AnalyticsEditorLogger.Disable();
                        }
                    }
                });
                Layout.Horizontal(() =>
                {
                    GUILayout.Space(15);
                    sdkLoggingEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Logging enabled", LOGGING_ENABLED_TOOLTIP), sdkLoggingEnabled, GUILayout.Width(125));
                    if (sdkLoggingEnabled != SDKLogger.IsLoggingEnabled())
                    {
                        SDKLogger.EnableLogging(sdkLoggingEnabled);
                    }
                });
            }, true);
        }
    }
}
