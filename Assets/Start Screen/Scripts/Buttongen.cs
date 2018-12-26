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


    public RectTransform[] levelSelectPanels;

    public Vector3[] targetPositions;

    public GameObject levelPanelPrefab;
    public float offset;
    public Vector3 initialPosition;
    int numOfPanels;


    bool rightMoveFlag;
    bool leftMoveFlag;
    int targetReachCount;


    void Awake()
    {
       // sc_number = SceneManager.sceneCountInBuildSettings;
        sc_number = 100;
        numOfPanels = sc_number / 9 + 1;
        Debug.Log("number of scenes in build settings: " + sc_number);
    }

	// Use this for initialization
	void Start ()
    {
        //The scene unlocker function that runs every time a new scene is loaded
        SceneManager.sceneLoaded += SceneUnlocker;

        levelSelectPanels = new RectTransform[numOfPanels];
        targetPositions = new Vector3[numOfPanels];
        //If the player prefs has no key that is called LevelCounter, create it
        if (!PlayerPrefs.HasKey("LevelCounter"))
        {
            PlayerPrefs.SetInt("LevelCounter", 1);
            PlayerPrefs.Save();

            clearedLevels = (PlayerPrefs.GetInt("LevelCounter"));
            //first level is unlocked, every other level is locked
            //    CreateButtons(PlayerPrefs.GetInt("LevelCounter"));
        }
        //If it has the key, then instantiate the buttons
        else {

            clearedLevels = PlayerPrefs.GetInt("LevelCounter");
          //  CreateButtons(PlayerPrefs.GetInt("LevelCounter"));

        }

        LevelPanelCreate();
        CreatingButtons();
       
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
        int tempButtonCreated = 0;
        for (int i = 0; i < levelSelectPanels.Length; i++)
        {
            for (int j = 0; j < levelsPerPanel; j++,tempButtonCreated++)
            { 
                if(tempButtonCreated < sc_number)
                {
                    CreateButtons(levelSelectPanels[i], levelsPerPanel, clearedLevels,tempButtonCreated);
                }
                else
                {
                    break;
                }
            }
        }
    }





    public void CreateButtons(RectTransform parentObject, int levelPerPanel, int clearedNum,int buttonsCreated)
{
      //  for (int i = 0; i < levelPerPanel; i++)
        {
            if (buttonsCreated < clearedNum)
              {
                //These buttons are interactable and are assigned with a scene function each
                GameObject goButton = (GameObject)Instantiate(prefabButton);
                goButton.transform.SetParent(parentObject.transform, false);
                goButton.transform.localScale = new Vector3(1, 1, 1);

                Button tempButton = goButton.GetComponent<Button>();
                tempButton.interactable = true;

                Debug.Log("Value of i is: " + clearedNum);
                int n = clearedNum;
                goButton.GetComponent<Button>().onClick.AddListener(() => { ButtonClicked(n); });
                goButton.GetComponent<Button>().transform.GetChild(0).GetComponent<Text>().text = n.ToString();
            }
            else {
                //These buttons are not interactable
                GameObject goButton = (GameObject)Instantiate(prefabButton);
                goButton.transform.SetParent(parentObject.transform, false);
                goButton.transform.localScale = new Vector3(1, 1, 1);

                Button tempButton = goButton.GetComponent<Button>();
                tempButton.interactable = false;
                tempButton.transform.GetChild(0).GetComponent<Text>().text = (buttonsCreated + 1).ToString();

            }
        }
    }

	public void ButtonClicked(int index)
	{
        Debug.Log("button number is: " + index);
        StartCoroutine(GameManager.Instance.GoToLevelWithFade(index));
	}

///////////////////////////////////////////////////////////level panel/////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////level panel/////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////level panel/////////////////////////////////////////////////////////////////////////


    void LevelPanelCreate()
    {
        for(int i = 0; i< numOfPanels;i++)
        {
            
            GameObject temp = Instantiate(levelPanelPrefab) as GameObject;
            levelSelectPanels[i] = temp.GetComponent<RectTransform>();
            
            
            levelSelectPanels[i].gameObject.transform.position = new Vector3(initialPosition.x + i * (offset), initialPosition.y, initialPosition.z);
            levelSelectPanels[i].gameObject.transform.SetParent(ParentPanel,false);
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

                        if (levelSelectPanels[i].transform.localPosition.x >= targetPositions[i].x)
                        {
                            levelSelectPanels[i].transform.localPosition = targetPositions[i];
                            targetReachCount++;
                        }
                    }
                    else
                    {
                        continue;
                    }

                }

                if (targetReachCount == levelSelectPanels.Length - 1)
                {
                    leftMoveFlag = false;
                    targetReachCount = 0;
                }
    
        }
        

        if (rightMoveFlag)
        {
            for (int i = 0; i < levelSelectPanels.Length; i++)
            {
                if (levelSelectPanels[i].transform.localPosition.x > targetPositions[i].x)
                 {
                     levelSelectPanels[i].transform.localPosition = Vector3.Lerp(levelSelectPanels[i].transform.localPosition, targetPositions[i], 0.1f );
                     Vector3 direction = targetPositions[i] - levelSelectPanels[i].transform.localPosition;
                     Debug.Log(direction.x);
                     Debug.Log(targetPositions[i]);
                     if (levelSelectPanels[i].transform.localPosition.x <= targetPositions[i].x)
                     {
                         levelSelectPanels[i].transform.localPosition = targetPositions[i];
                         targetReachCount++;
                     }
                 }
                 else
                 {
                     continue;
                 }
            }

          if(targetReachCount == levelSelectPanels.Length-1)
          {
              rightMoveFlag = false;
              targetReachCount = 0;
          }
        }
    }

    public void MoveRight()
    {
        rightMoveFlag = true;

        for (int i = 0; i < levelSelectPanels.Length; i++)
        {
            targetPositions[i] = levelSelectPanels[i].transform.localPosition;
            targetPositions[i].x = levelSelectPanels[i].transform.localPosition.x - offset;
        }

    }
    
   public void MoveLeft()
    {
        leftMoveFlag = true;

        for (int i = 0; i < levelSelectPanels.Length; i++)
        {
            targetPositions[i] = levelSelectPanels[i].transform.localPosition;
            targetPositions[i].x = levelSelectPanels[i].transform.localPosition.x + offset;
        }
    }


    
}
