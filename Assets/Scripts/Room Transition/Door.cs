using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    [HideInInspector]
    public Room room;  //Room this door belongs to.
    public Door connectingTo = null;   //The door this room connects to.

    bool locked = false;    //If a key needs to be used on the door, the key's use function unlocks it

    Player playerOnFocus = null;
	// Use this for initialization
	void Start () {
        room = GetComponentInParent<Room>();
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Player p = collider.GetComponent<Player>();
        if (p != null)
        {
            playerOnFocus = p;
            //Enable the button
            DynamicButton d = VirtualJoystick.CreateButton("tag_door");
            if (!d.active)
            {
                VirtualJoystick.EnableButton(d);
                d.button.onClick.AddListener(() =>
               {
                   p.userInputs.doorOpenPressed = true;

                   //Disable the button
                   VirtualJoystick.DisableButton(d);
               });
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject == playerOnFocus.gameObject)
        {
            playerOnFocus = null;

            //Disable the button
            VirtualJoystick.DisableButton("tag_door");
        }
    }

    // Update is called once per frame
    void Update () {
        if(playerOnFocus != null)
            if (playerOnFocus.userInputs.doorOpenPressed)
            {
                LevelManager.Instance.PrepareRoomShift(playerOnFocus, this);
            }
	}
}
