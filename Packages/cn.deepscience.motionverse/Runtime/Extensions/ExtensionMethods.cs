using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Linq;

namespace MotionverseSDK
{
    public static class ExtensionMethods
    {
        #region Coroutine Runner

        [ExecuteInEditMode]
        public class CoroutineRunner : MonoBehaviour
        {
            ~CoroutineRunner()
            {
                Destroy(gameObject);
            }
        }

        private static CoroutineRunner operation;


        public static Coroutine Run(this IEnumerator iEnumerator)
        {
            CoroutineRunner[] operations = Resources.FindObjectsOfTypeAll<CoroutineRunner>();
            if (operations.Length == 0)
            {
                operation = new GameObject("[CoroutineRunner]").AddComponent<CoroutineRunner>();

            }
            else
            {
                operation = operations[0];
            }

            return operation.StartCoroutine(iEnumerator);
        }

        public static void Stop(this Coroutine coroutine)
        {
            if (operation != null)
            {
                operation.StopCoroutine(coroutine);
            }
        }

        #endregion

        #region Get Picker

        private static readonly string[] HeadMeshNameFilter = { "Face_51" };
        private static readonly string[] BodyMeshNameFilter = { "body", "polySurface1", "body2", "body1", "Body", "f_avg", "NvKeFu_Clothes", "Render" };
        public static SkinnedMeshRenderer GetMeshRenderer(this GameObject gameObject, MeshType meshType)
        {
            SkinnedMeshRenderer mesh;
            var children = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();

            if (children.Count == 0)
            {
                Debug.Log("ExtensionMethods.GetMeshRenderer: No SkinnedMeshRenderer found on the Game Object.");
                return null;
            }

            switch (meshType)
            {
                case MeshType.HeadMesh:
                    mesh = children.FirstOrDefault(child => HeadMeshNameFilter.Contains(child.name));
                    break;
                case MeshType.BodyMesh:
                    mesh = children.FirstOrDefault(child => BodyMeshNameFilter.Contains(child.name));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(meshType), meshType, null);
            }

            if (mesh != null) return mesh;

            Debug.Log($"ExtensionMethods.GetMeshRenderer: Mesh type {meshType} not found on the Game Object.");
            return null;
        }

        #endregion
        public static TaskAwaiter<object> GetAwaiter(this UnityWebRequestAsyncOperation op)
        {
            var tcs = new TaskCompletionSource<object>();
            op.completed += (obj) =>
            {
                tcs.SetResult(null);
            };
            return tcs.Task.GetAwaiter();
        }
    }
}
