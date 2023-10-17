using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
namespace MotionverseSDK 
{
    public class AudioDownloader
    {
        private const string TAG = nameof(AudioDownloader);
        public int Timeout { get; set; }

        public async Task<AudioClip> Execute(string url, AudioType audioType, CancellationToken token)
        {
            try
            {
                var webRequestDispatcher = new WebRequestDispatcher();
                AudioClip audioClip = await webRequestDispatcher.DownloadAudioClip(url, audioType, token);
                return audioClip;
            }
            catch (CustomException exception)
            {
                return null;
            }
        }
    }
}