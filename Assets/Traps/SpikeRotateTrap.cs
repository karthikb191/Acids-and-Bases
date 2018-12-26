using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeRotateTrap : MonoBehaviour {

    [SerializeField]
    float speed;

    [SerializeField]
    float damageAmount;

    Character damageDealtTo;

    bool isOnFocus;

  
         

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1) * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {    
        if(collision.GetComponent<Character>() != null )
        {
            isOnFocus = true;
            collision.GetComponent<Character>().TakeDamage(damageAmount);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {       
        if (collision.GetComponent<Character>() != null && isOnFocus)
        {
            damageDealtTo = collision.GetComponent<Character>();
            Invoke("DealDamage", 0.5f);
        }     
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Character>() != null)
        {
            isOnFocus = false;
            damageDealtTo = null;
            damageAmount = 5;
        }
    }

    void DealDamage()
    {       
        if(damageDealtTo != null)
        {
            damageDealtTo.TakeDamage(damageAmount);
            damageAmount += 0.005f;           
        }
    }
}
