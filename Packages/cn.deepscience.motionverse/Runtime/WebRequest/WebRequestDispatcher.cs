using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;
namespace MotionverseSDK
{
    public static class WebRequestDispatcherConstants
    {
        public const int TIMEOUT = 50000;
        public const string NO_INTERNET_CONNECTION = "没有internet连接。";
    }
    public class WebRequestDispatcher
    {
        private static bool HasInternetConnection => Application.internetReachability != NetworkReachability.NotReachable;

        public async Task<WebResponse> Post(string url, string postData, CancellationToken token, int timeout = WebRequestDispatcherConstants.TIMEOUT, IProgress<float> progress = null)
        {
            var response = new WebResponse();

            if (HasInternetConnection)
            {
                try
                {
                    using var request = new UnityWebRequest(url);
                    request.timeout = timeout;
                    byte[] jsonToSend = new UTF8Encoding().GetBytes(postData);
                    request.uploadHandler = new UploadHandlerRaw(jsonToSend);

                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.method = UnityWebRequest.kHttpVerbPOST;
                    request.SetRequestHeader("Content-Type", "application/json");
                    if (TokenClient.token == null)
                    {
                        response.code = 300;
                        response.msg = "Token 为空";
                        return response;
                    }
                    request.SetRequestHeader("access_token", TokenClient.token);
                    var asyncOperation = request.SendWebRequest();
                    while (!asyncOperation.isDone && !token.IsCancellationRequested)
                    {
                        await Task.Yield();
                    }


                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        response.data = request.downloadHandler.text;
                        return response;
                    }

                    response.code = 300;
                    response.msg = "接口错误";
                    response.data = request.downloadHandler.text;
                    return response;
                }
                catch (Exception)
                {

                    response.code = 300;
                    response.msg = "接口错误";
                    return response;
                }
               

            }

            response.code = 400;
            response.msg = WebRequestDispatcherConstants.NO_INTERNET_CONNECTION;
            return response;
        }
        public async Task<byte[]> DownloadBuffer(string url, CancellationToken token, int timeout = WebRequestDispatcherConstants.TIMEOUT, IProgress<float> progress = null)
        {
            if (HasInternetConnection)
            {
                using (var request = new UnityWebRequest(url))
                {
                    try
                    {
                        request.timeout = timeout;
                        request.downloadHandler = new DownloadHandlerBuffer();
                        request.method = UnityWebRequest.kHttpVerbGET;

                        var asyncOperation = request.SendWebRequest();
                        while (!asyncOperation.isDone && !token.IsCancellationRequested)
                        {
                            await Task.Yield();
                        }

                        return request.downloadHandler.data;
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
            }

            throw new CustomException(FailureType.NoInternetConnection, WebRequestDispatcherConstants.NO_INTERNET_CONNECTION);
        }

        public async Task<string> Dispatch(string url, CancellationToken token, int timeout = WebRequestDispatcherConstants.TIMEOUT, IProgress<float> progress = null)
        {
            if (HasInternetConnection)
            {
                using (var request = new UnityWebRequest(url))
                {
                    try
                    {
                        request.timeout = timeout;
                        request.downloadHandler = new DownloadHandlerBuffer();
                        request.method = UnityWebRequest.kHttpVerbGET;

                        var asyncOperation = request.SendWebRequest();
                        while (!asyncOperation.isDone && !token.IsCancellationRequested)
                        {
                            await Task.Yield();
                        }

                        return request.downloadHandler.text;
                    }
                    catch (Exception e)
                    {
                        return null;
                    }

                }
            }

            throw new CustomException(FailureType.NoInternetConnection, WebRequestDispatcherConstants.NO_INTERNET_CONNECTION);
        }

        public async Task<AudioClip> DownloadAudioClip(string url, AudioType audioType, CancellationToken token, IProgress<float> progress = null)
        {
            if (HasInternetConnection)
            {
                using (var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
                {
                    try
                    {
                        var asyncOperation = webRequest.SendWebRequest();
                        while (!asyncOperation.isDone && !token.IsCancellationRequested)
                        {
                            await Task.Yield();
                            progress?.Report(webRequest.downloadProgress);
                        }
                        if (webRequest.result != UnityWebRequest.Result.Success)
                        {
                            throw new CustomException(FailureType.DownloadError, webRequest.error);
                        }
                        if (webRequest.downloadHandler is DownloadHandlerAudioClip downloadAudioClip)
                        {
                            return downloadAudioClip.audioClip;
                        }
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
            }
            throw new CustomException(FailureType.NoInternetConnection, WebRequestDispatcherConstants.NO_INTERNET_CONNECTION);
        }
    }
}