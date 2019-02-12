using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Dialogues{
	public string dialogue;
	public bool isQuestion;
	public Button[] answerOptions;
	public GameObject buttonHolder;
	public int currentAnswerVal;
}
public class OptionSelected{
	public int dialogeNumber;
	public int selectedAnswer;
}
public class DialogueSystem : MonoBehaviour {
	private bool isOver;
	private bool skip;
	private bool firstClick=false;
	private float nextClickTime;
	private float clickTimeGap=1.0f;
	public Text paraText;
	public GameObject[] allActors;
	public Dialogues[] allDialogues;
	public int currDialogueIndex=0;
	public float charWait=0.05f;
	public float nextDialogueWait=0.2f;
	void Start(){
		foreach(Dialogues d in allDialogues){
			if(d.isQuestion){
				for(int x=0;x<d.answerOptions.Length;x++){
					int temp=x;
					d.answerOptions[x].onClick.AddListener(() => {AnswerMCQ(temp);});
				}
			}
		}
		paraText.text="";
		BreakDialogue(allDialogues[currDialogueIndex].dialogue);
	}
	void Update(){
		if(!isOver){
			if(Input.GetMouseButtonUp(0)){
				if(!firstClick){
					firstClick=true;
					nextClickTime=Time.time+clickTimeGap;
				}else{
					if(Time.time<=nextClickTime){
						skip=true;
						firstClick=false;
						nextClickTime=Mathf.Infinity;
						print("Skipped");
					}
				}
			}
		}else{
			if(!allDialogues[currDialogueIndex].isQuestion){
				StartCoroutine(NextDialogue());
				isOver=false;
			}
		}
	}
	IEnumerator NextDialogue(){
		firstClick=false;
		if(allDialogues[currDialogueIndex].isQuestion){
			allDialogues[currDialogueIndex].buttonHolder.SetActive(false);
		}
		yield return new WaitForSeconds(nextDialogueWait);
		currDialogueIndex++;
		if(currDialogueIndex < allDialogues.Length){
			paraText.text="";
			isOver=false;
			BreakDialogue(allDialogues[currDialogueIndex].dialogue);
		}
	}
	void BreakDialogue(string dialg){
		int firstSpace=dialg.IndexOf(" ");
		int actorId=int.Parse(dialg.Substring(0,firstSpace));
		//Actor ID = Array Location + 1
		actorId-=1;
		paraText.text=allActors[actorId].GetComponent<ActorScript>().actorName+" : ";
		if(allDialogues[currDialogueIndex].isQuestion){
			allDialogues[currDialogueIndex].buttonHolder.SetActive(true);
		}
		string dialogue=dialg.Substring(firstSpace+1,dialg.Length-firstSpace-1);
		StartCoroutine(WriteLines(dialogue));
	}
	IEnumerator WriteLines(string sentence){
		int index=0;
		while(!skip && !isOver){
			paraText.text+=sentence[index];
			yield return new WaitForSeconds(charWait);
			index++;
			if(index==sentence.Length){
				isOver=true;
				break;
			}
		}
		if(skip){
			if(!isOver){
				paraText.text=sentence;
				isOver=true;
			}
		}
	}
	void AnswerMCQ(int i){
		if(!isOver){
			print("Wait till questsion is complete.");
			return;
		}
		print(i);
		if(i==allDialogues[currDialogueIndex].currentAnswerVal)
			print("Correct Answer");
		StartCoroutine(NextDialogue());		
		// Object with dialogue number and answer number
		OptionSelected answer=AnswerOBJ(i);
	}
	OptionSelected AnswerOBJ(int i){
		OptionSelected op=new OptionSelected();
		op.dialogeNumber=currDialogueIndex;
		op.selectedAnswer=i;
		return op;
	}
}
