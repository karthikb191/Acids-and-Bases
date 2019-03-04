
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GrabAnimationInfo
{
    public AnimationClip grabStartAnimationClip;    //Must be set in the inspector
}
[System.Serializable]
public struct LiquidInformation
{
    //All these values must be set inside the inspector
    public float minimumPosition;
    public float maximumPosition;
}

//Try making this script independent of monoBehavior
public class PlayerMechanics : CharacterMechanics
{

    //Item variables
    public bool itemPickUpEnabled = true;
    public GameObject itemPickedUp;
    
    //Enemy Absorb variables
    public bool canAbsorbEnemy = true;
    public float absorbDistance = 1.0f;
    private DynamicButton absorbButton = null;
    private bool absorbing = false;

    public bool isInContactwithField;

    private AudioSource playerAudioSource;
    [HideInInspector]
    public Player player;
    SpriteRenderer bodySkinnedMeshRenderer;

    float fallShoutCounter;


    //Information for specific mechanics
    [SerializeField]
    public GrabAnimationInfo grabAnimationInfo;

    //Mechanics Objects
    AbsorbMechanic absMec = null;

    [SerializeField]
    public ScanMechanicVisualInfo phScanVisualInfo;

    PHScanMechanic scan = null;

    private void Start()
    {
        base.Start();
        player = GetComponent<Player>();
        
        //playerAudioSource = GetComponent<AudioSource>();
        itemPickedUp = null;   //Initially, there are no item pick ups

        //TODO: Change this to a more automated form
        SetpH(phValue);

        //bodySkinnedMeshRenderer = player.playerSprite.GetComponent<SpriteRenderer>();

        //liquidBounds = player.Liquid.GetComponent<SpriteRenderer>().bounds;
    }

