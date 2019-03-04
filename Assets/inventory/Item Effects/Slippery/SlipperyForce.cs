using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyForce : MonoBehaviour {

    public int forceAmount = 50;
    public float stunDuration = 1.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character c = collision.GetComponent<Character>();
        if(c != null)
        {
            if (GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().Play();
            }
            c.AddExternalForce(forceAmount * new Vector3(c.playerSprite.transform.localScale.x, 0, 0));
            c.Stun(stunDuration);
        }
    }
}
