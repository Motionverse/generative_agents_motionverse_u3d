using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystickV2 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    public class Touch
    {
        public int pid;
        public Vector2 move = Vector2.zero;
    }

    private List<Touch> touches = new List<Touch>();

    private float lastAVG = 0;

    private Vector2 movedVec;

    private float scaleRate = 0.02f;

    private bool needCheck3D = false;

    public bool IsTouching
    {
        get => touches.Count > 0;
    }

    void Start()
    {

    }



    private float MeasureAVG()
    {
        Vector2 cp = Vector2.zero;
        for (var i = 0; i < touches.Count; ++i)
        {
            cp = cp + touches[i].move;
        }

        cp.x /= touches.Count;
        cp.y /= touches.Count;

        float result = 0;
        for (var i = 0; i < touches.Count; ++i)
        {
            result += (touches[i].move - cp).sqrMagnitude;
        }

        return result;
    }



    public void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            PointerEventData data = new PointerEventData(null);
            data.pointerId = 10000;
            data.position = new Vector2(400, 400);
            OnPointerDown(data);

        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            PointerEventData data = new PointerEventData(null);
            data.pointerId = 10000;
            data.position = new Vector2(400, 400);
            OnPointerUp(data);
        }
#endif

        if (touches.Count==1 && movedVec != Vector2.zero)
        {
            CameraControl.Instance.MoveCameraBy(movedVec);
            movedVec = Vector2.zero;
        }

    }


    private Touch GetTouch(int pid, bool created = false)
    {
        for (var i = 0; i < touches.Count; ++i)
        {
            if (touches[i].pid == pid)
            {
                return touches[i];
            }
        }
        if (created)
        {
            Touch t = new Touch();
            t.pid = pid;
            touches.Add(t);
            return t;
        }
        return null;
    }

    public void OnDrag(PointerEventData data)
    {
        var touch = GetTouch(data.pointerId);
        if (touch == null) return;
        needCheck3D = false;
        touch.move = data.position;

        if (touches.Count == 1)
        {
            movedVec += data.delta;
        }
        else
        {
            var currentAVG = MeasureAVG();
            var ret = Mathf.Abs(lastAVG - currentAVG);
            if (ret < 10)
            {
                return;
            }
            var c = CameraControl.Instance;
            if (c!=null)
            {
                c.Distance += (Mathf.Sqrt(currentAVG) - Mathf.Sqrt(lastAVG)) * scaleRate;
            }
            lastAVG = currentAVG;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        var touch = GetTouch(data.pointerId, true);

        touch.move = data.position;

        if (touches.Count > 1)
        {
            needCheck3D = false;
            lastAVG = MeasureAVG();
        }
        else
        {
            needCheck3D = true;
        }

        DetailControl.Hide();
    }


    public void OnPointerUp(PointerEventData data)
    {

        var touch = GetTouch(data.pointerId);
        if (touch == null) return;

        touches.Remove(touch);

        if (touches.Count == 0)
        {
            //CameraControl.Instance.AutoRotation();
            var c = CameraControl.Instance;
            if (needCheck3D && c != null)
            {
                var camera = c.GetComponent<Camera>();
                RaycastHit hit;
                if (Physics.Raycast(camera.ScreenPointToRay(data.position), out hit))
                {
                    var cc =  hit.transform.gameObject.GetComponent<Clickable>();
                    if (cc != null) cc.OnClick?.Invoke();
                }
            }
        }
        else if (touches.Count == 1)
        {
            movedVec = Vector3.zero;
        }
        else
        {
            lastAVG = MeasureAVG();
        }
    }

}
