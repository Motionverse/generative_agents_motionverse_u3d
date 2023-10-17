
using System.Collections.Generic;
using UnityEngine;
namespace MotionverseSDK
{
    public class BoneMap : MonoBehaviour
    {
        //public string Root = "root";
        //public string Hip = "pelvis";
        //public string LeftThigh = "thigh_l";
        //public string LeftCalf = "calf_l";
        //public string LeftFoot = "foot_l";
        //public string LeftToe = "ball_l";
        //public string RightThigh = "thigh_r";
        //public string RightCalf = "calf_r";
        //public string RightFoot = "foot_r";
        //public string RightToe = "ball_r";
        //public string Spine = "spine_01";
        //public string Spine1 = "spine_02";
        //public string Chest = "spine_03";
        //public string Neck = "neck_01";
        //public string Head = "head";

        //public string LeftShoulder = "clavicle_l";
        //public string LeftUpperarm = "upperarm_l";
        //public string LeftForearm = "lowerarm_l";
        //public string LeftHand = "hand_l";
        //public string RightShoulder = "clavicle_r";
        //public string RightUpperarm = "upperarm_r";
        //public string RightForearm = "lowerarm_r";
        //public string RightHand = "hand_r";
        //public string LeftThumb1 = "thumb_01_l";
        //public string LeftThumb2 = "thumb_02_l";
        //public string LeftThumb3 = "thumb_03_l";

        //public string LeftIndex1 = "index_01_l";
        //public string LeftIndex2 = "index_02_l";
        //public string LeftIndex3 = "index_03_l";

        //public string LeftMiddle1 = "middle_01_l";
        //public string LeftMiddle2 = "middle_02_l";
        //public string LeftMiddle3 = "middle_03_l";

        //public string LeftRing1 = "ring_01_l";
        //public string LeftRing2 = "ring_02_l";
        //public string LeftRing3 = "ring_03_l";

        //public string LeftPinky1 = "pinky_01_l";
        //public string LeftPinky2 = "pinky_02_l";
        //public string LeftPinky3 = "pinky_03_l";


        //public string RightThumb1 = "thumb_01_r";
        //public string RightThumb2 = "thumb_02_r";
        //public string RightThumb3 = "thumb_03_r";

        //public string RightIndex1 = "index_01_r";
        //public string RightIndex2 = "index_02_r";
        //public string RightIndex3 = "index_03_r";

        //public string RightMiddle1 = "middle_01_r";
        //public string RightMiddle2 = "middle_02_r";
        //public string RightMiddle3 = "middle_03_r";

        //public string RightRing1 = "ring_01_r";
        //public string RightRing2 = "ring_02_r";
        //public string RightRing3 = "ring_03_r";

        //public string RightPinky1 = "pinky_01_r";
        //public string RightPinky2 = "pinky_02_r";
        //public string RightPinky3 = "pinky_03_r";

        //public string Hip = "CC_Base_Hip";
        //public string LeftThigh = "CC_Base_L_Thigh";
        //public string LeftCalf = "CC_Base_L_Calf";
        //public string LeftFoot = "CC_Base_L_Foot";
        //public string LeftToe = "CC_Base_L_ToeBase";
        //public string RightThigh = "CC_Base_R_Thigh";
        //public string RightCalf = "CC_Base_R_Calf";
        //public string RightFoot = "CC_Base_R_Foot";
        //public string RightToe = "CC_Base_R_ToeBase";
        //public string Spine = "CC_Base_Waist";
        //public string Spine1 = "CC_Base_Spine01";
        //public string Chest = "CC_Base_Spine02";
        //public string Neck = "CC_Base_NeckTwist02";
        //public string Head = "CC_Base_Head";

        //public string LeftShoulder = "CC_Base_L_Clavicle";
        //public string LeftUpperarm = "CC_Base_L_Upperarm";
        //public string LeftForearm = "CC_Base_L_Forearm";
        //public string LeftHand = "CC_Base_L_Hand";

        //public string RightShoulder = "CC_Base_R_Clavicle";
        //public string RightUpperarm = "CC_Base_R_Upperarm";
        //public string RightForearm = "CC_Base_R_Forearm";
        //public string RightHand = "CC_Base_R_Hand";

        //public string LeftThumb1 = "CC_Base_L_Thumb1";
        //public string LeftThumb2 = "CC_Base_L_Thumb2";
        //public string LeftThumb3 = "CC_Base_L_Thumb3";

