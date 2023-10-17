using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MotionverseSDK
{
    public class Player : MonoBehaviour
    {
        private const string TAG = nameof(Player);
        private AudioSource audioSource;
        FaceAnimationHandler faceAnimationHandler;
        PoserAnimationHandler poserAnimationHandler;
        private static List<Drive> m_waitTask = new List<Drive>();//等待列表

        public string voiceId = null;
        public int bodyMotion = 1;
        public int voicespeed = 0;
        public int voiceVolume = 0;
        public int voiceFM = 0;
        public int bodyHeadXRot = 0;

        [HideInInspector]
        public HandFinger srcLeft;
        [HideInInspector]
        public HandFinger srcRight;
        [HideInInspector]
        public StandardSkeleton src;
        bool checkInited = false;

        public Vector3 leftHandOffest = Vector3.zero;
        public Vector3 rightHandOffest = Vector3.zero;
        public Quaternion headRotationOffest = Quaternion.identity;

        public Dictionary<RetargetBoneType, Transform> target = new Dictionary<RetargetBoneType, Transform>();
        public Dictionary<Skeleton, Transform> srcBone = new Dictionary<Skeleton, Transform>();

        string avatarlId;
        public BoneMap boneMap;
        private Transform skeletonTransform;

        public event Action OnPlayComplete;
        public event Action OnPlayStart;
        public  Action<string> OnPlayError;

        public AvatarStatus avatarStatus;
        void Start()
        {
            avatarlId = Utilities.EncryptWithMD5(transform.name + Utilities.GetTimeStampSecond());

            audioSource = GetComponent<AudioSource>();
           

            foreach (KeyValuePair<RetargetBoneType, string> pair in boneMap.boneNames)
            {
                if(pair.Key == RetargetBoneType.Root)
                {
                    Transform bone = Utilities.FindChildRecursively(transform, boneMap.boneNames[RetargetBoneType.Hip]);
                    target.Add(RetargetBoneType.Root, bone.parent);
                }
                else
                {
                    Transform bone = Utilities.FindChildRecursively(transform, pair.Value);
                    target.Add(pair.Key, bone);
                }
                
            }

            Dictionary<RetargetBoneType, Joint_t> tar_pose = new Dictionary<RetargetBoneType, Joint_t>();
            foreach (KeyValuePair<RetargetBoneType, Transform> pair in target)
            {
                Joint_t info = EngineInterface.Create(pair.Value.position, pair.Value.rotation);
                tar_pose.Add(pair.Key, info);
            }

            src = new StandardSkeleton();
            GameObject modle = Resources.Load<GameObject>("Skeleton");
            // 在场景中生成物体，生成后场景就会出现该物体实例
            GameObject modleGo = Instantiate<GameObject>(modle);
            modleGo.transform.SetParent(transform,false);
            modleGo.transform.localPosition = Vector3.zero;
            skeletonTransform = modleGo.transform;
            foreach (Skeleton bone in Enum.GetValues(typeof(Skeleton)))
            {
                srcBone.Add(bone, Utilities.FindChildRecursively(modleGo.transform, bone.ToString()));
            }


            src.CloneHuman(modleGo.transform.Find("Root"));
            srcLeft = new HandFinger(src.m_transformLeftHand, true);
            srcRight = new HandFinger(src.m_transformRightHand, false);
            StandardMotion src_motion = GetFromSkeleton(src);
          

            checkInited = CompatibleRetargetStandard.RetargetInit(avatarlId, src_motion, tar_pose);
            Debug.Log(checkInited);

            modleGo.GetComponent<Animator>().enabled = checkInited;

            faceAnimationHandler = GetComponentInChildren<FaceAnimationHandler>();
            poserAnimationHandler = GetComponentInChildren<PoserAnimationHandler>();
        }
        public void StartDrive(Drive drive)
        {
            m_waitTask.Add(drive);
            DataReady();
        }
       private void DataReady()
        {
            if (!audioSource.isPlaying)
            {
                OnPlayStart?.Invoke();
                skeletonTransform.GetComponent<Animator>().applyRootMotion = false;
                Drive drive = m_waitTask[0];
                m_waitTask.RemoveAt(0);
                audioSource.clip = drive.clip;
                faceAnimationHandler?.SetAnimationData(drive.bsData);
                poserAnimationHandler?.SetAnimationDataFromByte(drive.motionByte);
                avatarStatus = AvatarStatus.Talk;
                audioSource.Play();
            }
        }

        public void OnIdle()
        {
            faceAnimationHandler?.Resume();
        }

        void Update()
        {
           

        }

        public StandardMotion GetFromSkeleton(StandardSkeleton skeleton)
        {
            StandardMotion frame = StandardMotion.GetMotionFromStandardSkeleton(skeleton);
            frame.hasFingers = true;

            for (int i = 0; i < Config.Finger_Names.Length; ++i)
            {
                string name = Config.Finger_Names[i];
                Transform t = Utilities.FindChildRecursively(skeleton.m_root, name);
                if (t != null)
                {
                    if (i < 20)
                    {
                        frame.leftFingersPositions[i] = t.position;
                        frame.leftFingersRotations[i] = t.rotation;
                    }
                    else
                    {
                        frame.rightFingersPositions[i - 20] = t.position;
                        frame.rightFingersRotations[i - 20] = t.rotation;
                    }
                }
                else
                {
                    Debug.LogError(name + " not found");
                }
            }
            return frame;
        }

        public void OnPlayOver()
        {
            OnIdle();
            poserAnimationHandler?.PlayOver();
            StartCoroutine(Blend());
            OnPlayComplete?.Invoke();
        }

        IEnumerator Blend()
        {
            isBlend = true;
            yield return new WaitForSeconds(1.5f);
            isBlend = false;
        }
        [HideInInspector]
        public bool isBlend = false;
        public bool getBlend()
        {
            return isBlend;
        }
        public void StopPlay()
        {
            avatarStatus = AvatarStatus.Idle;
        }

        private bool isPlaying = false;
        private void LateUpdate()
        {
            if (audioSource.clip != null && audioSource.time > 0 && avatarStatus == AvatarStatus.Talk)
            {
                isPlaying = true;
                faceAnimationHandler?.PlayBS(audioSource.time);
                poserAnimationHandler?.PlayMotion(audioSource.time);
            }
            else if (audioSource.clip != null && isPlaying)
            {
                audioSource.clip = null;
                isPlaying = false;
             
                if (m_waitTask.Count > 0 && avatarStatus == AvatarStatus.Talk)
                {
                    DataReady();
                }
                else
                {
                    avatarStatus = AvatarStatus.Idle;
                    OnPlayOver();
                }

            }



            if (checkInited)
            {
                CompatibleRetargetStandard.SetHandOffset(avatarlId, leftHandOffest, rightHandOffest);
                CompatibleRetargetStandard.SetHeadRotationOffset(avatarlId, headRotationOffest);
                StandardMotion src_motion = GetFromSkeleton(src);

                Dictionary<RetargetBoneType, Joint_t> tar_action = CompatibleRetargetStandard.UpdateRetarget(avatarlId, src_motion);

                foreach (RetargetBoneType bone in Enum.GetValues(typeof(RetargetBoneType)))
                {
                    if (tar_action.TryGetValue(bone, out Joint_t joint))
                    {
                        target[bone].rotation = new Quaternion(joint.qx, joint.qy, joint.qz, joint.qw);
                        if(bone == RetargetBoneType.Hip)
                        {
                            target[bone].position = new Vector3(joint.px, joint.py, joint.pz);
                        }
                    }


                }

            }
        }
        private void OnDestroy()
        {
            CompatibleRetargetStandard.Close(avatarlId);
        }

    }

}

