using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExpectedItem
{
    public int sequenceNumber;
    public int questionNumber;
    public bool itemExpected;
    public GameObject item;
}

[RequireComponent(typeof(BoxCollider2D))]
public class QuestionBox : MonoBehaviour {

    GameObject visual;

    /// <summary>
    /// Must be set in the inspector incase a platform animation is required
    /// </summary>
    public GameObject blockingPlatform;
    public Vector3 targetLocalPosition;
    public Vector3 targetLocalRotation;

    public bool unlocked;

    //Prefab of the type of item expecting
    public List<ItemsDescription> itemsExpecting = new List<ItemsDescription>();

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
                        dialogueSystem.StartDialogue();
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

            //Disable the button
            VirtualJoystick.DisableDynamicButton("tag_question");
        }
    }

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
    

    public void CorrectAnswer()
    {
        Debug.Log("Correct Answer.....Checking for items");
        //TODO: Check the player's inventory and get the item
        if (itemsExpecting.Count > 0)
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

            //Enable the item selection canvas and wait for check
            InitiateItemSelection(p);
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
        selectedItems = items;
        Debug.Log("selected items: " + selectedItems.Count);
        CheckForRequiredItems();
    }

    void CheckForRequiredItems()
    {
        //TODO: add the check logic here. If the conditions are met, the door unlocks
        dialogueSystem.haltDialogue = false;

        if (!routineUnderProgress)
        {
            if (blockingPlatform != null)
                StartCoroutine(SimplePlatformInterpolation(targetLocalPosition, targetLocalRotation));
            else
                GetComponent<BoxCollider2D>().enabled = false;
        }
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

        //Disable the collider on the game object that is not letting the player pass
        GetComponent<BoxCollider2D>().enabled = enableCollider;

        yield break;
    }

}
