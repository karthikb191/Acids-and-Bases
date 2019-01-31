using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    List<Door> doorsInRoom; //Get the list of all the doors in the room

    public float width = 10;
    public float height = 5;
    public bool initialRoom = false;
    public GameObject cameraBoundaries = null;

    public bool chaseMustContinueToThisRoom = false;

    public float timeToSpawn = 3.0f;
    Vector3 positionToSpawn = Vector3.zero;

    [HideInInspector]
    List<Character> charactersThatMustBeSpawned;

	// Use this for initialization
	void Start () {
        doorsInRoom = new List<Door>();
        doorsInRoom.AddRange(GetComponentsInChildren<Door>());
        cameraBoundaries = gameObject.transform.Find("CameraBoundaries").gameObject;
        if (!initialRoom)
            gameObject.SetActive(false);
        else
            Camera.main.GetComponent<CameraScript>().boundariesContainer = cameraBoundaries;
    }
	
    public void ActivateRoom()
    {
        gameObject.SetActive(true);
    }
    public void DeactivateRoom()
    {
        gameObject.SetActive(false);
    }
    
    public void SetCharactersToSpawn(List<Character> characterList, Vector3 positionToSpawn)
    {
        charactersThatMustBeSpawned = characterList;
        this.positionToSpawn = positionToSpawn;
    }

    public void Spawn()
    {
        if (chaseMustContinueToThisRoom)
        {
            Debug.Log("Started Spawn");
            StartCoroutine(SpawnTimer());
        }
    }

    IEnumerator SpawnTimer()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < timeToSpawn)
        {
            elapsedTime += GameManager.Instance.DeltaTime;
            yield return new WaitForFixedUpdate();
        }
        //Setting the position of the characters that must be spawned
        for(int i = 0; i < charactersThatMustBeSpawned.Count; i++)
        {
            if (!charactersThatMustBeSpawned[i].gameObject.activeSelf)
            {
                charactersThatMustBeSpawned[i].gameObject.SetActive(true);
            }

            charactersThatMustBeSpawned[i].transform.position = positionToSpawn;

            //Reset the halt of the enemy so that he resumes his movement
            if(charactersThatMustBeSpawned[i].GetComponent<Enemy>())
                charactersThatMustBeSpawned[i].GetComponent<Enemy>().ResumeMovement();
        }
    }

    public bool showOutline = true;
    private void OnDrawGizmos()
    {
        if (showOutline)
        {
            Gizmos.color = Color.green;
            //Draw the outline of the current room
            Gizmos.DrawLine(new Vector3(gameObject.transform.position.x - width / 2, gameObject.transform.position.y - height / 2, gameObject.transform.position.z),
                            new Vector3(gameObject.transform.position.x - width / 2, gameObject.transform.position.y + height / 2, gameObject.transform.position.z));

            Gizmos.DrawLine(new Vector3(gameObject.transform.position.x - width / 2, gameObject.transform.position.y + height / 2, gameObject.transform.position.z),
                            new Vector3(gameObject.transform.position.x + width / 2, gameObject.transform.position.y + height / 2, gameObject.transform.position.z));

            Gizmos.DrawLine(new Vector3(gameObject.transform.position.x + width / 2, gameObject.transform.position.y + height / 2, gameObject.transform.position.z),
                            new Vector3(gameObject.transform.position.x + width / 2, gameObject.transform.position.y - height / 2, gameObject.transform.position.z));

            Gizmos.DrawLine(new Vector3(gameObject.transform.position.x + width / 2, gameObject.transform.position.y - height / 2, gameObject.transform.position.z),
                            new Vector3(gameObject.transform.position.x - width / 2, gameObject.transform.position.y - height / 2, gameObject.transform.position.z));
        }
    }

}
