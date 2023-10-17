using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{

    //private static Weak
    private static WeakReference<MoveControl> instance;

    public float speed = 6;

    private Vector3 moveDir = new Vector3(0,0,0);
    //private Vector3 faceDir = new Vector3(0, 0, 0);
    // Start is called before the first frame update

    private CharacterController cc;

    private Animator animator;

    private float fallSpeed = 0;

    public float gravity = -10;

    public float JumpSpeed = 10;

    private bool isJumping = false;

    private CameraControl cameraControl;




    public static MoveControl Instance
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
        instance = new WeakReference<MoveControl>(this);
        
    }

    void Start()
    {
        cameraControl = CameraControl.Instance;

        cameraControl.Target = transform;

        cc = GetComponent<CharacterController>();


        animator = GetComponentInChildren<Animator>();
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
        if (Input.GetKey(KeyCode.Space)) { Jump(); }

        if (joystick!=null && joystick.isMoved)
        {
            HasControl = true;
            newDir += joystick.x * cameraControl.right;
            newDir += joystick.y * cameraControl.forward;
        }

        newDir.y = 0;
        newDir = newDir.normalized;


        if (!HasControl && !isJumping)
        {
            moveDir = Vector3.zero;
            //if (animator != null) animator.SetInteger("state", 0);
            
            animator.SetBool("run", false);
            if (animator.GetInteger("action") != 0)
            {
                AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (animatorStateInfo.normalizedTime >= 1)
                {
                    animator.SetInteger("action", 0);
                }
            }
            if (!cc.isGrounded)
            {
                fallSpeed += gravity * Time.deltaTime;
                cc.Move(new Vector3(0, fallSpeed*Time.deltaTime, 0));
            }
            else
            {
                fallSpeed = 0;
            }

            return;
        }

        if (isJumping)
        {
            //if (animator != null) animator.SetInteger("state", 2);
            //if (animator != null) animator.SetInteger("state", 2);
        }
        else 
        {
            transform.forward = newDir;
            //if (animator != null) animator.SetInteger("state", 1);
            
            animator.SetBool("run", true);
            animator.SetInteger("action", 0);

            moveDir = newDir;
        }

        Vector3 position = Vector3.zero;
        

        position += moveDir * speed * Time.deltaTime;
        

        if (cc.isGrounded)
        {
            isJumping = false;
            fallSpeed = 0;
            position.y -= 0.1f;
        }
        else 
        {
            fallSpeed += gravity * Time.deltaTime;
            position.y += fallSpeed * Time.deltaTime;
        }

        cc.Move(position);

    }


    public void Jump()
    {
        if (!isJumping && cc.isGrounded  )
        {
            isJumping = true;
            fallSpeed = JumpSpeed;
            
            var p = Vector3.zero;
            p.y = fallSpeed * Time.deltaTime;
            cc.Move(p);
        }
        
    }



    public int GetAnimalState()
    {
        if (animator != null)
        {
            //Debug.Log("cccc_AgoraManager: animator (state) = " + animator.GetInteger("state"));
            int m_stat = animator.GetInteger("state");
            return m_stat;
        }
        else
        {
            return 0;
        }
    }

    public void AnimatorControl(string key,int value)
    {
        animator.SetInteger(key, value);
    }

}
