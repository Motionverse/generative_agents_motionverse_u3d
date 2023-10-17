using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorLock : MonoBehaviour
{
    [SerializeField][Tooltip("Defines the Cursor Lock Mode to apply")] 
    private CursorLockMode cursorLockMode;
    [SerializeField][Tooltip("If true will hide mouse cursor")] 
    private bool hideCursor = true;
    [SerializeField][Tooltip("If true it apply cursor settings on start")]
    private bool applyOnStart = true;
    
    // Start is called before the first frame update
    void Start()
    {
        if (applyOnStart)
        {
            Apply();
        }
    }

    public void Apply()
    {
        Cursor.visible = hideCursor;
        Cursor.lockState = cursorLockMode;
    }
}
