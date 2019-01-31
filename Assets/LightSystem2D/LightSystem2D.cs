using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LightSystem2D : MonoBehaviour {

    Camera mainCamera;
    public Camera lightCamera;

    public float lightSize = 0.1f;
    public float lightcameraOffset = 3.0f;

    public LayerMask lightsLayer;
    public LayerMask lightBlockersLayer;

    public float emission = 1;

    /// <summary>
    /// This shader in this material is directly responsible for all the effects
    /// </summary>
    public Material OverlayMaterial;
    public Material pHScannerMaterial;

    float LightPixelsPerUnit { get { return 1 / lightSize; } }

    Vector2 offsetScale;

    RenderTexture finalScreenBlitTexture;
    //RenderTexture gameScreenTexture;
    RenderTexture lightsTexture;
    RenderTexture obstaclesTexture;
    RenderTexture ambientTexture;
    RenderTexture pHTexture;

    
    float rawCameraHeight;
    float rawCameraWidth;
    Vector2Int lightsRenderTextureSize;
    float rawSmallCameraHeight;
    Vector2Int smallLightTextureSize;


	// Use this for initialization
	void Start () {
        mainCamera = GetComponent<Camera>();

        if (lightCamera == null)
        {
            Debug.LogError("Light camera is null. Please set the light camera for lighting system to work");
            return;
        }

        //Main camera must not render lights and blockers. Set the culling for it
        mainCamera.cullingMask = mainCamera.cullingMask & ~(lightsLayer | lightBlockersLayer);

        //Get the width and height of the camera with a small offset
        rawCameraHeight = (mainCamera.orthographicSize + lightcameraOffset) * 2;
        rawCameraWidth = (mainCamera.orthographicSize * mainCamera.aspect + lightcameraOffset) * 2;

        lightsRenderTextureSize = new Vector2Int(Mathf.RoundToInt(rawCameraWidth * LightPixelsPerUnit),
                                    Mathf.RoundToInt(rawCameraHeight * LightPixelsPerUnit));

        rawSmallCameraHeight = mainCamera.orthographicSize * 2 * LightPixelsPerUnit;

        smallLightTextureSize = new Vector2Int(Mathf.RoundToInt(rawSmallCameraHeight * mainCamera.aspect), 
                                                Mathf.RoundToInt(rawSmallCameraHeight));

        //Creating render textures for effects
        finalScreenBlitTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 0, RenderTextureFormat.ARGB32);
        finalScreenBlitTexture.filterMode = FilterMode.Point;

        lightsTexture = new RenderTexture(lightsRenderTextureSize.x, lightsRenderTextureSize.y, 0, RenderTextureFormat.ARGB32);
        lightsTexture.filterMode = FilterMode.Point;

        obstaclesTexture = new RenderTexture(lightsRenderTextureSize.x, lightsRenderTextureSize.y, 0, RenderTextureFormat.ARGB32);
        
        pHTexture = new RenderTexture(lightsRenderTextureSize.x, lightsRenderTextureSize.y, 0, RenderTextureFormat.ARGB32);
        
	}
	

    private void OnPreRender()
    {
        lightCamera.orthographicSize = mainCamera.orthographicSize;
        RenderTexture.active = finalScreenBlitTexture;
        GL.Clear(true, true, new Color(0, 0, 0, 0));
    }

    //Source renders all the layers besides lights layer and lights blocker layer
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetUpLightCamera();
        RenderLightSources();
        OverlayEffects(source, destination);
        //Graphics.Blit(source, destination);
    }

    void SetUpLightCamera()
    {
        lightCamera.orthographicSize = mainCamera.orthographicSize;
        lightCamera.fieldOfView = mainCamera.fieldOfView;
        lightCamera.aspect = smallLightTextureSize.x / (float)smallLightTextureSize.y;
    }

    void RenderLightSources()
    {
        lightsTexture.DiscardContents();
        //The camera doesn't need to be enables to force it to render
        lightCamera.enabled = false;
        Color bgColor = lightCamera.backgroundColor;
        lightCamera.backgroundColor = new Color(0, 0, 0, 0);
        lightCamera.cullingMask = lightsLayer;
        lightCamera.targetTexture = lightsTexture;

        //Forcing the camera to render writes the pixels to the render texture...Very important point to note
        lightCamera.Render();

        //The stuff camera sees is not stored in the render texture. Pass it as a global shader variable so that shaders can access it
        Shader.SetGlobalTexture("_LightsTexture", lightsTexture);

        //Reset the lights camera
        lightCamera.targetTexture = null;
        lightCamera.backgroundColor = bgColor;
        lightCamera.cullingMask = 0;
        
    }
    

    void OverlayEffects(RenderTexture source, RenderTexture destination)
    {
        //Source contains everything besides the lights and light obstacles
        OverlayMaterial.SetTexture("_MainTex", source);
        finalScreenBlitTexture.DiscardContents();

        Graphics.Blit(null, finalScreenBlitTexture, OverlayMaterial);
        
        //pH Shader overlay, if it is activated
        
        //finalScreenBlitTexture.DiscardContents();

        Graphics.Blit(finalScreenBlitTexture, destination, pHScannerMaterial);
        //GL.Clear(true, true, new Color(0, 0, 0, 0));
    }

}
