using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toast : MonoBehaviour
{
    private static Toast instance = null;
    [SerializeReference]
    private TMPro.TMP_Text text;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
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


    private void OnEnable()
    {
        StartCoroutine(autoHide());
    }
    private IEnumerator autoHide()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    public static void Show(string text)
    {
        instance.gameObject.SetActive(true);
        instance.text.text = text;
        
    }

}
