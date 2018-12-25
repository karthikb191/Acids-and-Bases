using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// Duty is to collect the forces of all the external fields except gravity.
/// Collects them ans stores in some variable, which can be accessed by the player or the
/// obstacles that want to interact with forces
/// 
/// </summary>
public class ExternalForce : MonoBehaviour {

    public Vector3 force;

	// Use this for initialization
	void Start () {
	    	
	}
	
	// Update is called once per frame
	void Update () {
        //Add this force to the movement script of the player
        if (gameObject.GetComponent<MovementScript2D>() != null)
        {
            gameObject.GetComponent<MovementScript2D>().externalForce += force;
        }
        
        //If it's the objects we are worried about, then add it to the objects
	}
    private void LateUpdate()
    {
        force = Vector3.zero;
    }
}
