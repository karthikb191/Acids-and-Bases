using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgChangeTrigger : MonoBehaviour {
    public AudioClip audioClip;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //If in contact with the player, then call the bg change script
        if (collider.tag == "tag_player")
        {
            if (audioClip != null)
            {
                StartCoroutine(BackgroundAudio.ChangeTracks(audioClip));
            }
        }
    }

}
