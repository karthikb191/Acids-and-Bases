using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorActivation : MonoBehaviour {

    public List< SwichAndDoorActivation> switchScripts;

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
        if(openDoor && Input.GetKeyDown(KeyCode.I) && isOnFocus )
        {
            Invoke("OpenDoor", 0.25f);
        }
		
	}

    void CheckSwitchStatus()
    {
        for (int i = 0; i < switchScripts.Count; i++)
        {
            if(switchScripts[i].isActivated)
            {
              
                openDoor = true;
                doorCollider.isTrigger = true;
                continue;
            }

            else
            {
               
                doorCollider.isTrigger = false;
                openDoor = false;
                break;

            }
        }
    }


    void OpenDoor()
    {
        doorCollider.isTrigger = true;
        doorAnimator.SetBool("OpenDoor", true);     
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(tagToCompare))
        {
            isOnFocus = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(tagToCompare))
        {
            isOnFocus = false;
        }
    }


}
