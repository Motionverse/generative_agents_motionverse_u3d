namespace MotionverseSDK
{
    public class Config
    {

        public static string TextMotionUrl = "/v3.0/api/textBroadcastMotion";
        public static string AudioMotionUrl =   "/v3.1/api/voiceBroadcastMotion";
        public static string NLPMotionUrl = "/v3.0/api/AnswerCollectMotion";
        public static string GetTokenUrl =  "/users/getAppToken";


        public static string[] Finger_Names = new string[40] {
        "LeftHandThumb1", "LeftHandThumb2", "LeftHandThumb3", "LeftHandThumb4",//25
        "LeftHandIndex1", "LeftHandIndex2", "LeftHandIndex3", "LeftHandIndex4",//29
        "LeftHandMiddle1", "LeftHandMiddle2", "LeftHandMiddle3", "LeftHandMiddle4",//33
        "LeftHandRing1", "LeftHandRing2", "LeftHandRing3", "LeftHandRing4",//37
        "LeftHandPinky1", "LeftHandPinky2", "LeftHandPinky3", "LeftHandPinky4",//41

        "RightHandThumb1", "RightHandThumb2", "RightHandThumb3", "RightHandThumb4",//45
        "RightHandIndex1", "RightHandIndex2", "RightHandIndex3", "RightHandIndex4",
        "RightHandMiddle1", "RightHandMiddle2", "RightHandMiddle3", "RightHandMiddle4",
        "RightHandRing1", "RightHandRing2", "RightHandRing3", "RightHandRing4",
        "RightHandPinky1", "RightHandPinky2", "RightHandPinky3", "RightHandPinky4",//61
        };
    }

   
}
