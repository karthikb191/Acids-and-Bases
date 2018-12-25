using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldsManager : MonoBehaviour {

    private MovementScript2D playerms;

    
    public float tolerance;

    //get all the fields in the level
    public List<GameObject> forceFields;

    //camera manager script
    private CameraScript cameraScript;

    //get all the forcefields that are in contact with the player
    [SerializeField]
    public List<AirPadScript> airFieldsInContact;
    [SerializeField]
    public List<MagneticFieldScript> magneticFieldsInContact;

    public static Vector3 fieldsForce;

    private void Awake()
    {
        forceFields = new List<GameObject>();
    }

    // Use this for initialization
    void Start () {
        playerms = FindObjectOfType<MovementScript2D>();

        cameraScript = FindObjectOfType<CameraScript>();
        GameObject[] forceFieldArray;
        //forceFieldArray = GameObject.FindGameObjectsWithTag("tag_forcefield");
        //forceFields.AddRange(forceFieldArray);

        Debug.Log("Tolerance is: " + tolerance);
	}

    private void FixedUpdate()
    {
        
    }

    // Update is called once per frame
    void Update () {
        //Add the fields force to the ExternalForce script.
        
        //playerms.externalForce += fieldsForce;
        //Debug.Log("Fields force is: " + fieldsForce);
        foreach (GameObject field in forceFields) {
            if (field.transform.position.x < GameManager.Instance.cameraBounds[2].x + tolerance &&
                field.transform.position.x > GameManager.Instance.cameraBounds[1].x - tolerance &&
                field.transform.position.y > GameManager.Instance.cameraBounds[3].y - tolerance &&
                field.transform.position.y < GameManager.Instance.cameraBounds[2].y + tolerance) {
                //Debug.Log("Border tolerance: " + tolerance);
                //Debug.Log("inside the cam view");
                if(!field.activeSelf)
                    field.SetActive(true);

            }
            else
                if(field.activeSelf)
                    field.SetActive(false);
        }

        //CheckForContactWithPlayer();
	}

    void CheckForContactWithPlayer() {
        foreach (GameObject field in forceFields) {
            if (field.GetComponent<AirPadScript>() != null) {
                AirPadScript airField = field.GetComponent<AirPadScript>();
                //If the pad is in contact with the player
                if (airField.isInContactWithPlayer) {
                    //If the list doesnt contain the field add it
                    if (!airFieldsInContact.Contains(airField)) {
                        airFieldsInContact.Add(airField);
                    }
                }
                //If the airpad is not in contact with the player
                else {

                    if (airFieldsInContact.Contains(airField)) {
                        airFieldsInContact.Remove(airField);
                    }

                }
            }
            else if(field.GetComponent<MagneticFieldScript>() != null) {
                MagneticFieldScript magneticField = field.GetComponent<MagneticFieldScript>();
                //If the pad is in contact with the player
                if (magneticField.isInContactWithPlayer) {
                    //If the list doesnt contain the field add it
                    if (!magneticFieldsInContact.Contains(magneticField)) {
                        magneticFieldsInContact.Add(magneticField);
                    }
                }
                //If the magneticPad is not in contact with the player
                else {

                    if (magneticFieldsInContact.Contains(magneticField)) {
                        magneticFieldsInContact.Remove(magneticField);
                    }

                }
                
            }
        }
        

    }
    private void LateUpdate()
    {
        fieldsForce = Vector3.zero;
    }
}
