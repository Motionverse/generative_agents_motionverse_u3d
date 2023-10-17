using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class is a simple example on how to load a request a 2D render of a Ready Player Me avatar at runtime.
    /// </summary>
    public class MultipleAvatarRenderExample : MonoBehaviour
    {
        [Serializable]
        private class RenderData
        {
            public string url;
            public AvatarRenderScene avatarRenderScene;
            public bool imageLoaded;
        }

        private const string TAG = nameof(MultipleAvatarRenderExample);

        [SerializeField]
        private GameObject renderPanelPrefab;
        [SerializeField]
        private Transform renderPanelParent;
        [SerializeField]
        private RenderData[] dataList;
        [SerializeField]
        private GameObject loadingPanel;

        /// A collection of blendshape names and values to pose the face mesh into a smile using blendshapes
        private readonly Dictionary<string, float> blendShapes = new Dictionary<string, float>
        {
            { "mouthSmile", 0.7f },
            { "viseme_aa", 0.5f },
            { "jawOpen", 0.3f }
        };

        private readonly string[] blendShapeMeshes = { "Wolf3D_Head", "Wolf3D_Teeth" };


        private async void Start()
        {
            loadingPanel.SetActive(true);

            foreach (var renderData in dataList)
            {
                var avatarRenderer = new AvatarRenderLoader();
                avatarRenderer.OnCompleted = texture =>
                {
                    var renderPanel = Instantiate(renderPanelPrefab, renderPanelParent).GetComponent<RenderPanel>();
                    renderPanel.SetHeading(renderData.avatarRenderScene.ToString());
                    renderPanel.SetImage(texture);
                    SDKLogger.Log(TAG, "Sprite Updated ");
                    renderData.imageLoaded = true;
                };
                avatarRenderer.OnFailed = Fail;
                avatarRenderer.LoadRender(renderData.url, renderData.avatarRenderScene, blendShapeMeshes, blendShapes);
            }

            while (dataList.Any(x => !x.imageLoaded))
            {
                await Task.Yield();
            }
            loadingPanel.SetActive(false);
        }

        private void Fail(FailureType type, string message)
        {
            SDKLogger.Log(TAG, $"Failed with error type: {type} and message: {message}");
        }
    }
}
