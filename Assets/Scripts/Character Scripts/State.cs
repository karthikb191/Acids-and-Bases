using System.Collections.Generic;
using System.Linq;
using UnityEngine;

interface IEquals<T>
{
    bool Equals(T state);
}

public abstract class States : IEquals<System.Type>
{
    protected RaycastHit2D[] info;
    //public States previousState;
    //public States nextState;
    public int index = 1;
    protected float angle = 0;
    protected float yCorr = 0;

    public virtual void UpdateState(Character c, UserInput input, RaycastHit2D[] info)
    {
        CharacterUtility.CastRayAndOrientPlayer(c, info, out angle, out yCorr);
        //Animation for touching the ground is false by default
        c.playerSprite.GetComponent<Animator>().SetFloat("JumpSpeed", c.currentJumpSpeed);
        
    }
    
    public bool Equals(System.Type state)
    {
        States s = this;
        return state == s.GetType() ? true : false;
    }

}

public class IdleState: States
{
    bool contactWithLadder = false;
    Platform ladder = null;
    //RaycastHit2D[] info;
    public override void UpdateState(Character c, UserInput input, RaycastHit2D[] info)
    {
        this.info = info;
        base.UpdateState(c, input, info);
        //Debug.Log("Idle state updating");
        c.currentLinearSpeed = input.xInput * c.maxSpeed * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime;
        
        if (c.StateList.Count == index)
        {
            //Check if character is within the ladder's proximity
            //Ladder can only be climbed when the character is idle
            contactWithLadder = CheckLadderProximity();
            
            //Audio
            c.SetSoundEffect();
            //animation
            AnimateState(c);
        }

        //State Exit conditions
        if (input.xInput != 0 && c.StateList.Count == index)
        {
            //Debug.Log("Running");
            //c.State = null;
            //c.State = new RunningState();
            //Debug.Log("runnnn");
            States nextState = new RunningState();
            nextState.index = c.StateList.Count + 1;
            c.StateList.Add(nextState);
        }

        if (input.jumpPressed && c.StateList.Count == index)
        {
            //c.State = null;
            //c.State = new JumpingState();
            States nextState = new JumpingState();
            nextState.index = c.StateList.Count + 1;
            c.StateList.Add(nextState);
        }
        
        //Shifting to climbing state
        if(contactWithLadder && input.climbPressed && c.StateList.Count == index)
        {

            Debug.Log("climbing state initiated");
            States nextState = new ClimbingState(ladder, c);
            nextState.index = c.StateList.Count + 1;
            c.StateList.Add(nextState);
        }
    }

    public bool CheckLadderProximity()
    {
        for(int i = 0; i < info.Length; i++)
        {
            //if (info[i].GetComponent<Collider2D>().tag == "tag_ladder")
            if (info[i].collider.tag == "tag_ladder")
            {
                ladder = info[i].collider.GetComponent<Platform>();
                return contactWithLadder = true;
                //TODO: Display the virtual button for the ladder climb here and feed the appropriate function to the button
            }
        }
        return false;
    }

    public void AnimateState(Character c)
    {
        c.playerSprite.GetComponent<Animator>().SetBool("Walk", false);
        c.playerSprite.GetComponent<Animator>().SetBool("BottomContact", true);
    }
    
}

public class RunningState : States
{
    //RaycastHit2D[] info;
    public override void UpdateState(Character c, UserInput input, RaycastHit2D[] info)
    {
        this.info = info;
        //base.UpdateState(c, input);

        
        //Debug.Log("Running state updating " + c.currentLinearSpeed);
        
        if (c.currentLinearSpeed > 0)
        {
            c.playerSprite.transform.localScale = new Vector3(1, 1, 1);
        }
        if (c.currentLinearSpeed < 0)
        {
            c.playerSprite.transform.localScale = new Vector3(-1, 1, 1);
        }

        if(c.StateList.Count == index)
        {
            //animation
            AnimateState(c);
            //sound
            c.SetSoundEffect(c.playerAudio.walk, true, false, 0.15f);

            //Debug.Log("sdfssfsfsfsfsfsfsf");
            //Exit conditions
            //if (c.State.GetType() != typeof(FallingState))
            //{
            //Debug.Log("x input: " + input.xInput);
            if (input.xInput == 0)
            {
                //c.State = null;
                //c.State = new IdleState();
                c.StateList.RemoveAt(c.StateList.Count - 1);
                //Debug.Log("Removed");

            }
            if (input.jumpPressed)
            {
                //c.State = null;
                //c.State = new JumpingState();
                States nextState = new JumpingState();
                nextState.index = c.StateList.Count + 1;
                c.StateList.Add(nextState);
                return;
            }

            if (info.Length == 0 && c.StateList[c.StateList.Count - 1].GetType() != typeof(FallingState))
            {
                States nextState = new FallingState(1);
                nextState.index = c.StateList.Count + 1;
                c.StateList.Add(nextState);
                return;
            }
            
        }
        
    }

