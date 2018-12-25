using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiddleButton : MonoBehaviour, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler {

    RectTransform r;
	// Use this for initialization
	void Start () {
        r = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void OnPointerDown(PointerEventData ped) {
        //ControlCanvas.middleButtonPressed = true;
        //ControlCanvas.clickControls = true;
        //Debug.Log("Middle press is true");
    }

    public virtual void OnPointerUp(PointerEventData ped) {
        //ControlCanvas.middleButtonPressed = false;
        //ControlCanvas.middleButtonReleased = true;  //set to false in the control canvas update
        //ControlCanvas.clickControls = false;
        //Debug.Log("middle press is false");
    }
    
    public virtual void OnPointerExit(PointerEventData ped) {
        //ControlCanvas.middleButtonPressed = false;
        //ControlCanvas.middleButtonReleased = true;
        //ControlCanvas.clickControls = false;
        //Debug.Log("middle press is false");
    }
}
