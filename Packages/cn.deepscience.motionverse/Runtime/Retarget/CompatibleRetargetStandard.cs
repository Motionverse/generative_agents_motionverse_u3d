
using System.Collections.Generic;
using UnityEngine;
namespace MotionverseSDK
{
    public class CompatibleRetargetStandard
    {
        private static Dictionary<string, CompatibleRetargetSolver> RetargetDictionary = new Dictionary<string, CompatibleRetargetSolver>();

        #region main interface
        /// <summary>
        /// 使用unity坐标系，初始化要求源模型和目标模型都符合人体正面朝向forward方向，保持T pose，所有手指并拢伸直
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool RetargetInit(string id, StandardMotion source, Dictionary<RetargetBoneType, Joint_t> target)
        {
            if (RetargetDictionary.ContainsKey(id))
            {
                Debug.Log(id + " retargeter was initialized");
                return false;
            }

            var srcPose = Translate(source);
            if (srcPose is null || srcPose.Count <= 0) 
            {
                Debug.Log(id + " source is illegal");
                return false;
            }

            CompatibleRetargetSolver solver = new CompatibleRetargetSolver();
            if (!solver.Init(srcPose, target))
            {
                Debug.Log(id + " retargeter initialize failed");
                return false;
            }

            RetargetDictionary.Add(id, solver);
            return true;
        }

        public static Dictionary<RetargetBoneType, Joint_t> UpdateRetarget(string id, StandardMotion frame)
        {
            if (!RetargetDictionary.ContainsKey(id))
            {
                Debug.Log(id + " retargeter does not exist");
                return new Dictionary<RetargetBoneType, Joint_t>();
            }

            var f = Translate(frame);

            if (f is null || f.Count <= 0)
            {
                Debug.Log(id + " frame is illegal");
                return new Dictionary<RetargetBoneType, Joint_t>();
            }
            if (RetargetDictionary[id].UpdateSolver(f))
            {
                var targetFrame = RetargetDictionary[id].GetFrame();
                if (targetFrame != null && targetFrame.Count > 0)
                {
                    return targetFrame;
                }
                else
                {
                    Debug.LogError(id + " get motion failed");
                    return new Dictionary<RetargetBoneType, Joint_t>();
                }
            }
            else
            {
                Debug.LogError(id + " update solver failed");
                return new Dictionary<RetargetBoneType, Joint_t>();
            }
        }

        public static void Close(string id)
        {
            if (RetargetDictionary.ContainsKey(id))
            {
                RetargetDictionary[id].Clear();
                RetargetDictionary.Remove(id);
            }
        }

        public static void SetNewTransfrom(string id, Vector3 newCoordinationPosition, Quaternion newCoordinationRotation)
        {
            if (RetargetDictionary.ContainsKey(id))
            {
                RetargetDictionary[id].SetOrigin(newCoordinationPosition, newCoordinationRotation);
            }
            else
            {
                Debug.Log(id + " retargeter does not exist");
            }
        }

        public static void SetHandOffset(string id, Vector3 leftHandPosOffset, Vector3 rightHandPosOffset)
        {
            if (RetargetDictionary.ContainsKey(id))
            {
                RetargetDictionary[id].leftHandPosOffset = leftHandPosOffset;
                RetargetDictionary[id].leftHandRotOffset = Quaternion.identity;
                RetargetDictionary[id].rightHandPosOffset = rightHandPosOffset;
                RetargetDictionary[id].rightHandRotOffset = Quaternion.identity;
            }
            else
            {
                Debug.Log(id + " retargeter does not exist");
            }
        }

        public static void SetHeadRotationOffset(string id, Quaternion offset)
        {
            if (RetargetDictionary.ContainsKey(id))
            {
                RetargetDictionary[id].headOffset = offset;
            }
            else
            {
                Debug.Log(id + " retargeter does not exist");
            }
        }

        #endregion

        #region other method

        static Dictionary<RetargetBoneType, int> JointDictionaryPair = new Dictionary<RetargetBoneType, int>()
        {
            {RetargetBoneType.Root, StandardMotion.RootIndex},
            {RetargetBoneType.Hip, StandardMotion.HipIndex},
            {RetargetBoneType.LeftThigh, StandardMotion.LeftThighIndex},
            {RetargetBoneType.LeftCalf, StandardMotion.LeftCalfIndex},
            {RetargetBoneType.LeftFoot, StandardMotion.LeftFootIndex},
            {RetargetBoneType.LeftToe, StandardMotion.LeftToeIndex},
            {RetargetBoneType.RightThigh, StandardMotion.RightThighIndex},
            {RetargetBoneType.RightCalf, StandardMotion.RightCalfIndex},
            {RetargetBoneType.RightFoot, StandardMotion.RightFootIndex},
            {RetargetBoneType.RightToe, StandardMotion.RightToeIndex},
            {RetargetBoneType.Spine, StandardMotion.SpineIndex},
            {RetargetBoneType.Spine1, StandardMotion.Spine1Index},
            {RetargetBoneType.Chest, StandardMotion.ChestIndex},
            {RetargetBoneType.Neck, StandardMotion.NeckIndex},
            {RetargetBoneType.Head, StandardMotion.HeadIndex},
            {RetargetBoneType.LeftShoulder, StandardMotion.LeftShoulderIndex},
            {RetargetBoneType.LeftUpperarm, StandardMotion.LeftUpperarmIndex},
            {RetargetBoneType.LeftForearm, StandardMotion.LeftForearmIndex},
            {RetargetBoneType.LeftHand, StandardMotion.LeftHandIndex},
            {RetargetBoneType.RightShoulder, StandardMotion.RightShoulderIndex},
            {RetargetBoneType.RightUpperarm, StandardMotion.RightUpperarmIndex},
            {RetargetBoneType.RightForearm, StandardMotion.RightForearmIndex},
            {RetargetBoneType.RightHand, StandardMotion.RightHandIndex},
        };

