using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    public Vector3[] cameraBounds
    {
        get
        {
            return CameraBounds;
        }
    }
    public float camHalfHeight;
    public float camHalfWidth;

    public float currentZValue;

    public bool keyboardControls;

    public bool paused;

    private float deltaTime;
    public float DeltaTime
    {
        get { return deltaTime; }
    }

    private Vector3[] CameraBounds = new Vector3[5];

    public GameObject fadeCanvasPrefab;
    [HideInInspector]
    public GameObject fadeCanvas;

    public GameObject globalCanvasPrefab;
    [HideInInspector]
    private GameObject globalCanvas;

    [HideInInspector]
    public VirtualJoystick virtualJoystick;

    private Camera mainCam;
    private GameObject player;
    private float camPlayerDistance;

    //fade events
    public delegate void FadeInStart();
    public event FadeInStart FadeInStartEvent;
    public delegate void FadeOutStart();
    public event FadeOutStart FadeOutStartEvent;
    public delegate void FadeOutComplete();
    public event FadeOutComplete FadeOutCompleteEvent;

    public List<Level> levelsCleared = new List<Level>();

    
    private void Awake()
    {
        //If the instance is null, then set the instance to this gameobject
        if (Instance == null)
        {
            Instance = this;
        }
        //If the instance already exists and is not this instance, then destroy this gameobject
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        
    }

    // Use this for initialization
    void Start () {
        virtualJoystick = FindObjectOfType<VirtualJoystick>();

        //Limiting frame rate to 40 fps
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        if (Application.isPlaying)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        currentZValue = 0;
        paused = false;

        mainCam = Camera.main;
        

        if (FindObjectOfType<MovementScript2D>() == null)
        {
            camPlayerDistance = 0;
        }
        else
        {
            player = FindObjectOfType<MovementScript2D>().gameObject.gameObject;
            camPlayerDistance = Vector3.Distance(mainCam.transform.position, new Vector3(mainCam.transform.position.x,
                                                                                            mainCam.transform.position.y,
                                                                                            player.transform.position.z));
        }

        
        //Debug.Log("sddds: " + SceneManager.GetActiveScene().buildIndex);
        //Debug.Log("sddds: " + virtualJoystick.gameObject.name);
        

        //Instantiate the fade canvas
        if(fadeCanvasPrefab != null)
        {
            fadeCanvas = Instantiate(fadeCanvasPrefab);
            fadeCanvas.SetActive(true);
            Debug.Log("Fade canvas instantiated");

            DontDestroyOnLoad(fadeCanvas);
            
        }
        //Instantiate the global canvas
        if(globalCanvasPrefab != null)
        {
            globalCanvas = Instantiate(globalCanvasPrefab);
            globalCanvas.SetActive(true);
            DeactivatePausePanel();

            //Assign proper functions to the pause panel
            globalCanvas.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(ActivatePausePanel);

            globalCanvas.transform.GetChild(1).gameObject.SetActive(false);
            globalCanvas.transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(DeactivatePausePanel);
            globalCanvas.transform.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(RestartLevel);
            globalCanvas.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(GoToMainMenu);
            globalCanvas.GetComponent<Canvas>().sortingOrder = 25;  //TODO: This needs to change to more intuitive method of representation
            DontDestroyOnLoad(globalCanvas);
        }

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if(virtualJoystick != null)
                virtualJoystick.gameObject.SetActive(false);
            if(globalCanvas!=null)
                globalCanvas.SetActive(false);
        }

        //Set up a function that is called everytime a scene is loaded
        //Deactivate the fade canvas.

        
        SceneManager.sceneLoaded += OnSceneLoad;
        SetCameraBounds();
    }
    
	// Update is called once per frame
	void Update () {
        //Debug.Log("sddds: " + virtualJoystick.gameObject.name);
        deltaTime = Time.deltaTime > 0.03f ? 0.03f : Time.deltaTime;
        //Debug.Log("Delta time is: " + deltaTime);

        SetCameraBounds();


	}


    public Level GetLevelData(int LevelNumber)
    {
        return levelsCleared[LevelNumber];
    }



    void OnSceneLoad(Scene scene, LoadSceneMode scenMode)
    {
        Debug.Log("New scene loaded");
        //Whenever a new level is loaded, the old camera is destroyed. So, we must assign a new camera
        mainCam = Camera.main;

        //When a scene is loaded, if the fade canvas is activated, then deacticate it
        if (fadeCanvas != null)
        {
            if(fadeCanvas.activeSelf)
                StartCoroutine(DeactivateFadeCanvas());
        }

        //If the player is null, try to grab the player
        //player = FindObjectOfType<MovementScript2D>().gameObject.gameObject;
        if (FindObjectOfType<MovementScript2D>() == null)
        {
            camPlayerDistance = 0;
        }
        else
        {
            player = FindObjectOfType<MovementScript2D>().gameObject.gameObject;
            camPlayerDistance = Vector3.Distance(mainCam.transform.position, new Vector3(mainCam.transform.position.x,
                                                                                            mainCam.transform.position.y,
                                                                                           player.transform.position.z));
        }

        // This function disables virtual joystick if the scene under test is the only active scene in build settings
        if (scene.buildIndex == 0 || scene.buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            //Check if there's a virtual joystick. If found, destroy it
            if (virtualJoystick != null)
                virtualJoystick.gameObject.SetActive(false);
            
            if (globalCanvas != null && scene.buildIndex == 0)
                globalCanvas.SetActive(false);
        }
        else
        {
            if(globalCanvas!=null)
                globalCanvas.SetActive(true);
            else
            {
                globalCanvas = Instantiate(globalCanvasPrefab);
                globalCanvas.SetActive(true);
            }

            if (virtualJoystick == null)
                virtualJoystick = FindObjectOfType<VirtualJoystick>();
            if (virtualJoystick != null)
            {
                virtualJoystick.gameObject.SetActive(true);

                VirtualJoystick.pickUpButton.SetActive(false);
                VirtualJoystick.itemSpecialActionButton.SetActive(false);
            }
        }
    }

    void SetCameraBounds()
    {
        if (mainCam.orthographic) {
            camHalfHeight = mainCam.orthographicSize;
            camHalfWidth = mainCam.aspect * camHalfHeight;
            // 0 for the center 
            CameraBounds[0] = mainCam.transform.position;
            //1 for top left
            CameraBounds[1] = new Vector3( mainCam.transform.position.x - camHalfWidth,
                                            mainCam.transform.position.y + camHalfHeight,
                                            mainCam.transform.position.z);
            //2 for top right
            CameraBounds[2] = new Vector3( mainCam.transform.position.x + camHalfWidth,
                                            mainCam.transform.position.y + camHalfHeight,
                                            mainCam.transform.position.z);
            //3 for bottom right
            CameraBounds[3] = new Vector3( mainCam.transform.position.x + camHalfWidth,
                                            mainCam.transform.position.y - camHalfHeight,
                                            mainCam.transform.position.z);
            //4 for bottom left
            CameraBounds[4] = new Vector3( mainCam.transform.position.x - camHalfWidth,
                                            mainCam.transform.position.y - camHalfHeight,
                                            mainCam.transform.position.z);
            Debug.DrawLine(Vector3.zero, CameraBounds[0]);
            Debug.DrawLine(Vector3.zero, CameraBounds[1]);
            Debug.DrawLine(Vector3.zero, CameraBounds[2]);
            Debug.DrawLine(Vector3.zero, CameraBounds[3]);
            Debug.DrawLine(Vector3.zero, CameraBounds[4]);
        }
        else
        {
            Debug.LogError("main camera is not orthographic");
        }

    }

    public IEnumerator GoToLevelWithFade(int index)
    {
        //set fade canvas to true
        fadeCanvas.SetActive(true);
        fadeCanvas.transform.GetChild(0).GetComponent<Animator>().SetBool("FadeIn", true);
        yield return new WaitForSeconds(fadeCanvas.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + 1.5f);
        
        SceneManager.LoadScene(index);
    }

    public IEnumerator FadeIn()
    {
        if(FadeInStartEvent != null)
            FadeInStartEvent();

        Instance.fadeCanvas.SetActive(true);
        Instance.fadeCanvas.transform.GetChild(0).GetComponent<Animator>().SetBool("FadeIn", true);
        
        float duration = Instance.fadeCanvas.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + 1.5f;
        yield return new WaitForSeconds(duration);

        StartCoroutine(Instance.DeactivateFadeCanvas());
    }
    
    public IEnumerator IncreaseLevelWithFade()
    {
        //set fade canvas to true
        if (fadeCanvas != null)
        {
            fadeCanvas.SetActive(true);
            fadeCanvas.transform.GetChild(0).GetComponent<Animator>().SetBool("FadeIn", true);
            yield return new WaitForSeconds(fadeCanvas.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + 1.5f);
        }

        //The flag must be set properly to increase the level
      /*  while (!Goal.gameManagerCanIncreaseLevel)
        {
            yield return new WaitForFixedUpdate();
        }
        Goal.gameManagerCanIncreaseLevel = false;*/
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }

    public IEnumerator DeactivateFadeCanvas()
    {
        if (FadeOutStartEvent != null)
            FadeOutStartEvent();

        Debug.Log("Deactivating Fade canvas");
        fadeCanvas.transform.GetChild(0).GetComponent<Animator>().SetBool("FadeIn", false);
        AnimatorStateInfo ass = fadeCanvas.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(ass.length);

        if (FadeOutCompleteEvent != null)
            FadeOutCompleteEvent();

        fadeCanvas.SetActive(false);
    }

    public void RestartLevel()
    {
        StartCoroutine(GoToLevelWithFade(SceneManager.GetActiveScene().buildIndex));
        DeactivatePausePanel();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoToMainMenu()
    {
        StartCoroutine(GoToLevelWithFade(0));
        DeactivatePausePanel();
        //SceneManager.LoadScene(0);
    }



    public void ActivatePausePanel()
    {
        Debug.Log("Activating the global canvas");
        globalCanvas.transform.GetChild(1).gameObject.SetActive(true);
        Time.timeScale = 0;
        
        paused = true;
    }
    public void DeactivatePausePanel()
    {
        globalCanvas.transform.GetChild(1).gameObject.SetActive(false);
        Time.timeScale = 1;
        paused = false;
    }

    public IEnumerator FadeOutAudio(AudioSource source)
    {
        Debug.Log("Audio source is: " + source);
        if (source != null)
        {
            while(source.volume > 0)
            {
                source.volume -= Time.fixedDeltaTime;
                Debug.Log("Fading out audio");
                yield return new WaitForFixedUpdate();
            }
        }
        yield break;
    }

    public IEnumerator FadeInAudio(AudioSource source)
    {
        
        if (source != null)
        {
            while (source.volume < 1)
            {
                source.volume += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        yield break;
    }

}
