using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    OptionSelected answer;

    private void Start()
    {
        DialogueSystem.CorrectAnswerEvent += CorrectAnswerObtained;
    }

    /// <summary>
    /// If a correct answer has been obtained, the dialogue system attached must be checked for the answer object
    /// </summary>
    void CorrectAnswerObtained()
    {
        answer = DialogueSystem.correctAnswer;
    }
}
