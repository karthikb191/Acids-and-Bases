using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

abstract class EnemyBehaviorAI
{
    protected Enemy character;
    protected AI aiComponent;

    protected bool forceChase;
    
    public EnemyBehaviorAI(Enemy e, AI ai)
    {
        character = e;
        aiComponent = ai;
    }
    public int aggressionLevel = 1;
    public int mood = 0;    //Negative mood leads to depressions and higher positive mood leads to anger and aggression
    public abstract void BehaviorUpdate();
    public abstract void BehaviorExitConditions();
    
    public static void ForceChaseBehavior(Enemy e, Character chaseCharacter)
    {
        Debug.Log("Forced behavior");
        e.GetComponent<AI>().ResetChaseAndRunAway();
        e.behaviorAI = new ChasingBehavior(e, e.GetComponent<AI>(), chaseCharacter);
    }
}

class RoamingBehavoir : EnemyBehaviorAI
{
    protected List<Character> charactersWithinProximity;
    public RoamingBehavoir(Enemy e, AI ai) : base(e, ai)
    {
        character = e;
        aiComponent = ai;
    }
    public override void BehaviorUpdate()
    {
        //Nothing very interesting here. He will occasionally pause and traverse his path again
        //He will also be on lookout for the possible character encounters
        charactersWithinProximity = FindCharacters();
        //Debug.Log("Roaming");
        BehaviorExitConditions();
    }

    List<Character> FindCharacters()
    {
        //Get the grid the character is currently in
        GridIndex index = WorldGrid.GetGridIndex(character.transform.position);
        GridCell cell = WorldGrid.GetTheWorldGridCell(index);

        //check if the cell position is within the search radius
        //bool xSearch = true, ySearch = true;
        //add all characters to a list
        List<Character> proximityCharacters = new List<Character>();

        //TODO: add more intuitive and wider search logic depending on character behavior
        if(character.gridCell != null)
            if (character.gridCell.character.Count > 0)
            {
                //Remove yourself from the characters list
                proximityCharacters.AddRange(character.gridCell.character);
                //Debug.Log("count : " + proximityCharacters.Count);
                //Debug.Log("names: " + proximityCharacters[0].name + "   " + character.name);
                for (int i = 0; i < proximityCharacters.Count; i++)
                    if (proximityCharacters[i].gameObject == character.gameObject)
                    {
                        proximityCharacters.RemoveAt(i);
                        //Debug.Log("Removed");
                    }
            }

        //TODO: Filter the characters to find the ones that are withing this character's FOV
        return proximityCharacters;
    }


    public override void BehaviorExitConditions()
    {
        //Behavior Changes when he finds another character
        //Add a functionality that enables him to detect other characters
        
        if(charactersWithinProximity.Count > 0)
        {
            character.behaviorAI = null;
            character.behaviorAI = new SpottedBehavior(character, aiComponent, charactersWithinProximity);
        }
    }
}

//This class decides what action the character should take. Chase or run away or don't care at all
class SpottedBehavior : EnemyBehaviorAI
{
    protected List<Character> charactersWithinProximity;
    protected Character chasingCharacter = null;

    public SpottedBehavior(Enemy e, AI ai, List<Character> characters) : base(e, ai)
    {
        character = e; aiComponent = ai;
        charactersWithinProximity = characters;
    }
    public override void BehaviorUpdate()
    {
        
        //Debug.Log("Spotted Behavior");
        //We are searching for player character for now. More behavior will be added later
        foreach(Character c in charactersWithinProximity)
        {
            //Player p = c.GetComponent<Player>();
            if (c.GetComponent<Player>())
            {
                Debug.Log("Player exists: " + c);
                chasingCharacter = c;
                break;
            }
            //TODO: Add much more interesting behavior for selecting the character to chase
            //For now, if no player is found, the interaction character is set to the last one occuring in the loop
            chasingCharacter = c;
        }

        BehaviorExitConditions();
    }

