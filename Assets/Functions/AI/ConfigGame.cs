using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigGame : MonoBehaviour
{
    [SerializeReference]
    private Slider speedSlider;
    [SerializeReference]
    private Slider progressSlider;


    [SerializeReference]
    private TMPro.TMP_Text speedText;

    [SerializeReference]
    private TMPro.TMP_Text progressText;


    public Action OnChose;
    public Action<float,float> OnStart;
    // Start is called before the first frame update

    public Button choseButton;
    public Button startButton;

    private void Awake()
    {
        speedSlider.onValueChanged.AddListener(OnSpeedChanged);
        progressSlider.onValueChanged.AddListener(OnProressChanged);
    }

    void Start()
    {


    }

    private void OnEnable()
    {
        speedSlider.value = 1;
        progressSlider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        startButton.interactable = AIManager.Instance.total > 0 && !AIManager.Instance.choosing;
    }

    private void OnSpeedChanged(float value)
    {
        speedText.text =  "x"+ value;
    }
    private void OnProressChanged(float value)
    {
        progressText.text = "" + value + "%";
    }

    public void OnClickChose()
    {
        OnChose?.Invoke();
    }

    public void OnClickStart()
    {
        OnStart?.Invoke(progressSlider.value,speedSlider.value);
    }

}
