using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraProjectionChanger : MonoBehaviour {

    public float orthographicSize = 5;

    private float tempOrthographicSize;
    static bool cameraMoving;
    static Coroutine c;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "tag_player")
        {
            if (cameraMoving)
            {
                if (c != null)
                {
                    StopCoroutine(c);
                    c = null;
                }

            }
            
            tempOrthographicSize = Camera.main.orthographicSize;
            if (Camera.main.orthographicSize < orthographicSize + 0.05f && Camera.main.orthographicSize > orthographicSize - 0.05f)
                Camera.main.orthographicSize = orthographicSize;
            else
                c = StartCoroutine(ChangeCameraSize());
            
        }
    }
    

    IEnumerator ChangeCameraSize()
    {
        cameraMoving = true;
        while(Camera.main.orthographicSize > orthographicSize + 0.05f || Camera.main.orthographicSize < orthographicSize - 0.05f)
        {
            Camera.main.orthographicSize = tempOrthographicSize;
            yield return new WaitForFixedUpdate();

            if(Camera.main.orthographicSize > orthographicSize + 0.05f)
                tempOrthographicSize -= GameManager.Instance.DeltaTime;

            else if(Camera.main.orthographicSize < orthographicSize - 0.05f)
                tempOrthographicSize += GameManager.Instance.DeltaTime;
        }
        Camera.main.orthographicSize = orthographicSize;
        cameraMoving = false;
    }

}
