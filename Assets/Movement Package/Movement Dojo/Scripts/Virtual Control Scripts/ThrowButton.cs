using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ThrowButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Player player;

    Vector3 initialPosition;

    bool buttonPressed = false;

    bool setTarget;


    private void Awake()
    {
      //  initialPosition = transform.position;
    }
    private void Start()
    {
        player = FindObjectOfType<Player>();
    }
    private void Update()
    {
        /* if(setTarget)
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
         */

        if (player.GetComponentInChildren<PlayerInventory>().activeItem != null)
        {
            this.gameObject.GetComponent<Image>().enabled = true;
        }
        else
        {
            this.gameObject.GetComponent<Image>().enabled = false;
        }

        if(Input.GetMouseButtonDown(1))
        {
            tempTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("New Target set to: _____ " + tempTarget);
            ThrowItem();
        }
    }

    Vector3 tempTarget;
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
           
            ThrowItem();
          //  setTarget = false;
            //
        }
    }


    public void ThrowItem()
    {
        if(player.GetComponentInChildren<PlayerInventory>().activeItem != null)
        {

            tempTarget.z = 0;

            Vector3 tempTar = player.Hand.transform.position;
            //Debug.Log(player.right + "o throw");
            if (player.playerSprite.transform.localScale.x > 0)
            {
                tempTar.x += 6;
            }
            else
            {
                tempTar.x -= 6;
                Quaternion temp = player.GetComponentInChildren<PlayerInventory>().activeItem.transform.localRotation;
                temp.y = 180;

            }
           
            Debug.Log("Player position" + player.Hand.transform.position + " <<>><><>" + "Temp target" + tempTarget);
            if(tempTarget!= null)
            player.GetComponentInChildren<PlayerInventory>().ThrowItem(tempTarget, 5);

            else
                player.GetComponentInChildren<PlayerInventory>().ThrowItem(tempTar, 5);
        }
       
       // ResetPosition();
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        setTarget = false;
    }
}