    public void AnimateState(Character c)
    {
        c.playerSprite.GetComponent<Animator>().SetBool("Walk", true);
        
        c.playerSprite.GetComponent<Animator>().SetBool("BottomContact", true);
        
    }
}

public class JumpingState : States
{
    float jumpDamper = 1;
    bool jumping = false;
    //RaycastHit2D[] info;
    public override void UpdateState(Character c, UserInput input, RaycastHit2D[] info)
    {
        this.info = info;
        //Debug.Log("Jumping state updating");

        //c.currentLinearSpeed = input.xInput * c.maxSpeed * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime;

        
        if(input.jumpPressed && input.jumpReleased && !jumping)
        {
            //sound must only be played once
            c.SetSoundEffect(c.playerAudio.jump, false, false);

            jumping = true;
            c.currentJumpSpeed = c.maxJumpSpeed * 0.03f * 0.03f;
            //This is set so that the jump in the next frame is not detected
            input.jumpReleased = false;
        }

        //setting the animation
        AnimateState(c);

        if (input.jumpPressed)
        {
            if (jumpDamper > 0.35f)
            {
                jumpDamper -= c.jumpMultiplier * GameManager.Instance.DeltaTime;
            }
            else
            {
                jumpDamper = 0.35f;
            }
        }
        
        //There has to be a default downwardforce acting all the time, which is gravity
        c.currentJumpSpeed += c.gravity * 4.5f * jumpDamper * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime;


        if (c.StateList.Count == index)
        {
            
            Collider2D upwardCollision = Physics2D.OverlapCircle(c.ceilingCheck.transform.position, c.groundCheckCircleRadius, LayerMask.GetMask("Platform"));
            //Debug.Log("upward collision: " + upwardCollision);
            
            if (upwardCollision != null && c.currentJumpSpeed > 0)
            {
                Debug.Log("Upward collision is not null");
                c.currentJumpSpeed = 0;
                //c.State = null;
                //c.State = new FallingState(1);
                c.StateList.RemoveAt(c.StateList.Count - 1);
                States nextState = new FallingState(1);
                nextState.index = c.StateList.Count + 1;
                c.StateList.Add(nextState);
            }
            else if (c.currentJumpSpeed < 0)
            {
                //c.State = null;
                //c.State = new FallingState(1);
                c.StateList.RemoveAt(c.StateList.Count - 1);
                States nextState = new FallingState(1);
                nextState.index = c.StateList.Count + 1;
                c.StateList.Add(nextState);
            }
        }
    }
    public void AnimateState(Character c)
    {
        c.playerSprite.GetComponent<Animator>().SetBool("BottomContact", false);
        c.playerSprite.GetComponent<Animator>().SetFloat("JumpSpeed", c.currentJumpSpeed);
    }

}

public class FallingState : States
{
    float jumpDamper = 1;
    float fallTimer = 0;
    //RaycastHit2D[] info;
    public FallingState(float JumpDamper)
    {
        jumpDamper = JumpDamper;
    }

