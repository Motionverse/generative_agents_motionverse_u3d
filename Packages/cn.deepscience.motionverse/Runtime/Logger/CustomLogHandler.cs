using System;
using UnityEngine;
using Object = UnityEngine.Object;
namespace MotionverseSDK {
    public class CustomLogHandler : ILogHandler
    {
        private const string CONTEXT = "Motionverse";
        private readonly ILogHandler logHandler = Debug.unityLogger.logHandler;

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            logHandler.LogFormat(logType, context, $"[{CONTEXT}] {format}", args);
        }

        public void LogException(Exception exception, Object context)
        {
            logHandler.LogException(exception, context);
        }
    }
}