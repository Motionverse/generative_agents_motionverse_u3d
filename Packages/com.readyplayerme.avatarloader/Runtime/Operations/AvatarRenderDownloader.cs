using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class is responsible for requesting and downloading a 2D render of an avatar from a URL.
    /// </summary>
    public class AvatarRenderDownloader : IOperation<AvatarContext>
    {
        private const string TAG = nameof(AvatarRenderDownloader);
        private const string AVATAR_RENDER_DOWNLOADED = "Avatar Render Downloaded";

        /// <summary>
        /// Can be used to set the Timeout (in seconds) used by the <see cref="WebRequestDispatcherExtension" /> when making the web request.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// An <see cref="Action" /> callback that can be used to subscribe to <see cref="WebRequestDispatcherExtension" />
        /// <c>ProgressChanged</c> events.
        /// </summary>
        public Action<float> ProgressChanged { get; set; }

        /// <summary>
        /// Executes the operation to request and download the 2D render and returns the updated context.
        /// </summary>
        /// <param name="context">A container for all the data related to the Avatar model.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        /// <returns>The updated <c>AvatarContext</c>.</returns>
        public async Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            try
            {
                var renderUrl = RenderParameterProcessor.GetRenderUrl(context);
                context.Data = await RequestAvatarRender(renderUrl, token);
                SDKLogger.Log(TAG, AVATAR_RENDER_DOWNLOADED);
                return context;
            }
            catch (CustomException exception)
            {
                throw new CustomException(FailureType.AvatarRenderError, exception.Message);
            }
        }

        /// <summary>
        /// Requests an avatar render URL asynchronously
        /// </summary>
        /// <param name="payload">The binary data of the avatar model .glb file.</param>
        /// <param name="token">Can be used to cancel the operation.</param>
        public async Task<Texture2D> RequestAvatarRender(string url, CancellationToken token = new CancellationToken())
        {
            var webRequestDispatcher = new WebRequestDispatcher();
            webRequestDispatcher.ProgressChanged += ProgressChanged;

            try
            {
                return await webRequestDispatcher.DownloadTexture(url, token);

            }
            catch (CustomException exception)
            {
                throw new CustomException(exception.FailureType, exception.Message);
            }
        }
    }
}