    public override void UpdateState(Character c, UserInput input, RaycastHit2D[] info)
    {
        this.info = info;
        //Debug.Log("Falling state updating");
        //base.UpdateState(c, input);

        fallTimer += GameManager.Instance.DeltaTime;

        if (jumpDamper < 1)
        {
            jumpDamper += c.jumpMultiplier * 2 * GameManager.Instance.DeltaTime;
        }
        else
        {
            jumpDamper = 1;
        }

        c.currentJumpSpeed += c.gravity * 11.5f * jumpDamper * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime;

        if (Mathf.Sign(c.currentJumpSpeed) < 0 && Mathf.Abs(c.currentJumpSpeed) > 0.45f)
            c.currentJumpSpeed = -0.45f;


        //c.currentLinearSpeed = input.xInput * c.maxSpeed * GameManager.Instance.DeltaTime * GameManager.Instance.DeltaTime;
        

        //Debug.Log("Fall timer: " + fallTimer);

        //float angle = 0;
        //float yCorr = 0;
        //CharacterUtility.CastRayAndOrientPlayer(c, out angle, out yCorr);
        //Debug.Log("Angle: " + angle);

        if (c.StateList.Count == index)
        {
            //animation
            AnimateState(c);
            //Music
            c.SetSoundEffect(c.playerAudio.fall, true, false, 1.5f);
        }

        if (info.Length > 0)
        {
            int count = 0;
            for (int i = 0; i < info.Length; i++)
            {
                if(info[i].collider.gameObject.layer != LayerMask.NameToLayer("Platform"))
                {
                    count++;
                }
            }
            if (count == info.Length)
                return;

            if (angle < 65)
            {
                //Pause player movement for a while
                if (fallTimer > 0.08f)
                {
                    c.BlockInputs();
                    //Debug.Log("blocking inputs");
                }

                //c.State = null;
                //c.State = new LandState();
                if (c.StateList.Count == index)
                {
                    c.currentJumpSpeed = yCorr;

                    c.StateList.RemoveAt(c.StateList.Count - 1);

                    States nextState = new LandState();
                    nextState.index = c.StateList.Count + 1;
                    c.StateList.Add(nextState);
                }

                if (Mathf.Abs(c.currentJumpSpeed) > 0)
                {
                    c.currentJumpSpeed = 0;
                }
                
            }
        }
        
        //If the angle with the collider is greater than 65, then set the state as falling cuz player wont be able to walk on it
        
    }

    public void AnimateState(Character c)
    {
        c.playerSprite.GetComponent<Animator>().SetFloat("JumpSpeed", c.currentJumpSpeed);
        c.playerSprite.GetComponent<Animator>().SetBool("BottomContact", false);
    }

}

public class LandState : States
{
    //RaycastHit2D[] info;
    public override void UpdateState(Character c, UserInput input, RaycastHit2D[] info)
    {
        this.info = info;
        //play the sound effect for landing
        c.SetSoundEffect(c.playerAudio.landOnLand, false, false);
        
        //TODO: This must be changed to a proper place
        c.playerSprite.GetComponent<Animator>().SetBool("BottomContact", true);

        //amount of time taken to go back to idle state depends on the block status
        //TODO: Change this to more intuitive form
        if (c.horizontalInputBlock)
            return;
        if (c.StateList.Count == index)
        {
            if (c.currentLinearSpeed == 0)
            {
                //c.State = null;
                //c.State = new IdleState();
                c.StateList.RemoveAt(c.StateList.Count - 1);
                //c.StateList.Add(new IdleState());
            }
            else
            {
                //c.State = null;
                //c.State = new RunningState();
                c.StateList.RemoveAt(c.StateList.Count - 1);
                if(c.StateList[c.StateList.Count-1].GetType() != typeof(RunningState))
                {
                    States nextState = new RunningState();
                    nextState.index = c.StateList.Count + 1;
                    c.StateList.Add(nextState);
                }
            }
        }
    }
}

public class ClimbingState : States
{

    Platform ladder;
    public ClimbingState(Platform l, Character c)
    {
        ladder = l;
        //This call to virtual joystick rotates the arrows
        VirtualJoystick.RotateArrows(90, c);
    }

    bool goingUp = true;
    Node bottomNode = null;
    Node topNode = null;

    bool directionSet = false;

    float climbSpeed = 3;

    float minimumDifference = 0.15f;

