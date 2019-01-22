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
       if(player == null)
        player = FindObjectOfType<Player>();

        if (player.GetComponentInChildren<PlayerInventory>().activeItem != null)
        {
            this.gameObject.GetComponent<Image>().enabled = true;
        }
        else
        {
            this.gameObject.GetComponent<Image>().enabled = false;
        }

      
        

        if(buttonPressed)
        {
          
            ThrowItem();
            buttonPressed = false;
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

            Vector3 tempTar = player.transform.position;
            //Debug.Log(player.right + "o throw");

            float maxRange = player.GetComponentInChildren<PlayerInventory>().activeItem.GetComponent<ItemBase>().maxRangeOfThrow;
            if (player.playerSprite.transform.localScale.x > 0)
            {
                tempTar.x += maxRange;
            }
            else
            {
                tempTar.x -= maxRange;
               /* Quaternion temp = player.GetComponentInChildren<PlayerInventory>().activeItem.transform.localRotation;
                temp.y = 180;*/

            }
           
            Debug.Log("Player position" + player.Hand.transform.position + " <<>><><>" + "Temp target" + tempTarget);
           
            {
                player.GetComponentInChildren<PlayerInventory>().ThrowItem(tempTar, 5);
                Debug.Log("Auto target called" + tempTar);

            }
        }
       
       // ResetPosition();
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        setTarget = false;
    }
}