        //public string LeftIndex1 = "CC_Base_L_Index1";
        //public string LeftIndex2 = "CC_Base_L_Index2";
        //public string LeftIndex3 = "CC_Base_L_Index3";

        //public string LeftMiddle1 = "CC_Base_L_Mid1";
        //public string LeftMiddle2 = "CC_Base_L_Mid2";
        //public string LeftMiddle3 = "CC_Base_L_Mid3";

        //public string LeftRing1 = "CC_Base_L_Ring1";
        //public string LeftRing2 = "CC_Base_L_Ring2";
        //public string LeftRing3 = "CC_Base_L_Ring3";

        //public string LeftPinky1 = "CC_Base_L_Pinky1";
        //public string LeftPinky2 = "CC_Base_L_Pinky2";
        //public string LeftPinky3 = "CC_Base_L_Pinky3";


        //public string RightThumb1 = "CC_Base_R_Thumb1";
        //public string RightThumb2 = "CC_Base_R_Thumb2";
        //public string RightThumb3 = "CC_Base_R_Thumb3";

        //public string RightIndex1 = "CC_Base_R_Index1";
        //public string RightIndex2 = "CC_Base_R_Index2";
        //public string RightIndex3 = "CC_Base_R_Index3";

        //public string RightMiddle1 = "CC_Base_R_Mid1";
        //public string RightMiddle2 = "CC_Base_R_Mid2";
        //public string RightMiddle3 = "CC_Base_R_Mid3";

        //public string RightRing1 = "CC_Base_R_Ring1";
        //public string RightRing2 = "CC_Base_R_Ring2";
        //public string RightRing3 = "CC_Base_R_Ring3";

        //public string RightPinky1 = "CC_Base_R_Pinky1";
        //public string RightPinky2 = "CC_Base_R_Pinky2";
        //public string RightPinky3 = "CC_Base_R_Pinky3";

        public string Hip = "Hips";
        public string LeftThigh = "LeftUpLeg";
        public string LeftCalf = "LeftLeg";
        public string LeftFoot = "LeftFoot";
        public string LeftToe = "LeftToeBase";
        public string RightThigh = "RightUpLeg";
        public string RightCalf = "RightLeg";
        public string RightFoot = "RightFoot";
        public string RightToe = "RightToeBase";
        public string Spine = "Spine";
        public string Spine1 = "Spine1";
        public string Chest = "Spine2";
        public string Neck = "Neck";
        public string Head = "Head";

        public string LeftShoulder = "LeftShoulder";
        public string LeftUpperarm = "LeftArm";
        public string LeftForearm = "LeftForeArm";
        public string LeftHand = "LeftHand";
        public string RightShoulder = "RightShoulder";
        public string RightUpperarm = "RightArm";
        public string RightForearm = "RightForeArm";
        public string RightHand = "RightHand";
        public string LeftThumb1 = "LeftHandThumb1";
        public string LeftThumb2 = "LeftHandThumb2";
        public string LeftThumb3 = "LeftHandThumb3";

        public string LeftIndex1 = "LeftHandIndex1";
        public string LeftIndex2 = "LeftHandIndex2";
        public string LeftIndex3 = "LeftHandIndex3";

        public string LeftMiddle1 = "LeftHandMiddle1";
        public string LeftMiddle2 = "LeftHandMiddle2";
        public string LeftMiddle3 = "LeftHandMiddle3";

        public string LeftRing1 = "LeftHandRing1";
        public string LeftRing2 = "LeftHandRing2";
        public string LeftRing3 = "LeftHandRing3";

        public string LeftPinky1 = "LeftHandPinky1";
        public string LeftPinky2 = "LeftHandPinky2";
        public string LeftPinky3 = "LeftHandPinky3";


        public string RightThumb1 = "RightHandThumb1";
        public string RightThumb2 = "RightHandThumb2";
        public string RightThumb3 = "RightHandThumb3";

        public string RightIndex1 = "RightHandIndex1";
        public string RightIndex2 = "RightHandIndex2";
        public string RightIndex3 = "RightHandIndex3";

        public string RightMiddle1 = "RightHandMiddle1";
        public string RightMiddle2 = "RightHandMiddle2";
        public string RightMiddle3 = "RightHandMiddle3";

        public string RightRing1 = "RightHandRing1";
        public string RightRing2 = "RightHandRing2";
        public string RightRing3 = "RightHandRing3";

