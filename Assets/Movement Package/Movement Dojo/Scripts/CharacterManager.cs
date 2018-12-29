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
    }

    private void LateUpdate()
    {
        characterClicked = false;
    }
}
