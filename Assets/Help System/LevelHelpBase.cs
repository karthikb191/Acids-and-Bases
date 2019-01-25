using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHelpBase : MonoBehaviour {
    
    public virtual void DisplayHelp()
    {

    }
    public HelpSystem helpSystem;

    public List<string> hintsForTheLevel = new List<string>();

    public int hintNumber;

  //  public Sprite helpTextBackGround;

//    public Sprite hintTextBackGround;

    public List<HelpTrigger> helpTriggers = new List<HelpTrigger>();

    private void Start()
    {
        helpSystem = FindObjectOfType<HelpSystem>();
     
        foreach(HelpTrigger helpTriggerObj in GetComponentsInChildren<HelpTrigger>())
        {
            helpTriggers.Add(helpTriggerObj);
        }

    }

    public virtual void DisplayHelp(string helpMessage)
    {
        helpSystem.HelpDisplay(helpMessage);

    }

    public void TriggeredHelp()
    {
        for (int i = 0; i < helpTriggers.Count; i++)
        {
            if (helpTriggers[i].isActivated && !helpTriggers[i].isShown)
            {
                Debug.Log("Help message:  " + helpTriggers[i].helpMessage);

                DisplayHelp(helpTriggers[i].helpMessage);
                helpTriggers[i].isActivated = false;

                //To show multiple times remove the below line
                helpTriggers[i].isShown = true;

               

                if (helpTriggers[i].arrowPointer != null)
                {
                    helpSystem.arrowDisplay = helpTriggers[i].arrowPointer;
                }

                if (helpTriggers[i].isInstruction)
                {
                    helpSystem.ShowHint();
                }
                    break;
            }
        }
    }

    public void HintDisplay()
    {
        DisplayHelp(hintsForTheLevel[hintNumber]);
    }
    
}
