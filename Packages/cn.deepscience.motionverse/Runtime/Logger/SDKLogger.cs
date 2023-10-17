using UnityEngine;
namespace MotionverseSDK
{
    public static class SDKLogger
    {
        public static readonly Logger logger = new Logger(new CustomLogHandler());

        public static void Log(string tag, object message)
        {
            logger.Log($"<color=#12bae9ff>[{tag}]</color>", message);
        }
    }
}