using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

    public GameObject target;

    Vector3 direction;
    bool released = false;

    public float angle;
    public float time = 1.5f;

    Vector3 originalPosition;
	// Use this for initialization
	void Start () {
        direction = target.transform.position - gameObject.transform.position;
        originalPosition = gameObject.transform.position;
        //angle *= Mathf.Deg2Rad;
	}
    

    Vector3 t = Vector3.zero;
    float velocity = 0;

    float timeElapsed = 0;

    Vector3 previousDirection = Vector3.zero;
	// Update is called once per frame
	void Update () {
        
        if (Input.GetMouseButtonDown(0))
        {
            direction = target.transform.position - gameObject.transform.position;
            velocity = direction.magnitude / time;

            t = Quaternion.AngleAxis(angle, Vector3.forward) * direction;
            previousDirection = t;

            released = true;
        }
        if (released)
        {
            timeElapsed += Time.deltaTime;
            if(timeElapsed > time / 3.0f)
            {
                t = target.transform.position - gameObject.transform.position;
            }

            //gameObject.transform.position += Vector3.Lerp(gameObject.transform.position, t.normalized * velocity * Time.deltaTime, 0.4f);
            //Vector3 pos = gameObject.transform.position + t.normalized * velocity * Time.deltaTime;
            Vector3 direction = Vector3.Lerp(previousDirection.normalized, t.normalized, 0.1f);
            gameObject.transform.position += direction.normalized * velocity * Time.deltaTime;

            previousDirection = direction;

            if(timeElapsed > time + 0.3f)
            {
                gameObject.transform.position = originalPosition;
                released = false;
                timeElapsed = 0;
            }
            
        }
	}
}
