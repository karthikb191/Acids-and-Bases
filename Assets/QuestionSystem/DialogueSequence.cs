using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum numberOfChoices{__2,__3,__4};

[System.Serializable]
public class Dialogues{
	public string dialogue;
	public bool isQuestion;
	public string[] answerOptions;
	public int currentAnswerVal;
	public numberOfChoices currentChoice=numberOfChoices.__2;
}
public class OptionSelected{
    public int sequenceNumber;
	public int dialogeNumber;
	public int selectedAnswer;
}

[System.Serializable]
public class DialogueSequence{
    public int correctAnswerSequenceIndex;
    public int wrongAnswerSequenceIndex;
    public List<ItemsDescription> itemsExpecting;
	public List<Dialogues> allDialogues=new List<Dialogues>();
}
