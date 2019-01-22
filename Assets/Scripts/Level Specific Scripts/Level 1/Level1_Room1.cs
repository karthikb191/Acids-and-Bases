using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScripts : MonoBehaviour
{
    protected CameraScript cameraScript;
    public bool playOnlyOnce = true;
    public bool playedOnce = false;
    protected static int triggerNumber = -1;
    
    public static void SetTriggerNumber(int num) { triggerNumber = num; }
    

    protected Player player;
}

public class Level1_Room1 : LevelScripts {

    public float waitDurationNearDoor = 1.0f;

    public float waitPeriodAtEnemyDoor = 1.0f;

    /// <summary>
    /// After waiting for the duration specified above, the camera focus starts
    /// </summary>
    GameObject focusObject;

    public List<GameObject> objectsToSpawn;
    Vector3 spawnAt;

    private void Start()
    {
        cameraScript = FindObjectOfType<CameraScript>();
        player = FindObjectOfType<Player>();
        if (GetComponent<Room>().initialRoom)
        {
            GameManager.Instance.FadeOutCompleteEvent += ShowHint;
        }
        Debug.Log("exec");
        focusObject = transform.Find("EnemyDoor").gameObject;
    }

    private void Update()
    {
        CheckDependencies();
    }
    void CheckDependencies()
    {
        if(focusObject == null)
        {
            Debug.Log("Focus object is null");
            focusObject = transform.Find("EnemyDoor").gameObject;
        }
        else
        {
            Debug.Log("focus object is not null" + focusObject.name);
        }

    }

    void ShowHint()
    {
        Debug.Log("Showing Hint Now");
        HelpSystem.Help();
        GameManager.Instance.FadeOutCompleteEvent -= ShowHint;

        //When player enters the trigger enter region, the trigger number is checked and the next sequence of events happens
        CinematicTrigger.CinematicTriggerEnterEvent += CheckTriggerNumber;
    }

    void CheckTriggerNumber()
    {
        if (triggerNumber == 1)
            CameraPan();
    }

    void CameraPan()
    {
        CinematicTrigger.CinematicTriggerEnterEvent -= CameraPan;
        Debug.Log("focus object: " + focusObject);
        StartCoroutine(StartCameraFocus(focusObject.transform.position, waitDurationNearDoor));
    }

    IEnumerator StartCameraFocus(Vector3 location, float timeToWait)
    {
        float waitPeriodAtEnemyDoor = 2.0f;
        float shakeTime = 3.0f;
        player.BlockInputs(timeToWait + waitPeriodAtEnemyDoor, true, false);

        yield return new WaitForSeconds(timeToWait);

        cameraScript.FocusCameraAt(location, 1.5f, shakeTime);
        InvokeRepeating("ShakeAtInterval", 0, shakeTime / 3);

        yield return new WaitForSeconds(shakeTime);

        Invoke("SpawnObjects", waitPeriodAtEnemyDoor / 2.0f);


        CancelInvoke("ShakeAtInterval");
        yield break;
    }
    void ShakeAtInterval()
    {
        cameraScript.ShakeCamera(0.78f, 1.32f, 0.739f);
    }

    void SpawnObjects()
    {
        if(objectsToSpawn != null)
        {
            spawnAt = focusObject.transform.position;
            for(int i = 0; i < objectsToSpawn.Count; i++)
            {
                objectsToSpawn[i].SetActive(true);
                //set the chasing behavior for these guys
                Invoke("StartChase", 1.0f);
            }
        }
    }
    void StartChase()
    {
        for(int i = 0; i < objectsToSpawn.Count; i++)
        {
            EnemyBehaviorAI.ForceChaseBehavior(objectsToSpawn[i].GetComponent<Enemy>(), player);
        }
    }

    private void OnDisable()
    {
        //Unsubscrible from all the events
        CinematicTrigger.CinematicTriggerEnterEvent -= CheckTriggerNumber;
    }
}
