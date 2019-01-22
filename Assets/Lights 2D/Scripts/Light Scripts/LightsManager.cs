using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightsManager : MonoBehaviour {

    public Material sampleVertexMaterial;

    public float globalLightMultiplier = 0.5f;

    //Array of light properties
    public List<Color> lightColors;
    public List<Vector4> lightPositions;
    public List<Vector4> lightDirections;
    public List<float> lightDistances;
    public List<float> noFadeDistances;
    public List<float> lightMixtureAdjustmentControllers;
    public List<float> lightAngles;
    public List<float> isAPixelLight;   //A flag that indicates that the light is a pixel light

    //Arrays of the light projection properties
    private List<Vector4> leftVector;
    private List<Vector4> rightVector;
    private List<Vector4> perpendicularLeftVector;
    private List<Vector4> perpendicularRightVector;
    private List<Vector4> leftFogVector;
    private List<Vector4> rightFogVector;
    private List<Vector4> perpendicularLeftFogVector;
    private List<Vector4> perpendiculatRightFogVector;
    
    //The light objects in the scene. Depends on their activation, they must be emitting the light
    //Checking for lights activation must also be done on the lights manager
    public List<Lights2D> lights;
    public static List<Lights2D> lightsInScene;
    public static int maxLightsAllowed = 10;    //This is the control for the maximum number of lights allowed

    private GameObject lightMesh;
    private Material lightMat;

    private bool initialized = false;
    private void Awake()
    {
        if (!initialized)
        {
            InitializeLists();   
        }
    }
    void InitializeLists()
    {
        lightsInScene = new List<Lights2D>();

        lights = new List<Lights2D>();
        lightColors = new List<Color>();
        lightPositions = new List<Vector4>();
        lightDirections = new List<Vector4>();
        lightDistances = new List<float>();
        noFadeDistances = new List<float>();
        lightMixtureAdjustmentControllers = new List<float>();
        lightAngles = new List<float>();


        leftVector = new List<Vector4>();
        rightVector = new List<Vector4>();
        perpendicularLeftVector = new List<Vector4>();
        perpendicularRightVector = new List<Vector4>();
        leftFogVector = new List<Vector4>();
        rightFogVector = new List<Vector4>();
        perpendicularLeftFogVector = new List<Vector4>();
        perpendiculatRightFogVector = new List<Vector4>();

        initialized = true;
    }

    bool sorting = false;
    // Use this for initialization
    void Start () {
        sorting = false;
        //lights.AddRange(FindObjectsOfType<Lights2D>());
        //lightMat = lightMesh.GetComponent<MeshRenderer>().sharedMaterial;
        if (!initialized)
            InitializeLists();

        if (Application.isPlaying)
        {
            StartCoroutine(SortLights());
        }
    }

    public bool enableLightSelection;

    private void Update()
    {
#if UNITY_EDITOR
        if (!initialized)
            InitializeLists();
#endif

        Shader.SetGlobalFloat("_Multiplier", globalLightMultiplier);

        lights = lightsInScene;

        //Sort the lights in the scene and add the first 10 lights to the lights list 
        if (!sorting && Application.isPlaying)
            StartCoroutine(SortLights());

#if UNITY_EDITOR
        if(!Application.isPlaying && !enableLightSelection)
            SortLightsEditor();
        
        if (!Application.isPlaying)
            LateUpdate();
#endif

        //AddToLists();
        //Debug.Log("Lights manager running");
    }

    IEnumerator SortLights()
    {
        //Debug.Log("lights list: " + lightsInScene.Count);
        if (!sorting)
        {
            //Debug.Log("sort");
            sorting = true;
            int counter = 0;
            int c = 0;
            Vector3 cam = new Vector3(GameManager.Instance.cameraBounds[0].x, GameManager.Instance.cameraBounds[0].y, 0);
            //Debug.Log("camera pos:" + cam);
            //Debug.Log("lights in scene: " + lightsInScene.Count);

            for (int i = 0; i < lightsInScene.Count; i++)
            {
                Lights2D l = lightsInScene[i];
                float distance1 = Vector3.SqrMagnitude(cam - lightsInScene[i].transform.position);
                if (c < 5)
                {
                    for (int j = i + 1; j < lightsInScene.Count; j++)
                    {
                        float distance2 = Vector3.SqrMagnitude(cam - lightsInScene[j].transform.position);
                        //Debug.Log("distance: " + distance2 + "light distance : " + lightsInScene[j].distance * lightsInScene[j].distance);
                        if (distance2 < distance1)
                        {
                            //Swap the lights
                            Lights2D temp = lightsInScene[i];
                            lightsInScene[i] = lightsInScene[j];
                            lightsInScene[j] = temp;

                        }
                        l = lightsInScene[i];
                        counter++;
                    }
                }
                if(c > 5)
                {
                    if (!l.controlledBySwitch)
                        l.gameObject.SetActive(false);
                    
                }

                c++;

                //l.gameObject.SetActive(false);
                if(counter > 10)
                {
                    yield return new WaitForFixedUpdate();
                    counter = 0;
                }
                
            }

            //Gets the closest 5 lights and places them in the lights list
            lights.Clear();

            int lightCounter = 0;
            int maxLights = 10;
            while(lightCounter < maxLights)
            {
                if (lightCounter > lightsInScene.Count - 1)
                    break;
                if (lightsInScene[lightCounter].controlledBySwitch)
                {
                    if (lightsInScene[lightCounter].gameObject.activeSelf)
                    {
                        lights.Add(lightsInScene[lightCounter]);
                    }
                    else
                        maxLights++;
                }
                else
                {
                    lightsInScene[lightCounter].gameObject.SetActive(true);
                    lights.Add(lightsInScene[lightCounter]);
                    
                }
                lightCounter++;
            }
            //for (int i = 0; i < 5; i++)
            //{
            //    if (i > lightsInScene.Count - 1)
            //        break;
            //
            //    lightsInScene[i].gameObject.SetActive(true);
            //    lights.Add(lightsInScene[i]);
            //}
        }
        sorting = false;
        yield break;
    }

    void SortLightsEditor()
    {
        
        //Debug.Log("lights list: " + lightsInScene.Count);
        if (!sorting)
        {
            sorting = true;
            int counter = 0;
            int c = 0;
            Vector3 cam = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
            //Debug.Log("lights in scene: " + lightsInScene.Count);

            for (int i = 0; i < 5; i++)
            {
                Lights2D l = lightsInScene[i];
                float distance1 = Vector3.SqrMagnitude(cam - lightsInScene[i].transform.position);
                for (int j = i + 1; j < lightsInScene.Count; j++)
                {
                    float distance2 = Vector3.SqrMagnitude(cam - lightsInScene[j].transform.position);
                    //Debug.Log("distance: " + distance2 + "light distance : " + lightsInScene[j].distance * lightsInScene[j].distance);
                    if (distance2 < distance1)
                    {
                        //Swap the lights
                        Lights2D temp = lightsInScene[i];
                        lightsInScene[i] = lightsInScene[j];
                        lightsInScene[j] = temp;

                    }
                    l = lightsInScene[i];
                    counter++;
                }
                if (c > 5)
                {
                    l.gameObject.SetActive(false);
                }
                c++;
                //l.gameObject.SetActive(false);
                if (counter > 10)
                {
                    counter = 0;
                }

            }
            //Debug.Log("Lights are being sorted in editor view");
            //Gets the closest 5 lights and places them in the lights list
            lights.Clear();
            for (int i = 0; i < 5; i++)
            {
                if (i > lightsInScene.Count - 1)
                    break;

                lightsInScene[i].gameObject.SetActive(true);
                lights.Add(lightsInScene[i]);
            }
        }
        sorting = false;
    }


    void LateUpdate () {
        //sampleVertexMaterial.SetVector("_light", gameObject.transform.position);
        ClearLists();
        //Set the global multiplier value for all the sprites
        //Debug.Log("Late Update");
        for(int i = 0; i< lights.Count; i++)
        {
            lightColors.Add(lights[i].lightColor);
            lightPositions.Add(lights[i].transform.position);
            //Debug.Log(lights[i].transform.position);
            lightDirections.Add(lights[i].transform.right);
            lightDistances.Add(lights[i].distance);
            noFadeDistances.Add(lights[i].noFadeDistance);
            lightMixtureAdjustmentControllers.Add(lights[i].lightMixtureAdjustmentController);

            isAPixelLight.Add(lights[i].isAPixelLight ? 1 : 0); //Checks if the light is a pixel light and converts the result to float
    
            lightAngles.Add(Mathf.Cos(lights[i].lightAngle * Mathf.Deg2Rad));
    
            leftVector.Add(lights[i].actualLeftVector);
            rightVector.Add(lights[i].actualRightVector);
            perpendicularLeftVector.Add(lights[i].perpendicularLeftVector);
            perpendicularRightVector.Add(lights[i].perpendicularRightVector);
    
            leftFogVector.Add(lights[i].actualLeftFogVector);
            rightFogVector.Add(lights[i].actualRightFogVector);
            perpendicularLeftFogVector.Add(lights[i].perpendicularLeftFogVector);
            perpendiculatRightFogVector.Add(lights[i].perpendicularRightFogVector);
            //Debug.Log(lights[i].actualRightVector);
            //Debug.Log(lights[i].actualRightFogVector);
        }
        
        //Add these to the shader
        //lightMat.SetColorArray("_Color", lightColors);
        //lightMat.SetVectorArray("_LightPosition", lightPositions);
        //lightMat.SetVectorArray("_LightDirection", lightDirections);
        //lightMat.SetFloatArray("_Distance", lightDistances);
        //lightMat.SetFloatArray("_NoFadeDistance", noFadeDistances);
        //lightMat.SetFloatArray("_LightMixtureAdjustmentController", lightMixtureAdjustmentControllers);
    
        //lightMat.SetVectorArray("_ActualLeftVector", leftVector);
        //lightMat.SetVectorArray("_ActualRightVector", rightVector);
        //lightMat.SetVectorArray("_PerpendicularLeftVector", perpendicularLeftVector);
        //lightMat.SetVectorArray("_PerpendicularRightVector", perpendicularRightVector);
        //lightMat.SetVectorArray("_ActualLeftFogVector", leftFogVector);
        //lightMat.SetVectorArray("_ActualRightFogVector", rightFogVector);
        //lightMat.SetVectorArray("_PerpendiculatLeftFogVector", perpendicularLeftFogVector);
        //lightMat.SetVectorArray("_PerpendiculatRightFogVector", perpendiculatRightFogVector);
    }

    void ClearLists()
    {
        lightColors.Clear();
        lightPositions.Clear();
        lightDirections.Clear();
        lightDistances.Clear();
        noFadeDistances.Clear();
        lightMixtureAdjustmentControllers.Clear();
        lightAngles.Clear();

        leftVector.Clear();
        rightVector.Clear();
        perpendicularLeftVector.Clear();
        perpendicularRightVector.Clear();
        leftFogVector.Clear();
        rightFogVector.Clear(); 
        perpendicularLeftFogVector.Clear();
        perpendiculatRightFogVector.Clear();
        isAPixelLight.Clear();
    }
}
