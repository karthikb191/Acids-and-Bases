using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpTrigger : MonoBehaviour {

    public bool isActivated;

    public string helpMessage;

    public bool isShown;

    public LevelHelpBase levelHelper;

    public GameObject arrowPointer;

	// Use this for initialization
	void Start () {
        levelHelper = GetComponentInParent<LevelHelpBase>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Player>())
        {
            isActivated = true;

            levelHelper.TriggeredHelp();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            isActivated = false;
        }

    }
}
