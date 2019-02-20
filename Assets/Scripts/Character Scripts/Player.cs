using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public abstract class Character : MonoBehaviour, ICharacter
{
    //private Movement2D movementScript;
    //public virtual Movement2D MovementScript { get { return movementScript; } }
    public virtual States State { get; set; }
    public virtual List<States> StateList { get; set; }

    //Player speed properties
    public float mass;              //mass of the body moving
    public float gravity = -9.8f;   //Default downward force that is always acting
    public float maxSpeed = 450.0f; //max speed that is allowed in horizontal axis
    public float maxJumpSpeed;      //max speed allowed in vertical Axis
    public LayerMask whatToDetect;

    public float currentLinearSpeed = 0;
    public float currentJumpSpeed = 0;

    public float externalHorizontalMovementDamp;
    public float externalVerticalMovementDamp;

    public Vector3 externalForce;

    public Vector3 jumpDirection = Vector3.up;

    public Vector3 velocity = Vector3.zero;

    //blocks to inputs
    public bool horizontalInputBlock;// { get; set; }
    public bool verticalInputBlock;// { get; set; }

    public GameObject ceilingCheck;
    public GameObject groundCheck;

    [Range(0.01f, 2.0f)]
    public float groundCheckCircleRadius = 0.2f;    // The ground check radius of the circle cast
    public float ceilingCheckCircleRadius = 0.2f;

    public float jumpMultiplier = 1.0f;
    
    public GameObject Hand { get; set; }
    public GameObject Liquid { get; set; }
    public GameObject Body { get; set; }

    public Vector3 playerUpOrientation = Vector3.up;
    public GameObject playerSprite;

    //Audio Functions
    public PlayerAudio playerAudio;
    protected AudioSource audioSource;

    //Stun variables
    protected bool stunned = false;

    [SerializeField]
    public UserInput userInputs = new UserInput();

    //Grid the character belongs to
    public GridCell gridCell = null;
    public GridCell previousGridCell = null;

    public Coroutine blockRoutine = null;

    //Inventory variables
    protected Inventory inventory;
    
    // pH button show/hide
    public Button phMeterShow;

    //Chemical the character carrying
    public System.Enum chemical;

    #region Block Movement Functions
    public virtual void BlockInputs()
    {
        //Debug.Log("meh");
        if(blockRoutine != null)
        {
            StopCoroutine(blockRoutine);
            blockRoutine = null;
        }
        blockRoutine = StartCoroutine(CharacterUtility.BlockInputs(this, true, true, true, 0.3f));
    }
    public virtual void BlockInputs(float duration, bool horizontal, bool vertical)
    {
        //Debug.Log("meh");
        if (blockRoutine != null)
        {
            StopCoroutine(blockRoutine);
            blockRoutine = null;
        }
        blockRoutine =  StartCoroutine(CharacterUtility.BlockInputs(this, horizontal, vertical, true, duration));
    }

    public virtual void StopMovement(bool stopAnimations = true, bool horizontal = true, bool vertical = true)
    {
        horizontalInputBlock = horizontal;
        verticalInputBlock = vertical;
    }
    public virtual void ResumeMovement()
    {
        Debug.Log("movement resumed");
        verticalInputBlock = false;
        horizontalInputBlock = false;
    }
    #endregion
    //protected Collider2D[] CastGroundOverlapCircle()
    //{
    //    return Physics2D.OverlapCircleAll(groundCheck.transform.position, groundCheckCircleRadius, whatToDetect);
    //}

    protected RaycastHit2D[] CastGroundOverlapCircle()
    {
        return Physics2D.CircleCastAll(groundCheck.transform.position, groundCheckCircleRadius, Vector2.zero, 0, whatToDetect);
    }

    //public virtual void SetSoundEffect(AudioClip clipToPlay = null, bool loop = false, bool playOneShot = false, float delay = 0.0f) { }

    public virtual void SetAnimations() { }
    
    protected virtual void GetInput() { }


    public virtual void UseItem() { }

    protected virtual void MoveCharacter()
    {
        int horizontalBlockValue = 0;
        int verticalBlockValue = 0;
        externalForce = Vector3.zero;
        velocity = Vector3.zero;
        externalHorizontalMovementDamp = externalVerticalMovementDamp = 1.0f;
        if (!GameManager.Instance.paused)
        {
            //Debug.Log("linear speed: " + currentLinearSpeed);
            if (!horizontalInputBlock)
            {
                horizontalBlockValue = 1;
                velocity.x = currentLinearSpeed;
            }
            if (!verticalInputBlock)
            {
                verticalBlockValue = 1;
                velocity.y = currentJumpSpeed;
            }

            Vector3 externalSpeed = ((externalForce * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime) / mass);

            if (Mathf.Abs(externalSpeed.y) > 0.7f)
                externalSpeed.y = 0.7f * Mathf.Sign(externalSpeed.y);

            //0.001f moves the character slightly downwards so that it's always touching platform
            velocity = gameObject.transform.right * velocity.x * externalHorizontalMovementDamp * horizontalBlockValue +
                                                jumpDirection * (velocity.y - 0.001f) * externalVerticalMovementDamp * verticalBlockValue +
                                                externalSpeed;
            
            gameObject.transform.up = playerUpOrientation;

            gameObject.transform.position += velocity;
            
        }
    }

    public void SetSoundEffect(AudioClip clipToPlay = null, bool loop = false, bool playOneShot = false, float delay = 0.0f)
    {
        //Debug.Log("Sound effet set");
        if (clipToPlay == null && audioSource.isPlaying && audioSource.loop == false)
            return;

        if (clipToPlay != null)
        {
            audioSource.loop = loop;
            if (audioSource.clip != clipToPlay)
            {
                audioSource.clip = clipToPlay;
                if (!audioSource.isPlaying)
                {
                    if (playOneShot)
                    {
                        audioSource.Stop();
                        audioSource.PlayOneShot(clipToPlay);
                    }
                    else
                    {
                        audioSource.PlayDelayed(delay);
                    }
                }

            }
        }
        else
        {
            audioSource.clip = null;
        }
    }
    
    //Call this function to register player to the world grid
    protected void UpdatePositionInWorld()//Vector3 position, out GridCell gridCell)
    {
        GridIndex index = WorldGrid.GetGridIndex(gameObject.transform.position);

        if(gridCell == null)
            gridCell = WorldGrid.GetTheWorldGridCell(index);
        else if(index.x != gridCell.index.x || index.y != gridCell.index.y)
            gridCell = WorldGrid.GetTheWorldGridCell(index);

        if(gridCell != previousGridCell)
        {
            if(gridCell != null)
            {
                WorldGrid.AddToCell(this, gridCell);
                if (previousGridCell != null)
                    WorldGrid.RemoveFromCell(this, previousGridCell);
                previousGridCell = gridCell;
            }
            else
            {
                Debug.Log("Something went wrong. Player doesn't have a grid cell");
            }
        }

        //Debug.DrawLine(Vector3.zero, gridCell.worldPosition);
        //Debug.Log("Character: " + gameObject.name);
        //Debug.Log("Grid cell indices: " + gridCell.index.x + "    " + gridCell.index.y);
        //Debug.Log("Grid cell indices: " + gridCell.index.x + "    " + gridCell.index.y);
        //Debug.Log("Grid character count: " + gridCell.character.Count);
        //Debug.Log("Grid node count: " + gridCell.node.Count);

        //List<GridCell> cells = WorldGrid.Instance.gridArray[gridIndex.x, gridIndex.y];
    }

    public GridCell GetGridCell()
    {
        return gridCell;
    }

    #region health variables and functions
    [HideInInspector]
    public float Health { get; set; }

    public float maxHealth = 100;

    public virtual void Heal(float healValue)
    {
        Health += healValue;

        Health = Mathf.Clamp(Health, 0, 100);
    }

    public virtual void TakeDamage(float damageValue)
    {
        if (Health > damageValue)
        {
            Health -= damageValue;
            Debug.Log("Player health decreased" + Health);
            Health = Mathf.Clamp(Health, 0, 100);
        }
        else
        {
            Debug.Log("Player dead");

            ///Player die function
        }

    }

    #endregion
    
}