    private new void Update()
    {
        base.Update();

        if (Application.isPlaying)
        {
            if (canAbsorbEnemy)
            {
                if (absMec == null)
                    AbsorbCharacter();
                else
                {
                    absMec.StunUpdate(ref absMec);
                }
            }

            //TODO: remove this input code later. This is only for testing
            if (Input.GetKeyDown(KeyCode.S))
                StartScan();

            if (scan != null)
                scan.Update(ref scan, ref player);

            SetVolume();
        }
    }


    
    private void LateUpdate()
    {
        isInContactwithField = false;
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
                    InitiateAbsorb(e, 3.0f);
                }
            }
            else if(absorbButton!=null && !absorbing)
            {
                Debug.Log("Character is not stunned");
                VirtualJoystick.DisableDynamicButton(absorbButton);
                absorbButton = null;
            }
        }
        else if (absorbButton != null && !absorbing)
        {
            Debug.Log("character is null");
            VirtualJoystick.DisableDynamicButton(absorbButton);
            absorbButton = null;
        }
    }
    
    Enemy DetectCharacter()
    {
        Enemy e = null;
        //Debug.DrawRay(new Vector3(player.transform.position.x +(player.Body.GetComponent<SkinnedMeshRenderer>().bounds.extents.x + 0.1f) * Mathf.Sign(player.playerSprite.transform.localScale.x),
        //                    player.transform.position.y + player.Body.GetComponent<SkinnedMeshRenderer>().bounds.extents.y,
        //                    player.transform.position.z), Vector3.right*100, Color.green);

        if (player.State.Equals(typeof(IdleState)))
        {
            //Debug.Log("Casting ray to find the enemy");
            //If player is idle, Cast ray directly to the player's right to detect a hit
            Vector3 pointOfCast = new Vector3(player.transform.position.x + (player.Body.GetComponent<SkinnedMeshRenderer>().bounds.extents.x + 0.1f) * Mathf.Sign(player.playerSprite.transform.localScale.x),
                            player.transform.position.y + player.Body.GetComponent<SkinnedMeshRenderer>().bounds.extents.y,
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
        absorbButton = VirtualJoystick.CreateDynamicButton("tag_absorb");
        if (!absorbButton.active)
        {
            VirtualJoystick.EnableDynamicButton(absorbButton);
            absorbButton.button.onClick.AddListener(() =>
            {
                player.userInputs.absorbPressed = true;
                Debug.Log("Absorb pressed");
                //Disable the button
                VirtualJoystick.DisableDynamicButton(absorbButton);
                absorbButton = null;
            });
        }
    }

    void InitiateAbsorb(Character c, float duration)
    {
        Debug.Log("initiated the absorb mechanic");
        absMec = new AbsorbMechanic();
        absMec.Setup(this, c, grabAnimationInfo);
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

            //Debug.Log("Normalized state time: " + stateInfo.normalizedTime);
            if (stageOneEntered)
            {
                if (!clipSet)
                {
                    Debug.Log("Setting the clip: " + grabanimationHash);

                    //AnimatorClipInfo[] clips = animator.GetNextAnimatorClipInfo(0);
                    stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    if(stateInfo.IsName("Grab"))
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
                    //After ta certain amount of time, set the enemy as child to the player's hand
                    if (elapsedTime > time * 0.4f)
                    {
                        //Halt character movement of both the player and the enemy
                        c.GetComponent<Enemy>().GettingAbsorbed(player, time);

                        c.GetComponent<Collider2D>().enabled = false;
                        Debug.Log("Parent set");
                        c.transform.parent = player.Hand.transform;
                        c.transform.localPosition = new Vector3(0, -1.0f, 0);
                        c.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        //c.playerSprite.GetComponent<SpriteRenderer>().sortingOrder = player.playerSprite.GetComponent<SpriteRenderer>().sortingOrder - 1;

                        parentSet = true;
                    }
                    return;
                }

                if (elapsedTime > time)
                {
                    animator.SetBool("Absorb", false);

                    if(!stateInfo.IsName("Grab") && !animator.IsInTransition(0))
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

            SaltsList resultantSalt = SaltsList.NaCl;
            //System.Enum.TryParse(Reactions.reactionDictionary[player.chemical][c.chemical].ToString(), out resultantSalt);
            System.Enum.TryParse(Reactions.React(player.chemical, c.chemical).ToString(), out resultantSalt);
            //resultantSalt = System.Enum.Parse(typeof(Salt), Reactions.reactionDictionary[player.chemical][c.chemical].ToString());

            if (resultantSalt != SaltsList.Null)
            {
                Debug.Log("salt generated: " + resultantSalt);
            }
            else
            {
                Debug.Log("Salt is null....Something went wrong");
            }
        }
        
    }
    
    #region pH Scan Functions

    public void StartScan()
    {
        if (scan == null)
            scan = new PHScanMechanic(player.GetGridCell(), player.transform.position, (int)player.playerSprite.transform.localScale.x,
                                        phScanVisualInfo);
        else
            Debug.Log("Scan is already in progress");
    }

    [System.Serializable]
    public class ScanMechanicVisualInfo
    {
        public GameObject scanVisual;
        public float visualScaleMultiplier = 1.0f;
        public float finalFadeMultiplier = 4.0f;
        public bool ignorepHLimit = true;
    }
    
    public class PHScanMechanic
    {
        public PHScanMechanic(GridCell gridCell, Vector3 startPosition, int direction, ScanMechanicVisualInfo visualInfo)
        {
            this.gridCell = gridCell;
            this.startPosition = startPosition;
            currentScanPosition = this.startPosition;
            this.direction = direction;
            previousScanPosition = currentScanPosition;

            verticalNodeSearchCount = Mathf.FloorToInt(verticalReach / WorldGrid.Instance.gridSize);
            horizontalNodeSearchCount = Mathf.FloorToInt(horizontalReach / WorldGrid.Instance.gridSize);

            scanVisualObject = Instantiate(visualInfo.scanVisual, startPosition, Quaternion.identity);
            ignorepHLimit = visualInfo.ignorepHLimit;
            scaleMultiplier = visualInfo.visualScaleMultiplier;
            finalFadeMultiplier = visualInfo.finalFadeMultiplier;

            ps = scanVisualObject.GetComponent<ParticleSystem>();
            finalWaitPeriod = ps.main.startLifetime.constant;
        }

        GridCell gridCell;
        Vector3 startPosition;
        Vector3 previousScanPosition = Vector3.zero;
        Vector3 currentScanPosition;

        int verticalReach = 25;
        int horizontalReach = 20;

        int verticalNodeSearchCount;
        int horizontalNodeSearchCount;

        int direction;

        float speed = 10.0f;

        GameObject scanVisualObject;
        bool ignorepHLimit;
        bool initialAnimationFinished = false;
        bool finalAnimationStarted = false;
        float scaleMultiplier;
        ParticleSystem ps;
        float finalFadeMultiplier;
        float finalWaitPeriod;

        public void Update(ref PHScanMechanic scan, ref Player player)
        {
            if (finalAnimationStarted)
            {
                //If the final animation is playing, nothing else must be played.
                //If the control reaches this block, just return so that the rest of the code doesn't execute
                FinalAnimation(ref scan);
                return;
            }

            if (!ContinuepHScan(ref scan, ref player))
            {
                finalAnimationStarted = true;
            }

            if (!initialAnimationFinished)
            {
                InitialAnimation();
            }


            if (initialAnimationFinished)
            {
                currentScanPosition.x += speed * direction * GameManager.Instance.DeltaTime;
                //Position of the scan visual object must be set
                scanVisualObject.transform.position = currentScanPosition;

                //Debug.Log("current scan position: " + currentScanPosition);
                //Debug.DrawLine(Vector3.zero, currentScanPosition);
                Debug.DrawLine(new Vector3(currentScanPosition.x, currentScanPosition.y - verticalReach, currentScanPosition.z),
                                new Vector3(currentScanPosition.x, currentScanPosition.y + verticalReach, currentScanPosition.z),
                                Color.red);

                //Search all the nodes starting from the top range of the player's node
                

                GridCell previousCell = null;
                //Iterate the grid cells from bottom to top in the column searching for the items and enemies 
                for (int i = -verticalReach; i < verticalReach; i++)
                {
                    GridCell cellToCheck = null;
                    List<GridCell> gridCells = WorldGrid.Instance.gridArray[gridCell.index.x, gridCell.index.y + i];
                    List<Character> enemiesList = new List<Character>();

                    //if the grid cells exist, get the correct grid cell to check for enemies and items
                    Vector3 positionToCheck = new Vector3(currentScanPosition.x, currentScanPosition.y + i, currentScanPosition.z);
                    cellToCheck = WorldGrid.GetTheWorldGridCellWithoutCreatingNewCells(WorldGrid.GetGridIndex(positionToCheck));
                    if (cellToCheck == previousCell)
                        continue;
                    else
                        previousCell = cellToCheck;

                    if (cellToCheck == null)
                    {
                        Debug.LogError("Cell to check is null...Breaking out of the function");
                        continue;
                    }

                    Debug.Log("Cell to check position: " + cellToCheck.worldPosition);
                    enemiesList = cellToCheck.character;
                    Debug.Log("Drawing line");
                    Debug.DrawLine(Vector3.zero, cellToCheck.worldPosition, Color.red);


                    if (enemiesList != null)
                    {
                        for (int j = 0; j < enemiesList.Count; j++)
                        {
                            if (direction > 0)
                            {
                                if (enemiesList[j].transform.position.x <= currentScanPosition.x)
                                {
                                    Debug.Log("marked the enemy....Revealing pH");
                                    if (enemiesList[j].GetComponent<CharacterMechanics>())
                                    {
                                        RevealpH(enemiesList[j].GetComponent<CharacterMechanics>(), ref player);
                                    }
                                }
                            }
                            else
                            {
                                if (enemiesList[j].transform.position.x >= currentScanPosition.x)
                                {
                                    Debug.Log("marked the enemy....revealing pH");
                                    if (enemiesList[j].GetComponent<CharacterMechanics>())
                                    {
                                        RevealpH(enemiesList[j].GetComponent<CharacterMechanics>(), ref player);
                                    }
                                }
                            }
                        }
                    }
                }
            }


        }

        void InitialAnimation()
        {
            //Scale animation for y Axis
            float targetScale = verticalReach * 2 * scaleMultiplier;
            float speed = 5;
            Vector3 target = new Vector3(1, targetScale, 1);

            if(targetScale - scanVisualObject.transform.localScale.y < 0.2f)
            {
                initialAnimationFinished = true;
                scanVisualObject.transform.localScale = new Vector3(1, targetScale, 1);
                return;
            }

            scanVisualObject.transform.localScale = Vector3.Lerp(scanVisualObject.transform.localScale, target, speed * Time.deltaTime);

            //Animate the size of edge shape of the particle
            //ParticleSystem.ShapeModule shape = ps.shape;
            //shape.radius = scanVisualObject.transform.localScale.y;
        }

        float finalStepTimeElapsed = 0;
        void FinalAnimation(ref PHScanMechanic scan)
        {
            //Target scale is now 0
            float targetScale = 0;
            float speed = 5;
            Vector3 target = new Vector3(1, targetScale, 1);

            if (scanVisualObject.transform.localScale.y - targetScale < 0.2f)
            {
                if(finalStepTimeElapsed == 0)
                {
                    ParticleSystem.MainModule module = ps.main;
                    module.simulationSpeed = finalFadeMultiplier;
                    ParticleSystem.EmissionModule em = ps.emission;
                    em.enabled = false;
                    finalWaitPeriod /= module.simulationSpeed;
                }

                //Wait for a while for particles to disappear
                finalStepTimeElapsed += GameManager.Instance.DeltaTime;

                if(finalStepTimeElapsed > finalWaitPeriod)
                {
                    //Destroy the pH scanner visual object
                    Destroy(scanVisualObject);
                    scan = null;
                    return;
                }
            }

            scanVisualObject.transform.localScale = Vector3.Lerp(scanVisualObject.transform.localScale, target, speed * Time.deltaTime);

            //Animate the size of edge shape of the particle
            //ParticleSystem.ShapeModule shape = ps.shape;
            //shape.radius = scanVisualObject.transform.localScale.y;
        }

        bool ContinuepHScan(ref PHScanMechanic scan, ref Player player)
        {
            //Scan Stop Conditions
            if (direction > 0)
            {
                if (currentScanPosition.x > startPosition.x + horizontalReach)
                {
                    finalAnimationStarted = true;
                    //scan = null;
                    //return false;
                }
            }
            else
            {
                if (currentScanPosition.x < startPosition.x - horizontalReach)
                {
                    finalAnimationStarted = true;
                    //scan = null;
                    //return false;
                }
            }

            if(!ignorepHLimit)
                if (player.GetPlayerStatus().pHIndicator == null || player.GetPlayerStatus().pHUseCounter <= 0)
                {
                    Debug.Log("use counter: " + player.GetPlayerStatus().pHUseCounter + "  indicator: " + player.GetPlayerStatus().pHIndicator);
                    finalAnimationStarted = true;
                    //scan = null;
                    //return false;
                }

            return true;
        }

        void RevealpH(CharacterMechanics mech, ref Player player)
        {
            if (!mech.IspHrevealed())
            {
                //Decrese the pH count of the player
                mech.RevealpH(true);
                player.DecrementpHUse();
            }
        }
        
    }
    #endregion

}


