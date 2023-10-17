using System;
using System.Collections.Generic;
using UnityEngine;

namespace MotionverseSDK
{
    public class CompatibleRetarget : MonoBehaviour
    {
        private Transform tarRoot;
        public bool useFinger = false;
        public bool useRotabone = false;
        public Dictionary<RetargetBoneType, Transform> target = new Dictionary<RetargetBoneType, Transform>();
        public Dictionary<Skeleton, Transform> srcBone = new Dictionary<Skeleton, Transform>();

        public Vector3 leftHandPositionOffset = Vector3.zero;
        public Vector3 rightHandPositionOffset = Vector3.zero;
        public Quaternion headOffset = Quaternion.identity;

        public Transform retargetOrigin;

        int serialId = 1;
        [HideInInspector]
        public StandardSkeleton src;
        [HideInInspector]
        public HandFinger srcLeft;
        [HideInInspector]
        public HandFinger srcRight;
        [HideInInspector]
        public BoneMap boneMap;

        // Start is called before the first frame update
        bool checkInited = false;
        public void Init(Transform _tarRoot)
        {
            serialId++;
            tarRoot = _tarRoot;
            boneMap = GetComponent<BoneMap>();

            src = new StandardSkeleton();
            GameObject modle = Resources.Load<GameObject>("Skeleton");
            // 在场景中生成物体，生成后场景就会出现该物体实例
            GameObject modleGo = Instantiate<GameObject>(modle);
            modleGo.transform.SetParent(transform);

            foreach (Skeleton bone in Enum.GetValues(typeof(Skeleton)))
            {
                srcBone.Add(bone, Utilities.FindChildRecursively(modleGo.transform, bone.ToString()));
            }
            src.CloneHuman(modleGo.transform);
            srcLeft = new HandFinger(src.m_transformLeftHand, true);
            srcRight = new HandFinger(src.m_transformRightHand, false);
            StandardMotion srcMotion = StandardMotion.GetMotionFromStandardSkeleton(src);
            GetFingerData(ref srcMotion, srcLeft, srcRight);

            foreach (KeyValuePair<RetargetBoneType, string> pair in boneMap.boneNames)
            {
                if (pair.Key == RetargetBoneType.Root)
                {
                    Transform bone = Utilities.FindChildRecursively(transform, boneMap.boneNames[RetargetBoneType.Hip]);
                    target.Add(RetargetBoneType.Root, bone.parent);
                }
                else
                {
                    Transform bone = Utilities.FindChildRecursively(transform, pair.Value);
                    target.Add(pair.Key, bone);
                }

            }

            Dictionary<RetargetBoneType, Joint_t> tar_pose = new Dictionary<RetargetBoneType, Joint_t>();
            foreach (KeyValuePair<RetargetBoneType, Transform> pair in target)
            {
                Joint_t info = EngineInterface.Create(pair.Value.position, pair.Value.rotation);
                tar_pose.Add(pair.Key, info);
            }


          

        }
        // Update is called once per frame
        void Update()
        {
            if (checkInited)
            {
                //CompatibleRetargetStandard.SetHandOffset(serialId, leftHandPositionOffset, rightHandPositionOffset);
                //CompatibleRetargetStandard.SetHeadRotationOffset(serialId, headOffset);
                //StandardMotion frame = GetFromSkeleton(srcSkeleton);
                //var targetFrame = CompatibleRetargetStandard.UpdateRetarget(serialId, frame);
                //SetFrame(targetFrame, tarRoot);
            }

        }

        

