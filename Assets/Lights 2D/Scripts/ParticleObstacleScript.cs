using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObstacleScript : MonoBehaviour
{
    LightsManager lightsManager;
    Material obstacleMat;
    ParticleSystem particleSystem;
    MeshRenderer meshRenderer;
    // Use this for initialization
    void Start()
    {
        lightsManager = FindObjectOfType<LightsManager>();
        particleSystem = GetComponent<ParticleSystem>();
        meshRenderer = GetComponent<MeshRenderer>();
        if (particleSystem == null && meshRenderer == null)
        {
            Debug.Log("no renderer is present on the object");

        }
        if (particleSystem)
        {
            Material[] mats = particleSystem.GetComponent<Renderer>().materials;
            foreach (Material m in mats)
            {
                obstacleMat = m;
                //Debug.Log("material: " + obstacleMat.name);
                break;

            }
        }
        if (meshRenderer)
        {
            Material[] mats = meshRenderer.materials;
            foreach (Material m in mats)
            {
                obstacleMat = m;
                //Debug.Log("material: " + obstacleMat.name);
                break;

            }
        }


    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Debug.Log("Obstacle script is executing");
        for (int i = 0; i < lightsManager.lights.Count; i++)
        {
            obstacleMat.SetColorArray("_LightColors", lightsManager.lightColors);
            obstacleMat.SetVectorArray("_LightPositions", lightsManager.lightPositions);
            obstacleMat.SetVectorArray("_LightDirections", lightsManager.lightDirections);
            obstacleMat.SetFloatArray("_LightDistances", lightsManager.lightDistances);
            obstacleMat.SetFloatArray("_NoFadeDistances", lightsManager.noFadeDistances);
            obstacleMat.SetFloatArray("_LightMixtureAdjustmentControllers", lightsManager.lightMixtureAdjustmentControllers);
            obstacleMat.SetFloatArray("_LightAngles", lightsManager.lightAngles);

            obstacleMat.SetFloatArray("_IsAPixelLight", lightsManager.isAPixelLight);


            //Debug.Log("light angle : " + lightsManager.lightAngles[0]);
        }
    }
}

