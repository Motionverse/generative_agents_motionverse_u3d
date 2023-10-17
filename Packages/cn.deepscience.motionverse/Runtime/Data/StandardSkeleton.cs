using System;
using System.Collections.Generic;
using UnityEngine;

namespace MotionverseSDK 
{
    [System.Serializable]
    public struct TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;

        public TransformInfo(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }

        public static TransformInfo Infinity
        {
            get
            {
                Quaternion r = new Quaternion(0, 0, 0, 0);
                return new TransformInfo(Vector3.negativeInfinity, r);
            }
        }

        public bool Check()
        {
            Quaternion r = new Quaternion(0, 0, 0, 0);
            if (position == Vector3.negativeInfinity || position == Vector3.positiveInfinity || rotation == r)
                return false;
            else
                return true;
        }
    }
    [Serializable]
    public class StandardSkeleton
    {
        public Transform m_root;
        public Transform m_transformHip;
        public Transform m_transformLeftThigh;
        public Transform m_transformRightThigh;
        public Transform m_transformLeftCalf;
        public Transform m_transformRightCalf;
        public Transform m_transformLeftFoot;
        public Transform m_transformRightFoot;
        public Transform[] m_transformSpines;
        public Transform m_transformNeck;
        public Transform m_transformHead;
        public Transform m_transformLeftShouder;
        public Transform m_transformRightShouder;
        public Transform m_transformLeftUpArm;
        public Transform m_transformRightUpArm;
        public Transform m_transformLeftForeArm;
        public Transform m_transformRightForeArm;
        public Transform m_transformLeftHand;
        public Transform m_transformRightHand;
        public Transform m_transformLeftRotateBone;
        public Transform m_transformRightRotateBone;
        public Transform m_transformLeftToe;
        public Transform m_transformRightToe;

        public List<Transform> BodySkeleton = new List<Transform>();
        public Dictionary<string, Transform> BodySkeletonDic = new Dictionary<string, Transform>();
        public bool has_rotabone { get; private set; }
        public bool m_val = false;
        public void CloneHuman(Transform root)
        {
            m_val = false;
            if (root == null)
            {
                return;
            }

            m_root = root;
            m_transformHip = root.Find("Hips");
            if (m_transformHip == null)
            {
                return;
            }
            m_transformLeftThigh = m_transformHip.Find("LeftUpLeg");
            if (m_transformLeftThigh == null)
            {
                return;
            }
            m_transformLeftCalf = m_transformLeftThigh.Find("LeftLeg");
            if (m_transformLeftCalf == null)
            {
                return;
            }
            m_transformLeftFoot = m_transformLeftCalf.Find("LeftFoot");
            if (m_transformLeftFoot == null)
            {
                return;
            }
            m_transformLeftToe = m_transformLeftFoot.Find("LeftToeBase");
            //
            m_transformRightThigh = m_transformHip.Find("RightUpLeg");
            if (m_transformLeftThigh == null)
            {
                return;
            }
            m_transformRightCalf = m_transformRightThigh.Find("RightLeg");
            if (m_transformRightCalf == null)
            {
                return;
            }
            m_transformRightFoot = m_transformRightCalf.Find("RightFoot");
            if (m_transformRightFoot == null)
            {
                return;
            }
            m_transformRightToe = m_transformRightFoot.Find("RightToeBase");

            List<Transform> spineList = new List<Transform>();
            Transform spine = m_transformHip.Find("Spine");
            spineList.Add(spine);
            int spinecount = 1;
            while ((spine = spine.Find("Spine" + spinecount)) != null)
            {
                spineList.Add(spine);
                ++spinecount;
            }
            m_transformSpines = new Transform[spineList.Count];
            for (int i = 0; i < spineList.Count; ++i)
            {
                m_transformSpines[i] = spineList[i];
            }
            //
            m_transformNeck = m_transformSpines[m_transformSpines.Length - 1].Find("Neck");
            if (m_transformNeck == null)
            {
                return;
            }
            m_transformHead = m_transformNeck.Find("Head");
            if (m_transformHead == null)
            {
                return;
            }
            //
            m_transformLeftShouder = m_transformSpines[m_transformSpines.Length - 1].Find("LeftShoulder");
            if (m_transformLeftShouder == null)
            {
                return;
            }
            m_transformLeftUpArm = m_transformLeftShouder.Find("LeftArm");
            if (m_transformLeftUpArm == null)
            {
                return;
            }
            m_transformLeftForeArm = m_transformLeftUpArm.Find("LeftForeArm");
            if (m_transformLeftForeArm == null)
            {
                return;
            }
            m_transformLeftHand = m_transformLeftForeArm.Find("LeftHand");
            if (m_transformLeftHand == null)
            {
                if ((m_transformLeftRotateBone = m_transformLeftForeArm.Find("LeftForeArm1")) != null)
                {
                    m_transformLeftHand = m_transformLeftRotateBone.Find("LeftHand");
                }
                else
                { return; }
            }
            //
            m_transformRightShouder = m_transformSpines[m_transformSpines.Length - 1].Find("RightShoulder");
            if (m_transformRightShouder == null)
            {
                return;
            }
            m_transformRightUpArm = m_transformRightShouder.Find("RightArm");
            if (m_transformRightUpArm == null)
            {
                return;
            }
            m_transformRightForeArm = m_transformRightUpArm.Find("RightForeArm");
            if (m_transformRightForeArm == null)
            {
                return;
            }
            m_transformRightHand = m_transformRightForeArm.Find("RightHand");
            if (m_transformRightHand == null)
            {
                if ((m_transformRightRotateBone = m_transformRightForeArm.Find("RightForeArm1")) != null)
                {
                    m_transformRightHand = m_transformRightRotateBone.Find("RightHand");
                }
                else
                { return; }
            }
            IsVal();
        }
        public bool IsVal()
        {
            m_val = false;
            m_val = m_transformHip != null &&
                   m_transformLeftThigh != null &&
                   m_transformRightThigh != null &&
                   m_transformLeftCalf != null &&
                   m_transformLeftFoot != null &&
                   m_transformRightCalf != null &&
                   m_transformRightFoot != null &&
                   m_transformSpines != null &&
                   m_transformSpines.Length == 3 &&
                   m_transformSpines[0] != null &&
                   m_transformSpines[1] != null &&
                   m_transformSpines[2] != null &&
                   m_transformLeftShouder != null &&
                   m_transformLeftUpArm != null &&
                   m_transformLeftForeArm != null &&
                   m_transformLeftHand != null &&
                   m_transformRightShouder != null &&
                   m_transformRightUpArm != null &&
                   m_transformRightForeArm != null &&
                   m_transformRightHand != null &&
                   m_transformNeck != null &&
                   m_transformHead != null &&
                   m_root != null;

            if (m_val)
            {
                
                if (BodySkeletonDic == null) BodySkeletonDic = new Dictionary<string, Transform>();


                if (!BodySkeletonDic.ContainsKey("Hips")) BodySkeletonDic.Add("Hips", m_transformHip);
                if (!BodySkeletonDic.ContainsKey("LeftUpLeg")) BodySkeletonDic.Add("LeftUpLeg", m_transformLeftThigh);
                if (!BodySkeletonDic.ContainsKey("RightUpLeg")) BodySkeletonDic.Add("RightUpLeg", m_transformRightThigh);
                if (!BodySkeletonDic.ContainsKey("LeftLeg")) BodySkeletonDic.Add("LeftLeg", m_transformLeftCalf);
                if (!BodySkeletonDic.ContainsKey("LeftFoot")) BodySkeletonDic.Add("LeftFoot", m_transformLeftFoot);
                if (!BodySkeletonDic.ContainsKey("LeftToeBase")) BodySkeletonDic.Add("LeftToeBase", m_transformLeftToe);
                if (!BodySkeletonDic.ContainsKey("RightLeg")) BodySkeletonDic.Add("RightLeg", m_transformRightCalf);
                if (!BodySkeletonDic.ContainsKey("RightFoot")) BodySkeletonDic.Add("RightFoot", m_transformRightFoot);
                if (!BodySkeletonDic.ContainsKey("Spine")) BodySkeletonDic.Add("Spine", m_transformSpines[0]);
                if (!BodySkeletonDic.ContainsKey("Spine1")) BodySkeletonDic.Add("Spine1", m_transformSpines[1]);
                if (!BodySkeletonDic.ContainsKey("Spine2")) BodySkeletonDic.Add("Spine2", m_transformSpines[2]);
                if (!BodySkeletonDic.ContainsKey("LeftShoulder")) BodySkeletonDic.Add("LeftShoulder", m_transformLeftShouder);
                if (!BodySkeletonDic.ContainsKey("LeftArm")) BodySkeletonDic.Add("LeftArm", m_transformLeftUpArm);
                if (!BodySkeletonDic.ContainsKey("LeftForeArm")) BodySkeletonDic.Add("LeftForeArm", m_transformLeftForeArm);
                if (!BodySkeletonDic.ContainsKey("LeftHand")) BodySkeletonDic.Add("LeftHand", m_transformLeftHand);
                if (!BodySkeletonDic.ContainsKey("RightShoulder")) BodySkeletonDic.Add("RightShoulder", m_transformRightShouder);
                if (!BodySkeletonDic.ContainsKey("RightArm")) BodySkeletonDic.Add("RightArm", m_transformRightUpArm);
                if (!BodySkeletonDic.ContainsKey("RightForeArm")) BodySkeletonDic.Add("RightForeArm", m_transformRightForeArm);
                if (!BodySkeletonDic.ContainsKey("RightHand")) BodySkeletonDic.Add("RightHand", m_transformRightHand);
                if (!BodySkeletonDic.ContainsKey("Neck")) BodySkeletonDic.Add("Neck", m_transformNeck);
                if (!BodySkeletonDic.ContainsKey("Head")) BodySkeletonDic.Add("Head", m_transformHead);
                if (!BodySkeletonDic.ContainsKey("Root")) BodySkeletonDic.Add("Root", m_root);

                if (m_transformLeftRotateBone != null && m_transformRightRotateBone != null)
                {
                    has_rotabone = true;
                }
            }
            return m_val;
        }
    }
}

