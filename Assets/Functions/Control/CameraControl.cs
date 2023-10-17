using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    [HideInInspector]
    public Transform Target;

    private Vector3 dir;

    [SerializeField]
    private float distance = 20;

    private static WeakReference<CameraControl> weakReference = null;

    public static CameraControl Instance
    {
        get
        {
            CameraControl cameraControl = null;
            if (weakReference != null) weakReference.TryGetTarget(out cameraControl);
            return cameraControl;
        }
    }

    private void Awake()
    {
        weakReference = new WeakReference<CameraControl>(this);
    }

    private void OnDestroy()
    {
        
    }

    void Start()
    {
        dir = new Vector3(0,0.2f,1).normalized * distance;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        if (Target == null)
        {
            return;
        }
        float sw = Input.GetAxis("Mouse ScrollWheel");
        if (sw!=0)
        {
            distance += sw;

            if (distance < 2.8f) distance = 2.8f;
            else if (distance > 200) distance = 200;

            dir = dir.normalized * distance;
        }


        transform.position =  Target.position + dir;

        transform.LookAt(Target.position);

    }




    public Vector3 forward
    {
        get
        {
            return transform.forward;
        }
    }

    public Vector3 right
    {
        get
        {
            return transform.right;
        }
    }


    public void MoveCameraBy(Vector2 touchRotation)
    {
        if (touchRotation.x != 0)
        {
            dir = Quaternion.AngleAxis(touchRotation.x, Vector3.up) * dir;
        }

        if (touchRotation.y!= 0)
        {
            dir = Quaternion.AngleAxis(touchRotation.y, -transform.right) * dir;

            if (dir.y < 0)
            {
                dir.y = 0;
                dir = dir.normalized * distance;
            }

            //distance += touchRotation.y/10;

            //if(distance < 1)
            //{
            //    distance = 1;
            //}else if(distance > 50)
            //{
            //    distance = 50;
            //}

            //dir = dir.normalized * distance;
        }

    }



    public Vector3 Dir { get => dir.normalized; }

    public float Distance { get => distance; set{

            distance = value;
            dir = dir.normalized * distance;
        } }



}
