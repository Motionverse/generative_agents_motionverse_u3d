using UnityEditor;

namespace ReadyPlayerMe.Core.Analytics
{
    public static class AnalyticsEditorLogger
    {
        private const string ANALYTICS_LOGGING_ACCEPTED = "rpm-sdk-metrics-logging-accepted";

        public static readonly IAnalyticsEventLogger EventLogger;

        static AnalyticsEditorLogger()
        {
            IsEnabled = EditorPrefs.GetBool(ANALYTICS_LOGGING_ACCEPTED);
            EventLogger = new AnalyticsEventLogger(IsEnabled);
        }

        public static bool IsEnabled { get; private set; }

        public static void Enable()
        {
            IsEnabled = true;
            EventLogger.Enable();
            EditorPrefs.SetBool(ANALYTICS_LOGGING_ACCEPTED, true);
        }

        public static void Disable()
        {
            EventLogger.Disable();
            IsEnabled = false;
            EditorPrefs.SetBool(ANALYTICS_LOGGING_ACCEPTED, false);
        }
    }
}
