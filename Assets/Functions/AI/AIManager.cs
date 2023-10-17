using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    public const int TileSize = 1;

    public const int TileCount = 140;

    public static AIManager Instance { get; private set; }

    public class Chat
    {
        public AIControl ai;
        public string content;
    }

    public class Chats
    {
        public List<Chat> chats;
        public int status = 0;
        public int lastFrameIndex = 0;

        public float Distance()
        {
            return Vector3.Distance(chats[0].ai.transform.position, chats[1].ai.transform.position);
            
        }

    }

    private List<Chats> chatss = new List<Chats>();

    [SerializeReference]
    public GameObject[] prefabs;

    private List<string> names = new List<string>();

    private List<AIControl> persons = new List<AIControl>();

    public int index = 0;

    public int total;

    public float speed = 1;

    public Transform ground;

    public ConfigGame config;

    public TMPro.TMP_Text timeText;

    [HideInInspector]
    public string dataPath;

    private bool HasTalking
    {

        get
        {
            for (var i = 0; i < chatss.Count; ++i)
            {
                var chats = chatss[i];
                if (chats.status==1) return true;
            }
            return false;
        }

    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        var s = TileSize * TileCount / 10.0f;
        ground.localScale = new Vector3(s, 1, s);

        var ss = TileSize * TileCount / 2;
        ground.localPosition = new Vector3(ss, ground.localPosition.y, ss);

        config.OnChose = StartChose;
        config.OnStart = StartGame;
        StartChose();

    }



    private void Choose()
    {
        var  path = Utils.ChooseDirectory("选择数据目录");

        //#if UNITY_EDITOR
        //        if (string.IsNullOrEmpty(path))
        //        {
        //            dataPath = Application.dataPath + "/Resources/data";
        //        }
        //#endif
        choosing = false;
        if (path == null || path.Length==0 || !File.Exists(path + "/movement/0.json"))
        {
            path = "";
            total = 0;
            return;
        }
        dataPath = path;
        total = Directory.GetFiles(dataPath + "/movement").Length;
        Debug.Log("chose:"+dataPath + ",total:"+total);
    }
    public bool choosing = false;
    private void StartChose()
    {
        if (choosing) return;
        choosing = true;
        dataPath = "";
        config.gameObject.SetActive(true);
        Thread thread = new Thread(Choose);
        thread.Start();
        
    }

    public void StartGame(float progress,float speed)
    {
        
        config.gameObject.SetActive(false);
        index = (int)( progress * total/100);
        this.speed = speed;
        StartCoroutine(UpdateFrame());
    }

    //private void OnChooseFinish(bool ret, string path)
    //{
    //    if (!ret)
    //    {
    //        Utils.ChooseDirectory("选择数据目录", OnChooseFinish);
    //    }
    //    else
    //    {
    //        dataPath = path;
    //        StartCoroutine(UpdateFrame());
    //    }
    //}


    public string GetAction(string name)
    {
        return GetAI(name).action;
    }

    // Update is called once per frame
    IEnumerator UpdateFrame()
    {

        List<AIControl> movings = new List<AIControl>();

        float duration = 10f;
        while (true)
        {

            if (HasTalking)//说话的时候，停止， 等待说话结束。
            {
                yield return null;
                continue;
            }
            var file = dataPath + string.Format("/movement/{0}.json", index);
            var text = File.Exists(file) ? File.ReadAllText(file) : ""; //Resources.Load<TextAsset>(string.Format("data/movement/{0}.txt",index));
            if(text==null  || text.Length == 0)
            {
                StartChose();
                index = 0;
                Reset();
                yield break;
            }

            JsonData jsonObject = JsonMapper.ToObject(text);

            JsonData jsonData = jsonObject["persona"];

            timeText.text = (string)(jsonObject["meta"]["curr_time"]);

            movings.Clear();
            var keys = jsonData.Keys;
            foreach (var name in keys)
            {
                var item = jsonData[name];
                var pos = item["movement"];
                Vector3 to = new Vector3((TileCount - (int)(pos[0])) * TileSize, ground.localPosition.y - 0.25f, ((int)(pos[1]) + 0.5f) * TileSize);
                var ai = GetAI(name);
                if (ai.BeginMove(to))
                {
                    movings.Add(ai);
                }

                ai.ShowEmo((string)(item["pronunciatio"]));
                ai.action = (string)(item["description"]);
                DetailControl.UpdateCurrent(name, ai.action);

                var chat = item["chat"];
                if (chat != null)
                {
                    AddChat(chat);
                }
            }
            
            var time = 0f;
            while (time<duration)
            {
                var p = time / duration;
                var sp = speed * TileSize/20;
                for(var i = 0;i < movings.Count;++i)
                {
                    movings[i].Move(p, sp);
                }
                time += Time.deltaTime*speed;
                yield return null;
            }
            movings.Clear();
            //yield return  new WaitForSeconds(duration/speed);
            HandleChat();

            ++index;
        }
    }

    private void Reset()
    {
        for (var i = 0; i < transform.childCount; ++i)
        {
            var ai = transform.GetChild(i).GetComponent<AIControl>();
            ai.Reset();
        }
    }

    private void HandleChat()
    {
        for (var i = chatss.Count - 1; i >= 0; --i)
        {
            var cts = chatss[i];
            if (cts.status == 0)
            {
                var maxDis = TileSize * 3.2f;
                var dis = cts.Distance();
                if (dis <= maxDis)
                {
                    //var minDis = TileSize * 3.5f;
                    //if (dis < minDis)
                    //{
                    //    Debug.Log("");
                    //}
                    cts.status = 1;
                    StartCoroutine(Talk(cts));
                }
            }
            if (cts.status == 2 && cts.lastFrameIndex < index)
            {
                chatss.RemoveAt(i);
            }
        }
    }

    //对话存在多帧重复的数据，对比后去重处理。
    public bool CompareChats(Chats chats,JsonData jsonData)
    {
        if (chats.chats.Count != jsonData.Count) return false;

        for (var i = 0; i < jsonData.Count; ++i)
        {
            var item = jsonData[i];
            var name = (string)(item[0]);
            var content = (string)(item[1]);
            var chat = chats.chats[i];
            if(!chat.ai.gameObject.name.Equals(name) || !chat.content.Equals(content))
            {
                return false;
            }
        }
        return true;
    }

    private Chats CreateChat(JsonData jsonData)
    {
        List<Chat> cs = new List<Chat>();
        for (var i = 0; i < jsonData.Count; ++i)
        {
            var item = jsonData[i];
            Chat ct = new Chat();
            ct.ai = GetAI((string)(item[0]));
            ct.content = (string)(item[1]);
            cs.Add(ct);
        }
        if (cs.Count == 0) return null;
        var cc = new Chats();
        cc.chats = cs;
        return cc;


    }
    private void AddChat(JsonData jsonData)
    {
        Chats cts = null;
        for (var i = chatss.Count - 1; i >= 0; --i)
        {
            cts = chatss[i];
            if (CompareChats(cts,jsonData))
            {
                cts.lastFrameIndex = index;
                return;
            }
        }
        cts = CreateChat(jsonData);
        cts.lastFrameIndex = index;
        chatss.Add(cts);
    }


    private AIControl createAI(string name)
    {
        names.Add(name);
        var prefab = prefabs[(names.Count-1)%prefabs.Length];
        var gameObject = Instantiate(prefab,transform,false);
        gameObject.name = name;
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        var ai = gameObject.AddComponent<AIControl>();
        persons.Add(ai);
        if (!isInit)
        {
            isInit = true;
            //WatchControl.Instance.follow = ai.transform;
        }
        UIAIList.Instance.AddAI(name);
        return ai;
    }

    private bool isInit=false;

    public AIControl GetAI(string name)
    {
        var index = names.IndexOf(name);
        if (index == -1)
        {
            return createAI(name);
        }
        return persons[index];
    }

    
    private IEnumerator Talk(Chats chats)
    {

        for (var i = 0; i < transform.childCount; ++i)
        {
            var ai = transform.GetChild(i).GetComponent<AIControl>();
            ai.StopMove();
        }

        Vector3 p = Vector3.zero;
        for (var i = 0; i < chats.chats.Count; ++i)
        {
            var chat = chats.chats[i];
            p += chat.ai.transform.position;
        }

        p = p / chats.chats.Count;

        for (var i = 0; i < chats.chats.Count; ++i)
        {
            var chat = chats.chats[i];
            chat.ai.transform.forward = p - chat.ai.transform.position;
        }


        for (var i = 0;i < chats.chats.Count;++i)
        {
            var chat = chats.chats[i];
            var ai = chat.ai;
            ai.Talk(chat.content);
            
            while (ai.isTalking)
            {
                yield return null;
            }

        }
        chats.status = 2;
    }




}
