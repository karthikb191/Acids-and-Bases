using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ItemHolderScript : MonoBehaviour {
    public Material itemMaterial;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.GetChild(0) != null)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sharedMaterial.SetVector("_Center", new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0));
        }
	}
}
