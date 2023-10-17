using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Analytics;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class AvatarLoaderEditorWindow : EditorWindowBase
    {
        private const string VOICE_TO_ANIM_SAVE_KEY = "VoiceToAnimSaveKey";
        private const string EYE_ANIMATION_SAVE_KEY = "EyeAnimationSaveKey";
        private const string MODEL_CACHING_SAVE_KEY = "ModelCachingSaveKey";

        private const string EDITOR_WINDOW_NAME = "avatar loader";
        private const string WINDOW_HEADING = "Avatar Loader";

        private bool useVoiceToAnim;
        private bool useEyeAnimations;
        private bool initialized;

        private readonly GUILayoutOption fieldHeight = GUILayout.Height(20);

        private GUIStyle errorButtonStyle;
        private GUIStyle avatarButtonStyle;
        private GUIStyle parametersSelectButtonStyle;

        private AvatarLoaderSettings avatarLoaderSettings;
        private AvatarUrlField avatarUrlField;

        private double startTime;
        
        [MenuItem("Ready Player Me/Avatar Loader", priority = 0)]
        public static void ShowWindowMenu()
        {
            var window = (AvatarLoaderEditorWindow) GetWindow(typeof(AvatarLoaderEditorWindow));
            window.titleContent = new GUIContent(WINDOW_HEADING);
            window.ShowUtility();
            AnalyticsEditorLogger.EventLogger.LogOpenDialog(EDITOR_WINDOW_NAME);
        }

        private void OnGUI()
        {
            if (!initialized) Initialize();
            if (avatarUrlField == null)
            {
                Initialize();
            }
            LoadStyles();
            DrawContent(DrawContent, false);
        }

        private void OnFocus()
        {
            avatarUrlField?.ValidateUrl();
        }

        private void DrawContent()
        {
            Layout.Vertical(() =>
            {
                avatarUrlField.Draw();
                DrawExtras();
                DrawLoadAvatarButton();
            });
        }

        private void LoadStyles()
        {
            if (avatarButtonStyle == null)
            {
                avatarButtonStyle = new GUIStyle(GUI.skin.button);
                avatarButtonStyle.fontStyle = FontStyle.Bold;
                avatarButtonStyle.fontSize = 14;
                avatarButtonStyle.margin = new RectOffset(15, 15, 0, 0);
                avatarButtonStyle.fixedHeight = ButtonHeight;
            }

            if (parametersSelectButtonStyle == null)
            {
                parametersSelectButtonStyle = new GUIStyle(GUI.skin.button);
                parametersSelectButtonStyle.fontStyle = FontStyle.Bold;
                parametersSelectButtonStyle.fontSize = 10;
                parametersSelectButtonStyle.fixedHeight = 18;
                parametersSelectButtonStyle.fixedWidth = 60;
            }

            if (errorButtonStyle == null)
            {
                errorButtonStyle = new GUIStyle();
                errorButtonStyle.fixedWidth = 20;
                errorButtonStyle.fixedHeight = 20;
                errorButtonStyle.margin = new RectOffset(2, 0, 2, 2);
            }
        }

        private void Initialize()
        {
            avatarUrlField = new AvatarUrlField();
            useEyeAnimations = EditorPrefs.GetBool(EYE_ANIMATION_SAVE_KEY);
            useVoiceToAnim = EditorPrefs.GetBool(VOICE_TO_ANIM_SAVE_KEY);

            if (EditorPrefs.GetBool(MODEL_CACHING_SAVE_KEY)) EditorPrefs.SetBool(MODEL_CACHING_SAVE_KEY, false);
            SetEditorWindowName(EDITOR_WINDOW_NAME,WINDOW_HEADING);
            initialized = true;
        }

        private void DrawExtras()
        {
            Layout.Vertical(() =>
            {
                GUILayout.Label("Extras", HeadingStyle);

                Layout.Horizontal(() =>
                {
                    GUILayout.Space(15);

                    Layout.Vertical(() =>
                    {
                        useEyeAnimations = EditorGUILayout.ToggleLeft(new GUIContent("Use Eye Animations",
                                "Optional helper component for random eye rotation and blinking, for a less static look."), useEyeAnimations,
                            fieldHeight);
                        EditorPrefs.SetBool(EYE_ANIMATION_SAVE_KEY, useEyeAnimations);

                        useVoiceToAnim = EditorGUILayout.ToggleLeft(new GUIContent("Use Voice To Animation",
                            "Optional helper component for voice amplitude to jaw bone movement."), useVoiceToAnim, fieldHeight);
                        EditorPrefs.SetBool(VOICE_TO_ANIM_SAVE_KEY, useVoiceToAnim);
                    });
                });
                GUILayout.Space(2);
            });
        }


        private void DrawLoadAvatarButton()
        {
            Layout.Horizontal(() =>
            {
                GUI.enabled = avatarUrlField.IsValidUrlShortCode;
                if (GUILayout.Button("Load Avatar into the Current Scene", avatarButtonStyle))
                {
                    startTime = EditorApplication.timeSinceStartup;
                    AnalyticsEditorLogger.EventLogger.LogLoadAvatarFromDialog(avatarUrlField.Url, useEyeAnimations, useVoiceToAnim);
                    if (avatarLoaderSettings == null)
                    {
                        avatarLoaderSettings = AvatarLoaderSettings.LoadSettings();
                    }
                    var avatarLoader = new AvatarObjectLoader();
                    avatarLoader.SaveInProjectFolder = true;
                    avatarLoader.OnFailed += Failed;
                    avatarLoader.OnCompleted += Completed;
                    avatarLoader.OperationCompleted += OnOperationCompleted;
                    if (avatarLoaderSettings != null)
                    {
                        avatarLoader.AvatarConfig = avatarLoaderSettings.AvatarConfig;
                        if (avatarLoaderSettings.GLTFDeferAgent != null)
                        {
                            avatarLoader.GLTFDeferAgent = avatarLoaderSettings.GLTFDeferAgent;
                        }
                    }
                    avatarLoader.LoadAvatar(avatarUrlField.Url);
                }

                GUI.enabled = true;

                GUILayout.Space(4);
            });
        }

        private void OnOperationCompleted(object sender, IOperation<AvatarContext> e)
        {
            if (e.GetType() == typeof(MetadataDownloader))
            {
                AnalyticsEditorLogger.EventLogger.LogMetadataDownloaded(EditorApplication.timeSinceStartup - startTime);
            }
        }

        private void Failed(object sender, FailureEventArgs args)
        {
            Debug.LogError($"{args.Type} - {args.Message}");
        }

        private void Completed(object sender, CompletionEventArgs args)
        {
            GameObject avatar = args.Avatar;

            if (useEyeAnimations) avatar.AddComponent<EyeAnimationHandler>();
            if (useVoiceToAnim) avatar.AddComponent<VoiceHandler>();

            EditorUtilities.CreatePrefab(avatar, $"{DirectoryUtility.GetRelativeProjectPath(avatar.name)}/{avatar.name}.prefab");

            Selection.activeObject = args.Avatar;
            AnalyticsEditorLogger.EventLogger.LogAvatarLoaded(EditorApplication.timeSinceStartup - startTime);
        }
    }
}
