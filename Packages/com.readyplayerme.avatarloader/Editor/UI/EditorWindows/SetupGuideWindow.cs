using ReadyPlayerMe.Core.Analytics;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    [InitializeOnLoad]
    public class SetupGuideWindow : EditorWindow
    {
        private const string WINDOW_NAME = "Setup Guide";
        private const int BUTTON_FONT_SIZE = 12;

        public const string FIRST_TIME_SETUP_DONE = "first-time-setup";
        private const string BACK_BUTTON_LABEL = "Back";
        private const string FINISH_SETUP_BUTTON_LABEL = "Finish Setup";
        private const string OPEN_QUICKSTART_SCENE_BUTTON_LABEL = "Open QuickStart Scene";

        private Header header;
        private IEditorWindowComponent[] panels;
        private int currentPanelIndex;

        /// <summary>
        ///     Constructor method that subscribes to the StartUp event.
        /// </summary>
        static SetupGuideWindow()
        {
            EditorApplication.update += OnStartup;
        }

        /// <summary>
        ///     This method is called when a Unity project is opened or after this Unity package has finished importing and is
        ///     responsible for displaying the window. It also calls analytics events if enabled.
        /// </summary>
        private static void OnStartup()
        {
            if (ProjectPrefs.GetBool(FIRST_TIME_SETUP_DONE)) return;
            if (AnalyticsEditorLogger.IsEnabled)
            {
                EditorApplication.quitting += OnQuit;
            }
            AnalyticsEditorLogger.Enable();
            ShowWindow();

            EditorApplication.update -= OnStartup;
        }

        /// <summary>
        ///     This method is called when the Unity Editor is closed and logs the close event.
        /// </summary>
        private static void OnQuit()
        {
            AnalyticsEditorLogger.EventLogger.LogCloseProject();
        }

        [MenuItem("Ready Player Me/Re-run Setup")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SetupGuideWindow), false, WINDOW_NAME);
            ProjectPrefs.SetBool(FIRST_TIME_SETUP_DONE, true);
        }

        /// <summary>
        ///     This method the panels and banner if not set already and sets the button event listeners
        /// </summary>
        private void LoadPanels()
        {
            panels ??= new IEditorWindowComponent[]
            {
                new SubdomainPanel(),
                new AvatarConfigPanel(),
                new AnalyticsPanel()
            };

            header ??= new Header(WINDOW_NAME);
        }

        private void OnGUI()
        {
            LoadPanels();
            DrawContent();
        }

        private GUIStyle GetButtonStyle()
        {
            return new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                fontSize = BUTTON_FONT_SIZE,
                padding = new RectOffset(10, 10, 5, 5),
                fixedHeight = 30,
                stretchWidth = true
            };
        }

        private void DrawContent()
        {
            minSize = new Vector2(460, 400);

            Layout.Horizontal(() =>
            {
                GUILayout.FlexibleSpace();

                Layout.Vertical(() =>
                {
                    header.Draw(position);
                    panels[currentPanelIndex].Draw(position);
                    GUILayout.FlexibleSpace();
                    DrawFooter(panels[currentPanelIndex]);
                }, false, GUILayout.Height(400));
            });
        }

        private void DrawFooter(IEditorWindowComponent panel)
        {
            Layout.Horizontal(() =>
            {
                if (!(panel is SubdomainPanel))
                {
                    GUILayout.Space(15);
                    if (GUILayout.Button(BACK_BUTTON_LABEL, GetButtonStyle()))
                    {
                        OnBackButton();
                    }
                }

                GUILayout.FlexibleSpace();

                if (panel is AnalyticsPanel)
                {
                    if (GUILayout.Button(FINISH_SETUP_BUTTON_LABEL, GetButtonStyle()))
                    {
                        OnFinishSetup();
                    }

                    if (GUILayout.Button(OPEN_QUICKSTART_SCENE_BUTTON_LABEL, GetButtonStyle()))
                    {
                        OnOpenQuickStartScene();
                    }
                }
                else
                {
                    if (panel is SubdomainPanel { IsSubdomainFieldEmpty: true })
                    {
                        GUI.enabled = false;
                    }

                    if (panel is AvatarConfigPanel { IsAvatarConfigFieldEmpty: true })
                    {
                        GUI.enabled = false;
                    }

                    if (GUILayout.Button("Next", GetButtonStyle()))
                    {
                        OnNextButton();
                    }
                    GUI.enabled = true;
                }

                GUILayout.Space(15);

            }, true, GUILayout.Width(460));
        }

        private void OnBackButton()
        {
            currentPanelIndex--;
        }

        private void OnNextButton()
        {
            if (panels[currentPanelIndex] is SubdomainPanel)
            {
                var subdomainPanel = (SubdomainPanel) panels[currentPanelIndex];
                subdomainPanel.SaveSubdomain();
            }
            currentPanelIndex++;
        }

        private void OnFinishSetup()
        {
            Close();
        }

        private void OnOpenQuickStartScene()
        {
            if (!new QuickStartHelper().Open())
            {
                EditorUtility.DisplayDialog(WINDOW_NAME, "No quick start sample found.", "OK");
            }
        }
    }

}
