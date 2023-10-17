using UnityEngine;
namespace MotionverseSDK
{
    public class StandardMotion
    {
        public Vector3[] positions = new Vector3[25];
        public Quaternion[] rotations = new Quaternion[25];
        public Vector3[] leftFingersPositions = new Vector3[20];
        public Quaternion[] leftFingersRotations = new Quaternion[20];
        public Vector3[] rightFingersPositions = new Vector3[20];
        public Quaternion[] rightFingersRotations = new Quaternion[20];
        public bool hasBody = false;
        public bool hasRotabone = false;
        public bool hasFingers = false;

        public static int RootIndex = 0;
        public static int HipIndex = 1;
        public static int LeftThighIndex = 2;
        public static int LeftCalfIndex = 3;
        public static int LeftFootIndex = 4;
        public static int LeftToeIndex = 5;
        public static int RightThighIndex = 6;
        public static int RightCalfIndex = 7;
        public static int RightFootIndex = 8;
        public static int RightToeIndex = 9;
        public static int SpineIndex = 10;
        public static int Spine1Index = 11;
        public static int ChestIndex = 12;
        public static int NeckIndex = 13;
        public static int HeadIndex = 14;
        public static int LeftShoulderIndex = 15;
        public static int LeftUpperarmIndex = 16;
        public static int LeftForearmIndex = 17;
        public static int LeftRotaboneIndex = 18;
        public static int LeftHandIndex = 19;
        public static int RightShoulderIndex = 20;
        public static int RightUpperarmIndex = 21;
        public static int RightForearmIndex = 22;
        public static int RightRotaboneIndex = 23;
        public static int RightHandIndex = 24;

        public static int Thumb1Index = 0;
        public static int Thumb2Index = 1;
        public static int Thumb3Index = 2;
        public static int Thumb4Index = 3;
        public static int Index1Index = 4;
        public static int Index2Index = 5;
        public static int Index3Index = 6;
        public static int Index4Index = 7;
        public static int Middle1Index = 8;
        public static int Middle2Index = 9;
        public static int Middle3Index = 10;
        public static int Middle4Index = 11;
        public static int Ring1Index = 12;
        public static int Ring2Index = 13;
        public static int Ring3Index = 14;
        public static int Ring4Index = 15;
        public static int Pinky1Index = 16;
        public static int Pinky2Index = 17;
        public static int Pinky3Index = 18;
        public static int Pinky4Index = 19;

        public static StandardMotion GetMotionFromStandardSkeleton(StandardSkeleton skeleton)
        {
            StandardMotion motion = new StandardMotion();

            motion.hasBody = true;
            motion.hasRotabone = skeleton.has_rotabone;

            motion.positions[RootIndex] = skeleton.m_root.position;
            motion.rotations[RootIndex] = skeleton.m_root.rotation;
            motion.positions[HipIndex] = skeleton.m_transformHip.position;
            motion.rotations[HipIndex] = skeleton.m_transformHip.rotation;
            motion.positions[LeftThighIndex] = skeleton.m_transformLeftThigh.position;
            motion.rotations[LeftThighIndex] = skeleton.m_transformLeftThigh.rotation;
            motion.positions[LeftCalfIndex] = skeleton.m_transformLeftCalf.position;
            motion.rotations[LeftCalfIndex] = skeleton.m_transformLeftCalf.rotation;
            motion.positions[LeftFootIndex] = skeleton.m_transformLeftFoot.position;
            motion.rotations[LeftFootIndex] = skeleton.m_transformLeftFoot.rotation;
            motion.positions[LeftToeIndex] = skeleton.m_transformLeftToe.position;
            motion.rotations[LeftToeIndex] = skeleton.m_transformLeftToe.rotation;
            motion.positions[RightThighIndex] = skeleton.m_transformRightThigh.position;
            motion.rotations[RightThighIndex] = skeleton.m_transformRightThigh.rotation;
            motion.positions[RightCalfIndex] = skeleton.m_transformRightCalf.position;
            motion.rotations[RightCalfIndex] = skeleton.m_transformRightCalf.rotation;
            motion.positions[RightFootIndex] = skeleton.m_transformRightFoot.position;
            motion.rotations[RightFootIndex] = skeleton.m_transformRightFoot.rotation;
            motion.positions[RightToeIndex] = skeleton.m_transformRightToe.position;
            motion.rotations[RightToeIndex] = skeleton.m_transformRightToe.rotation;
            motion.positions[SpineIndex] = skeleton.m_transformSpines[0].position;
            motion.rotations[SpineIndex] = skeleton.m_transformSpines[0].rotation;
            motion.positions[Spine1Index] = skeleton.m_transformSpines[1].position;
            motion.rotations[Spine1Index] = skeleton.m_transformSpines[1].rotation;
            motion.positions[ChestIndex] = skeleton.m_transformSpines[2].position;
            motion.rotations[ChestIndex] = skeleton.m_transformSpines[2].rotation;
            motion.positions[NeckIndex] = skeleton.m_transformNeck.position;
            motion.rotations[NeckIndex] = skeleton.m_transformNeck.rotation;
            motion.positions[HeadIndex] = skeleton.m_transformHead.position;
            motion.rotations[HeadIndex] = skeleton.m_transformHead.rotation;
            motion.positions[LeftShoulderIndex] = skeleton.m_transformLeftShouder.position;
            motion.rotations[LeftShoulderIndex] = skeleton.m_transformLeftShouder.rotation;
            motion.positions[LeftUpperarmIndex] = skeleton.m_transformLeftUpArm.position;
            motion.rotations[LeftUpperarmIndex] = skeleton.m_transformLeftUpArm.rotation;
            motion.positions[LeftForearmIndex] = skeleton.m_transformLeftForeArm.position;
            motion.rotations[LeftForearmIndex] = skeleton.m_transformLeftForeArm.rotation;
            motion.positions[LeftHandIndex] = skeleton.m_transformLeftHand.position;
            motion.rotations[LeftHandIndex] = skeleton.m_transformLeftHand.rotation;
            if (motion.hasRotabone)
            {
                motion.positions[LeftRotaboneIndex] = skeleton.m_transformLeftRotateBone.position;
                motion.rotations[LeftRotaboneIndex] = skeleton.m_transformLeftRotateBone.rotation;
            }
            motion.positions[RightHandIndex] = skeleton.m_transformRightHand.position;
            motion.rotations[RightHandIndex] = skeleton.m_transformRightHand.rotation;
            motion.positions[RightShoulderIndex] = skeleton.m_transformRightShouder.position;
            motion.rotations[RightShoulderIndex] = skeleton.m_transformRightShouder.rotation;
            motion.positions[RightUpperarmIndex] = skeleton.m_transformRightUpArm.position;
            motion.rotations[RightUpperarmIndex] = skeleton.m_transformRightUpArm.rotation;
            motion.positions[RightForearmIndex] = skeleton.m_transformRightForeArm.position;
            motion.rotations[RightForearmIndex] = skeleton.m_transformRightForeArm.rotation;
            if (motion.hasRotabone)
            {
                motion.positions[RightRotaboneIndex] = skeleton.m_transformRightRotateBone.position;
                motion.rotations[RightRotaboneIndex] = skeleton.m_transformRightRotateBone.rotation;
            }
            motion.positions[RightHandIndex] = skeleton.m_transformRightHand.position;
            motion.rotations[RightHandIndex] = skeleton.m_transformRightHand.rotation;

            return motion;
        }
    }
}
