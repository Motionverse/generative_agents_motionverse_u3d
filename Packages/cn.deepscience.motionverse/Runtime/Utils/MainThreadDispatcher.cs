using System.Collections.Generic;
using UnityEngine;
namespace MotionverseSDK
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<System.Action> _executionQueue = new Queue<System.Action>();
        private static MainThreadDispatcher _instance;

        private void Awake()
        {
            _instance = this;
        }

        private void Update()
        {
            lock (_executionQueue)
            {
                while (_executionQueue.Count > 0)
                {
                    _executionQueue.Dequeue().Invoke();
                }
            }
        }

        public static void Enqueue(System.Action action)
        {
            lock (_executionQueue)
            {
                _executionQueue.Enqueue(action);
            }
        }
    }
}
