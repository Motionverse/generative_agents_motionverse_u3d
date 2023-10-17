using System;
namespace MotionverseSDK
{
    public class CustomException : Exception
    {
        public readonly FailureType FailureType;

        public CustomException(FailureType failureType, string message) : base(message)
        {
            FailureType = failureType;
        }
    }
}