    public override void BehaviorExitConditions()
    {
        //TODO: Add more interesting behavior later
        if (aiComponent.ignoreOtherEnemies && chasingCharacter != null)
        {
            Debug.Log("Chase started");
            aiComponent.chaseStarted = true;
            character.behaviorAI = new ChasingBehavior(character, aiComponent, chasingCharacter);

            Debug.Log("Character Added");
            
        }

        if (chasingCharacter != null)
        {
            if (character.characterType == CharacterType.acidic)
            {
                if (chasingCharacter.GetComponent<Player>())
                {
                    Debug.Log("Chase started");
                    aiComponent.chaseStarted = true;
                    character.behaviorAI = new ChasingBehavior(character, aiComponent, chasingCharacter);

                    Debug.Log("Character Added");
                    //Add the enemy character to player's chasing characters list
                    //chasingCharacter.GetComponent<Player>().AddEnemyChasingPlayer(character);
                }
                //If acid spots a base, then start the chase
                else 
                if (chasingCharacter.GetComponent<Enemy>() && !aiComponent.ignoreOtherEnemies)
                    if(chasingCharacter.GetComponent<Enemy>().characterType == CharacterType.basic)
                    {
                        Debug.Log("Chase started");
                        aiComponent.chaseStarted = true;
                        character.behaviorAI = new ChasingBehavior(character, aiComponent, chasingCharacter);
                    }
                    
            }
            else if (character.characterType == CharacterType.basic)
            {
                Debug.Log("basic");
                if (chasingCharacter.GetComponent<Player>())
                {
                    Debug.Log("Run away");
                    aiComponent.runaway = true;
                    character.behaviorAI = new RunAwayBehavior(character, aiComponent, chasingCharacter);

                    //Add the enemy character to player's chasing characters list
                    //chasingCharacter.GetComponent<Player>().AddEnemyChasingPlayer(character);
                }

                else
                if(chasingCharacter.GetComponent<Enemy>() && !aiComponent.ignoreOtherEnemies)
                    if (chasingCharacter.GetComponent<Enemy>().characterType == CharacterType.acidic)
                    {
                        aiComponent.runaway = true;
                        character.behaviorAI = new RunAwayBehavior(character, aiComponent, chasingCharacter);
                    }
            }
        }
        else
        {
            //If the character is not found, then return to roaming stage
            aiComponent.chaseStarted = false;
            aiComponent.runaway = false;
            character.behaviorAI = new RoamingBehavoir(character, aiComponent);
        }
    }
}

class ChasingBehavior : EnemyBehaviorAI
{
    Character chasingCharacter;
    
    public ChasingBehavior(Enemy e, AI ai, Character c) : base(e, ai)
    {
        character = e; aiComponent = ai;
        chasingCharacter = c;
        aiComponent.PrepareForEncounter();

        //Add the enemy character to player's chasing characters list
        if (chasingCharacter.GetComponent<Player>() != null)
            chasingCharacter.GetComponent<Player>().AddEnemyChasingPlayer(character);
    }
    public override void BehaviorUpdate()
    {
        //Debug.Log("Chasing Behavior");
        aiComponent.chaseStarted = true;
        aiComponent.chasingCharacter = chasingCharacter;

        BehaviorExitConditions();
    }

    public override void BehaviorExitConditions()
    {
        bool reset = false;
        Vector3 directionToCharacter = chasingCharacter.transform.position - character.transform.position;

        //TODO: Add attack pattern for base here

        //If the distance to the chasing character is less than a certain value, try to attack the character
        //TODO: Add more interesting attack pattern
        if (Vector3.SqrMagnitude(directionToCharacter) < Random.Range(5, 10))
        {
            //Shift to Attacking behavior
            character.behaviorAI = null;
            character.behaviorAI = new AttackingBehavior(character, aiComponent, chasingCharacter);
            //reset = true;
            //aiComponent.ChaseReset();   //Reset chase so that no new nodes are added in the AI component
        }

        if (Vector3.SqrMagnitude(directionToCharacter) > Random.Range(90, 100) && !aiComponent.persistentChase)
        {
            character.behaviorAI = null;
            character.behaviorAI = new RoamingBehavoir(character, aiComponent);
            reset = true;
            aiComponent.ChaseReset();
        }

        if (reset)
        {
            if (chasingCharacter.GetComponent<Player>())
            {
                Debug.Log("Character removed");
                chasingCharacter.GetComponent<Player>().RemoveEnemyChasingPlayer(character);
            }
        }
    }
}

