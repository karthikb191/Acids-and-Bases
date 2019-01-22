using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObstacleScript : MonoBehaviour {
    LightsManager lightsManager;
    Material obstacleMat;
    SpriteRenderer spriteRenderer;
    MeshRenderer meshRenderer;

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start () {
        lightsManager = FindObjectOfType<LightsManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
        if(spriteRenderer == null && meshRenderer == null)
        {
            Debug.Log("no renderer is present on the object");
            
        }
        if (spriteRenderer)
        {
            Material[] mats = spriteRenderer.sharedMaterials;
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
    private void Update()
    {
        if (!Application.isPlaying)
            LateUpdate();
    }

    // Update is called once per frame
    void LateUpdate() {
        //Debug.Log("Obstacle script is executing");
        //Debug.Log("Lights count in edit mode: " + lightsManager.lights.Count);
        //Debug.Log("Late update 2");
		for(int i = 0; i < lightsManager.lights.Count; i++)
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

    void InitializeValuesInShader()
    {
        Debug.Log("vec: " + new Vector4[10]);
        obstacleMat.SetColorArray("_LightColors", new Color[10]);
        obstacleMat.SetVectorArray("_LightPositions", new Vector4[10]);
        obstacleMat.SetVectorArray("_LightDirections", new Vector4[10]);
        obstacleMat.SetFloatArray("_LightDistances", new float[10]);
        obstacleMat.SetFloatArray("_NoFadeDistances", new float[10]);
        obstacleMat.SetFloatArray("_LightMixtureAdjustmentControllers", new float[10]);
        obstacleMat.SetFloatArray("_LightAngles", new float[10]);

        obstacleMat.SetFloatArray("_IsAPixelLight", new float[LightsManager.maxLightsAllowed]);
    }
}
