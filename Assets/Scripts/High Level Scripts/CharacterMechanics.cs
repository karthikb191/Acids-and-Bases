using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMechanics : MonoBehaviour {

    protected SpriteRenderer characterSprite;
    public bool PhTransitionMechanic = true;

    [SerializeField]
    [Range(0, 14)]
    protected float phValue = 7;
    
    Color currentColor = Color.white;
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
        if (transform.Find("Sprite") != null)
        {
            characterSprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        }
        if (characterSprite != null)
            characterSprite.color = phColorDictionary[Mathf.FloorToInt(phValue + 0.1f)];
        else
            PhTransitionMechanic = false;
    }

    // Update is called once per frame
    protected void Update () {
        if (PhTransitionMechanic)
            CheckForPhChange();
	}

    #region pH Functions

    void CheckForPhChange()
    {
        float stepSize = 0.1f;
        if(currentColor != phColorDictionary[Mathf.FloorToInt(phValue)])
        {
            //Debug.Log("Trying to achieve the target color: " + phValue);
            //get the destination PH
            Color targetColor = phColorDictionary[Mathf.FloorToInt(phValue+0.1f)];

            //currentColor = (currentColor + targetColor) / 0.2f;
            //currentColor = currentColor + (targetColor - currentColor) * stepSize;
            currentColor = (1-stepSize)*currentColor + (targetColor) * stepSize;

            if(Mathf.Abs(currentColor.r - targetColor.r) < 0.05f &&
                Mathf.Abs(currentColor.g - targetColor.g) < 0.05f &&
                Mathf.Abs(currentColor.b - targetColor.b) < 0.05f)
            {
                currentColor = targetColor;
            }

            characterSprite.color = currentColor;
        }
        //This function later changes the pH meter also
    }

    public void SetpH(float value)
    {
        phValue = value;
    }

    public float getpH()
    {
        return phValue;
    }

    #endregion
}
