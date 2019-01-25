using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HelpSystem : MonoBehaviour {

    public static HelpSystem Instance { get; set; }

    public LevelHelpBase levelHelper;

   // public Image backgroundSprite;

    public Text textToDisplay;

    bool startTimer;

    public Image hintTriggered;

    public float displayTime;

    public GameObject displayHelp;

    public GameObject arrowDisplay;

    float timer;

 //   public Sprite hintTriggerSprite;

    public delegate void StartedShowingHint();
    public static StartedShowingHint startedShowingHintEvent;

    public delegate void FinishedShowingHint();
    public static FinishedShowingHint FinishedShowingHintEvent;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
     //   levelHelper = GetComponentInChildren<LevelHelpBase>();
        //Try to find the helper for the scene when the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoad;


      
    }



    void OnSceneLoad(Scene s, LoadSceneMode mode)
    {
        levelHelper = FindObjectOfType<LevelHelpBase>();

        Debug.Log("found");

    }
    

    public void HelpDisplay(string helpMessage)
    {
        textToDisplay.text = helpMessage;
       // backgroundSprite.sprite = bgSprite;       
        Debug.Log("Triggered hint activated>>>>><<<><><><><>");
        hintTriggered.enabled = true;
       // hintTriggered.sprite = hintTriggerSprite;

        //Debug.Log(hintTriggered.sprite.name);
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
        GameManager.Instance.Resume();

        // displayHelp.SetActive(false);
        displayHelp.GetComponent<Animator>().SetBool("PopUp", false);
        timer = 0;
        startTimer = false;
        Debug.Log("bjjgjgj");
        showHint = false;

        hintTriggered.enabled = false;
       // hintTriggered.sprite = null;


        if (arrowDisplay != null)
        {
            arrowDisplay.SetActive(false);
        }

        arrowDisplay = null;

        //Calls the event when the hint had finished displaying
        if (FinishedShowingHintEvent != null)
            FinishedShowingHintEvent();
    }
    bool showHint = false;
    public void ShowHint()
    {
        // displayHelp.SetActive(true);
        GameManager.Instance.Pause();
        displayHelp.GetComponent<Animator>().SetBool("PopUp", true);

        startTimer = true;
        showHint = true;

        if (arrowDisplay != null)
            arrowDisplay.SetActive(true);

        if(startedShowingHintEvent != null)
            startedShowingHintEvent();
    }
    public static void Help()
    {
        Debug.Log("help Displayed");
        Instance.ShowHint();
    }
}
