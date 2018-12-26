using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwichAndDoorActivation : MonoBehaviour

{
    SpriteRenderer switchSprite;
    bool isOnFocus;
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
        if(isOnFocus && Input.GetKeyDown(KeyCode.O))
        {
            switchSprite.color = Color.green;
            isActivated = true;   
        }
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(tagToCompare))
        {
            isOnFocus = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(tagToCompare))
        {
            isOnFocus = false;
        }
    }
}
