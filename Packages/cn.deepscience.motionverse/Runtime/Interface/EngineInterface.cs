using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;


namespace MotionverseSDK
{
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct Vector3_t
    {
        public float x;
        public float y;
        public float z;
    };

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct Quaternion_t
    {
        public float x;
        public float y;
        public float z;
        public float w;
    };

    /// <summary>
    /// limb
    /// 0 root
    /// 1 spines
    /// 2 headneck
    /// 
    /// 6 leftleg
    /// 7 rightleg
    /// 8 leftarm
    /// 9 rightarm
    /// 
    /// 12 leftthumb
    /// 13 leftindex
    /// 14 leftmiddle
    /// 15 leftring
    /// 16 leftpinky
    /// 17 rightthumb
    /// 18 rightindex
    /// 19 rightmiddle
    /// 20 rightring
    /// 21 rightpinky
    /// </summary>


    [StructLayout(LayoutKind.Sequential)]
    public struct Transform_t
    {
        public Quaternion_t rotation;
        public Vector3_t position;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string name;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct Frame_t
    {
        public bool has_finger;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 63)]
        public Transform_t[] joint;
    };

    public enum CoordinateType
    {
        ARKit,  //RightStandard
        MediaPipeIOS, //Xdown
        MediaPipeAndroid,//ZDownRight右手坐标系
        MediaPipeX86,//mediapipe x86版的坐标系
        Unity, //leftStandard
        UnReal,//ZupLeft
        Maya, // rightStandard       
        FrankMocap,//YDownRight坐标系
    };

    public enum RetargetBoneType
    {
        Root = 0,
		Hip,
		LeftThigh,
		LeftCalf,
		LeftFoot,
		LeftToe,
		RightThigh,
		RightCalf,
		RightFoot,
		RightToe,
		Spine,
		Spine1,
		Chest,
		Neck,
		Head,
		LeftShoulder,
		LeftUpperarm,
		LeftForearm,
		LeftRotaBone,
		LeftHand,
		RightShoulder,
		RightUpperarm,
		RightForearm,
		RightRotaBone,
		RightHand,
		LeftThumb1,
		LeftThumb2,
		LeftThumb3,
		LeftThumb4,
		LeftIndex1,
		LeftIndex2,
		LeftIndex3,
		LeftIndex4,
		LeftMiddle1,
		LeftMiddle2,
		LeftMiddle3,
		LeftMiddle4,
		LeftRing1,
		LeftRing2,
		LeftRing3,
		LeftRing4,
		LeftPinky1,
		LeftPinky2,
		LeftPinky3,
		LeftPinky4,
		RightThumb1,
		RightThumb2,
		RightThumb3,
		RightThumb4,
		RightIndex1,
		RightIndex2,
		RightIndex3,
		RightIndex4,
		RightMiddle1,
		RightMiddle2,
		RightMiddle3,
		RightMiddle4,
		RightRing1,
		RightRing2,
		RightRing3,
		RightRing4,
		RightPinky1,
		RightPinky2,
		RightPinky3,
		RightPinky4,
	};

    public class EngineInterface
    {
       
        private static extern bool GenerationMotionFile(IntPtr src_tpose_path, IntPtr motion_path, IntPtr tar_path);


        #region Footskate Clean
        public static Transform_t Create(Vector3 pos, Quaternion rot, string s = "")
        {
            Transform_t t = new Transform_t();
            t.position = Create(pos);
            t.rotation = Create(rot);
            t.name = s;
            return t;
        }

        public static Frame_t Create()
        {
            Frame_t frame = new Frame_t();
            frame.joint = new Transform_t[63];
            frame.has_finger = false;
            return frame;
        }

        public static Frame_t Translate(StandardMotion motion)
        {
            Frame_t frame = Create();
            if (!motion.hasBody) return frame;

            frame.has_finger = motion.hasFingers;
            for (int i = 0; i < frame.joint.Length; ++i)
            {
                frame.joint[i] = Create(Vector3.zero, Quaternion.identity, "");
            }
            if (motion.hasBody)
            {
                frame.joint[0] = Create(motion.positions[StandardMotion.RootIndex], motion.rotations[StandardMotion.RootIndex], "");
                frame.joint[1] = Create(motion.positions[StandardMotion.HipIndex], motion.rotations[StandardMotion.HipIndex], "");
                frame.joint[2] = Create(motion.positions[StandardMotion.LeftThighIndex], motion.rotations[StandardMotion.LeftThighIndex], "");
                frame.joint[3] = Create(motion.positions[StandardMotion.LeftCalfIndex], motion.rotations[StandardMotion.LeftCalfIndex], "");
                frame.joint[4] = Create(motion.positions[StandardMotion.LeftFootIndex], motion.rotations[StandardMotion.LeftFootIndex], "");
                frame.joint[5] = Create(motion.positions[StandardMotion.LeftToeIndex], motion.rotations[StandardMotion.LeftToeIndex], "");
                frame.joint[6] = Create(motion.positions[StandardMotion.RightThighIndex], motion.rotations[StandardMotion.RightThighIndex], "");
                frame.joint[7] = Create(motion.positions[StandardMotion.RightCalfIndex], motion.rotations[StandardMotion.RightCalfIndex], "");
                frame.joint[8] = Create(motion.positions[StandardMotion.RightFootIndex], motion.rotations[StandardMotion.RightFootIndex], "");
                frame.joint[9] = Create(motion.positions[StandardMotion.RightToeIndex], motion.rotations[StandardMotion.RightToeIndex], "");
                frame.joint[10] = Create(motion.positions[StandardMotion.SpineIndex], motion.rotations[StandardMotion.SpineIndex], "");
                frame.joint[11] = Create(motion.positions[StandardMotion.Spine1Index], motion.rotations[StandardMotion.Spine1Index], "");
                frame.joint[12] = Create(motion.positions[StandardMotion.ChestIndex], motion.rotations[StandardMotion.ChestIndex], "");
                frame.joint[13] = Create(motion.positions[StandardMotion.NeckIndex], motion.rotations[StandardMotion.NeckIndex], "");
                frame.joint[14] = Create(motion.positions[StandardMotion.HeadIndex], motion.rotations[StandardMotion.HeadIndex], "");
                frame.joint[15] = Create(motion.positions[StandardMotion.LeftShoulderIndex], motion.rotations[StandardMotion.LeftShoulderIndex], "");
                frame.joint[16] = Create(motion.positions[StandardMotion.LeftUpperarmIndex], motion.rotations[StandardMotion.LeftUpperarmIndex], "");
                frame.joint[17] = Create(motion.positions[StandardMotion.LeftForearmIndex], motion.rotations[StandardMotion.LeftForearmIndex], "");
                frame.joint[18] = Create(motion.positions[StandardMotion.LeftHandIndex], motion.rotations[StandardMotion.LeftHandIndex], "");
                frame.joint[19] = Create(motion.positions[StandardMotion.RightShoulderIndex], motion.rotations[StandardMotion.RightShoulderIndex], "");
                frame.joint[20] = Create(motion.positions[StandardMotion.RightUpperarmIndex], motion.rotations[StandardMotion.RightUpperarmIndex], "");
                frame.joint[21] = Create(motion.positions[StandardMotion.RightForearmIndex], motion.rotations[StandardMotion.RightForearmIndex], "");
                frame.joint[22] = Create(motion.positions[StandardMotion.RightHandIndex], motion.rotations[StandardMotion.RightHandIndex], "");
            }
            if (motion.hasFingers)
            {
                int index = 23;
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Thumb1Index], motion.leftFingersRotations[StandardMotion.Thumb1Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Thumb2Index], motion.leftFingersRotations[StandardMotion.Thumb2Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Thumb3Index], motion.leftFingersRotations[StandardMotion.Thumb3Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Thumb4Index], motion.leftFingersRotations[StandardMotion.Thumb4Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Index1Index], motion.leftFingersRotations[StandardMotion.Index1Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Index2Index], motion.leftFingersRotations[StandardMotion.Index2Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Index3Index], motion.leftFingersRotations[StandardMotion.Index3Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Index4Index], motion.leftFingersRotations[StandardMotion.Index4Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Middle1Index], motion.leftFingersRotations[StandardMotion.Middle1Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Middle2Index], motion.leftFingersRotations[StandardMotion.Middle2Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Middle3Index], motion.leftFingersRotations[StandardMotion.Middle3Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Middle4Index], motion.leftFingersRotations[StandardMotion.Middle4Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Ring1Index], motion.leftFingersRotations[StandardMotion.Ring1Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Ring2Index], motion.leftFingersRotations[StandardMotion.Ring2Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Ring3Index], motion.leftFingersRotations[StandardMotion.Ring3Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Ring4Index], motion.leftFingersRotations[StandardMotion.Ring4Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Pinky1Index], motion.leftFingersRotations[StandardMotion.Pinky1Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Pinky1Index], motion.leftFingersRotations[StandardMotion.Pinky1Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Pinky1Index], motion.leftFingersRotations[StandardMotion.Pinky1Index], "");
                frame.joint[index++] = Create(motion.leftFingersPositions[StandardMotion.Pinky1Index], motion.leftFingersRotations[StandardMotion.Pinky1Index], "");

                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Thumb1Index], motion.rightFingersRotations[StandardMotion.Thumb1Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Thumb2Index], motion.rightFingersRotations[StandardMotion.Thumb2Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Thumb3Index], motion.rightFingersRotations[StandardMotion.Thumb3Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Thumb4Index], motion.rightFingersRotations[StandardMotion.Thumb4Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Index1Index], motion.rightFingersRotations[StandardMotion.Index1Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Index2Index], motion.rightFingersRotations[StandardMotion.Index2Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Index3Index], motion.rightFingersRotations[StandardMotion.Index3Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Index4Index], motion.rightFingersRotations[StandardMotion.Index4Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Middle1Index], motion.rightFingersRotations[StandardMotion.Middle1Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Middle2Index], motion.rightFingersRotations[StandardMotion.Middle2Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Middle3Index], motion.rightFingersRotations[StandardMotion.Middle3Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Middle4Index], motion.rightFingersRotations[StandardMotion.Middle4Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Ring1Index], motion.rightFingersRotations[StandardMotion.Ring1Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Ring2Index], motion.rightFingersRotations[StandardMotion.Ring2Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Ring3Index], motion.rightFingersRotations[StandardMotion.Ring3Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Ring4Index], motion.rightFingersRotations[StandardMotion.Ring4Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Pinky1Index], motion.rightFingersRotations[StandardMotion.Pinky1Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Pinky1Index], motion.rightFingersRotations[StandardMotion.Pinky1Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Pinky1Index], motion.rightFingersRotations[StandardMotion.Pinky1Index], "");
                frame.joint[index++] = Create(motion.rightFingersPositions[StandardMotion.Pinky1Index], motion.rightFingersRotations[StandardMotion.Pinky1Index], "");
            }
            return frame;
        }

        public static StandardMotion Translate(Frame_t frame)
        {
            StandardMotion motion = new StandardMotion();
            motion.hasBody = true;
            motion.hasFingers = frame.has_finger;
            motion.hasRotabone = false;

            motion.positions[StandardMotion.RootIndex] = Create(frame.joint[0].position);
            motion.positions[StandardMotion.HipIndex] = Create(frame.joint[1].position);
            motion.positions[StandardMotion.LeftThighIndex] = Create(frame.joint[2].position);
            motion.positions[StandardMotion.LeftCalfIndex] = Create(frame.joint[3].position);
            motion.positions[StandardMotion.LeftFootIndex] = Create(frame.joint[4].position);
            motion.positions[StandardMotion.LeftToeIndex] = Create(frame.joint[5].position);
            motion.positions[StandardMotion.RightThighIndex] = Create(frame.joint[6].position);
            motion.positions[StandardMotion.RightCalfIndex] = Create(frame.joint[7].position);
            motion.positions[StandardMotion.RightFootIndex] = Create(frame.joint[8].position);
            motion.positions[StandardMotion.RightToeIndex] = Create(frame.joint[9].position);
            motion.positions[StandardMotion.SpineIndex] = Create(frame.joint[10].position);
            motion.positions[StandardMotion.Spine1Index] = Create(frame.joint[11].position);
            motion.positions[StandardMotion.ChestIndex] = Create(frame.joint[12].position);
            motion.positions[StandardMotion.NeckIndex] = Create(frame.joint[13].position);
            motion.positions[StandardMotion.HeadIndex] = Create(frame.joint[14].position);
            motion.positions[StandardMotion.LeftShoulderIndex] = Create(frame.joint[15].position);
            motion.positions[StandardMotion.LeftUpperarmIndex] = Create(frame.joint[16].position);
            motion.positions[StandardMotion.LeftForearmIndex] = Create(frame.joint[17].position);
            motion.positions[StandardMotion.LeftHandIndex] = Create(frame.joint[18].position);
            motion.positions[StandardMotion.RightShoulderIndex] = Create(frame.joint[19].position);
            motion.positions[StandardMotion.RightUpperarmIndex] = Create(frame.joint[20].position);
            motion.positions[StandardMotion.RightForearmIndex] = Create(frame.joint[21].position);
            motion.positions[StandardMotion.RightHandIndex] = Create(frame.joint[22].position);

            motion.rotations[StandardMotion.RootIndex] = Create(frame.joint[0].rotation);
            motion.rotations[StandardMotion.HipIndex] = Create(frame.joint[1].rotation);
            motion.rotations[StandardMotion.LeftThighIndex] = Create(frame.joint[2].rotation);
            motion.rotations[StandardMotion.LeftCalfIndex] = Create(frame.joint[3].rotation);
            motion.rotations[StandardMotion.LeftFootIndex] = Create(frame.joint[4].rotation);
            motion.rotations[StandardMotion.LeftToeIndex] = Create(frame.joint[5].rotation);
            motion.rotations[StandardMotion.RightThighIndex] = Create(frame.joint[6].rotation);
            motion.rotations[StandardMotion.RightCalfIndex] = Create(frame.joint[7].rotation);
            motion.rotations[StandardMotion.RightFootIndex] = Create(frame.joint[8].rotation);
            motion.rotations[StandardMotion.RightToeIndex] = Create(frame.joint[9].rotation);
            motion.rotations[StandardMotion.SpineIndex] = Create(frame.joint[10].rotation);
            motion.rotations[StandardMotion.Spine1Index] = Create(frame.joint[11].rotation);
            motion.rotations[StandardMotion.ChestIndex] = Create(frame.joint[12].rotation);
            motion.rotations[StandardMotion.NeckIndex] = Create(frame.joint[13].rotation);
            motion.rotations[StandardMotion.HeadIndex] = Create(frame.joint[14].rotation);
            motion.rotations[StandardMotion.LeftShoulderIndex] = Create(frame.joint[15].rotation);
            motion.rotations[StandardMotion.LeftUpperarmIndex] = Create(frame.joint[16].rotation);
            motion.rotations[StandardMotion.LeftForearmIndex] = Create(frame.joint[17].rotation);
            motion.rotations[StandardMotion.LeftHandIndex] = Create(frame.joint[18].rotation);
            motion.rotations[StandardMotion.RightShoulderIndex] = Create(frame.joint[19].rotation);
            motion.rotations[StandardMotion.RightUpperarmIndex] = Create(frame.joint[20].rotation);
            motion.rotations[StandardMotion.RightForearmIndex] = Create(frame.joint[21].rotation);
            motion.rotations[StandardMotion.RightHandIndex] = Create(frame.joint[22].rotation);

            if (motion.hasFingers)
            {
                motion.leftFingersPositions[StandardMotion.Thumb1Index] = Create(frame.joint[23].position);
                motion.leftFingersPositions[StandardMotion.Thumb2Index] = Create(frame.joint[24].position);
                motion.leftFingersPositions[StandardMotion.Thumb3Index] = Create(frame.joint[25].position);
                motion.leftFingersPositions[StandardMotion.Thumb4Index] = Create(frame.joint[26].position);
                motion.leftFingersPositions[StandardMotion.Index1Index] = Create(frame.joint[27].position);
                motion.leftFingersPositions[StandardMotion.Index2Index] = Create(frame.joint[28].position);
                motion.leftFingersPositions[StandardMotion.Index3Index] = Create(frame.joint[29].position);
                motion.leftFingersPositions[StandardMotion.Index4Index] = Create(frame.joint[30].position);
                motion.leftFingersPositions[StandardMotion.Middle1Index] = Create(frame.joint[31].position);
                motion.leftFingersPositions[StandardMotion.Middle2Index] = Create(frame.joint[32].position);
                motion.leftFingersPositions[StandardMotion.Middle3Index] = Create(frame.joint[33].position);
                motion.leftFingersPositions[StandardMotion.Middle4Index] = Create(frame.joint[34].position);
                motion.leftFingersPositions[StandardMotion.Ring1Index] = Create(frame.joint[35].position);
                motion.leftFingersPositions[StandardMotion.Ring2Index] = Create(frame.joint[36].position);
                motion.leftFingersPositions[StandardMotion.Ring3Index] = Create(frame.joint[37].position);
                motion.leftFingersPositions[StandardMotion.Ring4Index] = Create(frame.joint[38].position);
                motion.leftFingersPositions[StandardMotion.Pinky1Index] = Create(frame.joint[39].position);
                motion.leftFingersPositions[StandardMotion.Pinky2Index] = Create(frame.joint[40].position);
                motion.leftFingersPositions[StandardMotion.Pinky3Index] = Create(frame.joint[41].position);
                motion.leftFingersPositions[StandardMotion.Pinky4Index] = Create(frame.joint[42].position);

                motion.leftFingersRotations[StandardMotion.Thumb1Index] = Create(frame.joint[23].rotation);
                motion.leftFingersRotations[StandardMotion.Thumb2Index] = Create(frame.joint[24].rotation);
                motion.leftFingersRotations[StandardMotion.Thumb3Index] = Create(frame.joint[25].rotation);
                motion.leftFingersRotations[StandardMotion.Thumb4Index] = Create(frame.joint[26].rotation);
                motion.leftFingersRotations[StandardMotion.Index1Index] = Create(frame.joint[27].rotation);
                motion.leftFingersRotations[StandardMotion.Index2Index] = Create(frame.joint[28].rotation);
                motion.leftFingersRotations[StandardMotion.Index3Index] = Create(frame.joint[29].rotation);
                motion.leftFingersRotations[StandardMotion.Index4Index] = Create(frame.joint[30].rotation);
                motion.leftFingersRotations[StandardMotion.Middle1Index] = Create(frame.joint[31].rotation);
                motion.leftFingersRotations[StandardMotion.Middle2Index] = Create(frame.joint[32].rotation);
                motion.leftFingersRotations[StandardMotion.Middle3Index] = Create(frame.joint[33].rotation);
                motion.leftFingersRotations[StandardMotion.Middle4Index] = Create(frame.joint[34].rotation);
                motion.leftFingersRotations[StandardMotion.Ring1Index] = Create(frame.joint[35].rotation);
                motion.leftFingersRotations[StandardMotion.Ring2Index] = Create(frame.joint[36].rotation);
                motion.leftFingersRotations[StandardMotion.Ring3Index] = Create(frame.joint[37].rotation);
                motion.leftFingersRotations[StandardMotion.Ring4Index] = Create(frame.joint[38].rotation);
                motion.leftFingersRotations[StandardMotion.Pinky1Index] = Create(frame.joint[39].rotation);
                motion.leftFingersRotations[StandardMotion.Pinky2Index] = Create(frame.joint[40].rotation);
                motion.leftFingersRotations[StandardMotion.Pinky3Index] = Create(frame.joint[41].rotation);
                motion.leftFingersRotations[StandardMotion.Pinky4Index] = Create(frame.joint[42].rotation);

                motion.rightFingersPositions[StandardMotion.Thumb1Index] = Create(frame.joint[43].position);
                motion.rightFingersPositions[StandardMotion.Thumb2Index] = Create(frame.joint[44].position);
                motion.rightFingersPositions[StandardMotion.Thumb3Index] = Create(frame.joint[45].position);
                motion.rightFingersPositions[StandardMotion.Thumb4Index] = Create(frame.joint[46].position);
                motion.rightFingersPositions[StandardMotion.Index1Index] = Create(frame.joint[47].position);
                motion.rightFingersPositions[StandardMotion.Index2Index] = Create(frame.joint[48].position);
                motion.rightFingersPositions[StandardMotion.Index3Index] = Create(frame.joint[49].position);
                motion.rightFingersPositions[StandardMotion.Index4Index] = Create(frame.joint[50].position);
                motion.rightFingersPositions[StandardMotion.Middle1Index] = Create(frame.joint[51].position);
                motion.rightFingersPositions[StandardMotion.Middle2Index] = Create(frame.joint[52].position);
                motion.rightFingersPositions[StandardMotion.Middle3Index] = Create(frame.joint[53].position);
                motion.rightFingersPositions[StandardMotion.Middle4Index] = Create(frame.joint[54].position);
                motion.rightFingersPositions[StandardMotion.Ring1Index] = Create(frame.joint[55].position);
                motion.rightFingersPositions[StandardMotion.Ring2Index] = Create(frame.joint[56].position);
                motion.rightFingersPositions[StandardMotion.Ring3Index] = Create(frame.joint[57].position);
                motion.rightFingersPositions[StandardMotion.Ring4Index] = Create(frame.joint[58].position);
                motion.rightFingersPositions[StandardMotion.Pinky1Index] = Create(frame.joint[59].position);
                motion.rightFingersPositions[StandardMotion.Pinky2Index] = Create(frame.joint[60].position);
                motion.rightFingersPositions[StandardMotion.Pinky3Index] = Create(frame.joint[61].position);
                motion.rightFingersPositions[StandardMotion.Pinky4Index] = Create(frame.joint[62].position);

                motion.rightFingersRotations[StandardMotion.Thumb1Index] = Create(frame.joint[43].rotation);
                motion.rightFingersRotations[StandardMotion.Thumb2Index] = Create(frame.joint[44].rotation);
                motion.rightFingersRotations[StandardMotion.Thumb3Index] = Create(frame.joint[45].rotation);
                motion.rightFingersRotations[StandardMotion.Thumb4Index] = Create(frame.joint[46].rotation);
                motion.rightFingersRotations[StandardMotion.Index1Index] = Create(frame.joint[47].rotation);
                motion.rightFingersRotations[StandardMotion.Index2Index] = Create(frame.joint[48].rotation);
                motion.rightFingersRotations[StandardMotion.Index3Index] = Create(frame.joint[49].rotation);
                motion.rightFingersRotations[StandardMotion.Index4Index] = Create(frame.joint[50].rotation);
                motion.rightFingersRotations[StandardMotion.Middle1Index] = Create(frame.joint[51].rotation);
                motion.rightFingersRotations[StandardMotion.Middle2Index] = Create(frame.joint[52].rotation);
                motion.rightFingersRotations[StandardMotion.Middle3Index] = Create(frame.joint[53].rotation);
                motion.rightFingersRotations[StandardMotion.Middle4Index] = Create(frame.joint[54].rotation);
                motion.rightFingersRotations[StandardMotion.Ring1Index] = Create(frame.joint[55].rotation);
                motion.rightFingersRotations[StandardMotion.Ring2Index] = Create(frame.joint[56].rotation);
                motion.rightFingersRotations[StandardMotion.Ring3Index] = Create(frame.joint[57].rotation);
                motion.rightFingersRotations[StandardMotion.Ring4Index] = Create(frame.joint[58].rotation);
                motion.rightFingersRotations[StandardMotion.Pinky1Index] = Create(frame.joint[59].rotation);
                motion.rightFingersRotations[StandardMotion.Pinky2Index] = Create(frame.joint[60].rotation);
                motion.rightFingersRotations[StandardMotion.Pinky3Index] = Create(frame.joint[61].rotation);
                motion.rightFingersRotations[StandardMotion.Pinky4Index] = Create(frame.joint[62].rotation);
            }

            return motion;
        }

        public static List<Frame_t> StandardMotion2Frames(List<StandardMotion> animation)
        {
            List<Frame_t> frames = new List<Frame_t>();
            foreach(StandardMotion motion in animation)
            {
                frames.Add(Translate(motion));
            }
            return frames;
        }

        public static List<StandardMotion> Frames2StandardMotion(List<Frame_t> animation)
        {
            List<StandardMotion> frames = new List<StandardMotion>();
            foreach (Frame_t motion in animation)
            {
                frames.Add(Translate(motion));
            }
            return frames;
        }



        #endregion

        #region main interface

        public static Joint_t Create(Vector3 pos, Quaternion rot)
        {
            Joint_t joint = new Joint_t();
            joint.px = pos.x;
            joint.py = pos.y;
            joint.pz = pos.z;
            joint.qx = rot.x;
            joint.qy = rot.y;
            joint.qz = rot.z;
            joint.qw = rot.w;

            joint.limb = -1;
            joint.index = 0;
            return joint;
        }

        public static string aString(Joint_t j)
        {
            string s = $"{j.px},{j.py},{j.pz},{j.qx},{j.qy},{j.qz},{j.qw}";
            return s;
        }


        public static Vector3_t Create(Vector3 pos)
        {
            Vector3_t v = new Vector3_t();
            v.x = pos.x;
            v.y = pos.y;
            v.z = pos.z;
            return v;
        }

        public static Vector3 Create(Vector3_t pos)
        {
            Vector3 p = Vector3.zero;
            p.x = pos.x;
            p.y = pos.y;
            p.z = pos.z;
            return p;
        }

        public static Quaternion_t Create(Quaternion rot)
        {
            Quaternion_t r = new Quaternion_t();
            r.x = rot.x;
            r.y = rot.y;
            r.z = rot.z;
            r.w = rot.w;
            return r;
        }

        public static Quaternion Create(Quaternion_t rot)
        {
            Quaternion r = Quaternion.identity;
            r.x = rot.x;
            r.y = rot.y;
            r.z = rot.z;
            r.w = rot.w;
            return r;
        }


        





       

        #region translate coordinate
       
       

        // SetSeries 与 TranslateCoordinate 需要搭配使用, SetSeries函数设置坐标系变换关系，TranslateCoordinate按照此关系默认变换坐标数据
       
       

        #endregion


        //general retarget 通用重定向
        #region General Retarget
       
        //更新重定向帧数据
       
        //关闭重定向
       
        #endregion

        #region DisplacementRebuild

        private static Dictionary<int, int> keySizePair = new Dictionary<int, int>();

        public static void ChangeKey(int id, int motionSize)
        {
            if (motionSize <= 0)
                return;

            if (!keySizePair.ContainsKey(id))
            {
                keySizePair.Add(id, motionSize);
            }
            else
            {
                keySizePair[id] = motionSize;
            }
        }

      


        //位移生成
        //源骨骼T pose数据路径，动作数据路径（local记录），生成后动作数据路径（local记录）
        public static bool GenerateMotionFile(string srcTPosePath, string srcMotionPath, string tarMotionPath)
        {
            IntPtr src_pose = StringToIntPtr(srcTPosePath);
            IntPtr src_motion = StringToIntPtr(srcMotionPath);
            IntPtr tar_motion = StringToIntPtr(tarMotionPath);
            bool tag = GenerationMotionFile(src_pose, src_motion, tar_motion);
            Marshal.FreeHGlobal(src_pose);
            Marshal.FreeHGlobal(src_motion);
            Marshal.FreeHGlobal(tar_motion);
            return tag;
        }

        #endregion

        public static IntPtr StringToIntPtr(string str)
        {
            IntPtr ptr = IntPtr.Zero;
            if (!string.IsNullOrEmpty(str))
            {
                ptr = Marshal.StringToHGlobalAnsi(str);
            }
            return ptr;
        }



       
        #endregion

        #region Local<->Global Motion
        public static Vector3_t Translate(Vector3 vec)
        {
            Vector3_t v = new Vector3_t();
            v.x = vec.x;
            v.y = vec.y;
            v.z = vec.z;
            return v;
        }

        public static Vector3 Translate(Vector3_t vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }

        public static Quaternion_t Translate(Quaternion rot)
        {
            Quaternion_t q = new Quaternion_t();
            q.x = rot.x;
            q.y = rot.y;
            q.z = rot.z;
            q.w = rot.w;
            return q;
        }

        public static Quaternion Translate(Quaternion_t rot)
        {
            return new Quaternion(rot.x, rot.y, rot.z, rot.w);
        }

        public static TransformInfo Translate(Transform_t trans)
        {
            TransformInfo info = new TransformInfo(Translate(trans.position), Translate(trans.rotation));
            return info;
        }



        #endregion

        #region util method
        public enum LimbType 
        {
            Root,
            Spines,
            Necks,
            LeftLeg,
            RightLeg,
            LeftArm,
            RightArm,
            LeftThumb,
            LeftIndex,
            LeftMiddle,
            LeftRing,
            LeftPinky,
            RightThumb,
            RightIndex,
            RightMiddle,
            RightRing,
            RightPinky,
        };

        private static Dictionary<LimbType, int> Limb_Dictionary = new Dictionary<LimbType, int>
        {
            {LimbType.Root, 0},
            {LimbType.Spines, 1},
            {LimbType.Necks, 2},
            {LimbType.LeftLeg, 6},
            {LimbType.RightLeg, 7},
            {LimbType.LeftArm, 8},
            {LimbType.RightArm, 9},
            {LimbType.LeftThumb, 12},
            {LimbType.LeftIndex, 13},
            {LimbType.LeftMiddle, 14},
            {LimbType.LeftRing, 15},
            {LimbType.LeftPinky, 16},
            {LimbType.RightThumb, 17},
            {LimbType.RightIndex, 18},
            {LimbType.RightMiddle, 19},
            {LimbType.RightRing, 20},
            {LimbType.RightPinky, 21},
        };

        private static Dictionary<CoordinateType, int> Coordinate_Dictionary = new Dictionary<CoordinateType, int>
        {
            {CoordinateType.ARKit, 0},
            {CoordinateType.MediaPipeIOS, 1},
            {CoordinateType.MediaPipeAndroid, 2},
            {CoordinateType.MediaPipeX86, 3},
            {CoordinateType.Unity, 4},
            {CoordinateType.UnReal, 5},
            {CoordinateType.Maya, 6},
            {CoordinateType.FrankMocap, 7}
        };

        private static Dictionary<int, int> dic_serialID = new Dictionary<int, int>();
        
        private static int Max_Length = 1000;


        private static Joint_t CreateJoint_t(Vector3 pos, Quaternion rot, int limb, int index)
        {
            Joint_t j = new Joint_t();
            j.px = pos.x;
            j.py = pos.y;
            j.pz = pos.z;
            j.qx = rot.x;
            j.qy = rot.y;
            j.qz = rot.z;
            j.qw = rot.w;
            j.limb = limb;
            j.index = index;
            return j;
        }

        private static Joint_t CreateJoint_t(Joint_t joint, int limb, int index)
        {
            Joint_t j = new Joint_t();
            j.px = joint.px;
            j.py = joint.py;
            j.pz = joint.pz;
            j.qx = joint.qx;
            j.qy = joint.qy;
            j.qz = joint.qz;
            j.qw = joint.qw;
            j.limb = limb;
            j.index = index;
            return j;
        }


        public static void StandardMotion2Joint_tArray(StandardMotion skeleton, out Joint_t[] arrays)
        {
            arrays = new Joint_t[23];
            if (skeleton.hasBody && skeleton.hasFingers)
            {
                arrays = new Joint_t[63];
            }
            
            arrays[0] = CreateJoint_t(skeleton.positions[StandardMotion.RootIndex], skeleton.rotations[StandardMotion.RootIndex], 0, 0);
            arrays[1] = CreateJoint_t(skeleton.positions[StandardMotion.HipIndex], skeleton.rotations[StandardMotion.HipIndex], 1, 0);
            arrays[2] = CreateJoint_t(skeleton.positions[StandardMotion.SpineIndex], skeleton.rotations[StandardMotion.SpineIndex], 1, 1);
            arrays[3] = CreateJoint_t(skeleton.positions[StandardMotion.Spine1Index], skeleton.rotations[StandardMotion.Spine1Index], 1, 2);
            arrays[4] = CreateJoint_t(skeleton.positions[StandardMotion.ChestIndex], skeleton.rotations[StandardMotion.ChestIndex], 1, 3);
            //
            arrays[5] = CreateJoint_t(skeleton.positions[StandardMotion.LeftThighIndex], skeleton.rotations[StandardMotion.LeftThighIndex], 6, 0);
            arrays[6] = CreateJoint_t(skeleton.positions[StandardMotion.LeftCalfIndex], skeleton.rotations[StandardMotion.LeftCalfIndex], 6, 1);
            arrays[7] = CreateJoint_t(skeleton.positions[StandardMotion.LeftFootIndex], skeleton.rotations[StandardMotion.LeftFootIndex], 6, 2);
            arrays[8] = CreateJoint_t(skeleton.positions[StandardMotion.LeftToeIndex], skeleton.rotations[StandardMotion.LeftToeIndex], 6, 3);
            arrays[9] = CreateJoint_t(skeleton.positions[StandardMotion.RightThighIndex], skeleton.rotations[StandardMotion.RightThighIndex], 7, 0);
            arrays[10] = CreateJoint_t(skeleton.positions[StandardMotion.RightCalfIndex], skeleton.rotations[StandardMotion.RightCalfIndex], 7, 1);
            arrays[11] = CreateJoint_t(skeleton.positions[StandardMotion.RightFootIndex], skeleton.rotations[StandardMotion.RightFootIndex], 7, 2);
            arrays[12] = CreateJoint_t(skeleton.positions[StandardMotion.RightToeIndex], skeleton.rotations[StandardMotion.RightToeIndex], 7, 3);
            //
            arrays[13] = CreateJoint_t(skeleton.positions[StandardMotion.NeckIndex], skeleton.rotations[StandardMotion.NeckIndex], 2, 0);
            arrays[14] = CreateJoint_t(skeleton.positions[StandardMotion.HeadIndex], skeleton.rotations[StandardMotion.HeadIndex], 2, 1);
            //
            arrays[15] = CreateJoint_t(skeleton.positions[StandardMotion.LeftShoulderIndex], skeleton.rotations[StandardMotion.LeftShoulderIndex], 8, 0);
            arrays[16] = CreateJoint_t(skeleton.positions[StandardMotion.LeftUpperarmIndex], skeleton.rotations[StandardMotion.LeftUpperarmIndex], 8, 1);
            arrays[17] = CreateJoint_t(skeleton.positions[StandardMotion.LeftForearmIndex], skeleton.rotations[StandardMotion.LeftForearmIndex], 8, 2);
            arrays[18] = CreateJoint_t(skeleton.positions[StandardMotion.LeftHandIndex], skeleton.rotations[StandardMotion.LeftHandIndex], 8, 4);
            arrays[19] = CreateJoint_t(skeleton.positions[StandardMotion.RightShoulderIndex], skeleton.rotations[StandardMotion.RightShoulderIndex], 9, 0);
            arrays[20] = CreateJoint_t(skeleton.positions[StandardMotion.RightUpperarmIndex], skeleton.rotations[StandardMotion.RightUpperarmIndex], 9, 1);
            arrays[21] = CreateJoint_t(skeleton.positions[StandardMotion.RightForearmIndex], skeleton.rotations[StandardMotion.RightForearmIndex], 9, 2);
            arrays[22] = CreateJoint_t(skeleton.positions[StandardMotion.RightHandIndex], skeleton.rotations[StandardMotion.RightHandIndex], 9, 4);

            if (skeleton.hasFingers)
            {
                arrays[23] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Thumb1Index], skeleton.leftFingersRotations[StandardMotion.Thumb1Index], 12, 0);
                arrays[24] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Thumb2Index], skeleton.leftFingersRotations[StandardMotion.Thumb2Index], 12, 1);
                arrays[25] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Thumb3Index], skeleton.leftFingersRotations[StandardMotion.Thumb3Index], 12, 2);
                arrays[26] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Thumb4Index], skeleton.leftFingersRotations[StandardMotion.Thumb4Index], 12, 3);

                arrays[27] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Index1Index], skeleton.leftFingersRotations[StandardMotion.Index1Index], 13, 0);
                arrays[28] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Index2Index], skeleton.leftFingersRotations[StandardMotion.Index2Index], 13, 1);
                arrays[29] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Index3Index], skeleton.leftFingersRotations[StandardMotion.Index3Index], 13, 2);
                arrays[30] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Index4Index], skeleton.leftFingersRotations[StandardMotion.Index4Index], 13, 3);

                arrays[31] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Middle1Index], skeleton.leftFingersRotations[StandardMotion.Middle1Index], 14, 0);
                arrays[32] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Middle2Index], skeleton.leftFingersRotations[StandardMotion.Middle2Index], 14, 1);
                arrays[33] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Middle3Index], skeleton.leftFingersRotations[StandardMotion.Middle3Index], 14, 2);
                arrays[34] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Middle4Index], skeleton.leftFingersRotations[StandardMotion.Middle4Index], 14, 3);

                arrays[35] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Ring1Index], skeleton.leftFingersRotations[StandardMotion.Ring1Index], 15, 0);
                arrays[36] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Ring2Index], skeleton.leftFingersRotations[StandardMotion.Ring2Index], 15, 1);
                arrays[37] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Ring3Index], skeleton.leftFingersRotations[StandardMotion.Ring3Index], 15, 2);
                arrays[38] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Ring4Index], skeleton.leftFingersRotations[StandardMotion.Ring4Index], 15, 3);

                arrays[39] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Pinky1Index], skeleton.leftFingersRotations[StandardMotion.Pinky1Index], 16, 0);
                arrays[40] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Pinky2Index], skeleton.leftFingersRotations[StandardMotion.Pinky2Index], 16, 1);
                arrays[41] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Pinky3Index], skeleton.leftFingersRotations[StandardMotion.Pinky3Index], 16, 2);
                arrays[42] = CreateJoint_t(skeleton.leftFingersPositions[StandardMotion.Pinky4Index], skeleton.leftFingersRotations[StandardMotion.Pinky4Index], 16, 3);

                //////////////////////////////////////////////////////////////////////////////////////////
                arrays[43] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Thumb1Index], skeleton.rightFingersRotations[StandardMotion.Thumb1Index], 17, 0);
                arrays[44] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Thumb2Index], skeleton.rightFingersRotations[StandardMotion.Thumb2Index], 17, 1);
                arrays[45] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Thumb3Index], skeleton.rightFingersRotations[StandardMotion.Thumb3Index], 17, 2);
                arrays[46] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Thumb4Index], skeleton.rightFingersRotations[StandardMotion.Thumb4Index], 17, 3);

                arrays[47] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Index1Index], skeleton.rightFingersRotations[StandardMotion.Index1Index], 18, 0);
                arrays[48] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Index2Index], skeleton.rightFingersRotations[StandardMotion.Index2Index], 18, 1);
                arrays[49] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Index3Index], skeleton.rightFingersRotations[StandardMotion.Index3Index], 18, 2);
                arrays[50] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Index4Index], skeleton.rightFingersRotations[StandardMotion.Index4Index], 18, 3);

                arrays[51] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Middle1Index], skeleton.rightFingersRotations[StandardMotion.Middle1Index], 19, 0);
                arrays[52] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Middle2Index], skeleton.rightFingersRotations[StandardMotion.Middle2Index], 19, 1);
                arrays[53] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Middle3Index], skeleton.rightFingersRotations[StandardMotion.Middle3Index], 19, 2);
                arrays[54] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Middle4Index], skeleton.rightFingersRotations[StandardMotion.Middle4Index], 19, 3);

                arrays[55] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Ring1Index], skeleton.rightFingersRotations[StandardMotion.Ring1Index], 20, 0);
                arrays[56] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Ring2Index], skeleton.rightFingersRotations[StandardMotion.Ring2Index], 20, 1);
                arrays[57] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Ring3Index], skeleton.rightFingersRotations[StandardMotion.Ring3Index], 20, 2);
                arrays[58] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Ring4Index], skeleton.rightFingersRotations[StandardMotion.Ring4Index], 20, 3);

                arrays[59] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Pinky1Index], skeleton.rightFingersRotations[StandardMotion.Pinky1Index], 21, 0);
                arrays[60] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Pinky2Index], skeleton.rightFingersRotations[StandardMotion.Pinky2Index], 21, 1);
                arrays[61] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Pinky3Index], skeleton.rightFingersRotations[StandardMotion.Pinky3Index], 21, 2);
                arrays[62] = CreateJoint_t(skeleton.rightFingersPositions[StandardMotion.Pinky4Index], skeleton.rightFingersRotations[StandardMotion.Pinky4Index], 21, 3);
            }
        }

        private static void SkeletonDictionary2Joint_tArray(Dictionary<RetargetBoneType, Joint_t> skeleton, out Joint_t[] arrays)
        {
            bool has_rotabone = false;
            bool has_finger = false;
            if (skeleton.ContainsKey(RetargetBoneType.LeftRotaBone) && skeleton.ContainsKey(RetargetBoneType.RightRotaBone))
            {
                has_rotabone = true;
            }
            else
            {
                has_rotabone = false;
            }

            if (skeleton.ContainsKey(RetargetBoneType.LeftThumb1) && skeleton.ContainsKey(RetargetBoneType.RightThumb1))
            {
                has_finger = true;
            }

            arrays = new Joint_t[23];
            if (has_rotabone && has_finger)
            {
                arrays = new Joint_t[65];
            }
            else if (has_rotabone && !has_finger)
            {
                arrays = new Joint_t[25];
            }
            else if (!has_rotabone && has_finger)
            {
                arrays = new Joint_t[63];
            }
            
            arrays[0] = CreateJoint_t(skeleton[RetargetBoneType.Root], 0, 0);
            arrays[1] = CreateJoint_t(skeleton[RetargetBoneType.Hip], 1, 0);
            arrays[2] = CreateJoint_t(skeleton[RetargetBoneType.Spine], 1, 1);
            arrays[3] = CreateJoint_t(skeleton[RetargetBoneType.Spine1], 1, 2);
            arrays[4] = CreateJoint_t(skeleton[RetargetBoneType.Chest], 1, 3);
            //
            arrays[5] = CreateJoint_t(skeleton[RetargetBoneType.LeftThigh], 6, 0);
            arrays[6] = CreateJoint_t(skeleton[RetargetBoneType.LeftCalf], 6, 1);
            arrays[7] = CreateJoint_t(skeleton[RetargetBoneType.LeftFoot], 6, 2);
            arrays[8] = CreateJoint_t(skeleton[RetargetBoneType.LeftToe], 6, 3);
            arrays[9] = CreateJoint_t(skeleton[RetargetBoneType.RightThigh], 7, 0);
            arrays[10] = CreateJoint_t(skeleton[RetargetBoneType.RightCalf], 7, 1);
            arrays[11] = CreateJoint_t(skeleton[RetargetBoneType.RightFoot], 7, 2);
            arrays[12] = CreateJoint_t(skeleton[RetargetBoneType.RightToe], 7, 3);
            //
            arrays[13] = CreateJoint_t(skeleton[RetargetBoneType.Neck], 2, 0);
            arrays[14] = CreateJoint_t(skeleton[RetargetBoneType.Head], 2, 1);
            //
            arrays[15] = CreateJoint_t(skeleton[RetargetBoneType.LeftShoulder], 8, 0);
            arrays[16] = CreateJoint_t(skeleton[RetargetBoneType.LeftUpperarm], 8, 1);
            arrays[17] = CreateJoint_t(skeleton[RetargetBoneType.LeftForearm], 8, 2);
            arrays[18] = CreateJoint_t(skeleton[RetargetBoneType.LeftHand], 8, 4);
            arrays[19] = CreateJoint_t(skeleton[RetargetBoneType.RightShoulder], 9, 0);
            arrays[20] = CreateJoint_t(skeleton[RetargetBoneType.RightUpperarm], 9, 1);
            arrays[21] = CreateJoint_t(skeleton[RetargetBoneType.RightForearm], 9, 2);
            arrays[22] = CreateJoint_t(skeleton[RetargetBoneType.RightHand], 9, 4);
            if (has_rotabone)
            {
                arrays[23] = CreateJoint_t(skeleton[RetargetBoneType.LeftRotaBone], 8, 3);
                arrays[24] = CreateJoint_t(skeleton[RetargetBoneType.RightRotaBone], 9, 3);
            }

            if (has_finger)
            {
                int index = 23;
                if (has_rotabone)
                    index = 25;
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftThumb1], 12, 0);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftThumb2], 12, 1);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftThumb3], 12, 2);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftThumb4], 12, 3);

                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftIndex1], 13, 0);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftIndex2], 13, 1);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftIndex3], 13, 2);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftIndex4], 13, 3);

                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftMiddle1], 14, 0);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftMiddle2], 14, 1);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftMiddle3], 14, 2);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftMiddle4], 14, 3);

                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftRing1], 15, 0);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftRing2], 15, 1);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftRing3], 15, 2);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftRing4], 15, 3);

                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftPinky1], 16, 0);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftPinky2], 16, 1);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftPinky3], 16, 2);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.LeftPinky4], 16, 3);

                //////////////////////////////////////////////////////////////////////////////////////////
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightThumb1], 17, 0);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightThumb2], 17, 1);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightThumb3], 17, 2);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightThumb4], 17, 3);

                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightIndex1], 18, 0);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightIndex2], 18, 1);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightIndex3], 18, 2);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightIndex4], 18, 3);

                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightMiddle1], 19, 0);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightMiddle2], 19, 1);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightMiddle3], 19, 2);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightMiddle4], 19, 3);

                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightRing1], 20, 0);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightRing2], 20, 1);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightRing3], 20, 2);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightRing4], 20, 3);

                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightPinky1], 21, 0);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightPinky2], 21, 1);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightPinky3], 21, 2);
                arrays[index++] = CreateJoint_t(skeleton[RetargetBoneType.RightPinky4], 21, 3);
            }
        }

        public static void Joint_tArray2StandardMotion(Joint_t[] arrays, out StandardMotion skeleton)
        {
            skeleton = new StandardMotion();
            for (int i = 0; i < arrays.Length; ++i)
            {
                if (arrays[i].limb == 0)
                {
                    skeleton.hasBody = true;
                    skeleton.positions[StandardMotion.RootIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                    skeleton.rotations[StandardMotion.RootIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);                    
                }
                else if (arrays[i].limb == 1)
                {
                    skeleton.hasBody = true;

                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton.positions[StandardMotion.HipIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.HipIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 1:
                            skeleton.positions[StandardMotion.SpineIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.SpineIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 2:
                            skeleton.positions[StandardMotion.Spine1Index] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.Spine1Index] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 3:
                            skeleton.positions[StandardMotion.ChestIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.ChestIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == 2)
                {
                    skeleton.hasBody = true;

                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton.positions[StandardMotion.NeckIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.NeckIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 1:
                            skeleton.positions[StandardMotion.HeadIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.HeadIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;                        
                        default:
                            break;
                    }
                }
                //
                else if (arrays[i].limb == 6)
                {
                    skeleton.hasBody = true;

                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton.positions[StandardMotion.LeftThighIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.LeftThighIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 1:
                            skeleton.positions[StandardMotion.LeftCalfIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.LeftCalfIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 2:
                            skeleton.positions[StandardMotion.LeftFootIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.LeftFootIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 3:
                            skeleton.positions[StandardMotion.LeftToeIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.LeftToeIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == 7)
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton.positions[StandardMotion.RightThighIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.RightThighIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 1:
                            skeleton.positions[StandardMotion.RightCalfIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.RightCalfIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 2:
                            skeleton.positions[StandardMotion.RightFootIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.RightFootIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 3:
                            skeleton.positions[StandardMotion.RightToeIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.RightToeIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == 8)
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton.positions[StandardMotion.LeftShoulderIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.LeftShoulderIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 1:
                            skeleton.positions[StandardMotion.LeftUpperarmIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.LeftUpperarmIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 2:
                            skeleton.positions[StandardMotion.LeftForearmIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.LeftForearmIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 3:
                            skeleton.hasRotabone = true;
                            skeleton.positions[StandardMotion.LeftRotaboneIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.LeftRotaboneIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 4:
                            skeleton.hasRotabone = true;
                            skeleton.positions[StandardMotion.LeftHandIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.LeftHandIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == 9)
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton.positions[StandardMotion.RightShoulderIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.RightShoulderIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 1:
                            skeleton.positions[StandardMotion.RightUpperarmIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.RightUpperarmIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 2:
                            skeleton.positions[StandardMotion.RightForearmIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.RightForearmIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 3:
                            skeleton.hasRotabone = true;
                            skeleton.positions[StandardMotion.RightRotaboneIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.RightRotaboneIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        case 4:
                            skeleton.hasRotabone = true;
                            skeleton.positions[StandardMotion.RightHandIndex] = new Vector3(arrays[i].px, arrays[i].py, arrays[i].pz);
                            skeleton.rotations[StandardMotion.RightHandIndex] = new Quaternion(arrays[i].qx, arrays[i].qy, arrays[i].qz, arrays[i].qw);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static void Joint_tArray2SkeletonDictionary(Joint_t[] arrays, out Dictionary<RetargetBoneType, Joint_t> skeleton)
        {
            skeleton = new Dictionary<RetargetBoneType, Joint_t>();
            for (int i = 0; i < arrays.Length; ++i)
            {
                if (arrays[i].limb == 0)
                {
                    skeleton[RetargetBoneType.Root] = arrays[i];
                }
                else if (arrays[i].limb == 1)
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.Hip] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.Spine] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.Spine1] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.Chest] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == 2)
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.Neck] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.Head] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                //
                else if (arrays[i].limb == 6)
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.LeftThigh] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.LeftCalf] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.LeftFoot] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.LeftToe] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == 7)
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.RightThigh] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.RightCalf] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.RightFoot] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.RightToe] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == 8)
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.LeftShoulder] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.LeftUpperarm] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.LeftForearm] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.LeftRotaBone] = arrays[i];
                            break;
                        case 4:
                            skeleton[RetargetBoneType.LeftHand] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == 9)
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.RightShoulder] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.RightUpperarm] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.RightForearm] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.RightRotaBone] = arrays[i];
                            break;
                        case 4:
                            skeleton[RetargetBoneType.RightHand] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                //
                else if (arrays[i].limb == Limb_Dictionary[LimbType.LeftThumb])
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.LeftThumb1] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.LeftThumb2] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.LeftThumb3] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.LeftThumb4] = arrays[i];
                            break;                        
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == Limb_Dictionary[LimbType.LeftIndex])
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.LeftIndex1] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.LeftIndex2] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.LeftIndex3] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.LeftIndex4] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == Limb_Dictionary[LimbType.LeftMiddle])
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.LeftMiddle1] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.LeftMiddle2] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.LeftMiddle3] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.LeftMiddle4] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == Limb_Dictionary[LimbType.LeftRing])
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.LeftRing1] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.LeftRing2] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.LeftRing3] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.LeftRing4] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == Limb_Dictionary[LimbType.LeftPinky])
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.LeftPinky1] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.LeftPinky2] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.LeftPinky3] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.LeftPinky4] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                //
                else if (arrays[i].limb == Limb_Dictionary[LimbType.RightThumb])
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.RightThumb1] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.RightThumb2] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.RightThumb3] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.RightThumb4] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == Limb_Dictionary[LimbType.RightIndex])
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.RightIndex1] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.RightIndex2] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.RightIndex3] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.RightIndex4] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == Limb_Dictionary[LimbType.RightMiddle])
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.RightMiddle1] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.RightMiddle2] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.RightMiddle3] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.RightMiddle4] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == Limb_Dictionary[LimbType.RightRing])
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.RightRing1] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.RightRing2] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.RightRing3] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.RightRing4] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
                else if (arrays[i].limb == Limb_Dictionary[LimbType.RightPinky])
                {
                    switch (arrays[i].index)
                    {
                        case 0:
                            skeleton[RetargetBoneType.RightPinky1] = arrays[i];
                            break;
                        case 1:
                            skeleton[RetargetBoneType.RightPinky2] = arrays[i];
                            break;
                        case 2:
                            skeleton[RetargetBoneType.RightPinky3] = arrays[i];
                            break;
                        case 3:
                            skeleton[RetargetBoneType.RightPinky4] = arrays[i];
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static byte[] StructBytes(object objStruct)
        {
            int length = Marshal.SizeOf(objStruct);
            IntPtr ptr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(objStruct, ptr, false);
            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        public static T Bytes2Struct<T>(byte[] bytes)
        {
            int nStructSize = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocHGlobal(nStructSize);
            Marshal.Copy(bytes, 0, ptr, nStructSize);

            object obj = Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return (T)obj;
        }

        public static byte[] StructArrayBytes<T>(T[] structArray)
        {
            if (null == structArray || structArray.Length == 0)
            {
                return null;
            }

            int nStructSize = Marshal.SizeOf(typeof(T));
            byte[] bytes = new byte[nStructSize * structArray.Length];
            for (int i = 0; i < structArray.Length; ++i)
            {
                StructBytes(structArray[i]).CopyTo(bytes, i * nStructSize);
            }

            return bytes;
        }

        public static bool InitIntPtrFromStructArray<T>(ref IntPtr ptr, T[] structArray)
        {
            byte[] bytes = StructArrayBytes(structArray);
            if (null == bytes)
            {
                return false;
            }
            ptr = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            return true;
        }

        public static bool InitIntStructArrayFromInPtr<T>(IntPtr ptr, int length, out List<T> structArray)
        {
            structArray = new List<T>();
            int nStructSize = Marshal.SizeOf(typeof(T));

            if (length <= 0)
            {
                return false;
            }

            byte[] byteArrays = new byte[nStructSize * length];
            Marshal.Copy(ptr, byteArrays, 0, byteArrays.Length);
            //Marshal.Copy(byteArrays, 0, ptr, byteArrays.Length);

            for (int i = 0; i < length; ++i)
            {
                byte[] bytes = new byte[nStructSize];
                Buffer.BlockCopy(byteArrays, i * nStructSize, bytes, 0, nStructSize);
                T nStruct = Bytes2Struct<T>(bytes);
                structArray.Add(nStruct);
            }

            return true;
        }

        public static void InitIntPtrFromStruct<T>(ref IntPtr ptr, T objStruct)
        {
            byte[] bytes = StructBytes(objStruct);
            ptr = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
        }

        public static void InitStructFromIntPtr<T>(ref T objStruct, IntPtr ptr)
        {
            if (ptr == null)
            {
                return;
            }

            int length = Marshal.SizeOf(typeof(T));
            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);

            IntPtr structPtr = Marshal.AllocHGlobal(length);
            Marshal.Copy(bytes, 0, structPtr, length);
            object obj = Marshal.PtrToStructure(structPtr, typeof(T));
            Marshal.FreeHGlobal(structPtr);

            objStruct = (T)obj;
        }

#endregion

    }
}
