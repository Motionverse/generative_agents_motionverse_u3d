using System;
using System.Collections.Generic;
using UnityEngine;

namespace MotionverseSDK
{
    public class PoserAnimationHandler : MonoBehaviour
    {
        private const string TAG = nameof(PoserAnimationHandler);
        [SerializeField]
      
        List<Transform> bones = new List<Transform>();
        AnimationData animationData;
        private Player m_player;
        private Dictionary<string, Quaternion> lastRotList = new Dictionary<string, Quaternion>();
        bool isPlaying = false;
        void Start()
        {
            m_player = transform.parent.parent.GetComponent<Player>();

            foreach (Skeleton bone in Enum.GetValues(typeof(Skeleton)))
            {
                bones.Add(Utilities.FindChildRecursively(transform, bone.ToString()));
            }
        }
        public void SetAnimationDataFromByte(byte[] data)
        {
            foreach (var bone in bones)
            {
                lastRotList[bone.name] = bone.localRotation;
            }

            animationData = new AnimationData();
            animationData.Init(data);
            isPlaying = true;
        }
        public void PlayOver() 
        {
            isPlaying = false;
        }

        public void PlayMotion(float evalTime)
        {
           
            if (animationData == null)
                return;

            foreach (var bone in bones)
            {
                
                if (animationData.rotationCurves.TryGetValue(bone.name, out var animCurves))
                {
                    var to = new Quaternion(
                        animCurves["m_LocalRotation.x"].Evaluate(evalTime),
                        animCurves["m_LocalRotation.y"].Evaluate(evalTime),
                        animCurves["m_LocalRotation.z"].Evaluate(evalTime),
                        animCurves["m_LocalRotation.w"].Evaluate(evalTime));
                    if (m_player.isBlend)
                    {
                        var a = lastRotList[bone.name];
                        Quaternion localRot = Quaternion.Lerp(a, to, 0.01f * 320.0f / fps);
                        bone.localRotation = localRot;
                    }
                    else
                    {
                        bone.localRotation = to;
                    }
                    lastRotList[bone.name] = bone.localRotation;
                }
            }

            if (animationData.positionCurves.TryGetValue("m_LocalPos.x", out AnimationCurve x) && animationData.positionCurves.TryGetValue("m_LocalPos.y", out AnimationCurve y) && animationData.positionCurves.TryGetValue("m_LocalPos.z", out AnimationCurve z))
            {
                var to = new Vector3(x.Evaluate(evalTime)-animationData.firstPos.x, bones[0].localPosition.y, z.Evaluate(evalTime)-animationData.firstPos.z);
                bones[0].localPosition = to;
            }
        }
        private void LateUpdate()
        {
            if (!isPlaying && m_player&& m_player.getBlend())
            {
                foreach (var bone in bones)
                {
                    var a = lastRotList[bone.name];
                    Quaternion b = bone.localRotation;
                    Quaternion localRot = Quaternion.Slerp(a, b, 0.01f * 320.0f / fps);
                    bone.localRotation = localRot;
                    lastRotList[bone.name] = bone.localRotation;
                }
            }
        }
        private float updateTimeT = 0.1f;
        private int framesCount = 0;
        private float accumTime = 0.0f;
        private float leftTime = 0.0f;
        private string stringFps;
        private float fps;

        private void Update()
        {
            leftTime -= Time.unscaledDeltaTime; 
            accumTime += Time.unscaledDeltaTime;
            ++framesCount;

            if (leftTime <= 0)
            {
                fps = framesCount / accumTime;
                accumTime = 0.0f;                     
                framesCount = 0;
                leftTime = updateTimeT;
            }
        }
    }
}
