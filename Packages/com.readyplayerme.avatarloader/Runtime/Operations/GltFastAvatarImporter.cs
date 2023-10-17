using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GLTFast;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Loader;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class is responsible for the avatar model using the GltFast API.
    /// </summary>
    public class GltFastAvatarImporter : IImporter
    {
        private const string TAG = nameof(GltFastAvatarImporter);
        private const string IMPORTING_AVATAR_FROM_BYTE_ARRAY = "Importing avatar from byte array.";
        private readonly GLTFDeferAgent gltfDeferAgent;

        public int Timeout { get; set; }

        /// <summary>
        /// An <see cref="Action" /> callback that can be used to subscribe to <see cref="WebRequestDispatcherExtension" />
        /// <c>ProgressChanged</c> events.
        /// </summary>
        public Action<float> ProgressChanged { get; set; }

        public GltFastAvatarImporter(GLTFDeferAgent gltfDeferAgent = default)
        {
            this.gltfDeferAgent = gltfDeferAgent;
        }

        /// <summary>
        /// Executes the operation to import the module from the avatar model data.
        /// </summary>
        /// <param name="context">A container for all the data related to the Avatar model.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// ///
        /// <returns>The updated <see cref="AvatarContext" />.</returns>
        public async Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            if (context.Bytes == null)
            {
                throw new NullReferenceException();
            }

            context.Data = await ImportModel(context.Bytes, token);
            return context;
        }

        /// <summary>
        /// Imports the model from <c>byte[]</c> asynchronously.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// <returns>The <see cref="GameObject" /> of the avatar model.</returns>
        public async Task<GameObject> ImportModel(byte[] bytes, CancellationToken token)
        {
            SDKLogger.Log(TAG, IMPORTING_AVATAR_FROM_BYTE_ARRAY);

            try
            {
                GameObject avatar = null;
                IDeferAgent agent = gltfDeferAgent == null ? new UninterruptedDeferAgent() : gltfDeferAgent.GetGLTFastDeferAgent();

                var gltf = new GltfImport(deferAgent: agent);
                var success = await gltf.LoadGltfBinary(bytes, cancellationToken: token);
                if (success)
                {
                    avatar = new GameObject();
                    avatar.SetActive(false);
                    var customInstantiator = new GltFastGameObjectInstantiator(gltf, avatar.transform);

                    await gltf.InstantiateMainSceneAsync(customInstantiator, token);
                }

                return avatar;
            }
            catch (Exception exception)
            {
                throw Fail(exception.Message);
            }
        }

        /// <summary>
        /// Imports the model from the URL <c>string</c> asynchronously.
        /// </summary>
        /// <param name="path">The path to the file for importing.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// <returns>The instantiated GameObject of the imported model.</returns>
        public async Task<GameObject> ImportModel(string path, CancellationToken token)
        {
            SDKLogger.Log(TAG, $"Importing avatar from path {path}");

            try
            {
                GameObject avatar = null;

                var data = File.ReadAllBytes(path);

                var gltf = new GltfImport(deferAgent: new UninterruptedDeferAgent());

                var success = await gltf.LoadGltfBinary(
                    data,
                    new Uri(path)
                );

                if (success)
                {
                    avatar = new GameObject();
                    avatar.SetActive(false);
                    var customInstantiator = new GltFastGameObjectInstantiator(gltf, avatar.transform);

                    await gltf.InstantiateMainSceneAsync(customInstantiator);
                }

                return avatar;
            }
            catch (Exception exception)
            {
                throw Fail(exception.Message);
            }
        }

        /// <summary>
        /// A method used to throw the <see cref="FailureType.ModelImportError" /> exception.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <returns>The <see cref="Exception" />.</returns>
        private Exception Fail(string error)
        {
            var message = $"Failed to import glb model from bytes. {error}";
            SDKLogger.Log(TAG, message);
            throw new CustomException(FailureType.ModelImportError, message);
        }
    }
}
