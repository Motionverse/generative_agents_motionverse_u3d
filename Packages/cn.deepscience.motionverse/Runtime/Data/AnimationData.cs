using System;
using System.Collections.Generic;
using UnityEngine;
namespace MotionverseSDK
{
    public class AnimationData
    {
        private const string TAG = nameof(AnimationData);
        public readonly Dictionary<string, Dictionary<string, AnimationCurve>> rotationCurves =new Dictionary<string, Dictionary<string, AnimationCurve>>();
        public readonly Dictionary<string, AnimationCurve> positionCurves = new Dictionary<string, AnimationCurve>();
        public Vector3 firstPos;
        public bool Init(string motionData)
        {
            try
            {
                var bodyData = Utilities.StrToObj<BodyData>(motionData);
                List<Keyframe> posX = new List<Keyframe>();
                List<Keyframe> posY = new List<Keyframe>();
                List<Keyframe> posZ = new List<Keyframe>();
                float timeNow = 0f;
                foreach (var jPos in bodyData.Hips.pos)
                {
                    posX.Add(new Keyframe(timeNow, jPos.x));
                    posY.Add(new Keyframe(timeNow, jPos.y));
                    posZ.Add(new Keyframe(timeNow, jPos.z));
                    if(timeNow == 0)
                    {
                        firstPos = new Vector3(jPos.x, jPos.y, jPos.z);
                    }
                    timeNow += 1.0f / 30.0f;
                }
                positionCurves.Add("m_LocalPos.x", new AnimationCurve(posX.ToArray()));
                positionCurves.Add("m_LocalPos.y", new AnimationCurve(posY.ToArray()));
                positionCurves.Add("m_LocalPos.z", new AnimationCurve(posZ.ToArray()));

                foreach (var item in bodyData.curves)
                {
                    timeNow = 0f;
                    List<Keyframe> rx = new List<Keyframe>();
                    List<Keyframe> ry = new List<Keyframe>();
                    List<Keyframe> rz = new List<Keyframe>();
                    List<Keyframe> rw = new List<Keyframe>();
                    foreach (var jRot in item.rotas)
                    {
                        rx.Add(new Keyframe(timeNow, jRot.x));
                        ry.Add(new Keyframe(timeNow, jRot.y));
                        rz.Add(new Keyframe(timeNow, jRot.z));
                        rw.Add(new Keyframe(timeNow, jRot.w));

                        timeNow += 1.0f / 30.0f;
                    }
                    Dictionary<string, AnimationCurve> boneCurves = new Dictionary<string, AnimationCurve>()
                    {
                        { "m_LocalRotation.x", new AnimationCurve(rx.ToArray()) },
                        { "m_LocalRotation.y", new AnimationCurve(ry.ToArray()) },
                        { "m_LocalRotation.z", new AnimationCurve(rz.ToArray()) },
                        { "m_LocalRotation.w", new AnimationCurve(rw.ToArray()) }
                    };

                    rotationCurves.Add(item.name, boneCurves);
                }
                return true;
            }
            catch (Exception e)
            {
                SDKLogger.Log(TAG, e.Message);
                return false;
            }

        }
        public bool Init(byte[] data)
        {
            int frameLength = (int)BitConverter.ToSingle(data, 0);
            int jointLength = (int)BitConverter.ToSingle(data, 4);
            float timeNow;
            for (int i = 0; i < jointLength; i++)
            {

                List<Keyframe> rx = new List<Keyframe>();
                List<Keyframe> ry = new List<Keyframe>();
                List<Keyframe> rz = new List<Keyframe>();
                List<Keyframe> rw = new List<Keyframe>();

                timeNow = 0;
                for (int j = 0; j < frameLength; j++)
                {

                    rx.Add(new Keyframe(timeNow, BitConverter.ToSingle(data, 16 * frameLength * i + j * 16 + 12)));
                    ry.Add(new Keyframe(timeNow, BitConverter.ToSingle(data, 16 * frameLength * i + j * 16 + 16)));
                    rz.Add(new Keyframe(timeNow, BitConverter.ToSingle(data, 16 * frameLength * i + j * 16 + 20)));
                    rw.Add(new Keyframe(timeNow, BitConverter.ToSingle(data, 16 * frameLength * i + j * 16 + 24)));

                    timeNow += 1.0f / 30.0f;
                }

                Dictionary<string, AnimationCurve> boneCurves = new Dictionary<string, AnimationCurve>()
                {
                    { "m_LocalRotation.x", new AnimationCurve(rx.ToArray()) },
                    { "m_LocalRotation.y", new AnimationCurve(ry.ToArray()) },
                    { "m_LocalRotation.z", new AnimationCurve(rz.ToArray()) },
                    { "m_LocalRotation.w", new AnimationCurve(rw.ToArray()) }
                };

                WrapMode wm = WrapMode.Once;

                boneCurves["m_LocalRotation.x"].preWrapMode = wm;
                boneCurves["m_LocalRotation.x"].postWrapMode = wm;
                boneCurves["m_LocalRotation.y"].preWrapMode = wm;
                boneCurves["m_LocalRotation.y"].postWrapMode = wm;
                boneCurves["m_LocalRotation.z"].preWrapMode = wm;
                boneCurves["m_LocalRotation.z"].postWrapMode = wm;
                boneCurves["m_LocalRotation.w"].preWrapMode = wm;
                boneCurves["m_LocalRotation.w"].postWrapMode = wm;
                rotationCurves.Add(((Skeleton)i).ToString(), boneCurves);

            }
            int startIndex = 16 * frameLength * (jointLength - 1) + (frameLength - 1) * 16 + 28 + 8;

            List<Keyframe> posX = new List<Keyframe>();
            List<Keyframe> posY = new List<Keyframe>();
            List<Keyframe> posZ = new List<Keyframe>();

  


            timeNow = 0f;
            for (int j = 0; j < frameLength; j++)
            {
                posX.Add(new Keyframe(timeNow, BitConverter.ToSingle(data, 12 * j + startIndex + 4)));
                posY.Add(new Keyframe(timeNow, BitConverter.ToSingle(data, 12 * j + startIndex + 8)));
                posZ.Add(new Keyframe(timeNow, BitConverter.ToSingle(data, 12 * j + startIndex + 12)));
                if(j == 0)
                {
                    if (timeNow == 0)
                    {
                        firstPos = new Vector3(BitConverter.ToSingle(data, 12 * j + startIndex + 4), BitConverter.ToSingle(data, 12 * j + startIndex + 8), BitConverter.ToSingle(data, 12 * j + startIndex + 12));
                    }
                }
                timeNow += 1.0f / 30.0f;
            }
            positionCurves.Add("m_LocalPos.x", new AnimationCurve(posX.ToArray()));
            positionCurves.Add("m_LocalPos.y", new AnimationCurve(posY.ToArray()));
            positionCurves.Add("m_LocalPos.z", new AnimationCurve(posZ.ToArray()));


            return true;
        }
    }

    public class BodyData
    {
        public float totalTime;
        public List<BoneData> curves = new List<BoneData>();
        public HipsData Hips = new HipsData();
    }
    public class BoneData
    {
        public string name;
        public List<Quaternion> rotas = new List<Quaternion>();
    }
    public class HipsData
    {
        public string name;
        public List<Vector3> pos = new List<Vector3>();
    }
}

