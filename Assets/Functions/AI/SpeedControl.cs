using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedControl : MonoBehaviour
{

    private Slider slider;

    [SerializeReference]
    private TMPro.TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderChanged);
        OnSliderChanged(slider.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSliderChanged(float value)
    {
        AIManager.Instance.speed = value;
        text.text = "x" + value;
    }

}
