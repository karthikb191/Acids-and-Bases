using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Lights2D : MonoBehaviour {
    
    public GameObject lightMesh;

    public Color lightColor = Color.white;

    public float noFadeDistance = 0.2f;
    public float distance = 0.5f;

    public float angle = 10;
    public float fog = 0;

    public float lightMixtureAdjustmentController = 0.4f;

    public bool isAPixelLight = false;

    public bool castRay = false;

    
    public bool controlledBySwitch = false;

    [HideInInspector]
    public Material lightMat;


    [HideInInspector]
    public Vector3 perpendicularRightVector;
    [HideInInspector]
    public Vector3 perpendicularLeftVector;

    [HideInInspector]
    public Vector3 perpendicularLeftFogVector;
    [HideInInspector]
    public Vector3 perpendicularRightFogVector;

    [HideInInspector]
    public Vector3 actualRightVector;
    [HideInInspector]
    public Vector3 actualLeftVector;

    [HideInInspector]
    public Vector3 actualLeftFogVector;
    [HideInInspector]
    public Vector3 actualRightFogVector;

    [HideInInspector]
    public Vector3 rightEndPoint;
    [HideInInspector]
    public Vector3 leftEndPoint;
    [HideInInspector]
    public Vector3 topEndPoint;

    [HideInInspector]
    public float lightAngle;

    public bool lightIsActive = true;
    public LayerMask whatShouldIDetect;
    // Update is called once per frame
    private void Start()
    {
        //lightMat = lightMesh.GetComponent<MeshRenderer>().sharedMaterial;
        if (isAPixelLight)
        {
            LightMap lightMap = GetComponent<LightMap>();
            if(lightMap != null)
                lightMat = GetComponent<LightMap>().lightMapMesh.GetComponent<MeshRenderer>().sharedMaterial;

            Debug.Log("Light material is: " + lightMat);
        }

        //Each light will add itself to the lights manager once it's activated
        LightsManager.lightsInScene.Add(this);
    }
    void Update () {

#if UNITY_EDITOR
        if (!LightsManager.lightsInScene.Contains(this))
            LightsManager.lightsInScene.Add(this);
#endif

        if (isAPixelLight)
        {
            if(lightMat == null)
            {
                LightMap lightMap = GetComponent<LightMap>();
                if (lightMap != null)
                    lightMat = GetComponent<LightMap>().lightMapMesh.GetComponent<MeshRenderer>().sharedMaterial;
                Debug.Log("Light material is: " + lightMat);
            }
        }

        if (lightIsActive && isAPixelLight)
        {
            if(gameObject.transform.parent != null)
            {
                MovementScript2D player = transform.GetComponentInParent<MovementScript2D>();
                if (player != null)
                {
                    Vector3 playerAngles = player.transform.rotation.eulerAngles;
                    if (gameObject.transform.parent.localScale.x == -1)
                    {
                        //gameObject.transform.rotation = Quaternion.Euler(0, 0, -180);
                        Rotate(Quaternion.Euler(playerAngles.x, playerAngles.y, playerAngles.z - 180));
                    }
                    else
                    {
                        //gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                        Rotate(Quaternion.Euler(playerAngles.x, playerAngles.y, playerAngles.z));

                    }
                }
            }
            //set the right vector according to the angle provided

            //First, get the right guy's direction
            //transform.GetChild(0).RotateAround(transform.position, transform.up, rightAngle);

            perpendicularLeftVector = Quaternion.AngleAxis(angle - 90, transform.forward) * transform.right ;
            perpendicularRightVector = Quaternion.AngleAxis(90 - angle, transform.forward) * transform.right ;

            perpendicularLeftFogVector = Quaternion.AngleAxis(angle + fog - 90, transform.forward) * transform.right;
            perpendicularRightFogVector = Quaternion.AngleAxis(90 - angle + (-fog), transform.forward) * transform.right;
            

            actualRightVector = Quaternion.AngleAxis(-angle, transform.forward) * transform.right;
            actualLeftVector = Quaternion.AngleAxis(angle, transform.forward) * transform.right;
            

            actualLeftFogVector = Quaternion.AngleAxis(angle + fog, transform.forward) * transform.right ;
            actualRightFogVector = Quaternion.AngleAxis(-angle + (-fog), transform.forward) * transform.right;

            //Debug.DrawLine(gameObject.transform.position, perpendicularRightVector);
            //Debug.DrawLine(gameObject.transform.position, perpendicularLeftVector);
            //
            Debug.DrawLine(gameObject.transform.position, actualLeftVector + gameObject.transform.position);
            //Debug.DrawLine(gameObject.transform.position, actualRightVector + gameObject.transform.position);

            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + actualLeftFogVector);
            //Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + actualRightFogVector);
            //Debug.DrawLine(gameObject.transform.position, perpendicularRightFogVector);


            float rightVectorAngle = Vector3.Angle(transform.forward, perpendicularRightFogVector);
            float leftVectorAngle = Vector3.Angle(perpendicularLeftFogVector, transform.forward);
            //Debug.Log("right vector angle is: " + rightVectorAngle);

            lightAngle = (angle + fog);
            
            //setting the properties for the mesh
            
            lightMat.SetColor("_Color", lightColor);
            lightMat.SetVector("_LightPosition", transform.position);
            lightMat.SetVector("_LightDirection", transform.forward);

            lightMat.SetFloat("_Distance", distance);
            lightMat.SetFloat("_NoFadeDistance", noFadeDistance);

            lightMat.SetFloat("_LightMixtureAdjustmentController", lightMixtureAdjustmentController);


            lightMat.SetVector("_PerpendicularRightVector", perpendicularRightVector);
            lightMat.SetVector("_PerpendicularLeftVector", perpendicularLeftVector);

            lightMat.SetVector("_PerpendiculatRightFogVector", perpendicularRightFogVector);
            lightMat.SetVector("_PerpendiculatLeftFogVector", perpendicularLeftFogVector);

            lightMat.SetVector("_ActualRightVector", actualRightVector);
            lightMat.SetVector("_ActualLeftVector", actualLeftVector);
            lightMat.SetVector("_ActualRightFogVector", actualRightFogVector);
            lightMat.SetVector("_ActualLeftFogVector", actualLeftFogVector);
            
        }

        //Cast Ray to the light detector. If light hits the light detector, add the light to the list lightsInContact
        if (castRay && Application.isPlaying)
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.right, (Mathf.Clamp(distance, 0, Mathf.Infinity)), whatShouldIDetect);
            if(hit.collider != null)
            {
                hit.collider.GetComponent<LightDetector>().lightsInContact.Add(this.gameObject);
            }
        }


    }



    void Rotate(Quaternion rotation) {
        float angle = Quaternion.Angle(gameObject.transform.rotation, rotation);
        if (angle > 0.2f)
        {
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, rotation, 0.3f);
        }
        else
        {
            gameObject.transform.rotation = rotation;
        }

    }

    private void OnDestroy()
    {
        //Once the light has been destroyed, remove it from the list
        //LightsManager.lightsInScene.Remove(this);
    }
}
