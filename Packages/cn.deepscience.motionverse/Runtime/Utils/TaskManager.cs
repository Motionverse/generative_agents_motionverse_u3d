using System.Collections;
namespace MotionverseSDK
{
    public class TaskManager : Singleton<TaskManager>
    {
        public void Create(IEnumerator routine)
        {
            StartCoroutine(routine);
        }
    }
}