public class PlayerStatus
{
    public PlayerStatus(Canvas statusCanvas)
    {
        playerStatusCanvas = statusCanvas;
        healthImage = playerStatusCanvas.transform.Find("Health").GetChild(0).GetComponent<RawImage>();

        if (healthImage != null)
            heightOfHealthImage = healthImage.rectTransform.rect.height;
        
        else
            heightOfHealthImage = 0.0f;
        Debug.Log("Height of health image: " + heightOfHealthImage);

        currentHealth = 0.0f;
        maxHealth = 100.0f;

        ratioBetweenMaxHealthAndImageHeight = heightOfHealthImage / maxHealth;

        //pH meter variables
        //TODO: change this to search by name
     
        pHMeterImage = playerStatusCanvas.transform.Find("pHMeterIndicator").GetChild(0).GetChild(0).GetComponent<RawImage>();

        Debug.Log(pHMeterImage + "PH meter image");
        pHpointer = pHMeterImage.transform.GetChild(0).GetComponent<RawImage>();

        Debug.Log("phpointer: " + pHpointer.name);

        pHpointer.rectTransform.localPosition = Vector3.zero;

        widthOfpHMeter = pHMeterImage.rectTransform.rect.width;

        phShowButton = playerStatusCanvas.transform.Find("pHMeterIndicator").GetChild(1).GetComponent<Button>();

        pHIndicatorImage = phShowButton.GetComponent<Image>();

        pHIndicatorName = playerStatusCanvas.transform.Find("pHMeterIndicator").Find("pHName").transform.GetChild(0).GetComponent<Text>();
        pHUseCounterText = playerStatusCanvas.transform.Find("pHMeterIndicator").Find("pHUseCounter").transform.GetChild(0).GetComponent<Text>();
        pHUseCounter = 0;

        pHIndicator = null;
        PHIndicatorChangedEvent = null;
    }

