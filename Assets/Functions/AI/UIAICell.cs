using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAICell : MonoBehaviour
{

    public TMPro.TMP_Text nameText;

    public Image talkFlag;

    // Start is called before the first frame update
    void Start()
    {
        var toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnClickToggle);
        talkFlag.gameObject.SetActive(false);
    }


    public string AIName
    {
        get
        {
            return gameObject.name;
        }
        set
        {
            gameObject.name = value;
            nameText.text = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private Coroutine MoveWatchControlCoroutine;
    //IEnumerator MoveWatchControl()
    //{
    //    var duration = 0.2f;
    //    var time = 0f;
    //    var camera = WatchControl.Instance.transform;
    //    var from = camera.position;
    //    var toTarget = AIManager.Instance.GetAI(gameObject.name).transform;
    //    var to = toTarget.position;
    //    while (time<duration)
    //    {
    //        //talkFlag.gameObject.SetActive(!talkFlag.gameObject.activeSelf);
    //        camera.position = Vector3.Lerp(from,to,time/duration);
    //        time += Time.deltaTime;
    //        yield return null;
    //    }

    //    WatchControl.Instance.follow = toTarget;
    //    MoveWatchControlCoroutine = null;

    //    DetailControl.Show(gameObject.name);

    //}

    public void OnClickToggle(bool  value)
    {
        if (value)
        {
            AIControl ai = AIManager.Instance.GetAI(gameObject.name);
            ISRTSCamera.FollowForMain(ai.transform);
            DetailControl.Show(gameObject.name);
        }

        //if (MoveWatchControlCoroutine != null)
        //{
        //    StopCoroutine(MoveWatchControlCoroutine);
        //    MoveWatchControlCoroutine = null;
        //}
        //if (value)
        //{
        //    MoveWatchControlCoroutine = StartCoroutine(MoveWatchControl());
        //}
        //else
        //{
        //    StopCoroutine(MoveWatchControl());
        //    WatchControl.Instance.follow = null;
        //}
    }

    IEnumerator flashFlag()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            talkFlag.gameObject.SetActive(!talkFlag.gameObject.activeSelf);
            yield return new WaitForSeconds(0.3f);
        }
    }

    private Coroutine flashFlagCoroutine;
    public void ShowFlag()
    {
        if (flashFlagCoroutine != null)
        {
            StopCoroutine(flashFlagCoroutine);
            flashFlagCoroutine = null;
        }
        flashFlagCoroutine = StartCoroutine(flashFlag());
    }

    public void HideFlag()
    {
        StopCoroutine(flashFlagCoroutine);
        flashFlagCoroutine = null;
        talkFlag.gameObject.SetActive(false);
    }
}
