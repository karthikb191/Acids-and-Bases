using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudio : MonoBehaviour {

    public static AudioSource backgroundSound1;
    public static AudioSource backgroundSound2;

    private static bool changingTrack;
    bool disablingAudio = false;
	// Use this for initialization
	void Start () {
        backgroundSound1 = transform.Find("BackgroundSound1").GetComponent<AudioSource>();
        backgroundSound2 = transform.Find("BackgroundSound2").GetComponent<AudioSource>();
        //Initially, background1 has the highest volume and background 2 has 0 volume
        backgroundSound1.volume = 1;
        backgroundSound2.volume = 0;

        changingTrack = false;
        
	}
	
    public static IEnumerator ChangeTracks(AudioClip audioClip)
    {
        if (!changingTrack)
        {
            
            if (backgroundSound2.clip == null && audioClip != backgroundSound1.clip)
            {
                changingTrack = true;
                Debug.Log("Background sound 2 has no clip....So adding the clip");
                backgroundSound2.clip = audioClip;
                //Decrease the volume of background sound 1 and add the clip to the second one and increase it's volume
                float bg1Volume = backgroundSound1.volume;
                float bg2Volume = backgroundSound2.volume;
                while(bg1Volume > 0)
                {
                    yield return new WaitForFixedUpdate();
                    bg1Volume -= Time.fixedDeltaTime * 0.5f;
                    bg2Volume += Time.fixedDeltaTime * 2;

                    backgroundSound1.volume = bg1Volume;
                    backgroundSound2.volume = Mathf.Clamp(bg2Volume, 0, 0.8f);
                    if(!backgroundSound2.isPlaying)
                        backgroundSound2.Play();
                }
                //Remove the clip inside the background sound 1 after it's volume has been completely reduced
                backgroundSound1.clip = null;

            }
            else
            {
                if (backgroundSound1.clip == null && audioClip != backgroundSound2.clip)
                {
                    changingTrack = true;
                    Debug.Log("Background sound 2 has no clip....So adding the clip");
                    backgroundSound1.clip = audioClip;
                    //Decrease the volume of background sound 1 and add the clip to the second one and increase it's volume
                    float bg1Volume = backgroundSound1.volume;
                    float bg2Volume = backgroundSound2.volume;
                    
                    while (bg2Volume > 0)
                    {
                        yield return new WaitForFixedUpdate();
                        bg1Volume += Time.fixedDeltaTime * 2;
                        bg2Volume -= Time.deltaTime * 0.5f;

                        backgroundSound1.volume = Mathf.Clamp(bg1Volume, 0, 0.8f);
                        backgroundSound2.volume = bg2Volume;

                        if (!backgroundSound1.isPlaying)
                            backgroundSound1.Play();
                    }
                    //Remove the clip inside the background sound 1 after it's volume has been completely reduced
                    backgroundSound2.clip = null;

                }

            }
            changingTrack = false;
        }
        yield return null;
    }

    private void Update()
    {
        if (Goal.goalReached && !disablingAudio)
        {
            Goal.goalReached = false;
            disablingAudio = true;
            if(backgroundSound1.clip != null)
            {
                StartCoroutine(GameManager.Instance.FadeOutAudio(backgroundSound1));
            }
            if(backgroundSound2.clip != null)
            {
                StartCoroutine(GameManager.Instance.FadeOutAudio(backgroundSound2));
            }
        }
    }

}
