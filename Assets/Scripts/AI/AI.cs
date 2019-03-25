using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AI : MonoBehaviour {

    Node currentNode;

    public float visibilityDistance = 6.0f;

    [Range(0.1f, 4.0f)]
    public float nodeReachTolerance = 0.25f;

    public bool persistentChase = false;    //Enable this if the enemies must chase the character at all times

    public bool ignoreOtherEnemies = true;

    [SerializeField]
    public List<Node> targetNodePath;

    public Vector3 targetLocation;

    public Node targetNode;

    //Access points for setting the run / chase behavior
    public bool runaway = false;
    public bool chaseStarted = false;
    public Character chasingCharacter = null;

    //MovementScript2D movementScript;
    Enemy enemy;
    //TODO: remove this later
    Player player;  

    public float horizontalMovement;
    public bool jump;
    public bool jumpRelease;
    public bool promptPickup;
    
    public bool characterCanClimb = false;
    
    public float speedMultiplier = 0.5f;   //Set this in the inspector

    //public int processingDepth = 1;

    Node destinationNode;
    
    int directionFacing = 1;
    float maxJumpHeight = 5.0f;

    Vector3 directionToTarget;
    Vector3 directionToTargetNormalized;

    Vector3 directionToDestination;
    Vector3 directionToDestinationNormalized;

    bool haltMovement = false;

    private void Awake()
    {
        targetNodePath = new List<Node>();
        nodesPassed = new List<Node>();
        
    }

    private void Start()
    {

        enemy = GetComponent<Enemy>();
        player = FindObjectOfType<Player>();
        
        //movementScript = GetComponent<MovementScript2D>();
        //Initial target position is your own position
        targetLocation = gameObject.transform.position;

        float dist = Mathf.Infinity;
        for (int i = 0; i < PathGenerator.nodes.Count; i++)
        {
            float d = Vector3.Distance(gameObject.transform.position, PathGenerator.nodes[i].position);
            if (d < dist)
            {
                currentNode = PathGenerator.nodes[i];
                dist = d;
                Debug.Log("Closest node set");

            }
        }

        RaycastAndFindFloor(false);
        targetNode = currentNode;
        previousDirection = gameObject.transform.right;
    }

    private void Update()
    {
        if (enemy!=null)
        {
            distanceToTarget = Mathf.Infinity;

            if(enemy.playerSprite != null)
                directionFacing = (int)enemy.playerSprite.transform.localScale.x;

            //Debug.Log("node path count: " + targetNodePath.Count);
            //Set the target position

            //TODO: Check this code again
            //UpdateCurrentNode();
            //Debug.Log("Halt movement: " + haltMovement);

            //TODO: Remove comments later while still enabling testing
            //if (Input.GetMouseButtonDown(0))// && !CharacterManager.Instance.characterClicked)
            //{
            //    //TODO: change this
            //    targetNodePath.Clear();
            //    nodesPassed.Clear();
            //    targetLocation = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            //    //CalculateNodePath(currentNode, player.transform.position);
            //    CalculateNodePath(targetLocation);
            //}
            if (haltMovement)
                return;

            if(chasingCharacter != null && !haltMovement)
            {
                if (chaseStarted)
                    ChaseAI();
                if (runaway)
                    RunAwayAI();
            }
            
            if (!haltMovement)
            {
                //Here, we set the direction to the player's immediate target
                if (targetNode != null)
                {
                    directionToTarget = targetNode.position - gameObject.transform.position;
                    directionToTargetNormalized = directionToTarget.normalized;
                }

                //If the target node is reached, it is cleared in this
                CheckIfCharacterCanClimbLadder();

                CalculatePath();

                EvaluateTargetNode();
            }
            
            Move();
            //TODO: Consider adding this funciton later for better resolution
            //TrackMovementAndResolve();
            previousDirection = gameObject.transform.right;
        }
    }

    //float trackTime;
    //Vector3 trackPosition;
    public void ResolveEnemyCollision(List<Character> enemiesList)
    {
        for(int i = 0; i < enemiesList.Count; i++)
        {
            Enemy e = enemiesList[i].GetComponent<Enemy>();
            if (e != null)
            {
                if(e.characterType == enemy.characterType || ignoreOtherEnemies)
                {
                    Debug.Log("Resoving stuff between enemies");
                    //if(Vector3.SqrMagnitude(enemiesList[i].transform.position - gameObject.transform.position) < 5f)
                    if(Vector3.SqrMagnitude(enemiesList[i].transform.position - gameObject.transform.position) < 5f)
                        if (Vector3.Dot( (targetNode.position - gameObject.transform.position),
                                        (e.transform.position - gameObject.transform.position)) > 0)
                        {
                            if(-directionFacing == e.GetComponent<AI>().directionFacing)
                            {
                                if(directionFacing == 1)
                                {
                                    Node n = GetBestLeftNode(targetNode);
                                    if (targetNodePath.Count == 0 && n != null)
                                        targetNode = n;
                                    HaltMovement(0.5f, true, false);
                                }
                                else
                                {
                                    Node n = GetBestRightNode(targetNode);
                                    if (targetNodePath.Count == 0 && n != null)
                                        targetNode = n;
                                    HaltMovement(0.5f, true, false);
                                }
                            }
                        }
                }
            }
        }
    }

    Node GetTargetNode() { return targetNode; }

    void UpdateCurrentNode()
    {
        //TODO: Might need to add a condition to find the appropriate current node
        //Debug.Log("inside the Update current node: current node is: " + currentNode);
        Vector3 currentNodeDirection = currentNode.position - gameObject.transform.position;
        if (Vector3.Dot(gameObject.transform.right * directionFacing, currentNodeDirection) >= 0)
        {
            for (int i = 0; i < currentNode.rightConnections.Count; i++)
            {
                Vector3 directionToRightNode = currentNode.rightConnections[i].position - gameObject.transform.position;
                float dist = Vector3.Magnitude(directionToRightNode);

                if (dist < Vector3.Magnitude(currentNodeDirection) && Mathf.Abs(directionToRightNode.y) < 0.1f)
                {
                    currentNode = currentNode.rightConnections[i];
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < currentNode.leftConnections.Count; i++)
            {
                Vector3 directionToLeftNode = currentNode.leftConnections[i].position - gameObject.transform.position;
                float dist = Vector3.Magnitude(directionToLeftNode);

                if (dist < Vector3.Magnitude(currentNodeDirection) && Mathf.Abs(directionToLeftNode.y) < 0.1f)
                {
                    currentNode = currentNode.leftConnections[i];
                    break;
                }
            }
        }
    }
    
    //An access function into the AI pathfinding that tries to find path to the specified node.
    public void LayoutPathToNode(Node target)
    {
        CalculateNodePath(target);
    }

    Node previousDestinationNode = null;

    void CheckIfCharacterCanClimbLadder()
    {
        if (!enemy.State.Equals(typeof(ClimbingState)))
        {
            bool contactWithLadder = false;
            //Loop through the contact info of the enemy and if there's a contact with ladder, set the appropriate value 
            //to true
            for(int i = 0; i < enemy.info.Length; i++) {
                if (enemy.info[i].collider.tag == "tag_ladder")
                    contactWithLadder = true;
            }
            //if (Mathf.Abs(directionToTarget.x) < 0.8f && Mathf.Abs(directionToTarget.y) < 0.8f && targetNode.platform != null)
            if (contactWithLadder && targetNode.platform != null)
            {
                if (targetNode.platform.tag == "tag_ladder" && Mathf.Abs(directionToTarget.magnitude) < nodeReachTolerance * 5)
                {
                    Debug.Log("Character is able to climb.....AI is set");
                    //The following variable is passed to the Enemy class and this determines if the character should climb
                    characterCanClimb = true;
                    if (!haltMovement && !enemy.State.Equals(typeof(ClimbingState)) && !enemy.State.Equals(typeof(LandState)))
                    {
                        HaltMovement(0.5f, true, false);
                        //StartCoroutine(PauseMovement());
                        Debug.Log("Routine started");
                    }
                    return;
                }
            }
            else
            {
                //Debug.Log("Character can't climb......wow");
                characterCanClimb = false;
            }
        }
    }
    
    void CalculateNodePath(Vector3 targetLocation)
    {
        targetNodePath.Clear();
        nodesPassed.Clear();
        //TODO: Check this
        Node tempTarget = targetNode;
        targetNode = DecideDestinationNodeBasedOnPosition(targetLocation);
        Debug.Log("Calculating node path");
        //If there's no target found, raycast to find the floor and return
        if (targetNode == null)
        {
            if (targetNodePath.Count > 0)
                RaycastAndFindFloor(false);
            else
                targetNode = tempTarget;
            Debug.Log("No target node found");
            return;
        }
        
        Debug.Log("Target node is: " + targetNode);

        //Defining the target distance and direction here because I have obtained the target node now
        if (targetNode != null)
        {
            directionToDestination = targetNode.position - gameObject.transform.position;
            directionToDestinationNormalized = directionToDestination.normalized;
        }

        //Traverse the tree to find the target node
        TraverseNodeTree(currentNode);

        //Target node is now set to the character's immediate target position to reach
        if(targetNodePath.Count >= 1)
        {
            targetNode = targetNodePath[targetNodePath.Count - 1];

            previousDestinationNode = destinationNode;
            //destinationNode is the final destination of the character
            destinationNode = targetNodePath[0];
            Debug.Log("nodes: " + targetNodePath.Count);
        }
        
    }

    #region Search Functions
    Node DecideDestinationNodeBasedOnPosition(Vector3 targetPosition)
    {
        float distToTarget = Mathf.Infinity;
        int searchRadius = 1;
        //first, we need the grid on which the position if found
        GridIndex index = WorldGrid.GetGridIndex(targetPosition);

        //Check if the current cell has any nodes
        Node closestNode = null;

        //Searching for the nodes is only done in the downward direction for now
        for(int i = 0; i <= searchRadius; i++)
        {
            for(int j = 0; j <= i; j++)
            {
                List<GridCell> cell = new List<GridCell>();
                if (j == i)
                {
                    for(int k = 0; k <= j; k++)
                    {
                        if (k == 0)
                        {
                            if (WorldGrid.Instance.gridArray[index.x, index.y - j] != null)
                            {
                                cell = WorldGrid.Instance.gridArray[index.x, index.y - j];  //Left Cell Search
                                CheckIfAnyOfNodesInCurrentCellIsClosest(cell, index, distToTarget, out distanceToTarget, targetPosition, closestNode, out closestNode);
                            }
                        }
                        else
                        {
                            if (WorldGrid.Instance.gridArray[index.x - k, index.y - j] != null)
                            {
                                cell = WorldGrid.Instance.gridArray[index.x - k, index.y - j];  //Left Cell Search
                                CheckIfAnyOfNodesInCurrentCellIsClosest(cell, index, distToTarget, out distanceToTarget, targetPosition, closestNode, out closestNode);
                            }

                            if (WorldGrid.Instance.gridArray[index.x + k, index.y - j] != null)
                            {
                                cell = WorldGrid.Instance.gridArray[index.x + k, index.y - j];  //Right cell search
                                CheckIfAnyOfNodesInCurrentCellIsClosest(cell, index, distToTarget, out distanceToTarget, targetPosition, closestNode, out closestNode);
                            }
                        }
                        if (closestNode != null)
                            break;
                    }
                }
                else
                {
                    //Downward search
                    if(WorldGrid.Instance.gridArray[index.x - i, index.y - j] != null)
                    {
                        cell = WorldGrid.Instance.gridArray[index.x - i, index.y - j];  //Left Cell Search
                        CheckIfAnyOfNodesInCurrentCellIsClosest(cell, index, distToTarget, out distanceToTarget, targetPosition, closestNode, out closestNode);
                    }

                    if (WorldGrid.Instance.gridArray[index.x + i, index.y - j] != null)
                    {
                        cell = WorldGrid.Instance.gridArray[index.x + i, index.y - j];  //Right cell search
                        CheckIfAnyOfNodesInCurrentCellIsClosest(cell, index, distToTarget, out distanceToTarget, targetPosition, closestNode, out closestNode);
                    }
                }

                if (closestNode != null)
                    break;
            }
            //Check for the closest node
        }
        return closestNode;

    }
    
    void CheckIfAnyOfNodesInCurrentCellIsClosest(List<GridCell> cell, GridIndex index, float distance, out float closestDistance, Vector3 targetPosition, Node currentClosest, out Node closestNode)
    {
        closestNode = currentClosest;
        closestDistance = distance;
        GridCell c = null;
        
        //Each position in the array index can house multiple cells. This gets the exact cell
        for(int i = 0; i < cell.Count; i++)
        {
            if (cell[i].index.x == index.x && cell[i].index.y == index.y)
                c = cell[i];
        }

        if (c != null)
        {
            Debug.DrawLine(Vector3.zero, new Vector3(c.index.x, c.index.y), Color.blue);
            for(int i = 0; i < c.node.Count; i++)
            {
                float d = Vector3.SqrMagnitude(c.node[i].position - targetPosition);
                if ( d < distance)
                {
                    closestNode = c.node[i];
                    distance = d;
                }
            }
        }
    }
    #endregion

    #region Node Traversal Functions
    Node TraverseNodeTree(Node node)
    {
        if (node == targetNode)
        {
            targetNodePath.Add(node);
            Debug.Log("Target found");
            return node;
        }

        //Debug.Log("Traversing the tree");
        nodesPassed.Add(node);
        List<Node> allConnectedNodes = new List<Node>();

        //Left and right connections of the given node are added to a list so that they can be checked at the same time
        allConnectedNodes.InsertRange(0, node.leftConnections);
        allConnectedNodes.InsertRange(allConnectedNodes.Count, node.rightConnections);
        //allConnectedNodes.AddRange(node.leftConnections);
        //allConnectedNodes.AddRange(node.rightConnections);

        SortConnections(node, ref allConnectedNodes, targetNode);
        
        
        for (int i = 0; i < allConnectedNodes.Count; i++)
        {
            if (!nodesPassed.Contains(allConnectedNodes[i]))
            {
                //When the destination node is found, the path is retraced back to the player's current node
                Node n = TraverseNodeTree(allConnectedNodes[i]);
                if (n != null)
                {
                    targetNodePath.Add(node);
                    return node;
                }
            }
        }

        return null;
    }

    void SortConnections(Node origin, ref List<Node> connectedNodes, Node target)
    {
        //Main intent of this function is to sort. NOT to get the most optimal path directions.
        //Because, most optimal always need not have a path to the target node
        //Vector3 directionToTarget = (target.position - gameObject.transform.position).normalized;
        Vector3 originToTargetNormalized = (target.position - origin.position).normalized;
        for (int i = 0; i < connectedNodes.Count - 1; i++)
        {
            Vector3 dir1 = (connectedNodes[i].position - origin.position).normalized;
            for (int j = i + 1; j < connectedNodes.Count; j++)
            {
                Vector3 dir2 = (connectedNodes[j].position - origin.position).normalized;

                //TODO: Try better sorting
                //if the dot product is greater, that means that they are aligned properly
                //if (Vector3.Dot(dir2, directionToDestinationNormalized) >= Vector3.Dot(dir1, directionToDestinationNormalized))
                if (Vector3.Dot(dir2, originToTargetNormalized) >= Vector3.Dot(dir1, originToTargetNormalized))
                {
                    //swap nodes
                    Node temp = connectedNodes[i];
                    connectedNodes[i] = connectedNodes[j];
                    connectedNodes[j] = temp;
                    dir1 = (connectedNodes[i].position - origin.position).normalized;
                }
            }
        }
    }
    #endregion


    Vector3 previousDirection;

    void CalculatePath()
    {
        if (targetNode != null)
        {
            //If the character is idly moving, this block makes the character wait for a while when the 
            //target node is reached
            if (targetNodePath.Count == 0)// && !chaseStarted)
            {
                //destinationNode = targetNode;
                //Vector3 directionToTargetNode = targetNode.position - gameObject.transform.position;
                float dotProduct = Vector3.Dot(directionToTargetNormalized, Vector3.right);
                if (Mathf.Abs(directionToTarget.x) < nodeReachTolerance)
                {
                    if (Mathf.Abs(directionToTarget.y) < nodeReachTolerance)
                    {
                        currentNode = targetNode;
                        StartCoroutine(PauseMovement(true));
                    }
                }
            }
            else
            {
                //Check the node next to the target node and optimize the movement path
                if (targetNodePath.Count > 1)// && !enemy.State.Equals(typeof(ClimbingState)))
                {
                    Vector3 directionToTargetNode = targetNode.position - gameObject.transform.position;
                    Vector3 directionToNextNode = targetNodePath[targetNodePath.Count - 2].position - gameObject.transform.position;

                    //Debug.DrawLine(gameObject.transform.position, targetNode.position, Color.green);
                    //Debug.DrawLine(gameObject.transform.position, targetNodePath[targetNodePath.Count - 2].position, Color.green);

                    if (Vector3.Dot(directionToTargetNormalized, directionToNextNode.normalized) < 0)
                    {
                        if (enemy.State.Equals(typeof(ClimbingState)))
                        {
                            if (targetNodePath[targetNodePath.Count - 2].platform.tag == "tag_ladder" &&
                            Mathf.Abs(directionToNextNode.y) > Random.Range(1.0f, 1.5f))
                            {
                                //if (targetNodePath[targetNodePath.Count - 2].platform.tag == "tag_ladder" &&
                                //Mathf.Abs(directionToNextNode.y) > Random.Range(1.5f, 2.0f))
                                //targetNodePath.RemoveAt(targetNodePath.Count - 1);
                                Debug.Log("Climb Removed 1");
                                //currentNode = targetNode;
                                //targetNode = targetNodePath[targetNodePath.Count - 1];
                                TargetNodeChanged();
                                return;
                            }
                        }

                        else if (Mathf.Abs(directionToNextNode.x) > Random.Range(1.5f, 2.0f))
                        {
                            //targetNodePath.RemoveAt(targetNodePath.Count - 1);
                            Debug.Log("Removed 1");
                            //currentNode = targetNode;
                            //targetNode = targetNodePath[targetNodePath.Count - 1];
                            TargetNodeChanged();
                            //directionToTarget = targetNode.position - gameObject.transform.position;
                        }
                    }
                }

                //This part of code updates the target node. Once the player reaches his immediate target node,
                //that node is removed from the list 
                //Debug.Log("Target node is: " + targetNode);

                //targetNode = targetNodePath[targetNodePath.Count - 1];
                
                //TODO: This might be a little problematic. Keep an eye on this. This reflects on how close character
                //must be to transfer the target to the next node
                //if (Vector3.Distance(gameObject.transform.position, targetNode.position) < 0.5f)
                if (Mathf.Abs(directionToTarget.x) < nodeReachTolerance && Mathf.Abs(directionToTarget.y) < nodeReachTolerance)
                {
                    Debug.Log("removed default");
                    //targetNodePath.RemoveAt(targetNodePath.Count - 1);
                    //currentNode = targetNode;
                    TargetNodeChanged();
                }


                //Global direction to target and normalized variables are modified here for further usage
                //target is also set to the appropriate node within the node path
                if(targetNodePath.Count > 0)
                {
                    targetNode = targetNodePath[targetNodePath.Count - 1];
                    directionToTarget = targetNode.position - gameObject.transform.position;
                    directionToTargetNormalized = directionToTarget.normalized;
                }
            }
        }
    }
    
    void EvaluateTargetNode()
    {
        //This function assigns temporary target nodes
        float dotProduct = Vector3.Dot(previousDirection, gameObject.transform.right * directionFacing);
        
        if(targetNode == null)
        {
            Debug.Log("Target node is null....So Trying to get to the default node");
            RaycastAndFindFloor(false);
            return;
        }
        
        //The logic for the falling state. If character is in falling state, it returns from the function after
        //executing the block
        if (targetNode.platform != null)
            if (targetNode.platform.tag == "tag_ladder")
            {
                if (enemy.State.Equals(typeof(ClimbingState)))
                {
                    return;
                }
                else
                {
                    if (targetNodePath.Count == 0)
                    {
                        RaycastAndFindFloor(false);
                    }
                }
            }

        if (targetNodePath.Count == 0)
        {
            //TODO: THis block might not even be necessary. Consider deleting it
            
            //Check if the object is too close to the target node
            //if (Mathf.Abs(Vector3.Dot(directionToTargetNormalized, gameObject.transform.right * directionFacing)) < 0.3f)
            //{
            //    //Check the x distance
            //    if (Mathf.Abs(directionToTarget.x) < 0.2f)
            //    {
            //        //Pause movement and calculate the next node
            //        StartCoroutine(PauseMovement());
            //        Debug.Log("Raycasting.................................");
            //        //change the current node and target node to the node on the platform below
            //        RaycastAndFindFloor(false);
            //    }
            //}
            if (Mathf.Abs(directionToTarget.y) > 4.0f && !enemy.State.Equals(typeof(JumpingState)) && !enemy.State.Equals(typeof(FallingState))) //movementScript.playerState != PlayerStates.jumping && movementScript.playerState != PlayerStates.falling))
            {
                RaycastAndFindFloor(false);
            }
        }
        else
        {
            //TODO: Consider adding more conditions when raycasting might be needed
            if (Mathf.Abs(directionToTarget.y) > maxJumpHeight + 0.5f
                && !enemy.State.Equals(typeof(JumpingState)) && !enemy.State.Equals(typeof(FallingState)) && !enemy.State.Equals(typeof(ClimbingState)))
            {
                //parameter is set to true because we are still trying to find the optimal path to the destination
                RaycastAndFindFloor(true);
                Debug.Log("Finding floor");
            }


            //Conditions for the chase node elimination
            if (chaseStarted)
            {
                if (targetNode.platform != null)
                    if (targetNode.platform.tag == "tag_ladder" && enemy.State.Equals(typeof(ClimbingState)))
                    {
                        if(targetNodePath.Count > 1)
                        {
                            if (targetNodePath[targetNodePath.Count - 2].platform.tag == "tag_ladder")
                                TargetNodeChanged();
                        }
                        return;
                    }
                if (targetNodePath.Count >= 2)
                {
                    if (directionToTarget.y > maxJumpHeight + 1.5f)
                    {
                        //targetNodePath.RemoveAt(targetNodePath.Count - 1);
                        Debug.Log("Removed 2");
                        TargetNodeChanged();
                        //directionToTarget = targetNode.position - gameObject.transform.position;
                        //targetNodePath.Clear();
                    }
                }
                if (targetNodePath.Count >= 2)
                {
                    float sqDistanceBetweenNodes = Vector3.SqrMagnitude(targetNodePath[targetNodePath.Count - 1].position -
                                                                        targetNodePath[targetNodePath.Count - 2].position);
                    if (sqDistanceBetweenNodes < 1.0f)
                    {
                        //targetNodePath.RemoveAt(targetNodePath.Count - 1);
                        TargetNodeChanged();
                        Debug.Log("Removed 3");
                    }
                }
                if (targetNodePath.Count >= 2)
                {
                    
                    //int directionFacing = (int)enemy.playerSprite.transform.localScale.x;
                    if (directionToTarget.x < 0.5f && Vector3.Dot(directionToTarget.normalized, enemy.transform.right * directionFacing) < 0.3f)
                    {
                        //targetNodePath.RemoveAt(targetNodePath.Count - 1);
                        Debug.Log("Removed 4");
                        TargetNodeChanged();
                        //directionToTarget = targetNode.position - gameObject.transform.position;
                        //targetNodePath.Clear();
                    }
                }
                if (targetNodePath.Count > 1)
                    targetNode = targetNodePath[targetNodePath.Count - 1];
                //return;
            }

            if (targetNode.platform != null)
                if (targetNode.platform.tag == "tag_ladder")
                    return;

            

            //Check if the object is too close to the target node
            //TODO: consider adding a random angle range
            if (Mathf.Abs(Vector3.Dot(directionToTarget.normalized, gameObject.transform.right * directionFacing)) < 0.15f)
            {
                //Check the x distance
                Vector3 directionToNextNode = Vector3.zero;

                //Try to assign new calculations only when the player is on ground
                //This block of code deals with the case when the target node is directly above the character's head
                //Character pauses for a while and tries to assign itself best right or left node based on target
                if (targetNode.platform != null)
                    if (targetNode.platform.tag == "tag_ladder" || currentNode.platform.tag == "tag_ladder")
                        return;

                //TODO: Check this block of code
                if (Mathf.Abs(directionToTarget.x) < nodeReachTolerance && (directionToTarget.y > 0.2f || directionToTarget.y < 0.5f) &&
                    !enemy.State.Equals(typeof(JumpingState)) && !enemy.State.Equals(typeof(FallingState)))
                    //(movementScript.playerState != PlayerStates.jumping && movementScript.playerState != PlayerStates.falling))
                {

                    StartCoroutine(PauseMovement());
                    
                    //Get the best possible right or left connection and set it to target node so that
                    //the target node will be checked again in the next frame
                    Node bestLeftNode = null; float leftYDist = Mathf.Infinity;
                    Node bestRightNode = null; float rightYDist = Mathf.Infinity;
                    bestLeftNode = GetBestLeftNode(targetNode);
                    bestRightNode = GetBestRightNode(targetNode);
                    //TODO: Changed this portion. If the code doesn;t work as expected, consider changing this
                    //for (int i = 0; i < targetNode.leftConnections.Count; i++)
                    //{
                    //    Vector3 dir = targetNode.leftConnections[i].position - gameObject.transform.position;
                    //    if (Mathf.Abs(dir.y) < leftYDist)
                    //    {
                    //        //Debug.LogError("Added node");
                    //        bestLeftNode = targetNode.leftConnections[i];
                    //        leftYDist = dir.y;
                    //    }
                    //}
                    //for (int i = 0; i < targetNode.rightConnections.Count; i++)
                    //{
                    //    Vector3 dir = targetNode.rightConnections[i].position - gameObject.transform.position;
                    //    if (Mathf.Abs(dir.y) < rightYDist)
                    //    {
                    //        //Debug.LogError("Added node");
                    //        bestRightNode = targetNode.rightConnections[i];
                    //        rightYDist = dir.y;
                    //    }
                    //}
                    if (leftYDist <= rightYDist && bestLeftNode != null)
                    {
                        Debug.LogError("Added node");
                        targetNodePath.Add(bestLeftNode);
                        targetNode = targetNodePath[targetNodePath.Count - 1];
                    }
                    else if (bestRightNode != null)
                    {
                        Debug.LogError("Added node");
                        targetNodePath.Add(bestRightNode);
                        targetNode = targetNodePath[targetNodePath.Count - 1];
                    }

                }
            }
        }
    }

    Node GetBestLeftNode(Node node)
    {
        Node bestLeftNode = null; float leftYDist = Mathf.Infinity;
        for (int i = 0; i < targetNode.leftConnections.Count; i++)
        {
            Vector3 dir = targetNode.leftConnections[i].position - gameObject.transform.position;
            if (Mathf.Abs(dir.y) < leftYDist)
            {
                //Debug.LogError("Added node");
                bestLeftNode = targetNode.leftConnections[i];
                leftYDist = dir.y;
            }
        }
        return bestLeftNode;
    }

    Node GetBestRightNode(Node node)
    {
        Node bestRightNode = null; float rightYDist = Mathf.Infinity;
        for (int i = 0; i < targetNode.rightConnections.Count; i++)
        {
            Vector3 dir = targetNode.rightConnections[i].position - gameObject.transform.position;
            if (Mathf.Abs(dir.y) < rightYDist)
            {
                //Debug.LogError("Added node");
                bestRightNode = targetNode.rightConnections[i];
                rightYDist = dir.y;
            }
        }
        return bestRightNode;
    }

    void TargetNodeChanged()
    {
        Debug.Log("target node cahnged");
        currentNode = targetNode;
        if (targetNodePath.Count != 0)
        {
            targetNodePath.RemoveAt(targetNodePath.Count - 1);
            targetNode = targetNodePath[targetNodePath.Count - 1];
        }
        if (targetNode != null)
        {
            directionToTarget = targetNode.position - gameObject.transform.position;
            directionToTargetNormalized = directionToTarget.normalized;
        }
    }

    void RaycastAndFindFloor(bool reCalculateNodePath)
    {
        
        RaycastHit2D[] hit = Physics2D.RaycastAll(gameObject.transform.position, Vector3.down, 20, LayerMask.GetMask("Platform"));
        Node backupPlatformNode = null;
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.GetComponent<Platform>())
            {
                Debug.Log("Platform found...");
                Platform p = hit[i].collider.GetComponent<Platform>();
                float dist1 = Vector3.Distance(gameObject.transform.position, p.leftNode.position);
                float dist2 = Vector3.Distance(gameObject.transform.position, p.rightNode.position);
                if (dist1 < dist2)
                    targetNode = p.leftNode;
                else
                    targetNode = p.rightNode;

                currentNode = targetNode;
                destinationNode = targetNode;
                //backupPlatformNode = targetNode;
                break;
            }
            else
            {
                Debug.Log("no platform found underneath");
            }
        }
        //TODO: Optimize the node search procedure
        //Make a grid that houses the nodes and localize the search to the area player is in
        if (reCalculateNodePath)
        {
            Debug.Log("Recalculating nodes");
            targetNodePath.Clear();
            nodesPassed.Clear();
            
            CalculateNodePath (targetLocation);
            currentNode = targetNode;
            //if (targetNode == null)
            //{
            //    targetNode = backupPlatformNode;
            //    currentNode = targetNode;
            //}
            //else
            //{
            //    currentNode = targetNode;
            //}
            //currentNode = targetNode;
        }
    }

    void Move()
    {
        //Debug.Log("target node: " + targetNode.position);
        Debug.DrawLine(new Vector3(10, 10, 0), targetNode.position, Color.green);
        
        //this is for the jump speed multiplier
        if (!enemy.State.Equals(typeof(JumpingState)) && !enemy.State.Equals(typeof(FallingState)))
            horizontalMovement = 0;
        else
            horizontalMovement *= Random.Range(0.98f, 0.9999f);

        //Debug.Log("Waiting: " + waiting);
        //jump = false;
        if (!haltMovement)
        {
            Vector3 directionVector = directionToTarget;
            //Debug.Log("State: " + enemy.State);
            float dotProduct = Vector3.Dot(directionToTargetNormalized, Vector3.right);
            //Debug.Log("target node position: " + targetNode.position);
            //change movement direction only when the character is on the ground

            //Conditions for jump when character is in climbing state
            if (enemy.State.Equals(typeof(ClimbingState)))
            {
                //Get the ladder on which the character is currently climbing using the state
                ClimbingState cs = enemy.State as ClimbingState;
                //If the current node is not the platform, but the character is still trying to climb
                if (targetNode.platform != null)
                {
                    if (targetNode.platform.tag != "tag_ladder" || targetNode.platform != cs.ladder)
                    {
                        horizontalMovement = Mathf.Sign(targetNode.position.x - gameObject.transform.position.x);
                        EnableJump(EvaluateJumpTarget());
                        //If the target node is not the ladder and character is still in climbing state, force the state reset
                        characterCanClimb = false;
                        enemy.userInputs.climbPressed = false;
                    }
                }

                if (targetNodePath.Count > 1)
                {
                    if (targetNodePath[targetNodePath.Count-2].platform.tag == "tag_ladder")
                    {
                        if (targetNodePath[targetNodePath.Count - 2].position.y > enemy.transform.position.y)
                        {
                            horizontalMovement = 1;
                            Debug.Log("Climbing up");
                        }
                        else
                        {
                            horizontalMovement = -1;
                            Debug.Log("Climbing Down");
                        }
                        //return;
                    }
                }
                //Debug.Log("climbingggggggggggggggg");
                if (targetNode.position.y > enemy.transform.position.y)
                {
                    horizontalMovement = 1;
                    Debug.Log("Climbing up");
                }
                else
                {
                    horizontalMovement = -1;
                    Debug.Log("Climbing Down");
                }
                


                //Jump conditions
                if(targetNodePath.Count > 1)
                {
                    //TODO: This might cause some issues. Check it out later
                    if(targetNode.platform.tag != "tag_ladder")
                    {
                        Debug.Log("sfdfsfsdfsdfsdgdffgbdfbfbfgfgdsfgfsd" + -Mathf.Sign(targetNodePath[targetNodePath.Count - 1].position.x - gameObject.transform.position.x));
                        horizontalMovement = Mathf.Sign(targetNodePath[targetNodePath.Count - 1].position.x - gameObject.transform.position.x);
                        EnableJump(EvaluateJumpTarget());
                        return;
                    }

                    //The player mush be close to the target node
                    if(Mathf.Abs(directionToTarget.x) < 0.3f && Mathf.Abs(directionToTarget.y) < 0.3f)
                        if(targetNodePath[targetNodePath.Count-1].platform != targetNodePath[targetNodePath.Count - 2].platform)
                        {
                            //Debug.Log("sfdfsfsdfsdfsdgdffgbdfbfbfgfgdsfgfsd" + -Mathf.Sign(targetNodePath[targetNodePath.Count - 1].position.x - gameObject.transform.position.x));
                            //The inputs will be reversed once the player comes out of the climbing state
                            horizontalMovement = Mathf.Sign(targetNodePath[targetNodePath.Count - 1].position.x - gameObject.transform.position.x);    
                            EnableJump(EvaluateJumpTarget());
                        }
                }
                else if (targetNode.platform.tag!="tag_ladder" && Mathf.Abs(directionToTarget.y) > 1.0f)
                {
                    //Debug.Log("ASDfasdfadfsdfasfadfgfghfbxbfdbdgfb");
                    horizontalMovement = Mathf.Sign(targetNodePath[targetNodePath.Count - 1].position.x - gameObject.transform.position.x);
                    EnableJump(EvaluateJumpTarget());
                }

                return;
            }


            if (!enemy.State.Equals(typeof(JumpingState)) && !enemy.State.Equals(typeof(FallingState)))
            {
                if (dotProduct >= 0)
                {
                    horizontalMovement = 1;
                }
                else
                {
                    horizontalMovement = -1;
                }

                //Jump check conditions - enabling or disabling jump
                if (jumpRoutine == null)// && !enemy.State.Equals(typeof(ClimbingState)))
                {
                    if (Mathf.Abs(directionVector.y) > -0.02f && Mathf.Abs(directionVector.y) < maxJumpHeight)
                    {
                        bool skip = false;
                        //Check where the character is with respect to the platform
                        if (targetNode.platform != null)
                        {
                            if (targetNode.platform.tag == "tag_ladder")
                                return;

                            //This block checks if the character is below the platform. If that's the case,
                            //then if the character jumps, he hits the platform above. So, jump  must be skipped
                            if (targetNode == targetNode.platform.leftNode)
                            {
                                if (gameObject.transform.position.y < targetNode.position.y)
                                {
                                    if (gameObject.transform.position.x > targetNode.position.x)
                                        skip = true;
                                }
                            }
                            else if (targetNode == targetNode.platform.rightNode)
                            {
                                if (gameObject.transform.position.y < targetNode.position.y)
                                {
                                    if (gameObject.transform.position.x < targetNode.position.x)
                                        skip = true;
                                }
                            }

                            //If the target node's platform is same as the current node, then there's no need to jump
                            if (targetNode.platform == currentNode.platform)
                                skip = true;
                        }
                        else
                        {
                            //This block considers the case where the platform of target node is null
                            //Used Mostly for chasing
                            skip = true;
                            if (Mathf.Abs(directionVector.y) > 1.0f)
                            {
                                Debug.Log("Skipped");
                                skip = false;
                            }
                            //for (int i = 0; i < enemy.gridCell.node.Count; i++)
                            //{
                            //    Vector3 directionToNode = enemy.gridCell.node[i].position - gameObject.transform.position;
                            //    if (Mathf.Abs(directionToNode.x) < Random.Range(0.2f, 0.5f) && Mathf.Abs(directionToNode.y) < Random.Range(0.2f, 0.5f))
                            //    {
                            //        if (Mathf.Abs(directionVector.y) > 0.2f)
                            //        {
                            //            Debug.Log("Skipped");
                            //            skip = false;
                            //        }
                            //    }
                            //}
                        }

                        //This is the condition to skip the downward jump. If the target platform is below the player,
                        //and closer to him, he won't have to jump
                        if (directionVector.y < 0 && Mathf.Abs(directionVector.x) < Random.Range(2.4f, 2.9f))
                        {
                            Debug.Log("Skipped");
                            skip = true;
                        }

                        
                        if (Mathf.Abs(directionVector.x) < 12.0f && !skip)
                        {
                            EnableJump(EvaluateJumpTarget());
                        }
                    }
                    else
                    {
                        //This block of code checks the horizontal jump conditions. If the difference of y between two horizontal
                        //platforms is very less, then the platform with current node is compared and jumps if conditions meet
                        bool skip = false;
                        if(targetNode.platform != null)
                        {
                            if(targetNode.platform.tag == "tag_ladder"){
                                skip = true;
                            }
                        }

                        if (Mathf.Abs(directionVector.y) < 0.5f && !skip)
                        {
                            if (Mathf.Abs(directionVector.x) > 0.4f && currentNode.platform != targetNode.platform && targetNode.platform != null)
                            {
                                //Disable jump if the target node is a ladder
                                if (currentNode.platform != targetNode.platform)
                                {
                                    //if (Mathf.Abs(targetNode.position.y - currentNode.position.y) < 0.3f)
                                    EnableJump(EvaluateJumpTarget());
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    IEnumerator PauseMovement(bool calculateIdleNodes)
    {
        haltMovement = true;
        if (calculateIdleNodes)
        {
            if (targetNode.platform != null)
            {
                if (targetNode.platform.leftNode == targetNode)
                    targetNode = targetNode.platform.rightNode;
                else
                    targetNode = targetNode.platform.leftNode;
            }
        }
        
        yield return new WaitForSeconds(1);

        haltMovement = false;
    }

    public void HaltMovement(float duration, bool horizontal, bool vertical)
    {
        haltMovement = true;
        horizontalMovement = 0; // Reset the movement value to 0
        if(pauseRoutine != null)
        {
            StopCoroutine(pauseRoutine);
            pauseRoutine = null;
        }
        pauseRoutine = StartCoroutine(PauseMovement(duration));    //Initiate waiting for certain duration 
        enemy.BlockInputs(duration, horizontal, vertical);  //Finally, block the inputs on the base character class
    }

    public void HaltMovement()
    {
        haltMovement = true;

        StopAllCoroutines();

        horizontalMovement = 0;
    }

    public void ResumeMovement()
    {
        Debug.Log("AI Movement resumed");
        haltMovement = false;
        if (pauseRoutine != null)
            StopCoroutine(pauseRoutine);
    }

    Coroutine pauseRoutine;
    IEnumerator PauseMovement(float duration = 0.5f)
    {
        haltMovement = true;
        //Debug.Log("Movement paused");
        yield return new WaitForSeconds(duration);
        haltMovement = false;
        //haltMovement = false;
        pauseRoutine = null;
    }

    Coroutine jumpRoutine;

    //Change the max jump range here
    //This function sets the appropritate jump Hold time. This determines how long the character jumps
    float EvaluateJumpTarget()
    {
        float maxJumpHoldTime = 1.0f;
        float min = 0.05f;
        float max = maxJumpHeight + PathGenerator.Instance.xSpacing;
        float result = 0;
        Vector3 directionToTarget = targetNode.position - gameObject.transform.position;

        result = Mathf.Clamp((Mathf.Abs(directionToTarget.y) + Mathf.Abs(directionToTarget.x) - min) / (max - min), 0, 1);
        result = Mathf.Abs(maxJumpHoldTime * (result * result * (3 - 2 * result)));
        Debug.Log("jump result: " + result);
        return Random.Range(result - 0.2f, result + 0.2f);
    }


    void EnableJump(float duration)
    {
        Debug.Log("Trying to jump");
        jump = true;
        jumpRelease = false;
        jumpRoutine = StartCoroutine(JumpToFalse(duration));
    }

    IEnumerator JumpToFalse(float duration)
    {
        yield return new WaitForSeconds(duration);
        jumpRelease = true;
        jump = false;
        jumpRoutine = null;
        yield break;
    }


    float distanceToTarget = Mathf.Infinity;
    
    void CalculateNodePath(Node target)
    {
        if (targetNode == target)
            return;

        targetNodePath.Clear();

        targetNode = target;
        destinationNode = target;

        //Traverse the tree to find the target node
        //targetNode = currentNode;
        Debug.Log("target node count " + targetNodePath.Count);
        TraverseNodeTree(currentNode);

        if (targetNodePath.Count > 0)
        {
            Debug.Log("nodes.................: " + targetNodePath.Count);
            targetNode = targetNodePath[targetNodePath.Count - 1];
            previousDestinationNode = destinationNode;
            //destinationNode is the final destination of the character
            destinationNode = targetNodePath[0];
        }
        
    }

    List<Node> nodesPassed;
    
    bool waiting = false;

    bool StateEqualTo(States state, System.Type t)
    {
        return state.GetType() == t ? true : false;
    }

    /*void FindCharacters()
    {
        //Get the grid the character is currently in
        GridIndex index = WorldGrid.GetGridIndex(gameObject.transform.position);
        GridCell cell = WorldGrid.GetTheWorldGridCell(index);

        //check if the cell position is within the search radius
        //bool xSearch = true, ySearch = true;
        //add all characters to a list
        List<Character> charactersInProximity = new List<Character>();
        
        if(enemy.gridCell.character.Count > 0)
            charactersInProximity.AddRange(enemy.gridCell.character);

        //TODO: add more intuitive search logic
        
        //loop through characters and find the player
        for(int i = 0; i < charactersInProximity.Count; i++)
        {
            if (charactersInProximity[i].GetComponent<Player>())
            {
                //chaseStarted = true;
                runaway = true;
                chasingCharacter = charactersInProximity[i];
                targetNodePath.Clear();

                if (chaseStarted)
                {
                    targetNodePath.Add(new Node { position = chasingCharacter.transform.position });
                    targetNode = targetNodePath[0];
                }
                if (runaway)
                {
                    targetNodePath.Clear();
                    //RunAwayAI();
                }

                break;
            }
        }
        
    }*/

    //Chase AI is completely different than the simple path finding AI
    //Later, this can be added to the enemy behavior
    float t = 0;
    private Node lastKnownPlayerNode = null;
    bool chasingCharacterWithinProximity = false;
    void ChaseAI()
    {
        //Debug.Log("chase AI working ");
        if(chasingCharacter != null)
        {
            Vector3 directionToCharacter = chasingCharacter.transform.position - gameObject.transform.position;

            //TODO: Optimize the persistent chase code
            //This block is for persistently chasing the character
            if (persistentChase && directionToCharacter.magnitude > visibilityDistance)
            {
                //Debug.Log("Executing block");
                chasingCharacterWithinProximity = false;
                Player p = chasingCharacter.GetComponent<Player>();
                bool layoutPath = false;
                if (player != null) {
                    //if(enemy.State.GetType() != typeof(ClimbingState))
                    //{
                        //Try to reach the player's current node
                        if (lastKnownPlayerNode == null)
                        {
                            lastKnownPlayerNode = p.currentNodeOfPlayer;
                            layoutPath = true;
                        }
                        else
                        {
                            if(lastKnownPlayerNode != p.currentNodeOfPlayer)
                            {
                                layoutPath = true;
                            }
                        }
                    //}
                    lastKnownPlayerNode = p.currentNodeOfPlayer;
                }

                if (layoutPath)
                {
                    Debug.Log("Player path: " + lastKnownPlayerNode.position);
                    //TODO: Add a better search algorithm
                    targetLocation = lastKnownPlayerNode.position;
                    //RaycastAndFindFloor(true);
                    CalculateNodePath(targetLocation);
                }
                return;
            }

            if (enemy.State.Equals(typeof(ClimbingState)))
                return;

            
            if (persistentChase && !chasingCharacterWithinProximity)
            {
                Debug.Log("Character within proximity");
                chasingCharacterWithinProximity = true;
                targetNodePath.Clear();
                RaycastAndFindFloor(false);
            }

            
            if(targetNodePath.Count == 0)
            {
                targetNodePath.Add(new Node { position = chasingCharacter.transform.position });
                targetNode = targetNodePath[0];   
            }
            
            //New nodes are inserted at certain intervals
            t += GameManager.Instance.DeltaTime;
            //for(int i = 0; i < targetNodePath.Count; i++)
            //{
            if(t > 0.3f)// && Mathf.Abs(directionToCharacter.y) < maxJumpHeight + 1.5f)
            {
                targetNodePath.Insert(0, new Node { position = chasingCharacter.transform.position });
                destinationNode = targetNodePath[0];
                t = 0;
            }
            //}

            //if (Vector3.SqrMagnitude(directionToCharacter) > Random.Range(90, 100)
            //    && !enemy.State.Equals(typeof(JumpingState)) && !enemy.State.Equals(typeof(FallingState)))
            //{
            //    chaseStarted = false;
            //    targetNodePath.Clear();
            //
            //    RaycastAndFindFloor(false);
            //    Node tempTarget = targetNode;
            //    //TODO: add a more wider search algorithm
            //    //Try to go to the last known position of the character
            //    if (currentNode != null)
            //        CalculateNodePath(currentNode, chasingCharacter.transform.position);
            //
            //    //If there is no node within the search radius, raycast for the floor and get a suitable node
            //    if(targetNode == null)
            //    {
            //        targetNode = tempTarget;
            //        currentNode = targetNode;
            //    }
            //    
            //    chasingCharacter = null;
            //}

        }
        else
        {
            RaycastAndFindFloor(false);
        }
    }

    //Consider adding reset methods to an event
    public void PrepareForEncounter()
    {
        targetNodePath.Clear();
        if(!persistentChase)
            RaycastAndFindFloor(false);
    }

    public void ChaseReset()
    {
        if (chaseStarted)
        {
            Debug.Log("Chase reset");
            RaycastAndFindFloor(false);

            targetNodePath.Clear();
            if (currentNode != null && chasingCharacter != null)
            {
                Node tempTarget = targetNode;
                //TODO: add a more wider search algorithm
                //Try to go to the last known position of the character
                CalculateNodePath(chasingCharacter.transform.position);

                //If there is no node within the search radius, raycast for the floor and get a suitable node
                if (targetNode == null)
                {
                    targetNode = tempTarget;
                    currentNode = targetNode;
                }
            }
            chasingCharacter = null;
            chaseStarted = false;
        }
    }

    void RunAwayAI()
    {
        Vector3 directionToCharacter = chasingCharacter.transform.position - gameObject.transform.position;

        if (t == 0)
        {
            int searchDepth = 4;    //This variables determines how far the character should run away

            Node lastNode = null;
            Node activeNode = currentNode;
            float distanceToLastNode = -1;
            targetNodePath.Clear();

            if (targetNodePath.Count > 0)
            {
                lastNode = targetNodePath[targetNodePath.Count - 1];
                distanceToLastNode = Vector3.SqrMagnitude(lastNode.position - chasingCharacter.transform.position);
            }

            int directionToCheck = (int)-Mathf.Sign(directionToCharacter.x);
            if (Vector3.Dot(directionToCharacter, (currentNode.position - chasingCharacter.transform.position)) < 0)
            {
                targetNodePath.Add(currentNode);
            }

            if (directionToCheck < 0)
            {
                Debug.Log("checking left");
                //Get the randomized left collection
                int count = 0;

                while (count < searchDepth)
                {
                    List<Node> leftConnections = activeNode.leftConnections;
                    if (leftConnections.Count > 0)
                    {
                        targetNodePath.Add(leftConnections[Random.Range(0, leftConnections.Count)]);
                        activeNode = targetNodePath[targetNodePath.Count - 1];
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }
            else
            {
                Debug.Log("Checking right");
                //get the randomized right collection
                int count = 0;

                while (count < searchDepth)
                {
                    List<Node> rightConnections = activeNode.rightConnections;
                    if (rightConnections.Count > 0)
                    {
                        targetNodePath.Add(rightConnections[Random.Range(0, rightConnections.Count)]);
                        activeNode = targetNodePath[targetNodePath.Count - 1];
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }
            if (targetNodePath.Count > 0)
            {
                Debug.Log("reversed nodes");
                //The list must be reversed to stay consistent with how we are searching the path
                targetNodePath.Reverse();

                targetNode = targetNodePath[targetNodePath.Count - 1];
                currentNode = targetNode;
                destinationNode = targetNodePath[0];
            }
        }

        t += GameManager.Instance.DeltaTime;
        if (t > 2)
            t = 0;
    }
    
    public void RunAwayReset()
    {
        if (runaway)
        {
            Vector3 directionToCharacter = chasingCharacter.transform.position - gameObject.transform.position;
            RaycastAndFindFloor(false);
        
            targetNodePath.Clear();
            
            runaway = false;
            chasingCharacter = null;
            t = 0;
        }
        
    }

    public void ResetChaseAndRunAway()
    {
        bool resetFinished = false;
        if (runaway)
        {
            RunAwayReset();
            resetFinished = true;
        }
        if (chaseStarted)
        {
            ChaseReset();
            resetFinished = true;
        }
        if (!resetFinished)
            RaycastAndFindFloor(false);
    }

    public bool IsHalted() { return haltMovement; }

    private void OnDrawGizmos()
    {
        if (currentNode != null)
            Gizmos.DrawSphere(currentNode.position, 0.5f);

        Gizmos.color = Color.red;

        if (destinationNode != null)
            Gizmos.DrawSphere(destinationNode.position, 0.3f);

        Gizmos.color = Color.green;

        if (targetNode != null)
            Gizmos.DrawSphere(targetNode.position, 0.15f);

        if(Application.isPlaying)
            for (int i = 0; i < targetNodePath.Count; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(targetNodePath[i].position, 0.35f);
                Gizmos.DrawLine(Vector3.zero, targetNodePath[0].position);
            }

    }
    
}
