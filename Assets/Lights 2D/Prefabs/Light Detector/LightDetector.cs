using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetector : MonoBehaviour
{
    [Tooltip("Do no set value in the inspector")]
    public bool absorbingLight;

    [HideInInspector]
    public List<GameObject> lightsInScene;

    //public LayerMask whatIsTheDetector;

    public ParticleSystem[] particleSystems;    //Assign in the inspector

    [HideInInspector]
    public Color lightColor;


    public string color;

    public float absorbTime = 5;

    private LightsManager levelManager;

    public List<GameObject> lightsInContact;

    public string targetColor = "Red";

    private bool colorAchieved = false;

    public delegate void ColorAchieved(string c);
    public event ColorAchieved ColorAchievedEvent;

    public delegate void StartAnimation(string c);
    public event StartAnimation StartAnimationEvent;

    public delegate void InterruptAnimation(string c);
    public event InterruptAnimation InterruptAnimationEvent;

    public delegate void FinishAnimation(string c);
    public event FinishAnimation FinishAnimationEvent;

    // Use this for initialization
    void Start()
    {
        lightsInScene = new List<GameObject>();
        lightsInContact = new List<GameObject>();

        levelManager = FindObjectOfType<LightsManager>();

        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].gameObject.SetActive(false);
        }

        //ColorAchievedEvent += TestDetector;
        
    }

    

    // Update is called once per frame
    void Update()
    {
        color = "";

        //make each light emmit a ray
        lightColor = Color.black;
        
        for (int i = 0; i < lightsInContact.Count; i++)
        {
            absorbingLight = true;

            Color c = lightsInContact[i].GetComponent<Lights2D>().lightColor;
            NamingColor();

            lightColor += new Color(c.r, c.g, c.b, 0);
            //particleSystems[0].startColor = lightColor;
            //
            //if (!particleSystems[0].gameObject.activeSelf)
            //    particleSystems[0].gameObject.SetActive(true);

        }

        if (lightsInContact.Count == 0)
        {
            //Debug.Log("Light is not striking the detector");
            particleSystems[0].gameObject.SetActive(false);

            //set absorbing of the level manager to false
            absorbingLight = false;
        }
        
        
        //check if the light color is matching with one of the goal colors.... Give it a little vaiance
        NamingColor();
        LightCheck();

        if (colorAchieved)
        {
            ColorAchievedEvent(targetColor);    //Target color has been achieved. so the respective event will be called now
            colorAchieved = false;
        }


        //Debug.Log("light color: " + lightColor);
    }

    private void LateUpdate()
    {
        lightsInContact.Clear();
    }

    public string GetCurrentLightAbsorbing()
    {
        return color;
    }

    void NamingColor()
    {

        if (lightColor.r == 1 && lightColor.g == 1 && lightColor.b == 1)
        {
            color = "White";
            return;
        }
        if (lightColor.r == 0 && lightColor.g == 0 && lightColor.b == 1)
        {
            color = "Blue";
            return;
        }
        if (lightColor.r == 1 && lightColor.g == 0 && lightColor.b == 0)
        {
            color = "Red";
            return;
        }
        if (lightColor.r == 0 && lightColor.g == 1 && lightColor.b == 0)
        {
            color = "Green";
            return;
        }
        if (lightColor.r == 1 && lightColor.g == 1 && lightColor.b == 0)
        {
            color = "Yellow";
            return;
        }
        if (lightColor.r == 0 && lightColor.g == 1 && lightColor.b == 1)
        {
            color = "Cyan";
            return;
        }
        if (lightColor.r == 1 && lightColor.g == 0 && lightColor.b == 1)
        {
            color = "Magenta";
            return;
        }
        
        //Debug.Log(lightColor);
    }

    private Coroutine c;
    void LightCheck()
    {
        if (absorbingLight)
        {
            if(color == targetColor && targetColor != "")
            {
                if (c == null)
                {
                    Debug.Log("Started coroutine");
                    c = StartCoroutine(DetectLight());
                }
            }
            else
            {
                if(c != null)
                {
                    //Event to reset animations

                    StopCoroutine(c);
                    InterruptAnimationEvent(colorToCheck);
                    colorToCheck = "";
                    c = null;
                    particleSystems[1].gameObject.SetActive(false);
                    particleSystems[2].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if(c != null)
                StopCoroutine(c);
            InterruptAnimationEvent(colorToCheck);
            colorToCheck = "";
            c = null;
            particleSystems[0].gameObject.SetActive(false);
            particleSystems[1].gameObject.SetActive(false);
            particleSystems[2].gameObject.SetActive(false);
        }

    }

    bool showParticleEffects = false;
    public bool ShowParticleEffects { set { showParticleEffects = value; } }
    string colorToCheck;

    IEnumerator DetectLight()
    {
        colorToCheck = targetColor;
        if (absorbingLight)
        {
            if (showParticleEffects)
            {
                ParticleSystem.MainModule particleModule0 = particleSystems[0].main;
                particleModule0.startColor = lightColor;

                yield return new WaitForSeconds(absorbTime / 2.0f);
                ParticleSystem.MainModule particleModule1 = particleSystems[1].main;
                ParticleSystem.MainModule particleModule2 = particleSystems[2].main;

                //lightDetector.particleSystems[0].gameObject.SetActive(false);
                particleSystems[1].gameObject.SetActive(true);
                particleModule1.startColor = lightColor;
                particleModule1.simulationSpeed = 1.5f;     //Change the speed here
                float timeToWait = (particleSystems[1].main.duration + particleSystems[1].main.startLifetime.constant) / particleModule1.simulationSpeed;

                yield return new WaitForSeconds(timeToWait / 1.5f);

                //lightDetector.particleSystems[1].gameObject.SetActive(false);
                particleSystems[2].gameObject.SetActive(true);
                particleModule2.startColor = lightColor;
                particleModule2.simulationSpeed = 1.0f;     //Change the speed here
                timeToWait = particleSystems[2].main.duration + particleSystems[2].main.startLifetime.constant / particleModule2.simulationSpeed;


                yield return new WaitForSeconds(timeToWait);

                particleSystems[1].gameObject.SetActive(false);
                particleSystems[2].gameObject.SetActive(false);

                //set the absorbing to false again 

                absorbingLight = false;
                colorAchieved = true;
                //targetColor = "";
                c = null;
                yield break;
            }
            else
            {
                //Wait for a while
                yield return new WaitForSeconds(1.0f);
                StartAnimationEvent(colorToCheck);
                yield return new WaitForSeconds(2.0f);
                FinishAnimationEvent(colorToCheck);
                colorAchieved = true;
                absorbingLight = false;
                c = null;
                yield break;
            }
        }
        else
        {
            InterruptAnimationEvent(colorToCheck);
            colorToCheck = "";
            c = null;
            //Debug.Log("Exiting the coroutine");
            yield break;
        }
    }

}


