﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public abstract class CharacterNew : MonoBehaviour, ICharacter
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
    public bool horizontalInputBlock { get; set; }
    public bool verticalInputBlock { get; set; }

    public GameObject ceilingCheck;
    public GameObject groundCheck;

    [Range(0.01f, 2.0f)]
    public float groundCheckCircleRadius = 0.2f;    // The ground check radius of the circle cast
    public float ceilingCheckCircleRadius = 0.2f;

    public Vector3 playerUpOrientation = Vector3.up;
    public GameObject playerSprite;

    //////PLayer HEalth////
    public float health = 50;

    public float maxHealth = 100;

    public void HealHealth(float healValue)
    {
        health += healValue;

        health = Mathf.Clamp(health, 0, 100);
    }
    
    public void TakeDamage(float damageValue)
    {
        if(health>damageValue)
        {
            health -= damageValue;
            Debug.Log("Player health decreased" + health);
            health = Mathf.Clamp(health, 0, 100);
        }
        else
        {
            Debug.Log("Player dead");

            ///Player die function
        }
        
    }


    ///////////////////////////////////////////////////
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

    //Functions
    //public virtual void BlockInputs()
    //{
    //    Debug.Log("meh");
    //    if(blockRoutine != null)
    //        StopCoroutine(blockRoutine);
    //    blockRoutine = StartCoroutine(CharacterUtility.BlockInputs(this, true, true, true, 0.3f));
    //}
    //public virtual void BlockInputs(float duration, bool horizontal, bool vertical)
    //{
    //    Debug.Log("meh");
    //    if (blockRoutine != null)
    //        StopCoroutine(blockRoutine);
    //    blockRoutine =  StartCoroutine(CharacterUtility.BlockInputs(this, horizontal, vertical, true, duration));
    //}

    protected Collider2D[] CastGroundOverlapCircle()
    {
        return Physics2D.OverlapCircleAll(groundCheck.transform.position, groundCheckCircleRadius, whatToDetect);
    }
    
    //public virtual void SetSoundEffect(AudioClip clipToPlay = null, bool loop = false, bool playOneShot = false, float delay = 0.0f) { }

    public virtual void SetAnimations() { }
    
    protected virtual void GetInput() { }


    public virtual void UseItem() { }

    protected void MoveCharacter()
    {
        externalForce = Vector3.zero;
        velocity = Vector3.zero;
        externalHorizontalMovementDamp = externalVerticalMovementDamp = 1.0f;
        if (!GameManager.Instance.paused)
        {
            //Debug.Log("linear speed: " + currentLinearSpeed);
            if (!horizontalInputBlock)
                velocity.x = currentLinearSpeed;
            if (!verticalInputBlock)
                velocity.y = currentJumpSpeed;

            Vector3 externalSpeed = ((externalForce * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime) / mass);

            if (Mathf.Abs(externalSpeed.y) > 0.7f)
                externalSpeed.y = 0.7f * Mathf.Sign(externalSpeed.y);

            velocity = gameObject.transform.right * velocity.x * externalHorizontalMovementDamp +
                                                jumpDirection * (velocity.y) * externalVerticalMovementDamp +
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
    //protected void UpdatePositionInWorld()//Vector3 position, out GridCell gridCell)
    //{
    //    GridIndex index = WorldGrid.GetGridIndex(gameObject.transform.position);
    //
    //    if(gridCell == null)
    //        gridCell = WorldGrid.GetTheWorldGridCell(index);
    //    else if(index.x != gridCell.index.x || index.y != gridCell.index.y)
    //        gridCell = WorldGrid.GetTheWorldGridCell(index);
    //
    //    if(gridCell != previousGridCell)
    //    {
    //        if(gridCell != null)
    //        {
    //            WorldGrid.AddToCell(this, gridCell);
    //            if (previousGridCell != null)
    //          //      WorldGrid.RemoveFromCell(this, previousGridCell);
    //            previousGridCell = gridCell;
    //        }
    //        else
    //        {
    //            Debug.Log("Something went wrong. Player doesn't have a grid cell");
    //        }
    //    }
    //    //Debug.Log("Character: " + gameObject.name);
    //    //Debug.Log("Grid cell indices: " + gridCell.index.x + "    " + gridCell.index.y);
    //    //Debug.Log("Grid cell indices: " + gridCell.index.x + "    " + gridCell.index.y);
    //    //Debug.Log("Grid character count: " + gridCell.character.Count);
    //    //Debug.Log("Grid node count: " + gridCell.node.Count);
    //
    //    //List<GridCell> cells = WorldGrid.Instance.gridArray[gridIndex.x, gridIndex.y];
    //}


}

public class PlayerNew : Character
{
    Collider2D[] info;
    private void Start()
    {
        Debug.Log("script is working");
        State = new IdleState();

        ceilingCheck = gameObject.transform.Find("CeilingCheck").gameObject;
        groundCheck = gameObject.transform.Find("GroundCheck").gameObject;

        playerSprite = gameObject.transform.GetChild(0).gameObject;

        playerAudio = GetComponent<PlayerAudio>();
        audioSource = GetComponent<AudioSource>();

        StateList = new List<States>();
        StateList.Add(new IdleState());
        
    }
    
    private void Update()
    {
        //This has the information of all the object the player is currently in contact with
        info = CastGroundOverlapCircle();

        GetInput();
        //State.UpdateState(this, userInputs);
        List<States> tempStates = StateList;
         
        //Debug.Log("States count: " + StateList.Count);
        for (int i = 0; i < StateList.Count; i++)
            StateList[i].UpdateState(this, userInputs, info);
        
        MoveCharacter();

        
    }

    private void LateUpdate()
    {
        UpdatePositionInWorld();

        
        //SetGridIndex();
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
        if (Input.GetKeyUp(KeyCode.Space) || VirtualJoystick.jumpButtonUp)
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
        else
        {
            userInputs.climbPressed = false;
            //Debug.Log("Special button to false");
        }

        //Absorbing input
        if (Input.GetKeyDown(KeyCode.V))
        {
            userInputs.absorbPressed = true;
        }
        else
        {
            userInputs.absorbPressed = false;
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
    }

    
}