    public Canvas playerStatusCanvas;

    //Health variables
    public RawImage healthImage;
    public float heightOfHealthImage;

    public float maxHealth;
    public float currentHealth;
    float ratioBetweenMaxHealthAndImageHeight;

    //Ph Value variables
    public RawImage pHMeterImage;
    public RawImage pHpointer;
    public float widthOfpHMeter;
    public Image pHIndicatorImage;
    public Text pHIndicatorName;
    public Text pHUseCounterText;
    public int pHUseCounter;

    //ph button Show/Hide
    public Button phShowButton;

    
    public PH pHIndicator;
    public delegate void PHIndicatorChanged();
    public event PHIndicatorChanged PHIndicatorChangedEvent;


    public void Heal(float healAmount)
    {
        Debug.Log("Healer used");
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void TakeDamage(float DamageAmount)
    {
        currentHealth -= DamageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void SetHealthBar(float amount)
    {
        float result = amount * ratioBetweenMaxHealthAndImageHeight;
        healthImage.rectTransform.sizeDelta = new Vector2(healthImage.rectTransform.rect.width, result);
    }

    public void SetpHGraphic(float value , Color phColor)
    {
        float block = widthOfpHMeter / 14;

        float xPos = -(widthOfpHMeter * 0.5f) + (value * block) + (block * 0.5f);

        pHpointer.rectTransform.localPosition = new Vector3(xPos, 0, 0);

        Debug.Log("pH pointer is set at: " + xPos);

        phShowButton.GetComponent<Image>().color = phColor;
        phShowButton.GetComponentInChildren<Text>().text = "PhMeter" + value;
    }


    public void SetpHIndicator(PH newIndicator)
    {
        //Add the event and raise an event to notify subscribers that pH indicator has changed
        pHIndicator = newIndicator;
        //Set the image in the button
        SetpHIndicatorImage(newIndicator.PHIndicatorImage);

        //TODO: Must be changed to reflect the pH use counter to the number of items of same type in the inventory
        //Increment the pH use counter
        pHUseCounter = newIndicator.GetUseCount();
        pHIndicatorName.text = newIndicator.indicator.ToString();
        pHUseCounterText.text = pHUseCounter.ToString();

        if (PHIndicatorChangedEvent != null)
            PHIndicatorChangedEvent();
    }
    void SetpHIndicatorImage(Sprite pHImage)
    {
        if(pHIndicatorImage != null)
        {
            pHIndicatorImage.sprite = pHImage;
        }
        else
        {
            Debug.LogError("pH meter");
        }
    }

    public PH GetpHIndicator()
    {
        return pHIndicator;
    }

    public void DecrementpHUse()
    {
        pHUseCounter--;
        pHUseCounterText.text = pHUseCounter.ToString();
        if (pHUseCounter == 0)
        {
            pHIndicatorName.text = "None";
        }
    }
    
}

[System.Serializable]
public class Player : Character
{
    RaycastHit2D[] info;

    //Player Status Canvas
    PlayerStatus playerStatus;
    //public PlayerStatus PlayerStats { get { return playerStatus; } set { playerStatus = value; } }

    Animator phMeterAnimator;

    public List<Character> enemiesChasing { get; private set; }

    public Node currentNodeOfPlayer { get; private set;}
    
    private ThrowTapInput tapInput;

   public void ShowPhMeter()
    {
        Debug.Log( "Animebool" + phMeterAnimator.GetBool("ShowPhMeter"));
        
       if(phMeterAnimator.GetBool("ShowPhMeter"))
        {
            phMeterAnimator.SetBool("ShowPhMeter", false);
        }
       else
        {
            phMeterAnimator.SetBool("ShowPhMeter", true);
            Invoke("ShowPhMeter", 15f);
        }
    }

    
    private void Start()
    {
        enemiesChasing = new List<Character>();

        Debug.Log("script is working");
        State = new IdleState();

        ceilingCheck = gameObject.transform.Find("CeilingCheck").gameObject;
        groundCheck = gameObject.transform.Find("GroundCheck").gameObject;

        playerSprite = transform.Find("Sprite").gameObject;
        Hand = playerSprite.transform.Find("Hand").gameObject;
        Liquid = playerSprite.transform.Find("Liquid").gameObject;
        Body = playerSprite.transform.Find("Body").gameObject;

        //Initializing the player status variables
        playerStatus = new PlayerStatus(transform.GetComponentInChildren<Canvas>());

        phMeterAnimator = transform.GetComponentInChildren<Canvas>().transform.Find("pHMeterIndicator").GetChild(0).GetComponent<Animator>();
        playerAudio = GetComponent<PlayerAudio>();
        audioSource = GetComponent<AudioSource>();

        StateList = new List<States>();
        StateList.Add(new IdleState());

        //Input objects
        tapInput = new ThrowTapInput(this, GameManager.Instance.virtualJoystick.GetComponent<Canvas>());

        //Add health to player initially
        Heal(100);
    }
    
    private void Update()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        //TODO: Remove this test later
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Pressed P....Opening Journal");
            Journal.Instance.ToggleJournal();
        }


        //This has the information of all the object the player is currently in contact with
        info = CastGroundOverlapCircle();

        UpdateCurrentNodeOfPlayer();

        GetInput();

        //Debug.Log("climb: " + userInputs.climbPressed);

        //State.UpdateState(this, userInputs);
        List<States> tempStates = StateList;
         
        //Debug.Log("States count: " + StateList.Count);
        for (int i = 0; i < StateList.Count; i++)
            StateList[i].UpdateState(this, userInputs, info);

        MoveCharacter();

        State = StateList[StateList.Count - 1];
    }
    
    
    private void LateUpdate()
    {
        UpdatePositionInWorld();
        
        ResetInputs();
        //SetGridIndex();
        //Reading inputs is left to unity's input functions. So, reset functions work here
    }

