using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAIList : MonoBehaviour
{

    public static UIAIList Instance { get; private set; }


    private int index = 0;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAI(string name)
    {
        UIAICell cell = null;
        if (index < transform.childCount)
        {
            cell = transform.GetChild(index).GetComponent<UIAICell>();
            cell.gameObject.SetActive(true);
        }
        else
        {
            cell = Instantiate(transform.GetChild(0).gameObject,transform,false).GetComponent<UIAICell>();
        }

        cell.AIName = name;
        ++index;
    }

    public UIAICell GetCell(string name)
    {
        return transform.Find(name).GetComponent<UIAICell>();
    }

    public void Selected(string name)
    {
        transform.Find(name).GetComponent<Toggle>().isOn = true;
    }

    public void Cancel()
    {
        Toggle[]  toggles = GetComponentsInChildren<Toggle>();
        for(var i = 0;i < toggles.Length; ++i)
        {
            toggles[i].isOn = false;
        }
    }

}
