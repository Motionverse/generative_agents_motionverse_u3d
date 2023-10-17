using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MotionverseSDK {
    [System.Serializable]
    public class HandFinger
    {
        public Transform m_Thumb1;//拇指 第1节
        public Transform m_Thumb2;
        public Transform m_Thumb3;
        public Transform m_Thumb4;

        public Transform m_Index1;//食指
        public Transform m_Index2;
        public Transform m_Index3;
        public Transform m_Index4;

        public Transform m_Middle1;//中指
        public Transform m_Middle2;
        public Transform m_Middle3;
        public Transform m_Middle4;

        public Transform m_Ring1;//无名指
        public Transform m_Ring2;
        public Transform m_Ring3;
        public Transform m_Ring4;

        public Transform m_Pinky1;//小指
        public Transform m_Pinky2;
        public Transform m_Pinky3;
        public Transform m_Pinky4;

        List<Transform> transformList = new List<Transform>();
        private readonly string[] fingerName = { "Thumb1", "Index1", "Middle1", "Ring1", "Pinky1",
                                            "Thumb2", "Index2", "Middle2", "Ring2", "Pinky2",
                                            "Thumb3", "Index3", "Middle3", "Ring3", "Pinky3",
                                            "Thumb4", "Index4", "Middle4", "Ring4", "Pinky4",};


        public HandFinger(Transform hand, bool isLeft)
        {
            if (hand == null)
            {
                Debug.LogError("hand is null");
                return;
            }

            Transform[] transforms = hand.GetComponentsInChildren<Transform>();
            foreach (var item in transforms)
            {
                if (item.name.Contains("Thumb"))
                {
                    if (item.name.Contains("1")) m_Thumb1 = item;
                    if (item.name.Contains("2")) m_Thumb2 = item;
                    if (item.name.Contains("3")) m_Thumb3 = item;
                    if (item.name.Contains("4")) m_Thumb4 = item;
                }
                else if (item.name.Contains("Index"))
                {
                    if (item.name.Contains("1")) m_Index1 = item;
                    if (item.name.Contains("2")) m_Index2 = item;
                    if (item.name.Contains("3")) m_Index3 = item;
                    if (item.name.Contains("4")) m_Index4 = item;
                }
                else if (item.name.Contains("Middle"))
                {
                    if (item.name.Contains("1")) m_Middle1 = item;
                    if (item.name.Contains("2")) m_Middle2 = item;
                    if (item.name.Contains("3")) m_Middle3 = item;
                    if (item.name.Contains("4")) m_Middle4 = item;
                }
                else if (item.name.Contains("Ring"))
                {
                    if (item.name.Contains("1")) m_Ring1 = item;
                    if (item.name.Contains("2")) m_Ring2 = item;
                    if (item.name.Contains("3")) m_Ring3 = item;
                    if (item.name.Contains("4")) m_Ring4 = item;
                }
                else if (item.name.Contains("Pinky"))
                {
                    if (item.name.Contains("1")) m_Pinky1 = item;
                    if (item.name.Contains("2")) m_Pinky2 = item;
                    if (item.name.Contains("3")) m_Pinky3 = item;
                    if (item.name.Contains("4")) m_Pinky4 = item;
                }

            }

            transformList.Add(m_Thumb1);
            transformList.Add(m_Thumb2);
            transformList.Add(m_Thumb3);
            transformList.Add(m_Thumb4);

            transformList.Add(m_Index1);
            transformList.Add(m_Index2);
            transformList.Add(m_Index3);
            transformList.Add(m_Index4);

            transformList.Add(m_Middle1);
            transformList.Add(m_Middle2);
            transformList.Add(m_Middle3);
            transformList.Add(m_Middle4);

            transformList.Add(m_Ring1);
            transformList.Add(m_Ring2);
            transformList.Add(m_Ring3);
            transformList.Add(m_Ring4);

            transformList.Add(m_Pinky1);
            transformList.Add(m_Pinky2);
            transformList.Add(m_Pinky3);
            transformList.Add(m_Pinky4);
        }

        public bool IsVal()
        {
            bool tag = m_Thumb1 == null || m_Thumb2 == null || m_Thumb3 == null ||
                      m_Index1 == null || m_Index2 == null || m_Index3 == null ||
                      m_Middle1 == null || m_Middle2 == null || m_Middle3 == null ||
                      m_Ring1 == null || m_Ring2 == null || m_Ring3 == null ||
                      m_Pinky1 == null || m_Pinky2 == null || m_Pinky3 == null;
            return tag;
        }

        public Transform[] GetFingers()
        {
            if (IsVal())
            {
                Debug.Log("手指没找到");
                return null;
            }
            Transform[] fingers = new Transform[20];
            fingers[0] = m_Thumb1;
            fingers[1] = m_Thumb2;
            fingers[2] = m_Thumb3;
            fingers[3] = m_Thumb4;
            fingers[4] = m_Index1;
            fingers[5] = m_Index2;
            fingers[6] = m_Index3;
            fingers[7] = m_Index4;
            fingers[8] = m_Middle1;
            fingers[9] = m_Middle2;
            fingers[10] = m_Middle3;
            fingers[11] = m_Middle4;
            fingers[12] = m_Ring1;
            fingers[13] = m_Ring2;
            fingers[14] = m_Ring3;
            fingers[15] = m_Ring4;
            fingers[16] = m_Pinky1;
            fingers[17] = m_Pinky2;
            fingers[18] = m_Pinky3;
            fingers[19] = m_Pinky4;
            return fingers;
        }
    }
}
