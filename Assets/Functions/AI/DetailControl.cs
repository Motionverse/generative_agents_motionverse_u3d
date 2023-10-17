using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DetailControl : MonoBehaviour,IPointerDownHandler
{

    private static DetailControl instance;

    public RectTransform content;

    public Transform[] tabs;


    public TMPro.TMP_Text tab1NameText;
    public TMPro.TMP_Text tab1AgeText;
    public TMPro.TMP_Text tab1SexText;
    public TMPro.TMP_Text tab1StatusText;

    public Transform tab2Content;

    public Transform tab3Content;


    private void Awake()
    {
        instance = this;
        for (var i = 0; i < tabs.Length; ++i)
        {
            tabs[i].transform.gameObject.SetActive(0 == i);
        }
        gameObject.SetActive(false);

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private static string TrimAction(string action)
    {
        var index = action.IndexOf("@");
        if (index>=0)
        {
            return action.Substring(0, index);
        }
        return action;
    }

    public void OnToggleValueChanged(Toggle toggle)
    {
        //text.color = value ? selectedColor : unselectedColor;

        if (toggle.isOn)
        {
            for(var i = 0;i < tabs.Length; ++i)
            {
                var isOn = toggle.transform.GetSiblingIndex() == i;
                if (isOn )
                {
                    if (i == 0)
                    {
                        content.sizeDelta = new Vector2(0,300);
                    }
                    else
                    {
                        content.sizeDelta = new Vector2(0, 540);
                    }
                }

                tabs[i].gameObject.SetActive(isOn);
            }
        }
    }


    public void UpdatePerson(string aiName)
    {
        

        tab1NameText.text = aiName;

        var am = AIManager.Instance;
        //AIManager.Instance.index;
        tab1StatusText.text = TrimAction( am.GetAction(aiName));

        var file = am.dataPath + string.Format("/personas/{0}/bootstrap_memory/scratch.json", aiName);
        var text = File.Exists(file) ? File.ReadAllText(file) : ""; //Resources.Load<TextAsset>(string.Format("data/movement/{0}.txt",index));

        //var text = Resources.Load<TextAsset>(string.Format("data/personas/{0}/bootstrap_memory/scratch", aiName));
        if (text!=null && text.Length>0 )
        {
            JsonData jsonObject = JsonMapper.ToObject(text);

            tab1AgeText.text = "Age: " + ((int)(jsonObject["age"]));

            tab1SexText.text = "";// "Gender: Male";


            List<ItemData> datas = new List<ItemData>();
            datas.Add(new ItemData("Innate tendency", (string)(jsonObject["innate"])));
            datas.Add(new ItemData("Learned tendency", (string)(jsonObject["learned"])));
            datas.Add(new ItemData("Currently", (string)(jsonObject["currently"])));
            datas.Add(new ItemData("Lifestyle", (string)(jsonObject["lifestyle"])));
            UpdateContent(tab2Content,datas);

            datas.Clear();
            var daily_req = jsonObject["daily_req"];
            StringBuilder sb = new StringBuilder();
            for (var i = 0;i<daily_req.Count;++i)
            {
                if (i != 0) sb.Append("\n");
                sb.Append(i + 1);
                sb.Append(". ");
                sb.Append((string)(daily_req[i]));
            }

            datas.Add(new ItemData("Daily Requirement", sb.ToString()));


            sb.Clear();
            var daily_sch = jsonObject["f_daily_schedule_hourly_org"];
            for (var i = 0; i < daily_sch.Count; ++i)
            {
                if (i != 0) sb.Append("\n");
                sb.Append(i + 1);
                sb.Append(". ['");
                sb.Append((string)(daily_sch[i][0]));
                sb.Append("',");
                sb.Append((int)(daily_sch[i][1]));
                sb.Append("]");
            }

            datas.Add(new ItemData("Daily Schedule", sb.ToString()));
            UpdateContent(tab3Content, datas);

        }

    }

    private class ItemData
    {
        public string title;
        public string content;

        public ItemData(string title,string content)
        {
            this.title = title;
            this.content = content;
        }
    }

    private void UpdateContent(Transform content, List<ItemData> data)
    {

        for (var i = 0;i < data.Count;++i)
        {
            Transform cell = null;
            if (i < content.childCount)
            {
                cell = content.GetChild(i);
            }
            else
            {
                cell = Instantiate(content.GetChild(0).gameObject,content,false).transform;
            }
            cell.GetChild(0).GetComponent<TMPro.TMP_Text>().text = data[i].title;
            cell.GetChild(1).GetComponent<TMPro.TMP_Text>().text = data[i].content;
        }
    }

    public static void UpdateCurrent(string name,string action)
    {
        if (!instance.gameObject.activeSelf)
        {
            return;
        }

        if (instance.tab1NameText.text.Equals(name))
        {
            instance.tab1StatusText.text = TrimAction(action);
        }

    }

    public static void Show(string aiName)
    {
        instance.gameObject.SetActive(true);
        instance.UpdatePerson(aiName);
    }

    public static void Hide()
    {
        instance.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }

}
