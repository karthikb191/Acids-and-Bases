using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Try making this script independent of monoBehavior
public class PlayerMechanics : CharacterMechanics
{

    //Item variables
    public bool itemPickUpEnabled = true;
    public GameObject itemPickedUp;

    //Transformation variables
    public string whatAmI = "Haldi";
    public Color haldiColor;
    public Color kumkumColor;

    //Enemy Absorb variables
    public bool canAbsorbEnemy = true;
    public float absorbDistance = 1.0f;
    private DynamicButton absorbButton = null;
    private bool absorbing = false;

    public bool isInContactwithField;

    private AudioSource playerAudioSource;
    private Player player;

    float fallShoutCounter;

    private void Start()
    {
        base.Start();
        player = GetComponent<Player>();
        
        //playerAudioSource = GetComponent<AudioSource>();
        itemPickedUp = null;   //Initially, there are no item pick ups
        //StartCoroutine(Transformation());
    }
    private new void Update()
    {
        base.Update();
        
        if (canAbsorbEnemy)
        {
            //if (player.userInputs.absorbPressed) {

                //Debug.Log("absorb pressed:" + player.userInputs.absorbPressed);
                AbsorbCharacter();
            //}
        }
        //SetVirtualControls();
    }
    private void LateUpdate()
    {
        isInContactwithField = false;
    }
    
    //Will be called by Liquid
    public IEnumerator Transformation()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("transforming the player");
        if (whatAmI == "Haldi")
        {
            whatAmI = "Kumkum";
            Vector4 presentColor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            Vector4 targetColor = kumkumColor;
            while ( (presentColor - targetColor).sqrMagnitude > 0.1f ){
                presentColor = Vector4.Lerp(presentColor, targetColor, 2 * Time.deltaTime);
                //Change the sprite color now
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = presentColor;
                yield return new WaitForFixedUpdate();
            }
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = kumkumColor;
            
        }
        else if(whatAmI == "Kumkum")
        {
            whatAmI = "Haldi";
            Debug.Log("transforming to kumkum");
            Vector4 presentColor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            Vector4 targetColor = haldiColor;
            while ((presentColor - targetColor).sqrMagnitude > 0.1f)
            {
                presentColor = Vector4.Lerp(presentColor, targetColor, 2 * Time.deltaTime);
                //Change the sprite color now
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = presentColor;
                yield return new WaitForFixedUpdate();
            }
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = haldiColor;
            
        }
      //  Liquid.TransformationCoroutine = null;
        yield return null;

    }
    
    void FallSoundCondition()
    {
        if (itemPickedUp != null)
        {
            if (itemPickedUp.GetComponent<PickUpItem>().item == Item.parachute)
            {
                if (itemPickedUp.GetComponent<PickUpItem>().itemAnimator.GetBool("Open"))
                    fallShoutCounter = 0;
            }
        }

        //if (player.State == PlayerStates.falling)
        //{
        //    fallShoutCounter += Time.deltaTime;
        //    //Debug.Log("fall shout counter: " + fallShoutCounter);
        //    if (isInContactwithField)
        //        fallShoutCounter = 0;
        //
        //    if (fallShoutCounter > 1.5f)
        //    {
        //        if(!playerAudioSource.isPlaying)
        //            playerAudioSource.Play();
        //    }
        //    else
        //        playerAudioSource.Stop();
        //}
        //else
        //{
        //    fallShoutCounter = 0;
        //}
    }


    #region Absorb Functions
    void AbsorbCharacter()
    {
        Enemy e = DetectCharacter();
        if(e != null)
        {
            if (e.IsStunned())
            {
                //Enable Absorb Button
                if (absorbButton == null && !absorbing && !player.userInputs.absorbPressed)
                    EnableAbsorbButton();

                if (player.userInputs.absorbPressed)
                {
                    e.GettingAbsorbed(player, 3.0f);
                    StartCoroutine(AbsorbReset(e, 3.0f));
                }

            }
            else if(absorbButton!=null && !absorbing)
            {
                VirtualJoystick.DisableButton(absorbButton);
                absorbButton = null;
            }
        }
        else if (absorbButton != null && !absorbing)
        {
            VirtualJoystick.DisableButton(absorbButton);
            absorbButton = null;
        }
    }

    Enemy DetectCharacter()
    {
        Enemy e = null;
        if (player.State.Equals(typeof(IdleState)))
        {
            //Debug.Log("Casting ray to find the enemy");
            //If player is idle, Cast ray directly to the player's right to detect a hit
            Vector3 pointOfCast = new Vector3(player.transform.position.x + player.playerSprite.GetComponent<SpriteRenderer>().bounds.extents.x * (int)player.playerSprite.transform.localScale.x,
                            player.transform.position.y + player.playerSprite.GetComponent<SpriteRenderer>().bounds.extents.y,
                            player.transform.position.z);
            RaycastHit2D hit = Physics2D.Raycast(pointOfCast, player.transform.right * (int)player.playerSprite.transform.localScale.x,
                                absorbDistance, LayerMask.GetMask("Character"));

            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<Enemy>())
                {
                    e = hit.collider.GetComponent<Enemy>();
                }
            }
        }
        return e;
    }

    void EnableAbsorbButton()
    {
        absorbButton = VirtualJoystick.CreateButton("tag_absorb");
        if (!absorbButton.active)
        {
            VirtualJoystick.EnableButton(absorbButton);
            absorbButton.button.onClick.AddListener(() =>
            {
                player.userInputs.absorbPressed = true;
                
                //Disable the button
                VirtualJoystick.DisableButton(absorbButton);
                absorbButton = null;
            });
        }
    }

    IEnumerator AbsorbReset(Character c, float duration)
    {
        absorbing = true;
        yield return new WaitForSeconds(duration * 0.5f);
        StartReaction(c);
        yield return new WaitForSeconds(duration * 0.5f);
        absorbing = false;
    }

    void StartReaction(Character c)
    {
        ChangepH(c);
        GenerateSalt(c);
    }

    void ChangepH(Character c)
    {
        float resultantpH = 0;
        CharacterMechanics chMec = c.GetComponent<CharacterMechanics>();

        //TODO: Add the volume calculation here
        if (chMec != null)
        {
            resultantpH = (phValue + chMec.getpH()) / 2;
        }

        SetpH(resultantpH);
    }

    void GenerateSalt(Character c)
    {
        Debug.Log("Generating salt......Under progress");
        Debug.Log("Player chemical: " + player.chemical);
        Debug.Log("Enemy chemical: " + c.chemical);
        Salt resultantSalt = Reactions.reactionDictionary[player.chemical][c.chemical];

        if (resultantSalt != Salt.Null)
        {
            Debug.Log("salt generated: " + resultantSalt);
        }
        else
        {
            Debug.Log("Salt is null....Something went wrong");
        }
    }

    #endregion

}