    private void UpdateCurrentNodeOfPlayer()
    {
        for(int i = 0; i < info.Length; i++)
        {
            Platform p = info[i].collider.GetComponent<Platform>();
            if (p != null)
            {
                //Check the distance between left and right nodes and assign the closest one
                if(Vector3.SqrMagnitude(p.leftNode.position - gameObject.transform.position) <=
                    Vector3.SqrMagnitude(p.rightNode.position - gameObject.transform.position))
                {
                    currentNodeOfPlayer = p.leftNode;
                }
                else
                {
                    currentNodeOfPlayer = p.rightNode;
                }
            }
        }
    }

    protected override void GetInput()
    {

        //Horizontal Movement
        if (GameManager.Instance.keyboardControls)
            userInputs.xInput = Input.GetAxis("Horizontal");
        else
            userInputs.xInput = VirtualJoystick.horizontalValue;

        //Jump
        if ((Input.GetKeyDown(KeyCode.Space) || VirtualJoystick.jumpButtonDown))
        {
            userInputs.jumpPressed = true;
        }
        if ((Input.GetKeyUp(KeyCode.Space) || VirtualJoystick.jumpButtonUp) || currentJumpSpeed < 0)
        {
            userInputs.jumpPressed = false;
            userInputs.jumpReleased = true;
        }
        
        //Check for the climb button press. Input later changes to a virtual function
        if (Input.GetKeyDown(KeyCode.C) && !userInputs.climbPressed)
        {
            userInputs.climbPressed = true;
            Debug.Log("Special button pressed......Doing something crazy");
        }
        //else
        //{
        //    userInputs.climbPressed = false;
        //    //Debug.Log("Special button to false");
        //}

        //Absorbing input
        if (Input.GetKeyDown(KeyCode.V))
        {
            userInputs.absorbPressed = true;
        }

        //open door input
        if (Input.GetKeyDown(KeyCode.O))
        {
            userInputs.doorOpenPressed = true;
        }
        else
        {
            userInputs.doorOpenPressed = false;
        }

        //VirtualInputs();

        //External input object updates
        tapInput.Update();
    }

