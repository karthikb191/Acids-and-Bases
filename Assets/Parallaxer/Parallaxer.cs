using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour {

    public SpriteRenderer[] spritesToInstantiate;

    [Range(-100, 100)]
    public int distanceFromCamera;

    [Range(0, 100)]
    public float horizontalGapBetweenObjects;

    List<SpriteRenderer> activeRenderers = new List<SpriteRenderer>();
    List<SpriteRenderer> deactiveRenderers = new List<SpriteRenderer>();
    Vector3[] cameraBounds;

    CameraScript cs;

    float multiplier;

    private void Start()
    {
        cs = Camera.main.GetComponent<CameraScript>();
        multiplier = ((float)distanceFromCamera / 100);
        Debug.Log("Parallaxer Multiplier: " + multiplier);
        //set the position of the parallaxer object straight ahead of the camera
        gameObject.transform.position = gameObject.transform.position + Camera.main.transform.forward * distanceFromCamera;

        //Instantiate at the camera view
        cameraBounds = GameManager.Instance.cameraBounds;

        float currentX = cameraBounds[1].x;
        float currentY = gameObject.transform.position.y;

        Debug.Log("bounds: " + cameraBounds[1]);
        Debug.Log("bounds: " + cameraBounds[2]);
        while(currentX < cameraBounds[2].x)
        {
            SpriteRenderer r = GetRandomSpriteRendererFromList();
            if (r != null)
            {
                Debug.Log("currentX: " + currentX);

                r = InstantiateSprite(r, new Vector3(currentX, currentY, transform.position.z), Quaternion.identity, this.transform);
                activeRenderers.Add(r);
                

                //Update the currentX
                currentX += r.bounds.size.x;
                Debug.Log("bounds x: " + r.bounds.size.x);
            }
            else
            {
                Debug.Log("No sprites provided.....Breaking up");
                break;
            }
        }
    }

    SpriteRenderer GetRandomSpriteRendererFromList()
    {
        return spritesToInstantiate[Random.Range(0, spritesToInstantiate.Length)];
    }

    SpriteRenderer InstantiateSprite(Vector3 position, Quaternion rotation, Transform parent)
    {
        SpriteRenderer r = GetRandomSpriteRendererFromList();
        r = Instantiate(r.gameObject, position, rotation).GetComponent<SpriteRenderer>();

        r.transform.SetParent(parent);
        r.sortingOrder = -10 * distanceFromCamera;
        return r;
    }

    SpriteRenderer InstantiateSprite(SpriteRenderer g, Vector3 position, Quaternion rotation, Transform parent)
    {
        SpriteRenderer r = Instantiate(g.gameObject, position, rotation).GetComponent<SpriteRenderer>();

        r.transform.SetParent(parent);
        r.sortingOrder = -10 * distanceFromCamera;
        return r;
    }

    void ActivateSprite(SpriteRenderer s, Vector3 position, Quaternion rotation, Transform parent = null)
    {

        s.gameObject.SetActive(true);
        
    }

    private void Update()
    {
        //Moving parallaxer
        gameObject.transform.position += new Vector3(cs.CameraMovementSpeed.x * GameManager.Instance.DeltaTime, 0, 0) * multiplier;

        //Checking the first object in the active objects list
        //Check if the first renderer in the list is not being rendered by camera anymore
        if (activeRenderers.Count > 0)
        {
            Debug.DrawLine(Vector3.zero, activeRenderers[0].transform.position);
            if (activeRenderers[0].transform.position.x + activeRenderers[0].bounds.size.x * 2.0f < cameraBounds[1].x)
            {
                //Deactivate or destroy the object
                activeRenderers[0].gameObject.SetActive(false);
                deactiveRenderers.Add(activeRenderers[0]);
                activeRenderers.RemoveAt(0);
            }

            //Depending on the position of the first renderer in the list, instantiate new object
            if (activeRenderers[0].transform.position.x + activeRenderers[0].bounds.size.x > cameraBounds[1].x)
            {
                //Check the pool and see if there are any objects
                if (deactiveRenderers.Count > 0)
                {
                    int randomIndex = Random.Range(0, deactiveRenderers.Count);
                    SpriteRenderer r = deactiveRenderers[randomIndex];
                    r.transform.position = new Vector3(activeRenderers[0].transform.position.x - activeRenderers[0].bounds.size.x,
                                                        activeRenderers[0].transform.position.y, activeRenderers[0].transform.position.z);
                    r.gameObject.SetActive(true);
                    activeRenderers.Insert(0, r);   //Insert at the beginning so that we can keep track of the objects
                    deactiveRenderers.RemoveAt(randomIndex);
                }
                else
                {
                    //If the pool is empty, instantiate a new object
                    SpriteRenderer r = GetRandomSpriteRendererFromList();
                    r = InstantiateSprite(new Vector3(activeRenderers[0].transform.position.x - activeRenderers[0].bounds.size.x,
                                                        activeRenderers[0].transform.position.y, activeRenderers[0].transform.position.z),
                                                        Quaternion.identity, this.transform);

                    activeRenderers.Insert(0, r);   //Insert at the beginning so that we can keep track of the objects
                    
                }
            }

            
            //Checking the last object in the active renderers
            if (activeRenderers[activeRenderers.Count - 1].transform.position.x - activeRenderers[activeRenderers.Count - 1].bounds.size.x * 2.0f > cameraBounds[2].x)
            {
                //deactivate or detroy the object
                activeRenderers[activeRenderers.Count - 1].gameObject.SetActive(false);
                deactiveRenderers.Add(activeRenderers[activeRenderers.Count - 1]);
                activeRenderers.RemoveAt(activeRenderers.Count - 1);
            }

            //Depending on the position of the last renderer in the list, instantiate new object
            if (activeRenderers[activeRenderers.Count - 1].transform.position.x - activeRenderers[activeRenderers.Count - 1].bounds.size.x < cameraBounds[2].x)
            {
                //Check the pool and see if there are any objects
                if (deactiveRenderers.Count > 0)
                {
                    int randomIndex = Random.Range(0, deactiveRenderers.Count);
                    SpriteRenderer r = deactiveRenderers[randomIndex];

                    r.transform.position = new Vector3(activeRenderers[activeRenderers.Count - 1].transform.position.x + activeRenderers[activeRenderers.Count - 1].bounds.size.x,
                                                        activeRenderers[activeRenderers.Count - 1].transform.position.y, activeRenderers[activeRenderers.Count - 1].transform.position.z);
                    r.gameObject.SetActive(true);
                    activeRenderers.Add(r);
                    deactiveRenderers.RemoveAt(randomIndex);
                }
                else
                {
                    //If the pool is empty, instantiate a new object
                    SpriteRenderer r = GetRandomSpriteRendererFromList();
                    r = InstantiateSprite(new Vector3(activeRenderers[activeRenderers.Count - 1].transform.position.x + activeRenderers[activeRenderers.Count - 1].bounds.size.x,
                                                        activeRenderers[activeRenderers.Count - 1].transform.position.y, activeRenderers[activeRenderers.Count - 1].transform.position.z),
                                                        Quaternion.identity, this.transform);
                    activeRenderers.Add(r);
                }
            }

        }
    }
}
