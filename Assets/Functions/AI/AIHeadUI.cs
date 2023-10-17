using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class AIHeadUI : MonoBehaviour
{

    private static Dictionary<string, string> emoMap = new Dictionary<string, string>();

    public TMPro.TMP_Text nameText;

    public GameObject talkPanel;

    public TMPro.TMP_Text talkText;

    public Image emoImage;

    // Start is called before the first frame update
    void Start()
    {
        talkPanel.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ShowTalk(string text)
    {
        talkText.text = text;
        talkPanel.SetActive(true);
    }

    private string ParseFourHex(byte[] bytes ,int offset, StringBuilder hex)
    {

        var sb = new StringBuilder();
        sb.Append(bytes[offset+1].ToString("x2"));
        sb.Append(bytes[offset+0].ToString("x2"));

        string s1 = sb.ToString();
        hex.Append(s1);
        int t1 = int.Parse(s1, System.Globalization.NumberStyles.HexNumber);

        sb.Clear();
        sb.Append(bytes[offset+3].ToString("x2"));
        sb.Append(bytes[offset+2].ToString("x2"));
        string s2 = sb.ToString();
        hex.Append(s2);
        int t2 = int.Parse(s2, System.Globalization.NumberStyles.HexNumber);
        return ((t1 - 0xD800) * 0x400 + (t2 - 0xDC00) + 0x10000).ToString("x2");
    }

    public void ShowEmo(string status)
    {
        //var b = 0xd83dde34 - 0x1f634;

        if(!emoMap.TryGetValue(status,out var emoName))
        {

            byte[] unic = Encoding.Unicode.GetBytes(status);
            var sb = new StringBuilder();
            var sb1 = new StringBuilder();
            for (var i = 0;i+4 <= unic.Length; i += 4)
            {
                if (sb.Length > 0)
                {
                    sb.Append("-");
                    sb1.Append("-");
                }
                sb.Append(ParseFourHex(unic, i, sb1));
            }
            emoName = sb.ToString();
            emoMap.Add(status,emoName);
            Debug.Log("New EMO:"+ sb1.ToString()  + "->" + emoName);
        }

        emoImage.sprite = Resources.Load<Sprite>("imgs/" + emoName);
        


    }
    //\\ud83d\\ude34  F400 234  1f634
    //public string emojiUnicode(string emoji)
    //{
    //    int value = 0;
    //    if (emoji.length === 1)
    //    {
    //        comp = emoji.charCodeAt(0);
    //    }
    //    comp = (
    //        (emoji.charCodeAt(0) - 0xD800) * 0x400
    //      + (emoji.charCodeAt(1) - 0xDC00) + 0x10000
    //    );
    //    if (comp < 0)
    //    {
    //        comp = emoji.charCodeAt(0);
    //    }
    //    return comp.toString("16");
    //}


    public static string toUnicode(string s)
    {
        var bytes = Encoding.Unicode.GetBytes(s);
        var stringBuilder = new StringBuilder();
        List<List<byte>> resultList = new List<List<byte>>();
        List<byte> tempList = new List<byte>();
        for (int i = 0, length = bytes.Length; i < length; i++)
        {
            tempList.Add(bytes[i]);
            if (tempList.Count == 4)
            {
                tempList.Reverse();
                resultList.Add(tempList);
                tempList = new List<byte>();
            }
        }
        for (int i = 0, length = resultList.Count; i < length; i++)
        {
            List<byte> elist = resultList[i];
            bool isFirst = false;
            for (int j = 0; j < elist.Count; j++)
            {
                string b = elist[j].ToString("x2");
                if (b == "00" && j < 2)
                    continue;
                if (!isFirst)
                {
                    isFirst = true;
                    if (j < 2)
                        b = elist[j].ToString("x");
                    stringBuilder.Append(b);
                }
                else
                {
                    stringBuilder.Append(b);
                }
                //Debug.Log(b);
            }
            if (isFirst && i != length - 1)
            {
                stringBuilder.Append("_");
            }
        }
        return stringBuilder.ToString().Replace("_0000", "");
    }

    public void HideTalk()
    {
        talkPanel.SetActive(false);
    }

}
