﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwichAndDoorActivation : MonoBehaviour

{
    SpriteRenderer switchSprite;
   
    public bool isActivated;
    [SerializeField]
    string tagToCompare;
	// Use this for initialization
	void Start ()
    {
        switchSprite = gameObject.GetComponent<SpriteRenderer>();
	}	
	// Update is called once per frame
	void Update ()
    {
       
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
                        ActivateDoor();
                        VirtualJoystick.DisableButton(d);
                    });
                }
            }
        }
        Debug.Log("thrown item: " + collision.gameObject.name);
    
        if (collision.gameObject.GetComponent<ItemBase>() != null && !collision.gameObject.GetComponent<ItemBase>().isFromEnemy)
        {
            ActivateDoor();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
      
        if (collision.gameObject.GetComponent<Player>())
        {
            VirtualJoystick.DisableButton("tag_door");
        }

        
    }

    private void ActivateDoor()
    {
        switchSprite.color = Color.green;
        isActivated = true;
    }
}
