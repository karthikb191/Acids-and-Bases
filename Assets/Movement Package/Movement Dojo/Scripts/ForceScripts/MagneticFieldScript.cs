using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticFieldScript : MonoBehaviour {

    public bool isActive;

    public float floatHeight;

    public float magneticForce;     //This variable is also used to set the jumpDamper of the playerscript

    public bool north;     //0 for south and 1 for north

    public float magnetPadHeight = 1;
    public float magnetPadRadius = 1;

    //private Vector3 force;
    [Range(1, 100)]
    public float magneticDamp = 1;

    public LayerMask objectsToFloat;

    public bool alwaysActive;

    Ray ray;

    [Range(0,1)]
    private float multiplier = 0;   //the multiplier variable for adding smoothness
    private int sign = 1;
    [Range(0, 1)]
    public float itemMultiplier = 1;

    private Vector2 boxPosition;
    private Vector2 boxDimensions;

    public bool isInContactWithPlayer;

    private FieldsManager fieldsManager;    // Only used to collect all the fields

    private List<ExternalForce> objectsInContact;   //This has the list of all the objects that are in contact with the field
    private List<Vector3> force;    //The force experienced by each object in the list

    // Use this for initialization
    void Start () {

        ray = new Ray(gameObject.transform.position, gameObject.transform.up);

        fieldsManager = FindObjectOfType<FieldsManager>();
        objectsInContact = new List<ExternalForce>();
        force = new List<Vector3>();

        if(!alwaysActive)
            fieldsManager.forceFields.Add(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(boxPosition, new Vector3(boxDimensions.x, boxDimensions.y, 0));
        float angle = Vector3.Angle(Vector3.right, gameObject.transform.up);
        Vector3 bottomLeft = new Vector3(gameObject.transform.position.x - magnetPadRadius, gameObject.transform.position.y, gameObject.transform.position.z);
        //bottomLeft = gameObject.transform.rotation * bottomLeft;
        bottomLeft = Quaternion.AngleAxis(angle, gameObject.transform.position + gameObject.transform.forward) * bottomLeft;
        bottomLeft = gameObject.transform.position + -gameObject.transform.right * magnetPadRadius;

        Vector3 bottomRight = new Vector3(gameObject.transform.position.x + magnetPadRadius, gameObject.transform.position.y, gameObject.transform.position.z);
        bottomRight = gameObject.transform.position + gameObject.transform.right * magnetPadRadius;

        Vector3 topLeft = new Vector3(gameObject.transform.position.x - magnetPadRadius,
                                        gameObject.transform.position.y + magnetPadHeight, gameObject.transform.position.z);
        topLeft = gameObject.transform.position + (gameObject.transform.up * magnetPadHeight) - gameObject.transform.right * magnetPadRadius;

        Vector3 topRight = new Vector3(gameObject.transform.position.x + magnetPadRadius,
                                        gameObject.transform.position.y + magnetPadHeight, gameObject.transform.position.z);
        topRight = gameObject.transform.position + (gameObject.transform.up * magnetPadHeight) + gameObject.transform.right * magnetPadRadius;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(bottomRight, topRight);
    }

    // Update is called once per frame
    void FixedUpdate() {
        PickUpItem itemPickedUp = null;
        force.Clear();
        objectsInContact.Clear();   //Clear the list so that new objects can be added

        RaycastHit2D[] c = Physics2D.CircleCastAll(gameObject.transform.position, magnetPadRadius, gameObject.transform.up, magnetPadHeight);


        //If the ray hits the player object, then apply a force to the player
        int count = 0;
        for (int i = 0; i < c.Length; i++)
            if (c[i].collider != null)
                if ((c[i].collider.gameObject.GetComponent<ExternalForce>() != null))
                {
                    objectsInContact.Add(c[i].collider.GetComponent<ExternalForce>());
                    force.Add(Vector3.zero);    //initializing the force component to 0

                }

        float randomMax = Random.Range(1.0f, 1.1f);
        float randomMin = Random.Range(0.9f, 0.95f);
        multiplier = HelperFunctions.CounterAnimation(multiplier, randomMin, randomMax, GameManager.Instance.DeltaTime, sign, out sign);
        for (int i = 0; i < objectsInContact.Count; i++)
        {
            if (Vector3.Dot(gameObject.transform.up, (objectsInContact[i].gameObject.transform.position - gameObject.transform.position).normalized) > 0)
            {
                count++;
                isInContactWithPlayer = true;

                Vector3 hitPosition = objectsInContact[i].gameObject.transform.position;     //The hit position of the ray
                float distanceFrac = Mathf.Clamp(Vector3.Distance(hitPosition, gameObject.transform.position) / floatHeight, 0, 1);
                //Debug.Log("Fraction of distance is: " + distanceFrac);

                force[i] = gameObject.transform.up * magneticForce * 100 * (1 - distanceFrac) * multiplier;

                //Debug.Log("Force of magnetic field is: " + force[i]);

            }

        }
        if (objectsInContact.Count == 0)
        {
            multiplier = 0;
        }
    }
    private void Update()
    {
        //Depending on the type of item the player currently has, the force must be cut off appropriately
        for (int i = 0; i < objectsInContact.Count; i++)
        {
            itemMultiplier = 1;
            if (objectsInContact[i].GetComponent<MovementScript2D>() != null)
            {
                MovementScript2D playerms = objectsInContact[i].GetComponent<MovementScript2D>();

                playerms.GetComponent<PlayerMechanics>().isInContactwithField = true;
                //if(playerms.playerState == PlayerStates.falling)
                //    playerms.GetComponent<AudioSource>().Stop(); //If within the field, stop all the sounds

                //If no item is picked up
                if (objectsInContact[i].GetComponent<PlayerMechanics>().itemPickedUp == null)
                {
                    itemMultiplier = 0.0f;
                }
                else
                {
                    switch (objectsInContact[i].GetComponent<PlayerMechanics>().itemPickedUp.GetComponent<PickUpItem>().item)
                    {
                        case Item.light:
                            itemMultiplier = 0.0f;
                            break;
                        case Item.magnet:
                            PolarityCheck(objectsInContact[i].gameObject);
                            if(itemMultiplier > 0)
                            {
                                if (force[i] != Vector3.zero)
                                {
                                    Debug.Log("setting the damp");
                                    playerms.externalVerticalMovementDamp = 0.2f;
                                }
                            }
                            break;
                        case Item.parachute:
                            itemMultiplier = 0.0f;
                            break;
                        default:
                            itemMultiplier = 0.0f;
                            break;
                    }
                }
            }
            objectsInContact[i].force += force[i] * itemMultiplier;
        }
    }

    void PolarityCheck(GameObject itemInContact)
    {
        //now we know that pickup item is the magnet
        bool itemNorth = itemInContact.GetComponent<PlayerMechanics>().itemPickedUp.GetComponent<PickUpItem>().north;
        //Item and field are north facing
        if(north && itemNorth)
        {
            itemMultiplier = 1.0f;
        }
        //If item and field are south facing
        else if(!north && !itemNorth)
        {
            itemMultiplier = 1.0f;
        }
        else
        {
            itemMultiplier = -0.4f;
            itemInContact.GetComponent<MovementScript2D>().externalHorizontalMovementDamp = 0.7f;
            //itemInContact.GetComponent<MovementScript2D>().externalVerticalMovementDamp = 0.2f;
        }
        
    }

}
