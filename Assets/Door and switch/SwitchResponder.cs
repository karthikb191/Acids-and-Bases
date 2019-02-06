using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchResponder : MonoBehaviour {

    public List<Switch> switchScripts;


    bool isOnFocus;

    [SerializeField]
    string tagToCompare;


    Animator doorAnimator;
    BoxCollider2D doorCollider;

    public bool createDynamicButton = true;
    bool doorOpened = false;

    public delegate void DoorOpened();
    public event DoorOpened DoorOpenedEvent;
    
    // Use this for initialization
    void Start ()
    {
        doorAnimator = gameObject.GetComponent<Animator>();
        doorCollider = gameObject.GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        CheckSwitchStatus();
	}

    void CheckSwitchStatus()
    {
        bool openDoor = false;
        for (int i = 0; i < switchScripts.Count; i++)
        {
            if(switchScripts[i].isActivated)
            {
              
                openDoor = true;
                //doorCollider.isTrigger = true;
                continue;
            }
            else
            {
                //doorCollider.isTrigger = false;
                openDoor = false;
                
                break;
            }
        }

        if(openDoor && !doorOpened)
        {
            OpenDoor();
            doorOpened = true;
        }
    }

    void OpenDoor()
    {
        doorCollider.isTrigger = true;
        if(doorAnimator != null)
            doorAnimator.SetBool("OpenDoor", true);

        //doorCollider.enabled = false;

        if(DoorOpenedEvent != null)
            DoorOpenedEvent();
    }

    public bool IsDoorOpen()
    {
        return doorOpened;
    }

    public void ResetSwitches()
    {
        for (int i = 0; i < switchScripts.Count; i++)
        {
            switchScripts[i].isActivated = false;
            doorOpened = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {

            if (collision.GetComponent<Player>() != null)
            {
                //Enable the button
                DynamicButton d = VirtualJoystick.CreateDynamicButton("tag_door");
                if (!d.active)
                {
                    VirtualJoystick.EnableDynamicButton(d);
                    d.button.onClick.AddListener(() =>
                    {
                        OpenDoor();
                        VirtualJoystick.DisableDynamicButton(d);
                    });
                }
            }
        }

        if (collision.gameObject.GetComponent<ItemBase>() != null)
        {
            if(collision.gameObject.GetComponent<ItemBase>().thrown && !collision.gameObject.GetComponent<ItemBase>().isFromEnemy)
                OpenDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            VirtualJoystick.DisableDynamicButton("tag_door");
        }

       
    }


}
