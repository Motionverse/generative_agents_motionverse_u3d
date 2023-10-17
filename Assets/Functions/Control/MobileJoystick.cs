using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    private static WeakReference<MobileJoystick> instance;

    private float MovementRange = 150;


    public GameObject big;
    public GameObject small;

    public bool isTouched { get; set; }

    public bool isMoved { get; set; }

    private long pointerId = - 99;

    public static MobileJoystick Instance
    {
        get
        {
            MobileJoystick joystick = null;
            if(instance!=null) instance.TryGetTarget(out joystick);
            return joystick;
        }
    }

    private void Awake()
    {
        instance = new WeakReference<MobileJoystick>(this);
    }

  


    void Start(){
        MovementRange = (big.transform as RectTransform).rect.width / 2;
    }
  
    public void OnPointerMove(PointerEventData data)
	{
        if (pointerId!= data.pointerId) return;
        Vector3 newPos = Vector3.zero;
        
        newPos.x = (int)(data.position.x - data.pressPosition.x); 
		
        newPos.y = (int)(data.position.y - data.pressPosition.y); 

        //float deg = AngleSigned(Vector3.up, newPos.normalized, Vector3.forward);

        small.transform.localPosition = Vector3.ClampMagnitude(newPos, MovementRange) ;

        isMoved = true;
    }

    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    public void OnPointerDown(PointerEventData data){

        if (pointerId == -99)
        {
            pointerId = data.pointerId;
            big.transform.position = data.position;
            isTouched = true;
        }

    }


    public void OnPointerUp(PointerEventData data){
        if (pointerId != data.pointerId) return;
        CancelMove();
    }

    public void CancelMove()
    {
        
        big.transform.localPosition = Vector3.zero;
        small.transform.localPosition = Vector3.zero;
        isTouched = false;
        isMoved = false;
        pointerId = -99;
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnPointerMove(eventData);
    }

    public float x
    {
        get=> small.transform.localPosition.x;
    }

    public float y
    {
        get => small.transform.localPosition.y;
    }
    
}