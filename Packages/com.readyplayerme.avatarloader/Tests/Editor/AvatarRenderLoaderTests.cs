using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class AvatarRenderLoaderTests
    {
        private const AvatarRenderScene RENDER_SCENE = AvatarRenderScene.FullbodyPortraitTransparent;
        private readonly string[] renderBlendshapeMeshes = { "Wolf3D_Head" };
        private readonly string[] renderWrongBlendshapeMesh = { "wrong_blendshape_mesh" };
        private const string RENDER_BLENDSHAPE = "mouthSmile";
        private const string RENDER_WRONG_BLENDSHAPE = "wrong_blendshape";
        private const float BLENDSHAPE_VALUE = 0.5f;


        [UnityTest]
        public IEnumerator RenderLoader_Load()
        {
            Texture2D renderTexture = null;
            var failureType = FailureType.None;

            var renderLoader = new AvatarRenderLoader();
            renderLoader.OnCompleted = data => renderTexture = data;
            renderLoader.OnFailed = (failType, message) => failureType = failType;

            renderLoader.LoadRender(TestAvatarData.DefaultAvatarUri.ModelUrl, RENDER_SCENE);

            yield return new WaitUntil(() => renderTexture != null || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(renderTexture);
        }

        [UnityTest]
        public IEnumerator Fail_RenderLoader_Load_Wrong_Url()
        {
            Texture2D renderTexture = null;
            var failureType = FailureType.None;

            var renderLoader = new AvatarRenderLoader();
            renderLoader.OnCompleted = data => renderTexture = data;
            renderLoader.OnFailed = (failType, message) => failureType = failType;

            renderLoader.LoadRender(TestAvatarData.WrongUri.ModelUrl, RENDER_SCENE);

            yield return new WaitUntil(() => renderTexture != null || failureType != FailureType.None);

            Assert.AreEqual(FailureType.MetadataDownloadError, failureType);
            Assert.IsNull(renderTexture);
        }

        [UnityTest]
        public IEnumerator RenderLoader_Load_With_Correct_BlendShape_Parameters()
        {
            Texture2D renderTexture = null;
            var failureType = FailureType.None;

            var renderLoader = new AvatarRenderLoader();
            renderLoader.OnCompleted = data => renderTexture = data;
            renderLoader.OnFailed = (failType, message) => failureType = failType;

            renderLoader.LoadRender(
                TestAvatarData.DefaultAvatarUri.ModelUrl,
                RENDER_SCENE,
                renderBlendshapeMeshes,
                new Dictionary<string, float> { { RENDER_BLENDSHAPE, BLENDSHAPE_VALUE } }
            );

            yield return new WaitUntil(() => renderTexture != null || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(renderTexture);
        }

        [UnityTest]
        public IEnumerator RenderLoader_Load_Incorrect_BlendShape_Mesh_Parameter()
        {
            Texture2D renderTexture = null;
            var failureType = FailureType.None;

            var renderLoader = new AvatarRenderLoader();
            renderLoader.OnCompleted = data => renderTexture = data;
            renderLoader.OnFailed = (failType, message) => failureType = failType;

            renderLoader.LoadRender(
                TestAvatarData.DefaultAvatarUri.ModelUrl,
                RENDER_SCENE,
                renderWrongBlendshapeMesh,
                new Dictionary<string, float> { { RENDER_BLENDSHAPE, BLENDSHAPE_VALUE } }
            );

            yield return new WaitUntil(() => renderTexture != null || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(renderTexture);
        }

        [UnityTest]
        public IEnumerator RenderLoader_Load_Incorrect_BlendShape_Shape_Parameter()
        {
            Texture2D renderTexture = null;
            var failureType = FailureType.None;

            var renderLoader = new AvatarRenderLoader();
            renderLoader.OnCompleted = data => renderTexture = data;
            renderLoader.OnFailed = (failType, message) => failureType = failType;

            renderLoader.LoadRender(
                TestAvatarData.DefaultAvatarUri.ModelUrl,
                RENDER_SCENE,
                renderBlendshapeMeshes,
                new Dictionary<string, float> { { RENDER_WRONG_BLENDSHAPE, BLENDSHAPE_VALUE } }
            );

            yield return new WaitUntil(() => renderTexture != null || failureType != FailureType.None);

            Assert.AreEqual(FailureType.None, failureType);
            Assert.IsNotNull(renderTexture);
        }
    }
}
