using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    List<Door> doorsInRoom; //Get the list of all the doors in the room

    public float width = 10;
    public float height = 5;
    public bool initialRoom = false;
    public GameObject cameraBoundaries = null;

	// Use this for initialization
	void Start () {
        doorsInRoom = new List<Door>();
        doorsInRoom.AddRange(GetComponentsInChildren<Door>());
        cameraBoundaries = gameObject.transform.Find("CameraBoundaries").gameObject;
        if (!initialRoom)
            gameObject.SetActive(false);
	}
	
    public void ActivateRoom()
    {
        gameObject.SetActive(true);
    }
    public void DeactivateRoom()
    {
        gameObject.SetActive(false);
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
