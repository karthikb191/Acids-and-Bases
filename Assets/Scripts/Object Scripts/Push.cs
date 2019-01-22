using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Push : MonoBehaviour {
    public float timeElapsed = 0.0f;
    Player player;
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.GetComponent<Player>() != null)
        {
            timeElapsed += GameManager.Instance.DeltaTime;
            player = collision.collider.GetComponent<Player>();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Player>() != null)
        {
            timeElapsed = 0.0f;
            player = null;
        }
    }

    private void Update()
    {
        if(timeElapsed > 0.5f && player != null && player.State.Equals(typeof(RunningState)))
        {
            gameObject.transform.position += player.velocity;
        }
    }
}
