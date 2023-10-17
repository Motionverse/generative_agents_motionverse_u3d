using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MotionverseSDK
{
    public class FaceAnimationHandler : MonoBehaviour
    {
        private const string TAG = nameof(FaceAnimationHandler);

        SkinnedMeshRenderer skinnedMeshRenderer;

        Dictionary<string, AnimationCurve> faceAnimCurves = new Dictionary<string, AnimationCurve>();
        Dictionary<string, string> bsDictionary = new Dictionary<string, string>();
        [SerializeField]
        float bsValueScale = 100.0f;
        void Start()
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

            for(int i = 0;i< skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
            {
                foreach (BlendShape value in Enum.GetValues(typeof(BlendShape)))
                {
                    if (skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i).Contains(value.ToString()))
                    {
                        bsDictionary.Add(value.ToString(), skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i));
                    }
                }
            }

           
        }
        public void Resume()
        {
            if (skinnedMeshRenderer == null)
                return;
            foreach (BlendShape value in Enum.GetValues(typeof(BlendShape)))
            {
                skinnedMeshRenderer.SetBlendShapeWeight((int)value, 0);
            }
        }
        public void SetAnimationData(string bsData)
        {
            faceAnimCurves.Clear();
            var blendshapeArr = JArray.Parse(bsData);
            float timeNow = 0f;
            Dictionary<int, List<Keyframe>> KeyframeData = new Dictionary<int, List<Keyframe>>();
            for (int i = 0; i < 51; i++)
                KeyframeData.Add(i, new List<Keyframe>());

            foreach (JArray item in blendshapeArr.Cast<JArray>())
            {
                for (int i = 0; i < 51; i++)
                {
                    KeyframeData[i].Add(new Keyframe(timeNow, (float)item[i]));
                }
                timeNow += 1f / 30f;
            }



            for (int i = 0; i < 51; i++)
            {
                faceAnimCurves.Add(((BlendShape)i).ToString(), new AnimationCurve(KeyframeData[i].ToArray()));
            }

        }

        public void PlayBS(float evalTime)
        {
            if (skinnedMeshRenderer == null)
                return;


            for (int i = 0; i < 51; ++i)
            {
                if (faceAnimCurves.TryGetValue(((BlendShape)i).ToString(), out AnimationCurve animationCurve))
                {
                    lock (animationCurve)
                    {
                        var bs = animationCurve.Evaluate(evalTime);
                        bs = bsValueScale * (bs / 100.0f);
                        if (bs < 0)
                            bs = 0;
                        if (bsDictionary.TryGetValue(((BlendShape)i).ToString(), out string name))
                        {
                            skinnedMeshRenderer.SetBlendShapeWeight(skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(name), bs);
                        }
                       
                    }
                }
            }

        }
    }
}