using MotionverseSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : MonoBehaviour
{

    private bool hasInit = false;

    private Animator animator;

    private Player player;

    private AudioSource audioSource;

    public bool isTalking
    {
        get => talkCount != 0;
    }

    private int talkCount = 0;

    private AIHeadUI headUI;

    [HideInInspector]
    public string action;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
        player = gameObject.GetComponent<Player>();
        //player.boneMap = gameObject.GetComponent<BoneMap>();
        //player.voicespeed = 50;
        //player.voiceFM = 50;
        //player.voiceVolume = 50;
        //player.bodyMotion = 1;
        //player.voiceId = "";

    }


    // Start is called before the first frame update
    void Start()
    {

        headUI = Instantiate(Resources.Load<GameObject>("AI_Head_UI"),transform,false).GetComponent<AIHeadUI>();
        headUI.transform.localPosition = new Vector3(0, 2.45f, 0);
        headUI.nameText.text = gameObject.name;

        player.OnPlayComplete += OnPlayComplete;
        player.OnPlayError += OnPlayError;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        hasInit = false;
    }

    private Vector3 from;
    private Vector3 to;

    public bool BeginMove(Vector3 to)
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                //animator.transform.localScale = new Vector3(3, 3, 3);

                animator.SetBool("IsMove", false);
                var t = animator.gameObject.AddComponent<Footstep>();
                t.OnAction = OnFootstep;
            }
        }

        if (!hasInit)
        {
            hasInit = true;
            transform.position = to;
            return false;
        }

        this.from = transform.position;
        this.to = to;
        
        
        var dis = Vector3.Distance(from, to);
        if (dis < 0.1f)
        {
            if(animator!=null) animator.SetBool("IsMove", false);
            transform.position = to;
            return false;
        }
        else
        {
            transform.forward = to - from;
            if (animator != null) animator.SetBool("IsMove", true);
            return true;
        }
        
    }

    public void Move(float progress,float speed)
    {
        transform.position = Vector3.Lerp(from, to, progress);
        if (animator != null) animator.SetFloat("MotionSpeed", speed);
    }

    

    public void StopMove()
    {
        animator.SetBool("IsMove", false);
    }

    //private void SplitChars(string text, char[] chars, List<string> outResult)
    //{
    //    int index = 0;
    //    while(index< text.Length)
    //    {
    //        var fIndex = text.IndexOfAny(chars, index);
    //        if (fIndex == -1)
    //        {
    //            outResult.Add(text.Substring(index, text.Length - index));
    //            break;
    //        }
    //        else
    //        {
    //            ++fIndex;
    //            outResult.Add(text.Substring(index, fIndex - index));
    //            index = fIndex;
    //        }
    //    }

    //}

    //private List<string>  Split(string text)
    //{
    //    List<string> result = new List<string>();

    //    string[] t1s = text.Split("...");

    //    char[] chars = new char[] { '.','?','!'};

    //    for (var i = 0; i < t1s.Length; ++i)
    //    {
    //        var t1 = t1s[i];

    //        SplitChars(t1,chars,result);

    //        if(i < t1s.Length - 1)
    //        {
    //            result[result.Count - 1] +=  "...";
    //        }

    //    }

    //    return result;
    //}


    public void Talk(string text)
    {
        //var texts = Split(text);
        talkCount = 1;

        //for (var i = 0;i < texts.Count; ++i)
        //{

            DriveTask driveTask = new DriveTask();
            driveTask.player = player;
            driveTask.text = text;
            TextDrive.GetDrive(driveTask);

        //}




        UIAIList.Instance.GetCell(gameObject.name).ShowFlag();
         headUI.ShowTalk(text);
        //StartCoroutine(WaitFinish());
        

    }

    private void OnPlayComplete()
    {
        talkCount--;
        if (talkCount <= 0)
        {
            UIAIList.Instance.GetCell(gameObject.name).HideFlag();
            headUI.HideTalk();
        }
    }

    private void OnPlayError(string error)
    {
        OnPlayComplete();
    }

    //private IEnumerator WaitFinish()
    //{
    //    while (!audioSource.isPlaying)
    //    {
    //        yield return null;
    //    }
    //    while (audioSource.isPlaying)
    //    {
    //        yield return null;
    //    }
    //    yield return new WaitForSeconds(1);
    //    UIAIList.Instance.GetCell(gameObject.name).HideFlag();
    //    headUI.HideTalk();
    //    isTalking = false;
    //}

    public void ShowEmo(string emo)
    {
        if (headUI != null) headUI.ShowEmo(emo);
    }
    private void OnFootstep()
    {

    }

    void OnMouseUp()
    {
        UIAIList.Instance.Selected(gameObject.name);
    }
}