        public void SetFrame(Dictionary<RetargetBoneType, Joint_t> frame, Transform root)
        {
            if (frame.Count <= 0)
            {
                Debug.Log("frame null");
                return;
            }
            SetJoint(RetargetBoneType.Hip, frame, root);
            SetJoint(RetargetBoneType.LeftThigh, frame, root);
            SetJoint(RetargetBoneType.LeftCalf, frame, root);
            SetJoint(RetargetBoneType.LeftFoot, frame, root);
            SetJoint(RetargetBoneType.LeftToe, frame, root);
            SetJoint(RetargetBoneType.RightThigh, frame, root);
            SetJoint(RetargetBoneType.RightCalf, frame, root);
            SetJoint(RetargetBoneType.RightFoot, frame, root);
            SetJoint(RetargetBoneType.RightToe, frame, root);
            SetJoint(RetargetBoneType.Spine, frame, root);
            SetJoint(RetargetBoneType.Spine1, frame, root);
            SetJoint(RetargetBoneType.Chest, frame, root);
            SetJoint(RetargetBoneType.Neck, frame, root);
            SetJoint(RetargetBoneType.Head, frame, root);
            SetJoint(RetargetBoneType.LeftShoulder, frame, root);
            SetJoint(RetargetBoneType.LeftUpperarm, frame, root);
            SetJoint(RetargetBoneType.LeftForearm, frame, root);
            if (useRotabone)
            {
                SetJoint(RetargetBoneType.LeftRotaBone, frame, root);
            }
            SetJoint(RetargetBoneType.LeftHand, frame, root);
            SetJoint(RetargetBoneType.RightShoulder, frame, root);
            SetJoint(RetargetBoneType.RightUpperarm, frame, root);
            SetJoint(RetargetBoneType.RightForearm, frame, root);
            if (useRotabone)
            {
                SetJoint(RetargetBoneType.RightRotaBone, frame, root);
            }
            SetJoint(RetargetBoneType.RightHand, frame, root);

            if (useFinger)
            {
                foreach (var item in FingerType)
                {
                    Transform t = Utilities.FindChildRecursively(root, item.Value);
                    if (t != null)
                    {
                        if (item.Key.Equals(RetargetBoneType.Hip))
                        {
                            t.position = CompatibleRetargetSolver.Position(frame[item.Key]);
                        }
                        t.rotation = CompatibleRetargetSolver.Rotation(frame[item.Key]);
                    }
                    else
                    {
                        Debug.LogError(item.Value + " do not found");
                    }
                }
            }
        }

        private void SetJoint(RetargetBoneType type, Dictionary<RetargetBoneType, Joint_t> frame, Transform root)
        {
            string name = "";
            if (JointType.ContainsKey(type))
                name = JointType[type];
            else
                name = RotaboneType[type];
            Transform t = Utilities.FindChildRecursively(root, name);
            if (t != null)
            {
                if (type == (RetargetBoneType.Hip))
                {
                    t.position = CompatibleRetargetSolver.Position(frame[type]);
                }
                //t.position = CompatibleRetargetSolver.Position(frame[type]);
                t.rotation = CompatibleRetargetSolver.Rotation(frame[type]);
            }
            else
            {
                Debug.LogError(name + " do not found");
            }
        }

        public Dictionary<RetargetBoneType, Joint_t> GetFromRoot(Transform root)
        {
            Dictionary<RetargetBoneType, Joint_t> dic = new Dictionary<RetargetBoneType, Joint_t>();
            foreach (var item in JointType)
            {
                Transform t = Utilities.FindChildRecursively(root, item.Value);
                if (t != null)
                {
                    dic.Add(item.Key, EngineInterface.Create(t.position, t.rotation));

                }
                else
                {
                    Debug.LogError(item.Value + " not found");
                }
            }

            if (useRotabone)
            {
                foreach (var item in RotaboneType)
                {
                    Transform t = Utilities.FindChildRecursively(root, item.Value);
                    if (t != null)
                    {
                        dic.Add(item.Key, EngineInterface.Create(t.position, t.rotation));
                    }
                    else
                    {
                        Debug.LogError(item.Value + " not found");
                    }
                }
            }

            //
            if (useFinger)
            {
                foreach (var item in FingerType)
                {
                    Transform t = Utilities.FindChildRecursively(root, item.Value);
                    if (t != null)
                    {
                        dic.Add(item.Key, EngineInterface.Create(t.position, t.rotation));
                    }
                    else
                    {
                        Debug.LogError(item.Value + " not found");
                    }
                }
            }
            return dic;
        }


