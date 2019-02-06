using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_Room5 : LevelScripts {
    
    public List<Character> charactersToSpawn;

    public GameObject cageLookAtObject;
    public float waitPeriodAfterLookingAtCages = 5.0f;

    public GameObject enemyEntrancePoint;

    // Use this for initialization
    private void OnEnable() {
        cameraScript = FindObjectOfType<CameraScript>();
        player = FindObjectOfType<Player>();

        if (GetComponent<Room>())
        {
            if (GetComponent<Room>().chaseMustContinueToThisRoom)
                GetComponent<Room>().chaseMustContinueToThisRoom = false;
        }

        if (charactersToSpawn != null)
        {
            for(int i = 0; i < charactersToSpawn.Count; i++)
            {
                charactersToSpawn[i].gameObject.SetActive(false);
            }
        }
        CinematicTrigger.CinematicTriggerEnterEvent += CinematicEvents;
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (GameManager.Instance.IsPaused())
                GameManager.Instance.Resume();
            else
                GameManager.Instance.Pause();
        }
    }

    void CinematicEvents()
    {
        if(LevelScripts.triggerNumber == 0)
        {
            float durationToWaitAtCages = 3.0f;
            cameraScript.FocusCameraAt(cageLookAtObject.transform.position, 2, durationToWaitAtCages);
            StartCoroutine(StageTwo(durationToWaitAtCages));
        }
    }

    int charactersCaught = 0;
    IEnumerator StageTwo(float durationOfwaitAtCages)
    {
        float extraBufferPeriod = 3.0f;
        yield return new WaitForSeconds(durationOfwaitAtCages + extraBufferPeriod);

        //Spawn first character
        float durationToWaitNearEnemy = 1.0f;
        Character c = charactersToSpawn[0];
        c.gameObject.SetActive(true);
        c.gameObject.transform.position = enemyEntrancePoint.transform.position;

        //Resume the movement just in case if it's paused
        if (c.GetComponent<Enemy>())
            c.GetComponent<Enemy>().ResumeMovement();

        EnemyBehaviorAI.ForceChaseBehavior(c.GetComponent<Enemy>(), player);
        charactersToSpawn.Remove(c);
        cameraScript.ChangeCameraProjectionSize(15, 3);
        cameraScript.FocusCameraAt(enemyEntrancePoint.transform.position, 3, durationToWaitNearEnemy);

        CageTrap.CharacterCaughtEvent += StageThree;
    }
    
    void StageThree()
    {
        charactersCaught++;
        CageTrap.CharacterCaughtEvent -= StageThree;
        StartCoroutine(StageThreeRoutine());
    }

    IEnumerator StageThreeRoutine()
    {
        float initialWaitDuration = 2.0f;
        yield return new WaitForSeconds(initialWaitDuration);

        //Spawn Second character
        float durationToWaitNearEnemy = 1.0f;
        Character c = charactersToSpawn[0];
        c.gameObject.SetActive(true);
        c.gameObject.transform.position = enemyEntrancePoint.transform.position;

        //Resume the movement just in case if it's paused
        if (c.GetComponent<Enemy>())
            c.GetComponent<Enemy>().ResumeMovement();

        EnemyBehaviorAI.ForceChaseBehavior(c.GetComponent<Enemy>(), player);
        charactersToSpawn.Remove(c);

        cameraScript.FocusCameraAt(enemyEntrancePoint.transform.position, 3, durationToWaitNearEnemy);

        CageTrap.CharacterCaughtEvent += StageFour;
    }

    void StageFour()
    {
        charactersCaught++;
        CageTrap.CharacterCaughtEvent -= StageFour;
    }

    private void OnDisable()
    {
        CinematicTrigger.CinematicTriggerEnterEvent -= CinematicEvents;
    }

    private void OnDestroy()
    {
        CinematicTrigger.CinematicTriggerEnterEvent -= CinematicEvents;
    }
}
