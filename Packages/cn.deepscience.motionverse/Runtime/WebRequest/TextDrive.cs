using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace MotionverseSDK
{
    public class TextDrive
    {
        private const string TAG = nameof(TextDrive);

        private static Dictionary<string, Drive> m_cacheDrive = new Dictionary<string, Drive>();//缓存

        private static List<DriveTask> m_waitTask = new List<DriveTask>();//等待列表
        private static List<DriveTask> m_curTask = new List<DriveTask>();//当前正在加载列表


        public static void GetDrive(DriveTask driveTask,bool isFirst = false)
        {
            CastTask(driveTask, isFirst);
        }
        private static async void CastTask(DriveTask driveTask = null,bool isFirst = false)
        {
            if (driveTask == null)
            {
                if (m_waitTask.Count == 0)
                {
                    return;//没有等待下载的任务
                }
                driveTask = m_waitTask[0];
                m_waitTask.RemoveAt(0);
            }
            if (m_curTask.Count > 0)
            {
                if (isFirst)
                {
                    m_waitTask.Insert(0, driveTask);
                }
                else
                {
                    m_waitTask.Add(driveTask);
                }
               
            }
            else
            {
                m_curTask.Add(driveTask);
                WebRequestDispatcher webRequestDispatcher = new WebRequestDispatcher();
                TextMotionParams postData = new TextMotionParams()
                {
                    draft_content = driveTask.text,
                };
                postData.body_config.body_motion = driveTask.player.bodyMotion;
                postData.body_config.body_head_x_rot= driveTask.player.bodyHeadXRot;
                postData.body_config.body_compress = true;
                if (driveTask.player.voiceId.Length > 0)
                {
                    postData.tts_config.tts_voice_name = driveTask.player.voiceId;
                }
                postData.tts_config.tts_volume = driveTask.player.voiceVolume;
                postData.tts_config.tts_speed = driveTask.player.voicespeed;
                postData.tts_config.tts_fm = driveTask.player.voiceFM;


                SDKLogger.Log(TAG, JsonUtility.ToJson(postData));
                WebResponse response = await webRequestDispatcher.Post(TokenClient.Host + Config.TextMotionUrl, JsonUtility.ToJson(postData), new CancellationTokenSource().Token);
                SDKLogger.Log(TAG, TokenClient.Host + Config.TextMotionUrl);
      
                if (response.code == 200)
                {
                    JObject obj = JObject.Parse(response.data);
                    Debug.Log(response.data);
                    if (int.Parse(obj["code"].ToString()) == 0)
                    {
                        string audioUrl = obj["data"]["ttsSynthesizeData"]["audio_url"].ToString();
                        string faceUrl = obj["data"]["allfaceData"]["oss_url"].ToString();
                        string motionUrl = obj["data"]["motionFusionedData"]["oss_url"].ToString();
             
                        if (!m_cacheDrive.TryGetValue(audioUrl,out Drive drive)){
                            m_cacheDrive.Add(audioUrl, new Drive());
                        }

                        AudioDownLoad(audioUrl,driveTask.player);
                        FaceDownLoad(faceUrl, audioUrl, driveTask.player);
                        MotionDownLoad(motionUrl, audioUrl, driveTask.player);
                      
                    }
                }
                else
                {
                    m_curTask.Clear();
                    CastTask();
                    driveTask.player.OnPlayError?.Invoke(response.data);
                }
            }

        }

        private static async void AudioDownLoad(string audioUrl,Player player)
        {
            AudioDownloader audioDownloader = new AudioDownloader();
            CancellationToken token = new CancellationToken();
            AudioClip clip = await audioDownloader.Execute(audioUrl, AudioType.WAV, token);
            if (clip == null)
            {
                m_curTask.Clear();
                CastTask();
                player.OnPlayError?.Invoke("Audio DownLoad Error");
                return;
            }

            if (m_cacheDrive.TryGetValue(audioUrl, out Drive drive))
            {
                drive.clip= clip;
                drive.step++;
                SDKLogger.Log(TAG, "音频下载成功");

                if(drive.step == 3)
                {
                    m_curTask.Clear();
                    CastTask();
                    player.StartDrive(drive);
                }
            }

           

        }

        private static async void FaceDownLoad(string faceUrl,string key, Player player)
        {
            JsonDownloader jsonDownloader = new JsonDownloader();
            CancellationToken token = new CancellationToken();
            string data = await jsonDownloader.Execute(faceUrl, token);
            if (data == null)
            {
                m_curTask.Clear();
                CastTask();
                player.OnPlayError?.Invoke("Face DownLoad Error");
                return;
            }
            if (m_cacheDrive.TryGetValue(key, out Drive drive))
            {
                drive.bsData= data;
                drive.step++;
                SDKLogger.Log(TAG, "表情数据下载成功");
                if (drive.step == 3)
                {
                    m_curTask.Clear();
                    CastTask();
                    player.StartDrive(drive);
                }
            }
        }

        private static async void MotionDownLoad(string motionUrl, string key, Player player)
        {
            JsonDownloader jsonDownloader = new JsonDownloader();
            CancellationToken token = new CancellationToken();
            byte[] data = await jsonDownloader.ExecuteBuffer(motionUrl, token);
            if (data == null)
            {
                m_curTask.Clear();
                CastTask();
                player.OnPlayError?.Invoke("Motion DownLoad Error");
                return;
            }

            if (m_cacheDrive.TryGetValue(key, out Drive drive))
            {
                drive.motionByte = data;
                drive.step++;
                SDKLogger.Log(TAG, "动作数据下载成功"+ motionUrl);
                if(drive.step == 3)
                {
                    m_curTask.Clear();
                    CastTask();
                    player.StartDrive(drive);
                }
            }
        }

    }
}
