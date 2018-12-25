using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {
    private static CharacterManager instance;
    public static CharacterManager Instance { get { return instance; } }

    public bool characterClicked = false;

    public List<Character> CharacterList { get; set; }
    public Character ControlledCharacter { get; set; }
    public Player ThePlayer { get; set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start () {
        CharacterList = new List<Character>();
        CharacterList.AddRange(FindObjectsOfType<Character>());
        
        //for(int i = 0; i < characterList.Count; i++)
        //{
        //    if (!characterList[i].IsAI)
        //        ControlledCharacter = characterList[i];
        //}
        //characterList = ControlledCharacter;
	}
	
	// Update is called once per frame
	void Update () {
        SwitchCharacter();
	}
    void SwitchCharacter()
    {
        if(Input.GetMouseButtonDown(0))
            CheckForClickOnCharacter();
    }

    //TODO: Remove this function later
    void CheckForClickOnCharacter()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector3.forward, 15);
        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<Character>())
            {
                hit.collider.GetComponent<Character>().UseItem();
            }
        }
            /*MovementScript2D ms = null;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            //TODO: Add a layer mask that filters out the character colliders
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector3.forward, 15);
            if(hit.collider != null)
            {
                if (hit.collider.GetComponent<MovementScript2D>())
                {
                    ms = hit.collider.GetComponent<MovementScript2D>();
                    if (ms != ControlledCharacter && (ms.IsAI || ms == Player))
                    {
                        //If the previous controlled character is not out main charcater,
                        //Make it work under AI
                        if (ControlledCharacter != Player)
                            ControlledCharacter.IsAI = true;
                        else
                        {
                            ControlledCharacter.playerState = PlayerStates.stun;
                        }

                        //Controlled character is changed to the character that's clicked
                        ControlledCharacter = hit.collider.GetComponent<MovementScript2D>();

                        if(ControlledCharacter == Player)
                        {
                            ControlledCharacter.playerState = PlayerStates.idle;
                        }
                        else
                        {
                            //AI of the character that we take control right now is disabled for us to manipulate it
                            ControlledCharacter.IsAI = false;
                            characterClicked = true;
                        }
                    }
                }
            }
            return ms;*/
        }

    private void LateUpdate()
    {
        characterClicked = false;
    }
}