        public void GetFingerData(ref StandardMotion motion, HandFinger left, HandFinger right)
        {
            if (left == null || right == null)
            {
                motion.hasFingers = false;
                return;
            }

            motion.hasFingers = true;
            motion.leftFingersPositions[StandardMotion.Thumb1Index] = left.m_Thumb1.position;
            motion.leftFingersPositions[StandardMotion.Thumb2Index] = left.m_Thumb2.position;
            motion.leftFingersPositions[StandardMotion.Thumb3Index] = left.m_Thumb3.position;
            motion.leftFingersPositions[StandardMotion.Thumb4Index] = left.m_Thumb4.position;
            motion.leftFingersPositions[StandardMotion.Index1Index] = left.m_Index1.position;
            motion.leftFingersPositions[StandardMotion.Index2Index] = left.m_Index2.position;
            motion.leftFingersPositions[StandardMotion.Index3Index] = left.m_Index3.position;
            motion.leftFingersPositions[StandardMotion.Index4Index] = left.m_Index4.position;
            motion.leftFingersPositions[StandardMotion.Middle1Index] = left.m_Middle1.position;
            motion.leftFingersPositions[StandardMotion.Middle2Index] = left.m_Middle2.position;
            motion.leftFingersPositions[StandardMotion.Middle3Index] = left.m_Middle3.position;
            motion.leftFingersPositions[StandardMotion.Middle4Index] = left.m_Middle4.position;
            motion.leftFingersPositions[StandardMotion.Ring1Index] = left.m_Ring1.position;
            motion.leftFingersPositions[StandardMotion.Ring2Index] = left.m_Ring2.position;
            motion.leftFingersPositions[StandardMotion.Ring3Index] = left.m_Ring3.position;
            motion.leftFingersPositions[StandardMotion.Ring4Index] = left.m_Ring4.position;
            motion.leftFingersPositions[StandardMotion.Pinky1Index] = left.m_Pinky1.position;
            motion.leftFingersPositions[StandardMotion.Pinky2Index] = left.m_Pinky2.position;
            motion.leftFingersPositions[StandardMotion.Pinky3Index] = left.m_Pinky3.position;
            motion.leftFingersPositions[StandardMotion.Pinky4Index] = left.m_Pinky4.position;

            motion.leftFingersRotations[StandardMotion.Thumb1Index] = left.m_Thumb1.rotation;
            motion.leftFingersRotations[StandardMotion.Thumb2Index] = left.m_Thumb2.rotation;
            motion.leftFingersRotations[StandardMotion.Thumb3Index] = left.m_Thumb3.rotation;
            motion.leftFingersRotations[StandardMotion.Thumb4Index] = left.m_Thumb4.rotation;
            motion.leftFingersRotations[StandardMotion.Index1Index] = left.m_Index1.rotation;
            motion.leftFingersRotations[StandardMotion.Index2Index] = left.m_Index2.rotation;
            motion.leftFingersRotations[StandardMotion.Index3Index] = left.m_Index3.rotation;
            motion.leftFingersRotations[StandardMotion.Index4Index] = left.m_Index4.rotation;
            motion.leftFingersRotations[StandardMotion.Middle1Index] = left.m_Middle1.rotation;
            motion.leftFingersRotations[StandardMotion.Middle2Index] = left.m_Middle2.rotation;
            motion.leftFingersRotations[StandardMotion.Middle3Index] = left.m_Middle3.rotation;
            motion.leftFingersRotations[StandardMotion.Middle4Index] = left.m_Middle4.rotation;
            motion.leftFingersRotations[StandardMotion.Ring1Index] = left.m_Ring1.rotation;
            motion.leftFingersRotations[StandardMotion.Ring2Index] = left.m_Ring2.rotation;
            motion.leftFingersRotations[StandardMotion.Ring3Index] = left.m_Ring3.rotation;
            motion.leftFingersRotations[StandardMotion.Ring4Index] = left.m_Ring4.rotation;
            motion.leftFingersRotations[StandardMotion.Pinky1Index] = left.m_Pinky1.rotation;
            motion.leftFingersRotations[StandardMotion.Pinky2Index] = left.m_Pinky2.rotation;
            motion.leftFingersRotations[StandardMotion.Pinky3Index] = left.m_Pinky3.rotation;
            motion.leftFingersRotations[StandardMotion.Pinky4Index] = left.m_Pinky4.rotation;

            motion.rightFingersPositions[StandardMotion.Thumb1Index] = right.m_Thumb1.position;
            motion.rightFingersPositions[StandardMotion.Thumb2Index] = right.m_Thumb2.position;
            motion.rightFingersPositions[StandardMotion.Thumb3Index] = right.m_Thumb3.position;
            motion.rightFingersPositions[StandardMotion.Thumb4Index] = right.m_Thumb4.position;
            motion.rightFingersPositions[StandardMotion.Index1Index] = right.m_Index1.position;
            motion.rightFingersPositions[StandardMotion.Index2Index] = right.m_Index2.position;
            motion.rightFingersPositions[StandardMotion.Index3Index] = right.m_Index3.position;
            motion.rightFingersPositions[StandardMotion.Index4Index] = right.m_Index4.position;
            motion.rightFingersPositions[StandardMotion.Middle1Index] = right.m_Middle1.position;
            motion.rightFingersPositions[StandardMotion.Middle2Index] = right.m_Middle2.position;
            motion.rightFingersPositions[StandardMotion.Middle3Index] = right.m_Middle3.position;
            motion.rightFingersPositions[StandardMotion.Middle4Index] = right.m_Middle4.position;
            motion.rightFingersPositions[StandardMotion.Ring1Index] = right.m_Ring1.position;
            motion.rightFingersPositions[StandardMotion.Ring2Index] = right.m_Ring2.position;
            motion.rightFingersPositions[StandardMotion.Ring3Index] = right.m_Ring3.position;
            motion.rightFingersPositions[StandardMotion.Ring4Index] = right.m_Ring4.position;
            motion.rightFingersPositions[StandardMotion.Pinky1Index] = right.m_Pinky1.position;
            motion.rightFingersPositions[StandardMotion.Pinky2Index] = right.m_Pinky2.position;
            motion.rightFingersPositions[StandardMotion.Pinky3Index] = right.m_Pinky3.position;
            motion.rightFingersPositions[StandardMotion.Pinky4Index] = right.m_Pinky4.position;

            motion.rightFingersRotations[StandardMotion.Thumb1Index] = right.m_Thumb1.rotation;
            motion.rightFingersRotations[StandardMotion.Thumb2Index] = right.m_Thumb2.rotation;
            motion.rightFingersRotations[StandardMotion.Thumb3Index] = right.m_Thumb3.rotation;
            motion.rightFingersRotations[StandardMotion.Thumb4Index] = right.m_Thumb4.rotation;
            motion.rightFingersRotations[StandardMotion.Index1Index] = right.m_Index1.rotation;
            motion.rightFingersRotations[StandardMotion.Index2Index] = right.m_Index2.rotation;
            motion.rightFingersRotations[StandardMotion.Index3Index] = right.m_Index3.rotation;
            motion.rightFingersRotations[StandardMotion.Index4Index] = right.m_Index4.rotation;
            motion.rightFingersRotations[StandardMotion.Middle1Index] = right.m_Middle1.rotation;
            motion.rightFingersRotations[StandardMotion.Middle2Index] = right.m_Middle2.rotation;
            motion.rightFingersRotations[StandardMotion.Middle3Index] = right.m_Middle3.rotation;
            motion.rightFingersRotations[StandardMotion.Middle4Index] = right.m_Middle4.rotation;
            motion.rightFingersRotations[StandardMotion.Ring1Index] = right.m_Ring1.rotation;
            motion.rightFingersRotations[StandardMotion.Ring2Index] = right.m_Ring2.rotation;
            motion.rightFingersRotations[StandardMotion.Ring3Index] = right.m_Ring3.rotation;
            motion.rightFingersRotations[StandardMotion.Ring4Index] = right.m_Ring4.rotation;
            motion.rightFingersRotations[StandardMotion.Pinky1Index] = right.m_Pinky1.rotation;
            motion.rightFingersRotations[StandardMotion.Pinky2Index] = right.m_Pinky2.rotation;
            motion.rightFingersRotations[StandardMotion.Pinky3Index] = right.m_Pinky3.rotation;
            motion.rightFingersRotations[StandardMotion.Pinky4Index] = right.m_Pinky4.rotation;

        }


