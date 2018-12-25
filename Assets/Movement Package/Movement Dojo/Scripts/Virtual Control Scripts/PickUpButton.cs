using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PickUpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    bool buttonPressed = false;

    public Sprite pickupSprite;
    public Sprite dropSprite;

    private void FixedUpdate()
    {
        if (buttonPressed && VirtualJoystick.pickUpButtonDown)
            VirtualJoystick.pickUpButtonDown = false;
    }

    public void OnPointerDown(PointerEventData ped)
    {
        if (!buttonPressed)
        {
            VirtualJoystick.pickUpButtonDown = true;
            VirtualJoystick.pickUpButtonUp = false;
            buttonPressed = true;
        }
    }
    public void OnPointerUp(PointerEventData ped)
    {
        if (buttonPressed)
        {
            VirtualJoystick.pickUpButtonUp = true;
            VirtualJoystick.pickUpButtonDown = false;
            buttonPressed = false;
        }
    }
}
