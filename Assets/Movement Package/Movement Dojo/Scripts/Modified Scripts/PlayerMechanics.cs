
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GrabAnimationInfo
{
    public AnimationClip grabStartAnimationClip;
}


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
    public Player player;

    float fallShoutCounter;

    private void Start()
    {
        base.Start();
        player = GetComponent<Player>();
        
        //playerAudioSource = GetComponent<AudioSource>();
        itemPickedUp = null;   //Initially, there are no item pick ups

        //TODO: Change this to a more automated form
        SetpH(phValue);
        //StartCoroutine(Transformation());
    }
    private new void Update()
    {
        base.Update();
        
        if (canAbsorbEnemy)
        {
            if(absMec == null)
                AbsorbCharacter();
            else
            {
                absMec.StunUpdate(ref absMec);
                //Debug.Log("Not null......" + absMec.ToString());
            }
            
        }
    }
    [SerializeField]
    public GrabAnimationInfo grabAnimationInfo;
    AbsorbMechanic absMec = null;

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
    Character absorbingCharacter = null;
    
    void AbsorbCharacter()
    {

        //Enable the virtual joystick button by checking the target's status
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
                    StartCoroutine(AbsorbReset(e, 3.0f));
                }
            }
            else if(absorbButton!=null && !absorbing)
            {
                Debug.Log("Character is not stunned");
                VirtualJoystick.DisableButton(absorbButton);
                absorbButton = null;
            }
        }
        else if (absorbButton != null && !absorbing)
        {
            Debug.Log("character is null");
            VirtualJoystick.DisableButton(absorbButton);
            absorbButton = null;
        }
    }
    

    Enemy DetectCharacter()
    {
        Enemy e = null;
        Debug.DrawRay(new Vector3(player.transform.position.x +(player.playerSprite.GetComponent<SpriteRenderer>().bounds.extents.x + 0.1f) * Mathf.Sign(player.playerSprite.transform.localScale.x),
                            player.transform.position.y + player.playerSprite.GetComponent<SpriteRenderer>().bounds.extents.y,
                            player.transform.position.z), Vector3.right*100, Color.green);
        if (player.State.Equals(typeof(IdleState)))
        {
            //Debug.Log("Casting ray to find the enemy");
            //If player is idle, Cast ray directly to the player's right to detect a hit
            Vector3 pointOfCast = new Vector3(player.transform.position.x + (player.playerSprite.GetComponent<SpriteRenderer>().bounds.extents.x + 0.1f) * Mathf.Sign(player.playerSprite.transform.localScale.x),
                            player.transform.position.y + player.playerSprite.GetComponent<SpriteRenderer>().bounds.extents.y,
                            player.transform.position.z);
            RaycastHit2D hit = Physics2D.Raycast(pointOfCast, player.transform.right * Mathf.Sign(player.playerSprite.transform.localScale.x),
                                absorbDistance, LayerMask.GetMask("Character"));

            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<Enemy>())
                {
                    //Debug.Log("Detecting character");
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
                Debug.Log("Absorb pressed");
                //Disable the button
                VirtualJoystick.DisableButton(absorbButton);
                absorbButton = null;
            });
        }
    }

    IEnumerator AbsorbReset(Character c, float duration)
    {
        Debug.Log("initiated the absorb mechanic");
        absMec = new AbsorbMechanic();
        absMec.Setup(this, c, grabAnimationInfo);
        yield return null;
    }
    
    #endregion
    
    public class AbsorbMechanic
    {
        AnimationClip grabClip;
        int grabanimationHash;

        float time;
        float elapsedTime;

        bool parentSet = false;
        bool clipSet = false;

        bool stageOneEntered = false;
        bool stageTwoEntered = false;

        Animator animator;

        Player player;
        PlayerMechanics playerMechanics;
        Character c;
        

        public void Setup(PlayerMechanics playerMechs, Character c, GrabAnimationInfo animInfo)
        {
            stageOneEntered = true;
            playerMechanics = playerMechs;
            this.player = playerMechs.player;
            animator = playerMechanics.player.playerSprite.GetComponent<Animator>();
            this.c = c;

            Debug.Log("animator set: " + animator.name);

            if(animInfo.grabStartAnimationClip != null)
            {
                grabanimationHash = animInfo.grabStartAnimationClip.GetHashCode();
                grabClip = animInfo.grabStartAnimationClip;
            }

            //Once the conditions are met, this block is executed only once. So, the absorb has to be set here
            animator.SetBool("Absorb", true);
            
        }
        AnimatorStateInfo stateInfo;
        public void StunUpdate(ref AbsorbMechanic absMec)
        {
            //Debug.Log("updating stun");
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            Debug.Log("Normalized state time: " + stateInfo.normalizedTime);
            if (stageOneEntered)
            {
                if (!clipSet)
                {
                    Debug.Log("Setting the clip: " + grabanimationHash);

                    //AnimatorClipInfo[] clips = animator.GetNextAnimatorClipInfo(0);
                    stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    if(stateInfo.IsName("Grab Start"))
                    {
                        clipSet = true;
                        time = stateInfo.length;
                        Debug.Log("Time is: " + time);
                    }
                    else
                    {
                        Debug.Log("Animation playing: " + stateInfo.ToString());
                        return;
                    }
                }
                
                //Debug.Log("Normalized state time: " + stateInfo.normalizedTime);
                elapsedTime += GameManager.Instance.DeltaTime;

                //Set the target character's parent as the current character's hand
                if (!parentSet)
                {
                    if (elapsedTime > time * 0.4f)
                    {
                        //Halt character movement of both the player and the enemy
                        c.GetComponent<Enemy>().GettingAbsorbed(player, time);

                        c.GetComponent<Collider2D>().enabled = false;
                        Debug.Log("Parent set");
                        c.transform.parent = player.Hand.transform;
                        c.transform.localPosition = new Vector3(0, -1.0f, 0);
                        c.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        c.playerSprite.GetComponent<SpriteRenderer>().sortingOrder = player.playerSprite.GetComponent<SpriteRenderer>().sortingOrder - 1;

                        parentSet = true;
                    }
                    return;
                }

                if (elapsedTime > time)
                {
                    animator.SetBool("Absorb", false);

                    if(!stateInfo.IsName("Grab Start") && !animator.IsInTransition(0))
                    {
                        //TODO: Start a different animation here
                        //For now, just disable the character
                        stageOneEntered = false;
                        stageTwoEntered = true;
                    }
                }

                c.transform.localPosition = new Vector3(0, -1.0f, 0);
                c.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            if (stageTwoEntered)
            {
                //Here, the PH transformation must start and respective animation must play
                //Keep the player's movements blocked till then
                StartReaction(c);

                //Deactivate the enemy for now
                c.GetComponent<Enemy>().Die();

                absMec = null;
            }
            
        }

        public void StartReaction(Character c)
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
                resultantpH = (playerMechanics.phValue + chMec.getpH()) / 2;
            }

            playerMechanics.SetpH(resultantpH);
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
        
    }

}


