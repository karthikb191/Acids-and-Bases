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

    //Camera shake variables
    public bool shakeEnabled = false;
    CameraShake shakeObject;
    public float shakeAmount = 0.2f;
    public float shakeDamping = 0.5f;
    public float shakeSmoothness = 0.5f;

    //Camera Focus Variables
    public bool focusEnabled = false;
    CameraFocus focusObject;
    public Vector3 focusPosition;
    public float focusSpeed;
    public float focusDuration;

    //public bool OtherObjectCommandingCamera {   get { return otherObjectCommandingCamera; }
    //                                            set { otherObjectCommandingCamera = value; } }
    // Use this for initialization
    void Start() {
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
    void Update() {
        if (player != null)
        {
            if (focusObject != null)
            {
                Vector3 target;
                target = focusObject.FocusUpdate(gameObject.transform.position, ref focusObject);
                if (shakeObject != null)
                    target += shakeObject.ShakeUpdate(ref shakeObject);

                gameObject.transform.position = target;
                return;
            }


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

            Vector3 positionToReach = Vector3.Lerp(oldPosition, position, speed);

            if (shakeObject != null)
                positionToReach += shakeObject.ShakeUpdate(ref shakeObject);

            gameObject.transform.position = positionToReach;

            oldPosition = gameObject.transform.position;
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
            if (boundariesContainer.transform.childCount == 2)
            {
                if ((targetPosition.x - GameManager.Instance.camHalfWidth < boundariesContainer.transform.GetChild(0).transform.position.x &&
                    gameObject.transform.position.x - GameManager.Instance.camHalfWidth < boundariesContainer.transform.GetChild(0).transform.position.x) ||
                     (targetPosition.x + GameManager.Instance.camHalfWidth > boundariesContainer.transform.GetChild(1).transform.position.x &&
                     gameObject.transform.position.x + GameManager.Instance.camHalfWidth > boundariesContainer.transform.GetChild(1).transform.position.x))
                {
                    xMult = 0;
                }
                if ((targetPosition.y - GameManager.Instance.camHalfHeight < boundariesContainer.transform.GetChild(0).transform.position.y &&
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

    public void ShakeCamera(float amount = 1, float damping = 1, float smootheness = 0.5f)
    {
        if (shakeObject == null && shakeEnabled)
            shakeObject = new CameraShake(amount, damping, smootheness);
    }

    public void FocusCameraAt(Vector3 position, float speed = 1, float waitDuration = 1)
    {
        if(focusObject == null && focusEnabled)
        {
            focusObject = new CameraFocus(position, mainCam, speed, waitDuration);
        }
    }


    private float tempOrthographicSize;
    static bool cameraMoving;
    static Coroutine c;

    public void ChangeCameraProjectionSize(float value, float speed)
    {
        tempOrthographicSize = Camera.main.orthographicSize;
        if (cameraMoving)
        {
            if (c != null)
            {
                StopCoroutine(c);
                c = null;
            }

        }
        c = StartCoroutine(ChangeCameraSize(value));
    }

    IEnumerator ChangeCameraSize(float orthographicSize, float speed = 1)
    {
        cameraMoving = true;
        while (Camera.main.orthographicSize > orthographicSize + 0.05f || Camera.main.orthographicSize < orthographicSize - 0.05f)
        {
            Camera.main.orthographicSize = tempOrthographicSize;
            yield return new WaitForFixedUpdate();

            if (Camera.main.orthographicSize > orthographicSize + 0.05f)
                tempOrthographicSize -= GameManager.Instance.DeltaTime * speed;

            else if (Camera.main.orthographicSize < orthographicSize - 0.05f)
                tempOrthographicSize += GameManager.Instance.DeltaTime * speed;
        }
        Camera.main.orthographicSize = orthographicSize;
        cameraMoving = false;
    }

    class CameraShake
    {
        float amount;
        float damping;
        float smoothness;

        float currentAmount;

        Vector3 previousOffset = Vector3.zero;
        public CameraShake(float amount, float damping, float smoothness)
        {
            this.amount = amount;
            this.damping = damping;
            this.smoothness = smoothness;

            currentAmount = amount;
        }

        //Returns a random offset vector3 value
        public Vector3 ShakeUpdate(ref CameraShake cameraShakeObject)
        {
            Vector3 offset = Vector3.zero;
            ///Random Number Generator
            /// Get the random number in the range 0 and 1 using Random.value
            /// To get it in the range (-currentAmount, currentAmount),
            /// use the formula below
            ///

            offset.x = Random.value * 2 * currentAmount - currentAmount;
            offset.y = Random.value * 2 * currentAmount - currentAmount;

            currentAmount -= damping * Time.deltaTime;

            offset = previousOffset * (1 - smoothness) + offset * (smoothness);
            previousOffset = offset;

            if (currentAmount <= 0)
            {
                cameraShakeObject = null;
            }
            return offset;
        }
    }

    class CameraFocus
    {
        Vector3 initialPosition;
        Vector3 focusPoint;
        float waitDuration;
        float speed;

        float waitTimeElapsed = 0.0f;

        Camera mainCam;
        bool reachedTarget = false;
        bool waiting = false;
        bool returnedToInitialPosition = false;

        public delegate void CameraFocusBegin();
        public event CameraFocusBegin CameraFocusBeginEvent;
        public delegate void CameraFocusFinished();
        public event CameraFocusBegin CameraFocusFinishedEvent;

        Vector3 previousDirection;
        Vector3 direction;

        public CameraFocus(Vector3 focusPoint, Camera mainCam, float speed, float waitDuration)
        {
            //Call the camera focus begin event and notify it to all the subscribers
            if(CameraFocusBeginEvent != null)
                CameraFocusBeginEvent();

            this.focusPoint = new Vector3(focusPoint.x, focusPoint.y, mainCam.transform.position.z);
            this.speed = speed;
            this.waitDuration = waitDuration;
            this.initialPosition = mainCam.transform.position;
            this.mainCam = mainCam;

            previousDirection = (focusPoint - initialPosition).normalized;
            direction = previousDirection;
        }
        
        public Vector3 FocusUpdate(Vector3 cameraPosition, ref CameraFocus focusObject)
        {
            //Debug.Log("focus position: " + focusPoint);
            previousDirection = direction;
            direction = focusPoint - cameraPosition;
            Vector3 result = Vector3.zero;
            result = cameraPosition + (direction * speed) * GameManager.Instance.DeltaTime;
            //Debug.Log("Speed: " + speed);
            //Debug.Log("Camera Position: " + cameraPosition);
            //Debug.Log("Direction: " + direction);
            if (!reachedTarget)
            {
                if(direction.magnitude < 0.5f || Vector3.Dot(direction.normalized, previousDirection) < -0.1f)
                {
                    Debug.Log("Reached the Target");
                    reachedTarget = true;
                    waiting = true;
                    focusPoint = initialPosition;
                    return cameraPosition;
                }
                return result;
            }
            
            //If the camera is waiting, return the position of the camera itself
            if (waiting)
            {
                if (waitTimeElapsed > waitDuration)
                {
                    waiting = false;
                    returnedToInitialPosition = false;
                    speed *= 2;
                    Debug.Log("Wait finished");
                }
                else
                {
                    waitTimeElapsed += GameManager.Instance.DeltaTime;
                    result = cameraPosition;
                    return cameraPosition;
                }
            }

            if (!returnedToInitialPosition)
            {
                if (direction.magnitude < 0.5f || Vector3.Dot(direction.normalized, previousDirection) < 0)
                {
                    Debug.Log("Reached initial position");

                    if (CameraFocusFinishedEvent != null)
                    {
                        CameraFocusFinishedEvent();
                    }

                    returnedToInitialPosition = true;
                    focusObject = null;
                    return cameraPosition;
                }
                return result;
            }

            focusObject = null;
            return result;
        }
    }

}
