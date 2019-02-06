using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour {
    private Animator canvasanim;
    public static bool clicked;
	// Use this for initialization
	void Start () {
        canvasanim = GetComponent<Animator>();
        clicked = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Startgame()
    {
        if (!clicked && !GameManager.Instance.FadeInProgress()) {
            //canvasanim.SetBool("over", true);
            Nextscene();
            clicked = true;
        }
    }

    void Nextscene()
    {
        Debug.Log("startthegame");
        StartCoroutine(GameManager.Instance.IncreaseLevelWithFade());
    }
}
