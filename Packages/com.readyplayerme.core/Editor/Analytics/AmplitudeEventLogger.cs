using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Core.Analytics
{
    public class AmplitudeEventLogger
    {
        private const string ENDPOINT = "https://analytics-sdk.readyplayer.me/";
        private const string NO_INTERNET_CONNECTION = "No internet connection.";

        private readonly AppData appData;

        private long sessionId;

        public AmplitudeEventLogger()
        {
            appData = ApplicationData.GetData();
        }

        private bool HasInternetConnection => Application.internetReachability != NetworkReachability.NotReachable;

        public void SetSessionId(long id)
        {
            sessionId = id;
        }

        public bool IsSessionIdSet()
        {
            return sessionId != 0;
        }

        public void SetUserProperties()
        {
            var userProperties = new Dictionary<string, object>
            {
                { Constants.Properties.ENGINE_VERSION, appData.UnityVersion },
                { Constants.Properties.RENDER_PIPELINE, appData.RenderPipeline },
                { Constants.Properties.SUBDOMAIN, appData.PartnerName },
                { Constants.Properties.APP_NAME, PlayerSettings.productName },
                { Constants.Properties.SDK_TARGET, SDK_TARGET },
                { Constants.Properties.APP_IDENTIFIER, Application.identifier },
                { Constants.Properties.ALLOW_ANALYTICS, true }
            };

            Dictionary<string, string> modules = ModuleList.GetInstalledModuleVersionDictionary();

            foreach (KeyValuePair<string, string> module in modules)
            {
                userProperties.Add(module.Key, module.Value);
            }

            LogEvent(Constants.EventName.SET_USER_PROPERTIES, null, userProperties);
        }

        public async void LogEvent(string eventName, Dictionary<string, object> eventProperties = null, Dictionary<string, object> userProperties = null)
        {
            var eventData = new Dictionary<string, object>
            {
                { Constants.AmplitudeKeys.DEVICE_ID, SystemInfo.deviceUniqueIdentifier },
                { Constants.AmplitudeKeys.EVENT_TYPE, eventName },
                { Constants.AmplitudeKeys.PLATFORM, appData.UnityPlatform },
                { Constants.AmplitudeKeys.SESSION_ID, sessionId },
                { Constants.AmplitudeKeys.APP_VERSION, appData.SDKVersion },
                { Constants.AmplitudeKeys.OPERATING_SYSTEM, SystemInfo.operatingSystem }
            };

            if (userProperties != null)
            {
                eventData.Add(Constants.AmplitudeKeys.USER_PROPERTIES, userProperties);
            }

            if (eventProperties != null)
            {
                eventData.Add(Constants.AmplitudeKeys.EVENT_PROPERTIES, eventProperties);
            }

            var payload = new
            {
                target = GetAnalyticsTarget(),
                events = new[]
                {
                    eventData
                }
            };

            var json = JsonConvert.SerializeObject(payload);
            var bytes = Encoding.UTF8.GetBytes(json);

            try
            {
                await Dispatch(ENDPOINT, json);
            }
            catch (Exception exception)
            {
                SDKLogger.Log(nameof(AmplitudeEventLogger), exception);
            }
        }

        private async Task Dispatch(string url, string payload)
        {
            if (!HasInternetConnection)
            {
                throw new Exception(NO_INTERNET_CONNECTION);
            }

            var headers = new Dictionary<string, string>()
            {
                { "Content-Type", "application/json" }
            };

            var webRequestDispatcher = new WebRequestDispatcher();
            var response = await webRequestDispatcher.SendRequest<Response>(url, HttpMethod.POST, headers, payload);

            if (!response.IsSuccess)
            {
                throw new Exception(response.Error);
            }
        }

        #region Analytics Target

        private enum Target
        {
            Production,
            Development
        }

        private const string PRODUCTION = "unity";
        private const string DEVELOPMENT = "unity-dev";
        private const string SDK_TARGET = "Unity";

        private string GetAnalyticsTarget()
        {
            switch (GetTarget())
            {
                case Target.Development:
                    return DEVELOPMENT;
                case Target.Production:
                    return PRODUCTION;
                default:
                    return string.Empty;
            }
        }

        private Target GetTarget()
        {
            var path = AssetDatabase.FindAssets($"t:Script {nameof(AmplitudeEventLogger)}");
            var directoryPath = AssetDatabase.GUIDToAssetPath(path[0]);
            return directoryPath.Contains("com.readyplayerme.core") ? Target.Production : Target.Development;
        }

        #endregion
    }
}
