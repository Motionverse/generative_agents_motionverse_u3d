using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    // ��¼֡��
    private int _frame;
    // ��һ�μ���֡�ʵ�ʱ��
    private float _lastTime;
    // ƽ��ÿһ֡��ʱ��
    private float _frameDeltaTime;
    // ����೤ʱ��(��)����һ��֡��
    private float _Fps;
    private const float _timeInterval = 0.5f;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        _lastTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        FrameCalculate();
    }

    private void FrameCalculate()
    {
        _frame++;
        if (Time.realtimeSinceStartup - _lastTime < _timeInterval)
        {
            return;
        }

        float time = Time.realtimeSinceStartup - _lastTime;
        _Fps = _frame / time;
        _frameDeltaTime = time / _frame;

        _lastTime = Time.realtimeSinceStartup;
        _frame = 0;
    }

    private void OnGUI()
    {
        text.text = "" +_Fps;
    }
}