        static Dictionary<RetargetBoneType, string> JointType = new Dictionary<RetargetBoneType, string>()
        {
            {RetargetBoneType.Root, "Root"},
            {RetargetBoneType.Hip, "Hips"},
            {RetargetBoneType.LeftThigh, "LeftUpLeg"},
            {RetargetBoneType.LeftCalf, "LeftLeg"},
            {RetargetBoneType.LeftFoot, "LeftFoot"},
            {RetargetBoneType.LeftToe, "LeftToeBase"},
            {RetargetBoneType.RightThigh, "RightUpLeg"},
            {RetargetBoneType.RightCalf, "RightLeg"},
            {RetargetBoneType.RightFoot, "RightFoot"},
            {RetargetBoneType.RightToe, "RightToeBase"},
            {RetargetBoneType.Spine, "Spine"},
            {RetargetBoneType.Spine1, "Spine1"},
            {RetargetBoneType.Chest, "Spine2"},
            {RetargetBoneType.Neck, "Neck"},
            {RetargetBoneType.Head, "Head"},
            {RetargetBoneType.LeftShoulder, "LeftShoulder"},
            {RetargetBoneType.LeftUpperarm, "LeftArm"},
            {RetargetBoneType.LeftForearm, "LeftForeArm"},
            {RetargetBoneType.LeftHand, "LeftHand"},
            {RetargetBoneType.RightShoulder, "RightShoulder"},
            {RetargetBoneType.RightUpperarm, "RightArm"},
            {RetargetBoneType.RightForearm, "RightForeArm"},
            {RetargetBoneType.RightHand, "RightHand"},
        };

