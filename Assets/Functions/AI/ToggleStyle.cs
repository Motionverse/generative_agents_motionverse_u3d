using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleStyle : MonoBehaviour
{

    private Toggle toggle;
    // Start is called before the first frame update
    private TMPro.TMP_Text text;

    public Color selectedColor;

    public Color unselectedColor;

    private void Awake()
    {
        toggle = GetComponentInParent<Toggle>();
        toggle.onValueChanged.AddListener( OnToggleValueChanged );
        text = GetComponent<TMPro.TMP_Text>();
    }

    void Start()
    {
        OnToggleValueChanged(toggle.isOn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnToggleValueChanged(bool value)
    {
        text.color = value ? selectedColor : unselectedColor;
    }

}