    bool firstUpdateFinished = false;
    //RaycastHit2D[] info;
    public override void UpdateState(Character c, UserInput input, RaycastHit2D[] info)
    {
        this.info = info;

        //Debug.Log("Updating the Climbing state");
        if(!directionSet)
            goingUp  = CheckThePossibleDirection(c);

        if (input.specialButtonPressed)
            goingUp = !goingUp;

        Move(c, input);

        if(c.StateList.Count == index && firstUpdateFinished)
            StateExitConditions(c, input);

        firstUpdateFinished = true;
    }

    bool CheckThePossibleDirection(Character c)
    {
        directionSet = true;

        Vector3 directionToLeftNode = ladder.leftNode.position - c.transform.position;
        Vector3 directionToRightNode = ladder.rightNode.position - c.transform.position;
        
        if(Vector3.SqrMagnitude(directionToLeftNode) > Vector3.SqrMagnitude(directionToRightNode))
        {
            //Left node is further from the character than the right node
            if (directionToLeftNode.y > 0)
            {
                topNode = ladder.leftNode;
                bottomNode = ladder.rightNode;
                return true;
            }
            else
            {
                topNode = ladder.rightNode;
                bottomNode = ladder.leftNode;
                return false;
            }
        }
        else
        {
            //Right node is further from the character than the left node
            if (directionToRightNode.y > 0)
            {
                topNode = ladder.rightNode;
                bottomNode = ladder.leftNode;
                return true;
            }
            else
            {
                topNode = ladder.leftNode;
                bottomNode = ladder.rightNode;
                return false;
            }
        }
    }

    void Move(Character c, UserInput input)
    {
        //Reach the X position of the ladder first
        c.currentLinearSpeed = 0;
        c.currentJumpSpeed = 0;

        if (Mathf.Abs(ladder.transform.position.x - c.transform.position.x) > 0.02f)
        {
            c.currentLinearSpeed = (ladder.transform.position.x - c.transform.position.x) * 5 * Time.deltaTime;
        }

        c.currentJumpSpeed = input.xInput * Time.deltaTime * climbSpeed;
    }

    void StateExitConditions(Character c, UserInput input)
    {
        //If the special button pressed again while climbing, set the state to fall
        if (input.climbPressed)
        {
            VirtualJoystick.ResetArrows(c);
            Debug.Log("Falling from climbing");
            c.StateList.RemoveAt(c.StateList.Count - 1);
            //Add the falling state
            States nextState = new FallingState(1);
            //States nextState = new FallingState(1);
            nextState.index = c.StateList.Count + 1;
            c.StateList.Add(nextState);
        }
        else if(input.jumpPressed && input.jumpReleased)
        {
            VirtualJoystick.ResetArrows(c);
            Debug.Log("Transition to jump state");
            c.StateList.RemoveAt(c.StateList.Count - 1);
            //Add the falling state
            States nextState = new JumpingState();
            //States nextState = new FallingState(1);
            nextState.index = c.StateList.Count + 1;
            c.StateList.Add(nextState);
        }
        else if(input.xInput < 0 && (Mathf.Abs(bottomNode.position.y - c.transform.position.y) < minimumDifference ||
             c.transform.position.y < bottomNode.position.y))
        {
            Debug.Log("X input down");
            VirtualJoystick.ResetArrows(c);
            //If the character is closer to the bottom node and is trying to go down, 
            //transition to idle state
            c.StateList.RemoveAt(c.StateList.Count - 1);
            c.BlockInputs();
            //Add the falling state
            //States nextState = new LandState();
            States nextState = new FallingState(1);
            nextState.index = c.StateList.Count + 1;
            c.StateList.Add(nextState);
        }
        else if(Mathf.Abs(topNode.position.y - c.transform.position.y) < minimumDifference || c.transform.position.y > topNode.position.y)
        {
            Debug.Log("reached top");
            VirtualJoystick.ResetArrows(c);
            //Player state is set to falling if he attempts to go beyond the ladder proximity
            c.StateList.RemoveAt(c.StateList.Count - 1);
            c.currentJumpSpeed = 0;
            //Add the falling state
            States nextState = new FallingState(1);
            //States nextState = new FallingState(1);
            nextState.index = c.StateList.Count + 1;
            c.StateList.Add(nextState);
        }
    }

}