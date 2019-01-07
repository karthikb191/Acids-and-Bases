using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
<<<<<<< HEAD

public class HelpSystem : MonoBehaviour {

    static LevelHelpBase levelHelper;

    private void Start()
    {
        levelHelper = FindObjectOfType<LevelHelpBase>();
        //Try to find the helper for the scene when the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoad;
=======
using UnityEngine.UI;

public class HelpSystem : MonoBehaviour {

    public LevelHelpBase levelHelper;

    public Image backgroundSprite;

    public Text textToDisplay;

    bool startTimer;

   public Image hintTriggered;

    public float displayTime;

    public GameObject displayHelp;

    float timer;

    public Sprite hintTriggerSprite;
 

    private void Start()
    {
        levelHelper = GetComponentInChildren<LevelHelpBase>();
        //Try to find the helper for the scene when the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoad;

      
>>>>>>> df8913b80013a22f35e2765299dcfa2b0208e49e
    }
    
    public static void Help()
    {
<<<<<<< HEAD
        levelHelper.DisplayHelp();
=======
       // levelHelper.DisplayHelp();
>>>>>>> df8913b80013a22f35e2765299dcfa2b0208e49e
    }
    
    void OnSceneLoad(Scene s, LoadSceneMode mode)
    {
<<<<<<< HEAD
        levelHelper = FindObjectOfType<LevelHelpBase>();
    }

=======
     //   levelHelper = FindObjectOfType<LevelHelpBase>();
    }

    public void HelpDisplay(string helpMessage, Sprite bgSprite)
    {
        textToDisplay.text = helpMessage;
        backgroundSprite.sprite = bgSprite;

       
        
        Debug.Log("Triggered hint activated>>>>><<<><><><><>");
        hintTriggered.enabled = true;
        hintTriggered.sprite = hintTriggerSprite;

        Debug.Log(hintTriggered.sprite.name);
    }

   
    private void HelpTimer()
    {
        if (timer < displayTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            startTimer = false;
        }
    }

    private void Update()
    {
        if (startTimer && showHint)
            HelpTimer();
        else if(!startTimer && showHint)
            CloseHint();

    }


    public void CloseHint()
    {
        displayHelp.SetActive(false);
        timer = 0;
        startTimer = false;
        Debug.Log("bjjgjgj");
        showHint = false;
        hintTriggered.enabled = false;
        hintTriggered.sprite = null;

    }
    bool showHint = true;
    public void ShowHint()
    {
        displayHelp.SetActive(true);
        startTimer = true;
        showHint = true;
    }
>>>>>>> df8913b80013a22f35e2765299dcfa2b0208e49e
}
