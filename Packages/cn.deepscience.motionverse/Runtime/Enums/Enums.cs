using System;
using System.Collections.Generic;
using UnityEngine;
namespace MotionverseSDK
{
    public enum MeshType
    {
        HeadMesh,
        BodyMesh
    }
    public enum FailureType
    {
        None,
        NoInternetConnection,
        DownloadError,
        OperationCancelled
    }

    public enum AvatarStatus
    {
        Idle,
        Talk
    }
    public enum Region
    {
        EN_US,
        ZH_CN
       
    }
    public enum Skeleton
    {
            Hips,
            LeftUpLeg,
            LeftLeg,
            LeftFoot,
            LeftToeBase,
            RightUpLeg,
            RightLeg,
            RightFoot,
            RightToeBase,
            Spine,
            Spine1,
            Spine2,
            Neck,
            Head,
            LeftShoulder,
            LeftArm,
            LeftForeArm,
            LeftHand,
            RightShoulder,
            RightArm,
            RightForeArm,
            RightHand,
            LeftHandThumb1,
            LeftHandThumb2,
            LeftHandThumb3,
            LeftHandThumb4,
            LeftHandIndex1,
            LeftHandIndex2,
            LeftHandIndex3,
            LeftHandIndex4,
            LeftHandMiddle1,
            LeftHandMiddle2,
            LeftHandMiddle3,
            LeftHandMiddle4,
            LeftHandRing1,
            LeftHandRing2,
            LeftHandRing3,
            LeftHandRing4,
            LeftHandPinky1,
            LeftHandPinky2,
            LeftHandPinky3,
            LeftHandPinky4,
            RightHandThumb1,
            RightHandThumb2,
            RightHandThumb3,
            RightHandThumb4,
            RightHandIndex1,
            RightHandIndex2,
            RightHandIndex3,
            RightHandIndex4,
            RightHandMiddle1,
            RightHandMiddle2,
            RightHandMiddle3,
            RightHandMiddle4,
            RightHandRing1,
            RightHandRing2,
            RightHandRing3,
            RightHandRing4,
            RightHandPinky1,
            RightHandPinky2,
            RightHandPinky3,
            RightHandPinky4,
    }
   


    public enum BlendShape
    {
        eyeBlinkRight,
        eyeLookDownRight,
        eyeLookInRight,
        eyeLookOutRight,
        eyeLookUpRight,
        eyeSquintRight,
        eyeWideRight,
        eyeBlinkLeft,
        eyeLookDownLeft,
        eyeLookInLeft,
        eyeLookOutLeft,
        eyeLookUpLeft,
        eyeSquintLeft,
        eyeWideLeft,
        jawForward,
        jawRight,
        jawLeft,
        jawOpen,
        mouthClose,
        mouthFunnel,
        mouthPucker,
        mouthRight,
        mouthLeft,
        mouthSmileLeft,
        mouthSmileRight,
        mouthFrownRight,
        mouthFrownLeft,
        mouthDimpleRight,
        mouthDimpleLeft,
        mouthStretchRight,
        mouthStretchLeft,
        mouthRollLower,
        mouthRollUpper,
        mouthShrugLower,
        mouthShrugUpper,
        mouthPressRight,
        mouthPressLeft,
        mouthLowerDownRight,
        mouthLowerDownLeft,
        mouthUpperUpRight,
        mouthUpperUpLeft,
        browDownRight,
        browDownLeft,
        browInnerUp,
        browOuterUpRight,
        browOuterUpLeft,
        cheekPuff,
        cheekSquintRight,
        cheekSquintLeft,
        noseSneerRight,
        noseSneerLeft
    }
    public class WebResponse
    {
        public int code = 200;
        public string msg = null;
        public string data = null;
    }
    public class TalkData
    {
        public AudioClip clip;
        public Dictionary<string, AnimationCurve> faceAnimCurves = new Dictionary<string, AnimationCurve>();
        public AnimationData animationData = new AnimationData();
    }
    public class Drive
    {
        public int step = 0;
        public string text = null;
        public AudioClip clip;
        public string bsData;
        public Byte[] motionByte;
    }


    [Serializable]
    public class TextMotionParams
    {
        public string request_id = null;
        public string draft_content = null;
        public MotionParams body_config = new MotionParams();
        public FaceParams face_config = new FaceParams();
        public TTSParams tts_config = new TTSParams();
    }
    [Serializable]
    public class NLPMotionParams
    {
        public string request_id = null;
        public string text = "";            //请求文本(必填)
        public MotionParams body_config = new MotionParams();
        public FaceParams face_config = new FaceParams();
        public TTSParams tts_config = new TTSParams();
    }
    [Serializable]
    public class AudioUrlMotionParams
    {
        public string audio_url = null;
        public MotionParams body_config = new MotionParams();
        public FaceParams face_config = new FaceParams();
    }

    [Serializable]
    public class FaceParams
    {
        public string gender = "female";
        public int face_type = 2;
    }
    [Serializable]
    public class MotionParams
    {
        public int body_motion = 1;
        public int body_head_x_rot = 0;
        public bool body_compress = false;
    }

    [Serializable]
    public class TTSParams
    {
        public string tts_voice_name = null;
        public float tts_speed = 50f;
        public float tts_volume = 50f;
        public float tts_fm = 50f;
    }

    public class DialogueCache
    {
        public string text;
        public string audioUrl;
    }

    public class DriveTask
    {
        public Player player;
        public string text;
    }
}