        static Dictionary<RetargetBoneType, int> FingerDictionaryPair = new Dictionary<RetargetBoneType, int>()
        {
            {RetargetBoneType.LeftThumb1, StandardMotion.Thumb1Index},
            {RetargetBoneType.LeftThumb2, StandardMotion.Thumb2Index},
            {RetargetBoneType.LeftThumb3, StandardMotion.Thumb3Index},
            {RetargetBoneType.LeftThumb4, StandardMotion.Thumb4Index},
            {RetargetBoneType.LeftIndex1, StandardMotion.Index1Index},
            {RetargetBoneType.LeftIndex2, StandardMotion.Index2Index},
            {RetargetBoneType.LeftIndex3, StandardMotion.Index3Index},
            {RetargetBoneType.LeftIndex4, StandardMotion.Index4Index},
            {RetargetBoneType.LeftMiddle1, StandardMotion.Middle1Index},
            {RetargetBoneType.LeftMiddle2, StandardMotion.Middle2Index},
            {RetargetBoneType.LeftMiddle3, StandardMotion.Middle3Index},
            {RetargetBoneType.LeftMiddle4, StandardMotion.Middle4Index},
            {RetargetBoneType.LeftRing1, StandardMotion.Ring1Index},
            {RetargetBoneType.LeftRing2, StandardMotion.Ring2Index},
            {RetargetBoneType.LeftRing3, StandardMotion.Ring3Index},
            {RetargetBoneType.LeftRing4, StandardMotion.Ring4Index},
            {RetargetBoneType.LeftPinky1, StandardMotion.Pinky1Index},
            {RetargetBoneType.LeftPinky2, StandardMotion.Pinky2Index},
            {RetargetBoneType.LeftPinky3, StandardMotion.Pinky3Index},
            {RetargetBoneType.LeftPinky4, StandardMotion.Pinky4Index},

            {RetargetBoneType.RightThumb1, StandardMotion.Thumb1Index},
            {RetargetBoneType.RightThumb2, StandardMotion.Thumb2Index},
            {RetargetBoneType.RightThumb3, StandardMotion.Thumb3Index},
            {RetargetBoneType.RightThumb4, StandardMotion.Thumb4Index},
            {RetargetBoneType.RightIndex1, StandardMotion.Index1Index},
            {RetargetBoneType.RightIndex2, StandardMotion.Index2Index},
            {RetargetBoneType.RightIndex3, StandardMotion.Index3Index},
            {RetargetBoneType.RightIndex4, StandardMotion.Index4Index},
            {RetargetBoneType.RightMiddle1, StandardMotion.Middle1Index},
            {RetargetBoneType.RightMiddle2, StandardMotion.Middle2Index},
            {RetargetBoneType.RightMiddle3, StandardMotion.Middle3Index},
            {RetargetBoneType.RightMiddle4, StandardMotion.Middle4Index},
            {RetargetBoneType.RightRing1, StandardMotion.Ring1Index},
            {RetargetBoneType.RightRing2, StandardMotion.Ring2Index},
            {RetargetBoneType.RightRing3, StandardMotion.Ring3Index},
            {RetargetBoneType.RightRing4, StandardMotion.Ring4Index},
            {RetargetBoneType.RightPinky1, StandardMotion.Pinky1Index},
            {RetargetBoneType.RightPinky2, StandardMotion.Pinky2Index},
            {RetargetBoneType.RightPinky3, StandardMotion.Pinky3Index},
            {RetargetBoneType.RightPinky4, StandardMotion.Pinky4Index},
        };

        static Dictionary<RetargetBoneType, int> RotaboneDictionaryPair = new Dictionary<RetargetBoneType, int>()
        {
            {RetargetBoneType.LeftRotaBone, StandardMotion.LeftRotaboneIndex},
            {RetargetBoneType.RightRotaBone, StandardMotion.RightRotaboneIndex},
        };

        public static Dictionary<RetargetBoneType, Joint_t> Translate(StandardMotion source)
        {
            if (!source.hasBody) return new Dictionary<RetargetBoneType, Joint_t>();

            Dictionary<RetargetBoneType, Joint_t> dic = new Dictionary<RetargetBoneType, Joint_t>();
            foreach (var keyPair in JointDictionaryPair)
            {
                dic.Add(keyPair.Key, Joint_t.Create(source.positions[keyPair.Value], source.rotations[keyPair.Value]));
            }

            if (source.hasRotabone)
            {
                foreach (var keyPair in RotaboneDictionaryPair)
                {
                    dic.Add(keyPair.Key, Joint_t.Create(source.positions[keyPair.Value], source.rotations[keyPair.Value]));
                }
            }
         

            if (source.hasFingers)
            {
                int i = 0;
                foreach (var keyPair in FingerDictionaryPair)
                {
                    if(i < 20)
                        dic.Add(keyPair.Key, Joint_t.Create(source.leftFingersPositions[keyPair.Value], source.leftFingersRotations[keyPair.Value]));
                    else
                        dic.Add(keyPair.Key, Joint_t.Create(source.rightFingersPositions[keyPair.Value], source.rightFingersRotations[keyPair.Value]));
                    ++i;
                }
            }
            return dic;
        }

        #endregion
    }
}
