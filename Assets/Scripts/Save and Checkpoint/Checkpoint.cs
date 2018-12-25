using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    bool checkpointRegistered = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            if (collider.GetComponent<Player>())
            {
                if (!checkpointRegistered)
                {
                    //Register the checkpoint and change the color of the checkpoint for now
                    checkpointRegistered = true;
                    GetComponent<SpriteRenderer>().color = Color.green;
                    CheckPointManager.RegisterCheckPoint();
                }
            }
        }
    }

}
