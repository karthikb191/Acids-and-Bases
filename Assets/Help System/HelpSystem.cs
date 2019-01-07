using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpSystem : MonoBehaviour {

    static LevelHelpBase levelHelper;

    private void Start()
    {
        levelHelper = FindObjectOfType<LevelHelpBase>();
        //Try to find the helper for the scene when the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    
    public static void Help()
    {
        levelHelper.DisplayHelp();
    }
    
    void OnSceneLoad(Scene s, LoadSceneMode mode)
    {
        levelHelper = FindObjectOfType<LevelHelpBase>();
    }

}
