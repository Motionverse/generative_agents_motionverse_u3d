using System.Collections;
using UnityEngine;
namespace MotionverseSDK
{
    public class EyeAnimationHandler : MonoBehaviour
    {
        private const string TAG = nameof(EyeAnimationHandler);
        [SerializeField, Range(0, 1)] private float blinkSpeed = 0.1f;

        public float BlinkSpeed
        {
            get => blinkSpeed;
            set
            {
                blinkSpeed = value;
                if (Application.isPlaying) Initialize();
            }
        }

        [SerializeField, Range(1, 10)] private float blinkInterval = 3f;

        public float BlinkInterval
        {
            get => blinkInterval;
            set
            {
                blinkInterval = value;
                if (Application.isPlaying) Initialize();
            }
        }

        private WaitForSeconds blinkDelay;
        private Coroutine blinkCoroutine;

        private const int VERTICAL_MARGIN = 15;
        private const int HORIZONTAL_MARGIN = 5;

        private SkinnedMeshRenderer headMesh;
        [SerializeField]
        private string EYE_BLINK_LEFT_BLEND_SHAPE_NAME = "blendShape.eyeBlinkLeft";
        [SerializeField]
        private string EYE_BLINK_RIGHT_BLEND_SHAPE_NAME = "blendShape.eyeBlinkRight";
        private int eyeBlinkLeftBlendShapeIndex = -1;
        private int eyeBlinkRightBlendShapeIndex = -1;


        private const float EYE_BLINK_MULTIPLIER = 100f;
        private bool hasEyeBlendShapes;

        private void Start()
        {
            headMesh = GetComponent<SkinnedMeshRenderer>();
            eyeBlinkLeftBlendShapeIndex = headMesh.sharedMesh.GetBlendShapeIndex(EYE_BLINK_LEFT_BLEND_SHAPE_NAME);
            eyeBlinkRightBlendShapeIndex = headMesh.sharedMesh.GetBlendShapeIndex(EYE_BLINK_RIGHT_BLEND_SHAPE_NAME);
            hasEyeBlendShapes = (eyeBlinkLeftBlendShapeIndex > -1 && eyeBlinkRightBlendShapeIndex > -1);

        }

        private void OnDisable() => CancelInvoke();

        private void OnEnable() => Initialize();

        private void OnDestroy()
        {
            CancelInvoke();
            blinkCoroutine?.Stop();
        }

        public void Initialize()
        {
            blinkDelay = new WaitForSeconds(blinkSpeed);

            CancelInvoke();
            InvokeRepeating(nameof(AnimateEyes), 1, blinkInterval);
        }

        private void AnimateEyes()
        {
            RotateEyes();

            if (hasEyeBlendShapes)
            {
                blinkCoroutine = BlinkEyes().Run();
            }
        }

        private void RotateEyes()
        {
            float vertical = Random.Range(-VERTICAL_MARGIN, VERTICAL_MARGIN);
            float horizontal = Random.Range(-HORIZONTAL_MARGIN, HORIZONTAL_MARGIN);


        }

        private IEnumerator BlinkEyes()
        {
            headMesh.SetBlendShapeWeight(eyeBlinkLeftBlendShapeIndex, EYE_BLINK_MULTIPLIER);
            headMesh.SetBlendShapeWeight(eyeBlinkRightBlendShapeIndex, EYE_BLINK_MULTIPLIER);

            yield return blinkDelay;

            headMesh.SetBlendShapeWeight(eyeBlinkLeftBlendShapeIndex, 0);
            headMesh.SetBlendShapeWeight(eyeBlinkRightBlendShapeIndex, 0);
        }
    }
}