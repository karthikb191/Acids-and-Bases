using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    public GameObject background;
    public GameObject control;

    public RectTransform backgroundRect;
    public RectTransform controlRect;

	// Use this for initialization
	void Start () {
        background = transform.Find("Background").gameObject;
        control = background.transform.Find("Control").gameObject;

        backgroundRect = background.GetComponent<RectTransform>();
        controlRect = control.GetComponent<RectTransform>();
	}

    public void OnPointerDown(PointerEventData ped) {
        float halfWidth = backgroundRect.sizeDelta.x * 0.5f;
        float halfHeight = backgroundRect.sizeDelta.y * 0.5f;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundRect, ped.position, ped.pressEventCamera, out localPoint)){
            VirtualJoystick.horizontalValue = Mathf.Clamp(localPoint.x / (halfWidth / 1.5f), -1, 1);
            VirtualJoystick.verticalValue = Mathf.Clamp(localPoint.y / (halfWidth / 1.5f), -1, 1);
            float xPos = Mathf.Clamp(localPoint.x / (halfWidth), -1, 1);
            //float ypos = localPoint.y / (halfHeight);
            Debug.Log("scaled values of the control is: " + VirtualJoystick.horizontalValue + " : " + VirtualJoystick.verticalValue);

            controlRect.localPosition = new Vector3(xPos * halfWidth, 0, 0);

        }


    }

    public void OnDrag(PointerEventData ped) {
        float halfWidth = backgroundRect.sizeDelta.x * 0.5f;
        float halfHeight = backgroundRect.sizeDelta.y * 0.5f;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundRect, ped.position, ped.pressEventCamera, out localPoint)){
            Debug.Log("on drag returning true");
            VirtualJoystick.horizontalValue = Mathf.Clamp(localPoint.x / (halfWidth / 1.5f), -1, 1);
            float xPos = Mathf.Clamp(localPoint.x / (halfWidth), -1, 1);
            //VirtualJoystick.verticalValue = Mathf.Clamp(localPoint.y / (halfHeight), -1, 1);
            VirtualJoystick.verticalValue = 0;
            Debug.Log("scaled values of the control is: " + VirtualJoystick.horizontalValue + " : " + VirtualJoystick.verticalValue);
            controlRect.localPosition = new Vector3(xPos * halfWidth, 0, 0);
        }
    }

    public void OnPointerUp(PointerEventData ped) {
        controlRect.localPosition = Vector3.zero;
        VirtualJoystick.horizontalValue = 0;
        VirtualJoystick.verticalValue = 0;
    }
}
