using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageTrap : MonoBehaviour {

    SwitchResponder switchResponder;

    [SerializeField]
    Character characterCaught;

    Vector3 initialPosition;
    bool deployTrap = false;
    bool resetTrap = false;
    bool characterAligned = false;

    public GameObject targetLocationObject;

    public bool resetTrapIfNothingCaught;

    public delegate void CharacterCaught();
    public event CharacterCaught CharacterCaughtEvent;

	// Use this for initialization
	void Start () {
        initialPosition = gameObject.transform.position;
        switchResponder = GetComponent<SwitchResponder>();
        //Subscribe to the event
        switchResponder.DoorOpenedEvent += InitiateTrap;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check for the encounter with enemy
        if(characterCaught == null && deployTrap)
            if (collision.GetComponent<Character>())
            {
                Debug.Log("Collided with a character");
                characterCaught = collision.GetComponent<Character>();
                //Raise the event when the character is caught for the first time
                StartCoroutine(WorkOnCaughtCharacter());

                if(CharacterCaughtEvent != null)
                    CharacterCaughtEvent();
            }
    }

    void InitiateTrap()
    {
        deployTrap = true;
    }

    private void Update()
    {
        if (deployTrap)
        {
            if (targetLocationObject == null)
            {
                Debug.Log("Set the empty target location object in the inspector.");
                deployTrap = false;
                return;
            }
            if(Vector3.SqrMagnitude(targetLocationObject.transform.position - gameObject.transform.position) > 0.1f)
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetLocationObject.transform.position, 0.1f);
            }
            else
            {
                if (characterCaught)
                {
                    //Set the character's parent as cage
                    characterCaught.transform.parent = gameObject.transform;

                    //Disable Character's collider
                    if (characterCaught.GetComponent<Collider2D>())
                        characterCaught.GetComponent<Collider2D>().enabled = false;
                }


                gameObject.transform.position = targetLocationObject.transform.position;
                deployTrap = false;
                if (resetTrapIfNothingCaught)
                    resetTrap = true;
            }
        }
        if (resetTrap)
        {
            if (characterCaught)
                if (!characterAligned)
                    return;
            if (Vector3.SqrMagnitude(initialPosition - gameObject.transform.position) > 0.1f)
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, initialPosition, 0.1f);
            }
            else
            {
                resetTrap = false;
                gameObject.transform.position = initialPosition;
                
                
                //Reset the switch only if the character is not caught yet
                if (!characterCaught)
                    switchResponder.ResetSwitches();
            }
        }
    }

    private void OnDestroy()
    {
        //Unsubscribe from the event
        switchResponder.DoorOpenedEvent -= InitiateTrap;
    }

    IEnumerator WorkOnCaughtCharacter()
    {
        //Stop character movements
        if (characterCaught.GetComponent<Enemy>())
        {
            Vector3 targetPosition = new Vector3(targetLocationObject.transform.position.x, targetLocationObject.transform.position.y,
                                                    targetLocationObject.transform.position.z);

            characterCaught.GetComponent<Enemy>().StopMovement(true, true, true);

            
            //Change the character's animation here
            

            //Align the character to the center of the cage
            while(Vector3.SqrMagnitude(characterCaught.transform.position - targetPosition) > 0.1f)
            {
                characterCaught.transform.position = Vector3.Lerp(characterCaught.transform.position, targetPosition, 0.2f);
                yield return new WaitForFixedUpdate();
            }


            //Disable the character's collider
            //if (characterCaught.GetComponent<Collider2D>())
            //    characterCaught.GetComponent<Collider2D>().enabled = false;


            characterAligned = true;
            //characterCaught.transform.position = gameObject.transform.position;
        }
        yield break;

        //Align character to the center of the cage
        
    }

}
