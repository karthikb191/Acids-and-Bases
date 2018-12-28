using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Buttongen : MonoBehaviour
{

    public GameObject prefabButton;
    public RectTransform ParentPanel;

    public int levelsPerPanel;
    private int sc_number;
    int clearedLevels;

    int buttonsCreated = 0;


    int screenCount = 0;

    public RectTransform[] levelSelectPanels;

    public Vector3[] targetPositions;

    public GameObject levelPanelPrefab;
    public float offset;
    public Vector3 initialPosition;
    int numOfPanels;

    public Button rightButton;
    public Button leftButton;

    bool rightMoveFlag;
    bool leftMoveFlag;
    int targetReachCount;
    bool buttonDisbled;

    int levelSelected;
    //level Info display 
    
    public Text bestTime;
    public Text targetSalts;
    public Text saltsCollected;

    int starCount;

    public GameObject levelInfoDisplay;

    CameraAnim cameraScript;

    public Image[] starImages;

    public void StartGame()
    {
        StartCoroutine(GameManager.Instance.GoToLevelWithFade(levelSelected));
    }

    public void CloseDisplay()
    {
     //   levelInfoDisplay.SetActive(false);
    }

    public void DisplayInfo(int levelNumber)
    {
       levelInfoDisplay.SetActive(true);

        //Get level data 

        if(levelSelected > levelNumber)
        {

        }
      //  Level levelData = GameManager.Instance.GetLevelData(levelSelected - 1);

   //     Debug.Log(levelData.targetSalts);
        //   bestTime.text = "" + levelData.bestTime;

        //     starCount = levelData.starsCollected;

        starCount = 5;

        for(int i = 0;i<starCount;i++)
        {
            starImages[i].GetComponent<Animator>().SetBool("StarAnimate", true);
            Color tempColor = starImages[i].color;
            tempColor.a = 1f;
            starImages[i].color = tempColor;

        }
        for (int i = starCount; i < starImages.Length; i++)
        {
           
            Color tempColor = starImages[i].color;
            tempColor.a = 0.3f;
            starImages[i].color = tempColor;
            starImages[i].GetComponent<Animator>().SetBool("StarAnimate", false);

        }


    }
    [SerializeField]
    GameObject starPrefab;
    [SerializeField]
    Transform starParent;
    [SerializeField]
    int maxStarCount = 20;

    public void CreateStars()
    {
        for (int i = 0; i < maxStarCount; i++)
        {
            GameObject temp = Instantiate(starPrefab) as GameObject;
            starImages[i] = temp.GetComponent<Image>();
            starImages[i].transform.SetParent(starParent);
            starImages[i].transform.localScale = Vector3.one;
        }
    }




 /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        // sc_number = SceneManager.sceneCountInBuildSettings;
        sc_number = 47;
        numOfPanels = sc_number / 9 + 1;
        Debug.Log("number of scenes in build settings: " + sc_number);
    }

    // Use this for initialization
    void Start()
    {
        //The scene unlocker function that runs every time a new scene is loaded
        SceneManager.sceneLoaded += SceneUnlocker;

        levelSelectPanels = new RectTransform[numOfPanels];
        targetPositions = new Vector3[numOfPanels];

        starImages = new Image[maxStarCount];

        //If the player prefs has no key that is called LevelCounter, create it
        if (!PlayerPrefs.HasKey("LevelCounter"))
        {
            PlayerPrefs.SetInt("LevelCounter", 1);
            PlayerPrefs.Save();


            clearedLevels = (PlayerPrefs.GetInt("LevelCounter"));

            //first level is unlocked, every other level is locked

        }
        //If it has the key, then instantiate the buttons
        else
        {

            clearedLevels = PlayerPrefs.GetInt("LevelCounter");


        }
        cameraScript = FindObjectOfType<CameraAnim>();
        LevelPanelCreate();
        CreatingButtons();
        CreateStars();

    }

    void SceneUnlocker(Scene scene, LoadSceneMode load)
    {
        Debug.Log("The scene unlocker is working.....Yaaayyyyyyy");
        if (scene.buildIndex > PlayerPrefs.GetInt("LevelCounter"))
        {
            PlayerPrefs.SetInt("LevelCounter", PlayerPrefs.GetInt("LevelCounter") + 1);
        }
    }

    void CreatingButtons()
    {

        for (int i = 0; i < levelSelectPanels.Length; i++)
        {
            if (buttonsCreated < sc_number)
            {
                CreateButtons(levelSelectPanels[i], levelsPerPanel, clearedLevels);
            }
        }
    }

    public void CreateButtons(RectTransform parentObject, int levelPerPanel, int clearedNum)
    {
        for (int i = 0; i < levelPerPanel; i++)
        {
            if (buttonsCreated < sc_number)
            {

                if (buttonsCreated < clearedNum)
                {
                    //These buttons are interactable and are assigned with a scene function each
                    GameObject goButton = (GameObject)Instantiate(prefabButton);
                    goButton.transform.SetParent(parentObject.transform, false);
                    goButton.transform.localScale = new Vector3(1, 1, 1);

                    Button tempButton = goButton.GetComponent<Button>();
                    tempButton.interactable = true;

                    int n = buttonsCreated + 1;
                    goButton.GetComponent<Button>().onClick.AddListener(() => { ButtonClicked(n);
                            cameraScript.GoLeft();


                    });
                    tempButton.transform.GetChild(0).GetComponent<Text>().text = n.ToString();
                    buttonsCreated++;
                }
                else
                {
                    //These buttons are not interactable
                    GameObject goButton = (GameObject)Instantiate(prefabButton);
                    goButton.transform.SetParent(parentObject.transform, false);
                    goButton.transform.localScale = new Vector3(1, 1, 1);

                    Button tempButton = goButton.GetComponent<Button>();
                    tempButton.interactable = false;
                    tempButton.transform.GetChild(0).GetComponent<Text>().text = (buttonsCreated + 1).ToString();
                    buttonsCreated++;

                }
            }
        }
    }

    public void ButtonClicked(int index)
    {
        Debug.Log("button number is: " + index);
        //  StartCoroutine(GameManager.Instance.GoToLevelWithFade(index));

        levelSelected = index;
        DisplayInfo(index);
    }

    void LevelPanelCreate()
    {
        for (int i = 0; i < numOfPanels; i++)
        {

            GameObject temp = Instantiate(levelPanelPrefab) as GameObject;
            levelSelectPanels[i] = temp.GetComponent<RectTransform>();


            levelSelectPanels[i].gameObject.transform.position = new Vector3(initialPosition.x + i * (offset), initialPosition.y, initialPosition.z);
            levelSelectPanels[i].gameObject.transform.SetParent(ParentPanel, false);
        }
    }


    private void Update()
    {
        if (leftMoveFlag)
        {
            Debug.Log("left move called");

            for (int i = 0; i < levelSelectPanels.Length; i++)
            {


                if (levelSelectPanels[i].transform.localPosition.x < targetPositions[i].x)
                {
                    levelSelectPanels[i].transform.localPosition = Vector3.Lerp(levelSelectPanels[i].transform.localPosition, targetPositions[i], 0.1f);
                    Vector3 direction = targetPositions[i] - levelSelectPanels[i].transform.localPosition;

                    if (Vector3.Distance(levelSelectPanels[i].transform.localPosition, targetPositions[i]) < 0.5f)
                    {
                        levelSelectPanels[i].transform.localPosition = targetPositions[i];
                        targetReachCount++;
                        Debug.Log("Left move" + targetReachCount);
                    }
                }
                else
                {
                    continue;
                }

            }

            if (targetReachCount >= levelSelectPanels.Length)
            {

                leftMoveFlag = false;
                targetReachCount = 0;
                buttonDisbled = false;
            }

            if (buttonDisbled)
            {
                Debug.Log("Removed block");
                rightButton.interactable = false;
                leftButton.interactable = false;
            }

            if (screenCount == 0)
            {
                leftButton.interactable = false;
            }

        }

        if (rightMoveFlag)
        {
            for (int i = 0; i < levelSelectPanels.Length; i++)
            {
                if (levelSelectPanels[i].transform.localPosition.x > targetPositions[i].x)
                {
                    levelSelectPanels[i].transform.localPosition = Vector3.Lerp(levelSelectPanels[i].transform.localPosition, targetPositions[i], 0.1f);
                    Vector3 direction = targetPositions[i] - levelSelectPanels[i].transform.localPosition;
                    //  Debug.Log(direction.x);
                    //  Debug.Log(targetPositions[i]);

                    Debug.Log("Right move" + targetReachCount);

                    if (Vector3.Distance(levelSelectPanels[i].transform.localPosition, targetPositions[i]) < 0.5f)
                    {
                        levelSelectPanels[i].transform.localPosition = targetPositions[i];
                        targetReachCount++;
                        Debug.Log("Right move" + targetReachCount);
                    }
                }
                else
                {
                    continue;
                }
            }

            if (targetReachCount >= levelSelectPanels.Length)
            {
                Debug.Log("Removed block");
                rightMoveFlag = false;
                targetReachCount = 0;
                buttonDisbled = false;

            }
        }
    }
    public void MoveRight()
    {
        if (!buttonDisbled && screenCount < levelSelectPanels.Length - 1)
        {
            buttonDisbled = true;
            rightMoveFlag = true;
            for (int i = 0; i < levelSelectPanels.Length; i++)
            {
                targetPositions[i] = levelSelectPanels[i].transform.localPosition;
                targetPositions[i].x = levelSelectPanels[i].transform.localPosition.x - offset;
            }
            screenCount++;
        }
    }

    public void MoveLeft()
    {
        if (!buttonDisbled && screenCount > 0)
        {
            buttonDisbled = true;
            leftMoveFlag = true;
            for (int i = 0; i < levelSelectPanels.Length; i++)
            {
                targetPositions[i] = levelSelectPanels[i].transform.localPosition;
                targetPositions[i].x = levelSelectPanels[i].transform.localPosition.x + offset;
            }
            screenCount--;
        }
    }



}