    void VirtualInputs()
    {
        //ResetButtons();
        for (int i = 0; i < info.Length; i++)
        {
            LadderButtonLogic(info[i]);

            ItemButtonLogic(info[i]);
        }
    }

    public string[] playerManagedControls = { "tag_ladder" };

    void LadderButtonLogic(RaycastHit2D info)
    {
        if (info.collider.tag == "tag_ladder")
        {
            //Debug.Log("State:  " + State.ToString());
            if (!State.Equals(typeof(ClimbingState)) && State.Equals(typeof(IdleState)))
            {
                DynamicButton d = VirtualJoystick.CreateDynamicButton("tag_ladder");
                if (!d.active)
                {
                    VirtualJoystick.EnableDynamicButton(d);

                    d.button.onClick.AddListener(() => {
                        if (!State.Equals(typeof(ClimbingState)))
                        {
                            Debug.Log("climb pressed");
                            userInputs.climbPressed = true;
                            
                        }
                        else
                        {
                            userInputs.climbPressed = true;
                            VirtualJoystick.DisableDynamicButton("tag_ladder");
                        }
                    });

                }
            }
        }
    }

    #region health Functions and variables
    Coroutine healthCoroutine;

    public override void Heal(float healValue)
    {
        if (healthCoroutine == null)
            healthCoroutine = StartCoroutine(HealthAnimation(playerStatus.currentHealth, playerStatus.currentHealth + healValue));
        else
        {
            StopCoroutine(healthCoroutine);
            playerStatus.SetHealthBar(playerStatus.currentHealth);
            healthCoroutine = StartCoroutine(HealthAnimation(playerStatus.currentHealth, playerStatus.currentHealth + healValue));
        }

        playerStatus.Heal(healValue);
        //base.Heal(healValue);
    }
    public override void TakeDamage(float damageValue)
    {
        if (healthCoroutine == null)
            healthCoroutine = StartCoroutine(HealthAnimation(playerStatus.currentHealth, playerStatus.currentHealth - damageValue));
        else
        {
            StopCoroutine(healthCoroutine);
            playerStatus.SetHealthBar(playerStatus.currentHealth);
            healthCoroutine = StartCoroutine(HealthAnimation(playerStatus.currentHealth, playerStatus.currentHealth - damageValue));
        }

        playerStatus.TakeDamage(damageValue);
        //base.TakeDamage(damageValue);
    }

