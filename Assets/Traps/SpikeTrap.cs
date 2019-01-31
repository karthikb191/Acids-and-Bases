using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour {


    Animator spikeAnimator;
    Character damageDealtTo;

    [SerializeField]
    float damageAmount = 5;

    bool isOnFocus;

    public bool isActive = true;

    [SerializeField]
    Switch switchScript;

    // Use this for initialization
    void Start () {
        spikeAnimator = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        CheckActivation();
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Character>() != null && isActive)
        {
            isOnFocus = true;
            collision.GetComponent<Character>().TakeDamage(damageAmount);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Character>() != null && isOnFocus && isActive)
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
            damageAmount = 2;
        }
    }

    void DealDamage()
    {
        if (damageDealtTo != null)
        {
            damageDealtTo.TakeDamage(damageAmount);
            damageAmount += 0.005f;
        }
    }

    void CheckActivation()
    {
        if(switchScript.isActivated == true)
        {
            isActive = false;
            spikeAnimator.SetBool("TrapActive",false);
        }
    }
}
