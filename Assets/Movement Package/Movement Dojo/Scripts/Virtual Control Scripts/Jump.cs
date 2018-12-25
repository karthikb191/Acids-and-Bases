using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Jump : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public void OnPointerDown(PointerEventData ped) {
        VirtualJoystick.jumpButtonDown = true;
        VirtualJoystick.jumpButtonUp = false;
    }

    public void OnPointerUp(PointerEventData ped) {
        VirtualJoystick.jumpButtonDown = false;
        VirtualJoystick.jumpButtonUp = true;
    }
	
}