class RunAwayBehavior : EnemyBehaviorAI
{
    Character chasingCharacter;
    public RunAwayBehavior(Enemy e, AI ai, Character c) : base(e, ai)
    {
        character = e; aiComponent = ai;
        chasingCharacter = c;
        ai.PrepareForEncounter();
    }

    public override void BehaviorUpdate()
    {
        aiComponent.runaway = true;
        aiComponent.chasingCharacter = chasingCharacter;
        BehaviorExitConditions();
    }

    public override void BehaviorExitConditions()
    {
        Vector3 directionToCharacter = chasingCharacter.transform.position - character.transform.position;
        if (Vector3.SqrMagnitude(directionToCharacter) < Random.Range(25, 35) &&
                Vector3.SqrMagnitude(directionToCharacter) > Random.Range(20, 25))
        {
            //Shift to Attacking behavior
            character.behaviorAI = null;
            character.behaviorAI = new AttackingBehavior(character, aiComponent, chasingCharacter);
            return;
        }

        if (RunAwayReset())
        {
            if (!aiComponent.persistentChase)
            {
                character.behaviorAI = null;
                character.behaviorAI = new RoamingBehavoir(character, aiComponent);
            }
            else
            {
                character.behaviorAI = new ChasingBehavior(character, aiComponent, chasingCharacter);
            }
        }
    }

    bool RunAwayReset()
    {
        Vector3 directionToCharacter = chasingCharacter.transform.position - character.transform.position;
        
        if (Vector3.SqrMagnitude(directionToCharacter) > Random.Range(90, 100))
        {
            aiComponent.RunAwayReset();
            //aiComponent.runaway = false;
            //aiComponent.chasingCharacter = null;
            return true;
        }
        return false;
    }

}

class StunnedBehavior : EnemyBehaviorAI
{
    float duration = 1.0f;
    float timeElapsed = 0.0f;

    public StunnedBehavior(Enemy e, AI ai, float duration) : base(e, ai)
    {
        character = e; aiComponent = ai;
        this.duration = duration;
        character.SetStunned(true);
    }

    public override void BehaviorUpdate()
    {
        //Debug.Log("Stunned");
        if(timeElapsed < duration)
        {
            timeElapsed += GameManager.Instance.DeltaTime;
            if(character.IsStunned())
            {
                character.playerSprite.GetComponent<Animator>().speed = 0;
            }
        }
        else
        {
            //character.SetStunned(false);
            BehaviorExitConditions();
        }
    }
    public override void BehaviorExitConditions()
    {
        //if(!character.IsStunned())
        //{
        character.SetStunned(false);
        character.playerSprite.GetComponent<Animator>().speed = 1;
        character.behaviorAI = new RoamingBehavoir(character, aiComponent);
        //}
    }
}

class GettingAbsorbedBehavior : EnemyBehaviorAI
{
    Character characterAbsorbing;
    float time = 3.0f;
    float passedTime = 0.0f;
    public GettingAbsorbedBehavior(Enemy e, AI ai, Character c, float t) : base(e, ai)
    {
        character = e; aiComponent = ai;
        characterAbsorbing = c;
        time = t;
    }
    public override void BehaviorUpdate()
    {
        //Debug.Log("Character is being absorbed......");
        //TODO: Need to contain the animation that brings it closer to the character absorbing
        //TODO: Need to add particle effect for disabling the enemy

        //For now, start a timer. After a while, disable the character
        if(passedTime < time)
            if (!TimerUpdate())
            {
                character.Die();    //Change this to a much more suitable function
            }
    }
    public override void BehaviorExitConditions()
    {

    }
    bool TimerUpdate()
    {
        passedTime += GameManager.Instance.DeltaTime;
        if (passedTime > time)
            return false;
        return true;
    }
}

