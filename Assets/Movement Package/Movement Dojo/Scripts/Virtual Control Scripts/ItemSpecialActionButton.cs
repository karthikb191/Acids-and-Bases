using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSpecialActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    bool buttonPressed = false;

    private void FixedUpdate()
    {
        if (buttonPressed && VirtualJoystick.itemSpecialActionButtonDown)
            VirtualJoystick.itemSpecialActionButtonDown = false;
    }

    public void OnPointerDown(PointerEventData ped)
    {
        if (!buttonPressed)
        {
            VirtualJoystick.itemSpecialActionButtonDown = true;
            VirtualJoystick.itemSpecialActionButtonUp = false;
            buttonPressed = true;
        }
    }
    public void OnPointerUp(PointerEventData ped)
    {
        if (buttonPressed)
        {
            VirtualJoystick.itemSpecialActionButtonUp = true;
            VirtualJoystick.itemSpecialActionButtonDown = false;
            buttonPressed = false;
        }
    }

}
