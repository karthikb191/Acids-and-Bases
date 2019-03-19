using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class ExpectedItem
{
    public int sequenceNumber;
    public int questionNumber;
    public bool itemExpected;
    public GameObject item;
}

[System.Serializable]
public class QuestionBoxSaveData : SaveData
{
    public string[] itemsExpecting;
    public bool unlocked;
    public int index;
    public int dialogueSequenceIndex;
}

[RequireComponent(typeof(BoxCollider2D))]
public class QuestionBox : MonoBehaviour {

    GameObject visual;

    public int index = 0;
    /// <summary>
    /// Must be set in the inspector incase a platform animation is required
    /// </summary>
    public GameObject blockingPlatform;
    public Vector3 targetLocalPosition;
    public Vector3 targetLocalRotation;

    public bool unlocked;

    //Prefab of the type of item expecting
    //public List<ItemsDescription> itemsExpecting = new List<ItemsDescription>();

    Vector3 initialLocalPosition;
    Vector3 initialLocalRotation;

    bool routineUnderProgress = false;

    DialogueSystem dialogueSystem;

    Player playerOnFocus = null;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!dialogueSystem.IsDialoguePlaying())
        {
            Player p = collider.GetComponent<Player>();
            if (p != null)
            {
                playerOnFocus = p;
                DynamicButton d = VirtualJoystick.CreateDynamicButton("tag_question");
                if (!d.active)
                {
                    VirtualJoystick.EnableDynamicButton(d);
                    d.button.onClick.AddListener(() =>
                    {
                        dialogueSystem.StartDialogue(playerOnFocus);
                    //Disable the button
                    VirtualJoystick.DisableDynamicButton(d);
                    });
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (playerOnFocus != null && collider.gameObject == playerOnFocus.gameObject)
        {
            playerOnFocus = null;
            if (dialogueSystem.IsDialoguePlaying())
            {
                dialogueSystem.DialogueFinished();
            }
            //Disable the button
            VirtualJoystick.DisableDynamicButton("tag_question");
        }
    }

    [HideInInspector]
    public QuestionBoxSaveData saveData;

    private void Start()
    {
        dialogueSystem = GetComponent<DialogueSystem>();
        visual = transform.Find("Sprite").gameObject;

        if(blockingPlatform != null)
        {
            initialLocalPosition = blockingPlatform.transform.localPosition;
            initialLocalRotation = blockingPlatform.transform.localRotation.eulerAngles;
        }

        dialogueSystem.CorrectAnswerEvent += CorrectAnswer;

        SaveManager.SaveEvent += Save;

        CheckPointManager.RegisterCheckPointEvent += Save;
        CheckPointManager.LoadCheckpointEvent += Load;
    }

    void Save(System.Type type)
    {
        saveData = new QuestionBoxSaveData();
        saveData.unlocked = unlocked;
        saveData.index = index;
        saveData.dialogueSequenceIndex = dialogueSystem.currSequenceIndex;

        //saveData.itemsExpecting = new string[itemsExpecting.Count];
        //for(int i = 0; i < itemsExpecting.Count; i++)
        //{
        //    saveData.itemsExpecting[i] = itemsExpecting[i].GetItemType().ToString();
        //}

        Debug.Log("Added to the save object list successfully");

        if(type.Equals(typeof(SaveManager)))
            SaveManager.saveObject.AddObject(saveData);

        else if(type.Equals(typeof(CheckPointManager)))
            CheckPointManager.checkPointData.AddObject(saveData);
    }

    void Load(System.Type type)
    {
        //Get the appropriate value from the save data
        //TODO: This part might need a little tweaking
        List<QuestionBoxSaveData> saveDatas = new List<QuestionBoxSaveData>();

        if (type.Equals(typeof(SaveManager)))
        {
            Debug.Log("Loading");
            for(int i = 0; i < SaveManager.saveObject.types.Count; i++)
            {
                if(SaveManager.saveObject.types[i].type == typeof(QuestionBoxSaveData).ToString())
                {
                    for (int j = 0; j < SaveManager.saveObject.types[i].values.Count; j++)
                    {
                        saveDatas.Add((QuestionBoxSaveData)SaveManager.saveObject.types[i].values[j]);
                    }
                    break;
                }
            }

            for(int i = 0; i < saveDatas.Count; i++)
            {
                if(saveDatas[i].index == index)
                {
                    LoadData(saveDatas[i]);
                    saveDatas.RemoveAt(i);
                }
            }
        }
        else
        {
            if (type.Equals(typeof(CheckPointManager)))
            {
                Debug.Log("Loading the checkpoint");
                for (int i = 0; i < CheckPointManager.checkPointData.types.Count; i++)
                {
                    if (CheckPointManager.checkPointData.types[i].type == typeof(QuestionBoxSaveData).ToString())
                    {
                        for(int j = 0; j < CheckPointManager.checkPointData.types[i].values.Count; j++)
                        {
                            saveDatas.Add((QuestionBoxSaveData)CheckPointManager.checkPointData.types[i].values[j]);
                        }
                        break;
                    }
                }

                for (int i = 0; i < saveDatas.Count; i++)
                {
                    if (saveDatas[i].index == index)
                    {
                        LoadData(saveDatas[i]);
                        saveDatas.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    }



    void LoadData(QuestionBoxSaveData data)
    {
        if (data.itemsExpecting != null)
        {
            for (int i = 0; i < data.itemsExpecting.Length; i++)
            {
                System.Object o;
                o = System.Enum.Parse(typeof(AcidsList), data.itemsExpecting[i]);
                if(o == null)
                {
                    o = System.Enum.Parse(typeof(BasesList), data.itemsExpecting[i]);
                }
                else if (o == null)
                {
                    o = System.Enum.Parse(typeof(IndicatorsList), data.itemsExpecting[i]);
                }
                else if (o == null)
                {
                    o = System.Enum.Parse(typeof(SaltsList), data.itemsExpecting[i]);
                }
                else if (o == null)
                {
                    o = System.Enum.Parse(typeof(NormalItemList), data.itemsExpecting[i]);
                }

                Debug.Log("e is: " + o.ToString());
                //itemsExpecting[i] = ItemManager.instance.itemDictionary[o].GetComponent<ItemsDescription>();
            }
        }

        unlocked = data.unlocked;
        dialogueSystem.ChangeDialogueSequenceTo (data.dialogueSequenceIndex);
    }


    bool prevAnswerState = false;

    private void Update()
    {
        //TODO: Remove this test check later
        if(prevAnswerState != unlocked && !routineUnderProgress)
        {
            if (unlocked)
                CorrectAnswer();
            else
                StartCoroutine(SimplePlatformInterpolation(initialLocalPosition, initialLocalRotation, true));

            prevAnswerState = unlocked;
        }
    }

    int correctAnswers = 0;
    public void CorrectAnswer()
    {
        Debug.Log("Correct Answer.....Checking for items");
        //TODO: Check the player's inventory and get the item
        if (!unlocked)
        {
            
            Player p = null;
            for (int i = 0; i < dialogueSystem.allActors.Length; i++)
            {
                p = dialogueSystem.allActors[i].GetComponent<Player>();
                if (p != null)
                    break;
            }
            if (p == null)
            {
                Debug.Log("There is no player added to the actors list. Add the player.");
                return;
            }

            if (dialogueSystem.dialogueSequence[dialogueSystem.GetCurrentSequenceIndex()].itemsExpecting.Count > 0)
            {
                //Enable the item selection canvas and wait for check
                InitiateItemSelection(p);
            }
            else
            {
                correctAnswers++;
            }

            if(correctAnswers == dialogueSystem.numberOfQuestions)
            {
                if (!routineUnderProgress)
                {
                    if (blockingPlatform != null)
                        StartCoroutine(SimplePlatformInterpolation(targetLocalPosition, targetLocalRotation));
                    else
                        GetComponent<BoxCollider2D>().enabled = false;

                    //correctAnswers = 0;
                }
            }

            return;
            //the item does not exist in the inventory
        }

        if (!routineUnderProgress)
        {
            if(blockingPlatform != null)
                StartCoroutine(SimplePlatformInterpolation(targetLocalPosition, targetLocalRotation));
            else
                GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    
    List<ItemsDescription> selectedItems = new List<ItemsDescription>();

    void InitiateItemSelection(Player p)
    {
        ItemSelection.Instance.ToggleActivation(p.GetComponentInChildren<PlayerInventory>(), this);
        dialogueSystem.haltDialogue = true;
    }


    public void SetSelectedItems(List<ItemsDescription> items)
    {
        Debug.Log("items count: " + items.Count);

        for(int i = 0; i < dialogueSystem.dialogueSequence[dialogueSystem.GetCurrentSequenceIndex()].itemsExpecting.Count; i++)
        {
            for(int j = 0; j < items.Count; j++)
            {
                Debug.Log("type 1: " + items[j].GetItemType() + "  Type 2: " + dialogueSystem.dialogueSequence[dialogueSystem.GetCurrentSequenceIndex()].itemsExpecting[i].GetItemType());
                if(items[j].GetItemType().Equals(dialogueSystem.dialogueSequence[dialogueSystem.GetCurrentSequenceIndex()].itemsExpecting[i].GetItemType()))
                {
                    bool itemExists = false;
                    //Check if selected items already has this
                    for(int k = 0; k < selectedItems.Count; k++)
                    {
                        if (selectedItems[k].GetItemType() == items[j].GetItemType())
                            itemExists = true;
                    }

                    if(!itemExists)
                        selectedItems.Add(items[i]);

                    break;
                    //TODO Remove this item from the inventory
                }
            }
        }

        //selectedItems = items;
        Debug.Log("selected items count: " + selectedItems.Count);
        CheckForRequiredItems();
    }

    void CheckForRequiredItems()
    {
        if (dialogueSystem.dialogueSequence[dialogueSystem.GetCurrentSequenceIndex()].itemsExpecting.Count > 0)
        {
            Debug.Log("1: " + dialogueSystem.dialogueSequence[dialogueSystem.GetCurrentSequenceIndex()].itemsExpecting.Count +
                      " 2: " + selectedItems.Count);
            if (dialogueSystem.dialogueSequence[dialogueSystem.GetCurrentSequenceIndex()].itemsExpecting.Count == selectedItems.Count)
            {
                correctAnswers++;

                if (correctAnswers == dialogueSystem.numberOfQuestions)
                {
                    if (!routineUnderProgress)
                    {
                        if (blockingPlatform != null)
                            StartCoroutine(SimplePlatformInterpolation(targetLocalPosition, targetLocalRotation));
                        else
                            GetComponent<BoxCollider2D>().enabled = false;

                        //correctAnswers = 0;
                    }
                }
                selectedItems.Clear();

                //Change the dialogue sequence to the correct answer sequence
                dialogueSystem.ChangeDialogueSequenceTo(dialogueSystem.dialogueSequence[dialogueSystem.GetCurrentSequenceIndex()].correctAnswerSequenceIndex, true);
            }
            else
            {
                dialogueSystem.ShowNextDialogue();
            }
        }
        //else
        //{
        //    dialogueSystem.ChangeDialogueSequenceTo(dialogueSystem.dialogueSequence[dialogueSystem.GetCurrentSequenceIndex()].correctAnswerSequenceIndex, true);
        //}
        //TODO: add the check logic here. If the conditions are met, the door unlocks
        dialogueSystem.haltDialogue = false;
    }

    
    IEnumerator SimplePlatformInterpolation(Vector3 targetPosition, Vector3 targetRotation, bool enableCollider = false)
    {
        float speed = 3.0f;
        routineUnderProgress = true;

        while (true)
        {
            Vector3 positionDirectionVector = targetPosition - blockingPlatform.transform.localPosition;
            Vector3 rotationDirectionVector = targetRotation - blockingPlatform.transform.localRotation.eulerAngles;
            Debug.Log("rotation direction vector: " + rotationDirectionVector.sqrMagnitude);

            bool positionCheck = false, rotationCheck = false;

            if (positionDirectionVector.sqrMagnitude < 0.3f)
            {
                positionCheck = true;
                blockingPlatform.transform.localPosition = targetPosition;
            }
            else
            {
                blockingPlatform.transform.localPosition += positionDirectionVector * speed * GameManager.Instance.DeltaTime;
            }

            if (rotationDirectionVector.sqrMagnitude < 2)
            {
                rotationCheck = true;
                blockingPlatform.transform.localRotation = Quaternion.Euler(targetRotation);
                Debug.Log("Equals");
            }
            else
            {
                Debug.Log("Not Equals");
                Quaternion q = blockingPlatform.transform.localRotation;
                blockingPlatform.transform.localRotation = Quaternion.Lerp(q, Quaternion.Euler(targetRotation), 0.1f);
            }


            if (positionCheck && rotationCheck)
                break;

            yield return new WaitForFixedUpdate();

        }
        routineUnderProgress = false;

        unlocked = !enableCollider;
        //Disable the collider on the game object that is not letting the player pass
        GetComponent<BoxCollider2D>().enabled = enableCollider;

        yield break;
    }


    private void OnDestroy()
    {
        SaveManager.SaveEvent -= Save;

        CheckPointManager.RegisterCheckPointEvent -= Save;

        CheckPointManager.LoadCheckpointEvent -= Load;
    }
}
