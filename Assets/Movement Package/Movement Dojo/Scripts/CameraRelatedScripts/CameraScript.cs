using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour {
    private GameObject player;
    private Player playerms;

    //distance calculation to player
    private float distance;

    //camera
    private Camera mainCam;

    //camera boundaries
    [HideInInspector]
    public Vector3 center;
    [HideInInspector]
    public Vector3 leftBoundary;
    [HideInInspector]
    public Vector3 rightBoundary;
    [HideInInspector]
    public Vector3 topBoundary;
    [HideInInspector]
    public Vector3 bottomBoundary;

    //Boundary objects that must be set
    public GameObject boundariesContainer;

    //camera move properties
    
    [Range(0, 1)]
    public float speed;


    //Movement Multipliers
    
    [Range(-50, 50)]
    public float xPosMultiplier = 2;
    [Range(-50, 50)]
    public float yPosMultiplier = 2;

    Vector3 targetPosition;

    private Vector3 oldPosition;

    private bool otherObjectCommandingCamera;
    //public bool OtherObjectCommandingCamera {   get { return otherObjectCommandingCamera; }
    //                                            set { otherObjectCommandingCamera = value; } }
	// Use this for initialization
	void Start () {
        playerms = FindObjectOfType<Player>();
        mainCam = Camera.main;

        //playerms = player.GetComponent<MovementScript2D>();
        //playerms = CharacterManager.Instance.ThePlayer;
        player = playerms.gameObject;
        //speed = playerms.maxSpeed;
        

        targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, gameObject.transform.position.z);
        oldPosition = gameObject.transform.position;
    }
    
    
    // Update is called once per frame
    void Update () {

        if (player != null)
        {
            
            distance = Vector3.Distance(player.transform.position, new Vector3(targetPosition.x, targetPosition.y, player.transform.position.z));

            //targetPosition = new Vector3(player.transform.position.x + playerms.CurrentLinearSpeed * 5 * xPosMultiplier,
            //                                player.transform.position.y + playerms.CurrentJumpSpeed * 5 * yPosMultiplier,
            //                                gameObject.transform.position.z);

            //float distance = Vector3.Distance(player.transform.position, targetPosition);
            //Debug.Log("Distance between camera and player is: " + distance);

            if (distance > 0.5f && !otherObjectCommandingCamera)
            {
                FollowPlayer();
            }
            //gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, gameObject.transform.position.z);
            //targetPosition = Vector3.Lerp(gameObject.transform.position, targetPosition, 0.05f);

            Vector3 position = new Vector3(gameObject.transform.position.x * (1 - xMultiplier) + targetPosition.x * (xMultiplier),
                                                        gameObject.transform.position.y * (1 - yMultiplier) + targetPosition.y * (yMultiplier),
                                                        gameObject.transform.position.z);

            gameObject.transform.position = Vector3.Lerp(oldPosition, position, speed);
            //gameObject.transform.position = position;
            oldPosition = gameObject.transform.position;
            //oldPosition = mainCam.transform.position;
        }
    }
    Vector3 direction;
    void FollowPlayer()
    {
        //Debug.Log("velocity: " + playerms.velocity);
        Vector3 locationTarget = new Vector3(playerms.transform.position.x + playerms.velocity.x,
                                        playerms.transform.position.y + playerms.velocity.y,
                                        gameObject.transform.position.z);
        float deltaTime = GameManager.Instance.DeltaTime;
        targetPosition = new Vector3(playerms.transform.position.x + xPosMultiplier * playerms.velocity.x,
                                        playerms.transform.position.y + yPosMultiplier * playerms.velocity.y,
                                        gameObject.transform.position.z);

        
        //targetPosition = new Vector3(player.transform.position.x + playerms.CurrentLinearSpeed * 5 * xPosMultiplier, 
        //                                player.transform.position.y + playerms.CurrentJumpSpeed * 5 * yPosMultiplier, 
        //                                gameObject.transform.position.z);
        //
        //direction = targetPosition - gameObject.transform.position;


        //float camToTargetDistance = Vector3.Distance(gameObject.transform.position, targetPosition);
        //
        ////multiplier = Mathf.SmoothStep(0, allowedDist, targetDistDiff) / allowedDist;
        //multiplier = Mathf.Clamp01(camToTargetDistance/playerToTargetDistance);

        //Debug.Log("Camera Allowed dist: " + playerToTargetDistance);
        //Debug.Log("Camera target dist: " + camToTargetDistance);
        //Debug.Log("Camera multiplier: " + multiplier);

        //Only update position if it's within the max and min boundaries

        CameraIsWithinBounds(out xMultiplier, out yMultiplier);


    }
    int yMultiplier = 0; int xMultiplier = 0;
    
    //if the TARGET POSITION is within bounds, then move the camera
    void CameraIsWithinBounds(out int xMult, out int yMult)
    {
        //bool result = true;
        xMult = 1; yMult = 1;
        if (boundariesContainer != null)
        {
            if(boundariesContainer.transform.childCount == 2)
            {

                //if (targetPosition.x < boundariesContainer.transform.GetChild(0).transform.position.x + GameManager.Instance.camHalfWidth ||
                //    targetPosition.x > boundariesContainer.transform.GetChild(1).transform.position.x - GameManager.Instance.camHalfWidth ||
                //    targetPosition.y < boundariesContainer.transform.GetChild(0).transform.position.y + GameManager.Instance.camHalfHeight||
                //    targetPosition.y > boundariesContainer.transform.GetChild(1).transform.position.y - GameManager.Instance.camHalfHeight)
                if ((targetPosition.x - GameManager.Instance.camHalfWidth < boundariesContainer.transform.GetChild(0).transform.position.x &&
                    gameObject.transform.position.x - GameManager.Instance.camHalfWidth < boundariesContainer.transform.GetChild(0).transform.position.x) ||
                     (targetPosition.x + GameManager.Instance.camHalfWidth > boundariesContainer.transform.GetChild(1).transform.position.x &&
                     gameObject.transform.position.x + GameManager.Instance.camHalfWidth > boundariesContainer.transform.GetChild(1).transform.position.x))
                {
                    xMult = 0;
                }
                if((targetPosition.y - GameManager.Instance.camHalfHeight < boundariesContainer.transform.GetChild(0).transform.position.y &&
                  gameObject.transform.position.y - GameManager.Instance.camHalfHeight < boundariesContainer.transform.GetChild(0).transform.position.y) ||
                  (targetPosition.y + GameManager.Instance.camHalfHeight > boundariesContainer.transform.GetChild(1).transform.position.y &&
                   gameObject.transform.position.y + GameManager.Instance.camHalfHeight > boundariesContainer.transform.GetChild(1).transform.position.y))
                {
                    yMult = 0;
                }

            }
        }

        //return result;
    }
    
    //set by external scripts
    public void StopCameraMovement()
    {
        xMultiplier = yMultiplier = 0;
        otherObjectCommandingCamera = true;
    }
    //set by external scripts
    public void ResumeCameraMovement()
    {
        xMultiplier = yMultiplier = 1;
        otherObjectCommandingCamera = false;
    }

    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
        oldPosition = position;
    }
    
}