        static Dictionary<RetargetBoneType, string> FingerType = new Dictionary<RetargetBoneType, string>()
        {
            {RetargetBoneType.LeftThumb1, "LeftHandThumb1"},
            {RetargetBoneType.LeftThumb2, "LeftHandThumb2"},
            {RetargetBoneType.LeftThumb3, "LeftHandThumb3"},
            {RetargetBoneType.LeftThumb4, "LeftHandThumb4"},
            {RetargetBoneType.LeftIndex1, "LeftHandIndex1"},
            {RetargetBoneType.LeftIndex2, "LeftHandIndex2"},
            {RetargetBoneType.LeftIndex3, "LeftHandIndex3"},
            {RetargetBoneType.LeftIndex4, "LeftHandIndex4"},
            {RetargetBoneType.LeftMiddle1, "LeftHandMiddle1"},
            {RetargetBoneType.LeftMiddle2, "LeftHandMiddle2"},
            {RetargetBoneType.LeftMiddle3, "LeftHandMiddle3"},
            {RetargetBoneType.LeftMiddle4, "LeftHandMiddle4"},
            {RetargetBoneType.LeftRing1, "LeftHandRing1"},
            {RetargetBoneType.LeftRing2, "LeftHandRing2"},
            {RetargetBoneType.LeftRing3, "LeftHandRing3"},
            {RetargetBoneType.LeftRing4, "LeftHandRing4"},
            {RetargetBoneType.LeftPinky1, "LeftHandPinky1"},
            {RetargetBoneType.LeftPinky2, "LeftHandPinky2"},
            {RetargetBoneType.LeftPinky3, "LeftHandPinky3"},
            {RetargetBoneType.LeftPinky4, "LeftHandPinky4"},
            {RetargetBoneType.RightThumb1, "RightHandThumb1"},
            {RetargetBoneType.RightThumb2, "RightHandThumb2"},
            {RetargetBoneType.RightThumb3, "RightHandThumb3"},
            {RetargetBoneType.RightThumb4, "RightHandThumb4"},
            {RetargetBoneType.RightIndex1, "RightHandIndex1"},
            {RetargetBoneType.RightIndex2, "RightHandIndex2"},
            {RetargetBoneType.RightIndex3, "RightHandIndex3"},
            {RetargetBoneType.RightIndex4, "RightHandIndex4"},
            {RetargetBoneType.RightMiddle1, "RightHandMiddle1"},
            {RetargetBoneType.RightMiddle2, "RightHandMiddle2"},
            {RetargetBoneType.RightMiddle3, "RightHandMiddle3"},
            {RetargetBoneType.RightMiddle4, "RightHandMiddle4"},
            {RetargetBoneType.RightRing1, "RightHandRing1"},
            {RetargetBoneType.RightRing2, "RightHandRing2"},
            {RetargetBoneType.RightRing3, "RightHandRing3"},
            {RetargetBoneType.RightRing4, "RightHandRing4"},
            {RetargetBoneType.RightPinky1, "RightHandPinky1"},
            {RetargetBoneType.RightPinky2, "RightHandPinky2"},
            {RetargetBoneType.RightPinky3, "RightHandPinky3"},
            {RetargetBoneType.RightPinky4, "RightHandPinky4"},
        };

        static Dictionary<RetargetBoneType, string> RotaboneType = new Dictionary<RetargetBoneType, string>()
        {
            {RetargetBoneType.LeftRotaBone, "LeftForeArm1"},
            {RetargetBoneType.RightRotaBone, "RightForeArm1"},
        };
    }
}