    IEnumerator HealthAnimation(float currentHealth, float targetHealth)
    {
        float ch = currentHealth; float th = targetHealth;
        float speed = 0.07f;
        //Debug.Log("current and target: " + currentHealth + "   " + targetHealth);
        if(targetHealth < 0)
        {
            playerStatus.SetHealthBar(0);
            yield break;
        }

        while(Mathf.Abs(th - ch) > 1.0f)
        {
            //float diff = ((th - ch) / th) * speed;
            float diff = ch * (1 - speed) + th * speed;
            //Debug.Log("diff is: " + diff);
            ch = diff;
            playerStatus.SetHealthBar(ch);
            yield return new WaitForFixedUpdate();
        }
        playerStatus.SetHealthBar(th);
        healthCoroutine = null;
    }

    #endregion


    public void SetpHCanvasGraphic(float value,Color phColor)
    {
        playerStatus.SetpHGraphic(value,phColor);
    }

    void ItemButtonLogic(RaycastHit2D info)
    {
        if(info.collider.tag == "tag_item")
        {
            if (State.Equals(typeof(IdleState)) || State.Equals(typeof(RunningState)))
            {
                //Create an item button
                DynamicButton d = VirtualJoystick.CreateDynamicButton("tag_item");
                if (!d.active)
                {
                    VirtualJoystick.EnableDynamicButton(d);
                }
            }
        }
    }

    //TODO: consider putting this functiong in the virtual joystick class
    void ResetButtons()
    {
        for(int i = 0; i < VirtualJoystick.activeDynamicButtons.Count; i++)
        {
            for(int j = 0; j < info.Length; j++)
            {
                
                if (VirtualJoystick.activeDynamicButtons[i].tag == info[j].collider.tag)
                    return;
            }
            VirtualJoystick.DisableDynamicButton(VirtualJoystick.activeDynamicButtons[i]);
        }
    }

    public void ResetInputs()
    {
        userInputs.climbPressed = false;
        userInputs.doorOpenPressed = false;
        userInputs.absorbPressed = false;
    }

    public void AddEnemyChasingPlayer(Character e)
    {
        enemiesChasing.Add(e);
    }
    public void RemoveEnemyChasingPlayer(Character e)
    {
        enemiesChasing.Remove(e);
    }

    public void DecrementpHUse()
    {
        playerStatus.DecrementpHUse();
    }

    private void OnDrawGizmos()
    {
        if (currentNodeOfPlayer != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(currentNodeOfPlayer.position, 0.5f);
        }
    }

    public PlayerStatus GetPlayerStatus()
    {
        return playerStatus;
    }

    public System.Enum React(PlayerInventory.SelectionObjectData obj)
    {
        return GetComponent<PlayerMechanics>().StartReaction(obj.item);
    }

}

