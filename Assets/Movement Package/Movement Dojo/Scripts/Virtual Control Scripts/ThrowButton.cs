using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Player player;

    Vector3 initialPosition;

    bool buttonPressed = false;

    bool setTarget;


    private void Awake()
    {
        initialPosition = transform.position;
    }
    private void Start()
    {
        player = FindObjectOfType<Player>();
    }
    private void Update()
    {
        if(setTarget)
        {
            if(Input.GetMouseButtonUp(1))
            {
                Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                ThrowItem(targetPosition);
                setTarget = false;

               // transform.position = targetPosition;
            }


        }
        
        if (player.GetComponentInChildren<PlayerInventory>().activeItem == null)
        {
            
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }

    }


    private void FixedUpdate()
    {
      /*  if (buttonPressed && VirtualJoystick.throwButton)
            VirtualJoystick.throwButtonButtonDown = false;*/
    }

    public void OnPointerDown(PointerEventData ped)
    {
        if (!buttonPressed)
        {
            VirtualJoystick.throwButtonButtonDown = true;
            VirtualJoystick.throwButtonButtonUp = false;
            buttonPressed = true;
            setTarget = true;
          //  transform.parent = null;
        }
    }
    public void OnPointerUp(PointerEventData ped)
    {
        if (buttonPressed)
        {
            VirtualJoystick.throwButtonButtonUp = true;
            VirtualJoystick.throwButtonButtonDown = false;
            buttonPressed = false;
           
            //ThrowItem(transform.position);
          //  setTarget = false;
            //
        }
    }


    public void ThrowItem(Vector3 target)
    {
        player.GetComponentInChildren<PlayerInventory>().ThrowItem(target);
        ResetPosition();
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        setTarget = false;
    }
}

