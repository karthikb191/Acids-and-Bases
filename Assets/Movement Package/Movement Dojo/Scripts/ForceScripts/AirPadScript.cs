using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPadScript : MonoBehaviour {

    public bool isActive;

    public float floatHeight;

    public float airForce;

    public float airPadHeight;
    public float airpadRadius = 1;

    public LayerMask objectsToFloat;

    //private fields for accessing properties of other objects
    //private MovementScript2D playerms;

    Ray ray;

    [Range(0,1)]
    private float multiplier = 0;   //the multiplier variable for adding smoothness
    private int sign = 1;
    private Vector2 boxPosition;
    private Vector2 boxDimensions;

    public bool alwaysActive;

    //Vector3 force;

    public bool isInContactWithPlayer;

    [Range(0, 1)]
    public float itemMultiplier = 1;

    private List<ExternalForce> objectsInContact;   //This has the list of all the objects that are in contact with the field
    private List<Vector3> force;    //The force experienced by each object in the list

    private FieldsManager fieldsManager;
    //private List<float> multiplier;
    //float multiplier = 0;

    //private FieldsManager fieldsManager;

    // Use this for initialization
    void Start () {
        //playerms = null;
        ray = new Ray(gameObject.transform.position, gameObject.transform.up);
        objectsInContact = new List<ExternalForce>();
        force = new List<Vector3>();

        fieldsManager = FindObjectOfType<FieldsManager>();

        if(!alwaysActive)
            fieldsManager.forceFields.Add(this.gameObject);
    }

    private void OnDrawGizmos() {
        //Gizmos.DrawWireCube(boxPosition, new Vector3(boxDimensions.x, boxDimensions.y, 0));
        float angle = Vector3.Angle(Vector3.right, gameObject.transform.up);
        Vector3 bottomLeft = new Vector3(gameObject.transform.position.x - airpadRadius, gameObject.transform.position.y, gameObject.transform.position.z);
        //bottomLeft = gameObject.transform.rotation * bottomLeft;
        bottomLeft = Quaternion.AngleAxis(angle, gameObject.transform.position + gameObject.transform.forward) * bottomLeft;
        bottomLeft = gameObject.transform.position + -gameObject.transform.right * airpadRadius;

        Vector3 bottomRight = new Vector3(gameObject.transform.position.x + airpadRadius, gameObject.transform.position.y, gameObject.transform.position.z);
        bottomRight = gameObject.transform.position + gameObject.transform.right * airpadRadius;

        Vector3 topLeft = new Vector3(gameObject.transform.position.x - airpadRadius, 
                                        gameObject.transform.position.y + airPadHeight, gameObject.transform.position.z);
        topLeft = gameObject.transform.position + (gameObject.transform.up * airPadHeight) - gameObject.transform.right * airpadRadius;      

        Vector3 topRight = new Vector3(gameObject.transform.position.x + airpadRadius,
                                        gameObject.transform.position.y + airPadHeight, gameObject.transform.position.z);
        topRight = gameObject.transform.position + (gameObject.transform.up * airPadHeight) + gameObject.transform.right * airpadRadius;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(bottomRight, topRight);
    }

    // Update is called once per frame
    void FixedUpdate() {

        //force = Vector3.zero;
        force.Clear();
        objectsInContact.Clear();   //Clear the list so that new objects can be added

        boxPosition = new Vector2(gameObject.transform.position.x,
                                    gameObject.transform.position.y + airPadHeight/2);

        boxDimensions = new Vector2(1, airPadHeight);
        //Cast a ray upwards
        //RaycastHit2D rayCastHit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, floatHeight, objectsToFloat);

        //Collider2D col = Physics2D.OverlapBox(boxPosition, boxDimensions, 0, objectsToFloat);
        RaycastHit2D[] c = Physics2D.CircleCastAll(gameObject.transform.position, airpadRadius, gameObject.transform.up, airPadHeight);


        
        int count = 0;
        for (int i = 0; i < c.Length; i++)
            if (c[i].collider != null)
                if ((c[i].collider.gameObject.GetComponent<ExternalForce>() != null))
                {
                    objectsInContact.Add(c[i].collider.GetComponent<ExternalForce>());
                    force.Add(Vector3.zero);    //initializing the force component to 0
                    
                }

        //If the ray hits the player or another object, then apply a force to the player
        float randomMax = Random.Range(1.0f, 1.2f);
        float randomMin = Random.Range(0.1f, 0.6f);
        multiplier = HelperFunctions.CounterAnimation(multiplier, randomMin, randomMax, Time.deltaTime, sign, out sign);

        for (int i = 0; i < objectsInContact.Count; i++)
        {

            if (Vector3.Dot(gameObject.transform.up, (objectsInContact[i].gameObject.transform.position - gameObject.transform.position).normalized) > 0)
            {
                count++;
                isInContactWithPlayer = true;

                //Debug.Log("Collision with: " + c[i].collider.gameObject);

                //multiplier += Time.deltaTime;
                //multiplier = Mathf.Clamp(multiplier, 0, 1);

                //Debug.Log("Air Pad Multiplier is: " + multiplier);

                Vector3 hitPosition = objectsInContact[i].gameObject.transform.position;     //The hit position of the ray
                float distanceFrac = Mathf.Clamp(Vector3.Distance(hitPosition, gameObject.transform.position) / floatHeight, 0, 1);
                //Debug.Log("Fraction of distance is: " + distanceFrac);

                force[i] = gameObject.transform.up * airForce * 100 * (1 - distanceFrac) * multiplier;

                //Debug.Log("Force is airfield is: " + force);

            }
            
        }
        if(objectsInContact.Count == 0)
        {
            multiplier = 0;
        }
	}
    private void Update()
    {
        for (int i = 0; i < objectsInContact.Count; i++)
        {
            itemMultiplier = 1;
            //If the object that is in contact with is the player, then proceed with the following item calculations
            //For ordinary objects, this is not needed
            if (objectsInContact[i].GetComponent<MovementScript2D>() != null)
            {
                MovementScript2D playerms = objectsInContact[i].GetComponent<MovementScript2D>();
                //Depending on the type of item the player currently has, the force must be cut off appropriately
                if (playerms != null)
                {
                    //Set the external vertical drag to 0.2
                    //Debug.Log("Setting the damp");
                    playerms.GetComponent<PlayerMechanics>().isInContactwithField = true;
                    //if (playerms.playerState == PlayerStates.falling)
                    //    playerms.GetComponent<AudioSource>().Stop(); //If within the field, stop all the sounds

                    //This part of code sets the damp to the default downward force
                    if (force[i]!= Vector3.zero)
                        playerms.externalVerticalMovementDamp = 0.2f;

                    if (playerms.GetComponent<PlayerMechanics>().itemPickedUp == null)
                    {
                        itemMultiplier = 0.3f;
                    }
                    else
                    {
                        switch (playerms.GetComponent<PlayerMechanics>().itemPickedUp.GetComponent<PickUpItem>().item)
                        {
                            case Item.light:
                                itemMultiplier = 0.28f;
                                break;
                            case Item.magnet:
                                itemMultiplier = 0.20f;
                                break;
                            case Item.parachute:
                                if(playerms.GetComponent<PlayerMechanics>().itemPickedUp.GetComponent<PickUpItem>().itemAnimator.GetBool("Open"))
                                    itemMultiplier = 1.0f;
                                else
                                    itemMultiplier = 0.15f;
                                break;
                            default:
                                itemMultiplier = 0.5f;
                                break;
                        }
                    }
                }
            }
            objectsInContact[i].force += force[i] * itemMultiplier;
        }
        //FieldsManager.fieldsForce += force * itemMultiplier;
        
    }
}
