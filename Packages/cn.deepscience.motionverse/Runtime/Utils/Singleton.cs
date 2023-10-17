using UnityEngine;
namespace MotionverseSDK
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T sInstanceScript = null;
        private static GameObject sInstanceObject = null;
        private static bool sIsInitialized = false;

        public static T instance
        {
            get
            {
                if (!sIsInitialized)
                {
                    sInstanceObject = new GameObject(typeof(T).FullName);
                    sInstanceScript = sInstanceObject.AddComponent<T>();
                    sIsInitialized = true;
                }
                return sInstanceScript;
            }
        }

        public static void Clear()
        {
            if (sInstanceScript != null)
            {
                Destroy(sInstanceObject);
                sInstanceScript = null;
                sInstanceObject = null;
                sIsInitialized = false;
            }
        }

        protected virtual void Awake()
        {
            if (sInstanceScript != null)
            {
                Destroy(gameObject);
            }
            else
            {
                sInstanceScript = (T)this;
                sInstanceObject = gameObject;
                sIsInitialized = true;
            }
        }
    }
}
