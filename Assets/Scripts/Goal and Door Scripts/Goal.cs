using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {
    public static bool goalReached = false;
    public static bool gameManagerCanIncreaseLevel = false;

    public string whatShouldIAllow;         //set it in the inspector
    public GameObject particleEffect;       //set the prefab in the inspector

    //Observations canvas variables
    public static bool canClickToContinue = false;
    public GameObject levelEndObservationsCanvas;

    Animator animator;
    private void Awake()
    {
        Debug.Log("trigger awakened");
    }

    void Start() {
        goalReached = false;

        animator = GetComponent<Animator>();
        //particleEffect = Instantiate(particleEffect);
        //particleEffect.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collider) {

        if (collider.tag == "tag_player") {

            goalReached = true;

            /*if(whatShouldIAllow == "")
            {
                //Fade out audio
                StartCoroutine(GameManager.Instance.FadeOutAudio(collider.GetComponent<AudioSource>()));
                
                
                StartCoroutine(GameManager.Instance.IncreaseLevelWithFade());

                if (levelEndObservationsCanvas != null)
                    StartCoroutine(ShowInstructions());
                else
                    gameManagerCanIncreaseLevel = true;

            }

            if (collider.GetComponent<PlayerMechanics>().whatAmI == whatShouldIAllow)
            {
                
                if (animator != null)
                {
                    Debug.Log("color Matched and you may pass");
                    animator.SetBool("Open", true);
                }
                
                Debug.Log("color Matched and you may pass");
                StartCoroutine(GameManager.Instance.FadeOutAudio(collider.GetComponent<AudioSource>())); //Fade out the player audio because it's convenient to acces player here
                StartCoroutine(GameManager.Instance.IncreaseLevelWithFade());

                //game maneger can only increase the level if the flag is set here
                if (levelEndObservationsCanvas != null)
                    StartCoroutine(ShowInstructions());
                else
                    gameManagerCanIncreaseLevel = true;
                
            }*/
            
        }
    }

    //IEnumerator ShowInstructions()
    //{
    //    yield return new WaitForSeconds(1);
    //    levelEndObservationsCanvas.SetActive(true);
    //    yield return new WaitForSeconds(4);
    //
    //    //levelEndObservationsCanvas.transform.GetChild(0).GetComponent<Image>().color = Color.white;
    //    ClickToContinue.clickToContinueImage.SetActive(true);
    //    Goal.canClickToContinue = true;
    //
    //    while (!ClickToContinue.clickToContinue)
    //        yield return new WaitForFixedUpdate();
    //
    //    levelEndObservationsCanvas.SetActive(false);
    //
    //    gameManagerCanIncreaseLevel = true;
    //
    //    canClickToContinue = false;
    //    ClickToContinue.clickToContinue = false;
    //
    //    yield break;
    //}


    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "tag_player")
        {
            if (animator != null)
                animator.SetBool("Open", false);
            
        }
    }

    //On collision, the level transition must happen
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    //If colliding with the door and door is open
    //    if (animator.GetBool("Open"))
    //    {
    //        if (collision.collider.tag == "tag_player")
    //        {
    //            //Block the inputs so that the player cant move anymore
    //            collision.collider.GetComponent<MovementScript2D>().BlockInputs(true, true, true, 5.0f);
    //
    //            
    //            if (collision.collider.GetComponent<PlayerMechanics>().whatAmI == whatShouldIAllow)
    //            {
    //                Debug.Log("color Matched and you may pass");
    //                StartCoroutine(GameManager.Instance.IncreaseLevelWithFade());
    //            }
    //
    //        }
    //    }
    //}

    //IEnumerator IncreaseLevel() {
    //    Debug.Log("Increasing level");
    //    particleEffect.SetActive(true);
    //    GameManager.Instance.finishCanvas.GetComponent<Animator>().SetBool("Activated", true);
    //    yield return new WaitForSeconds(2);
    //    GameManager.Instance.finishCanvas.GetComponent<Animator>().SetBool("Activated", false);
    //    yield return new WaitForSeconds(1);
    //    particleEffect.SetActive(false);
    //    GameManager.Instance.IncreaseLevel();
    //}
}
