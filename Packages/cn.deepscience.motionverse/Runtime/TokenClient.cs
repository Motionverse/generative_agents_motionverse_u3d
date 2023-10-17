using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MotionverseSDK
{
    public class TokenClient : MonoBehaviour
    {
        public static string Host;
        public Region region;
        [SerializeField]
        private string AppID;
        [SerializeField]
        private string SecretKey;
        private const string TAG = nameof(TokenClient);
        CancellationToken cancellationToken = new CancellationToken();
        public static string token;
        async void Start()
        {
            if(region == Region.ZH_CN)
            {
                Host = "https://motionverseapi.deepscience.cn";
            }
            else
            {
                Host = "https://api.motionverse.ai";
            }
            token = await Execute(AppID, SecretKey);
            Debug.Log(token);
            Debug.Assert(token != null, "应用不存在");
        }
        public async Task<string> Execute(string AppID,string SecretKey)
        {
            try
            {
                Dictionary<string, string> getParameDic = new Dictionary<string, string>();
                long timestamp = Utilities.GetTimeStampSecond();
                getParameDic.Add("appid", AppID);
                getParameDic.Add("secret", SecretKey);
                getParameDic.Add("timestamp", timestamp.ToString());
                getParameDic.Add("sign", Utilities.SHA1(AppID + timestamp + SecretKey));

                StringBuilder stringBuilder = new StringBuilder();
                bool isFirst = true;
                foreach (var item in getParameDic)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        stringBuilder.Append('?');
                    }
                    else
                        stringBuilder.Append('&');

                    stringBuilder.Append(item.Key);
                    stringBuilder.Append('=');
                    stringBuilder.Append(item.Value);
                }
                var webRequestDispatcher = new WebRequestDispatcher();
                string text = await webRequestDispatcher.Dispatch(Host+Config.GetTokenUrl+stringBuilder, cancellationToken);

                JObject obj = JObject.Parse(text);
                if (int.Parse(obj["code"].ToString()) == 0)
                {
                    return obj["data"]["access_token"].ToString();
                }
                return null;
               
            }
            catch (CustomException exception)
            {
                SDKLogger.Log(TAG, exception + "重试");
                await Task.Delay(1000);
                return await Execute(AppID, SecretKey);
            }
        }
    }
}
