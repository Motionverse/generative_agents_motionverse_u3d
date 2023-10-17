using System;
namespace MotionverseSDK
{
    public class FailureEventArgs : EventArgs
    {
        public string Message { get; set; }
        public FailureType Type { get; set; }
    }

    public class ProgressChangeEventArgs : EventArgs
    {
        public float Progress { get; set; }
    }
}