class AttackingBehavior : EnemyBehaviorAI
{
    Character characterOnFocus = null;
    float coolDownTime = 1.5f;
    float timeElapsed = 0.0f;
    bool attacked = false;

    public AttackingBehavior(Enemy e, AI ai, Character c) : base(e, ai)
    {
        character = e; aiComponent = ai;
        characterOnFocus = c;
    }
    public override void BehaviorUpdate()
    {
        if(character.State.GetType() == typeof(IdleState) || character.State.GetType() == typeof(RunningState))
            if (timeElapsed < coolDownTime)
            {
                timeElapsed += GameManager.Instance.DeltaTime;
                if (!attacked)
                {
                    //Debug.Log("Attacked");
                    attacked = true;

                    aiComponent.HaltMovement(coolDownTime, true, false);

                    //look at the character
                    LookAtCharacterOnFocus();

                    //character.Attack(characterOnFocus);
                    character.ThrowAttack(characterOnFocus);
                }
            }
            else
            {
                timeElapsed = 0;
                BehaviorExitConditions();
            }
    }
    public void LookAtCharacterOnFocus()
    {
        int sign = (int)Mathf.Sign(characterOnFocus.transform.position.x - character.transform.position.x);
        character.playerSprite.transform.localScale = new Vector3(Mathf.Abs(character.playerSprite.transform.localScale.x) * sign,
                                            character.playerSprite.transform.localScale.y, 
                                            character.playerSprite.transform.localScale.z);
    }
    public override void BehaviorExitConditions()
    {
        //After the cool down time is achieved, check if the character is still close 
        Vector3 directionToCharacter = characterOnFocus.transform.position - character.transform.position;
        //TODO: Better character proximity detection is needed
        if (character.characterType == CharacterType.acidic)
        {
            if (Vector3.SqrMagnitude(directionToCharacter) < Random.Range(5, 10))
            {
                //Debug.Log("mag: " + Vector3.SqrMagnitude(directionToCharacter));
                Debug.Log("Trying to attack again");
                attacked = false;
                return;
            }
            else
            {
                //If the character is not within proximity, revert back to roaming
                aiComponent.ResumeMovement();
                character.behaviorAI = null;
                character.behaviorAI = new ChasingBehavior(character, aiComponent, characterOnFocus);
            }
        }
        else if (character.characterType == CharacterType.basic)
        {
            if (Vector3.SqrMagnitude(directionToCharacter) < Random.Range(25, 35) &&
                Vector3.SqrMagnitude(directionToCharacter) > Random.Range(10, 15))
            {
                
                //Debug.Log("mag: " + Vector3.SqrMagnitude(directionToCharacter));
                Debug.Log("Trying to attack again");
                
                attacked = false;
                return;
                
            }
            else
            {
                //If the character is not within proximity, revert back to roaming
                aiComponent.ResumeMovement();
                character.behaviorAI = null;
                character.behaviorAI = new RunAwayBehavior(character, aiComponent, characterOnFocus);
            }
        }
    }
}

class ApproachedCharacterBehavior : EnemyBehaviorAI
{
    public ApproachedCharacterBehavior(Enemy e, AI ai) : base(e, ai)
    {
        character = e; aiComponent = ai;
    }
    public override void BehaviorUpdate()
    {

    }
    public override void BehaviorExitConditions()
    {

    }
}

class ForceChaseBehavior : EnemyBehaviorAI
{
    Character chasingCharacter;

    public ForceChaseBehavior(Enemy e, AI ai, Character c) : base(e, ai)
    {
        forceChase = true;
        character = e; aiComponent = ai;
        chasingCharacter = c;
    }

    public override void BehaviorUpdate()
    {
        character.behaviorAI = new ChasingBehavior(character, aiComponent, chasingCharacter);
    }

    public override void BehaviorExitConditions()
    {

    }
}


