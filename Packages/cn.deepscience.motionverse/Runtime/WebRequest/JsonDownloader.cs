using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MotionverseSDK
{
    public class JsonDownloader
    {
        private const string TAG = nameof(JsonDownloader);
        public int Timeout { get; set; }

        public async Task<string> Execute(string url, CancellationToken token)
        {
            try
            {
                var webRequestDispatcher = new WebRequestDispatcher();
                string text = await webRequestDispatcher.Dispatch(url, token);
                return text;
            }
            catch (CustomException exception)
            {
                Debug.Log("stringœ¬‘ÿ¥ÌŒÛ");
                return null;
            }
        }

        public async Task<byte[]> ExecuteBuffer(string url, CancellationToken token)
        {
            try
            {
                var webRequestDispatcher = new WebRequestDispatcher();
                byte[] data = await webRequestDispatcher.DownloadBuffer(url, token);
                return data;
            }
            catch (CustomException exception)
            {
                Debug.Log("byteœ¬‘ÿ¥ÌŒÛ");
                return null;
            }
        }
        
    }
}