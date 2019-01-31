using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_Room5 : LevelScripts {
    
    public List<Character> charactersToSpawn;

    // Use this for initialization
    private void OnEnable() {
        cameraScript = FindObjectOfType<CameraScript>();

        if (charactersToSpawn != null)
        {
            for(int i = 0; i < charactersToSpawn.Count; i++)
            {
                charactersToSpawn[i].gameObject.SetActive(false);
            }
        }
        CinematicTrigger.CinematicTriggerEnterEvent += CinematicEvents;
	}
	
    void CinematicEvents()
    {
        if(LevelScripts.triggerNumber == 0)
        {

        }
    }
}
