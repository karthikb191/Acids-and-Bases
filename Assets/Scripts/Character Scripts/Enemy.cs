using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CharacterType
{
    neutral,
    acidic,
    basic
}

[System.Serializable]
class Enemy : EnemyBase
{
    protected Enemy enemy;

    AI aiComponent;
    [HideInInspector]
    public EnemyBehaviorAI behaviorAI;

    public bool promptClimb;
    public bool climbing;   //Must be set by enemy behavior

    public CharacterType characterType = CharacterType.acidic;

    RaycastHit2D[] info;

    bool halt = false;

    private void Start()
    {
        inventory = GetComponentInChildren<Inventory>();
        //if (inventory != null)
        //{
        //    enemyItem.itemProperties.maxHoldingCapacity = 10000;
        //    inventory.AddItem(enemyItem);
        //    Debug.Log("Inventory is not null and item added");
        //}

        enemy = this;
        aiComponent = GetComponent<AI>();
        behaviorAI = new RoamingBehavoir(enemy, aiComponent);

        Debug.Log("script is working");
        State = new IdleState();

        ceilingCheck = gameObject.transform.Find("CeilingCheck").gameObject;
        groundCheck = gameObject.transform.Find("GroundCheck").gameObject;

        playerSprite = transform.Find("Sprite").gameObject;
        Hand = playerSprite.transform.Find("Hand").gameObject;

        playerAudio = GetComponent<PlayerAudio>();
        audioSource = GetComponent<AudioSource>();

        info = CastGroundOverlapCircle();

        StateList = new List<States>();
        StateList.Add(new IdleState());
        StateList[0].UpdateState(this, userInputs, info);
    }

    private void Update()
    {

        info = CastGroundOverlapCircle();
        //State = StateList[StateList.Count - 1];

        if (halt)
            return;

        GetInput();
        //State.UpdateState(this, userInputs);
        List<States> tempStates = StateList;

        //Debug.Log("States count: " + StateList.Count);
        for (int i = 0; i < StateList.Count; i++)
            StateList[i].UpdateState(this, userInputs, info);

        //Update behavior
        if (behaviorAI != null)
            behaviorAI.BehaviorUpdate();


        MoveCharacter();

        State = StateList[StateList.Count - 1];

        //Debug.Log("Stunned: " + IsStunned());
    }

    private void LateUpdate()
    {
        UpdatePositionInWorld();
    }

    //bool promptClimb = false;
    protected override void GetInput()
    {
        userInputs.xInput = aiComponent.horizontalMovement;
        if (aiComponent.jump)
        {
            userInputs.jumpPressed = true;
            //Debug.Log("jump button release: " + jumpButtonReleased);
            //Debug.Log("jump button: " + jumpButtonPresssed);
        }
        if (aiComponent.jumpRelease)
        {
            userInputs.jumpReleased = true;
            userInputs.jumpPressed = false;
        }

        //The prompt climb input is obtained from the states itself once the character is close to the ladder
        for (int i = 0; i < info.Length; i++)
        {
            if (info[i].collider.tag == "tag_ladder")
            {
                promptClimb = true;
                climbing = true;        //This should later be set by the Enemy behavior script depending on climb prompt
            }
        }
        //Debug.Log("character can climb: " + aiComponent.characterCanClimb);
        if (promptClimb && !userInputs.climbPressed && aiComponent.characterCanClimb && !State.Equals(typeof(ClimbingState)))
        {
            promptClimb = false;
            userInputs.climbPressed = true;
            //Debug.Log("Input has been set to climbing");
        }
        else
        {
            //Debug.Log("input has been set to not clinbimg");
            userInputs.climbPressed = false;
        }

        //Debug.Log("Climb prompt is: " + promptClimb);
    }

    public override void UseItem()
    {
        //TODO: add the condition that checks if item has a stun variable
        Stun(5.0f);
    }

    #region Stun Scripts
    private void Stun(float duration)
    {
        //Revert the enemy behavior to roaming for now. Later there might be a separate stun behavior
        if (behaviorAI.GetType() != typeof(StunnedBehavior))
        {
            //StartCoroutine(StunResetCoroutine(duration));
            BlockInputs(duration, true, false);
            behaviorAI = new StunnedBehavior(enemy, aiComponent, duration);
        }
    }

    public bool IsStunned()
    {
        return stunned;
    }

    public void SetStunned(bool b)
    {
        stunned = b;
    }

    IEnumerator StunResetCoroutine(float duration)
    {
        stunned = true;
        yield return new WaitForSeconds(duration);
        stunned = false;
    }
    #endregion

    public void Halt()
    {
        halt = true;
        playerSprite.GetComponent<Animator>().speed = 0;
    }
    public void ResetHalt()
    {
        halt = false;
        playerSprite.GetComponent<Animator>().speed = 1;
    }

    public void GettingAbsorbed(Character c, float duration)
    {
        //Change the behavior to getting absorbed
        behaviorAI = null;
        c.BlockInputs(duration, true, true);    //Blocking player's inputs
        BlockInputs(duration, true, true);      //Blocking character's inputs also
        behaviorAI = new GettingAbsorbedBehavior(enemy, aiComponent, c, duration);
    }

    public void Attack(Character c)
    {
        //Attack a specific character
        Debug.Log("Attacking character: " + c.name);
    }

    public void ThrowAttack(Character c)
    {
        Debug.Log("Throwing item and attacking the proximity character");
        if (inventory != null)
            if (inventory.activeItem != null)
                inventory.ThrowItem(c.transform.position, this);
    }

    public void Die()
    {
        //TODO: Add particles and animation
        gameObject.SetActive(false);
    }

    protected override Vector3 GoBackToOriginalPosition(Node homeNode)
    {
        Vector3 result = Vector3.zero;
        return result;
    }

    protected override Node ResetHomePosition()
    {
        return null;
    }

    protected override void RoamAround()
    {

    }

    protected override void ChasePlayer(bool playerFound)
    {

    }

    protected override void UpdateChase(bool chasing)
    {

    }

    protected override void UseSkill(bool useSkill)
    {

    }

    protected override void Search(float coolDown, bool searching)
    {

    }

    protected override void GiveUpChase()
    {

    }
    
    public void ResetAI()
    {
        aiComponent.ResetChaseAndRunAway();
    }
    
}