using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class MovementScript2D : MonoBehaviour {


    [Range(0.01f, 2.0f)]
    public float groundCheckCircleRadius = 0.2f;    // The ground check radius of the circle cast
    public float ceilingCheckCircleRadius = 0.2f;

    
    public float mass;              //mass of the body moving
    public float gravity = -9.8f;   //Default downward force that is always acting
    public float maxSpeed;          //max speed that is allowed in horizontal axis
    public float maxJumpSpeed;      //max speed allowed in vertical Axis
    public LayerMask whatToDetect;

    public float jumpDamper = 1;    //The rate at which jump deceleration must happen
    public bool jumpButtonPresssed;
    public bool jumpButtonReleased;

    

    public PlayerStates playerState;

    GameObject ceilingCheck;
    GameObject groundCheck;

    //External Force variables
    [HideInInspector]
    public Vector3 externalForce;
    public float externalHorizontalMovementDamp;
    public float externalVerticalMovementDamp;
    private Vector3 externalSpeed;


    //Audio variables
    private PlayerAudio playerAudio;
    private AudioSource playerAudioSource;

    //Movement script speed variables
    public float CurrentLinearSpeed
    {
        get
        {
            return currentLinearSpeed;
        }
    }
    public float CurrentJumpSpeed
    {
        get
        {
            return currentJumpSpeed;
        }
    }
    private float currentLinearSpeed;
    private float currentJumpSpeed;
    public Vector3 velocity;

    //blocks to inputs
    bool horizontalInputBlock;
    bool verticalInputBlock;

    //few gameobjects that might be useful in other scripts
    [HideInInspector]
    public GameObject liquidObject;

    //The object that has the sprite renderer
    private GameObject playerSprite;
    private bool bottomContacted = false;

    private float animatorBlendVariable;    //This variable represents the BLend state of the animator
    private float jumpTimer = 0;

    private float ycorrection = 0;
    private Vector3 currentJumpDirection;
    private Vector3 playerUpOrientation;    // modified by the player orientation function

    //AI controls
    public bool IsAI;// { get; set; }  //TODO: Make this a property
    AI aiComponent;

    private void Awake()
    {
        playerUpOrientation = Vector3.up;
        animatorBlendVariable = 0;

        playerSprite = gameObject.transform.GetChild(0).gameObject;

        ceilingCheck = gameObject.transform.Find("CeilingCheck").gameObject;
        groundCheck = gameObject.transform.Find("GroundCheck").gameObject;
        //animator = GetComponent<Animator>();
        playerState = PlayerStates.idle;

        //Setting the blocks to false to allow inputs
        horizontalInputBlock = false;
        verticalInputBlock = false;

        //Setting the jump button variables
        jumpButtonPresssed = false;
        jumpButtonReleased = true;

        playerAudio = GetComponent<PlayerAudio>();
        playerAudioSource = GetComponent<AudioSource>();

        currentJumpDirection = Vector3.up;

        GetAiComponent();

    }

    private void OnDrawGizmos()
    {
        ceilingCheck = gameObject.transform.Find("CeilingCheck").gameObject;
        groundCheck = gameObject.transform.Find("GroundCheck").gameObject;

        Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckCircleRadius);
        Gizmos.DrawWireSphere(ceilingCheck.transform.position, ceilingCheckCircleRadius);
    }
    

    float ButtonInput()
    {
        return Input.GetAxis("Horizontal") * maxSpeed * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime;
    }

    void Jump()
    {
        Collider2D upwardCollision = Physics2D.OverlapCircle(ceilingCheck.transform.position, groundCheckCircleRadius / 2, whatToDetect);


        if (upwardCollision != null && currentJumpSpeed > 0)
        {
            //Debug.Log("Upward collision is not null");
            currentJumpSpeed = 0;
        }

        if (playerState == PlayerStates.jumping || playerState == PlayerStates.falling || playerState == PlayerStates.swimming)
        {
            if (playerState == PlayerStates.swimming)
            {
                //This is now, more of a constant downward force that's acting on the body
                //This can be varied a little bit so that it gives the effect of the body floating on the water

                currentJumpSpeed = mass * Random.Range(gravity, gravity + 1) * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime;
                
            }
            else
            {
                jumpTimer += GameManager.Instance.DeltaTime;
                //There has to be a default downwardforce acting all the time, which is gravity
                currentJumpSpeed += gravity * 4.5f * jumpDamper * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime;

                if (Mathf.Sign(currentJumpSpeed) < 0 && Mathf.Abs(currentJumpSpeed) > 0.3f)
                    currentJumpSpeed = -0.3f;
            }
            
        }
    }
    
    void GetAiComponent()
    {
        if (IsAI)
        {
            aiComponent = GetComponent<AI>();
        }
    }

    // Update is called once per frame
    void Update () {
        if (!GameManager.Instance.paused)
        {
            if (aiComponent == null && IsAI)
                GetAiComponent();

            //If the player is in the jumping state and the jump button is pressed
            if (jumpButtonPresssed && playerState == PlayerStates.jumping)
            {

                if (jumpDamper > 0.25f)
                {
                    jumpDamper -= 2 * GameManager.Instance.DeltaTime;
                }
                else
                {
                    jumpDamper = 0.25f;
                }

            }
            if (playerState == PlayerStates.falling)
            {
                if (jumpDamper < 1)
                {
                    jumpDamper += 4.0f * GameManager.Instance.DeltaTime;
                }
                else
                {
                    jumpDamper = 1;
                }
            }
            //SetState();

            Jump();

        }

        
        if (!GameManager.Instance.paused && playerState != PlayerStates.stun)
        {
            if (!playerAudioSource.isPlaying && playerAudioSource.loop)
                playerAudioSource.Play();

            //playerUpOrientation = Vector3.up;

            GetInput();
            SetState();
            //TODO: Add a separate function that sets the sounds separately
            SetAnimations();

            velocity.x = currentLinearSpeed;
            velocity.y = currentJumpSpeed;

            
            externalSpeed = ((externalForce * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime) / mass);

            if (Mathf.Abs(externalSpeed.y) > 0.7f)
                externalSpeed.y = 0.7f * Mathf.Sign(externalSpeed.y);


            velocity = gameObject.transform.right * velocity.x * externalHorizontalMovementDamp +
                                                currentJumpDirection * (velocity.y + ycorrection) * externalVerticalMovementDamp +
                                                externalSpeed;
            

            gameObject.transform.up = playerUpOrientation;

            gameObject.transform.position += velocity;
            
        }
        else
        {
            playerAudioSource.Pause();
        }
        ycorrection = 0;
        //Reset all the external forces
        externalForce = Vector3.zero;
        externalHorizontalMovementDamp = externalVerticalMovementDamp = 1.0f;
        
    }

    void SetState()
    {
        
        Collider2D[] info = Physics2D.OverlapCircleAll(groundCheck.transform.position, groundCheckCircleRadius, whatToDetect);
        

        if (Mathf.Abs(currentLinearSpeed) > 0 && playerState != PlayerStates.jumping && playerState != PlayerStates.falling)
        {
            ycorrection -= 0.01f;
            playerState = PlayerStates.running;

            //Setting the walk audio
            SetSoundEffect(playerAudio.walk, true, false, 0.15f);

        }

        bottomContacted = false;

        //this just sets the jumping state.....Falling state is set depending on the falling velocity(negative if it starts falling down
        if (jumpButtonPresssed && jumpButtonReleased)
        {
            if ((playerState == PlayerStates.idle || playerState == PlayerStates.running || playerState == PlayerStates.swimming))
            {
                //Debug.Log("Player Jump state is set");
                playerState = PlayerStates.jumping;
                jumpDamper = 1; //Setting the jump damper value to 1 once the jump starts

                //set the vertical velocity
                currentJumpSpeed = maxJumpSpeed * GameManager.Instance.stepSize * GameManager.Instance.stepSize;

                //Jump release variable is set here to false because we want this function to execute only on jump button press
                jumpButtonReleased = false;
                //currentJumpDirection = gameObject.transform.up;
                currentJumpDirection = Vector3.up;
                //Set the Jump sound here: Since this block executes only once, it's a perfect place to put the jump sound
                
                SetSoundEffect(playerAudio.jump, false, false);
            }
        }

        if (currentJumpSpeed < 0)
        {
            if (playerState != PlayerStates.falling && playerState != PlayerStates.swimming)
            {
                
                playerState = PlayerStates.falling;
                currentJumpDirection = Vector3.up;

                
                //Since this block executes only once for the falling, this a perfect place to set the fall sound effect
                SetSoundEffect(playerAudio.fall, true, false, 1.5f);

            }
        }

        float angle = 0; float yCorr = 0;
        if(playerState != PlayerStates.swimming)
            CastRayAndOrientPlayer(out angle, out yCorr);
        
        //TODO: add a better less costly ray casting

       
        if (info.Length > 0 && playerState != PlayerStates.jumping)
        {
            bottomContacted = true;
            bool swimming = false;
            liquidObject = null;    //set the liquid object to null, because it's not swimming right now
            currentJumpDirection = Vector3.up;

            
            animatorBlendVariable = 0.0f;   //0 is the default state
            //TODO: add conditions for animation for each item pickup

            
            for (int i = 0; i < info.Length; i++)
            {
                
                //set the state to swimming if any one of the collided items has that tag
                if (info[i].tag == "tag_liquid")
                {
                    animatorBlendVariable = 0.5f;   //set the animator blend variable to 0.5f

                    //Sound for falling into the water
                    if (playerState != PlayerStates.swimming)
                    {
                        SetSoundEffect(playerAudio.landOnWater, false, false);
                    }

                    swimming = true;
                    playerState = PlayerStates.swimming;
                    liquidObject = info[i].gameObject;
                    break;
                }
            }

            if (playerState == PlayerStates.falling && jumpTimer > 0.2f)
            {
                StartCoroutine(BlockInputs(true, true, true, 0.1f));
                //Debug.Log("Landed.....Blocked the input");
                //Land sound can be set here since the block to inputs is only called once
                SetSoundEffect(playerAudio.landOnLand, false, false);

                if (yCorr < -0.05f)
                {
                    //Debug.Log("y correction is: " + yCorr);
                    ycorrection = yCorr;
                }

            }

            //Give idle state for collision with any collider for now
            if (!swimming)
            {
                //If the angle with the collider is greater than 65, then set the state as falling cuz player wont be able to walk on it
                if (angle < 65)
                {
                    if (currentLinearSpeed == 0)
                    {
                        playerState = PlayerStates.idle;
                        //There must be no sound for the idle animation
                        
                        if(playerAudioSource.clip != playerAudio.landOnLand)
                            SetSoundEffect(null, false);
                    }
                    else
                        playerState = PlayerStates.running;

                    if (Mathf.Abs(currentJumpSpeed) > 0)
                    {
                        currentJumpSpeed = 0;
                    }
                }
                else
                {
                    if (playerState != PlayerStates.falling)
                    {
                        SetSoundEffect(playerAudio.fall, true, false, 1.3f);
                    }
                    playerState = PlayerStates.falling;
                    currentJumpDirection = Vector3.up;
                }

                
            }
            //Jump damper value has to be reset no matter what type of floor the player hits.
            //It could be liquid or a solid platform
            jumpDamper = 1;
            jumpTimer = 0;
            

        }
        //If no floor detection happened, then set the state to falling. Because that makes most sense
        else
        {
            if (playerState != PlayerStates.jumping)
            {
                //Condition because, the clip must be set only once
                if (playerState != PlayerStates.falling)
                    SetSoundEffect(playerAudio.fall, true, false, 1.3f);
                playerState = PlayerStates.falling;
                currentJumpDirection = Vector3.up;
            }
        }

        //Player fall sound condition function
    }

    void SetSoundEffect(AudioClip clipToPlay = null, bool loop = false, bool playOneShot = false, float delay = 0.0f)
    {
        
        //Debug.Log("Sound effet set");
        if (clipToPlay != null)
        {
            playerAudioSource.loop = loop;
            if (playerAudioSource.clip != clipToPlay)
            {
                playerAudioSource.clip = clipToPlay;
                if (!playerAudioSource.isPlaying)
                {
                    if (playOneShot)
                    {
                        playerAudioSource.Stop();
                        playerAudioSource.PlayOneShot(clipToPlay);
                    }
                    else
                    {
                        playerAudioSource.PlayDelayed(delay);
                    }
                }
                
            }
        }
        else
        {
            playerAudioSource.clip = null;
        }
    }

    void CastRayAndOrientPlayer(out float angle, out float yCorr)
    {
        //Cast the ray and have the player tile according to the angle of the ground
        RaycastHit2D hit = Physics2D.CircleCast(groundCheck.transform.position, groundCheckCircleRadius, -gameObject.transform.up, 0.5f, whatToDetect);

        //Get the angle with the world right vector
        Vector3 dir = gameObject.transform.up;

        yCorr = 0;
        angle = 0;
        //Debug.Log("collider is: " + hit.collider);
        
        if (hit.collider != null)
        {
            angle = Vector3.Angle(Vector3.up, hit.normal);

            if (playerState != PlayerStates.jumping)
            {
                if (angle < 65)
                {
                    if (angle > 0.05f)
                        dir = Vector3.Lerp(gameObject.transform.up, hit.normal, 0.2f);
                    else
                        dir = hit.normal;

                    playerUpOrientation = dir;
                }
            }

        }
        else
        {
            
            //Debug.Log("Player up orientation set to world");
            dir = Vector3.Lerp(gameObject.transform.up, Vector3.up, 0.15f);
            playerUpOrientation = dir;
            
        }
        if (playerState == PlayerStates.falling)
        {
            //Debug.Log("Distance to ground is: " + Vector3.Distance(hit.point, gameObject.transform.position));

            yCorr = -(gameObject.transform.position.y - hit.point.y);
        }


    }

    void GetInput()
    {
        //input for x-axis
        if (!horizontalInputBlock)
        {
            if (!IsAI)
            {
                if (GameManager.Instance.keyboardControls)
                    currentLinearSpeed = Input.GetAxis("Horizontal") * maxSpeed * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime;
                else
                    currentLinearSpeed = VirtualJoystick.horizontalValue * maxSpeed * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime;
            }
            else
            {
                if(aiComponent!=null)
                    currentLinearSpeed = aiComponent.horizontalMovement * maxSpeed * aiComponent.speedMultiplier * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime;
            }
        }
        //Input for y-axis
        if (!verticalInputBlock)
        {
            if (!IsAI)
            {
                if ((Input.GetKeyDown(KeyCode.Space) || VirtualJoystick.jumpButtonDown) && !verticalInputBlock)
                {
                    jumpButtonPresssed = true;

                }
            }
            else
            {
                if (aiComponent != null)
                {
                    if (aiComponent.jump)
                    {
                        jumpButtonPresssed = true;
                        //Debug.Log("jump button release: " + jumpButtonReleased);
                        //Debug.Log("jump button: " + jumpButtonPresssed);
                    }
                    if (aiComponent.jumpRelease)
                    {
                        jumpButtonReleased = true;
                        jumpButtonPresssed = false;
                    }
                }
            }
        }
        if (!IsAI)
        {
            if (Input.GetKeyUp(KeyCode.Space) || VirtualJoystick.jumpButtonUp)
            {
                jumpButtonPresssed = false;
                jumpButtonReleased = true;
            }
            
        }
        else
        {
            
        }

        if (verticalInputBlock)
            jumpButtonPresssed = false;
    }

    void SetAnimations()
    {
        //depending on the direction of movement, flip the sprite
        if(currentLinearSpeed > 0)
        {
            gameObject.transform.GetChild(0).localScale = new Vector3(1, 1, 1);
        }
        if(currentLinearSpeed < 0)
        {
            gameObject.transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
        }

        //Set the animator variables depending on the state
        if (currentLinearSpeed == 0)
        {
            playerSprite.GetComponent<Animator>().SetBool("Walk", false);
        }
        if (Mathf.Abs(currentLinearSpeed) > 0 && playerState == PlayerStates.running)
        {
            playerSprite.GetComponent<Animator>().SetBool("Walk", true);
        }

        //Setting the jump speed for the jump animation
        playerSprite.GetComponent<Animator>().SetFloat("JumpSpeed", currentJumpSpeed);
        playerSprite.GetComponent<Animator>().SetBool("BottomContact", bottomContacted);
        playerSprite.GetComponent<Animator>().SetFloat("Blend", animatorBlendVariable);
        
    }
    
    public IEnumerator BlockInputs(bool horizontal = true, bool vertical = true, bool rotation = true, float duration = 1.0f)
    {
        //Debug.Log("inside the blocks block");
        if (horizontal)
        {
            horizontalInputBlock = true;
            currentLinearSpeed = 0;
        }
        if (vertical)
        {
            verticalInputBlock = true;
            //currentJumpSpeed = 0;
        }

        yield return new WaitForSeconds(duration);

        horizontalInputBlock = false;
        verticalInputBlock = false;
        
    }

    public void UnBLockInputs()
    {
        horizontalInputBlock = false;
        verticalInputBlock = false;
    }
}
