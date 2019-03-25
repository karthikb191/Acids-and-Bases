using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour {
    public GameObject buttonPrefab;
    private GameObject dialogueUI;
    private Text paraText;
    private GameObject buttonHolder;

    public float charWait = 0.05f;
    public float nextDialogueWait = 0.2f;

    public GameObject[] allActors;
    

    public List<DialogueSequence> dialogueSequence;

    public int numberOfQuestions;

    public int currSequenceIndex = 0;
    public int currDialogueIndex = 0;

    private List<Dialogues> allDialoguesInCurrentSequence = new List<Dialogues>();

    bool dialoguePlaying = false;
    private bool isOver;
    private bool skip;
    private bool firstClick = false;
    private float nextClickTime;
    private float clickTimeGap = 1.0f;

    Player playerOnFocus;

    List<ItemsDescription> itemsObtained = new List<ItemsDescription>();

    void Start()
    {
        dialogueUI = GameManager.Instance.DialogueCanvas.transform.Find("DialoguePanel").gameObject;
        paraText = dialogueUI.transform.Find("DialogueText").GetComponent<Text>();
        buttonHolder = dialogueUI.transform.Find("ButtonHolder").gameObject;
        
        ChangeDialogueSequenceTo(currSequenceIndex);

        numberOfQuestions = GetNumberOfQuestions();
        //paraText.text = "";
        //AddDialogue("1 Hey Bitch", true, new string[] { "Nigger", "digger" }, 1);
        //AddDialogue("");
    }

    int GetNumberOfQuestions()
    {
        int count = 0;
        for(int i = 0; i < dialogueSequence.Count; i++)
        {
            for(int j = 0; j < dialogueSequence[i].allDialogues.Count; j++)
            {
                if (dialogueSequence[i].allDialogues[j].isQuestion)
                    count++;
            }
        }
        return count;
    }

    public bool IsDialoguePlaying() { return dialoguePlaying; }

    public void StartDialogue(Player p)
    {
        playerOnFocus = p;
        p.StopMovement();
        GameManager.Instance.DialogueCanvas.SetActive(true);
        BreakDialogue(allDialoguesInCurrentSequence[currDialogueIndex].dialogue);
        dialoguePlaying = true;
    }

    public void DialogueFinished()
    {
        Debug.Log("dialogue finished");
        playerOnFocus.ResumeMovement();
        GameManager.Instance.DialogueCanvas.SetActive(false);
        //Dialogue index must be reset once the dialogue has been delivered to start the dialogue again
        currDialogueIndex = 0;
        
        if (GameManager.Instance != null)
            GameManager.Instance.Resume();

        dialoguePlaying = false;
    }

    void AddDialogue(string dialg)
    {
        Dialogues d = new Dialogues();
        d.dialogue = dialg;
        allDialoguesInCurrentSequence.Add(d);
    }

    /// <summary>
    /// Adds options to dialogue also. This function needs changing if this is being extended
    /// </summary>
    void AddDialogue(string dialg, bool question, string[] options, int ans)
    {
        Dialogues d = new Dialogues();
        d.dialogue = dialg;
        d.isQuestion = question;
        d.answerOptions = options;
        d.currentAnswerVal = ans;
        allDialoguesInCurrentSequence.Add(d);
    }

    public bool haltDialogue = false;

    void Update()
    {
        if (currDialogueIndex >= allDialoguesInCurrentSequence.Count)
        {
            DialogueFinished();
        }

        if (!isOver && !haltDialogue)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (!firstClick)
                {
                    firstClick = true;
                    nextClickTime = Time.time + clickTimeGap;
                }
                else
                {
                    if (Time.time <= nextClickTime)
                    {
                        skip = true;
                        firstClick = false;
                        nextClickTime = Mathf.Infinity;
                        print("Skipped");
                    }
                }
            }
        }
        else
        {
            if (currDialogueIndex < allDialoguesInCurrentSequence.Count) {
                //If the current dialogue is not question, then display the next dialogue, or else wait until the 
                //Correct option is selected
                if (!allDialoguesInCurrentSequence[currDialogueIndex].isQuestion)
                {
                    Debug.Log("Not a question");
                    StartCoroutine(NextDialogue());
                    isOver = false;
                }
            }
        }
    }

    public void ChangeDialogueSequenceTo(int index, bool breakDialogue = false)
    {
        Debug.Log("Dialogue sequence changed to: " + index);
        isOver = false; skip = false;
        currSequenceIndex = index;
        currDialogueIndex = 0;
        allDialoguesInCurrentSequence = dialogueSequence[currSequenceIndex].allDialogues;
        if (breakDialogue)
            BreakDialogue(allDialoguesInCurrentSequence[0].dialogue);
    }

    public void ShowNextDialogue()
    {
        StartCoroutine(NextDialogue());
    }

    IEnumerator NextDialogue()
    {
        //Dialogue deactivation logic
        if (allDialoguesInCurrentSequence[currDialogueIndex].dialogue == "" || currDialogueIndex == allDialoguesInCurrentSequence.Count)
        {
            Debug.Log("Dialogue finished: " + currDialogueIndex);
            DialogueFinished();
            yield break;
        }

        firstClick = false;
        Debug.Log("Next Dialogue starting");
        //If the current dialogue is a question, then deactivate the buttons that have been activated
        if (allDialoguesInCurrentSequence[currDialogueIndex].isQuestion)
        {
            buttonHolder.SetActive(false);
        }

        yield return new WaitForSeconds(nextDialogueWait);
        currDialogueIndex++;    //Increment current dialogue

        Debug.Log("Incremented current index");

        //Preparing for the next dialogue
        if (currDialogueIndex < allDialoguesInCurrentSequence.Count)
        {
            paraText.text = "";
            isOver = false;
            BreakDialogue(allDialoguesInCurrentSequence[currDialogueIndex].dialogue);
        }
    }


    void RestartDialogue()
    {
        if (currDialogueIndex < allDialoguesInCurrentSequence.Count)
        {
            dialogueUI.SetActive(true);
            StartCoroutine(NextDialogue());
        }
    }


    void BreakDialogue(string dialg)
    {
        //Get the first occurance of space in the dialogue. This is used to identify the actors of the dialogue
        int firstSpace = dialg.IndexOf(" ");
        //Get the actor index ID by getting the substring

        int actorId = int.Parse(dialg.Substring(0, firstSpace));
        //Actor ID = Array Location + 1

        actorId -= 1;

        //Add the actor name to the string so that it displays the name of the actor in the dialogue
        //paraText.text = allActors[actorId].GetComponent<ActorScript>().actorName + " : ";
        paraText.text = allActors[actorId].name + " : ";

        //If the dialogue is a question, then the button options must be displayed

        if (allDialoguesInCurrentSequence[currDialogueIndex].isQuestion)
        {
            buttonHolder.SetActive(true);
            //int optionsLength = (int)allDialoguesInCurrentSequence[currDialogueIndex].currentChoice;

            for (int x = 0; x < allDialoguesInCurrentSequence[currDialogueIndex].answerOptions.Length; x++)
            {
                int temp = x;
                GameObject go = Instantiate(buttonPrefab);
                go.transform.SetParent(buttonHolder.transform);
                //The listener decides the logic that system must follow when answered correctly or wrongly

                go.GetComponent<Button>().onClick.AddListener(() => { AnswerMCQ(temp); });
                //Add the text to the buttons

                go.transform.GetChild(0).GetComponent<Text>().text = allDialoguesInCurrentSequence[currDialogueIndex].answerOptions[x];
            }
        }

        string dialogue = dialg.Substring(firstSpace + 1, dialg.Length - firstSpace - 1);

        //routine to animate lines of the dialogue
        StartCoroutine(WriteLines(dialogue, actorId));
    }



    IEnumerator WriteLines(string sentence, int actorId)
    {
        int index = 0;
        while (!skip && !isOver)
        {
            paraText.text += sentence[index];
            yield return new WaitForSeconds(charWait);
            index++;
            if (index == sentence.Length)
            {
                //Waiting for mouse input to go to next dialogue
                while (true)
                {
                    if (Input.GetMouseButtonDown(0))
                        break;
                    //Debug.Log("inner while");
                    yield return new WaitForFixedUpdate();
                }

                isOver = true;
                break;
            }
        }



        if (skip)
        {
            skip = false;
            if (!isOver)
            {
                paraText.text = allActors[actorId].name + " : " + sentence;
                while (true)
                {
                    if (Input.GetMouseButtonDown(0))
                        break;
                    Debug.Log("inner skip while");
                    yield return new WaitForFixedUpdate();
                }
                isOver = true;
                
            }
        }
    }



    OptionSelected AnswerMCQ(int i)
    {
        if (!isOver)
        {
            print("Wait till questsion is complete.");
            return null;
        }
        foreach (Transform child in buttonHolder.transform)
        {
            child.gameObject.SetActive(false);
        }

        print(i);

        if (i == allDialoguesInCurrentSequence[currDialogueIndex].currentAnswerVal)
        {
            if(dialogueSequence[currSequenceIndex].itemsExpecting.Count > 0)
            {
                if (CorrectAnswerEvent != null)
                    CorrectAnswerEvent();

                return null;
            }
            else
            {
                OptionSelected answer = AnswerOBJ(i);
                ChangeDialogueSequenceTo(dialogueSequence[currSequenceIndex].correctAnswerSequenceIndex);

                print("Correct Answer " + currDialogueIndex);


                //A Call to the subscribers that a correct answer has been recorded
                if (CorrectAnswerEvent != null)
                    CorrectAnswerEvent();

                //StartCoroutine(NextDialogue());
                BreakDialogue(allDialoguesInCurrentSequence[currDialogueIndex].dialogue);
                return answer;
            }
        }
        else
        {
            StartCoroutine(NextDialogue());
        }
        //Object with dialogue number and answer number
        return null;
    }

    public OptionSelected correctAnswer = null;
    public delegate void CorrectAnswer();
    public event CorrectAnswer CorrectAnswerEvent;

    OptionSelected AnswerOBJ(int i)
    {
        OptionSelected op = new OptionSelected();
        op.sequenceNumber = currSequenceIndex;
        op.dialogeNumber = currDialogueIndex;
        op.selectedAnswer = i;
        correctAnswer = op;
        return op;
    }

    public int GetCurrentSequenceIndex() { return currSequenceIndex; }

}
