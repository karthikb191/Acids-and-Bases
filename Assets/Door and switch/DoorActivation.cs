using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorActivation : MonoBehaviour {

    public List<SwichAndDoorActivation> switchScripts;

    public bool openDoor;

    bool isOnFocus;

    [SerializeField]
    string tagToCompare;


    Animator doorAnimator;
    BoxCollider2D doorCollider;



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

        if(openDoor)
        {
            OpenDoor();
        }
    }


    void OpenDoor()
    {
        //doorCollider.isTrigger = true;
        doorAnimator.SetBool("OpenDoor", true);
        doorCollider.enabled = false;
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {

            if (collision.GetComponent<Player>() != null)
            {
                //Enable the button
                DynamicButton d = VirtualJoystick.CreateButton("tag_door");
                if (!d.active)
                {
                    VirtualJoystick.EnableButton(d);
                    d.button.onClick.AddListener(() =>
                    {
                        OpenDoor();
                        VirtualJoystick.DisableButton(d);
                    });
                }
            }
        }

        if (collision.gameObject.GetComponent<ItemBase>() != null && !collision.gameObject.GetComponent<ItemBase>().isFromEnemy)
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            VirtualJoystick.DisableButton("tag_door");
        }

       
    }


}
