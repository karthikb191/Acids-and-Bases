using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CinematicTrigger : MonoBehaviour {

    public delegate void CinematicTriggerEnter();
    public static event CinematicTriggerEnter CinematicTriggerEnterEvent;

    public int triggerNumber;
    public bool deactivateAfterTrigger = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            LevelScripts.SetTriggerNumber(triggerNumber);
            
            if(CinematicTriggerEnterEvent != null)
                CinematicTriggerEnterEvent();

            if (deactivateAfterTrigger)
                gameObject.SetActive(false);
        }
    }

}
