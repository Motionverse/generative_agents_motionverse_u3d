using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchControl : MonoBehaviour
{

    //private static Weak
    private static WeakReference<WatchControl> instance;

    public float speed = 6;

    private CharacterController cc;

    private CameraControl cameraControl;

    [HideInInspector]
    public Transform follow;

    [SerializeReference]
    private Transform cameraTarget;
    public static WatchControl Instance
    {
        get
        {
            if (instance != null && instance.TryGetTarget(out var obj))
            {
                return obj;
            }
            return null;
        }
    }

    private void Awake()
    {
        instance = new WeakReference<WatchControl>(this);
        
    }

    void Start()
    {
        cameraControl = CameraControl.Instance;

        cameraControl.Target = cameraTarget;

        cc = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        var joystick = MobileJoystick.Instance;
        
        
        bool HasControl = false;
        Vector3 newDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) { newDir += cameraControl.forward; HasControl = true; }
        if (Input.GetKey(KeyCode.S)) { newDir -= cameraControl.forward; HasControl = true; }
        if (Input.GetKey(KeyCode.A)) { newDir -= cameraControl.right;HasControl = true; }
        if (Input.GetKey(KeyCode.D)) { newDir += cameraControl.right; HasControl = true; }

        if (joystick!=null && joystick.isMoved)
        {
            HasControl = true;
            newDir += joystick.x * cameraControl.right;
            newDir += joystick.y * cameraControl.forward;
        }
        if (!HasControl)
        {
            if (follow != null)
            {
                transform.position = follow.position;
            }

            return;
        }

        follow = null;

        newDir.y = 0;
        newDir = newDir.normalized;

       transform.forward = newDir; 
        
        cc.Move(newDir * speed * Time.deltaTime);

    }


}
