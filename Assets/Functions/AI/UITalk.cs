using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITalk : MonoBehaviour
{

    public static UITalk Instance { get; private set; }


    public TMPro.TMP_Text talkText;


    private void Awake()
    {
        Instance = this;
        Hide();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Show( string  name , string content)
    {
        Instance.gameObject.SetActive(true);
        Instance.talkText.text = content;

        Instance.transform.position =  UIAIList.Instance.GetCell(name).transform.position;

    }

    public static void Hide()
    {
        Instance.gameObject.SetActive(false);
    }

}
