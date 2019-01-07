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

    public Sprite helpTextBackGround;

    public Sprite hintTextBackGround;

    public List<HelpTrigger> helpTriggers = new List<HelpTrigger>();
   
    
    private void Start()
    {
        helpSystem = GetComponentInParent<HelpSystem>();

     
        foreach(HelpTrigger helpTriggerObj in GetComponentsInChildren<HelpTrigger>())
        {
            helpTriggers.Add(helpTriggerObj);
        }

    }

    public virtual void DisplayHelp(string helpMessage, Sprite background)
    {
        helpSystem.HelpDisplay(helpMessage,background);

    }

    public void TriggeredHelp()
    {
        for (int i = 0; i < helpTriggers.Count; i++)
        {
            if (helpTriggers[i].isActivated && !helpTriggers[i].isShown)
            {
                Debug.Log("Help message:  " + helpTriggers[i].helpMessage);

                DisplayHelp(helpTriggers[i].helpMessage, helpTextBackGround);
                helpTriggers[i].isActivated = false;

                //To show multiple times remove the below line
             //   helpTriggers[i].isShown = true;
                break;
            }
        }
    }

    public void HintDisplay()
    {
        DisplayHelp(hintsForTheLevel[hintNumber], hintTextBackGround);
    }
    
}
