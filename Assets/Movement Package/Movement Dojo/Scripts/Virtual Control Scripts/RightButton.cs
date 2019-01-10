using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightButton : MonoBehaviour, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler {
    RectTransform r;
    bool pressed;
	// Use this for initialization
	void Start () {
        r = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        if (pressed) {
            VirtualJoystick.horizontalValue = 1 * ControlCanvas.StartCounter();

        }
	}

    public virtual void OnPointerDown(PointerEventData ped) {
        ControlCanvas.ResetCounter(); // might have to be removed
        pressed = true;
        
    }

    public virtual void OnPointerUp(PointerEventData ped) {
        
        VirtualJoystick.horizontalValue = 0;
        ControlCanvas.ResetCounter();
        pressed = false;
    }
    
    public virtual void OnPointerExit(PointerEventData ped) {
        
        VirtualJoystick.horizontalValue = 0;
        ControlCanvas.ResetCounter();
        pressed = false;
    }
}