        public string RightPinky1 = "RightHandPinky1";
        public string RightPinky2 = "RightHandPinky2";
        public string RightPinky3 = "RightHandPinky3";


        public Dictionary<RetargetBoneType, string> boneNames;
        protected void Awake()
        {
          boneNames = new Dictionary<RetargetBoneType, string>() {
        { RetargetBoneType.Root, "Root"},
        { RetargetBoneType.Hip, Hip},
        { RetargetBoneType.LeftThigh,LeftThigh},
        { RetargetBoneType.LeftCalf,LeftCalf},
        { RetargetBoneType.LeftFoot,LeftFoot},
        { RetargetBoneType.LeftToe,LeftToe},
        { RetargetBoneType.RightThigh,RightThigh},
        { RetargetBoneType.RightCalf,RightCalf},
        { RetargetBoneType.RightFoot,RightFoot},
        { RetargetBoneType.RightToe,RightToe},
        { RetargetBoneType.Spine,Spine},
        { RetargetBoneType.Spine1,Spine1},
        { RetargetBoneType.Chest,Chest},
        { RetargetBoneType.Neck,Neck},
        { RetargetBoneType.Head,Head},
        { RetargetBoneType.LeftShoulder,LeftShoulder},
        { RetargetBoneType.LeftUpperarm,LeftUpperarm},
        { RetargetBoneType.LeftForearm,LeftForearm},
        { RetargetBoneType.LeftHand,LeftHand},
        { RetargetBoneType.RightShoulder,RightShoulder},
        { RetargetBoneType.RightUpperarm,RightUpperarm},
        { RetargetBoneType.RightForearm,RightForearm},
        { RetargetBoneType.RightHand,RightHand},

        { RetargetBoneType.LeftThumb1,LeftThumb1},
        { RetargetBoneType.LeftThumb2,LeftThumb2},
        { RetargetBoneType.LeftThumb3,LeftThumb3},
        { RetargetBoneType.LeftThumb4,LeftThumb3},
        { RetargetBoneType.LeftIndex1,LeftIndex1},
        { RetargetBoneType.LeftIndex2,LeftIndex2},
        { RetargetBoneType.LeftIndex3,LeftIndex3},
         { RetargetBoneType.LeftIndex4,LeftIndex3},

        { RetargetBoneType.LeftMiddle1,LeftMiddle1},
        { RetargetBoneType.LeftMiddle2,LeftMiddle2},
        { RetargetBoneType.LeftMiddle3,LeftMiddle3},
          { RetargetBoneType.LeftMiddle4,LeftMiddle3},
        { RetargetBoneType.LeftRing1,LeftRing1},
        { RetargetBoneType.LeftRing2,LeftRing2},
        { RetargetBoneType.LeftRing3,LeftRing3},
         { RetargetBoneType.LeftRing4,LeftRing3},
        { RetargetBoneType.LeftPinky1,LeftPinky1},
        { RetargetBoneType.LeftPinky2,LeftPinky2},
        { RetargetBoneType.LeftPinky3,LeftPinky3},
        { RetargetBoneType.LeftPinky4,LeftPinky3},

        { RetargetBoneType.RightThumb1,RightThumb1},
        { RetargetBoneType.RightThumb2,RightThumb2},
        { RetargetBoneType.RightThumb3,RightThumb3},
        { RetargetBoneType.RightThumb4,RightThumb3},
        { RetargetBoneType.RightIndex1,RightIndex1},
        { RetargetBoneType.RightIndex2,RightIndex2},
        { RetargetBoneType.RightIndex3,RightIndex3},
        { RetargetBoneType.RightIndex4,RightIndex3},
        { RetargetBoneType.RightMiddle1,RightMiddle1},
        { RetargetBoneType.RightMiddle2,RightMiddle2},
        { RetargetBoneType.RightMiddle3,RightMiddle3},
        { RetargetBoneType.RightMiddle4,RightMiddle3},
        { RetargetBoneType.RightRing1,RightRing1},
        { RetargetBoneType.RightRing2,RightRing2},
        { RetargetBoneType.RightRing3,RightRing3},
        { RetargetBoneType.RightRing4,RightRing3},
        { RetargetBoneType.RightPinky1,RightPinky1},
        { RetargetBoneType.RightPinky2,RightPinky2},
        { RetargetBoneType.RightPinky3,RightPinky3},
        { RetargetBoneType.RightPinky4,RightPinky3},
    };

        }
    }


}
