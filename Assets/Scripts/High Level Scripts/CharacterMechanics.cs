using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CharacterMechanics : MonoBehaviour {

    protected SpriteRenderer liquidSprite;
    protected Anima2D.SpriteMeshInstance spriteMeshInstance;
    public bool PhTransitionMechanic = true;

    public Color initialCharacterColor;

    [SerializeField]
    [Range(0, 14)]
    protected float phValue = 7;
    
    Color currentColor = Color.white;

    protected bool revealpH = false;

    MaterialPropertyBlock propertyBlock;

    Dictionary<float, Color> phColorDictionary = new Dictionary<float, Color>()
    {
        {0, new Color(1, 0 , 0)}, {1, new Color(0.9568f, 0.3921f, 0.1960f)},

        {2, new Color(0.9686f, 0.5607f , 0.1176f)}, {3, new Color(1, 0.7647f, 0.1411f)},

        {4, new Color(1, 1 , 0)}, {5, new Color(0.5098f, 0.7647f, 0.2392f)},

        {6, new Color(0.3019f, 0.7176f, 0.2862f)}, {7, new Color(0.2f, 0.6627f, 0.2941f)},

        {8, new Color(0.0392f, 0.7215f, 0.7137f)}, {9, new Color(0.2745f, 0.5647f, 0.8039f)},

        {10, new Color(0.2196f, 0.3254f, 0.6431f)}, {11, new Color(0.3529f, 0.3176f, 0.6352f)},

        {12, new Color(0.3882f, 0.2705f, 0.6156f)}, {13, new Color(0.4235f, 0.1294f, 0.5019f)},

        { 14, new Color(0.2862f, 0.0901f, 0.4313f)}

    };

    protected void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        if (transform.Find("Sprite") != null)
        {
            if(transform.Find("Sprite").transform.Find("Body") != null)
            {
                spriteMeshInstance = transform.Find("Sprite").transform.Find("Body").GetComponent<Anima2D.SpriteMeshInstance>();
                Material mat = Instantiate(spriteMeshInstance.GetComponent<Anima2D.SpriteMeshInstance>().sharedMaterial);
                //skinnedMeshRenderer.material = mat;
                spriteMeshInstance.GetComponent<Anima2D.SpriteMeshInstance>().sharedMaterial = mat; 
            }
            liquidSprite = transform.Find("Sprite").transform.Find("Liquid").GetComponent<SpriteRenderer>();
        }

        if (liquidSprite != null)
            liquidSprite.color = phColorDictionary[Mathf.FloorToInt(phValue + 0.1f)];
        else
        {
            PhTransitionMechanic = false;
        }
        
        //Checking for the sprite mesh renderer
        if(spriteMeshInstance != null)
        {
            spriteMeshInstance.GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", initialCharacterColor);
            spriteMeshInstance.GetComponent<Renderer>().SetPropertyBlock(propertyBlock);

            if (PhTransitionMechanic && revealpH)
            {
                Debug.Log("Changed color");
                spriteMeshInstance.sharedMaterial.SetColor("_LiquidColor", phColorDictionary[Mathf.FloorToInt(phValue + 0.1f)]);
                
            }
        }

    }

    // Update is called once per frame
    protected void Update () {
# if UNITY_EDITOR
        if (Application.isPlaying)
        {
            spriteMeshInstance.GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", initialCharacterColor);
            spriteMeshInstance.GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
        }
#endif
        if (PhTransitionMechanic && revealpH)
            CheckForPhChange();
	}

    #region pH Functions

    public void RevealpH(bool result)
    {
        revealpH = result;
    }
    public bool IspHrevealed()
    {
        return revealpH;
    }

    void SelectpHDictionary(PHIndicatorList indicator)
    {
        switch (indicator)
        {
            case PHIndicatorList.pH_Paper:
                phColorDictionary = new Dictionary<float, Color>()
                                        {
                                            {0, new Color(1, 0 , 0)}, {1, new Color(0.9568f, 0.3921f, 0.1960f)},

                                            {2, new Color(0.9686f, 0.5607f , 0.1176f)}, {3, new Color(1, 0.7647f, 0.1411f)},

                                            {4, new Color(1, 1 , 0)}, {5, new Color(0.5098f, 0.7647f, 0.2392f)},

                                            {6, new Color(0.3019f, 0.7176f, 0.2862f)}, {7, new Color(0.2f, 0.6627f, 0.2941f)},

                                            {8, new Color(0.0392f, 0.7215f, 0.7137f)}, {9, new Color(0.2745f, 0.5647f, 0.8039f)},

                                            {10, new Color(0.2196f, 0.3254f, 0.6431f)}, {11, new Color(0.3529f, 0.3176f, 0.6352f)},

                                            {12, new Color(0.3882f, 0.2705f, 0.6156f)}, {13, new Color(0.4235f, 0.1294f, 0.5019f)},

                                            { 14, new Color(0.2862f, 0.0901f, 0.4313f)}

                                        };
                break;
            case PHIndicatorList.Turmeric:
                phColorDictionary = new Dictionary<float, Color>()
                                        {
                                            {0, new Color(1, 0 , 0)}, {1, new Color(0.9568f, 0.3921f, 0.1960f)},

                                            {2, new Color(0.9686f, 0.5607f , 0.1176f)}, {3, new Color(1, 0.7647f, 0.1411f)},

                                            {4, new Color(1, 1 , 0)}, {5, new Color(0.5098f, 0.7647f, 0.2392f)},

                                            {6, new Color(0.3019f, 0.7176f, 0.2862f)}, {7, new Color(0.2f, 0.6627f, 0.2941f)},

                                            {8, new Color(0.0392f, 0.7215f, 0.7137f)}, {9, new Color(0.2745f, 0.5647f, 0.8039f)},

                                            {10, new Color(0.2196f, 0.3254f, 0.6431f)}, {11, new Color(0.3529f, 0.3176f, 0.6352f)},

                                            {12, new Color(0.3882f, 0.2705f, 0.6156f)}, {13, new Color(0.4235f, 0.1294f, 0.5019f)},

                                            { 14, new Color(0.2862f, 0.0901f, 0.4313f)}

                                        };
                break;
            case PHIndicatorList.Vermilion:
                phColorDictionary = new Dictionary<float, Color>()
                                        {
                                            {0, new Color(1, 0 , 0)}, {1, new Color(0.9568f, 0.3921f, 0.1960f)},

                                            {2, new Color(0.9686f, 0.5607f , 0.1176f)}, {3, new Color(1, 0.7647f, 0.1411f)},

                                            {4, new Color(1, 1 , 0)}, {5, new Color(0.5098f, 0.7647f, 0.2392f)},

                                            {6, new Color(0.3019f, 0.7176f, 0.2862f)}, {7, new Color(0.2f, 0.6627f, 0.2941f)},

                                            {8, new Color(0.0392f, 0.7215f, 0.7137f)}, {9, new Color(0.2745f, 0.5647f, 0.8039f)},

                                            {10, new Color(0.2196f, 0.3254f, 0.6431f)}, {11, new Color(0.3529f, 0.3176f, 0.6352f)},

                                            {12, new Color(0.3882f, 0.2705f, 0.6156f)}, {13, new Color(0.4235f, 0.1294f, 0.5019f)},

                                            { 14, new Color(0.2862f, 0.0901f, 0.4313f)}

                                        };
                break;
            case PHIndicatorList.Bromythol_Blue:
                phColorDictionary = new Dictionary<float, Color>()
                                        {
                                            {0, new Color(1, 0 , 0)}, {1, new Color(0.9568f, 0.3921f, 0.1960f)},

                                            {2, new Color(0.9686f, 0.5607f , 0.1176f)}, {3, new Color(1, 0.7647f, 0.1411f)},

                                            {4, new Color(1, 1 , 0)}, {5, new Color(0.5098f, 0.7647f, 0.2392f)},

                                            {6, new Color(0.3019f, 0.7176f, 0.2862f)}, {7, new Color(0.2f, 0.6627f, 0.2941f)},

                                            {8, new Color(0.0392f, 0.7215f, 0.7137f)}, {9, new Color(0.2745f, 0.5647f, 0.8039f)},

                                            {10, new Color(0.2196f, 0.3254f, 0.6431f)}, {11, new Color(0.3529f, 0.3176f, 0.6352f)},

                                            {12, new Color(0.3882f, 0.2705f, 0.6156f)}, {13, new Color(0.4235f, 0.1294f, 0.5019f)},

                                            { 14, new Color(0.2862f, 0.0901f, 0.4313f)}

                                        };
                break;
        }
    }


    protected virtual void CheckForPhChange()
    {
        float stepSize = 0.1f;
        if(currentColor != phColorDictionary[Mathf.FloorToInt(phValue)])
        {
            //Debug.Log("Trying to achieve the target color: " + phValue);
            //get the destination PH
            Color targetColor = phColorDictionary[Mathf.FloorToInt(phValue+0.1f)];

            //currentColor = (currentColor + targetColor) / 0.2f;
            //currentColor = currentColor + (targetColor - currentColor) * stepSize;
            currentColor = (1-stepSize) * currentColor + (targetColor) * stepSize;

            if(Mathf.Abs(currentColor.r - targetColor.r) < 0.05f &&
                Mathf.Abs(currentColor.g - targetColor.g) < 0.05f &&
                Mathf.Abs(currentColor.b - targetColor.b) < 0.05f)
            {
                currentColor = targetColor;
            }
            if (liquidSprite != null)
                liquidSprite.color = currentColor;
            else
                Debug.Log("Add a liquid sprite and then try changing the color");

            //Changing the color of the liquid in the material of the skinned mesh renderer
            if (spriteMeshInstance != null)
            {
                //spriteMeshInstance.sharedMaterial.SetColor("_LiquidColor", phColorDictionary[Mathf.FloorToInt(phValue + 0.1f)]);
                spriteMeshInstance.sharedMaterial.SetColor("_LiquidColor", currentColor);
            }

            //TODO: Remove this logic from here and find another place for it
            if (GetComponent<PlayerMechanics>())
            {
                GetComponent<PlayerMechanics>().player.SetpHCanvasGraphic(phValue,currentColor);
            }
        }
    }

    public virtual void SetpH(float value)
    {
        phValue = value;
    }

    public float getpH()
    {
        return phValue;
    }



    #endregion
}
